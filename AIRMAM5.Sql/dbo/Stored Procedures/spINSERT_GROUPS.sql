


-- =============================================
-- 描述:	新增GROUPS主檔資料
-- 記錄:	<2011/08/19><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_GROUPS]
	@fsGROUP_ID		NVARCHAR(128) ,
	@fsNAME			NVARCHAR(50),
	@fsDESCRIPTION	NVARCHAR(MAX) = '',
	@fsTYPE			VARCHAR(1) = '',
	@fsCREATED_BY	VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		INSERT
			tbmGROUPS
			(fsGROUP_ID, fsNAME, fsDESCRIPTION, fsTYPE, fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsGROUP_ID, @fsNAME, @fsDESCRIPTION, @fsTYPE, GETDATE(), @fsCREATED_BY)

		COMMIT	
		
		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




