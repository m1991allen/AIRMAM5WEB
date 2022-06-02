
-- =============================================
-- 描述:	取出CODE_SET主檔資料
-- 記錄:	<2011/10/20><Mihsiu.Chiu><新增本預存>
--			<2012/05/02><Mihsiu.Chiu><新增欄位t_USING_CNT>
--			<2012/10/04><Eric.Huang><MARK欄位t_USING_CNT(因為動作執行太久)>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_CODE_SET_IS_ENABLED]
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			CODE.fsCODE_ID, 
			CODE.fsTITLE, 
			CODE.fsTBCOL, 
			CODE.fsNOTE, 
			CODE.fsIS_ENABLED, 
			CODE.fsTYPE, 		
			_sIS_ENABLED = (case when fsIS_ENABLED = 'Y' then '是' else '' end),	
			CODE.fdCREATED_DATE, 
			CODE.fsCREATED_BY, 
			CODE.fdUPDATED_DATE, 
			CODE.fsUPDATED_BY,
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME
			--,t_USING_CNT = dbo.fn_t_GET_USING_CNT_BY_CODE_ID(fsCODE_ID)
		FROM
			tbzCODE_SET CODE
				LEFT JOIN tbmUSERS USERS_CRT ON CODE.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON CODE.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE
			(CODE.fsIS_ENABLED = 'Y')
		ORDER BY
			[fsCODE_ID]
END
