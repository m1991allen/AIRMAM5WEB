-- =============================================
-- 描述:	取得list中第i個項目(用分號隔開的)
-- 記錄:	<2011/10/14><Dennis.Wen><新增預存>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_STRING_MATCH]
(
	@STR1 VARCHAR(5000), @STR2 VARCHAR(5000)
)
RETURNS	VARCHAR(1)
AS
BEGIN
	DECLARE @IS_MATCH VARCHAR(1) = 'N'
	DECLARE @L1 INT, @L2 INT

	--長度
	SELECT @L1 = LEN(@STR1), @L2 = LEN(@STR2)

	--減少回圈數
	DECLARE @STR3 VARCHAR(5000)
	IF (@L1 > @L2)
	BEGIN
		SET @STR3 = @STR1
		SET @STR1 = @STR2
		SET @STR2 = @STR3
	END
	 
	--計算個數
	SET @L1 = LEN(REPLACE(@STR1,';',';;')) - LEN(@STR1)
	SET @L2 = LEN(REPLACE(@STR2,';',';;')) - LEN(@STR2)

	DECLARE @idx INT = '0'
	WHILE ((@idx < @L1) AND (@IS_MATCH = 'N') AND (@L2 <> 0))
	BEGIN
		IF(((';'+@STR2) LIKE '%;'+ dbo.fnGET_ITEM_BY_INDEX(@STR1,@idx) +';%') AND (dbo.fnGET_ITEM_BY_INDEX(@STR1,@idx) <> ''))
		BEGIN
			SET @IS_MATCH = 'Y'
		END
		
		SET @idx = @idx + 1
	END

	RETURN @IS_MATCH
END

