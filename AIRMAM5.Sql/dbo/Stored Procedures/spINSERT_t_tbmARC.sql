


-- =============================================
-- 描述:	新增t_tbmARC_INDEX主檔資料
-- 記錄:	<2012/04/30><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_t_tbmARC]
	@fsFILE_NO			VARCHAR(16),
	@fsTYPE				VARCHAR(4),
	@fsREASON			NVARCHAR(50),
	@fsSTATUS			CHAR(1),	
	@fsCREATED_BY		VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			t_tbmARC_INDEX
			(fsFILE_NO, fsTYPE, fsREASON, fsSTATUS,
				fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsFILE_NO, @fsTYPE, @fsREASON, @fsSTATUS,
				GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



