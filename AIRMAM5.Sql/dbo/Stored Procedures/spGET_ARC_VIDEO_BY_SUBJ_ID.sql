


-- =============================================
-- 描述:	依照SUBJ_ID取出ARC_VIDEO 入庫項目-影片檔主檔 資料
-- 記錄:	<2011/10/21><Dennis.Wen><新增本預存>
--		<2012/05/09><Eric.Huang><增加 _sKF_PATH 欄位>
--			<2012/05/21><Dennis.Wen><一堆欄位調整>
--		<2012/07/24><Eric.Huang><新增欄位 fnRESP_ID/fnRELA_ID/fnCHRO_ID>
-- 記錄:<2014/08/21><Eric.Huang><新增 fsKEYWORD/fsOTHINFO/fsOTHTYPE1/fsOTHTYPE2/fsOTHTYPE3>
-- 記錄:<2014/08/21><Eric.Huang><修改 _sIMAGE_URL 當E5時,就出現審核失敗的圖>
-- 記錄:<2016/03/16><Eric.Huang><ARC_LIST加碼顯示上傳檔案的版本 fnGET_PROG_VER_BY_FILE_NO，>
-- 記錄:<2016/11/14><David.Sin><刪除無須用的欄位>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ARC_VIDEO_BY_SUBJ_ID]
	@fsSUBJ_ID	VARCHAR(12)
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsURL VARCHAR(50) = (SELECT fsVALUE FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') 

	SELECT 	
		fsFILE_NO,fsTITLE, fsDESCRIPTION,

		_sIMAGE_URL = 
			CASE 
				WHEN (SELECT TOP 1 fsSTATUS FROM tblWORK WHERE _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) = '90' THEN 
					CASE 
						WHEN (SELECT COUNT(1) FROM tbmARC_VIDEO_K WHERE fsFILE_NO = tbmARC_VIDEO.fsFILE_NO AND fcHEAD_FRAME = 'Y') = 0 THEN REPLACE(@fsURL,'Media','Images') + 'Template_IMG/video.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
						ELSE (SELECT TOP 1 @fsURL + 'V/K/' + REPLACE(REPLACE([fsFILE_PATH],(SELECT [fsVALUE] FROM tbzCONFIG WHERE fsKEY = 'MEDIA_FOLDER_V_K'),''),'\','/') + [fsFILE_NO] + '_' + [fsTIME] + '.jpg?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5) FROM [dbo].[tbmARC_VIDEO_K] WHERE fsFILE_NO = tbmARC_VIDEO.fsFILE_NO AND fcHEAD_FRAME = 'Y' ORDER BY fsTIME)
					END
				WHEN (SELECT TOP 1 fsSTATUS FROM tblWORK WHERE _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) LIKE 'E%' THEN REPLACE(@fsURL,'Media','Images') + 'tran_error.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
				ELSE REPLACE(@fsURL,'Media','Images') + 'transcoding.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
			END,
		_sFILE_URL_L = 
			CASE 
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) = '90' 
					THEN @fsURL + 'V/L/' + REPLACE(REPLACE(tbmARC_VIDEO.fsFILE_PATH_L,(SELECT fsVALUE FROM tbzCONFIG WHERE fsKEY = 'MEDIA_FOLDER_V_L'),''),'\','/') +
						+ [fsFILE_NO] + '_L.' + fsFILE_TYPE
				ELSE ''
			END,
		_sSUBJ_PATH = dbo.fnGET_SUBJ_PATH_BY_SUBJECT_ID(@fsSUBJ_ID),
		_sKEYFRAME_COUNT = 
			CASE
				WHEN ((SELECT TOP 1 fsSTATUS FROM tblWORK WHERE _ITEM_ID = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') ORDER BY fnWORK_ID DESC) = '90' ) THEN (SELECT COUNT(1) FROM tbmARC_VIDEO_K WHERE fsFILE_NO = tbmARC_VIDEO.fsFILE_NO)
				ELSE 0
			END,
		_sSEGMENT_COUNT = (SELECT COUNT(1) FROM tbmARC_VIDEO_D WHERE fsFILE_NO = tbmARC_VIDEO.fsFILE_NO),
		_sCHANGE = 
			CASE
				WHEN ((SELECT TOP 1 fsSTATUS FROM tblWORK WHERE _ITEM_ID = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') ORDER BY fnWORK_ID DESC) NOT LIKE '2%' ) THEN 'Y'
				ELSE 'N'
			END
				
	FROM
		tbmARC_VIDEO
		
	WHERE
		(fsSUBJECT_ID = @fsSUBJ_ID)
	
	order by fsFILE_NO

END



