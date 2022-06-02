

-- =============================================
-- 描述:	新增FUNCTIONS主檔資料
-- 記錄:	<2011/08/19><Mihsiu.Chiu><新增預存>
--			<2012/01/05><Mihsiu.Chiu><新增fsSET>
--			<2012/02/15><Mihsiu.Chiu><新增欄位 fsHEADER fsTYPE_NAME fsIMAGE_URI fsCOMMON >
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_FUNCTIONS]
	@fsFUNC_ID	VARCHAR(50),
	@fsNAME		NVARCHAR(50),
	@fsDESCRIPTION	NVARCHAR(50),
	@fsTYPE		VARCHAR(1),
	@fnORDER	INT,
	@fsICON	VARCHAR(50),
	@fsPARENT_ID	VARCHAR(50),
	@fsCREATED_BY	NVARCHAR(50),
	@fsHEADER NVARCHAR(20),
	@fsCONTROLLER varchar(100), 
	@fsACTION varchar(100)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbmFUNCTIONS
			(fsFUNC_ID, fsNAME, fsDESCRIPTION, fsTYPE, fnORDER, 
			 fsICON, fsPARENT_ID,fsCONTROLLER,
			 fsACTION,fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsFUNC_ID, @fsNAME, @fsDESCRIPTION, @fsTYPE, @fnORDER, 
			 @fsICON, @fsPARENT_ID,@fsCONTROLLER,
			 @fsACTION,GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

