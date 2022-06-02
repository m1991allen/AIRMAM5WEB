



-- =============================================
-- 描述:	修改GROUPS主檔資料
-- 記錄:	<2011/08/19><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_GROUPS]

	@fsGROUP_ID		NVARCHAR(128) ,
	@fsNAME			NVARCHAR(50),
	@fsDESCRIPTION	NVARCHAR(MAX) = '',
	@fsTYPE			VARCHAR(1) = '',
	@fsUPDATED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		UPDATE
			tbmGROUPS
		SET
			fsGROUP_ID		= @fsGROUP_ID,
			fsNAME			= @fsNAME, 
			fsDESCRIPTION	= @fsDESCRIPTION,
			fsTYPE			= @fsTYPE,
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fsGROUP_ID = @fsGROUP_ID)
		
		COMMIT

		SELECT RESULT = ''
		
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





