

-- =============================================
-- 描述:	新增SYNONYMS主檔資料
-- 記錄:	<2012/05/31><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_SYNONYMS]
	@fsTEXT_LIST	NVARCHAR(4000),
	@fsTYPE			VARCHAR(10),
	@fsNOTE			NVARCHAR(MAX),
	@fsCREATED_BY	VARCHAR(50)
	
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		INSERT
			tbmSYNONYMS
			(fsTEXT_LIST, fsTYPE, fsNOTE, fdCREATED_DATE,fsCREATED_BY)
		VALUES
			(@fsTEXT_LIST, @fsTYPE, @fsNOTE, GETDATE() ,@fsCREATED_BY)
			
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


