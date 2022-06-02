





-- =============================================
-- 描述:	修改 TEMPLATE 主檔 資料
-- 記錄:	<2011/09/16><Eric.Huang><新增本預存>
-- 記錄:	<2011/10/14><Mihsiu.Chiu><修改本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_TEMPLATE]

	@fnTEMP_ID			INT,
	@fsNAME				NVARCHAR(50),
	@fsTABLE			CHAR(1),
	@fsDESCRIPTION		NVARCHAR(50),
	@fcIS_SEARCH		CHAR(1),
	@fsUPDATED_BY		VARCHAR(50)


AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE
			tbmTEMPLATE
		SET
			fsNAME			 = @fsNAME	,
			fsTABLE			 = @fsTABLE,
			fsDESCRIPTION	 = @fsDESCRIPTION,
			fcIS_SEARCH      = @fcIS_SEARCH,
            fdUPDATED_DATE   = GETDATE(),
            fsUPDATED_BY     = @fsUPDATED_BY
            
		WHERE
		
			(fnTEMP_ID = @fnTEMP_ID)
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END








