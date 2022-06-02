

-- =============================================
-- 描述:	修改MATERIAL主檔資料
-- 記錄:	<2012/04/18><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_MATERIAL]
	@fnMATERIAL_ID		BIGINT,
	@fsMARKED_BY		VARCHAR(50),
	@fsTYPE				CHAR(1),	
	@fsFILE_NO			VARCHAR(16),
	@fsDESCRIPTION		NVARCHAR(50),
	@fsNOTE				NVARCHAR(100),
	@fsPARAMETER		VARCHAR(100),
	@fsUPDATED_BY		VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		UPDATE
			tbmMATERIAL
		SET
			fsMARKED_BY	    = @fsMARKED_BY, 
			fsTYPE			= @fsTYPE, 
			fsFILE_NO		= @fsFILE_NO,
			fdUPDATED_DATE	= GETDATE(),
			fsDESCRIPTION	= @fsDESCRIPTION,
			fsNOTE			= @fsNOTE,
			fsPARAMETER		= @fsPARAMETER,
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fnMATERIAL_ID = @fnMATERIAL_ID)
			
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



