
-- =============================================
-- 描述:	解析WORK的PARAMETERS 字串(新版，加入檔案名稱及上傳說明)
-- 記錄:	<2012/07/23><Eric.Huang><新增預存>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_WORK_PARAMETERS_ANALYZE_UPLOADED_FILE_INFO]
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
	DECLARE @strTYPE1		NVARCHAR(50);	
	DECLARE @strTYPE		VARCHAR(1);
	DECLARE @strFILEINFO	NVARCHAR(100);	
	DECLARE @strFILENO      NVARCHAR(50);	
	DECLARE @strSUBJECTNO   NVARCHAR(50);	
	SELECT @cnt = 0, @i = 0,@strRESULT  = '' , @strSPLIT = '', @strTMP = ''
	
	SELECT @cnt = LEN(REPLACE(@PARAMETERS,';',';;')) - LEN(@PARAMETERS)
	
	WHILE (@cnt > @i)
	BEGIN
	
		SELECT @strSPLIT = dbo.fnGET_ITEM_BY_INDEX(@PARAMETERS,@i)		
		IF @i = 0
		BEGIN
			--SET @strTYPE1 = CASE @strSPLIT 
			--	WHEN 'V' THEN '檔案格式:影片檔'
			--	WHEN 'A' THEN '檔案格式:聲音檔'
			--	WHEN 'P' THEN '檔案格式:圖片檔'
			--	WHEN 'D' THEN '檔案格式:文件檔'
			--	ELSE 'NO'
			--END
			
			SET @strTYPE = CASE @strSPLIT 
				WHEN 'V' THEN 'V'
				WHEN 'A' THEN 'A'
				WHEN 'P' THEN 'P'
				WHEN 'D' THEN 'D'
			END
		END
		
		IF @i = 1
		BEGIN
			--SET @strTMP = '上傳目錄:' + @strSPLIT 
			SET @strFILENO = @strSPLIT
		END

		IF @i = 2
		BEGIN
			--SET @strTMP = '主題檔編號:' + @strSPLIT 
			SET @strSUBJECTNO = @strSPLIT
		END
		
		IF @i = 3
		BEGIN
			--SET @strTMP = CASE @strSPLIT 
			--	WHEN 'HL' THEN  '轉檔格式:高解及低解' 
			--	WHEN 'HLT' THEN '轉檔格式:高解及低解和縮圖' 
			--	WHEN 'HLK' THEN '轉檔格式:高解及低解和關鍵影格' 
			--END
			
			SET @strFILEINFO = CASE @strTYPE
				WHEN 'V' THEN '上傳名稱:' + (SELECT fsTITLE From dbo.tbmARC_VIDEO WHERE fsFILE_NO = @strFILENO and fsSUBJECT_ID = @strSUBJECTNO) + '/ 上傳說明：'+ (SELECT fsDESCRIPTION From dbo.tbmARC_VIDEO WHERE fsFILE_NO = @strFILENO and fsSUBJECT_ID = @strSUBJECTNO)
				WHEN 'A' THEN '上傳名稱:' + (SELECT fsTITLE From dbo.tbmARC_AUDIO WHERE fsFILE_NO = @strFILENO and fsSUBJECT_ID = @strSUBJECTNO) + '/ 上傳說明：'+ (SELECT fsDESCRIPTION From dbo.tbmARC_AUDIO WHERE fsFILE_NO = @strFILENO and fsSUBJECT_ID = @strSUBJECTNO)
				WHEN 'P' THEN '上傳名稱:' + (SELECT fsTITLE From dbo.tbmARC_PHOTO WHERE fsFILE_NO = @strFILENO and fsSUBJECT_ID = @strSUBJECTNO) + '/ 上傳說明：'+ (SELECT fsDESCRIPTION From dbo.tbmARC_PHOTO WHERE fsFILE_NO = @strFILENO and fsSUBJECT_ID = @strSUBJECTNO)
				WHEN 'D' THEN '上傳名稱:' + (SELECT fsTITLE From dbo.tbmARC_DOC   WHERE fsFILE_NO = @strFILENO and fsSUBJECT_ID = @strSUBJECTNO) + '/ 上傳說明：'+ (SELECT fsDESCRIPTION From dbo.tbmARC_DOC   WHERE fsFILE_NO = @strFILENO and fsSUBJECT_ID = @strSUBJECTNO)
			END	
		END

		--IF @i = 0
		--BEGIN
		--	SET @strRESULT = @strTMP + '/'		
		--END
	
		SET @i= @i + 1

	END
			
	SET @strRESULT = @strFILEINFO
	
	RETURN @strRESULT

END


