


-- =============================================
-- 描述:	新增L_WAIT_VOL主檔資料
-- 記錄:	<2013/02/20><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_L_WAIT_VOL]
	@fsVOL_ID		varchar(50),
	@fnWORK_ID		bigint,
	@fsSTATUS		varchar(2),
	@fsCREATED_BY	varchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tblWAIT_VOL
			(fsVOL_ID, fnWORK_ID, fsSTATUS, fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsVOL_ID, @fnWORK_ID, @fsSTATUS, GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




