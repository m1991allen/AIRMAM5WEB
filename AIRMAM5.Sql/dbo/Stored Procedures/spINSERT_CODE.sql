


-- =============================================
-- 描述:	新增NEWS主檔資料
-- 記錄:	<2011/08/17><Dennis.Wen><新增預存>
-- 記錄:	<2011/08/25><Mihsiu.Chiu><修改預存-新增欄位 fsTYPE>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_CODE]
	@fsCODE_ID		VARCHAR(10),
	@fsCODE			VARCHAR(20),
	@fsNAME			NVARCHAR(200),
	@fsENAME		VARCHAR(200) = '',
	@fnORDER		INT,
	@fsSET			VARCHAR(50) = '',
	@fsNOTE			NVARCHAR(200) = '',
	@fsIS_ENABLED	VARCHAR(1),
	@fsTYPE			VARCHAR(1),
	
	@fsCREATED_BY	VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbzCODE
			(fsCODE_ID, fsCODE, fsNAME, fsENAME, fnORDER, fsSET, fsNOTE, fsIS_ENABLED, fsTYPE,
				fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsCODE_ID, @fsCODE, @fsNAME, @fsENAME, @fnORDER, @fsSET, @fsNOTE, @fsIS_ENABLED, @fsTYPE, 
				GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



