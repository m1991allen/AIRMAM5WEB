

-- =============================================
-- 描述:	修改FUNC_GROUP主檔資料-BY GROUP
-- 記錄:	<2012/06/29><Mihsiu.Chiu><新增預存>
-- 記錄:	<2019/09/05><David.Sin><拿掉parent_id>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_FUNC_GROUP]
	@fsFUNC_IDs		VARCHAR(1000),
	@fsGROUP_ID		NVARCHAR(128),
	@fsUPDATED_BY	NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		DELETE FROM tbmFUNC_GROUP WHERE fsGROUP_ID = @fsGROUP_ID

		INSERT tbmFUNC_GROUP (fsFUNC_ID, fsGROUP_ID, fdCREATED_DATE, fsCREATED_BY)
		SELECT COL1,@fsGROUP_ID,GETDATE(), @fsUPDATED_BY FROM dbo.fn_SLPIT(@fsFUNC_IDs,',')
			
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


