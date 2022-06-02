

-- =============================================
-- 描述:	刪除BOOKING主檔資料
-- 記錄:	<2012/04/18><Mihsiu.Chiu><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_BOOKING]
	@fnBOOKING_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tbmBOOKING
		WHERE
			(fnBOOKING_ID = @fnBOOKING_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




