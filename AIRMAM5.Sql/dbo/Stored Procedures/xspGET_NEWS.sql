

-- =============================================
-- 描述:	取出NEWS主檔資料
-- 記錄:	<2011/08/09><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_NEWS]
	@fnNEWS_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			NEWS.fnNEWS_ID, NEWS.fsTITLE, NEWS.fsCONTENT, NEWS.fdDATE, NEWS.fsDEPT, NEWS.fsNOTE,
			NEWS.fdALTERD_DATE, NEWS.fsALTERD_BY, NEWS.fdUPDATED_DATE, NEWS.fsUPDATED_BY,
			_sDEPTNAME = (CASE WHEN (NEWS.fsDEPT = '') THEN '(未選擇)' 
							ELSE ISNULL(DEPT.fsNAME, '錯誤代碼: '+NEWS.fsDEPT) END)
		FROM
			tbmNEWS AS NEWS
			
			LEFT JOIN tbzDEPT AS DEPT
			ON (DEPT.fsDEPT_ID = NEWS.fsDEPT)
		WHERE
			(fnNEWS_ID = @fnNEWS_ID)
		ORDER BY
			[fnNEWS_ID] DESC
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



