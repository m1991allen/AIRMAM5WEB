﻿



-- =============================================
-- 描述:	新增DIR_GROUP主檔資料
-- 記錄:	<2011/09/14><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_DIR_GROUP]

	@fnDIR_ID			BIGINT,
	@fsGROUP_ID			NVARCHAR(128),
	@fsLIMIT_SUBJECT	VARCHAR(10),
	@fsLIMIT_VIDEO		VARCHAR(10),
	@fsLIMIT_AUDIO		VARCHAR(10),
	@fsLIMIT_PHOTO		VARCHAR(10),
	@fsLIMIT_DOC		VARCHAR(10),
	@fsCREATED_BY		VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		INSERT
			tbmDIR_GROUP
			(fnDIR_ID, fsGROUP_ID, fsLIMIT_SUBJECT, fsLIMIT_VIDEO, 
			 fsLIMIT_AUDIO, fsLIMIT_PHOTO, fsLIMIT_DOC, fdCREATED_DATE, fsCREATED_BY)
			
		VALUES
			(@fnDIR_ID, @fsGROUP_ID, @fsLIMIT_SUBJECT, @fsLIMIT_VIDEO, 
			 @fsLIMIT_AUDIO, @fsLIMIT_PHOTO, @fsLIMIT_DOC, GETDATE(), @fsCREATED_BY)
			
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




