

-- =============================================
-- 描述:	刪除t_tbmARC_INDEX主檔資料
-- 記錄:	<2012/04/30><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_t_tbmARC]
	@fnINDEX_ID		bigint
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			t_tbmARC_INDEX
		WHERE
			fnINDEX_ID		= @fnINDEX_ID
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




