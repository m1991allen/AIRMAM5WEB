


-- =============================================
-- 描述:	取出關鍵字清單
-- 記錄:	<2012/09/04><Dennis.Wen><新增本預存> 
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_L_SRH_KEYWORD_LIST]
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT
			fsKEYWORD, _nCOUNT = COUNT(fnSRH_ID)
		FROM
			tblSRH_KW
		GROUP BY
			fsKEYWORD
		HAVING
			(COUNT(fnSRH_ID)>1) AND (fsKEYWORD<>'')
		ORDER BY
			COUNT(fnSRH_ID) DESC, fsKEYWORD
END




