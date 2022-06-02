



-- =============================================
-- 描述:	修改CODE主檔資料
-- 記錄:	<2011/08/17><Dennis.Wen><新增本預存>
-- 記錄:	<2011/08/25><Mihsiu.Chiu><修改預存-新增欄位 fsTYPE>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_CODE]
	@fsCODE_ID		VARCHAR(10),
	@fsCODE			VARCHAR(20),
	@fsNAME			NVARCHAR(200),
	@fsENAME		VARCHAR(200) = '',
	@fnORDER		INT,
	@fsSET			VARCHAR(50) = '',
	@fsNOTE			NVARCHAR(200) = '',
	@fsIS_ENABLED	VARCHAR(1),
	@fsTYPE			VARCHAR(1),
	
	@fsUPDATED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE
			tbzCODE
		SET
			fsCODE_ID		= @fsCODE_ID,
			fsCODE			= @fsCODE,
			fsNAME			= @fsNAME,
			fsENAME			= @fsENAME,
			fnORDER			= @fnORDER,
			fsSET			= @fsSET,	
			fsNOTE			= @fsNOTE,	
			fsIS_ENABLED	= @fsIS_ENABLED,
			fsTYPE			= @fsTYPE,
			
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fsCODE_ID		= @fsCODE_ID)
		AND	(fsCODE			= @fsCODE)
		
		COMMIT

		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




