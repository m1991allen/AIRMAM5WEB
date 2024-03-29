﻿

-- =============================================
-- Author:		<Dennis.Wen>
-- Create date: <2013/01/22>
-- Description:	<取得某段日期內上傳的影音圖文清單>
-- =============================================
CREATE PROCEDURE [dbo].[xspRPT_STD_GET_ERRTRAN_LIST_BY_DATES_01]
	@SDATE		DATE,
	@EDATE		DATE,
	@QUERY_BY	NVARCHAR(50)
AS
BEGIN
	SELECT
		WRK.fsTYPE, WRK.fsSTATUS, WRK.fsPROGRESS, WRK.fdSTIME, WRK.fdETIME, WRK.fsRESULT, _sSTATUS_NAME = (CASE WHEN (SUBSTRING(WRK.fsSTATUS,1,1) IN ('0','1') ) THEN '轉檔時間過久' 
																										WHEN (WRK.fsTYPE = 'TRANSCODE') THEN CODE_TC.fsNAME
																										WHEN (WRK.fsTYPE = 'BOOKING') THEN CODE_BK.fsNAME
																										WHEN (WRK.fsTYPE = 'AVID') THEN CODE_EBC.fsNAME
																										WHEN (WRK.fsTYPE = 'NAS') THEN CODE_EBC.fsNAME
																										ELSE WRK.fsNOTE END),  WRK.fsNOTE,
	WRK.fdCREATED_DATE, CODE1.fsNAME, _ITEM_TYPE, _ITEM_ID, WRK.fnWORK_ID
	FROM
		tblWORK AS WRK
		LEFT JOIN [tbzCODE] AS CODE1 ON (CODE1.fsCODE_ID = 'WORK001') AND (CODE1.fsCODE = WRK.fsTYPE)
		LEFT JOIN [tbzCODE] AS CODE_BK ON (CODE_BK.fsCODE_ID = 'WORK_BK') AND (CODE_BK.fsCODE = WRK.fsSTATUS)
		LEFT JOIN [tbzCODE] AS CODE_TC ON (CODE_TC.fsCODE_ID = 'WORK_TC') AND (CODE_TC.fsCODE = WRK.fsSTATUS)
		LEFT JOIN [tbzCODE] AS CODE_EBC ON (CODE_EBC.fsCODE_ID = 'WORK_EBC') AND (CODE_EBC.fsCODE = WRK.fsSTATUS)
		
	WHERE	((CONVERT(VARCHAR(10), WRK.fdCREATED_DATE, 111) >= CONVERT(VARCHAR(10), @SDATE, 111)) AND
			(CONVERT(VARCHAR(10), WRK.fdCREATED_DATE, 111) <= CONVERT(VARCHAR(10), @EDATE, 111)) AND
			(fsSTATUS > 'A'))				
			--OR (fsSTATUS < '9' AND DATEDIFF(day, WRK.fdCREATED_DATE, GETDATE())>3)
	ORDER BY
		WRK.fsSTATUS, WRK.fdCREATED_DATE
END


