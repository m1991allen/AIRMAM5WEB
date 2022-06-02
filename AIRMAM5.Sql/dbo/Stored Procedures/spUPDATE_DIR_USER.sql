



-- =============================================
-- 描述:	修改DIR_USER主檔資料
-- 記錄:	<2011/09/14><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_DIR_USER]

	@fnDIR_ID			BIGINT,
	@fsLOGIN_ID			NVARCHAR(256),
	@fsLIMIT_SUBJECT	VARCHAR(10),
	@fsLIMIT_VIDEO		VARCHAR(10),
	@fsLIMIT_AUDIO		VARCHAR(10),
	@fsLIMIT_PHOTO		VARCHAR(10),
	@fsLIMIT_DOC		VARCHAR(10),
	@fsUPDATED_BY		VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		UPDATE
			tbmDIR_USER
		SET
			fnDIR_ID		=	@fnDIR_ID,
			fsLOGIN_ID		=   @fsLOGIN_ID,
			fsLIMIT_SUBJECT =   @fsLIMIT_SUBJECT,
			fsLIMIT_VIDEO	=   @fsLIMIT_VIDEO,
		    fsLIMIT_AUDIO	=   @fsLIMIT_AUDIO,
		    fsLIMIT_PHOTO	=   @fsLIMIT_PHOTO,
		    fsLIMIT_DOC		=   @fsLIMIT_DOC,
			fdUPDATED_DATE	=   GETDATE(),
			fsUPDATED_BY	=   @fsUPDATED_BY
		WHERE
			(fnDIR_ID  = @fnDIR_ID) AND
			(fsLOGIN_ID = @fsLOGIN_ID)
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END






