


-- =============================================
-- 描述:	修改CONFIG主檔資料
-- 記錄:	<2011/11/29><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_CONFIG]
	@fsKEY			VARCHAR(50),
	@fsVALUE		NVARCHAR(500),
	@fsTYPE			NVARCHAR(50),
	@fsDESCRIPTION	NVARCHAR(500),		
	@fsUPDATED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tbzCONFIG
		SET
			fsVALUE		= @fsVALUE,
			fsTYPE		= @fsTYPE,
			fsDESCRIPTION	= @fsDESCRIPTION,			
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fsKEY		= @fsKEY)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




