


-- =============================================
-- 描述:	取出SRH主檔資料
-- 記錄:	<2011/12/13><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_L_SRH]
	@fnSRH_ID		BIGINT
	
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
	WHERE
		(fnSRH_ID = @fnSRH_ID)
	ORDER BY
		fnSRH_ID
END



