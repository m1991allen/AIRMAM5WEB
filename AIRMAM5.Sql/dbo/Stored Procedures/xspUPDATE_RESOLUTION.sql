


-- =============================================
-- 描述:	修改RESOLUTION主檔資料
-- 記錄:	<2012/04/26><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_RESOLUTION]
	@fnRESOL_ID			BIGINT,
	@fsRATIO			varchar	(10),
	@fsNAME				varchar	(20),
	@fsWIDTH			varchar	(10),
	@fsHEIGHT			varchar	(10),
	@fsUPDATED_BY		varchar(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tbzRESOLUTION
		SET
			fsRATIO			= @fsRATIO, 
			fsNAME			= @fsNAME,
			fsWIDTH			= @fsWIDTH,
			fsHEIGHT		= @fsHEIGHT,
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fnRESOL_ID = @fnRESOL_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




