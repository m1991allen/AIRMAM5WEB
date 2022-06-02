

-- =============================================
-- 描述:	新增FUNC_GROUP主檔資料_BY_GROUP
-- 記錄:	<2012/06/29><Mihsiu.Chiu><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_FUNC_GROUP]
	@fsFUNC_IDs		VARCHAR(1000),
	@fsGROUP_ID		NVARCHAR(128),
	@fsCREATED_BY	NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		--DECLARE @t TABLE(fsFUNC_ID VARCHAR(50))
		--INSERT INTO @t SELECT COL1 FROM dbo.fn_SLPIT(@fsFUNC_IDs,',')

		INSERT tbmFUNC_GROUP (fsFUNC_ID, fsGROUP_ID, fdCREATED_DATE, fsCREATED_BY)
		SELECT COL1,@fsGROUP_ID,GETDATE(), @fsCREATED_BY FROM dbo.fn_SLPIT(@fsFUNC_IDs,',')
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

