

-- =============================================
-- 描述:	修改NEWS主檔資料
-- 記錄:	<2011/08/09><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_NEWS]
	@fnNEWS_ID	BIGINT,
	@fsTITLE	NVARCHAR(50),
	@fsCONTENT	NVARCHAR(MAX),
	@fdDATE	DATE,
	@fsDEPT		NVARCHAR(50),
	@fsNOTE		NVARCHAR(100),
	@fsUPDATED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tbmNEWS
		SET
			fsTITLE		= @fsTITLE, 
			fsCONTENT	= @fsCONTENT, 
			fdDATE	= @fdDATE, 
			fsDEPT		= @fsDEPT, 
			fsNOTE		= @fsNOTE,
			
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fnNEWS_ID = @fnNEWS_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



