



-- =============================================
-- 描述:	刪除ARC_SET主檔資料
-- 記錄:	<2011/11/14><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_ARC_SET]
	@fsTYPE		varchar(1)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tbmARC_SET
		WHERE
			(fsTYPE = @fsTYPE)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


