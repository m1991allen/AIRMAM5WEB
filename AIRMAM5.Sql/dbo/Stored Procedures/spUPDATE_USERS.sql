

-- =============================================
-- 描述:	修改USERS主檔資料
-- 記錄:	<2011/08/22><Mihsiu.Chiu><新增本預存>
--        	<2011/09/01><Eric.Huang> <新增4個欄位fsPHONE/fsSDATE/fsEDATE/fsTYPE>
--			<2011/09/07><Dennis.Wen><密碼加密處理>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_USERS]
	@fsUSER_ID				NVARCHAR(128),
	@fsPASSWORD				VARCHAR(225) = '',
	@fsNAME					NVARCHAR(25),
	@fsENAME				VARCHAR(50) = NULL,
	@fsTITLE				NVARCHAR(50) = NULL,
	@fsDEPT_ID				VARCHAR(10) = NULL,
	@fsEMAIL				VARCHAR(50) = NULL,
	@fsPHONE				VARCHAR(20) = NULL,	
	@fsDESCRIPTION			NVARCHAR(MAX) = NULL,
	@fsFILE_SECRET			VARCHAR(30) = '0;1;2',
	@fsBOOKING_TARGET_PATH	VARCHAR(500) = NULL,
	@fsIS_ACTIVE			BIT,
	@fsUPDATED_BY			VARCHAR(50),
	@fsGROUP_IDs			VARCHAR(2048)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE
			tbmUSERS
		SET		
			fsNAME = @fsNAME,
			fsENAME = @fsENAME,
			fsTITLE = @fsTITLE,
			fsDEPT_ID = @fsDEPT_ID,
			fsEMAIL = @fsEMAIL,
			fsPHONE = @fsPHONE,
			fsDESCRIPTION = @fsDESCRIPTION,
			fsFILE_SECRET = @fsFILE_SECRET,
			fsBOOKING_TARGET_PATH = @fsBOOKING_TARGET_PATH,
			fsIS_ACTIVE = @fsIS_ACTIVE,
			fdUPDATED_DATE = GETDATE(),
			fsUPDATED_BY = @fsUPDATED_BY
		WHERE
			(fsUSER_ID = @fsUSER_ID)	
		
		IF(@fsGROUP_IDs <> '')
		BEGIN
			
			DELETE FROM [dbo].[tbmUSER_GROUP] WHERE fsUSER_ID = @fsUSER_ID

			INSERT INTO [dbo].[tbmUSER_GROUP]([fsUSER_ID],[fsGROUP_ID],[fdCREATED_DATE],[fsCREATED_BY])
			SELECT @fsUSER_ID, COL1,GETDATE(), @fsUPDATED_BY FROM [dbo].[fn_SLPIT](@fsGROUP_IDs,';')

		END

		SELECT RESULT = ''

		COMMIT			
		
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



