﻿

-- =============================================
-- 描述:	取出L_LOGIN主檔資料
-- 記錄:	<2011/08/23><Mihsiu.Chiu><新增本預存>
-- 記錄:	<2016/10/20><David.Sin><整合查詢>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_L_LOGIN]
	@fnLOGIN_ID		BIGINT,
	@fdSDATE		VARCHAR(10),
	@fdEDATE		VARCHAR(10), 
	@fsLOGIN_ID		VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
		tblLOGIN.fnLOGIN_ID, 
		tblLOGIN.fsLOGIN_ID, 
		tblLOGIN.fdSTIME, 
		tblLOGIN.fdETIME, 
		tblLOGIN.fsNOTE,
		tblLOGIN.fdCREATED_DATE, 
		tblLOGIN.fsCREATED_BY, 
		tblLOGIN.fdUPDATED_DATE, 
		tblLOGIN.fsUPDATED_BY,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME
	FROM
		tblLOGIN
			LEFT JOIN tbmUSERS USERS_CRT ON tblLOGIN.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
			LEFT JOIN tbmUSERS USERS_UPD ON tblLOGIN.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID		
	WHERE
		(@fnLOGIN_ID = 0 OR tblLOGIN.fnLOGIN_ID = @fnLOGIN_ID) AND
		(@fdSDATE = '' OR CONVERT(VARCHAR(10),tblLOGIN.fdSTIME,111) >= @fdSDATE) AND
		(@fdEDATE = '' OR CONVERT(VARCHAR(10),tblLOGIN.fdSTIME,111) <= @fdEDATE) AND
		(@fsLOGIN_ID = '' OR tblLOGIN.fsLOGIN_ID = @fsLOGIN_ID)
	ORDER BY
		[fnLOGIN_ID] DESC
END

