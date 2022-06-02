
-- =============================================
-- 描述:	取出關鍵影格資料
-- 記錄:	<2011/12/05><Dennis.Wen><新增本預存>
-- 　　　　　<2011/12/05><Eric.Huang><加入SUBJECT ID>
-- 　　　　　<2011/12/06><Eric.Huang><移除SUBJECT ID,此sp供取video縮圖用>
--			<2012/05/21><Dennis.Wen><一堆欄位調整>
-- 記錄:	<2012/11/12><Eric.Huang><修改本函數 EBC USE>
-- 記錄:	<2012/11/13><Eric.Huang><修改本函數 改由tbzCONFIG取回 MEDIA FOLDER位置>
-- 記錄:	<2012/02/28><Eric.Huang><修改本函數 @PATH_VIDEO_THUMB>
-- 記錄:	<2015/02/12><Eric.Huang><從2015/02/13開始,低解存到ams01\newmedia\MAM_MEDIA\>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_KEYFRAME_INFO_BY_FILE_NO_AND_TIME]
	@fsFILE_NO	VARCHAR(16),
	@fsTIME		VARCHAR(16)
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @YYYY VARCHAR(4) = (select substring(@fsFILE_NO,1,4)),
			@MM   VARCHAR(2) = (select substring(@fsFILE_NO,5,2)),
			@DD   VARCHAR(2) = (select substring(@fsFILE_NO,7,2))


	DECLARE @PATH VARCHAR(100) = ( SELECT fsFILE_PATH 
								FROM tbmARC_VIDEO_K 
								WHERE (fsFILE_NO = @fsFILE_NO) AND (fsTIME = @fsTIME))
	DECLARE @FILE_PATH_V_L	VARCHAR(200) 
	IF	(@fsFILE_NO >= '20150213_0000000')
		BEGIN
			SET @FILE_PATH_V_L = ISNULL((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'MEDIA_FOLDER_V_L2')),'');		
		END
	ELSE
		BEGIN
			SET @FILE_PATH_V_L = ISNULL((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'MEDIA_FOLDER_V_L')),'');		
		END

	DECLARE @PATH_VIDEO_THUMB VARCHAR(100) = @FILE_PATH_V_L	+ @YYYY + '\' + @MM + '\' + @DD + '\' + 'thumbnail\'	
	
	DECLARE @URL VARCHAR(100) = dbo.fnGET_MEDIA_URL_BY_PATH(@PATH)

	DECLARE @TIME_N VARCHAR(16) = @fsTIME

	SET @TIME_N = REPLACE(@TIME_N, ':', '')
	SET @TIME_N = REPLACE(@TIME_N, ';', '')

	SELECT
				fsFILE_NO, fsTIME, fsTITLE ,fsDESCRIPTION,
				fsFILE_PATH	, fsFILE_SIZE, fsFILE_TYPE,
				fdCREATED_DATE, fsCREATED_BY , fdUPDATED_DATE, fsUPDATED_BY,
				_sFILE_PATH = @PATH + @fsFILE_NO + '_' + @TIME_N + '.jpg',
				_sFILE_PATH_VIDEO_THUMB = @PATH_VIDEO_THUMB + @fsFILE_NO + '_thumb.jpg', 
				_sFILE_PATH_VDO_THUMB_LOC = @PATH_VIDEO_THUMB  					
	FROM
		tbmARC_VIDEO_K
	WHERE
		(fsFILE_NO = @fsFILE_NO) AND
		(fsTIME = @fsTIME)
END



