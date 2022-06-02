

-- =============================================
-- 描述:	修改L_LOGIN主檔資料
-- 記錄:	<2011/08/23><Mihsiu.Chiu><新增本預存>
--          <2011/09/05><Eric.Huang> <新增fsCheckStatus欄位>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_L_LOGIN]
	@fnLOGIN_ID BIGINT,
	@fsLOGIN_ID	NVARCHAR(50),
	@fdSTIME	DATETIME,
	@fdETIME	DATETIME,
	@fsNOTE	NVARCHAR(200),	
	@fsCheckStatus	VARCHAR(1),	
	@fsUPDATED_BY	NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tblLOGIN
		SET
			fsLOGIN_ID	    = @fsLOGIN_ID, 
			fdSTIME	        = @fdSTIME, 
			fdETIME	        = @fdETIME, 
			fsNOTE		    = @fsNOTE,		
			fsCheckStatus   = @fsCheckStatus,
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fnLOGIN_ID = @fnLOGIN_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



