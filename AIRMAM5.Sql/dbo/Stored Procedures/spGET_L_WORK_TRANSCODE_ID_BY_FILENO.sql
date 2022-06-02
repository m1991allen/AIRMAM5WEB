



-- =============================================
-- 描述:	取出WORK編號BY檔案編號
-- 記錄:	<2019/07/26><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_L_WORK_TRANSCODE_ID_BY_FILENO]
	@fsFILE_NO		VARCHAR(16)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT
			TOP 1 fnWORK_ID
		FROM
			tblWORK
		WHERE
			fsTYPE = 'TRANSCODE' AND
			_ITEM_ID = @fsFILE_NO
		ORDER BY
			fnWORK_ID DESC
END



