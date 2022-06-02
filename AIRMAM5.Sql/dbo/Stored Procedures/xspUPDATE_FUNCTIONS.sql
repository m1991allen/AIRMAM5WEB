

-- =============================================
-- 描述:	修改FUNCTIONS主檔資料
-- 記錄:	<2011/08/19><Mihsiu.Chiu><修改預存>
--			<2012/01/05><Mihsiu.Chiu><新增fsSET>
--			<2012/02/15><Mihsiu.Chiu><新增欄位 fsHEADER fsTYPE_NAME fsIMAGE_URI fsCOMMON >
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_FUNCTIONS]
	@fsFUNC_ID	VARCHAR(50),
	@fsNAME		NVARCHAR(50),
	@fsDESCRIPTION	NVARCHAR(50),
	@fsTYPE		VARCHAR(1),
	@fnORDER	INT,
	@fsICON	VARCHAR(50),
	@fsPARENT_ID	VARCHAR(50),
	@fsUPDATED_BY	NVARCHAR(50),
	@fsHEADER NVARCHAR(20), 
	@fsCONTROLLER varchar(100), 
	@fsACTION varchar(100) 
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tbmFUNCTIONS
		SET			
			fsNAME = @fsNAME,
			fsDESCRIPTION = @fsDESCRIPTION,
			fsTYPE = @fsTYPE, 
			fnORDER = @fnORDER,
			fsICON = @fsICON,
			fsPARENT_ID = @fsPARENT_ID,
			fdUPDATED_DATE = GETDATE(), 
			fsUPDATED_BY = @fsUPDATED_BY,
			fsHEADER = @fsHEADER,
			fsCONTROLLER = @fsCONTROLLER,
			fsACTION = @fsACTION
		WHERE 
			(fsFUNC_ID = @fsFUNC_ID)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


