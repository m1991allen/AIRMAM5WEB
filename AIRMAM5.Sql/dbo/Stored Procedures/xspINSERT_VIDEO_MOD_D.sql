


-- =============================================
-- 描述:	新增tbxVIDEO_MOD 節目片庫MOD_D DATA 主檔 資料
-- 記錄:	<2015/10/11><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_VIDEO_MOD_D]

	 @fsPROG_NO varchar(20)        	 ,
     @fsSEQ_NO int					 ,
     @fsSUB_NAME nvarchar(200)		 ,
     @fsUNIT_NAME nvarchar(500)		 ,
     @fsPROG_LENGTH nvarchar(50)	 ,
     @fsPROG_EPISODE_S int			 ,
     @fsPROG_EPISODE_E int			 ,
     @fsPROG_SUBTITLE varchar(20)	 ,
     @fsPROG_DESC nvarchar(300)		 ,
     @fsPROG_DIRECTOR nvarchar(50)	 ,
     @fsPROG_SCRWRITER nvarchar(50)	 ,
     @fsPROG_GUEST nvarchar(1000)	 ,
     @fsPROG_PRODUCER nvarchar(50)	 ,
	 @fsPROG_HOST nvarchar(200)		 ,
	 @fsPROG_OUT_PLACE nvarchar(1000)	 ,
	 @fsPROG_SONG1 nvarchar(200)	 ,
	 @fsPROG_SONG2 nvarchar(200)	 ,
	 @fsPROG_MEMO nvarchar(max)		 ,
	 @fsPROG_CHECK varchar(2)		 ,
	 @fsSPONSOR_TIT nvarchar(20)	 ,
	 @fsCREATED_BY nvarchar(50)		 

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT INTO [dbo].[tbxVIDEO_MOD_D]
           ([fsPROG_NO]
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
           ,[fsCREATED_BY])

     VALUES
           (@fsPROG_NO,	@fsSEQ_NO, @fsSUB_NAME, @fsUNIT_NAME,
            @fsPROG_LENGTH, @fsPROG_EPISODE_S, @fsPROG_EPISODE_E, 
			@fsPROG_SUBTITLE, @fsPROG_DESC, @fsPROG_DIRECTOR,
			@fsPROG_SCRWRITER, @fsPROG_GUEST, @fsPROG_PRODUCER,
			@fsPROG_HOST,@fsPROG_OUT_PLACE,@fsPROG_SONG1,
			@fsPROG_SONG2,@fsPROG_MEMO, @fsPROG_CHECK,@fsSPONSOR_TIT,
			GETDATE(),	@fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





