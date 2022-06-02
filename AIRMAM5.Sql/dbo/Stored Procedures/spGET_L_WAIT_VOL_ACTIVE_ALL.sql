

-- =============================================
-- 描述:	新增ANNOUNCE主檔資料
-- 記錄:	<2013/02/20><Dennis.Wen><新增本預存>
-- 記錄:	<2013/03/06><Eric.Huang><修改本預存,order by VOL_ID>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_L_WAIT_VOL_ACTIVE_ALL]

AS
BEGIN
	SET NOCOUNT ON;

	SELECT
			VOL.fnWAIT_ID, VOL.fsVOL_ID, VOL.fnWORK_ID, VOL.fsSTATUS, 
			VOL.fdCREATED_DATE, VOL.fsCREATED_BY, VOL.fdUPDATED_DATE, VOL.fsUPDATED_BY,
			_sTATUS_NAME = CODE.fsNAME, _sBOOKING_REASON = WORK.fsNOTE, _sPRIORITY = fsPRIORITY
		FROM 
			tblWAIT_VOL AS VOL
		
			LEFT JOIN [tblWORK] WORK ON (WORK.fnWORK_ID = VOL.fnWORK_ID)
			LEFT JOIN [tbzCODE] CODE ON (CODE.fsCODE_ID = 'WAIT_VOL') AND (CODE.fsCODE = VOL.fsSTATUS)
			
		WHERE
			(VOL.fsSTATUS LIKE '0%') AND (WORK.fsSTATUS = '_T')

		ORDER BY VOL.fsVOL_ID
END



