-- =============================================
-- 描述:	取得list中第i個項目(用分號隔開的)
-- 記錄:	<2011/08/17><Dennis.Wen><新增預存>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_GROUPNAME_FROM_LIST]
(
	@GROUP_LIST VARCHAR(500)
)
RETURNS	NVARCHAR(4000)
AS
BEGIN
	DECLARE @cnt INT, @idx INT, @i INT, @result NVARCHAR(4000)
	SELECT @cnt = 0, @idx = 0, @i = 0, @result = ''
 
	SELECT @cnt = LEN(REPLACE(@GROUP_LIST,';',';;')) - LEN(@GROUP_LIST)
 
 
	WHILE(@i < @cnt)
	BEGIN
		Declare @Code nvarchar(20)
		Declare @Name nvarchar(100)

		SELECT @Code = dbo.fnGET_ITEM_BY_INDEX(@GROUP_LIST,@i)
		Set @Name = ISNULL((SELECT fsNAME FROM tbmGROUPS WHERE fsGROUP_ID = @Code),'')
		
		IF(@Name <> '') BEGIN SET @result = @result + @Name + ';' END
	
		SET @i = @i + 1
	END
    
	--SELECT @result
	RETURN @result
END

