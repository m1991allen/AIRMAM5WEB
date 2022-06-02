
-- =============================================
-- 描述:	新增ARC_SET主檔資料
-- 記錄:	<2011/11/14><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_ARC_SET]

	@fsTYPE			VARCHAR(1),
	@fsNAME			NVARCHAR(5),
	@fsTYPE_I		VARCHAR(1000),
	@fsTYPE_O		VARCHAR(500),
	@fsTYPE_S		VARCHAR(10),
	@fsCREATED_BY	VARCHAR(50)
	
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbmARC_SET
			(fsTYPE, fsNAME, fsTYPE_I, fsTYPE_O, fsTYPE_S, fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsTYPE, @fsNAME, @fsTYPE_I, @fsTYPE_O, @fsTYPE_S, GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



