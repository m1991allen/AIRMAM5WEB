



-- =============================================
-- 描述:	新增ARC_CLK主檔資料
-- 記錄:	<2012/07/12><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_L_ARC_CLK]
	@fsTYPE			VARCHAR(50),
	@fsFILE_NO		VARCHAR(16),
	@fsSUBJECT_ID	VARCHAR(12),
	@fsFROM			VARCHAR(10),
	@fsCREATED_BY	VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tblARC_CLK
			(fsTYPE, fsFILE_NO, fsSUBJECT_ID, fsFROM, fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsTYPE, @fsFILE_NO, @fsSUBJECT_ID, @fsFROM, GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




