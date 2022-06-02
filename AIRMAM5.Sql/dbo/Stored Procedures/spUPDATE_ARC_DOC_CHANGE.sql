



-- =============================================
-- 描述:	修改 ARC_DOC 入庫項目-文件檔置換
-- 記錄:	<2016/12/19><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_ARC_DOC_CHANGE]

	@fsFILE_NO				VARCHAR(16)  = '',
	@fsFILE_TYPE			VARCHAR(10)  = '',
	--@fsFILE_TYPE_1			VARCHAR(10) = '',
	--@fsFILE_TYPE_2			VARCHAR(10) = '',		
	@fsFILE_SIZE			NVARCHAR(50) = '',
	--@fsFILE_SIZE_1			NVARCHAR(50) = '',
	--@fsFILE_SIZE_2			NVARCHAR(50) = '',		
	@fsFILE_PATH			NVARCHAR(100) = '',
	--@fsFILE_PATH_1			NVARCHAR(100) = '',
	--@fsFILE_PATH_2			NVARCHAR(100) = '',		
	@fxMEDIA_INFO			NVARCHAR(MAX) = '',
	@fdFILE_CREATED_DATE	DATETIME,	
	@fdFILE_UPDATED_DATE	DATETIME = NULL,	
	@fsUPDATED_BY			VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION
		
		UPDATE
			tbmARC_DOC
		SET
            fsFILE_TYPE		 = @fsFILE_TYPE,
            --fsFILE_TYPE_1	 = @fsFILE_TYPE_1,
            --fsFILE_TYPE_2	 = @fsFILE_TYPE_2,
            fsFILE_SIZE		 = @fsFILE_SIZE,
            --fsFILE_SIZE_1	 = @fsFILE_SIZE_1,
            --fsFILE_SIZE_2	 = @fsFILE_SIZE_2,                        
            fsFILE_PATH	 	 = @fsFILE_PATH,
            --fsFILE_PATH_1 	 = @fsFILE_PATH_1,
            --fsFILE_PATH_2 	 = @fsFILE_PATH_2,
            fxMEDIA_INFO     = @fxMEDIA_INFO,
            fdFILE_CREATED_DATE    = @fdFILE_CREATED_DATE,
            fdFILE_UPDATED_DATE    = @fdFILE_UPDATED_DATE,
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





