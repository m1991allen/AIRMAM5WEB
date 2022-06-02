

-- =============================================
-- 描述:	取出NEWS主檔資料
-- 記錄:	<2012/01/20><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_DIR_PATH_BY_DIR_ID]
	@fnDIR_ID BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT RESULT = dbo.fnGET_DIR_PATH_BY_DIR_ID(@fnDIR_ID)
END



