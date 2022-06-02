
-- =============================================
-- 描述:	取得list中第i個項目(用分號隔開的)
-- 記錄:	<2011/09/28><Eric.Huang><新增預存>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_USERNAME_FROM_LIST]
(
	@USER_LIST VARCHAR(1000)
)
RETURNS	NVARCHAR(4000)
AS
BEGIN
	DECLARE @cnt INT, @idx INT, @i INT, @result NVARCHAR(4000)
	SELECT @cnt = 0, @idx = 0, @i = 0, @result = ''
 
	SELECT @cnt = LEN(REPLACE(@USER_LIST,';',';;')) - LEN(@USER_LIST)
 
 
	WHILE(@i < @cnt)
	BEGIN
		Declare @Code nvarchar(20)
		Declare @Name nvarchar(100)

		SELECT @Code = dbo.fnGET_ITEM_BY_INDEX(@USER_LIST,@i)
		Set @Name = ISNULL((SELECT fsNAME FROM tbmUSERS WHERE fsLOGIN_ID = @Code),'')
		
		SET @result = @result + @Name + ';'
	
		SET @i = @i + 1
	END
    
	--SELECT @result
	RETURN @result
END


