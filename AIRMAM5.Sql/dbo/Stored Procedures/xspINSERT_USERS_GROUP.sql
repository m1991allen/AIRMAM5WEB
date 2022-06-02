

-- =============================================
-- 描述:	新增USER_GROUP主檔資料
-- 記錄:	<2011/08/23><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_USERS_GROUP]
	@fnUSER_ID		bigint ,	
	@fsGROUP_ID		varchar(50) ,
	@fsCREATED_BY	nvarchar(50)
	
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbmUSER_GROUP
			(fnUSER_ID, fsGROUP_ID, fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fnUSER_ID, @fsGROUP_ID, GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @fnUSER_ID
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




