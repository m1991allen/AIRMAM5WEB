

-- =============================================
-- 描述:	取出關鍵影格圖片檔路徑
-- 記錄:	<2011/11/01><Dennis.Wen><新增本函數>
--			<2012/05/21><Dennis.Wen><一堆欄位調整>
-- 範例:	V => http://172.20.142.152/Media/[主題編號]/V/keyframe/[FILE_NO]/[FILE_NO]_[TIMECODE8碼].jpg
-- 記錄:	<2012/11/12><Eric.Huang><修改本函數 EBC USE>
-- 記錄:	<2013/02/28><Eric.Huang><修改本函數 路徑加串年月日>
-- 記錄:	<2015/02/12><Eric.Huang><從2015/02/13開始,低解/keyframe 存到ams01\newmedia\MAM_MEDIA\>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_KEYFRAME_IMAGE_URL_BY_FILE_NO_AND_TIME](
	@FILE_NO	VARCHAR(16),
	@TIME		   VARCHAR(16)
)
RETURNS VARCHAR(500)
AS
BEGIN   
	DECLARE @_sIMAGE_URL	VARCHAR(500),
			@fsSUBJECT_ID	VARCHAR(12)

	SET @fsSUBJECT_ID = (SELECT fsSUBJECT_ID FROM tbmARC_VIDEO WHERE (fsFILE_NO = @FILE_NO))			

	DECLARE @YYYY VARCHAR(4) = (select substring(@FILE_NO,1,4)),
			@MM   VARCHAR(2) = (select substring(@FILE_NO,5,2)),
			@DD   VARCHAR(2) = (select substring(@FILE_NO,7,2))

	SET @_sIMAGE_URL = dbo.fnGET_MEDIA_URL('VK') + @YYYY + '/' + @MM + '/' + @DD + '/'  + @FILE_NO + '/' + @FILE_NO + '_' + @TIME + '.jpg'
	 
	RETURN @_sIMAGE_URL
END
