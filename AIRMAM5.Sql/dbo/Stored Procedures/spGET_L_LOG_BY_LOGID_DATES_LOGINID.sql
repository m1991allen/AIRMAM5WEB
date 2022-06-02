


-- =============================================
-- 描述:	依照CREATE日期或LOG ID 取出L_LOG主檔資料
-- 記錄:	<2016/09/05><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_L_LOG_BY_LOGID_DATES_LOGINID]
	@fnlLOG_ID	BIGINT,
	@fdSDATE	VARCHAR(10),
	@fdEDATE	VARCHAR(10),
	@fsLOGIN_ID VARCHAR(50)
	
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
		tblLOG.fnlLOG_ID, 
		tblLOG.fsTYPE, 
		tblLOG.fsGROUP,
		tblLOG.fsDESCRIPTION, 
		tblLOG.fsNOTE, 
		tblLOG.fsDATA_KEY,
		tblLOG.fdCREATED_DATE, 
		tblLOG.fsCREATED_BY,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		_sTYPE_NAME  = (CASE WHEN (tblLOG.fsTYPE = '') THEN '(未選擇)' 
							ELSE ISNULL(CODE.fsNAME, '錯誤代碼: '+ tblLOG.fsTYPE) END),
		_sGROUP_NAME = (CASE WHEN (tblLOG.fsGROUP = '') THEN '(未選擇)' 
							ELSE ISNULL(CODE1.fsNAME, '錯誤代碼: '+ tblLOG.fsTYPE) END)
	FROM
		tblLOG
			LEFT JOIN tbzCODE AS CODE ON (tblLOG.fsTYPE = CODE.fsCODE) AND CODE.fsCODE_ID = 'LOG001'		
			LEFT JOIN tbzCODE AS CODE1 ON (tblLOG.fsGROUP = CODE1.fsCODE) AND CODE1.fsCODE_ID = 'LOG002'		
			LEFT JOIN tbmUSERS USERS_CRT ON CODE.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
	WHERE			
		(@fnlLOG_ID = 0 OR tblLOG.fnlLOG_ID = @fnlLOG_ID) AND
		(@fdSDATE = '' OR CONVERT(VARCHAR(10),tblLOG.[fdCREATED_DATE] , 111) >= @fdSDATE) AND
		(@fdEDATE = '' OR CONVERT(VARCHAR(10),tblLOG.[fdCREATED_DATE] , 111) <= @fdEDATE) AND
		(@fsLOGIN_ID = '' OR tblLOG.fsCREATED_BY = @fsLOGIN_ID)
	ORDER BY
		tblLOG.fdCREATED_DATE DESC
END



