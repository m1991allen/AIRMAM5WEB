﻿


-- =============================================
-- 描述:	取出ARC_AUDIO 入庫項目-聲音檔 資料
-- 記錄:	<2011/09/15><Eric.Huang><新增本預存>
--		<2012/05/21><Dennis.Wen><一堆欄位調整>
--		<2012/07/24><Eric.Huang><新增欄位 fnRESP_ID/fnRELA_ID/fnCHRO_ID>
--		<2014/08/21><Eric.Huang><新增 fsKEYWORD/fsOTHINFO/fsOTHTYPE1/fsOTHTYPE2/fsOTHTYPE3>
--		<2016/11/14><David.Sin><增加需要欄位>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ARC_AUDIO]
	@fsFILE_NO		VARCHAR(16),
	@fsSUBJ_ID		VARCHAR(12)
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsMEDIA_PREVIEW_URL VARCHAR(100) = (SELECT fsVALUE FROm tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL')

	;WITH reADMIN(fnDIR_ID,fsPATH_NAME) AS
	(
		SELECT fnDIR_ID,CAST(fsNAME AS VARCHAR(MAX)) FROM tbmDIRECTORIES WHERE fnDIR_ID = 1
		UNION ALL
		SELECT A.fnDIR_ID,CAST(B.fsPATH_NAME + '>' + A.fsNAME AS VARCHAR(MAX)) FROM tbmDIRECTORIES A JOIN reADMIN B ON A.fnPARENT_ID = B.fnDIR_ID
	)

	SELECT 
			
		tbmARC_AUDIO.*,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
		_sSUBJ_PATH = A.fsPATH_NAME,
		--_sFILE_URL_L = dbo.fnGET_FILE_URL_BY_TYPE_AND_FILE_NO('A',fsFILE_NO,'L'),
		_sFILE_URL_L = @fsMEDIA_PREVIEW_URL + 'A/' + REPLACE(REPLACE(tbmARC_AUDIO.fsFILE_PATH_L,(SELECT fsVALUE FROM tbzCONFIG WHERE fsKEY = 'MEDIA_FOLDER_A'),''),'\','/') +
							+ [fsFILE_NO] + '_L.' + fsFILE_TYPE,
		CASE [fcALBUM_PICTURE] 
			WHEN 'Y' THEN 'http:' + REPLACE([fsFILE_PATH],'\','/') + tbmARC_AUDIO.fsFILE_NO + '.jpg?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
			ELSE (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'Template_IMG/audio.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
		END AS _sIMAGE_URL
			
	FROM
		tbmARC_AUDIO 
			JOIN tbmSUBJECT ON tbmARC_AUDIO.fsSUBJECT_ID = tbmSUBJECT.fsSUBJ_ID
			JOIN reADMIN A ON tbmSUBJECT.fnDIR_ID = A.fnDIR_ID
			LEFT JOIN tbmUSERS USERS_CRT ON tbmARC_AUDIO.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
			LEFT JOIN tbmUSERS USERS_UPD ON tbmARC_AUDIO.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		
	WHERE
		(@fsFILE_NO = '' OR fsFILE_NO = @fsFILE_NO) AND
		(@fsSUBJ_ID = '' OR fsSUBJECT_ID = @fsSUBJ_ID)
END


