﻿-- =============================================
-- 描述:	取出FUNCTIONS主檔全部資料
-- 記錄:	<2011/09/07><Mihsiu.Chiu><新增本預存>
--			<2012/02/15><Mihsiu.Chiu><新增欄位 fsHEADER fsTYPE_NAME fsIMAGE_URI fsCOMMON >
-- =============================================
CREATE PROCEDURE [dbo].[spGET_FUNCTIONS_ALL]	
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
			--_sParents = dbo.fnGET_FUNCTION_LEVEL(tbmFUNCTIONS.fsFUNC_ID)
		FROM
			tbmFUNCTIONS 
				LEFT JOIN tbmUSERS USERS_CRT ON tbmFUNCTIONS.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON tbmFUNCTIONS.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE 
			(fsTYPE <> '' OR fsPARENT_ID <> '')
		ORDER BY 
			--_sParents, 
			fsPARENT_ID, fsTYPE desc, fnORDER, fsFUNC_ID
END