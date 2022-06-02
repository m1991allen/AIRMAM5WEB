


-- =============================================
-- 描述:	修改USERS主檔的密碼欄位資料
-- 記錄:	<2012/08/17><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_USERS_PSW]

	@fsLOGIN_ID        VARCHAR(50),
	@fsPASSWORD_OLD    VARCHAR(255),
	@fsPASSWORD_NEW    VARCHAR(255)
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DECLARE @fsPASSWORD_NOW VARCHAR(255)
		
		
		SET @fsPASSWORD_NOW	= (SELECT fsPASSWORD FROM tbmUSERS	WHERE (fsLOGIN_ID = @fsLOGIN_ID))
		
		IF (@fsPASSWORD_OLD <> @fsPASSWORD_NOW)
			BEGIN
				SELECT RESULT = 'ERROR:「目前密碼」驗證失敗'
			END
		ELSE
			BEGIN
				UPDATE
					tbmUSERS
				SET			
					fsPASSWORD = @fsPASSWORD_NEW

				WHERE
					(fsLOGIN_ID = @fsLOGIN_ID)			
			END
						
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




