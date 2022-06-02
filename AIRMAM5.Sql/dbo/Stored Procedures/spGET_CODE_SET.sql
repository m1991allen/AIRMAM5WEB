
-- =============================================
-- 描述:	取出CODE_SET主檔資料
-- 記錄:	<2011/08/25><Mihsiu.Chiu><新增本預存>
--			<2012/05/02><Mihsiu.Chiu><新增欄位t_USING_CNT>
--			<2012/10/04><Eric.Huang><MARK欄位t_USING_CNT(因為動作執行太久)>
--			<2016/10/18><David.Sin><整合查詢CODE_SET>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_CODE_SET]
	@fsCODE_ID		VARCHAR(20),
	@fsTITLE		NVARCHAR(50),
	@fsTYPE			CHAR(1)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			S.fsCODE_ID, 
			S.fsTITLE, 
			S.fsTBCOL, 
			S.fsNOTE, 
			S.fsIS_ENABLED, 
			S.fsTYPE, 	
			_sIS_ENABLED = (case when S.fsIS_ENABLED = 'Y' then '是' else '' end),		
			S.fdCREATED_DATE, 
			S.fsCREATED_BY, 
			S.fdUPDATED_DATE, 
			S.fsUPDATED_BY,
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
			_nCNT_CODE
			--t_USING_CNT = dbo.fn_t_GET_USING_CNT_BY_CODE_ID(@fsCODE_ID)
		FROM
			tbzCODE_SET S
				LEFT JOIN 
					(SELECT tbzCODE_SET.fsCODE_ID, COUNT(FSCODE) _nCNT_CODE
					 FROM tbzCODE_SET LEFT JOIN tbzCODE ON tbzCODE_SET.fsCODE_ID = tbzCODE.fsCODE_ID
					 GROUP BY tbzCODE_SET.fsCODE_ID) C ON S.fsCODE_ID = C.fsCODE_ID
				LEFT JOIN tbmUSERS USERS_CRT ON S.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON S.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE
			(@fsTYPE = '' OR S.fsTYPE = @fsTYPE) AND
			(@fsCODE_ID = '' OR S.fsCODE_ID LIKE '%'+@fsCODE_ID+'%') AND
			(@fsTITLE = '' OR S.fsTITLE LIKE '%'+@fsTITLE+'%' )
		ORDER BY
			[fsCODE_ID]
END
