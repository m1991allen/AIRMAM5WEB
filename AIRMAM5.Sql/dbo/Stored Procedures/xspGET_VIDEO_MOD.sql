


-- =============================================
-- 描述:	取出tbxVIDEO_MOD 節目片庫MOD_DATA 主檔 資料
-- 記錄:	<2015/10/11><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_VIDEO_MOD]
	@fsPROG_NO	VARCHAR(20)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT	fsPROG_NO, fsPROG_NAME, fsPROG_TYPE, fsPROG_SUBTYPE, fsPROG_VER, fsPROD_TYPE, 
                fsPROD_COMPANY, fsPROG_GRADE, fsVDO_SPEC, fsPROG_LENG, fsPROG_FIRST_RUN, fsPROG_DESC, 
                fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY

		FROM
			tbxVIDEO_MOD
		WHERE
			(fsPROG_NO = @fsPROG_NO)

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



