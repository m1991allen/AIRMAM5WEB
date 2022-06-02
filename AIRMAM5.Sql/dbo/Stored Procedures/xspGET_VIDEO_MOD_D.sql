


-- =============================================
-- 描述:	取出tbxVIDEO_MOD_D 節目片庫MOD_D DATA 主檔 資料
-- 記錄:	<2015/10/11><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_VIDEO_MOD_D]
	@fsPROG_NO	VARCHAR(20),
	@fsSEQ_NO   INT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT [fsPROG_NO]
      ,[fsSEQ_NO]
      ,[fsSUB_NAME]
      ,[fsUNIT_NAME]
      ,[fsPROG_LENGTH]
      ,[fsPROG_EPISODE_S]
      ,[fsPROG_EPISODE_E]
      ,[fsPROG_SUBTITLE]
      ,[fsPROG_DESC]
      ,[fsPROG_DIRECTOR]
      ,[fsPROG_SCRWRITER]
      ,[fsPROG_GUEST]
      ,[fsPROG_PRODUCER]
      ,[fsPROG_HOST]
      ,[fsPROG_OUT_PLACE]
      ,[fsPROG_SONG1]
      ,[fsPROG_SONG2]
      ,[fsPROG_MEMO]
      ,[fsPROG_CHECK]
      ,[fsSPONSOR_TIT]
      ,[fdCREATED_DATE]
      ,[fsCREATED_BY]
      ,[fdUPDATED_DATE]
      ,[fsUPDATED_BY]
  FROM [dbo].[tbxVIDEO_MOD_D]
		WHERE
			(fsPROG_NO = @fsPROG_NO) AND
			 (fsSEQ_NO =  @fsSEQ_NO )

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



