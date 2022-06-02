



-- =============================================
-- 描述:	刪除BOOK_T	主檔資料
-- 記錄:	<2012/04/26><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_BOOK_T]
	@fnBOOK_T_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tbmBOOKING_T
		WHERE
			(fnBOOK_T_ID = @fnBOOK_T_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END






