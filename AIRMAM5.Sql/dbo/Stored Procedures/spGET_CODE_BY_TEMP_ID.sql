

-- =============================================
-- 描述:	依照TEMP_ID取出有用到的CODE主檔資料
-- 記錄:	<2011/10/05><Dennis.Wen><新增本預存>
--			<2012/05/03><Mihsiu.Chiu><新增欄位t_USING_CNT>
-- 記錄:	<2012/10/03><Dennis.Wen><改變呼叫fn_t_GET_USING_CNT_BY_CODE2,並多傳入@fsTABLE>
--			<2012/10/04><Eric.Huang><MARK欄位t_USING_CNT(因為動作執行太久)>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_CODE_BY_TEMP_ID]
	@TEMP_ID	int
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN
 		SET NOCOUNT ON;

		DECLARE @fsTABLE VARCHAR(1) = (SELECT TOP 1 fsTABLE FROM [tbmTEMPLATE] WHERE (fnTEMP_ID = @TEMP_ID))
		
			SELECT
				CODE.*
				--,t_USING_CNT = dbo.fn_t_GET_USING_CNT_BY_CODE2(CODE.fsCODE_ID, CODE.fsCODE, @fsTABLE, @TEMP_ID)
			FROM
				tbzCODE AS CODE
			WHERE
				CODE.fsCODE_ID IN (	SELECT
										DISTINCT fsCODE_ID
									FROM
										tbmTEMPLATE_FIELDS
									WHERE
										(fnTEMP_ID = @TEMP_ID) AND (fsCODE_ID <> ''))
			ORDER BY
				fsCODE_ID, fnORDER, fsCODE
	END
END



