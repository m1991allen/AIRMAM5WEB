






-- =============================================
-- 描述:	修改 ARC_VIDEO 入庫項目-影片檔置換
-- 記錄:	<2016/12/16><David.Sin><新增本預存>
-- 記錄:	<2019/08/16><David.Sin><判斷是否刪除段落描述與關鍵影格>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_ARC_VIDEO_CHANGE]

	@fsFILE_NO			VARCHAR(16),
	@fsFILE_TYPE		VARCHAR(10) = '',	
	@fsFILE_TYPE_H		VARCHAR(10) = '',
	@fsFILE_TYPE_L		VARCHAR(10) = '',
	@fsFILE_SIZE		NVARCHAR(50) = '',
	@fsFILE_SIZE_H		NVARCHAR(50) = '',
	@fsFILE_SIZE_L		NVARCHAR(50) = '',
	@fsFILE_PATH		NVARCHAR(100) = '',			
	@fsFILE_PATH_H		NVARCHAR(100) = '',
	@fsFILE_PATH_L		NVARCHAR(100) = '',
	@fxMEDIA_INFO		NVARCHAR(MAX) = '',
	@fdBEG_TIME			DECIMAL(13,3) = 0.0,
	@fdEND_TIME			DECIMAL(13,3) = 0.0,
	@fdDURATION			DECIMAL(13,3) = 0.0,
	@fsRESOL_TAG		VARCHAR(2),
	@fsUPDATED_BY		VARCHAR(50),
	@fcDELETE_KF		CHAR(1) = 'N'
	
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		UPDATE
			tbmARC_VIDEO
		SET
			fsFILE_TYPE		 = @fsFILE_TYPE,
			fsFILE_TYPE_H	 = @fsFILE_TYPE_H,
			fsFILE_TYPE_L	 = @fsFILE_TYPE_L,
			fsFILE_SIZE		 = @fsFILE_SIZE,
			fsFILE_SIZE_H	 = @fsFILE_SIZE_H,
			fsFILE_SIZE_L	 = @fsFILE_SIZE_L,
			fsFILE_PATH		 = @fsFILE_PATH,
			fsFILE_PATH_H	 = @fsFILE_PATH_H,
			fsFILE_PATH_L	 = @fsFILE_PATH_L,
			fxMEDIA_INFO	 = @fxMEDIA_INFO,
			fdBEG_TIME		 = @fdBEG_TIME,
			fdEND_TIME		 = @fdEND_TIME,
			fdDURATION		 = @fdDURATION,
			fsRESOL_TAG		 = @fsRESOL_TAG,
			fdUPDATED_DATE	 = GETDATE(),
			fsUPDATED_BY	 = @fsUPDATED_BY
		WHERE
			(fsFILE_NO = @fsFILE_NO)
		
		--順到刪除段落描述與KF
		IF(@fcDELETE_KF = 'Y')
		BEGIN
			DELETE FROM tbmARC_VIDEO_D WHERE fsFILE_NO = @fsFILE_NO
			DELETE FROM tbmARC_VIDEO_K WHERE fsFILE_NO = @fsFILE_NO
		END
		ELSE
		BEGIN

			UPDATE tbmARC_VIDEO_K
			SET fsFILE_PATH = '',fsFILE_SIZE = '', fsFILE_TYPE = '', fcHEAD_FRAME = 'N'
			WHERE fsFILE_NO = @fsFILE_NO

		END
		COMMIT

		SELECT RESULT = ''
	END TRY
	
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END








