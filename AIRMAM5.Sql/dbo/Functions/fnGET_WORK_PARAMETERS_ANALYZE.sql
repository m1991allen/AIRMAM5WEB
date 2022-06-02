-- =============================================
-- 描述:	解析WORK的PARAMETERS 字串
-- 記錄:	<2012/01/16><Eric.Huang><新增預存>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_WORK_PARAMETERS_ANALYZE]
(
	@PARAMETERS		NVARCHAR(200)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN

	DECLARE @strSPLIT		NVARCHAR(200);	
	DECLARE @strTMP			NVARCHAR(50);		        	        	        
	DECLARE @strRESULT		NVARCHAR(MAX);
	DECLARE @cnt			INT;
	DECLARE @i				INT;

	SELECT @cnt = 0, @i = 0,@strRESULT  = '' , @strSPLIT = '', @strTMP = ''
	
	SELECT @cnt = LEN(REPLACE(@PARAMETERS,';',';;')) - LEN(@PARAMETERS)
	
	WHILE (@cnt > @i)
	BEGIN
	
		SELECT @strSPLIT = dbo.fnGET_ITEM_BY_INDEX(@PARAMETERS,@i)		
		IF @i = 0
		BEGIN
			SET @strTMP = CASE @strSPLIT 
				WHEN 'V' THEN '檔案格式:影片檔'
				WHEN 'A' THEN '檔案格式:聲音檔'
				WHEN 'P' THEN '檔案格式:圖片檔'
				WHEN 'D' THEN '檔案格式:文件檔'
				ELSE 'NO'
			END
		END

		IF @i = 1
		BEGIN
			SET @strTMP = '檔案編號:' + @strSPLIT 
		END

		IF @i = 2
		BEGIN
			SET @strTMP = '主題編號:' + @strSPLIT 
		END

		IF @i = 3
		BEGIN
			SET @strTMP = CASE @strSPLIT
				WHEN 'HL' THEN  '轉檔格式:高解及低解' 
				WHEN 'HLT' THEN '轉檔格式:高解及低解和縮圖' 
				WHEN 'HLK' THEN '轉檔格式:高解及低解和關鍵影格' 
				WHEN 'L' THEN	'轉檔格式:擷取內文'
				WHEN 'X' THEN	'轉檔格式:擷取內文' 
				ELSE '轉檔格式:無'
			END
		END
	
		SET @i= @i + 1
		SET @strRESULT = @strRESULT + @strTMP + '/'

	END

	RETURN @strRESULT

END

