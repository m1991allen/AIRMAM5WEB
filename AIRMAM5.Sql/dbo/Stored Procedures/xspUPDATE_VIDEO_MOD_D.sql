




-- =============================================
-- 描述:修改tbxVIDEO_MOD_D 節目片庫MOD_D DATA 主檔 資料
-- 記錄:	<2015/10/11><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_VIDEO_MOD_D]


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
     @fsUPDATED_BY		nvarchar(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			[tbxVIDEO_MOD_D]
		SET 
		    @fsPROG_NO        = fsPROG_NO         ,
			@fsSEQ_NO 		  =	fsSEQ_NO 		  ,
			@fsSUB_NAME		  =	fsSUB_NAME		  ,
			@fsUNIT_NAME	  =	fsUNIT_NAME	  	  ,
			@fsPROG_LENGTH	  =	fsPROG_LENGTH	  ,
			@fsPROG_EPISODE_S =	fsPROG_EPISODE_S  ,
			@fsPROG_EPISODE_E =	fsPROG_EPISODE_E  ,
			@fsPROG_SUBTITLE  =	fsPROG_SUBTITLE   ,
			@fsPROG_DESC	  =	fsPROG_DESC	  	  ,
			@fsPROG_DIRECTOR  =	fsPROG_DIRECTOR   ,
			@fsPROG_SCRWRITER =	fsPROG_SCRWRITER  ,
			@fsPROG_GUEST 	  =	fsPROG_GUEST 	  ,
			@fsPROG_PRODUCER  =	fsPROG_PRODUCER   ,
			@fsPROG_HOST	  =	fsPROG_HOST	  	  ,
			@fsPROG_OUT_PLACE =	fsPROG_OUT_PLACE  ,
			@fsPROG_SONG1	  =	fsPROG_SONG1	  ,
			@fsPROG_SONG2	  =	fsPROG_SONG2	  ,
			@fsPROG_MEMO 	  =	fsPROG_MEMO 	  ,
			@fsPROG_CHECK 	  =	fsPROG_CHECK 	  ,
			@fsSPONSOR_TIT	  =	fsSPONSOR_TIT	  ,
			@fsUPDATED_BY	  =	fsUPDATED_BY	  

		WHERE
		
			(fsPROG_NO = @fsPROG_NO) AND 
			 fsSEQ_NO = @fsSEQ_NO
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END







