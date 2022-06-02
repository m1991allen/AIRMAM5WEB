


-- =============================================
-- 描述:	取出IIS上MEDIA讀取路徑
-- 記錄:	<2011/11/18><Dennis.Wen><新增本函數>
-- 記錄:	<2012/02/05><Eric.Huang><修改本函數> 將 .21的實體位置指到.3
-- 記錄:	<2012/03/16><Eric.Huang><修改本函數> 將 .21的實體位置指到.2 (阿福說的,它會自己指到.3 or .4)
-- 記錄:	<2012/03/16><Eric.Huang><修改本函數> 將 "\\MAM\mamdfs\MAM_TEMP\MAM_UPLOAD\" 轉成 "MAM\UploadFolder\",低解播放用)
-- 記錄:	<2015/02/12><Eric.Huang><從2015/02/13開始,低解存到ams01\newmedia\MAM_MEDIA\>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_MEDIA_URL_BY_PATH](
	@PATH VARCHAR(100)
)
returns varchar(100)
as
begin   
	DECLARE @strRESULT VARCHAR(100) = ''
	
	SET @strRESULT = REPLACE(@PATH,'\\','http://')
	--SET @strRESULT = REPLACE(@strRESULT,'10.3.26.21','MAM')
	--SET @strRESULT = REPLACE(@strRESULT,'AIRMAM\mamdfs\MAM_TEMP\MAM_UPLOAD\','AIRMAM\UploadFolder\')
	--SET @strRESULT = REPLACE(@strRESULT,'172.30.101.70','172.30.101.53')

	--2015/02/12 eric test newmedia path

	SET @strRESULT = REPLACE(@strRESULT,'172.20.142.35','172.20.142.35/AIRMAM5')
	--2015/02/12 eric test newmedia path

	--SET @strRESULT = REPLACE(@strRESULT,'MAM_MEDIA','MEDIA')
	--SET @strRESULT = REPLACE(@strRESULT,'AMS01','AIRMAM')
	--SET @strRESULT = REPLACE(@strRESULT,'AMS01','172.20.144.55')
	SET @strRESULT = REPLACE(@strRESULT,'\','/')

	return @strRESULT
end




