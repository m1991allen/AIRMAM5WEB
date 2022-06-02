﻿




-- =============================================
-- 描述:	取出SRH主檔資料BY 
-- 記錄:	<2011/12/25><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_L_SRH_BY_USERID_AND_DATES]
	@fsCREATED_BY	NVARCHAR(50),
	@fdSDATE		DATE,
	@fdEDATE		DATE
		
AS
BEGIN
 	SET NOCOUNT ON;

IF	@fsCREATED_BY <> ''
			BEGIN
				SELECT 
					L_SRH.fnSRH_ID, 
					L_SRH.fsSTATEMENT, 
					dbo.fnGET_L_SRH_KEYWORD_BY_STATEMENT(L_SRH.fsSTATEMENT) AS _fnKEYWORD, 
					L_SRH.fdCREATED_DATE, 
					L_SRH.fsCREATED_BY,
					ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME
				FROM
					tblSRH AS L_SRH
						LEFT JOIN tbmUSERS USERS_CRT ON L_SRH.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				WHERE
					@fsCREATED_BY = L_SRH.fsCREATED_BY AND				
					DATEADD(dd, 0, DATEDIFF(dd, 0, L_SRH.fdCREATED_DATE)) BETWEEN @fdSDATE AND @fdEDATE
				ORDER BY
					fnSRH_ID DESC
			END
			
		ELSE
			BEGIN
				SELECT 
					L_SRH.fnSRH_ID, 
					L_SRH.fsSTATEMENT, 
					dbo.fnGET_L_SRH_KEYWORD_BY_STATEMENT(L_SRH.fsSTATEMENT) AS _fnKEYWORD, 
					L_SRH.fdCREATED_DATE, 
					L_SRH.fsCREATED_BY,
					ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME
				FROM
					tblSRH AS L_SRH
						LEFT JOIN tbmUSERS USERS_CRT ON L_SRH.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				WHERE
					DATEADD(dd, 0, DATEDIFF(dd, 0, L_SRH.fdCREATED_DATE)) BETWEEN @fdSDATE AND @fdEDATE
				ORDER BY
					fnSRH_ID DESC
			END
END




