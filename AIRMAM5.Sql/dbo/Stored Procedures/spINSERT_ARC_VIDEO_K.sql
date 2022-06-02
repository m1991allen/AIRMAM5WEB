

-- =============================================
-- 描述:	新增 ARC_VIDEO_K 入庫項目-影片關鍵影格檔 資料
-- 記錄:	<2011/11/25><Mihsiu.Chiu><新增本預存>
--			<2012/05/21><Dennis.Wen><一堆欄位調整>
--			<2012/06/08><Eric.Huang><如果新增成功，傳回新關鍵影格的URL供SL使用>
--			<2019/08/16><David.Sin><判斷是否已存在關鍵影格資訊>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_ARC_VIDEO_K]		
	@fsFILE_NO			VARCHAR(16),
	@fsTIME				VARCHAR(16),
	@fsTITLE			NVARCHAR(100),
	@fsDESCRIPTION		NVARCHAR(MAX) = '',
	@fsFILE_PATH		NVARCHAR(100), 
	@fsFILE_SIZE		VARCHAR(50), 
	@fsFILE_TYPE		VARCHAR(10),	
	@fsCREATED_BY		VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		IF ((SELECT COUNT(fsTIME) FROM tbmARC_VIDEO_K WHERE fsFILE_NO = @fsFILE_NO AND fsTIME = @fsTIME) = 0)
		BEGIN
			
			BEGIN TRANSACTION

			INSERT
				tbmARC_VIDEO_K			
				(fsFILE_NO, fsTIME , fsTITLE ,fsDESCRIPTION,fsFILE_PATH, fsFILE_SIZE, fsFILE_TYPE,fcHEAD_FRAME,fdCREATED_DATE, fsCREATED_BY)
			VALUES		
				(@fsFILE_NO, @fsTIME, @fsTITLE ,@fsDESCRIPTION,@fsFILE_PATH, @fsFILE_SIZE, @fsFILE_TYPE, 'N', GETDATE(), @fsCREATED_BY)
			
			COMMIT

			SELECT RESULT = dbo.fnGET_KEYFRAME_IMAGE_URL_BY_FILE_NO_AND_TIME(@fsFILE_NO, @fsTIME)
		END
		ELSE
		BEGIN
			--此狀況通常發生在沒有實體檔案卻有資訊
			BEGIN TRANSACTION

			UPDATE
				tbmARC_VIDEO_K
			SET
				fsTITLE = @fsTITLE,
				fsDESCRIPTION = @fsDESCRIPTION,
				fsFILE_PATH = @fsFILE_PATH,
				fsFILE_SIZE = @fsFILE_SIZE,
				fsFILE_TYPE = @fsFILE_TYPE,
				fcHEAD_FRAME = 'N',
				fdUPDATED_DATE = GETDATE(),
				fsUPDATED_BY = @fsCREATED_BY
			WHERE
				fsFILE_NO = @fsFILE_NO AND
				fsTIME = @fsTIME

			COMMIT

			SELECT RESULT = dbo.fnGET_KEYFRAME_IMAGE_URL_BY_FILE_NO_AND_TIME(@fsFILE_NO, @fsTIME)
		END
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



