
-- =============================================
-- 描述:	修改CODE_SET主檔資料
-- 記錄:	<2011/08/25><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_CODE_SET]
	@fsCODE_ID	VARCHAR(10),
	@fsTITLE		NVARCHAR(50),
	@fsTBCOL		VARCHAR(MAX) = '',
	@fsNOTE		NVARCHAR(200) = '',
	@fsIS_ENABLED	VARCHAR(1),	
	@fsTYPE			VARCHAR(1),
	@fsUPDATED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		UPDATE
			tbzCODE_SET
		SET
			fsTITLE			= @fsTITLE,
			fsTBCOL			= @fsTBCOL,
			fsNOTE			= @fsNOTE,	
			fsIS_ENABLED	= @fsIS_ENABLED,
			fsTYPE			= @fsTYPE,
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fsCODE_ID		= @fsCODE_ID)
		
		COMMIT
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




