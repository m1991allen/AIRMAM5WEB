



-- =============================================
-- 描述:	新增SRH主檔資料
-- 記錄:	<2011/12/13><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_L_SRH]
	@fsSTATEMENT	NVARCHAR(100),
	@fsCREATED_BY	VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tblSRH
			(fsSTATEMENT, fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsSTATEMENT, GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




