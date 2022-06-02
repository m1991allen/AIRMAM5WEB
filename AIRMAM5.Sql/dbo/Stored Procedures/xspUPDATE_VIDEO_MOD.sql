




-- =============================================
-- 描述:修改tbxVIDEO_MOD 節目片庫MOD_DATA 主檔 資料
-- 記錄:	<2015/10/11><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_VIDEO_MOD]

	 @fsPROG_NO			varchar(20),
     @fsPROG_NAME		nvarchar(100),
     @fsPROG_TYPE		nvarchar(20),
     @fsPROG_SUBTYPE	nvarchar(20),
     @fsPROG_VER		nvarchar(20),
     @fsPROD_TYPE		nvarchar(20),
     @fsPROD_COMPANY	nvarchar(500),
     @fsPROG_GRADE		nvarchar(20),
     @fsVDO_SPEC		nvarchar(20),
     @fsPROG_LENG		nvarchar(10),
     @fsPROG_FIRST_RUN  datetime,
     @fsPROG_DESC		nvarchar(1000),
     @fsUPDATED_BY		nvarchar(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			[tbxVIDEO_MOD]
		SET 
		    fsPROG_NO         =  @fsPROG_NO          ,
			fsPROG_NAME		  =	 @fsPROG_NAME		 ,
			fsPROG_TYPE		  =	 @fsPROG_TYPE		 ,
			fsPROG_SUBTYPE	  =	 @fsPROG_SUBTYPE	 ,
			fsPROG_VER		  =	 @fsPROG_VER		 ,
			fsPROD_TYPE		  =	 @fsPROD_TYPE		 ,
			fsPROD_COMPANY	  =	 @fsPROD_COMPANY	 ,
			fsPROG_GRADE	  =	 @fsPROG_GRADE		 ,
			fsVDO_SPEC		  =	 @fsVDO_SPEC		 ,
			fsPROG_LENG		  =	 @fsPROG_LENG		 ,
			fsPROG_FIRST_RUN  =	 @fsPROG_FIRST_RUN	 ,
			fsPROG_DESC		  =	 @fsPROG_DESC		 ,
			fdUPDATED_DATE   = GETDATE()			 ,
			fsUPDATED_BY	 = @fsUPDATED_BY		 

		WHERE
		
			(fsPROG_NO = @fsPROG_NO)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END







