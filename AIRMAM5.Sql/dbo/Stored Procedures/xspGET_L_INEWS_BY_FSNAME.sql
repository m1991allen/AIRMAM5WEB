

-- =============================================
-- 描述:	取出INEWS主檔資料 BY FSNAME
-- 記錄:	<2013/01/30><Eric.Huang><新增本預存>
-- 記錄:	<2013/03/15><Eric.Huang><修改本預存><新增 fsREPORTER, fsSTYLE, fsGROUP>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_L_INEWS_BY_FSNAME]
	@fsFILE_NAME	VARCHAR(255)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT TOP 1
			fsSTORY_ID, fsCONTENT, fsTITLE, fsREPORTER, fsSTYLE, fsGROUP
		FROM
			tblINEWS
			
		WHERE
			(fsFILE_NAME = @fsFILE_NAME)
						
		ORDER BY
			fdUPDATED_DATE DESC
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


