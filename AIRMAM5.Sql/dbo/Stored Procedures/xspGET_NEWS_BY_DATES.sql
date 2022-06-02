



-- =============================================
-- 描述:	依起迄日期取出NEWS主檔資料
-- 記錄:	<2011/08/09><Dennis.Wen><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_NEWS_BY_DATES]
	@SDATE	Date,
	@EDATE	Date
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		NEWS.fnNEWS_ID, NEWS.fsTITLE, NEWS.fsCONTENT, NEWS.fdDATE, NEWS.fsDEPT, NEWS.fsNOTE,
		NEWS.fdALTERD_DATE, NEWS.fsALTERD_BY, NEWS.fdUPDATED_DATE, NEWS.fsUPDATED_BY,
		_sDEPTNAME = (CASE WHEN (NEWS.fsDEPT = '') THEN '(未選擇)' ELSE ISNULL(DEPT.fsNAME, '錯誤代碼: '+NEWS.fsDEPT) END)
	FROM
		tbmNEWS AS NEWS
		
		LEFT JOIN tbzDEPT AS DEPT
		ON (DEPT.fsDEPT_ID = NEWS.fsDEPT)
	WHERE
		(fdDATE BETWEEN @SDate AND @EDate)
	ORDER BY
		[fnNEWS_ID] DESC
END





