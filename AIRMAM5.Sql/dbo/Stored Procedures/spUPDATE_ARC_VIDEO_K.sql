




-- =============================================
-- 描述:	修改 ARC_VIDEO_K 入庫項目-影片關鍵影格檔 資料
-- 記錄:	<2011/11/25><Mihsiu.Chiu><新增本預存>
--			<2012/05/21><Dennis.Wen><一堆欄位調整>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_ARC_VIDEO_K]
	@fsFILE_NO			VARCHAR(16),
	@fsTIME				VARCHAR(16),
	@fsTITLE			NVARCHAR(100) = '',
	@fsDESCRIPTION		NVARCHAR(MAX) = '',
	@fsFILE_PATH		NVARCHAR(100), 
	@fsFILE_SIZE		VARCHAR(50), 
	@fsFILE_TYPE		VARCHAR(10),	
	@fsUPDATED_BY		VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		IF(@fsTITLE = '') BEGIN SET @fsTITLE = (SELECT fsTITLE FROM tbmARC_VIDEO_K WHERE fsFILE_NO = @fsFILE_NO AND fsTIME = @fsTIME) END

		BEGIN TRANSACTION

		UPDATE
			tbmARC_VIDEO_K
		SET 
            fsTITLE = @fsTITLE,
            fsDESCRIPTION = @fsDESCRIPTION,
			fsFILE_PATH = @fsFILE_PATH, 
			fsFILE_SIZE = @fsFILE_SIZE, 
			fsFILE_TYPE = @fsFILE_TYPE,
            fdUPDATED_DATE   = GETDATE(),
            fsUPDATED_BY     = @fsUPDATED_BY
		WHERE		
			(fsFILE_NO = @fsFILE_NO)AND
			(fsTIME = @fsTIME)
		
		COMMIT

		SELECT RESULT = @@ROWCOUNT
	END TRY
	
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END







