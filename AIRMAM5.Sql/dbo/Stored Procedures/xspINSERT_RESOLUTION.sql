


-- =============================================
-- 描述:	新增RESOLUTION主檔資料
-- 記錄:	<2012/04/26><Eric.Huang><新增預存>
-- =============================================
create PROCEDURE [dbo].[xspINSERT_RESOLUTION]

	@fsRATIO			varchar	(10),
	@fsNAME				varchar	(20),
	@fsWIDTH			varchar	(10),
	@fsHEIGHT			varchar	(10),
	@fsCREATED_BY		nvarchar (50)
	
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbzRESOLUTION
			(fsRATIO, fsNAME, fsWIDTH, fsHEIGHT, fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsRATIO, @fsNAME, @fsWIDTH, @fsHEIGHT, GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



