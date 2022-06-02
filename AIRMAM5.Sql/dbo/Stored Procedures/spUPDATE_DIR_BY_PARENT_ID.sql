



-- =============================================
-- 描述:	修改DIR_fnPARENT_ID主檔資料
-- 記錄:	<2013/01/02><Albert.Chen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_DIR_BY_PARENT_ID]

    @fnDIR_ID           bigint,
	@fnPARENT_ID    	bigint,
	@fsUPDATED_BY		nvarchar(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		UPDATE
			tbmDIRECTORIES
		SET
			fnPARENT_ID		=	@fnPARENT_ID,
			fdUPDATED_DATE	=   GETDATE(),
			fsUPDATED_BY	=   @fsUPDATED_BY
		WHERE
			fnDIR_ID   = @fnDIR_ID
		
		COMMIT

		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END






