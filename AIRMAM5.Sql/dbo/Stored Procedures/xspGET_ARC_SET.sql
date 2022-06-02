


-- =============================================
-- 描述:	取出ARC_SET主檔資料
-- 記錄:	<2011/11/14><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_ARC_SET]
	@fsTYPE	VARCHAR(1)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			fsTYPE, fsNAME, fsTYPE_I, fsTYPE_O, fsTYPE_S, 
			fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY

		FROM
			tbmARC_SET
		WHERE
			(fsTYPE = @fsTYPE)
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



