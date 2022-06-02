


-- =============================================
-- 描述:	依照FILE_NO取出ARC_VIDEO 入庫項目-影片檔主檔 資料
-- 記錄:	<2012/05/15><Mihsiu.Chiu><複製新增本預存>
--			<2012/05/21><Dennis.Wen><一堆欄位調整>
--			<2012/10/03><Albert.Chen><新增fnCHRO_ID欄位>
--			<2014/08/21><Eric.Huang><新增 fsKEYWORD/fsOTHINFO/fsOTHTYPE1/fsOTHTYPE2/fsOTHTYPE3>
-- =============================================
CREATE PROCEDURE [dbo].[xsp_t_view_GET_ARC_VIDEO_BY_FILE_NO]
	@fsFILE_NO		VARCHAR(16),
	@fnINDEX_ID		INT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 				
			t_tbmARC_VIDEO.*,
			
			_sFILE_URL = dbo.fn_t_view_GET_FILE_URL_BY_TYPE_AND_FILE_NO('V',fsFILE_NO,''),
			_sFILE_URL_H = dbo.fn_t_view_GET_FILE_URL_BY_TYPE_AND_FILE_NO('V',fsFILE_NO,'H'),
			_sFILE_URL_L = dbo.fn_t_view_GET_FILE_URL_BY_TYPE_AND_FILE_NO('V',fsFILE_NO,'L'),
			
			_sIMAGE_URL = dbo.fn_t_view_GET_IMAGE_URL_BY_TYPE_AND_FILE_NO('V',fsFILE_NO),
			_sFILE_INFO = dbo.fnGET_FILE_INFO_BY_DataType_AND_FILE_NO('V',fsFILE_NO),
			
			_sDIR_PATH = dbo.fnGET_DIR_PATH_BY_SUBJECT_ID(fsSUBJECT_ID),
			_sSUBJ_PATH = dbo.fnGET_SUBJ_PATH_BY_SUBJECT_ID(fsSUBJECT_ID),
			_sKF_PATH	= dbo.fnGET_ARC_FILE_PATH(fsSUBJECT_ID,fsFILE_NO,'','K')
		FROM
			t_tbmARC_VIDEO
			
		WHERE
			(fsFILE_NO = @fsFILE_NO)

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



