

-- =============================================
-- 描述:	刪除FUNCTIONS主檔資料
-- 記錄:	<2011/08/19><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_FUNCTIONS]
	@fsFUNC_ID	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tbmFUNCTIONS
		WHERE
			(fsFUNC_ID = @fsFUNC_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




