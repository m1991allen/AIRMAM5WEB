


-- =============================================
-- 描述:	新增RESTORE主檔資料
-- 記錄:	<2013/02/25><Albert.Chen><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_RESTORE]
	       @fsFILE_NO        VARCHAR(16),
           @fsTYPE           VARCHAR(1),
           @fsTSM_PATH       VARCHAR(250),
           @fsNOTE           NVARCHAR(500),
           @fsCREATED_BY     NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tblRESTORE
			(fsFILE_NO, fsTYPE, fsTSM_PATH , fsNOTE, fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsFILE_NO, @fsTYPE, @fsTSM_PATH, @fsNOTE, GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



