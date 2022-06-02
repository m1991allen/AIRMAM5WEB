

-- =============================================
-- 描述:	取出SYNO類別清單資料
-- 記錄:	<2012/05/28><Dennis.Wen><新增預存>
-- 記錄:	<2012/06/01><Eric.Huang><修改預存，將空值過濾>
-- 記錄:	<2017/06/16><David.Sin><修改預存，改成撈代碼表>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_SYNONYMS_TYPE_LIST]

AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT
			fsCODE,fsNAME
		FROM 
			tbzCODE
		WHERE
			fsCODE_ID = 'SYNO_TYPE'
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



