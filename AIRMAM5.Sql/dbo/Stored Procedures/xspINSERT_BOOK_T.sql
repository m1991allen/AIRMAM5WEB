



-- =============================================
-- 描述:	新增BOOK_T	主檔資料
-- 記錄:	<2012/04/26><Eric.Huang><新增預存>
-- =============================================
create PROCEDURE [dbo].[xspINSERT_BOOK_T]

	@fsTITLE		nvarchar(20),
	@fsGROUP		nvarchar(50),
	@fsTYPE			varchar(1),
	@fsPATH_TYPE	varchar(1),
	@fsFOLDER		varchar(50),
	@fsPROFILE_NAME	nvarchar(128),
	@fsBEG_TIME		varchar(10),
	@fsEND_TIME		varchar(10),
	@fsWIDTH		varchar(10),
	@fsHEIGHT		varchar(10),
	@fsWATERMARK	varchar(500),
	@fsCREATED_BY	nvarchar(50)
	
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbmBOOKING_T
			(fsTITLE, fsGROUP, fsTYPE ,fsPATH_TYPE ,fsFOLDER, fsPROFILE_NAME, fsBEG_TIME, fsEND_TIME, fsWIDTH, fsHEIGHT, fsWATERMARK, fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsTITLE, @fsGROUP, @fsTYPE ,@fsPATH_TYPE ,@fsFOLDER, @fsPROFILE_NAME, @fsBEG_TIME, @fsEND_TIME, @fsWIDTH, @fsHEIGHT, @fsWATERMARK, GETDATE(), @fsCREATED_BY)		
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




