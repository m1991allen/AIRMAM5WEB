

-- =============================================
-- 描述:	刪除CONFIG主檔資料
-- 記錄:	<2011/11/29><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_CONFIG]
	@fsKEY		VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tbzCONFIG
		WHERE
			(fsKEY		= @fsKEY)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




