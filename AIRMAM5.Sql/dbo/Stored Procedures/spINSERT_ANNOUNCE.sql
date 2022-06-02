

-- =============================================
-- 描述:	新增ANNOUNCE主檔資料
-- 記錄:	<2011/08/17><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_ANNOUNCE]
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
	@fsCREATED_BY		VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION
		
		INSERT
			tbmANNOUNCE
			(fsTITLE, fsCONTENT, fdSDATE, fdEDATE, fsTYPE, fnORDER, fsGROUP_LIST, fsIS_HIDDEN,
			 fsDEPT, fsNOTE, fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsTITLE, @fsCONTENT, @fdSDATE, @fdEDATE, @fsTYPE, @fnORDER, @fsGROUP_LIST ,@fsIS_HIDDEN,
			 @fsDEPT, @fsNOTE, GETDATE(), @fsCREATED_BY)
		
		COMMIT
		
		SELECT RESULT = ''

	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



