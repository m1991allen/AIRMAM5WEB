﻿

-- =============================================
-- 描述:	取出MATERIAL主檔資料
-- 記錄:	<2012/04/18><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_MATERIAL]
	@fnMATERIAL_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
		M.fnMATERIAL_ID, 
		M.fsMARKED_BY, 
		M.fsTYPE,
		M.fsFILE_NO, 
		CASE M.fsTYPE
			WHEN 'V' THEN V.fsTITLE
			WHEN 'A' THEN A.fsTITLE
			WHEN 'P' THEN P.fsTITLE
			WHEN 'D' THEN D.fsTITLE
		END AS fsTITLE,
		M.fsDESCRIPTION, 
		M.fsNOTE, 
		M.fsPARAMETER,
		M.fsCREATED_BY, 
		M.fdCREATED_DATE, 
		M.fsUPDATED_BY, 
		M.fdUPDATED_DATE,
		_sTYPE = (CASE WHEN (M.fsTYPE = '') THEN '(未選擇)' ELSE ISNULL(T.fsNAME, '錯誤代碼: '+ M.fsTYPE) END),
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME
	FROM
		tbmMATERIAL	AS M	
			LEFT JOIN tbzCODE T ON M.fsTYPE = T.fsCODE AND T.fsCODE_ID = 'MTRL001'
			LEFT JOIN tbmARC_VIDEO V ON M.fsTYPE = 'V' and M.fsFILE_NO = V.fsFILE_NO		
			LEFT JOIN tbmARC_AUDIO A ON M.fsTYPE = 'A' and M.fsFILE_NO = A.fsFILE_NO
			LEFT JOIN tbmARC_PHOTO P ON M.fsTYPE = 'P' and M.fsFILE_NO = P.fsFILE_NO
			LEFT JOIN tbmARC_DOC D ON M.fsTYPE = 'D' and M.fsFILE_NO = D.fsFILE_NO
			LEFT JOIN tbmUSERS USERS_CRT ON M.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
			LEFT JOIN tbmUSERS USERS_UPD ON M.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID					
	WHERE
		(M.fnMATERIAL_ID = @fnMATERIAL_ID)
	ORDER BY
		M.fnMATERIAL_ID DESC
END


