

-- =============================================
-- 描述:	修改L_LOGIN主檔資料的登出時間
-- 記錄:	<2011/09/09><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_L_LOGIN_BY_ETIME]
	@fnLOGIN_ID BIGINT,
	@fsLOGIN_ID	NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tblLOGIN
		SET
			fdETIME	        = GETDATE(), 
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsLOGIN_ID,
			fsNOTE			= '已登出系統'
		WHERE
			(fnLOGIN_ID = @fnLOGIN_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



