


-- =============================================
-- 描述:	刪除ARC_CLK主檔資料
-- 記錄:	<2012/07/12><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_L_ARC_CLK]
	@fnARC_CLK_ID		BIGINT
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tblARC_CLK
		WHERE
			(fnARC_CLK_ID		= @fnARC_CLK_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





