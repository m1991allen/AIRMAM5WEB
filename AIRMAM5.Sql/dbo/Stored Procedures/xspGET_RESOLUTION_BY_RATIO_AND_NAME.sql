



-- =============================================
-- 描述:	依照 @fsRATIO 和 @fsNAME取出RESOLUTION主檔資料
-- 記錄:	<2012/07/30><Albert.Chen><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_RESOLUTION_BY_RATIO_AND_NAME]
	@fsRATIO	varchar(10),
	@fsNAME     varchar(20)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			fnRESOL_ID, fsRATIO, R.fsNAME, fsWIDTH, fsHEIGHT, fdCREATED_DATE, fsCREATED_BY, 
			fdUPDATED_DATE, fsUPDATED_BY,
			
			_sRATIO = (CASE WHEN (fsRATIO = '') THEN '(未選擇)' ELSE ISNULL(T.fsNAME, '錯誤代碼: '+fsRATIO) END)
		FROM
			tbzRESOLUTION	AS R		
			LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'RESOL003') AS T ON (fsRATIO = T.fsCODE)			
		WHERE (R.fsRATIO=@fsRATIO OR @fsRATIO='') AND
		      (R.fsNAME LIKE  '%' + @fsNAME    + '%' OR @fsNAME='')
		ORDER BY
			fsRATIO ,fsWIDTH
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


