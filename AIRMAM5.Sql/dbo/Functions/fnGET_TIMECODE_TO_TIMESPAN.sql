

-- =============================================
-- 描述:	轉換TimeCode TO TimeSpan FORMAT=> '000330000'
-- 記錄:	<2012/12/28><Eric.Huang><新增本函數>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_TIMECODE_TO_TIMESPAN](
	@TIME_CODE	VARCHAR(12)
)
RETURNS VARCHAR(20)
AS
BEGIN   
 	--SET NOCOUNT ON;
 	
 	DECLARE @fsTIME_SPAN VARCHAR(12),
 			@fsHH	 	 VARCHAR(2),
 			@fsMM	  	 VARCHAR(2),
 			@fsSS 		 VARCHAR(2),
 			@fsFF		 VARCHAR(2),
 			@fsCAL		 INT
		
		
		
		
	IF (@TIME_CODE <> '')
	BEGIN
		SET @fsHH = SUBSTRING(@TIME_CODE,1,2)
		SET @fsMM = SUBSTRING(@TIME_CODE,4,2)
		SET @fsSS = SUBSTRING(@TIME_CODE,7,2)
		SET @fsFF = SUBSTRING(@TIME_CODE,10,2)
				
		SET @fsCAL = CAST(@fsFF AS INT) / 29.97 * 1000
		
		SET @fsTIME_SPAN = @fsHH + @fsMM + @fsSS + RIGHT(REPLICATE('0',3) + CAST(@fscal AS VARCHAR),3)
	END
	 
	RETURN @fsTIME_SPAN
END



