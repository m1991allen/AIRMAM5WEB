﻿

-- =============================================
-- 描述:	依照fsPARENT_ID取出FUNCTIONS主檔資料
-- 記錄:	<2012/02/16><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_FUNCTIONS_BY_PARENT_ID]
	@fsPARENT_ID	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			tbmFUNCTIONS.fsFUNC_ID, 
			tbmFUNCTIONS.fsNAME, 
			tbmFUNCTIONS.fsDESCRIPTION, 
			tbmFUNCTIONS.fsTYPE, 
			tbmFUNCTIONS.fnORDER, 
			tbmFUNCTIONS.fsICON, 
			tbmFUNCTIONS.fsPARENT_ID, 
			tbmFUNCTIONS.fsHEADER, 
			tbmFUNCTIONS.fsCONTROLLER,
			tbmFUNCTIONS.fsACTION,
			tbmFUNCTIONS.fsIS_MULTI_SHEET,
			tbmFUNCTIONS.fdCREATED_DATE, 
			tbmFUNCTIONS.fsCREATED_BY, 
			tbmFUNCTIONS.fdUPDATED_DATE, 
			tbmFUNCTIONS.fsUPDATED_BY,
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME
		FROM
			tbmFUNCTIONS
				LEFT JOIN tbmUSERS USERS_CRT ON tbmFUNCTIONS.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON tbmFUNCTIONS.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID	
		WHERE
			(fsPARENT_ID = @fsPARENT_ID)
		ORDER BY 
			fnORDER 
END


