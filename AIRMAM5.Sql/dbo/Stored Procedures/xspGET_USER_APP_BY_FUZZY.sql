


-- =============================================
-- 描述:	依模糊搜尋取出USER_A主檔資料
--          (系統帳號/顯示名稱)
-- 記錄:	<2011/10/24><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_USER_APP_BY_FUZZY]

	@fsLOGIN_ID	NVARCHAR(50),
	@fsNAME	    NVARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

		SELECT 
			DISTINCT ISNULL(USER_A.fnUSER_A_ID, '錯誤'),USER_A.fnUSER_A_ID, fsLOGIN_ID, fsPASSWORD, USER_A.fsNAME, USER_A.fsENAME, fsTITLE, 
			fsDEPT_ID, fsEMAIL, fsPHONE, fsDESCRIPTION,
			_sDEPTNAME   = (CASE WHEN (fsDEPT_ID = '') THEN '(未選擇)' 
							ELSE ISNULL(DEPT001.fsNAME, '錯誤代碼: '+fsDEPT_ID) END),
			USER_A.fsSTATUS,	ISNULL(USER002.fsNAME, '錯誤') AS _sSTATUSNAME, 
			USER_A.fsNOTE,USER_A.fdCREATED_DATE, USER_A.fsCREATED_BY, USER_A.fdUPDATED_DATE, USER_A.fsUPDATED_BY			
		FROM
			tbmUSER_APP AS USER_A
			
			LEFT JOIN tbzCODE AS DEPT001 ON (DEPT001.fsCODE_ID = 'DEPT001') AND (DEPT001.fsCODE = USER_A.fsDEPT_ID)
			LEFT JOIN tbzCODE AS USER002 ON (USER002.fsCODE_ID = 'USER002') AND (USER002.fsCODE = USER_A.fsSTATUS)
			
		WHERE
		    ((fsLOGIN_ID	LIKE  '%' + @fsLOGIN_ID    + '%')  OR  (@fsLOGIN_ID	= ''))     AND
		    ((USER_A.fsNAME	LIKE  '%' + @fsNAME        + '%')  OR  (@fsNAME	    = ''))
		    		    
		ORDER BY fsSTATUS ASC ,fdCREATED_DATE	DESC
END



