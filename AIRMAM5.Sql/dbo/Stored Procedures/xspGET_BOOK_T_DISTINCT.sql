



-- =============================================
-- 描述:	取出distinct BOOK_T主檔資料
-- 記錄:	<2012/05/07><Mihsiu.Chiu><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_BOOK_T_DISTINCT]
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT
			distinct fsGROUP, fsTITLE
		FROM
			tbmBOOKING_T							
		ORDER BY 
			fsGROUP
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



