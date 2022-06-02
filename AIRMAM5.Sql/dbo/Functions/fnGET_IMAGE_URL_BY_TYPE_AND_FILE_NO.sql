

-- =============================================
-- 描述:	取出預覽縮圖路徑
-- 記錄:	<2011/11/01><Dennis.Wen><新增本函數>
-- 範例:	S => http://172.20.142.152/Media/[主題編號]/S/thumbnail/[主題編號]_thumb.jpg
--			V => http://172.20.142.152/Media/[主題編號]/V/thumbnail/[FILE_NO]_thumb.jpg
--			A => http://172.20.142.152/Media/[主題編號]/S/thumbnail/[主題編號]_thumb.jpg (跟S相同)
--			P => http://172.20.142.152/Media/[主題編號]/P/thumbnail/[FILE_NO]_thumb.jpg
--			D => http://172.20.142.152/Media/[主題編號]/S/thumbnail/[主題編號]_thumb.jpg (跟S相同)
-- 記錄:	<2012/11/12><Eric.Huang><修改本函數 EBC USE>
-- 記錄:	<2013/02/18><Eric.Huang><修改videno thumbnail路徑>
-- 記錄:	<2015/02/12><Eric.Huang><從2015/02/13開始,低解存到ams01\newmedia\MAM_MEDIA\>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_IMAGE_URL_BY_TYPE_AND_FILE_NO](
	@TYPE		VARCHAR(1),
	@FILE_NO	VARCHAR(16)
)
RETURNS VARCHAR(500)
AS
BEGIN   

	DECLARE @_sIMAGE_URL	VARCHAR(500),
			@fsSUBJECT_ID	VARCHAR(12),
			@fsFILE_PATH	VARCHAR(100),
			@fsFILE_TYPE	VARCHAR(10)
				
		DECLARE @YYYY VARCHAR(4) = (select substring(@FILE_NO,1,4)),
				@MM   VARCHAR(2) = (select substring(@FILE_NO,5,2)),
				@DD   VARCHAR(2) = (select substring(@FILE_NO,7,2))

	IF (@TYPE = 'S')
	BEGIN
		--@TYPE = 'S'時, @FILE_NO丟入fsSUBJECT_ID
		SET @fsSUBJECT_ID = @FILE_NO
		
	----- 2012/11/12 eric start -----			
		--SET @_sIMAGE_URL = dbo.fnGET_MEDIA_URL() + @fsSUBJECT_ID + '/S/thumbnail/' + @fsSUBJECT_ID + '_thumb.jpg'
		SET @_sIMAGE_URL = dbo.fnGET_MEDIA_URL('ST') + @fsSUBJECT_ID + '_thumb.jpg'		
	----- 2012/11/12 eric end -----			
	END
	
	ELSE IF (@TYPE = 'V')
	BEGIN



		SELECT @fsSUBJECT_ID = fsSUBJECT_ID, @fsFILE_PATH = fsFILE_PATH, @fsFILE_TYPE = fsFILE_TYPE
		FROM tbmARC_VIDEO WHERE (fsFILE_NO = @FILE_NO)
		
		SET @_sIMAGE_URL = dbo.fnGET_MEDIA_URL('VT') + @YYYY + '/' + @MM + '/' + @DD + '/' + 'thumbnail' + '/' + @FILE_NO + '_thumb.jpg'	
	END
	
	ELSE IF (@TYPE = 'A')
	BEGIN
		SELECT @fsSUBJECT_ID = fsSUBJECT_ID, @fsFILE_PATH = fsFILE_PATH, @fsFILE_TYPE = fsFILE_TYPE
		FROM tbmARC_AUDIO WHERE (fsFILE_NO = @FILE_NO)
		
	----- 2012/11/12 eric start -----					
		--SET @_sIMAGE_URL = dbo.fnGET_MEDIA_URL() + @fsSUBJECT_ID + '/S/thumbnail/' + @fsSUBJECT_ID + '_thumb.jpg'
		SET @_sIMAGE_URL = dbo.fnGET_MEDIA_URL('ST') + @fsSUBJECT_ID + '_thumb.jpg'		
	----- 2012/11/12 eric end -----	
			
	END --本身沒有縮圖, 一樣讀取S縮圖
	
	ELSE IF (@TYPE = 'P')
	BEGIN
		SELECT @fsSUBJECT_ID = fsSUBJECT_ID, @fsFILE_PATH = fsFILE_PATH, @fsFILE_TYPE = fsFILE_TYPE
		FROM tbmARC_PHOTO WHERE (fsFILE_NO = @FILE_NO)
		
	----- 2012/11/12 eric start -----					
		--SET @_sIMAGE_URL = dbo.fnGET_MEDIA_URL() + @fsSUBJECT_ID + '/P/thumbnail/' + @FILE_NO + '_thumb.jpg'
		--SET @_sIMAGE_URL = dbo.fnGET_MEDIA_URL('PT') + @FILE_NO + '_thumb.jpg'
		SET @_sIMAGE_URL = dbo.fnGET_MEDIA_URL('PT') + @YYYY + '/' + @MM + '/' + @DD + '/' + @FILE_NO + '_thumb.jpg'
		
	----- 2012/11/12 eric end -----	
			
	END
	
	ELSE IF (@TYPE = 'D')
	BEGIN
		SELECT @fsSUBJECT_ID = fsSUBJECT_ID, @fsFILE_PATH = fsFILE_PATH, @fsFILE_TYPE = fsFILE_TYPE
		FROM tbmARC_DOC WHERE (fsFILE_NO = @FILE_NO)
		
	----- 2012/11/12 eric start -----					
		--SET @_sIMAGE_URL = dbo.fnGET_MEDIA_URL() + @fsSUBJECT_ID + '/S/thumbnail/' + @fsSUBJECT_ID + '_thumb.jpg'
		SET @_sIMAGE_URL = dbo.fnGET_MEDIA_URL('ST') + @fsSUBJECT_ID + '_thumb.jpg'		
	----- 2012/11/12 eric end -----	
			
	END --本身沒有縮圖, 一樣讀取S縮圖
	 
	RETURN @_sIMAGE_URL
END
