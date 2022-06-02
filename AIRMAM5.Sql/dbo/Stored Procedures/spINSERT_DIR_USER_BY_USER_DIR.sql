﻿

-- =============================================
-- 描述:	根據使用者與目錄新增權限列表
-- 記錄:	<2011/11/04><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_DIR_USER_BY_USER_DIR]
	@fnDIR_ID				BIGINT,
	@fsLOGIN_ID				VARCHAR(20),
	@fsLIMIT_SUBJECT		VARCHAR(10),
	@fsLIMIT_VIDEO			VARCHAR(10),
	@fsLIMIT_AUDIO			VARCHAR(10),
	@fsLIMIT_PHOTO			VARCHAR(10),
	@fsLIMIT_DOC			VARCHAR(10),
	@fsCREATED_BY			VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
	
		DECLARE @fsUSER_ID NVARCHAR(128) = (SELECT fsUSER_ID FROm tbmUSERS WHERE fsLOGIN_ID = @fsLOGIN_ID)

		BEGIN TRANSACTION

		INSERT INTO
			tbmDIR_USER(fnDIR_ID,fsUSER_ID,fsLIMIT_SUBJECT,fsLIMIT_VIDEO,fsLIMIT_AUDIO,
			fsLIMIT_PHOTO,fsLIMIT_DOC,fdCREATED_DATE,fsCREATED_BY)
		VALUES
			(@fnDIR_ID,@fsUSER_ID,@fsLIMIT_SUBJECT,@fsLIMIT_VIDEO,@fsLIMIT_AUDIO,
			@fsLIMIT_PHOTO,@fsLIMIT_DOC,GETDATE(),@fsCREATED_BY)
		
		COMMIT

		SELECT RESULT = ''
		
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

