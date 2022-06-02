



-- =============================================
-- 描述:	依照傳入的FILE_NOs和TYPEs取出ARC入庫項目的縮圖和資訊(複數,用;串)
-- 記錄:	<2011/11/28><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_ARC_IMAGE_URLS_AND_FILE_INFOS_BY_TYPES_AND_FILE_NOS]
	@FILE_NOs	VARCHAR(MAX),	
	@TYPEs		VARCHAR(MAX)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DECLARE
			@FILE_INFOs	NVARCHAR(MAX) = '', --要回傳的
			@IMAGE_URLs	NVARCHAR(MAX) = '', --要回傳的
			@IMAGE_URL	NVARCHAR(100) = '', --每一個
			@FILE_INFO	NVARCHAR(200) = '', --每一個
			@FILE_NO	VARCHAR(16) = '',
			@TYPE		VARCHAR(1) = '',
			@COUNT INT, @i INT = 0
			
		SET @COUNT = LEN(@FILE_NOs) - LEN(REPLACE(@FILE_NOs,';',''))

		WHILE(@i < @COUNT)
		BEGIN
			/*依序取出每一個FILE_NO, TYPE*/
			SELECT	@FILE_NO = dbo.fnGET_ITEM_BY_INDEX(@FILE_NOs, @i),
					@TYPE = dbo.fnGET_ITEM_BY_INDEX(@TYPEs, @i)
					
			/*丟到函數取出IMAGE_URL, FILE_INFO*/
			SELECT	@IMAGE_URL = dbo.fnGET_IMAGE_URL_BY_TYPE_AND_FILE_NO(@TYPE,@FILE_NO),
					@FILE_INFO = dbo.fnGET_FILE_INFO_BY_DataType_AND_FILE_NO(@TYPE,@FILE_NO)
			
			SELECT	@IMAGE_URLs = @IMAGE_URLs + @IMAGE_URL + ';',
					@FILE_INFOs = @FILE_INFOs + @FILE_INFO + ';'

			SET @i = @i + 1
		END

		SELECT	IMAGE_URLs = @IMAGE_URLs, FILE_INFOs = @FILE_INFOs
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





