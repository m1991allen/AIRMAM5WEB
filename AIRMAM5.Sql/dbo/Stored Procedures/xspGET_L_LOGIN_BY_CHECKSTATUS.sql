


-- =============================================
-- 描述:	依照CHECKSTATUS及LOGIN_ID 取出L_LOGIN主檔資料
-- 記錄:	<2011/09/06><Eric.Huang><新增本預存>
------------<2011/09/08><Eric.Huang><加入判斷 fsNOTE like '登入失敗%'>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_L_LOGIN_BY_CHECKSTATUS]
	@STATUS VARCHAR(1),
	@LOGIN_ID NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY		
		SELECT 
			fnLOGIN_ID, fsLOGIN_ID, fdSTIME, fdETIME, tblLOGIN.fsNOTE, fsCheckStatus , 
			_sETIME = (CASE WHEN fdETIME='1900-01-01' THEN '未登出' ELSE CONVERT(VARCHAR(10), fdETIME, 111) +' '+CONVERT(VARCHAR(8), fdETIME, 114) END),
			_sOnlineTime = 
			(case when (DATEDIFF(HOUR, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end) >= 24 ) and (fdETIME='1900-01-01')
				  then '>' + CAST(DATEDIFF(D, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end)as VARCHAR) + 'D, 可能未正常登出'
				  when (DATEDIFF(HOUR, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end) < 24 ) and (fdETIME='1900-01-01')
				  then '00D:'+
					RIGHT(REPLICATE('0', 2) + CAST(DATEPART(HOUR,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'H:'+
					RIGHT(REPLICATE('0', 2) + CAST(DATEPART(MINUTE,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'m:'+
					RIGHT(REPLICATE('0', 2) + CAST(DATEPART(SECOND,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'S' 
				  else
					RIGHT(REPLICATE('0', 2) + CAST(DATEDIFF(D, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end) as VARCHAR) , 2)+'D:'+
					RIGHT(REPLICATE('0', 2) + CAST(DATEPART(HOUR,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'H:'+
					RIGHT(REPLICATE('0', 2) + CAST(DATEPART(MINUTE,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'m:'+
					RIGHT(REPLICATE('0', 2) + CAST(DATEPART(SECOND,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'S' 
				  end),	
			tblLOGIN.fdCREATED_DATE, tblLOGIN.fsCREATED_BY,
			_sCheckStatusName = CODE.fsNAME
		FROM
			tblLOGIN 
			
			JOIN tbzCODE  AS CODE ON (fsCheckStatus = CODE.fsCODE) AND (fsCODE_ID = 'LOGIN001') 
			
		WHERE
			(fsCheckStatus = @STATUS) AND (fsLOGIN_ID = @LOGIN_ID ) AND (tblLOGIN.fsNOTE like '登入失敗%')
			
			
		ORDER BY
			fdSTIME DESC
		
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



