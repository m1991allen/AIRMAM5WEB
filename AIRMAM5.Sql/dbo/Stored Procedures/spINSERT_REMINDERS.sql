

-- =============================================
-- 描述:	新增REMINDERS主檔資料
-- 記錄:	<2011/08/16><Mihsiu.Chiu><新增預存>
--      <2016/09/20><Mihsiu.Chiu><增加寫入fdupdated_date與fsupdate_by>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_REMINDERS]
	@fsTITLE			NVARCHAR(50),
	@fsCONTENT			NVARCHAR(MAX),
	@fdDDATE			DATETIME,
	@fsTYPE				CHAR(1),
	@fsTO_UID			VARCHAR(50),
	@fsSTATUS			CHAR(1),
	@fnORDER			INT,	
	@fsNOTE				NVARCHAR(MAX),
	@fsCREATED_BY		NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		INSERT
			tbmREMINDERS
			(fsTITLE, fsCONTENT, fdDDATE, fsTYPE, fsTO_UID, fsSTATUS, fnORDER, fsNOTE, 
			 fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY)
		VALUES
			(@fsTITLE, @fsCONTENT, @fdDDATE, @fsTYPE, @fsTO_UID, @fsSTATUS, @fnORDER, @fsNOTE, 
			 GETDATE(), @fsCREATED_BY,GETDATE(), @fsCREATED_BY)
			
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

