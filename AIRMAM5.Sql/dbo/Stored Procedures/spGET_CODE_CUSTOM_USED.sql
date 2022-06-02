
-- =============================================
-- 描述:	檢查 自訂代碼是否被使用
-- 記錄:	<2012/04/05><Mihsiu.Chiu><新增本預存>
--      	<2012/08/30><Mihsiu.Chiu><修改@fsCODE_CTRL AS Varchar(20)>
--      	<2018/08/21><David.Sin><修改只要樣版有用到此自訂代碼，就不可以刪除子項>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_CODE_CUSTOM_USED]
	@fsCODE_ID  VARCHAR(20),		--A
	@fsCODE		VARCHAR(50)			--S
AS
BEGIN
 	SET NOCOUNT ON;

	IF ((SELECT COUNT(1) FROM tbmTEMPLATE_FIELDS WHERE fsCODE_ID = @fsCODE_ID) > 0)
		BEGIN
			SELECT 'Y' AS RESULT
		END
		ELSE
		BEGIN
			SELECT 'N' AS RESULT
		END
END
