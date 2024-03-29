﻿

-- =============================================
-- 描述:	依據日期有效、符合使用者群組並且不隱藏 取出ANNOUNCE主檔資料
-- 記錄:	<2012/02/29><Mihsiu.Chiu><新增本預存>
--      	<2012/03/03><Eric.Huang><加入：該使用者新增的資料>
--      	<2012/03/26><Mihsiu.Chiu><修改條件>
--      	<2016/09/06><David.Sin><參數型別更換>
--      	<2019/07/01><David.Sin><重寫>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ANNOUNCE_BY_EFFECTIVE_DATE_AND_USERGROUP_AND_NOT_HIDDEN]
	@fsLOGIN_ID	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
		*
	FROM
	(
		--沒有指定群組的
		SELECT
			A.fnANN_ID,
			A.fsTITLE,
			A.fsCONTENT,
			A.fdSDATE,
			A.fdEDATE,
			A.fsTYPE, 
			A.fnORDER,
			A.fsGROUP_LIST,
			A.fsIS_HIDDEN,
			A.fsDEPT,
			A.fsNOTE,
			A.fdCREATED_DATE,
			A.fsCREATED_BY,
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			A.fdUPDATED_DATE,
			A.fsUPDATED_BY,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
			CASE 
				WHEN A.fsTYPE = '' THEN '(未選擇)' 
				ELSE ISNULL(B.fsNAME, '錯誤代碼: '+ A.fsTYPE) 
			END AS fsTYPE_NAME,
			CASE 
				WHEN A.fsDEPT = '' THEN '(未選擇)' 
				ELSE ISNULL(D.fsNAME, '錯誤代碼: '+ A.fsDEPT) 
			END AS fsDEPT_NAME
		FROM
			tbmANNOUNCE A
				LEFT JOIN tbzCODE AS B ON A.fsTYPE = B.fsCODE AND B.fsCODE_ID = 'ANN001'
				LEFT JOIN tbzCODE AS D ON A.fsDEPT = D.fsCODE AND D.fsCODE_ID = 'DEPT001'
				LEFT JOIN tbmUSERS USERS_CRT ON A.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON A.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE
			A.fsIS_HIDDEN = 'N' AND
			A.fdSDATE <= GETDATE() AND
			(A.fdEDATE IS NULL OR A.fdEDATE >= GETDATE()) AND
			(A.fsGROUP_LIST = '')
		UNION
		--有指定給群組
		SELECT
			A.fnANN_ID,
			A.fsTITLE,
			A.fsCONTENT,
			A.fdSDATE,
			A.fdEDATE,
			A.fsTYPE, 
			A.fnORDER,
			A.fsGROUP_LIST,
			A.fsIS_HIDDEN,
			A.fsDEPT,
			A.fsNOTE,
			A.fdCREATED_DATE,
			A.fsCREATED_BY,
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			A.fdUPDATED_DATE,
			A.fsUPDATED_BY,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
			CASE 
				WHEN A.fsTYPE = '' THEN '(未選擇)' 
				ELSE ISNULL(B.fsNAME, '錯誤代碼: '+ A.fsTYPE) 
			END AS fsTYPE_NAME,
			CASE 
				WHEN A.fsDEPT = '' THEN '(未選擇)' 
				ELSE ISNULL(D.fsNAME, '錯誤代碼: '+ A.fsDEPT) 
			END AS fsDEPT_NAME
		FROM
			tbmANNOUNCE A 
				LEFT JOIN tbzCODE AS B ON A.fsTYPE = B.fsCODE AND B.fsCODE_ID = 'ANN001'
				LEFT JOIN tbzCODE AS D ON A.fsDEPT = D.fsCODE AND D.fsCODE_ID = 'DEPT001'
				LEFT JOIN tbmUSERS USERS_CRT ON A.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON A.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE
			A.fsIS_HIDDEN = 'N' AND
			A.fdSDATE <= GETDATE() AND
			(A.fdEDATE IS NULL OR A.fdEDATE >= GETDATE()) AND
			(
				(SELECT COUNT(1) FROM STRING_SPLIT(A.fsGROUP_LIST,';') TCOL1 JOIN
					(SELECT fsGROUP_ID FROM tbmUSER_GROUP WHERE fsUSER_ID = 
						(SELECT fsUSER_ID FROM tbmUSERS WHERE fsLOGIN_ID = @fsLOGIN_ID)) TCOL2 ON TCOL1.[value] = TCOL2.fsGROUP_ID
				) > 0
			)
		UNION
		--管理群組
		SELECT
			A.fnANN_ID,
			A.fsTITLE,
			A.fsCONTENT,
			A.fdSDATE,
			A.fdEDATE,
			A.fsTYPE, 
			A.fnORDER,
			A.fsGROUP_LIST,
			A.fsIS_HIDDEN,
			A.fsDEPT,
			A.fsNOTE,
			A.fdCREATED_DATE,
			A.fsCREATED_BY,
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			A.fdUPDATED_DATE,
			A.fsUPDATED_BY,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
			CASE 
				WHEN A.fsTYPE = '' THEN '(未選擇)' 
				ELSE ISNULL(B.fsNAME, '錯誤代碼: '+ A.fsTYPE) 
			END AS fsTYPE_NAME,
			CASE 
				WHEN A.fsDEPT = '' THEN '(未選擇)' 
				ELSE ISNULL(D.fsNAME, '錯誤代碼: '+ A.fsDEPT) 
			END AS fsDEPT_NAME
		FROM
			tbmANNOUNCE A 
				LEFT JOIN tbzCODE AS B ON A.fsTYPE = B.fsCODE AND B.fsCODE_ID = 'ANN001'
				LEFT JOIN tbzCODE AS D ON A.fsDEPT = D.fsCODE AND D.fsCODE_ID = 'DEPT001'
				LEFT JOIN tbmUSERS USERS_CRT ON A.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON A.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE
			A.fsIS_HIDDEN = 'N' AND
			A.fdSDATE <= GETDATE() AND
			(A.fdEDATE IS NULL OR A.fdEDATE >= GETDATE()) AND
			(
				(SELECT COUNT(1) FROM STRING_SPLIT((SELECT fsVALUE FROM tbzCONFIG WHERE fsKEY = 'ADMIN_GROUPS'),';') TCOL1 JOIN
					(SELECT fsGROUP_ID FROM tbmUSER_GROUP WHERE fsUSER_ID = 
						(SELECT fsUSER_ID FROM tbmUSERS WHERE fsLOGIN_ID = @fsLOGIN_ID)) TCOL2 ON TCOL1.[value] = TCOL2.fsGROUP_ID
				) > 0
			)
	) T
	ORDER BY
		T.fnORDER,T.fdCREATED_DATE DESC
END



