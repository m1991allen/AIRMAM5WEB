﻿

-- =============================================
-- 描述:	取出FUNC_GROUP主檔資料
-- 記錄:	<2011/09/01><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_FUNC_GROUP]
	@fsGROUP_ID		NVARCHAR(128),
	@fsFUNC_ID		VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			tbmFUNC_GROUP.fsFUNC_ID,
			tbmFUNC_GROUP.fsGROUP_ID,
			tbmFUNC_GROUP.fdCREATED_DATE,
			tbmFUNC_GROUP.fsCREATED_BY,
			tbmFUNC_GROUP.fdUPDATED_DATE,
			tbmFUNC_GROUP.fsUPDATED_BY,
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
			tbmFUNCTIONS.fsNAME AS fsFUNCTION_NAME,
			tbmGROUPS.fsNAME AS fsGROUP_NAME
		FROM
			tbmFUNC_GROUP 
				JOIN tbmFUNCTIONS ON tbmFUNC_GROUP.fsFUNC_ID = tbmFUNCTIONS.fsFUNC_ID
				JOIN tbmGROUPS ON tbmFUNC_GROUP.fsGROUP_ID = tbmGROUPS.fsGROUP_ID
				LEFT JOIN tbmUSERS USERS_CRT ON tbmFUNC_GROUP.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON tbmFUNC_GROUP.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE
			(@fsGROUP_ID = '' OR tbmFUNC_GROUP.fsGROUP_ID = @fsGROUP_ID) AND
			(@fsFUNC_ID = '' OR tbmFUNC_GROUP.fsFUNC_ID = @fsFUNC_ID)
END


