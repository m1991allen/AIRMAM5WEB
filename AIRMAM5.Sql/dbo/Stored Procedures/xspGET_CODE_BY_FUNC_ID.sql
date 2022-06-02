

-- =============================================
-- 描述:	依照FUNC_ID取出有用到的CODE主檔資料
-- 記錄:	<2011/08/09><Dennis.Wen><新增本預存>
-- 記錄:	<2011/08/25><Mihsiu.Chiu><修改本預存-新增欄位 fsTYPE>
--			<2012/05/02><Mihsiu.Chiu><新增欄位t_USING_CNT>
--			<2012/10/04><Eric.Huang><MARK欄位t_USING_CNT(因為動作執行太久)>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_CODE_BY_FUNC_ID]
	@fsFUNC_ID	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN
 		SET NOCOUNT ON;

		BEGIN TRY
		
			SELECT 
				CODE.fsCODE_ID, CODE.fsCODE, CODE.fsNAME, CODE.fsENAME, CODE.fnORDER, CODE.fsSET, CODE.fsNOTE, CODE.fsIS_ENABLED, CODE.fsTYPE, 
				CODE.fdCREATED_DATE, CODE.fsCREATED_BY, CODE.fdUPDATED_DATE, CODE.fsUPDATED_BY
				--t_USING_CNT = dbo.fn_t_GET_USING_CNT_BY_CODE(CODE.fsCODE_ID, CODE.fsCODE)
			FROM
				tbmFUNCTIONS AS FUNC
				
				LEFT JOIN tbzCODE AS CODE ON (FUNC.fsCODE_LIST LIKE ('%'+CODE.fsCODE_ID+';%'))
			WHERE
				(FUNC.fsFUNC_ID	= @fsFUNC_ID)
			ORDER BY
				CODE.fsCODE_ID, CODE.fnORDER, CODE.fsCODE
		END TRY
		BEGIN CATCH
			SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
		END CATCH
	END
END


