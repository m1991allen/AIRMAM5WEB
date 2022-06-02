



-- =============================================
-- 描述:	刪除GROUPS主檔資料
-- 記錄:	<2011/08/19><Eric.Huang><新增本預存>
-- 記錄:	<2019/05/09><David.Sin><判斷群組是否還有使用者，且刪除時一併刪除其他相關資料表的群組>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_GROUPS]
	@fsGROUP_ID		NVARCHAR(128),
	@fsDELETED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		--查詢是否有使用者存在此群組中
		IF ((SELECT COUNT(1) FROM [dbo].[tbmUSER_GROUP] WHERE fsGROUP_ID = @fsGROUP_ID) = 0)
		BEGIN
			BEGIN TRANSACTION

			DECLARE @context_info VARBINARY(128)
			SET @context_info = CAST(@fsDELETED_BY AS VARBINARY(128))
			SET CONTEXT_INFO @context_info

			--刪除此群組在其他地方的紀錄
			DELETE FROM [dbo].[tbmDIR_GROUP] WHERE fsGROUP_ID = @fsGROUP_ID
			DELETE FROM [dbo].[tbmFUNC_GROUP] WHERE fsGROUP_ID = @fsGROUP_ID
			DELETE FROM [dbo].[tbmUSER_GROUP] WHERE fsGROUP_ID = @fsGROUP_ID

			DELETE FROM [dbo].[tbmGROUPS] WHERE fsGROUP_ID = @fsGROUP_ID
			
			COMMIT
			
			SELECT RESULT = ''
		END
		ELSE
		BEGIN
			SELECT RESULT = 'ERROR:此群組尚有使用者，不可刪除!'
		END
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END






