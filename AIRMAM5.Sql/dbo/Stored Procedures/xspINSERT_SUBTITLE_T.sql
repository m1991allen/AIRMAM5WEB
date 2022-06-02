


-- =============================================
-- 描述:	新增SUBTITLE_T主檔資料
-- 記錄:	<2015/08/17><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_SUBTITLE_T]

	@fsNAME		NVARCHAR(50),
	@fsDESCRIPTION NVARCHAR(50),
	@fsCONTENT	NVARCHAR(1000),
	@fsCREATED_BY	NVARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbmSUBTITLE_T
			(fsNAME, fsDESCRIPTION, fsCONTENT, 
			 fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsNAME, @fsDESCRIPTION, @fsCONTENT, 
			 GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


