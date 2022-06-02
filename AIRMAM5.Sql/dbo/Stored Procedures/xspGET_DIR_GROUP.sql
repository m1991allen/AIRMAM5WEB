



-- =============================================
-- 描述:	取出DIR_GROUP主檔資料
-- 記錄:	<2011/09/14><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_DIR_GROUP]

	@fnDIR_ID		BIGINT,
	@fsGROUP_ID		NVARCHAR(128)
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			fnDIR_ID, fsGROUP_ID, fsLIMIT_SUBJECT, fsLIMIT_VIDEO, fsLIMIT_AUDIO, fsLIMIT_PHOTO,
			fsLIMIT_DOC, fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY,
			_sDIR_PATH = dbo.fnGET_DIR_PATH_BY_DIR_ID(fnDIR_ID)

		FROM
			tbmDIR_GROUP
		WHERE
			(fnDIR_ID   = @fnDIR_ID) AND
			(fsGROUP_ID = @fsGROUP_ID)
			
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




