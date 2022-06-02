


-- =============================================
-- 描述:	依照代碼群組 取出CODE主檔的t_USING_CNT資料供判斷是否可刪除
-- 記錄:	<2012/10/05><Eric.Huang><取得欄位t_USING_CNT>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_CODE_t_USING_CNT_BY_CODEID]
	@fsCODE_ID		VARCHAR(20)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			fsCODE_ID, fsCODE, fsNAME,t_USING_CNT = dbo.fn_t_GET_USING_CNT_BY_CODE(@fsCODE_ID, fsCODE)
		FROM
			tbzCODE AS CODE
		WHERE
			(fsCODE_ID		= @fsCODE_ID)
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




