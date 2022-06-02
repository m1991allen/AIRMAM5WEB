

-- =============================================
-- 描述:	新增MATERIAL主檔資料
-- 記錄:	<2012/04/18><Mihsiu.Chiu><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_MATERIAL]
	@fsMARKED_BY		VARCHAR(50),
	@fsTYPE				CHAR(1),
	@fsFILE_NO			VARCHAR(16),
	@fsDESCRIPTION		NVARCHAR(50),
	@fsNOTE				NVARCHAR(100),
	@fsPARAMETER		VARCHAR(100),
	@fsCREATED_BY		VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		INSERT
			tbmMATERIAL
			(fsMARKED_BY, fsTYPE, fsFILE_NO,
			 fsDESCRIPTION, fsNOTE, fsPARAMETER,
			 fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsMARKED_BY, @fsTYPE, @fsFILE_NO,
			 @fsDESCRIPTION, @fsNOTE, @fsPARAMETER,			
			 GETDATE(), @fsCREATED_BY)
			
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


