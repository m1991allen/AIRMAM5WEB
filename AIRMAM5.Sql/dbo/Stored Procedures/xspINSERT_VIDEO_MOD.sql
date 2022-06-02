


-- =============================================
-- 描述:	新增tbxVIDEO_MOD 節目片庫MOD_DATA 主檔 資料
-- 記錄:	<2015/10/11><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_VIDEO_MOD]

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
     @fsCREATED_BY		nvarchar(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT INTO [dbo].[tbxVIDEO_MOD]
           ([fsPROG_NO]
           ,[fsPROG_NAME]
           ,[fsPROG_TYPE]
           ,[fsPROG_SUBTYPE]
           ,[fsPROG_VER]
           ,[fsPROD_TYPE]
           ,[fsPROD_COMPANY]
           ,[fsPROG_GRADE]
           ,[fsVDO_SPEC]
           ,[fsPROG_LENG]
           ,[fsPROG_FIRST_RUN]
           ,[fsPROG_DESC]
           ,[fdCREATED_DATE]
           ,[fsCREATED_BY])
     VALUES
           (@fsPROG_NO,	@fsPROG_NAME, @fsPROG_TYPE, @fsPROG_SUBTYPE,
            @fsPROG_VER, @fsPROD_TYPE, @fsPROD_COMPANY, 
			@fsPROG_GRADE, @fsVDO_SPEC, @fsPROG_LENG,
			@fsPROG_FIRST_RUN, @fsPROG_DESC, GETDATE(),
			@fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





