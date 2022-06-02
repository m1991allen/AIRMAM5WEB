



-- =============================================
-- 描述:	重設USERS密碼欄位資料
-- 記錄:	<2019/05/23><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_USERS_RESET_PSW]

	@fsLOGIN_ID        VARCHAR(50),
	@fsPASSWORD_NEW    VARCHAR(255)
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DECLARE @fsPASSWORD_NOW VARCHAR(255) = (SELECT fsPASSWORD FROM tbmUSERS	WHERE (fsLOGIN_ID = @fsLOGIN_ID AND fsPASSWORD = @fsLOGIN_ID AND fsIS_ACTIVE = 'Y'))
		
		IF (@fsPASSWORD_NOW <> @fsLOGIN_ID)
			BEGIN
				SELECT RESULT = 'ERROR:「目前密碼」驗證失敗'
			END
		ELSE
			BEGIN

				BEGIN TRANSACTION

				UPDATE
					tbmUSERS
				SET			
					fsPASSWORD = @fsPASSWORD_NEW

				WHERE
					(fsLOGIN_ID = @fsLOGIN_ID)		
					
				COMMIT	
			END
						
		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




