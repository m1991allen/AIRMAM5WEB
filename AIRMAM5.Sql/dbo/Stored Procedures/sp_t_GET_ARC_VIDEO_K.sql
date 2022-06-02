


-- =============================================
-- 描述:	取出暫時刪除的-影片關鍵影格檔 資料
-- 記錄:	<2019/09/17><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[sp_t_GET_ARC_VIDEO_K]
	@fnINDEX_ID		BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsURL VARCHAR(50) = (SELECT [dbo].[tbzCONFIG].[fsVALUE] FROM [dbo].[tbzCONFIG] WHERE [dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_PREVIEW_URL') 

		SELECT 
			[dbo].[t_tbmARC_VIDEO_K].[fsFILE_NO],
			[dbo].[t_tbmARC_VIDEO_K].[fsTITLE] ,
			[dbo].[t_tbmARC_VIDEO_K].[fsDESCRIPTION],
			[dbo].[t_tbmARC_VIDEO_K].[fsFILE_PATH]	, 
			[dbo].[t_tbmARC_VIDEO_K].[fsFILE_SIZE], 
			[dbo].[t_tbmARC_VIDEO_K].[fsFILE_TYPE], 
			CASE [dbo].[t_tbmARC_VIDEO_K].[fcHEAD_FRAME] WHEN 'Y' THEN 'Y' ELSE 'N' END AS fcHEAD_FRAME,
			[dbo].[t_tbmARC_VIDEO_K].[fdCREATED_DATE], 
			[dbo].[t_tbmARC_VIDEO_K].[fsCREATED_BY] , 
			[dbo].[t_tbmARC_VIDEO_K].[fdUPDATED_DATE], 
			[dbo].[t_tbmARC_VIDEO_K].[fsUPDATED_BY],
			[dbo].[t_tbmARC_VIDEO_K].[fsTIME],
			_sIMAGE_URL = 
				CASE
					WHEN [dbo].[t_tbmARC_VIDEO_K].[fsFILE_PATH] = '' THEN ''
					ELSE dbo.fnGET_KEYFRAME_IMAGE_URL_BY_FILE_NO_AND_TIME([dbo].[t_tbmARC_VIDEO_K].[fsFILE_NO], [dbo].[t_tbmARC_VIDEO_K].[fsTIME]) + '?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
				END,
			_sFILE_INFO	= dbo.fnGET_FILE_INFO_BY_DataType_AND_FILE_NO('K',[dbo].[t_tbmARC_VIDEO_K].[fsFILE_NO]),
			_sVIDEO_MAX_TIME = (SELECT [dbo].[t_tbmARC_VIDEO].[fdDURATION] FROM [dbo].[t_tbmARC_VIDEO] WHERE ([dbo].[t_tbmARC_VIDEO].[fsFILE_NO] = (SELECT [dbo].[t_tbmARC_INDEX].[fsFILE_NO] FROM [dbo].[t_tbmARC_INDEX] WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID)))
		FROM
			[dbo].[t_tbmARC_VIDEO_K]
				JOIN [dbo].[t_tbmARC_INDEX] IDX ON [dbo].[t_tbmARC_VIDEO_K].[fsFILE_NO] = IDX.fsFILE_NO
		WHERE
			IDX.fnINDEX_ID = @fnINDEX_ID
END


