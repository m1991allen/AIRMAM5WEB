

-- =============================================
-- 描述:	修改BOOKING主檔資料
-- 記錄:	<2012/04/18><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_BOOKING]
	@fnBOOKING_ID	BIGINT,
	@fsTEMP_ID		NVARCHAR(20),
	@fsREASON       VARCHAR(2),	
	@fsDESCRIPTION	NVARCHAR(50),
	@fsFOLDER		VARCHAR(50),	
	@fnORDER		INT,	
	@fsSTATUS		VARCHAR(2),	
	@fsUPDATED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tbmBOOKING
		SET
			fsREASON	    = @fsREASON, 
			fsDESCRIPTION	= @fsDESCRIPTION, 
			fsFOLDER		= @fsFOLDER,
			fnORDER			= @fnORDER,
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



