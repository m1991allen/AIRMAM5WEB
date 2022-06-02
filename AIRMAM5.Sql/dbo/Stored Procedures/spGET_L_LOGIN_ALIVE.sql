
-- =============================================
-- 描述:	取得目前可能在線人數並修改狀態
-- 記錄:	<2019/09/06><Dennis.Wen><新增本預存> 
-- =============================================
CREATE PROCEDURE [dbo].[spGET_L_LOGIN_ALIVE]
	@fnLOGIN_ID BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SET NOCOUNT ON;

		UPDATE
			[tblLOGIN]
		SET
			fdETIME			= '1900/01/01',
			fsNOTE			= '操作系統中',
			fdUPDATED_DATE	= GETDATE()
		WHERE
			(fnLOGIN_ID = @fnLOGIN_ID) AND
			(DATEDIFF(DAY, fdCREATED_DATE, GETDATE()) = 0)
			
			
		--超過2分鐘未更新狀態的未登出者
		UPDATE
			[tblLOGIN]
		SET
			fdETIME			= GETDATE(),
			fsNOTE			= '未正常登出',
			fdUPDATED_DATE	= GETDATE()

		WHERE
			(fdETIME = '1900-01-01 00:00:00.000') AND						--未登出者
			(DATEDIFF(minute, CAST(fdUPDATED_DATE AS DATETIME), GETDATE()) > 2)	--超過2分鐘未更新狀態
			AND (fsNOTE = '' OR fsNOTE = '操作系統中' OR fsNOTE = '暫時未回應')


		UPDATE
			[tblLOGIN]
		SET
			fsNOTE			= '暫時未回應',
			fdUPDATED_DATE	= GETDATE()/*,
			fsUPDATED_BY	= '暫時未回應'*/
		WHERE
			(fdETIME = '1900-01-01 00:00:00.000') AND						--未登出者
			(DATEDIFF(minute, fdUPDATED_DATE, GETDATE()) >= 1) AND	--超過1分鐘未更新狀態
			(DATEDIFF(minute, fdUPDATED_DATE, GETDATE()) < 2)	--未超過2分鐘未更新狀態
			AND (fsNOTE = '' OR fsNOTE = '操作系統中')

			
		--回傳還有多少人Alive

		SELECT
			*,
			_sOnlineTime = ''
			--(case when (DATEDIFF(HOUR, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end) >= 24 ) and (fdETIME='1900-01-01')
			--	  then '>' + CAST(DATEDIFF(D, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end)as VARCHAR) + '天, 可能未正常登出'
			--	  when (DATEDIFF(HOUR, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end) < 24 ) and (fdETIME='1900-01-01')
			--	  then '0天:'+
			--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(HOUR,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'時:'+
			--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(MINUTE,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'分:'+
			--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(SECOND,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'秒' 
			--	  else
			--		RIGHT(REPLICATE('0', 2) + CAST(DATEDIFF(D, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end) as VARCHAR) , 2)+'天:'+
			--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(HOUR,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'時:'+
			--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(MINUTE,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'分:'+
			--		RIGHT(REPLICATE('0', 2) + CAST(DATEPART(SECOND,DATEADD(SS, DATEDIFF(SS, fdSTIME, case when fdETIME='1900-01-01' then GETDATE() else fdETIME end), 0)) as VARCHAR) , 2)+'秒' 
			--	  end)
		FROM
			[tblLOGIN]
		WHERE
			(fsNOTE = '操作系統中' OR fsNOTE = '暫時未回應')
		ORDER BY 
			fdSTIME DESC
		--SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


