

-- =============================================
-- 描述:	修改REMINDERS主檔資料
-- 記錄:	<2011/08/16><Mihsiu.Chiu><修改預存>
-- 記錄:	<2011/09/08><Eric.Huang> <新增fsCheckStatus欄位>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_REMINDERS]
	@fnRMD_ID			BIGINT,
	@fsTITLE			NVARCHAR(50),
	@fsCONTENT			NVARCHAR(MAX),
	@fdDDATE			DATETIME,
	@fsTYPE				CHAR(1),
	@fsTO_UID			VARCHAR(50),
	@fsSTATUS			CHAR(1),
	@fnORDER			INT,	
	@fsNOTE				NVARCHAR(MAX),
	@fsCHECKSTATUS		CHAR(1),
	@fsUPDATED_BY		NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		UPDATE
			tbmREMINDERS
		SET
			fsTITLE = @fsTITLE, 
			fsCONTENT = @fsCONTENT, 
			fdDDATE = @fdDDATE, 
			fsTYPE = @fsTYPE, 
			fsTO_UID = @fsTO_UID, 
			fsSTATUS = @fsSTATUS, 
			fnORDER = @fnORDER, 
			fsNOTE = @fsNOTE, 
			fsCHECKSTATUS = @fsCHECKSTATUS,
			fdUPDATED_DATE = GETDATE(), 
			fsUPDATED_BY = @fsUPDATED_BY
		WHERE 
			(fnRMD_ID = @fnRMD_ID)
			
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


