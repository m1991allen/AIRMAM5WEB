

-- =============================================
-- 描述:	取出ARC_VIDEO_D 入庫項目-影片明細檔 資料
-- 記錄:	<2011/09/15><Eric.Huang><新增本預存>
--		<2011/11/09><Eric.Huang><增加一組KEY>
--		<2012/05/21><Dennis.Wen><一堆欄位調整>
--		<2012/07/24><Eric.Huang><新增欄位 fnRESP_ID>
-- 記錄:<2014/08/21><Eric.Huang><新增 fsKEYWORD>
-- 記錄:<2016/11/14><David.Sin><調整欄位>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ARC_VIDEO_D]
	@fsFILE_NO	VARCHAR(16),
	@fnSEQ_NO   INT	
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			tbmARC_VIDEO_D.[fsFILE_NO],
			tbmARC_VIDEO_D.[fnSEQ_NO],
			tbmARC_VIDEO_D.[fsDESCRIPTION],
			tbmARC_VIDEO_D.[fdBEG_TIME],
			tbmARC_VIDEO_D.[fdEND_TIME],
			tbmARC_VIDEO_D.[fdCREATED_DATE],
			tbmARC_VIDEO_D.[fsCREATED_BY],
			tbmARC_VIDEO_D.[fdUPDATED_DATE],
			tbmARC_VIDEO_D.[fsUPDATED_BY],
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME
		FROM
			tbmARC_VIDEO_D
				LEFT JOIN tbmUSERS USERS_CRT ON tbmARC_VIDEO_D.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON tbmARC_VIDEO_D.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE
			(@fsFILE_NO = '' OR fsFILE_NO = @fsFILE_NO) AND
			(@fnSEQ_NO = 0 OR fnSEQ_NO  = @fnSEQ_NO)
END


