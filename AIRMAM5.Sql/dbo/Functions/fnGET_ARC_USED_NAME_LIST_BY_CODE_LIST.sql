


-- =============================================
-- 描述:	取出一個媒體檔自訂欄位中有用到的所有代碼
-- 記錄:	<2012/04/27><Dennis.Wen><新增本函數>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_ARC_USED_NAME_LIST_BY_CODE_LIST](
	@CODE_LIST	VARCHAR(1000)
)
RETURNS NVARCHAR(2000)
AS
BEGIN   
	/*變數宣告*/
	DECLARE @RESULT			NVARCHAR(2000) = '',
			@CODE_LIST_ITEM	VARCHAR(1000) = '',
			@CODE_ID		VARCHAR(10) = '',
			@CODE			VARCHAR(20) = '',
			@CODE_NAME		NVARCHAR(50) = '',
			@CNT1 INT = 0, @idx1 INT = 0,
			@CNT2 INT = 0, @idx2 INT = 0

	/*暫時改變符號*/
	SET @CODE_LIST = REPLACE(@CODE_LIST, ';' , ',')
	SET @CODE_LIST = REPLACE(@CODE_LIST, '/' , ';')

	/*取得代碼組數*/
	SET @CNT1 = LEN(REPLACE(@CODE_LIST, ';' , ';;')) - LEN(@CODE_LIST)
	 
	WHILE(@idx1 < @CNT1)
	BEGIN
		SET @CODE_LIST_ITEM = dbo.fnGET_ITEM_BY_INDEX(@CODE_LIST, @idx1)

		----------開始處理每一組代碼---------------
		/*變數初始*/
		SELECT @CNT2 = 0, @idx2 = 0, @CODE_NAME = ''
		
		SET @CODE_ID = LEFT(@CODE_LIST_ITEM, CHARINDEX(':', @CODE_LIST_ITEM)-1)
		SET @CODE_LIST_ITEM =REPLACE(@CODE_LIST_ITEM, @CODE_ID + ':' , '')
		
		/*暫時改變符號*/
		SET @CODE_LIST_ITEM = REPLACE(@CODE_LIST_ITEM, ',' , ';')
	 
		/*取得代碼組數*/
		SET @CNT2 = LEN(REPLACE(@CODE_LIST_ITEM, ';' , ';;')) - LEN(@CODE_LIST_ITEM)
		
		WHILE(@idx2 < @CNT2)
		BEGIN
			SET @CODE = dbo.fnGET_ITEM_BY_INDEX(@CODE_LIST_ITEM, @idx2)
			SET @CODE_NAME = (SELECT fsNAME FROM tbzCODE WHERE (fsCODE_ID = @CODE_ID) AND (fsCODE = @CODE))+ ';'

			IF NOT(@RESULT LIKE '%'+@CODE_NAME+'%')
			BEGIN
				SET @RESULT = @RESULT + @CODE_NAME
			END
		 
			SET @idx2 = @idx2 + 1
		END 	
		----------開始處理每一組代碼---------------
	 
		SET @idx1 = @idx1 + 1
	END 

--	--SELECT @RESULT
	RETURN @RESULT
--RETURN''
END




