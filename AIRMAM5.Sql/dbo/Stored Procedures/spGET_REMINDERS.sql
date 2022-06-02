﻿

-- =============================================
-- 描述:	取出REMINDERS主檔資料
-- 記錄:	<2011/08/17><Mihsiu.Chiu><新增本預存>
-- 記錄:	<2011/09/19><Mihsiu.Chiu><修改本預存>
-- 記錄:	<2016/09/13><David.Sin><修改本預存，把查詢功能統一在此SP>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_REMINDERS]
	@fnRMD_ID	BIGINT,
	@fsSTATUS VARCHAR(1),
	@fsTYPE   VARCHAR(1),
	@fdSDATE	VARCHAR(10),
	@fdEDATE	VARCHAR(10),
	@fsTO_UID varchar(20)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
		RMD.fnRMD_ID, 
		RMD.fsTITLE, 
		RMD.fsCONTENT, 
		RMD.fdDDATE, 
		RMD.fsTYPE, 
		RMD.fsTO_UID, 
		RMD.fsSTATUS, 
		RMD.fnORDER, 
		RMD.fsNOTE, 
		_sTYPENAME = (CASE WHEN (RMD.fsTYPE = '') THEN '(未選擇)' ELSE ISNULL(T.fsNAME, '錯誤代碼: '+RMD.fsTYPE) END), 
		_sSTATUSNAME = (CASE WHEN (RMD.fsSTATUS = '') THEN '(未選擇)' ELSE ISNULL(S.fsNAME, '錯誤代碼: '+RMD.fsSTATUS) END), 
		RMD.fsCHECKSTATUS, 
		RMD.fdCREATED_DATE, 
		RMD.fsCREATED_BY, 
		RMD.fdUPDATED_DATE, 
		RMD.fsUPDATED_BY,
		_sCheckStatusName = V.fsNAME,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME
	FROM
		tbmREMINDERS AS RMD
		
			LEFT JOIN tbzCODE AS T ON RMD.fsTYPE = T.fsCODE AND T.fsCODE_ID = 'RMD001' 
			LEFT JOIN tbzCODE AS S ON RMD.fsSTATUS = S.fsCODE AND S.fsCODE_ID = 'RMD002'
			LEFT JOIN tbzCODE AS V ON RMD.fsCHECKSTATUS = V.fsCODE	AND V.fsCODE_ID = 'LOGIN001'
			LEFT JOIN tbmUSERS USERS_CRT ON RMD.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
			LEFT JOIN tbmUSERS USERS_UPD ON RMD.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID		
	WHERE
		(@fnRMD_ID = 0 OR RMD.fnRMD_ID = @fnRMD_ID) AND
		(@fsSTATUS = '' OR RMD.fsSTATUS = @fsSTATUS) AND
		(@fsTYPE = '' OR RMD.fsTYPE = @fsTYPE) AND
		(@fdSDATE = '' OR CONVERT(VARCHAR(10), RMD.fdDDATE,111) >= @fdSDATE) AND
		(@fdEDATE = '' OR CONVERT(VARCHAR(10), RMD.fdDDATE,111) <= @fdEDATE) AND
		(@fsTO_UID = '' OR RMD.fsTO_UID = @fsTO_UID)
END


