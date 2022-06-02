



-- =============================================
-- 描述:	取出SRH主檔 ALL 資料
-- 記錄:	<2011/12/22><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_L_SRH_ALL]
	
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
		L_SRH.fnSRH_ID, 
		L_SRH.fsSTATEMENT, 
		L_SRH.fdCREATED_DATE, 
		L_SRH.fsCREATED_BY,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME
	FROM
		tblSRH AS L_SRH
			LEFT JOIN tbmUSERS USERS_CRT ON L_SRH.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
		
	ORDER BY
		L_SRH.fnSRH_ID DESC
END




