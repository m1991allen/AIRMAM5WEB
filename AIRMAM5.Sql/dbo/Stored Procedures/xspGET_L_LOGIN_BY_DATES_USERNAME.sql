

-- =============================================
-- 描述:	依照登入日期或登入者 取出L_LOGIN主檔資料
-- 記錄:	<2011/08/23><Mihsiu.Chiu><新增本預存>
--			<2012/10/04><Dennis.Wen><抓結果前先把太久未更新狀態的改為離線> 
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_L_LOGIN_BY_DATES_USERNAME]
	@SDATE	Date,
	@EDATE	Date, 
	@fsLOGIN_ID NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY		
			--超過2分鐘未更新狀態的未登出者
			
			UPDATE
				[tblLOGIN]
			SET
				fsNOTE			= '暫時未回應',
				fdUPDATED_DATE	= GETDATE()/*,
				fsUPDATED_BY	= '暫時未回應'*/
			WHERE
				(fdETIME = '1900/01/01') AND						--未登出者
				(DATEDIFF(minute, fdUPDATED_DATE, GETDATE()) >= 1) AND	--超過1分鐘未更新狀態
				(DATEDIFF(minute, fdUPDATED_DATE, GETDATE()) < 2)	--未超過2分鐘未更新狀態
				AND (fsNOTE IN ('','操作系統中'))
			
			UPDATE
				[tblLOGIN]
			SET
				fdETIME			= GETDATE(),
				fsNOTE			= '未正常登出',
				fdUPDATED_DATE	= GETDATE()/*,
				fsUPDATED_BY	= '未正常登出'*/
			WHERE
				(fdETIME = '1900/01/01') AND						--未登出者
				(DATEDIFF(minute, fdUPDATED_DATE, GETDATE()) >= 2)	--超過2分鐘未更新狀態
			AND (fsNOTE IN ('','操作系統中', '暫時未回應'))
		
		SELECT 
			fnLOGIN_ID, fsLOGIN_ID, fdSTIME, fdETIME, fsNOTE , 
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
			fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY
		FROM
			tblLOGIN 
		WHERE
			((@fsLOGIN_ID <> '') AND (fdSTIME BETWEEN @SDate AND DATEADD(day,1,@EDate) AND (fsLOGIN_ID = @fsLOGIN_ID )))
			OR
			((@fsLOGIN_ID = '') AND (fdSTIME BETWEEN @SDate AND DATEADD(day,1,@EDate)))	
		ORDER BY
			fdSTIME DESC
		
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


