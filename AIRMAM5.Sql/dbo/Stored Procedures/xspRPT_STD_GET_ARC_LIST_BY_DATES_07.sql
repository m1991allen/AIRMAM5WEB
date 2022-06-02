-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<熱門查詢關鍵字統計表>
-- =============================================
CREATE PROCEDURE [dbo].[xspRPT_STD_GET_ARC_LIST_BY_DATES_07]
	@TYPE VARCHAR(2)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@TYPE = '1Y')
		BEGIN
			SELECT TOP 50 關鍵字=fsKEYWORD, 查詢次數=sum(fnCOUNT )
			FROM tblSRH_CNT1Y 
			WHERE fsKEYWORD<> ''
			GROUP BY fsKEYWORD 
			ORDER BY 查詢次數 DESC
		END
	ELSE IF (@TYPE = '3M')
		BEGIN
			SELECT TOP 50 關鍵字=fsKEYWORD, 查詢次數=sum(fnCOUNT )
			FROM tblSRH_CNT3M 
			WHERE fsKEYWORD<> ''
			GROUP BY fsKEYWORD 
			ORDER BY 查詢次數 DESC
		END
	ELSE IF (@TYPE = '1M')
		BEGIN
			SELECT TOP 50 關鍵字=fsKEYWORD, 查詢次數=sum(fnCOUNT )
			FROM tblSRH_CNT1M 
			WHERE fsKEYWORD<> ''
			GROUP BY fsKEYWORD 
			ORDER BY 查詢次數 DESC
		END
	ELSE IF (@TYPE = '1W')
		BEGIN
			SELECT TOP 50 關鍵字=fsKEYWORD, 查詢次數=sum(fnCOUNT )
			FROM tblSRH_CNT1W 
			WHERE fsKEYWORD<> ''
			GROUP BY fsKEYWORD 
			ORDER BY 查詢次數 DESC
		END
	ELSE IF (@TYPE = '3D')
		BEGIN
			SELECT TOP 50 關鍵字=fsKEYWORD, 查詢次數=sum(fnCOUNT )
			FROM tblSRH_CNT3D
			WHERE fsKEYWORD<> ''
			GROUP BY fsKEYWORD 
			ORDER BY 查詢次數 DESC
		END
END

