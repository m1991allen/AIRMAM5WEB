




-- =============================================
-- 描述:	修改 ARC_PHOTO 入庫項目-聲音檔置換
-- 記錄:	<2016/12/21><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_ARC_PHOTO_CHANGE]

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
	@fnWIDTH			INT = 0,
	@fnHEIGHT			INT = 0,
	@fnXDPI				INT = 0,
	@fnYDPI				INT = 0,
	@fsCAMERA_MAKE		NVARCHAR(100) = '',
	@fsCAMERA_MODEL		NVARCHAR(100) = '',
	@fsFOCAL_LENGTH		NVARCHAR(100) = '',
	@fsEXPOSURE_TIME	NVARCHAR(100) = '',
	@fsAPERTURE			NVARCHAR(100) = '',
	@fnISO				INT = 0,
	@fsUPDATED_BY		VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		UPDATE
			tbmARC_PHOTO
		SET
            fsFILE_TYPE		= @fsFILE_TYPE,
            fsFILE_TYPE_H	= @fsFILE_TYPE_H,
            fsFILE_TYPE_L	= @fsFILE_TYPE_L,                        
            fsFILE_SIZE		= @fsFILE_SIZE,
            fsFILE_SIZE_H	= @fsFILE_SIZE_H,
            fsFILE_SIZE_L	= @fsFILE_SIZE_L,                        
            fsFILE_PATH		= @fsFILE_PATH,
            fsFILE_PATH_H	= @fsFILE_PATH_H,
            fsFILE_PATH_L	= @fsFILE_PATH_L,  
			fxMEDIA_INFO	= @fxMEDIA_INFO,                     
            fnWIDTH         = @fnWIDTH,
            fnHEIGHT		= @fnHEIGHT,
            fnXDPI			= @fnXDPI,
			fnYDPI			= @fnYDPI,
			fsCAMERA_MAKE	= @fsCAMERA_MAKE,	
			fsCAMERA_MODEL	= @fsCAMERA_MODEL,	
			fsFOCAL_LENGTH	= @fsFOCAL_LENGTH,	
			fsEXPOSURE_TIME = @fsEXPOSURE_TIME,
			fsAPERTURE		= @fsAPERTURE,		
			fnISO			= @fnISO,
			fdUPDATED_DATE 	 = GETDATE(),
			fsUPDATED_BY	 = @fsUPDATED_BY
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







