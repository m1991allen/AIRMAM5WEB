

-- =============================================
-- 描述:	修改BOOKING主檔資料
-- 記錄:	<2012/05/09><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_BOOKING_STATUS]
	@fnBOOKING_ID	BIGINT,	
	@fsSTATUS		VARCHAR(2),	
	@fsUPDATED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tbmBOOKING
		SET			
			fsSTATUS		= @fsSTATUS,
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fnBOOKING_ID = @fnBOOKING_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



