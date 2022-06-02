


-- =============================================
-- 描述:	刪除DIRECTORIES主檔資料
-- 記錄:	<2011/09/14><Eric.Huang><新增本預存>
-- 記錄:	<2016/10/24><David.Sin><增加刪除目錄判斷>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_DIRECTORIES]
	@fnDIR_ID		BIGINT,
	@fsDELETED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY

		--檢查是否還有子節點
		IF(EXISTS(SELECT fnDIR_ID FROM tbmDIRECTORIES WHERE fnPARENT_ID = @fnDIR_ID))
		BEGIN
			
			SELECT RESULT = 'ERROR:此節點底下仍有子節點，請先刪除所有子節點!'

		END
		ELSE IF (EXISTS(SELECT * FROM tbmSUBJECT WHERE fnDIR_ID = @fnDIR_ID))
		BEGIN
			
			SELECT RESULT = 'ERROR:此節點有主題資料，請先刪除所有主題!'

		END
		ELSE
		BEGIN
			
			BEGIN TRANSACTION

			DECLARE @context_info VARBINARY(128)
			SET @context_info = CAST(@fsDELETED_BY AS VARBINARY(128))
			SET CONTEXT_INFO @context_info

			DELETE tbmDIR_GROUP WHERE (fnDIR_ID = @fnDIR_ID)
			DELETE tbmDIR_USER WHERE (fnDIR_ID = @fnDIR_ID)
			DELETE tbmDIRECTORIES WHERE (fnDIR_ID = @fnDIR_ID)

			COMMIT

			SELECT RESULT = ''
		END
		
			
		
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





