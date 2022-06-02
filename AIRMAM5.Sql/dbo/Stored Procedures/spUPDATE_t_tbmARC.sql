


-- =============================================
-- 描述:	修改t_tbmARC_INDEX主檔資料
-- 記錄:	<2012/04/30><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_t_tbmARC]
	@fnINDEX_ID			BIGINT,
	@fsFILE_NO			VARCHAR(16),
	@fsTYPE				VARCHAR(4),
	@fsREASON			NVARCHAR(50),
	@fsSTATUS			VARCHAR(1),	
	@fsUPDATED_BY		VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			t_tbmARC_INDEX
		SET
			fsFILE_NO		= @fsFILE_NO,
			fsTYPE			= @fsTYPE,
			fsREASON		= @fsREASON,
			fsSTATUS		= @fsSTATUS,			
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			fnINDEX_ID		= @fnINDEX_ID
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




