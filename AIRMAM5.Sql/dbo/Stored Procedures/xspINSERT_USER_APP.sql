

-- =============================================
-- 描述:	新增USER_APP主檔資料
-- 記錄:	<2011/10/18><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_USER_APP]

	@fsLOGIN_ID		NVARCHAR(50),
	@fsPASSWORD		VARCHAR(255),
	@fsNAME			NVARCHAR(50),
	@fsENAME		VARCHAR(50),
	@fsTITLE		NVARCHAR(50),
	@fsDEPT_ID		VARCHAR(10),
	@fsEMAIL		VARCHAR(50),
	@fsPHONE		VARCHAR(20),		
	@fsDESCRIPTION	NVARCHAR(MAX),
	@fsSTATUS       VARCHAR(1),
	@fsNOTE         NVARCHAR(20),	
	@fsCREATED_BY	NVARCHAR(50)
	
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbmUSER_APP
			(fsLOGIN_ID, fsPASSWORD, fsNAME, fsENAME, fsTITLE, fsDEPT_ID, 
			fsEMAIL, fsPHONE, fsDESCRIPTION, fsSTATUS, fsNOTE, fdCREATED_DATE, fsCREATED_BY)
			
		VALUES
			(@fsLOGIN_ID, @fsPASSWORD, @fsNAME, @fsENAME, @fsTITLE,	@fsDEPT_ID,
			 @fsEMAIL, @fsPHONE, @fsDESCRIPTION, @fsSTATUS, @fsNOTE, GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



