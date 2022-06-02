-- =============================================
-- 描述:	解析WORK的PARAMETERS 字串
-- 記錄:	<2012/04/18><Mihsiu.Chiu><新增預存>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_WORK_PARAMETERS_ANALYZE_BOOKING]
(
	@PARAMETERS		NVARCHAR(200),
	@nIndex			int,
	@sTITLE			varchar(1) = 'N',
	@sSource		varchar(1) = 'N'
)
RETURNS NVARCHAR(MAX)
AS
BEGIN

	DECLARE @strSPLIT		NVARCHAR(200);	
	DECLARE @strTMP			NVARCHAR(50);	
	DECLARE @cnt			INT;

	SELECT @cnt = 0, @strSPLIT = '', @strTMP = ''
	
	SELECT @cnt = LEN(REPLACE(@PARAMETERS,';',';;')) - LEN(@PARAMETERS)
	
	if (@cnt > @nIndex) begin
	
		SELECT @strSPLIT = dbo.fnGET_ITEM_BY_INDEX(@PARAMETERS,@nIndex)		
		IF @nIndex = 0
		BEGIN			
			SET @strTMP = CASE @strSPLIT 
				WHEN 'V' THEN '影片檔'
				WHEN 'A' THEN '聲音檔'
				WHEN 'P' THEN '圖片檔'
				WHEN 'D' THEN '文件檔'
				ELSE @strSPLIT
			END
			if (@sTITLE = 'Y') begin set @strTMP = '檔案格式:'+@strTMP; end
			if (@sSource = 'Y') begin set @strTMP = @strSPLIT end
		END		

		IF @nIndex = 1
		BEGIN			
			SET @strTMP = @strSPLIT ;
			if (@sTITLE = 'Y') begin set @strTMP = '檔案編號:'; end
		END
		
		--PATH_TYPE
		IF @nIndex = 2
		BEGIN
			SET @strTMP = (
				SELECT fsNAME FROM tbzCODE WHERE fsCODE = @strSPLIT 
				AND fsCODE_ID = 'BOOK003'
				);
			if (@sTITLE = 'Y') begin set @strTMP = '產生途徑:' + @strTMP; end
			if (@sSource = 'Y') begin set @strTMP = @strSPLIT end
		END

		IF @nIndex = 3
		BEGIN
			SET @strTMP = @strSPLIT
			if (@sTITLE = 'Y') begin set @strTMP = '產生目錄：'+ @strTMP; end
		END
		
		IF @nIndex = 4
		BEGIN
			SET @strTMP =  @strSPLIT
			if (@sTITLE = 'Y') begin set @strTMP = '調用格式：'+ @strTMP; end
		END
		
		IF @nIndex = 5
		BEGIN
			SET @strTMP =  @strSPLIT
			if (@sTITLE = 'Y') begin set @strTMP = '起始時間：'+ @strTMP; end
		END
		
		IF @nIndex = 6
		BEGIN
			SET @strTMP =  @strSPLIT
			if (@sTITLE = 'Y') begin set @strTMP = '結束時間：'+ @strTMP; end
		END
		
		IF @nIndex = 7
		BEGIN
			SET @strTMP =  @strSPLIT
			if (@sTITLE = 'Y') begin set @strTMP = '寬度：'+ @strTMP; end
		END
		
		IF @nIndex = 8
		BEGIN
			SET @strTMP =  @strSPLIT
			if (@sTITLE = 'Y') begin set @strTMP = '高度：'+ @strTMP; end
		END
		
		IF @nIndex = 9
		BEGIN
			SET @strTMP =  @strSPLIT
			if (@sTITLE = 'Y') begin set @strTMP = '浮水印：'+ @strTMP; end
		END	

	end
	
	RETURN @strTMP

END

