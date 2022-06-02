



-- =============================================
-- 描述:	修改USER_APP主檔資料二個欄位,進度跟備註
-- 記錄:	<2013/10/15><Eric.Huang><新增本預存>

-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_USER_APP_BY_STATUS_AND_NOTE]

	@fnUSER_A_ID	BIGINT,
	@fsSTATUS       VARCHAR(1),
	@fsNOTE         NVARCHAR(20),	
	@fsUPDATED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN
			UPDATE
				tbmUSER_APP
			SET			
				fsSTATUS = @fsSTATUS,	
				fsNOTE   = @fsNOTE,
					
				fdUPDATED_DATE = GETDATE(),
				fsUPDATED_BY = @fsUPDATED_BY
			WHERE
				(fnUSER_A_ID = @fnUSER_A_ID)			
		END						
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





