

-- =============================================
-- 描述:	取出CODE主檔資料
-- 記錄:	<2011/08/09><Dennis.Wen><新增本預存>
-- 記錄:	<2011/08/25><Mihsiu.Chiu><修改本預存-新增欄位 fsTYPE>
--			<2012/05/02><Mihsiu.Chiu><新增欄位t_USING_CNT>
--			<2012/10/04><Eric.Huang><MARK欄位t_USING_CNT(因為動作執行太久)>
--			<2016/10/18><David.Sin><合併查詢>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_CODE]
	@fsCODE_ID		VARCHAR(20),
	@fsCODE			VARCHAR(50),
	@fsNAME			NVARCHAR(20)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			CODE.fsCODE_ID, 
			CODE.fsCODE, 
			CODE.fsNAME, 
			CODE.fsENAME, 
			CODE.fnORDER, 
			CODE.fsSET, 
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
			--t_USING_CNT = dbo.fn_t_GET_USING_CNT_BY_CODE(@fsCODE_ID, @fsCODE)
		FROM
			tbzCODE AS CODE
				LEFT JOIN tbmUSERS USERS_CRT ON CODE.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON CODE.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE
			(@fsCODE_ID = '' OR CODE.fsCODE_ID = @fsCODE_ID) AND	
			(@fsCODE = '' OR CODE.fsCODE = @fsCODE) AND
			(@fsNAME = '' OR CODE.fsNAME LIKE '%' + @fsNAME + '%')
		ORDER BY
			[fsCODE_ID], [fnORDER], [fsCODE]
END


