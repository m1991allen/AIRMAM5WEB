



-- =============================================
-- 描述:	修改 ARC_AUDIO 入庫項目-聲音檔置換
-- 記錄:	<2016/12/19><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_ARC_AUDIO_CHANGE]

	@fsFILE_NO			VARCHAR(16),
	@fsFILE_TYPE		VARCHAR(10) = '',	
	@fsFILE_TYPE_H		VARCHAR(10) = '',
	@fsFILE_TYPE_L		VARCHAR(10) = '',
	@fsFILE_SIZE		VARCHAR(50) = '',
	@fsFILE_SIZE_H		VARCHAR(50) = '',
	@fsFILE_SIZE_L		VARCHAR(50) = '',
	@fsFILE_PATH		NVARCHAR(100) = '',			
	@fsFILE_PATH_H		NVARCHAR(100) = '',
	@fsFILE_PATH_L		NVARCHAR(100) = '',
	@fxMEDIA_INFO		NVARCHAR(MAX) = '',
	@fsALBUM			NVARCHAR(100) = '',
	@fsALBUM_TITLE		NVARCHAR(100) = '',
	@fsALBUM_ARTISTS	NVARCHAR(100) = '',
	@fsALBUM_PERFORMERS	NVARCHAR(100) = '',
	@fsALBUM_COMPOSERS	NVARCHAR(100) = '',
	@fnALBUM_YEAR		NVARCHAR(100) = '',
	@fsALBUM_COPYRIGHT	NVARCHAR(250) = '',
	@fsALBUM_GENRES		NVARCHAR(100) = '',
	@fsALBUM_COMMENT	NVARCHAR(250) = '',
	@fcALBUM_PICTURE	CHAR(1) = 'N',
	@fdBEG_TIME			DECIMAL(13,3) = 0.0,
	@fdEND_TIME			DECIMAL(13,3) = 0.0,
	@fdDURATION			DECIMAL(13,3) = 0.0,
	@fsUPDATED_BY		VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		UPDATE
			tbmARC_AUDIO
		SET
            fsFILE_TYPE			= @fsFILE_TYPE,
            fsFILE_TYPE_H		= @fsFILE_TYPE_H,
            fsFILE_TYPE_L		= @fsFILE_TYPE_L,            
            fsFILE_SIZE			= @fsFILE_SIZE,
            fsFILE_SIZE_H		= @fsFILE_SIZE_H,
            fsFILE_SIZE_L		= @fsFILE_SIZE_L,                        
            fsFILE_PATH			= @fsFILE_PATH,
            fsFILE_PATH_H		= @fsFILE_PATH_H,
            fsFILE_PATH_L		= @fsFILE_PATH_L,                        
            fxMEDIA_INFO		= @fxMEDIA_INFO,
			fsALBUM				= @fsALBUM,	
			fsALBUM_TITLE		= @fsALBUM_TITLE,
			fsALBUM_ARTISTS		= @fsALBUM_ARTISTS,
			fsALBUM_PERFORMERS	= @fsALBUM_PERFORMERS,
			fsALBUM_COMPOSERS	= @fsALBUM_COMPOSERS,
			fnALBUM_YEAR		= @fnALBUM_YEAR,
			fsALBUM_COPYRIGHT	= @fsALBUM_COPYRIGHT,
			fsALBUM_GENRES		= @fsALBUM_GENRES,
			fsALBUM_COMMENT		= @fsALBUM_COMMENT,
			fcALBUM_PICTURE		= @fcALBUM_PICTURE,
			fdBEG_TIME			= @fdBEG_TIME,
			fdEND_TIME			= @fdEND_TIME,
			fdDURATION			= @fdDURATION,
			fdUPDATED_DATE 		= GETDATE(),
			fsUPDATED_BY		= @fsUPDATED_BY

		WHERE
		
			(fsFILE_NO = @fsFILE_NO)
		
		COMMIT

		SELECT RESULT = ''
				
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END






