

-- =============================================
-- 描述:	新增 L_SRH檔後的觸發程式=>新增[tblSRH_KW]
-- 記錄:	<2012/01/20><Eric.Huang><新增本使用者函數>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_L_SRH_KEYWORD_BY_STATEMENT](

    @strTEXT	NVARCHAR(100)
)
RETURNS NVARCHAR(500)
AS
BEGIN

	----判斷 @strTEXT 中是否有'&|!'
	IF (@strTEXT like '%[&|!]%') OR (@strTEXT like '%[&|!]') OR (@strTEXT like '[&|!]%')  
		BEGIN
			SET @strTEXT = REPLACE(@strTEXT,'&',';')
			SET @strTEXT = REPLACE(@strTEXT,'|',';') 					
			SET @strTEXT = REPLACE(@strTEXT,'!',';')
			SET @strTEXT = REPLACE(@strTEXT,' ','') 				 					
		END
	ELSE
		BEGIN
			SET @strTEXT = REPLACE(@strTEXT,' ','') 			
		END			

	--判斷 @strTEXT 中是否有';;'
	IF (@strTEXT like '%;;%')
		BEGIN
			SET @strTEXT = REPLACE(@strTEXT,';;',';')
		END

	--將字串反轉
	SET @strTEXT = REVERSE(@strTEXT)
	----如果第一個字元不是;就加入;
	IF (CHARINDEX(';', @strTEXT) <> 1)
	BEGIN
		SET @strTEXT = ';' + @strTEXT 
	END

	--將字串反轉回來
	SET @strTEXT = REVERSE(@strTEXT)

	RETURN @strTEXT
	
END


