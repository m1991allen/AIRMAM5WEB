



-- =============================================
-- 描述:	取出ARC_SET主檔 ALL 資料
-- 記錄:	<2011/11/14><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_ARC_SET_ALL]

AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			fsTYPE, fsNAME, fsTYPE_I, fsTYPE_O, fsTYPE_S, 
			fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY

		FROM
			tbmARC_SET

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




