

-- =============================================
-- 描述:	取出IIS上MEDIA讀取路徑
-- 記錄:	<2011/11/18><Dennis.Wen><新增本函數>
-- 記錄:	<2012/11/12><Eric.Huang><修改本函數 EBC USE>
-- 記錄:	<2012/11/13><Eric.Huang><修改本函數 改由tbzCONFIG取回 MEDIA FOLDER位置>
-- 記錄:	<2013/02/28><Eric.Huang><修改本函數 不要串THUMBNAIL>
-- 記錄:	<2015/02/12><Eric.Huang><從2015/02/13開始,低解/keyframe 存到ams01\newmedia\MAM_MEDIA\>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_MEDIA_URL](

	@FILE_TYPE	VARCHAR(10)
)
returns varchar(200)
as
begin   
----- 2012/11/12 eric start -----	
	
	DECLARE @FILE_URL_S		VARCHAR(200) = ISNULL(dbo.fnGET_MEDIA_URL_BY_PATH((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'MEDIA_FOLDER_S'))),'');		
	DECLARE @FILE_URL_V_L	VARCHAR(200) = ISNULL(dbo.fnGET_MEDIA_URL_BY_PATH((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'MEDIA_FOLDER_V_L'))),'');		
	DECLARE @FILE_URL_V_K	VARCHAR(200) = ISNULL(dbo.fnGET_MEDIA_URL_BY_PATH((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'MEDIA_FOLDER_V_K'))),'');		
	DECLARE @FILE_URL_A		VARCHAR(200) = ISNULL(dbo.fnGET_MEDIA_URL_BY_PATH((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'MEDIA_FOLDER_A'))),'');		
	DECLARE @FILE_URL_P		VARCHAR(200) = ISNULL(dbo.fnGET_MEDIA_URL_BY_PATH((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'MEDIA_FOLDER_P'))),'');		
	DECLARE @FILE_URL_D		VARCHAR(200) = ISNULL(dbo.fnGET_MEDIA_URL_BY_PATH((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'MEDIA_FOLDER_D'))),'');		

	DECLARE @FILE_URL_V_L2	VARCHAR(200) = ISNULL(dbo.fnGET_MEDIA_URL_BY_PATH((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'MEDIA_FOLDER_V_L2'))),'');		
	DECLARE @FILE_URL_V_K2	VARCHAR(200) = ISNULL(dbo.fnGET_MEDIA_URL_BY_PATH((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'MEDIA_FOLDER_V_K2'))),'');		

	DECLARE @fsRETURN_URL NVARCHAR(100);
	
		SET @fsRETURN_URL = CASE @FILE_TYPE	WHEN 'ST'  THEN	@FILE_URL_S   + 'thumbnail/'
										    WHEN 'VT'  THEN	@FILE_URL_V_L --+ 'thumbnail/'	
											WHEN 'VT2' THEN	@FILE_URL_V_L2 --+ 'thumbnail/'	
											WHEN 'VK'  THEN	@FILE_URL_V_K
											WHEN 'VK2' THEN	@FILE_URL_V_K2					 															    																						    
											WHEN 'PT'  THEN	@FILE_URL_P + 'thumbnail/'

										    WHEN 'V'   THEN	@FILE_URL_V_L
											WHEN 'A'   THEN	@FILE_URL_A										    
											WHEN 'P'   THEN	@FILE_URL_P										    											
											WHEN 'D'   THEN	@FILE_URL_D											
							END
	
	
	--DECLARE @FILE_PATH_URL_S NVARCHAR(100);
	--DECLARE @FILE_PATH_URL_ST NVARCHAR(100);
	--DECLARE @FILE_PATH_URL_V NVARCHAR(100);
	--DECLARE @FILE_PATH_URL_VK NVARCHAR(100);
	--DECLARE @FILE_PATH_URL_VT NVARCHAR(100);
	--DECLARE @FILE_PATH_URL_A NVARCHAR(100);
	--DECLARE @FILE_PATH_URL_P NVARCHAR(100);
	--DECLARE @FILE_PATH_URL_PT NVARCHAR(100);
	--DECLARE @FILE_PATH_URL_D NVARCHAR(100);	
	
	--SET @FILE_PATH_URL_ST =  'http://172.20.142.152/s/thumbnail/';		
	--SET @FILE_PATH_URL_VT =  'http://172.20.142.152/v/thumbnail/';
	--SET @FILE_PATH_URL_VK =  'http://172.20.142.152/v_k/v_k/';	
	--SET @FILE_PATH_URL_PT =  'http://172.20.142.152/p_t/';
	
	--SET @FILE_PATH_URL_V  =  'http://172.20.142.152/v/';	
	--SET @FILE_PATH_URL_A  =  'http://172.20.142.152/a/';	
	--SET @FILE_PATH_URL_P  =  'http://172.20.142.152/p/';				
	--SET @FILE_PATH_URL_D  =  'http://172.20.142.152/d/';
	
	--DECLARE @fsRETURN_PATH NVARCHAR(100);
	
	--SET @fsRETURN_PATH = CASE @FILE_TYPE	WHEN 'ST' THEN	@FILE_PATH_URL_ST
	--									    WHEN 'VT' THEN	@FILE_PATH_URL_VT	
	--										WHEN 'VK' THEN	@FILE_PATH_URL_VK																				    																						    
	--										WHEN 'PT' THEN	@FILE_PATH_URL_PT

	--									    WHEN 'V' THEN	@FILE_PATH_URL_V
	--										WHEN 'A' THEN	@FILE_PATH_URL_A										    
	--										WHEN 'P' THEN	@FILE_PATH_URL_P										    											
	--										WHEN 'D' THEN	@FILE_PATH_URL_D	
	
	
	
	----- 2012/11/12 eric end -----									
	
	
	RETURN @fsRETURN_URL
	
	-- 2012/11/12 OLD
	-- RETURN 'http://172.20.142.152/Media_DEV/'	
end


