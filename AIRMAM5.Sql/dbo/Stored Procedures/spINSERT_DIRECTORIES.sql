﻿

-- =============================================
-- 描述:	新增DIRECTORIES主檔資料
-- 記錄:	<2011/09/14><Eric.Huang><新增本預存>
--			<2012/04/05><Eric.Huang><修改預存 SELECT RESULT = @@IDENTITY 改為 select RESULT = IDENT_CURRENT('dbo.tbmDIRECTORIES') 
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_DIRECTORIES]
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
	@fsADMIN_GROUP		VARCHAR(MAX) = '',
	@fsADMIN_USER		VARCHAR(MAX) = '',
	@fsSHOWTYPE			CHAR(1) = 'P',
	@fsCREATED_BY		VARCHAR(50)


AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		INSERT
			tbmDIRECTORIES
			(fsNAME, fnPARENT_ID, fsDESCRIPTION, fsDIRTYPE, fnORDER,
			fnTEMP_ID_SUBJECT, fnTEMP_ID_VIDEO, fnTEMP_ID_AUDIO, fnTEMP_ID_PHOTO,
			fnTEMP_ID_DOC, fsADMIN_GROUP, fsADMIN_USER, fsSHOWTYPE,fdCREATED_DATE, fsCREATED_BY)
			
		VALUES
			(@fsNAME, @fnPARENT_ID, @fsDESCRIPTION, @fsDIRTYPE, @fnORDER,
			@fnTEMP_ID_SUBJECT, @fnTEMP_ID_VIDEO, @fnTEMP_ID_AUDIO, @fnTEMP_ID_PHOTO,
			@fnTEMP_ID_DOC, @fsADMIN_GROUP, @fsADMIN_USER, @fsSHOWTYPE,GETDATE(), @fsCREATED_BY)
			
		COMMIT
		
		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




