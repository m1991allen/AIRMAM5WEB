

-- =============================================
-- 描述:	依照查詢條件取出t_tbmARC_INDEX主檔資料
-- 記錄:	<2012/05/02><Mihsiu.Chiu><新增本預存>
--			<2012/07/06><Mihsiu.Chiu><for english version>
-- =============================================
CREATE PROCEDURE [dbo].[sp_t_GET_ARC_INDEX_BY_CONDITION]	
	--@fsREASON		varchar(50),
	@fdDATE1		varchar(10),
	@fdDATE2		varchar(10),
	@fsSTATUS		varchar(10),
	@fsTYPE			varchar(10)
	--@fsDELETED_BY	nvarchar(50)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
		T.fnINDEX_ID,
		T.fsFILE_NO,
		T.fsTYPE,
		T.fsREASON,
		T.fsSTATUS,
		CASE 
			WHEN T.fsSTATUS = '' THEN '暫刪除'
			ELSE ISNULL(C1.fsNAME, '錯誤代碼: ' + T.fsSTATUS) 
		END AS _sSTATUS,
		CASE 
			WHEN T.fsTYPE = '' THEN '(未選擇)' 
			ELSE ISNULL(C2.fsNAME, '錯誤代碼: ' + T.fsTYPE) 
		END AS _sTYPE,
		CASE T.fsTYPE
			WHEN 'V' THEN (SELECT [dbo].[t_tbmARC_VIDEO].[fsTITLE] FROM [dbo].[t_tbmARC_VIDEO] WHERE [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] = T.fsFILE_NO UNION SELECT [dbo].[tbmARC_VIDEO].[fsTITLE] FROM [dbo].[tbmARC_VIDEO] WHERE [dbo].[tbmARC_VIDEO].[fsFILE_NO] = T.fsFILE_NO)
			WHEN 'A' THEN (SELECT [dbo].[t_tbmARC_AUDIO].[fsTITLE] FROM [dbo].[t_tbmARC_AUDIO] WHERE [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] = T.fsFILE_NO UNION SELECT [dbo].[tbmARC_AUDIO].[fsTITLE] FROM [dbo].[tbmARC_AUDIO] WHERE [dbo].[tbmARC_AUDIO].[fsFILE_NO] = T.fsFILE_NO)
			WHEN 'P' THEN (SELECT [dbo].[t_tbmARC_PHOTO].[fsTITLE] FROM [dbo].[t_tbmARC_PHOTO] WHERE [dbo].[t_tbmARC_PHOTO].[fsFILE_NO] = T.fsFILE_NO UNION SELECT [dbo].[tbmARC_PHOTO].[fsTITLE] FROM [dbo].[tbmARC_PHOTO] WHERE [dbo].[tbmARC_PHOTO].[fsFILE_NO] = T.fsFILE_NO)
			WHEN 'D' THEN (SELECT [dbo].[t_tbmARC_DOC].[fsTITLE] FROM [dbo].[t_tbmARC_DOC] WHERE [dbo].[t_tbmARC_DOC].[fsFILE_NO] = T.fsFILE_NO UNION SELECT [dbo].[tbmARC_DOC].[fsTITLE] FROM [dbo].[tbmARC_DOC] WHERE [dbo].[tbmARC_DOC].[fsFILE_NO] = T.fsFILE_NO)
		END AS _sTITLE,
		T.fdCREATED_DATE,
		T.fsCREATED_BY,
		T.fdUPDATED_DATE,
		T.fsUPDATED_BY,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME
	FROM
		[dbo].[t_tbmARC_INDEX] AS T  
			LEFT JOIN [dbo].[tbzCODE] C1 ON T.fsSTATUS = C1.fsCODE AND C1.fsCODE_ID='ARC006'
			LEFT JOIN [dbo].[tbzCODE] C2 ON T.fsTYPE = C2.fsCODE AND C2.fsCODE_ID='ARC004'
			LEFT JOIN [dbo].[tbmUSERS] AS USERS_CRT ON T.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
			LEFT JOIN [dbo].[tbmUSERS] AS USERS_UPD ON T.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
	WHERE
		(@fdDATE1 = '' OR CONVERT(VARCHAR(10),T.[fdCREATED_DATE],111) >= @fdDATE1) AND 
		(@fdDATE2 = '' OR CONVERT(VARCHAR(10),T.[fdCREATED_DATE],111) <= @fdDATE2) AND 
		(@fsSTATUS = '' OR T.[fsSTATUS] = @fsSTATUS) AND
		(@fsTYPE = '' OR T.fsTYPE = @fsTYPE)
END


