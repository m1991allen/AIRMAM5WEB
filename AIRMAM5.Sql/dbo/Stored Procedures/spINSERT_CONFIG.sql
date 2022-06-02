


-- =============================================
-- 描述:	新增CONFIG主檔資料
-- 記錄:	<2011/11/29><Mihsiu.Chiu><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_CONFIG]
	@fsKEY			VARCHAR(50),
	@fsVALUE		NVARCHAR(500),
	@fsTYPE			NVARCHAR(50),
	@fsDESCRIPTION	NVARCHAR(500),	
	@fsCREATED_BY	VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbzCONFIG
			(fsKEY, fsVALUE, fsTYPE, fsDESCRIPTION, 
				fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsKEY, @fsVALUE, @fsTYPE, @fsDESCRIPTION, 
				GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



