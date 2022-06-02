


-- =============================================
-- 描述:	新增L_SRH主檔資料
-- 記錄:	<2019/09/12><David.Sin><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_L_SRH]
	@fsSTATEMENT	NVARCHAR(MAX),
	@fsCREATED_BY	VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION
		
		INSERT
			[dbo].[tblSRH]
			([fsSTATEMENT],fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsSTATEMENT,GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = ''

		COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


