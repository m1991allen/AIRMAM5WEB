


-- =============================================
-- 描述:	修改ANNOUNCE主檔資料
-- 記錄:	<2011/08/17><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_ANNOUNCE]

	@fnANN_ID			BIGINT ,
	@fsTITLE			NVARCHAR(50),
	@fsCONTENT			NVARCHAR(MAX),
	@fdSDATE			DATETIME ,
	@fdEDATE			DATETIME = NULL, 
	@fsTYPE				CHAR(1), 
	@fnORDER			INT,
	@fsGROUP_LIST		VARCHAR(500) = '',
	@fsIS_HIDDEN		CHAR(1),
	@fsDEPT				NVARCHAR(50),
	@fsNOTE				NVARCHAR(200),
	@fsUPDATED_BY		VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		UPDATE
			tbmANNOUNCE
		SET
			fsTITLE			= @fsTITLE ,
			fsCONTENT		= @fsCONTENT ,
			fdSDATE		    = @fdSDATE ,
			fdEDATE			= @fdEDATE ,
			fsTYPE			= @fsTYPE ,
			fnORDER			= @fnORDER ,
			fsGROUP_LIST	= @fsGROUP_LIST ,
			fsIS_HIDDEN		= @fsIS_HIDDEN ,
			fsDEPT          = @fsDEPT ,
			fsNOTE          = @fsNOTE,
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fnANN_ID = @fnANN_ID)
		
		COMMIT
			
		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




