


-- =============================================
-- 描述:	依照SUBJ_ID取出ARC_AUDIO 入庫項目-聲音檔 資料
-- 記錄:	<2011/10/21><Dennis.Wen><新增本預存>
--      <2011/11/16><Eric.Huang><新增高解低解欄位>
--		<2012/05/21><Dennis.Wen><一堆欄位調整>
--		<2012/07/24><Eric.Huang><新增欄位 fnRESP_ID/fnRELA_ID/fnCHRO_ID>
--		<2014/08/21><Eric.Huang><新增 fsKEYWORD/fsOTHINFO/fsOTHTYPE1/fsOTHTYPE2/fsOTHTYPE3>
-- 記錄:<2016/11/14><David.Sin><刪除無須用的欄位>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ARC_AUDIO_BY_SUBJ_ID]
	@fsSUBJ_ID	VARCHAR(12)
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsMEDIA_PREVIEW_URL VARCHAR(100) = (SELECT fsVALUE FROm tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL')

	SELECT 	
		fsFILE_NO,fsTITLE, fsDESCRIPTION,fsFILE_PATH_H,
		_sIMAGE_URL = 
			CASE 
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) LIKE 'E%' THEN (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'tran_error.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) = '90' THEN 
					CASE [fcALBUM_PICTURE] 
						WHEN 'Y' THEN 'http:' + REPLACE([fsFILE_PATH],'\','/') + tbmARC_AUDIO.fsFILE_NO + '.jpg?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5) 
						ELSE (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'Template_IMG/audio.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
					END
				ELSE (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'transcoding.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
			END,
		_sFILE_URL_L = 
			CASE 
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) = '90' 
					THEN @fsMEDIA_PREVIEW_URL + 'A/' + REPLACE(REPLACE(tbmARC_AUDIO.fsFILE_PATH_L,(SELECT fsVALUE FROM tbzCONFIG WHERE fsKEY = 'MEDIA_FOLDER_A'),''),'\','/') +
							+ [fsFILE_NO] + '_L.' + fsFILE_TYPE
				ELSE ''
			END,
		_sSUBJ_PATH = dbo.fnGET_SUBJ_PATH_BY_SUBJECT_ID(@fsSUBJ_ID),		
		_sCHANGE = 
			CASE
				WHEN ((SELECT TOP 1 fsSTATUS FROM tblWORK WHERE _ITEM_ID = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') ORDER BY fnWORK_ID DESC) NOT LIKE '2%' ) THEN 'Y'
				ELSE 'N'
			END
	FROM
		tbmARC_AUDIO --LEFT JOIN tblWORK ON tbmARC_AUDIO.fsFILE_NO = tblWORK._ITEM_ID
		
	WHERE
		(fsSUBJECT_ID = @fsSUBJ_ID)
	
	ORDER BY 
		fsFILE_NO
END



