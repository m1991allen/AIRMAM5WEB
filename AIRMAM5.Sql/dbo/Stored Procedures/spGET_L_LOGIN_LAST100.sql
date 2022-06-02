

-- =============================================
-- 描述:	取出L_LOGIN主檔最新100筆資料
-- 記錄:	<2011/08/23><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_L_LOGIN_LAST100]	
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT TOP 100
		tblLOGIN.fnLOGIN_ID, 
		tblLOGIN.fsLOGIN_ID, 
		tblLOGIN.fdSTIME, 
		tblLOGIN.fdETIME, 
		tblLOGIN.fsNOTE , 
		_sETIME = (CASE WHEN fdETIME='1900-01-01' THEN '未登出' ELSE CONVERT(VARCHAR(10), fdETIME, 111) +' '+CONVERT(VARCHAR(8), fdETIME, 114) END),
		_sOnlineTime = '',
		--(case when (DATEDIFF(HOUR, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end) >= 24 ) and (fdETIME='1900-01-01')
		--	  then '>' + CAST(DATEDIFF(D, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end)as VARCHAR) + '天, 可能未正常登出'
		--	  when (DATEDIFF(HOUR, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end) < 24 ) and (fdETIME='1900-01-01')
		--	  then '00天:'+
		--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(HOUR,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'時:'+
		--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(MINUTE,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'分:'+
		--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(SECOND,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'秒' 
		--	  else
		--		RIGHT(REPLICATE('0', 2) + CAST(DATEDIFF(D, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end) as VARCHAR) , 2)+'天:'+
		--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(HOUR,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'時:'+
		--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(MINUTE,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'分:'+
		--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(SECOND,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'秒' 
		--	  end),	
		tblLOGIN.fdCREATED_DATE, 
		tblLOGIN.fsCREATED_BY, 
		tblLOGIN.fdUPDATED_DATE, 
		tblLOGIN.fsUPDATED_BY,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME
	FROM
		tblLOGIN 
			LEFT JOIN tbmUSERS USERS_CRT ON tblLOGIN.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
			LEFT JOIN tbmUSERS USERS_UPD ON tblLOGIN.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID	
	ORDER BY
		fdSTIME DESC
END


