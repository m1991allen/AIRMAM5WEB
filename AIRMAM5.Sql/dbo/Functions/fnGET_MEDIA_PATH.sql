
-- =============================================
-- 描述:	取出IIS上MEDIA讀取路徑
-- 記錄:	<2011/11/18><Dennis.Wen><新增本函數>
-- 記錄:	<2012/11/12><Eric.Huang><修改本函數 EBC USE>
-- 記錄:	<2012/11/13><Eric.Huang><修改本函數 改由tbzCONFIG取回 MEDIA FOLDER ROOT位置>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_MEDIA_PATH]()
returns varchar(100)
as
begin   
	----- 2012/11/12 eric start -----	
	--RETURN '\\172.20.142.152\Media_DEV\'
	--RETURN '\\172.20.142.152\Media_DEV2\'
	
	RETURN ISNULL((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'MEDIA_FOLDER_ROOT')),'')		
	----- 2012/11/12 eric end -----		
end


