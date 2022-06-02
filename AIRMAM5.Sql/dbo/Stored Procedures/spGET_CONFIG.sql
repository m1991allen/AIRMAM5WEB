

-- =============================================
-- 描述:	取出CONFIG主檔資料
-- 記錄:	<2011/11/29><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_CONFIG]
	@fsKEY		VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			C.fsKEY, 
			C.fsVALUE, 
			C.fsTYPE, 
			C.fsDESCRIPTION, 
			C.fdCREATED_DATE, 
			C.fsCREATED_BY, 
			C.fdUPDATED_DATE, 
			C.fsUPDATED_BY,
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME
		FROM
			tbzCONFIG C
				LEFT JOIN tbmUSERS USERS_CRT ON C.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON C.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE
			(C.fsKEY		= @fsKEY)
		ORDER BY
			[fsKEY], [fsTYPE]
END


