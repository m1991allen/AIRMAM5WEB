

-- =============================================
-- 描述:	新增FUNC_GROUP主檔資料_BY_GROUP
-- 記錄:	<2012/06/29><Mihsiu.Chiu><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_FUNC_GROUP_BY_GROUP]
	@fsFUNC_ID		VARCHAR(50),
	@fsGROUP_ID		VARCHAR(50),
	@fsCREATED_BY	NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbmFUNC_GROUP
			(fsFUNC_ID, fsGROUP_ID, fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsFUNC_ID, @fsGROUP_ID, GETDATE(), @fsCREATED_BY)
			 
	
	-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
	SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

