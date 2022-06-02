


-- =============================================
-- 描述:	修改SYNONYMS主檔資料
-- 記錄:	<2011/08/23><Mihsiu.Chiu><新增本預存>
--          <2011/09/05><Eric.Huang> <新增fsCheckStatus欄位>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_SYNONYMS]
	@fnINDEX_ID     BIGINT,
	@fsTEXT_LIST	NVARCHAR(450),
	@fsTYPE			VARCHAR(10),
	@fsNOTE			NVARCHAR(MAX),
	@fsUPDATED_BY	NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		UPDATE
			tbmSYNONYMS
		SET
			fsTEXT_LIST		= @fsTEXT_LIST, 
			fsTYPE	        = @fsTYPE, 
			fsNOTE		    = @fsNOTE,		
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fnINDEX_ID = @fnINDEX_ID)
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




