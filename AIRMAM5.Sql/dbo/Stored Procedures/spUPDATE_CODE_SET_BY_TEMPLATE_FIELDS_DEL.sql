
-- =============================================
-- 描述:	修改CODE_SET的fsTBCOL資料
-- 記錄:	<2011/10/20><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_CODE_SET_BY_TEMPLATE_FIELDS_DEL]
	@fsCODE_ID	VARCHAR(10),
	@fsTBCOL		VARCHAR(MAX),
	@fsUPDATED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE
			tbzCODE_SET
		SET
			fsTBCOL			= REPLACE(fsTBCOL, @fsTBCOL,''),
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fsCODE_ID		= @fsCODE_ID)
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




