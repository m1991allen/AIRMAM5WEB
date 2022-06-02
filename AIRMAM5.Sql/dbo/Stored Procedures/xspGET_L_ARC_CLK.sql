


-- =============================================
-- 描述:	取出ARC_CLK主檔資料
-- 記錄:	<2012/07/12><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_L_ARC_CLK]
	@fnARC_CLK_ID		BIGINT
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			fnARC_CLK_ID, fsTYPE, fsFILE_NO, fsSUBJECT_ID, fsFROM, 
			fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY
		FROM
			tblARC_CLK 
		WHERE
			(fnARC_CLK_ID		= @fnARC_CLK_ID)
		ORDER BY
			fnARC_CLK_ID
			
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



