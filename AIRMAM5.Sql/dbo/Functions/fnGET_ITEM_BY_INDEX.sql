-- =============================================
-- 描述:	取得list中第i個項目(用分號隔開的)
-- 記錄:	<2011/08/17><Dennis.Wen><新增預存>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_ITEM_BY_INDEX]
(
	@List varchar(max) , @i bigint
)
RETURNS	VARCHAR(200)
AS
BEGIN
	Declare @idx1 bigint
	Declare @idx2 bigint
	declare @j bigint
	Set @idx1=0
	Set @idx2=0  
	Set @j=0

	WHILE (@j<=@i) 
	BEGIN
		IF @idx2 = Len(@List)
			BEGIN
				Set @idx1 = @idx2
			END
		ELSE
			BEGIN
				Set @idx1 = @idx2+1
				Set @idx2 = CHARINDEX(';', SUBSTRING(@List,@idx1+1,Len(@List)-@idx1))+@idx1
				IF SUBSTRING(@List,@idx1,1)=';'
					BEGIN
						Set @idx2= @idx1 
					END
			END
		Set @j = @j+1
	END
 

	IF @idx2= @idx1
		BEGIN
			RETURN''
		END
	ELSE
		BEGIN
			RETURN SUBSTRING(@List,@idx1,@idx2-@idx1) 
		END	

	RETURN''

END

