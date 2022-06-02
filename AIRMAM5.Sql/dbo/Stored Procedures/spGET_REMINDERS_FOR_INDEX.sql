

-- =============================================
-- 描述:	取出REMINDERS給首頁顯示
-- 記錄:	<2019/05/24><David.Sin><修改本預存，把查詢功能統一在此SP>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_REMINDERS_FOR_INDEX]
	@fsTO_UID	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
		fnRMD_ID,
		fsTITLE, 
		fsCONTENT,
		fdDDATE,
		_sSTATUSNAME = (CASE WHEN (RMD.fsSTATUS = '') THEN '(未選擇)' ELSE ISNULL(S.fsNAME, '錯誤代碼: '+ RMD.fsSTATUS) END)
	FROM
		tbmREMINDERS AS RMD
			LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'RMD002')   AS S ON (RMD.fsSTATUS = S.fsCODE)		
	WHERE
		fsTO_UID = @fsTO_UID AND
		fdDDATE >= GETDATE() AND
		fsCHECKSTATUS = 'N'
	ORDER BY
		fdDDATE DESC
	

	--取出後狀態變更為已檢視
	UPDATE
		tbmREMINDERS
	SET
		[fsCHECKSTATUS] = 'V',
		[fdUPDATED_DATE] = GETDATE(),
		[fsUPDATED_BY] = @fsTO_UID
	WHERE
		fsTO_UID = @fsTO_UID AND
		fdDDATE >= GETDATE() AND
		fsCHECKSTATUS = 'N'


END


