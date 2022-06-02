

-- =============================================
-- 描述:	刪除CODE_SET主檔資料
-- 記錄:	<2011/08/25><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_CODE_SET]
	@fsCODE_ID		VARCHAR(10),
	@fsDELETED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		DECLARE @t_USING_CNT BIGINT
		
		SET @t_USING_CNT = (SELECT COUNT(1) FROM tbmTEMPLATE_FIELDS WHERE fsCODE_ID = @fsCODE_ID)
		
		IF (@t_USING_CNT > 0)
		BEGIN
			SELECT RESULT = 'ERROR:此代碼已經被使用,不得刪除'
		END
		ELSE
		BEGIN
			
			BEGIN TRANSACTION

			DECLARE @context_info VARBINARY(128)
			SET @context_info = CAST(@fsDELETED_BY AS VARBINARY(128))
			SET CONTEXT_INFO @context_info

			DELETE
				tbzCODE_SET
			WHERE
				(fsCODE_ID = @fsCODE_ID)
	
			COMMIT

			SELECT RESULT = ''
		END

		
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




