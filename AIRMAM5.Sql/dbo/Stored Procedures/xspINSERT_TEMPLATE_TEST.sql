CREATE PROCEDURE [dbo].[xspINSERT_TEMPLATE_TEST]
	
	@fsNAME				nvarchar(50),
	@fsTABLE			varchar(1),
	@fsDESCRIPTION		nvarchar(50),
	@fsCREATED_BY		nvarchar(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		if exists(select * from tbmTEMPLATE where fsNAME = @fsNAME)
		begin
			select RESULT = '名稱重複'+ CAST(@@IDENTITY AS VARCHAR(10))
		End
		else begin
		INSERT
			tbmTEMPLATE
			
			(fsNAME, fsTABLE, fsDESCRIPTION, fdCREATED_DATE, fsCREATED_BY)
			 
		VALUES
			(@fsNAME, @fsTABLE, @fsDESCRIPTION, GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
		End
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END
