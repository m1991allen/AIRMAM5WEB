


-- =============================================
-- 描述:	取出DIRECTORIES LOAD ON DEMAND
-- 記錄:	<2016/09/10><David.Sin><新增本預存>
-- 記錄:	<2016/09/10><David.Sin><增加關鍵字查詢>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_DIRECTORIES_LOAD_ON_DEMAND]

	@fnDIR_ID		BIGINT,
	@fsLOGIN_ID		VARCHAR(50),
	@fsKEYWORD		NVARCHAR(10) = '',
	@fcSHOW_SUBJ	CHAR(1)

AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsUSER_ID NVARCHAR(128) = (SELECT [fsUSER_ID] FROM [dbo].[tbmUSERS] WHERE fsLOGIN_ID = @fsLOGIN_ID)
	
	;WITH reADMIN(
		fnDIR_ID,fnPARENT_ID,fsNAME,fsDIRTYPE,fsADMIN_GROUP,fsADMIN_USER,fsSHOWTYPE,fnLEVEL,fnORDER,fsPATH,fsPATH_NAME) AS
	(
		SELECT fnDIR_ID,fnPARENT_ID,fsNAME,fsDIRTYPE,fsADMIN_GROUP,fsADMIN_USER,fsSHOWTYPE, 1, 
			fnORDER, CAST(CONVERT(VARCHAR(10),fnDIR_ID) AS VARCHAR(MAX)) ,CAST(fsNAME AS VARCHAR(MAX)) FROM tbmDIRECTORIES WHERE fnDIR_ID = 1
		UNION ALL
		SELECT A.fnDIR_ID,A.fnPARENT_ID,A.fsNAME,A.fsDIRTYPE,A.fsADMIN_GROUP,A.fsADMIN_USER,A.fsSHOWTYPE, B.fnLEVEL + 1, 
			A.fnORDER, CAST(B.fsPATH + CAST('/' + CONVERT(VARCHAR(10),A.fnDIR_ID) AS VARCHAR(1024)) AS VARCHAR(MAX)) ,CAST(B.fsPATH_NAME + '/' + A.fsNAME AS VARCHAR(MAX)) FROM tbmDIRECTORIES A JOIN reADMIN B ON A.fnPARENT_ID = B.fnDIR_ID
	)
	
	SELECT *,'' AS _ADMIN, '' AS _USER INTO #tbResult FROM reADMIN ORDER BY fnDIR_ID

	/*_ADMIN是大寫Y的表示其於Directories設定為管理群組或管理帳號*/
	UPDATE #tbResult 
	SET _ADMIN = 'Y'
	FROM #tbResult A JOIN (SELECT fsGROUP_ID FROM [dbo].[tbmUSER_GROUP] WHERE [fsUSER_ID] = @fsUSER_ID) B ON A.fsADMIN_GROUP LIKE '%' + B.fsGROUP_ID + ';%'
	
	UPDATE #tbResult 
	SET _ADMIN = 'Y'
	FROM #tbResult A
	WHERE A.fsADMIN_USER LIKE '%' + @fsLOGIN_ID + ';%'
	
	/*檢查每一筆在tbmDIR_GROUP與tbmDIR_USER是否有設定,有存在設定表示有使用權限*/
	UPDATE #tbResult 
	SET _USER = 'Y'
	WHERE fnDIR_ID IN (SELECT [fnDIR_ID] FROM [dbo].[tbmDIR_GROUP] WHERE [fsGROUP_ID] IN (SELECT [fsGROUP_ID] FROM [dbo].[tbmUSER_GROUP] WHERE [fsUSER_ID] = @fsUSER_ID))
	
	UPDATE #tbResult 
	SET _USER = 'Y'
	WHERE fnDIR_ID IN (SELECT [fnDIR_ID] FROM [dbo].[tbmDIR_USER] WHERE [fsLOGIN_ID] = @fsLOGIN_ID)
	
	/*_ADMIN 往下, 父階層是'Y'或'y'就'y'*/
	;WITH reADMIN(fnDIR_ID) AS(
		SELECT fnDIR_ID FROM #tbResult WHERE _ADMIN = 'Y'
		UNION ALL
		SELECT A.fnDIR_ID FROM #tbResult A JOIN reADMIN B ON A.fnPARENT_ID = B.fnDIR_ID 
	)
	
	UPDATE #tbResult 		
	SET _ADMIN = 'y'
	WHERE fnDIR_ID IN (SELECT fnDIR_ID FROM reADMIN) AND _ADMIN <> 'Y'
	
	/*_USER 往下, 父階層是'Y'或'y'就'y'*/
	;WITH reUSER(fnDIR_ID,_USER,fsDIRTYPE) AS(
		SELECT fnDIR_ID,_USER,fsDIRTYPE FROM #tbResult WHERE _USER = 'Y'
		UNION ALL
		SELECT A.fnDIR_ID,A._USER,A.fsDIRTYPE FROM #tbResult A JOIN reUSER B ON A.fnPARENT_ID = B.fnDIR_ID
	)
	
	UPDATE #tbResult 		
	SET _USER = 'y'
	WHERE fnDIR_ID IN (SELECT fnDIR_ID FROM reUSER) AND _USER <> 'Y'
	
	/*_USER OR _ADMIN往上, 子階層需掛而本身未設定使用, 則是'@'*/
	/*_USER OR _ADMIN往上, 子階層需掛而本身有管理權限, 則是'y'*/
	
	;WITH reAUTH(fnPARENT_ID) AS(
		SELECT fnPARENT_ID FROM #tbResult WHERE (_USER <> '' OR _ADMIN <>'')
		UNION ALL								
		SELECT A.fnPARENT_ID FROM #tbResult A JOIN reAUTH B ON A.fnDIR_ID = B.fnPARENT_ID
	)
	
	UPDATE #tbResult	
	SET _USER = CASE WHEN _ADMIN = '' AND _USER = '' THEN '@' ELSE '' END
	WHERE fnDIR_ID IN (SELECT fnPARENT_ID FROM reAUTH) AND _USER = '' AND fsSHOWTYPE <> 'H'

	IF (@fsKEYWORD <> '')
	BEGIN
		
		--根據長出來的樹相關的PathName符合關鍵字的Path找出來，並把所有Path合成一串字串，以方便split
		DECLARE @fsINPUT NVARCHAR(MAX)
		SELECT @fsINPUT = (
			CASE 
			WHEN @fsKEYWORD <> '' THEN 
					(SELECT CAST([fsPATH] as varchar) + '/'
					FROM #tbResult
					WHERE fsPATH_NAME LIKE '%' + @fsKEYWORD +'%'
					For XML PAth(''))
			END
		)


		SELECT 
			A.fnDIR_ID,A.fnPARENT_ID,
			CASE 
				WHEN fsDIRTYPE = 'Q' THEN 
					CASE @fcSHOW_SUBJ 
						WHEN 'Y' THEN A.fsNAME + '(' + CONVERT(VARCHAR(10),ISNULL(C.fnCOUNT,0)) + ')'
						ELSE A.fsNAME
					END
				ELSE A.fsNAME
			END AS fsNAME,
			A.fsDIRTYPE,A.fsSHOWTYPE,A._ADMIN,A._USER
		FROM #tbResult A 
			LEFT JOIN (SELECT fnDIR_ID, COUNT(*) AS fnCOUNT FROM tbmSUBJECT GROUP BY fnDIR_ID) C ON A.fnDIR_ID = C.fnDIR_ID
		WHERE
			(_ADMIN <> '' OR _USER <> '') AND (fnPARENT_ID = @fnDIR_ID) AND A.fnDIR_ID IN (SELECT CONVERT(bigint,COL1) FROM [dbo].[fn_SLPIT](@fsINPUT,'/'))
		ORDER BY fnORDER
	END
	ELSE
	BEGIN
		SELECT 
			A.fnDIR_ID,A.fnPARENT_ID,
			CASE 
				WHEN fsDIRTYPE = 'Q' THEN 
					CASE @fcSHOW_SUBJ 
						WHEN 'Y' THEN A.fsNAME + '(' + CONVERT(VARCHAR(10),ISNULL(C.fnCOUNT,0)) + ')'
						ELSE A.fsNAME
					END
				ELSE A.fsNAME
			END AS fsNAME,
			A.fsDIRTYPE,A.fsSHOWTYPE,A._ADMIN,A._USER,A.fsSHOWTYPE
		FROM #tbResult A 
			LEFT JOIN (SELECT fnDIR_ID, COUNT(*) AS fnCOUNT FROM tbmSUBJECT GROUP BY fnDIR_ID) C ON A.fnDIR_ID = C.fnDIR_ID
		WHERE
			(_ADMIN <> '' OR _USER <> '') AND (fnPARENT_ID = @fnDIR_ID)
		ORDER BY fnORDER

	END

	DROP TABLE #tbResult
END






