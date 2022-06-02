-- ============================================= 
-- 記錄:	<2013/10/15><Dennis.Wen><新增預存>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_TIMECODE_FROM_SECONDS3]
(
	@SECONDS DECIMAL(29,3)
)
RETURNS	VARCHAR(30)
AS
BEGIN
	DECLARE @HH	INT = 0, @MM INT = 0, @SS DECIMAL(6,2) = 0, @R INT = 0 , @SS_TEMP VARCHAR(2)

	SET @R = @SECONDS/60
	SET @MM = @R
	SET @SS = @SECONDS-@MM*60
 
	SET @R = @MM/60
	SET @HH = @R
	SET @MM = @MM-@HH*60

	SET @HH = @R

	SET @SS_TEMP = LEFT(@SS,CHARINDEX('.',@SS)-1)
	---

	--SELECT	RIGHT(('00'+CAST(@HH AS VARCHAR(2))),2) + N':' +
	--		RIGHT(('00'+CAST(@MM AS VARCHAR(2))),2) + N':' +
	--		CAST(@SS AS VARCHAR(7))

	DECLARE @HH_T VARCHAR(10) = (CASE WHEN (LEN(CAST(@HH AS VARCHAR(10))) > 3) THEN (LEFT(CAST(@HH AS VARCHAR(10)), LEN(CAST(@HH AS VARCHAR(10)))-3) + ',' + RIGHT(CAST(@HH AS VARCHAR(10)), 3) )
									WHEN (@HH <=9 ) THEN ('0'+CAST(@HH AS VARCHAR(2)))
								 ELSE CAST(@HH AS VARCHAR(10)) END)

	DECLARE @MM_T VARCHAR(10) = (CASE WHEN (@MM > 9) THEN CAST(@MM AS VARCHAR(2)) ELSE '0'+CAST(@MM AS VARCHAR(2)) END )

	
	DECLARE @SS_T VARCHAR(10) = (CASE WHEN (@SS > 9.99) THEN CAST(@SS AS VARCHAR(10)) 
	                                            ELSE '0'+ CAST(@SS AS VARCHAR(10)) END )
				 
	--RETURN 	@HH_T + N'h  ' +
	--		CAST(@MM AS VARCHAR(2)) + N'm  ' +
	--		--RIGHT(CAST(@SS AS INT),2)
	--		REPLACE(CAST(@SS AS VARCHAR(10)), '.000', '') + 's'

		RETURN 	@HH_T + N':' +
			@MM_T + N':' +
			@SS_T
END



