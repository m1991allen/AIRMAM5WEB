﻿




-- =============================================
-- 描述:	取出DIR_USER主檔資料
-- 記錄:	<2011/10/17><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_DIR_USER_BY_USER]

	@fsLOGIN_ID		NVARCHAR(256)
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 

			fnDIR_ID, fsLOGIN_ID, fsLIMIT_SUBJECT, fsLIMIT_VIDEO, fsLIMIT_AUDIO, fsLIMIT_PHOTO,
			fsLIMIT_DOC, fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY,
			_sDIR_PATH = dbo.fnGET_DIR_PATH_BY_DIR_ID(fnDIR_ID)

		FROM
			tbmDIR_USER
		WHERE
			fsLOGIN_ID  = @fsLOGIN_ID
			
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





