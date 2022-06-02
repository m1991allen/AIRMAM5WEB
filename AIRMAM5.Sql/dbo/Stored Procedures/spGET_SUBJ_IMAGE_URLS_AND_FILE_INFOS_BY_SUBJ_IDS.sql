



-- =============================================
-- 描述:	依照傳入的SUBJ_IDss取出主題檔的縮圖和資訊(複數,用;串)
-- 記錄:	<2012/05/23><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_SUBJ_IMAGE_URLS_AND_FILE_INFOS_BY_SUBJ_IDS]
	@SUBJ_IDs	VARCHAR(MAX)
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE
			@SUBJ_INFOs	NVARCHAR(MAX) = '', --要回傳的
			@IMAGE_URLs	NVARCHAR(MAX) = '', --要回傳的
			@IMAGE_URL	NVARCHAR(100) = '', --每一個
			@SUBJ_INFO	NVARCHAR(200) = '', --每一個
			@SUBJ_ID	VARCHAR(12) = '',
			@COUNT INT, @i INT = 0
			
		SET @COUNT = LEN(@SUBJ_IDs) - LEN(REPLACE(@SUBJ_IDs,';',''))

		WHILE(@i < @COUNT)
		BEGIN
			/*依序取出每一個@SUBJ_ID*/
			SELECT	@SUBJ_ID = dbo.fnGET_ITEM_BY_INDEX(@SUBJ_IDs, @i)
 
			/*丟到函數取出IMAGE_URL, FILE_INFO*/
			SELECT	@IMAGE_URL = dbo.fnGET_IMAGE_URL_BY_TYPE_AND_FILE_NO('S',@SUBJ_ID),
					@SUBJ_INFO = dbo.fnGET_FILE_INFO_BY_DataType_AND_FILE_NO('S',@SUBJ_ID)
			
			SELECT	@IMAGE_URLs = @IMAGE_URLs + @IMAGE_URL + ';',
					@SUBJ_INFOs = @SUBJ_INFOs + @SUBJ_INFO + ';'

			SET @i = @i + 1
		END

		SELECT	IMAGE_URLs = @IMAGE_URLs, SUBJ_INFOs = @SUBJ_INFOs
		
END





