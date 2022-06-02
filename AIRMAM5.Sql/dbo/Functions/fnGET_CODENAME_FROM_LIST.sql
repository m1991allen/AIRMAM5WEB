
-- =============================================
-- 描述:	根據代碼字串取回名稱字串
-- 記錄:	<2011/11/07><Dennis.Wen><新增預存>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_CODENAME_FROM_LIST]
(
	@CODE_ID VARCHAR(10),
	@CODE_LIST VARCHAR(MAX)
)
RETURNS	NVARCHAR(4000)
AS
BEGIN
	DECLARE @cnt INT, @idx INT, @i INT, @result NVARCHAR(4000)
	SELECT @cnt = 0, @idx = 0, @i = 0, @result = ''
 
	SELECT @cnt = LEN(REPLACE(@CODE_LIST,';',';;')) - LEN(@CODE_LIST)
 
 
	WHILE(@i < @cnt)
	BEGIN
		Declare @Code nvarchar(20)
		Declare @Name nvarchar(100)

		SELECT @Code = dbo.fnGET_ITEM_BY_INDEX(@CODE_LIST,@i)
		Set @Name = ISNULL((SELECT fsNAME FROM tbzCODE WHERE fsCODE_ID = @CODE_ID AND fsCODE = @Code),'')
		
		SET @result = @result + @Name + '、'
	
		SET @i = @i + 1
	END
    
	IF(LEN(@result) > 0) BEGIN SET @result = SUBSTRING(@result,1,LEN(@result) - 1) END

	--SELECT @result
	RETURN @result
END


