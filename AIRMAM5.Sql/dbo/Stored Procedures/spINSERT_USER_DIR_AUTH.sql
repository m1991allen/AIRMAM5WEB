





-- =============================================
-- 描述:	登入時新增使用者有權限的DIR
-- 記錄:	<2019/08/24><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_USER_DIR_AUTH]
	@fsLOGIN_ID		NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		DECLARE @fsUSER_ID NVARCHAR(128) = (SELECT [fsUSER_ID] FROM [dbo].[tbmUSERS] WHERE fsLOGIN_ID = @fsLOGIN_ID)
		
		
		;WITH reADMIN(fnDIR_ID,fnPARENT_ID,fsNAME,fsDIRTYPE,fsADMIN_GROUP,fsADMIN_USER,fsSHOWTYPE,fnLEVEL) AS(
			SELECT fnDIR_ID,fnPARENT_ID,fsNAME,fsDIRTYPE,fsADMIN_GROUP,fsADMIN_USER,fsSHOWTYPE, 1 FROM tbmDIRECTORIES WHERE fnDIR_ID = 1
			UNION ALL
			SELECT A.fnDIR_ID,A.fnPARENT_ID,A.fsNAME,A.fsDIRTYPE,A.fsADMIN_GROUP,A.fsADMIN_USER,A.fsSHOWTYPE, B.fnLEVEL + 1 FROM tbmDIRECTORIES A JOIN reADMIN B ON A.fnPARENT_ID = B.fnDIR_ID
		)
		
		SELECT *,'' AS _ADMIN, '' AS _USER INTO #tbResult FROM reADMIN
		
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
		WHERE fnDIR_ID IN (SELECT fnDIR_ID FROM reADMIN)
		
		/*_USER 往下, 父階層是'Y'或'y'就'y'*/
		;WITH reUSER(fnDIR_ID,_USER,fsDIRTYPE) AS(
			SELECT fnDIR_ID,_USER,fsDIRTYPE FROM #tbResult WHERE _USER = 'Y'
			UNION ALL
			SELECT A.fnDIR_ID,A._USER,A.fsDIRTYPE FROM #tbResult A JOIN reUSER B ON A.fnPARENT_ID = B.fnDIR_ID
		)
		
		UPDATE #tbResult 		
		SET _USER = 'y'
		WHERE fnDIR_ID IN (SELECT fnDIR_ID FROM reUSER)
		
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
		
		DECLARE @RESULT VARCHAR(MAX)
		SET @RESULT = (SELECT CAST(fnDIR_ID AS VARCHAR(5)) + ';' FROM #tbResult WHERE (_ADMIN <> '' OR _USER <> '') AND (fsDIRTYPE = 'Q') AND (fsSHOWTYPE <> 'H') FOR XML PATH(''))

		DELETE FROM tbmUSER_DIR WHERE fsLOGIN_ID = @fsLOGIN_ID
		
		INSERT INTO tbmUSER_DIR VALUES(@fsLOGIN_ID,@RESULT,GETDATE())

		DROP TABLE #tbResult

		SELECT RESULT = ''

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END






