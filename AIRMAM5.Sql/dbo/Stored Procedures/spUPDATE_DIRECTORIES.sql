

-- =============================================
-- 描述:	修改DIRECTORIES主檔資料
-- 記錄:	<2011/09/14><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_DIRECTORIES]

	@fnDIR_ID			BIGINT,
	@fsNAME				NVARCHAR(50),
	@fnPARENT_ID		BIGINT,
	@fsDESCRIPTION		NVARCHAR(MAX) = '',
	@fsDIRTYPE			VARCHAR(1) = '',
	@fnORDER			INT,
	@fnTEMP_ID_SUBJECT	INT,
	@fnTEMP_ID_VIDEO	INT,
	@fnTEMP_ID_AUDIO	INT,
	@fnTEMP_ID_PHOTO	INT,
	@fnTEMP_ID_DOC		INT,
	@fsADMIN_GROUP		VARCHAR(MAX) = 'Administrators;',
	@fsADMIN_USER		VARCHAR(MAX) = '',
	@fsSHOWTYPE			CHAR(1) = 'P',
	@fsUPDATED_BY		VARCHAR(50)


AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		UPDATE
			tbmDIRECTORIES
		SET
			fsNAME				=	@fsNAME ,
			fnPARENT_ID			=	@fnPARENT_ID ,
			fsDESCRIPTION		=	@fsDESCRIPTION ,
			fsDIRTYPE			=	@fsDIRTYPE ,
			fnORDER				=	@fnORDER ,
			fnTEMP_ID_SUBJECT	=	@fnTEMP_ID_SUBJECT ,
			fnTEMP_ID_VIDEO		=	@fnTEMP_ID_VIDEO ,
			fnTEMP_ID_AUDIO		=	@fnTEMP_ID_AUDIO ,
			fnTEMP_ID_PHOTO		=	@fnTEMP_ID_PHOTO ,
			fnTEMP_ID_DOC		=	@fnTEMP_ID_DOC ,
			fsADMIN_GROUP		=	@fsADMIN_GROUP ,
			fsADMIN_USER		=	@fsADMIN_USER ,
			fsSHOWTYPE			=	@fsSHOWTYPE,
			fdUPDATED_DATE		=   GETDATE(),
			fsUPDATED_BY		=   @fsUPDATED_BY			
		WHERE
			(fnDIR_ID   = @fnDIR_ID  )
		
		COMMIT

		SELECT RESULT = ''

	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH

END







