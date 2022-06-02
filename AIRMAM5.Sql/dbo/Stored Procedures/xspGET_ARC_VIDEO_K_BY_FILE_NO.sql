

-- =============================================
-- 描述:	依FILE_NO取出ARC_VIDEO_K 入庫項目-影片關鍵影格檔 資料
-- 記錄:	<2011/11/25><Dennis.Wen><新增本預存>
--		<2012/05/21><Dennis.Wen><一堆欄位調整>
--		<2012/07/24><Eric.Huang><新增欄位 fnRESP_ID>
--		<2013/10/24><Albert.Chen><修改Title顯示時間方式>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_ARC_VIDEO_K_BY_FILE_NO]
	@fsFILE_NO	VARCHAR(16)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			fsFILE_NO, /*fsTIME_CODE,*/fsTIME, fsTITLE ,fsDESCRIPTION,--201205一堆欄位調整
				----START 2013/10/24 Add By Albert 修改Title顯示時間方式
				--fsFILE_NO, fsTIME,LEFT(fsTITLE,CHARINDEX('=',fsTITLE)) + ' ' +  dbo.fnGET_TIMECODE_FROM_SECONDS(LTRIM(RIGHT(fsTITLE,CHARINDEX('=',fsTITLE))))
				--AS fsTITLE ,fsDESCRIPTION,--201205一堆欄位調整
				----END   2013/10/24 Add By Albert 修改Title顯示時間方式
			fsFILE_PATH	, fsFILE_SIZE, fsFILE_TYPE,
			fdCREATED_DATE, fsCREATED_BY , fdUPDATED_DATE, fsUPDATED_BY,
					
			--_sIMAGE_URL = dbo.fnGET_KEYFRAME_IMAGE_URL_BY_FILE_NO_AND_TIME_CODE(fsFILE_NO, fsTIME_CODE),
			_sIMAGE_URL = dbo.fnGET_KEYFRAME_IMAGE_URL_BY_FILE_NO_AND_TIME(fsFILE_NO, fsTIME),
			_sFILE_INFO	= dbo.fnGET_FILE_INFO_BY_DataType_AND_FILE_NO('K',fsFILE_NO)
			,_sVIDEO_MAX_TIME = (SELECT fdDURATION FROM tbmARC_VIDEO WHERE (fsFILE_NO = @fsFILE_NO))
		FROM
			tbmARC_VIDEO_K			
		WHERE
			(fsFILE_NO = @fsFILE_NO)
		ORDER BY
			--fsTIME_CODE
			fsTIME

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


