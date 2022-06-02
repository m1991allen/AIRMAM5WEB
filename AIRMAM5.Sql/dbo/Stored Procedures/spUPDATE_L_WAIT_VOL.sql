

-- =============================================
-- 描述:	新增L_WAIT_VOL主檔資料
-- 記錄:	<2013/02/20><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_L_WAIT_VOL]
	@fnWAIT_ID		bigint,
	@fsVOL_ID		varchar(50),
	@fnWORK_ID		bigint,
	@fsSTATUS		varchar(2),
	@fsUPDATED_BY	nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tblWAIT_VOL
		SET
			fsVOL_ID	= @fsVOL_ID, 
			fnWORK_ID	= @fnWORK_ID, 
			fsSTATUS	= @fsSTATUS, 
			fdUPDATED_DATE	= GETDATE(), 
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fnWAIT_ID = @fnWAIT_ID)
			
		IF (@fsSTATUS = '90')
		BEGIN
			UPDATE tblMESSAGE
			SET
				_OK = 'Y'
			WHERE _fnWAIT_ID = @fnWAIT_ID
		END

		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




