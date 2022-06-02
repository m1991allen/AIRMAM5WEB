


-- =============================================
-- 描述:	依照SUBJ_ID取出ARC_DOC 入庫項目-圖片檔 資料
-- 記錄:	<2011/10/21><Dennis.Wen><新增本預存>
--     	<2011/11/17><Eric.Huang><新增欄位>
--		<2012/07/24><Eric.Huang><新增欄位 fnRESP_ID/fnRELA_ID/fnCHRO_ID>
--		<2012/07/24><Eric.Huang><新增欄位 _sEXTRACT>
--		<2014/08/21><Eric.Huang><新增 fsKEYWORD/fsOTHINFO/fsOTHTYPE1/fsOTHTYPE2/fsOTHTYPE3>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ARC_DOC_BY_SUBJ_ID]
	@fsSUBJ_ID	VARCHAR(12)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 	
		fsFILE_NO,fsTITLE, fsDESCRIPTION,
		_sIMAGE_URL = 
			CASE 
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) LIKE 'E%' THEN (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'tran_error.png'
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) = '90' THEN (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'Template_IMG/doc.png'
				ELSE (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'transcoding.png'--dbo.fnGET_IMAGE_URL_BY_TYPE_AND_FILE_NO('D',fsFILE_NO)
			END,
		_sSUBJ_PATH = dbo.fnGET_SUBJ_PATH_BY_SUBJECT_ID(@fsSUBJ_ID)		

	FROM
		tbmARC_DOC --LEFT JOIN tblWORK ON tbmARC_DOC.fsFILE_NO = tblWORK._ITEM_ID
		
	WHERE
		(fsSUBJECT_ID = @fsSUBJ_ID)
	
	ORDER BY 
		fsFILE_NO
END


