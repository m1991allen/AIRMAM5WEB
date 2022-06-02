

-- =============================================
-- 描述:	依照代碼群組 取出CODE_SET主檔的t_USING_CNT資料供判斷是否可刪除
-- 記錄:	<2012/10/05><Eric.Huang><取得欄位t_USING_CNT>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_CODE_SET_t_USING_CNT_BY_TYPE]
	@fsTYPE			CHAR(1)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			S.fsCODE_ID, fsTITLE, t_USING_CNT = dbo.fn_t_GET_USING_CNT_BY_CODE_ID(S.fsCODE_ID)
		FROM
			tbzCODE_SET S
			LEFT JOIN 
			(SELECT tbzCODE_SET.fsCODE_ID, COUNT(FSCODE) _nCNT_CODE
			 FROM tbzCODE_SET LEFT JOIN tbzCODE ON tbzCODE_SET.fsCODE_ID = tbzCODE.fsCODE_ID
			 GROUP BY tbzCODE_SET.fsCODE_ID) C 
			ON S.fsCODE_ID = C.fsCODE_ID
		WHERE
			(fsTYPE = @fsTYPE)
		ORDER BY
			[fsCODE_ID]
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


