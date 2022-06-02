﻿

-- =============================================
-- 描述:	根據群組與目錄更新權限列表
-- 記錄:	<2011/11/03><David.Sin><新增本預存>
-- 記錄:	<2016/10/24><David.Sin><判斷是否已存在tbmDIR_GROUP>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_DIR_GROUP_BY_GROUP_DIR]
	@fnDIR_ID			BIGINT,
	@fsGROUP_ID			NVARCHAR(128),
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
	
	IF EXISTS(SELECT * FROM tbmDIR_GROUP WHERE fnDIR_ID = @fnDIR_ID AND fsGROUP_ID = @fsGROUP_ID)
	BEGIN
		
		BEGIN TRANSACTION

		UPDATE
			tbmDIR_GROUP
		SET
			fsLIMIT_SUBJECT=@fsLIMIT_SUBJECT,
			fsLIMIT_VIDEO=@fsLIMIT_VIDEO,
			fsLIMIT_AUDIO=@fsLIMIT_AUDIO,
			fsLIMIT_PHOTO=@fsLIMIT_PHOTO,
			fsLIMIT_DOC=@fsLIMIT_DOC,
			fdUPDATED_DATE=GETDATE(),
			fsUPDATED_BY=@fsUPDATED_BY
		WHERE
			fnDIR_ID=@fnDIR_ID AND fsGROUP_ID=@fsGROUP_ID

		COMMIT
			
	END	
	ELSE
	BEGIN
		
		BEGIN TRANSACTION

		INSERT INTO tbmDIR_GROUP
			(fnDIR_ID,fsGROUP_ID,fsLIMIT_SUBJECT,fsLIMIT_VIDEO,fsLIMIT_AUDIO,fsLIMIT_PHOTO,fsLIMIT_DOC,fdCREATED_DATE,fsCREATED_BY)
		VALUES
			(@fnDIR_ID,@fsGROUP_ID,@fsLIMIT_SUBJECT,@fsLIMIT_VIDEO,@fsLIMIT_AUDIO,@fsLIMIT_PHOTO,@fsLIMIT_DOC,GETDATE(),@fsUPDATED_BY)
		
		COMMIT
	END

	SELECT RESULT = @@ROWCOUNT

	END TRY
	
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
	
	
END


