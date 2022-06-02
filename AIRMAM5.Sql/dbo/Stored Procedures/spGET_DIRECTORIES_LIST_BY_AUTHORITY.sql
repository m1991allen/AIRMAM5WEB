





-- =============================================
-- 描述:	取出DIRECTORIES主檔資料
-- 記錄:	<2012/04/26><Dennis.Wen><新增本預存>
-- 記錄:	<2013/04/25><Eric.Huang><由Declare @xxx table 改為 create table #xxx , 速度變快很多!>
-- 記錄:	<2013/04/25><Eric.Huang><#tbResult 加入幾個INDEX>
-- 記錄:	<2013/09/10><Eric.Huang><#tbResult　@D_LIST => 5000>
-- 記錄:	<2015/12/09><David.Sin><改掉迴圈部分，用遞迴跑>
-- 記錄:	<2016/09/09><David.Sin><再精簡語法>
-- 記錄:	<2019/07/27><David.Sin><登入時直接寫入tbmUSER_DIR表>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_DIRECTORIES_LIST_BY_AUTHORITY]
	@fsLOGIN_ID	NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT fsDIR_LIST AS RESULT FROM USER_DIR WHERE fsLOGIN_ID = @fsLOGIN_ID
END






