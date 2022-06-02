


-- =============================================
-- 描述:	新增我的最愛資料
-- 記錄:	<2016/10/03><David.Sin><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_USERS_FAVORITE]
	@fsLOGIN_ID		VARCHAR(50),
	@fsFAVORITE		VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		INSERT INTO [dbo].[tbmUSER_FAVORITE]
		VALUES(@fsLOGIN_ID,@fsFAVORITE)

		COMMIT

		SELECT RESULT = ''

	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



