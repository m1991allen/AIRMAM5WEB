

-- =============================================
-- 描述:	刪除MATERIAL主檔資料
-- 記錄:	<2012/04/18><Mihsiu.Chiu><新增預存>
-- 記錄:	<2019/09/11><David.Sin><改為可批次刪除>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_MATERIAL]
	@fnMATERIAL_IDs		VARCHAR(MAX)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		DELETE
			tbmMATERIAL
		WHERE
			fnMATERIAL_ID IN (SELECT CONVERT(BIGINT,COL1) FROM dbo.fn_SLPIT(@fnMATERIAL_IDs,','))
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




