

-- =============================================
-- 描述:	刪除MATERIAL主檔資料
-- 記錄:	<2012/05/10><Mihsiu.Chiu><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_MATERIAL_BY_MARKED_BY]
	@fsMARKED_BY	nvarchar(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		DELETE
			tbmMATERIAL
		WHERE
			(fsMARKED_BY = @fsMARKED_BY)
			
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




