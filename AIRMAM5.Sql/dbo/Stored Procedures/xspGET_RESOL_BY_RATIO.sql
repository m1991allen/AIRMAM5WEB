



-- =============================================
-- 描述:	取出RESOLUTION主檔資料
-- 記錄:	<2012/04/26><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_RESOL_BY_RATIO]
	@fsRATIO	varchar(10)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		IF (@fsRATIO = '')
		BEGIN
			SELECT 
				fnRESOL_ID, fsRATIO, R.fsNAME, fsWIDTH, fsHEIGHT, fdCREATED_DATE, fsCREATED_BY, 
				fdUPDATED_DATE, fsUPDATED_BY,
				
				_sRATIO = (CASE WHEN (fsRATIO = '') THEN '(未選擇)' ELSE ISNULL(T.fsNAME, '錯誤代碼: '+fsRATIO) END)
			FROM
				tbzRESOLUTION	AS R		
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'RESOL003') AS T ON (fsRATIO = T.fsCODE)			
				
			ORDER BY
				fsWIDTH, fsHEIGHT
		END		
		ELSE
		BEGIN
			SELECT 
				fnRESOL_ID, fsRATIO, R.fsNAME, fsWIDTH, fsHEIGHT, fdCREATED_DATE, fsCREATED_BY, 
				fdUPDATED_DATE, fsUPDATED_BY,
				
				_sRATIO = (CASE WHEN (fsRATIO = '') THEN '(未選擇)' ELSE ISNULL(T.fsNAME, '錯誤代碼: '+fsRATIO) END)
			FROM
				tbzRESOLUTION	AS R		
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'RESOL003') AS T ON (fsRATIO = T.fsCODE)			
				
			WHERE
				(fsRATIO = @fsRATIO)
			ORDER BY
				fsWIDTH, fsHEIGHT
		END
						
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


