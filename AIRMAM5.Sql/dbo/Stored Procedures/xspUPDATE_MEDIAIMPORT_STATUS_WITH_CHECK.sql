
-- =============================================
-- 描述: 更新 MediaImport 任務的狀態，如果初始狀態非為預期，會無法更新狀態
-- 記錄: <2013/03/07><William.Chen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_MEDIAIMPORT_STATUS_WITH_CHECK]

	@fsFILE_PATH			nvarchar(MAX),
	@fsJOB_GUID				varchar(64),
	@fnJOB_STATUS_CURRENT	smallint,
	@fnJOB_STATUS_NEXT		smallint,
	@fsUPDATE_BY			varchar(64)

AS

BEGIN

	UPDATE
		tblMEDIAIMPORT
	SET
		fnJOB_STATUS = @fnJOB_STATUS_NEXT, 
		fdUPDATE_DATE = GETDATE(),
		fsUPDATE_BY = @fsUPDATE_BY
	WHERE
		fsJOB_GUID = @fsJOB_GUID AND 
		fsFILE_PATH = @fsFILE_PATH AND 
		fnJOB_STATUS = @fnJOB_STATUS_CURRENT

	SELECT @@ROWCOUNT

END
