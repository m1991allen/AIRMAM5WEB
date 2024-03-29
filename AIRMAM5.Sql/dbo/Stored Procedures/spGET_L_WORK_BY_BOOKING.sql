﻿
-- =============================================
-- 描述:	取出調用轉檔資料 
-- 記錄:	<2019/09/06><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_L_WORK_BY_BOOKING]
	@fnWORK_ID			BIGINT,
	@fsLOGIN_ID			NVARCHAR(128),
	@fdBEG_STIME		VARCHAR(10),
	@fdEND_STIME		VARCHAR(10),
	@fsSTATUS			VARCHAR(2)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT
		W.fnWORK_ID,
		B.fnBOOKING_ID,
		CASE _ARC_TYPE
			WHEN 'V' THEN V.fsTITLE
			WHEN 'A' THEN A.fsTITLE
			WHEN 'P' THEN P.fsTITLE
			WHEN 'D' THEN D.fsTITLE
		END AS fsTITLE,
		W._ARC_TYPE,
		CODE3.fsNAME AS _ARC_TYPE_NAME,
		CODE2.fsNAME AS fsTYPE_NAME,
		ISNULL(CODE1.fsNAME,'') AS fsSTATUS_NAME,
		W.fsPROGRESS,
		W.fsPRIORITY,
		W.fdSTIME,
		W.fdETIME,
		W.fsRESULT,
		W.fsNOTE,
		W.fsCREATED_BY, 
		W.fdCREATED_DATE, 
		W.fsUPDATED_BY, 
		W.fdUPDATED_DATE,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
		W._APPROVE_STATUS,
		W._APPROVE_DATE,
		W._APPROVE_BY,
		ISNULL(USERS_APP.fsNAME,'') AS _APPROVE_BY_NAME
	FROM
		tblWORK W
			JOIN tbmBOOKING B ON W.fnGROUP_ID = B.fnBOOKING_ID
			LEFT JOIN tbzCODE CODE1 ON W.fsSTATUS = CODE1.fsCODE AND CODE1.fsCODE_ID = 'WORK_BK'
			LEFT JOIN tbzCODE CODE2 ON W.fsTYPE = CODE2.fsCODE AND CODE2.fsCODE_ID = 'WORK001'
			LEFT JOIN tbzCODE CODE3 ON W._ARC_TYPE = CODE3.fsCODE AND CODE3.fsCODE_ID = 'MTRL001'
			LEFT JOIN tbmARC_VIDEO V ON _ARC_TYPE = 'V' AND W._ITEM_ID = V.fsFILE_NO
			LEFT JOIN tbmARC_AUDIO A ON _ARC_TYPE = 'A' AND W._ITEM_ID = A.fsFILE_NO
			LEFT JOIN tbmARC_PHOTO P ON _ARC_TYPE = 'P' AND W._ITEM_ID = P.fsFILE_NO
			LEFT JOIN tbmARC_DOC D ON _ARC_TYPE = 'D' AND W._ITEM_ID = D.fsFILE_NO
			LEFT JOIN tbmUSERS USERS_CRT ON W.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
			LEFT JOIN tbmUSERS USERS_UPD ON W.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
			LEFT JOIN tbmUSERS USERS_APP ON W._APPROVE_BY = USERS_APP.fsLOGIN_ID		 
	WHERE
		W.fsTYPE IN ('BOOKING','COPYFILE','NAS','AVID') AND
		B.fsCREATED_BY = @fsLOGIN_ID AND
		(@fnWORK_ID = 0 OR W.fnWORK_ID = @fnWORK_ID) AND
		(@fdBEG_STIME = '' OR CONVERT(VARCHAR(10),B.fdCREATED_DATE,111) >= @fdBEG_STIME) AND
		(@fdEND_STIME = '' OR CONVERT(VARCHAR(10),B.fdCREATED_DATE,111) <= @fdEND_STIME) AND
		(@fsSTATUS = '' OR W.fsSTATUS = @fsSTATUS)
END
