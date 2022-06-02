


-- =============================================
-- 描述:	取出USER_APP主檔資料
-- 記錄:	<2011/10/18><Eric.Huang><新增本預存>

-- =============================================
CREATE PROCEDURE [dbo].[xspGET_USER_APP]
	@fnUSER_A_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
	
		SELECT 
			fnUSER_A_ID, fsLOGIN_ID, fsPASSWORD, USER_A.fsNAME, fsENAME, fsTITLE, fsDEPT_ID,
			_sDEPTNAME   = (CASE WHEN (fsDEPT_ID = '') THEN '(未選擇)' 
							ELSE ISNULL(DEPT001.fsNAME, '錯誤代碼: '+fsDEPT_ID) END),
			ISNULL(USER002.fsNAME, '錯誤') AS _sSTATUSNAME, 
			fsEMAIL, fsPHONE, fsDESCRIPTION, fsSTATUS,fsNOTE,
			fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY
		FROM
			tbmUSER_APP AS USER_A
		
			LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'DEPT001') AS DEPT001 ON (USER_A.fsDEPT_ID = DEPT001.fsCODE)		
			LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'USER002') AS USER002 ON (USER_A.fsSTATUS = USER002.fsCODE)		
			
		WHERE
			(fnUSER_A_ID = @fnUSER_A_ID)
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



