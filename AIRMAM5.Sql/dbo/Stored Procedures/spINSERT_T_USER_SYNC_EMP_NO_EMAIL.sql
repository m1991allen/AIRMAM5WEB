



-- =============================================
-- 描述:	新增T_USER_SYNC主檔資料
-- 記錄:	<2014/02/10><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_T_USER_SYNC_EMP_NO_EMAIL]
	@fsFTV_EMAIL	VARCHAR(50),
	@fsEMP_NO		VARCHAR(10)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbt_USER_SYNC
			(fsFTV_EMAIL, fsEMP_NO)
		VALUES
			(@fsFTV_EMAIL, @fsEMP_NO)

		SELECT RESULT = IDENT_CURRENT('tbt_USER_SYNC')
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





