
CREATE FUNCTION [dbo].[fn_SLPIT](@input nvarchar(MAX),@split nvarchar(2))

RETURNS @tblReturn TABLE (ID BIGINT IDENTITY(1,1),COL1 nvarchar(60))
 
AS
BEGIN
	DECLARE @CIndex smallint
	WHILE (@input <> '')
	BEGIN
		SET @CIndex=CHARINDEX(@split,@input)
		IF @CIndex=0 SET @CIndex=LEN(@input)+1
 
		--透過substring函數取得第一個字串，並輸入資料表變數中
		INSERT INTO @tblReturn (COL1)
		VALUES (SUBSTRING(@input,1,@CIndex-1))

		IF @CIndex=LEN(@input)+1 BREAK
		SET @input=SUBSTRING(@input,@CIndex+1,LEN(@input)-@CIndex)
	END
	RETURN
END
