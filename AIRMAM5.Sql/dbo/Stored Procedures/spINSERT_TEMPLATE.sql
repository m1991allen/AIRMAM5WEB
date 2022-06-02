



-- =============================================
-- 描述:	新增 TEMPLATE 主檔 資料
-- 記錄:	<2011/09/16><Eric.Huang><新增本預存>
-- 記錄:	<2011/10/13><Mihsiu.Chiu><修改本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_TEMPLATE]
	
	@fsNAME				NVARCHAR(50),
	@fsTABLE			CHAR(1),
	@fsDESCRIPTION		NVARCHAR(50),
	@fcIS_SEARCH		CHAR(1),
	@fsCREATED_BY		VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		IF(EXISTS(SELECT * FROM tbmTEMPLATE WHERE fsNAME = @fsNAME))
		BEGIN

			SELECT RESULT = 'ERROR:樣板名稱重複'

		END
		ELSE
		BEGIN

			BEGIN TRANSACTION
			
			INSERT
				tbmTEMPLATE
				(fsNAME, fsTABLE, fsDESCRIPTION, fcIS_SEARCH, fdCREATED_DATE, fsCREATED_BY)
			VALUES
				(@fsNAME, @fsTABLE, @fsDESCRIPTION, @fcIS_SEARCH, GETDATE(), @fsCREATED_BY)
			
			COMMIT

			--要回傳樣板編號，讓樣板欄位可以新增
			SELECT RESULT = IDENT_CURRENT('tbmTEMPLATE')
		END
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





