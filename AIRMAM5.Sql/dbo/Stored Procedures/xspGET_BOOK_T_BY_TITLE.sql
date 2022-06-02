
-- =============================================
-- 描述:	取出BOOK_T 	主檔資料 by fsTITLE(like)
-- 記錄:	<2012/04/26><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_BOOK_T_BY_TITLE]
	@fsTITLE	nvarchar(20)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		  SELECT TOP 1
				fnBOOK_T_ID, fsTITLE, fsGROUP, fsTYPE ,fsPATH_TYPE ,fsFOLDER, fsPROFILE_NAME, fsBEG_TIME, fsEND_TIME, fsWIDTH, fsHEIGHT, fsWATERMARK, fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY, 

				_sTYPE = (CASE WHEN (fsTYPE = '') THEN '(未選擇)' ELSE ISNULL(T.fsNAME, '錯誤代碼: '+fsTYPE) END),
				_sPATH_TYPE = (CASE WHEN (fsPATH_TYPE = '') THEN '(未選擇)' ELSE ISNULL(P.fsNAME, '錯誤代碼: '+fsPATH_TYPE) END)
			FROM
				tbmBOOKING_T
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK004') AS T ON (fsTYPE = T.fsCODE)			
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK003') AS P ON (fsPATH_TYPE = P.fsCODE)						
			WHERE 
				fsTITLE = @fsTITLE AND fsTYPE = 'V'
				
		  UNION
			
		  SELECT TOP 1
				fnBOOK_T_ID, fsTITLE, fsGROUP, fsTYPE ,fsPATH_TYPE ,fsFOLDER, fsPROFILE_NAME, fsBEG_TIME, fsEND_TIME, fsWIDTH, fsHEIGHT, fsWATERMARK, fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY, 

				_sTYPE = (CASE WHEN (fsTYPE = '') THEN '(未選擇)' ELSE ISNULL(T.fsNAME, '錯誤代碼: '+fsTYPE) END),
				_sPATH_TYPE = (CASE WHEN (fsPATH_TYPE = '') THEN '(未選擇)' ELSE ISNULL(P.fsNAME, '錯誤代碼: '+fsPATH_TYPE) END)
			FROM
				tbmBOOKING_T
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK004') AS T ON (fsTYPE = T.fsCODE)			
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK003') AS P ON (fsPATH_TYPE = P.fsCODE)						
			WHERE 
				fsTITLE = @fsTITLE AND fsTYPE = 'A'
				
			UNION
			
		  SELECT TOP 1
				fnBOOK_T_ID, fsTITLE, fsGROUP, fsTYPE ,fsPATH_TYPE ,fsFOLDER, fsPROFILE_NAME, fsBEG_TIME, fsEND_TIME, fsWIDTH, fsHEIGHT, fsWATERMARK, fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY, 

				_sTYPE = (CASE WHEN (fsTYPE = '') THEN '(未選擇)' ELSE ISNULL(T.fsNAME, '錯誤代碼: '+fsTYPE) END),
				_sPATH_TYPE = (CASE WHEN (fsPATH_TYPE = '') THEN '(未選擇)' ELSE ISNULL(P.fsNAME, '錯誤代碼: '+fsPATH_TYPE) END)
			FROM
				tbmBOOKING_T
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK004') AS T ON (fsTYPE = T.fsCODE)			
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK003') AS P ON (fsPATH_TYPE = P.fsCODE)						
			WHERE 
				fsTITLE = @fsTITLE AND fsTYPE = 'P'

			UNION
			
		  SELECT TOP 1
				fnBOOK_T_ID, fsTITLE, fsGROUP, fsTYPE ,fsPATH_TYPE ,fsFOLDER, fsPROFILE_NAME, fsBEG_TIME, fsEND_TIME, fsWIDTH, fsHEIGHT, fsWATERMARK, fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY, 

				_sTYPE = (CASE WHEN (fsTYPE = '') THEN '(未選擇)' ELSE ISNULL(T.fsNAME, '錯誤代碼: '+fsTYPE) END),
				_sPATH_TYPE = (CASE WHEN (fsPATH_TYPE = '') THEN '(未選擇)' ELSE ISNULL(P.fsNAME, '錯誤代碼: '+fsPATH_TYPE) END)
			FROM
				tbmBOOKING_T
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK004') AS T ON (fsTYPE = T.fsCODE)			
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK003') AS P ON (fsPATH_TYPE = P.fsCODE)						
			WHERE 
				fsTITLE = @fsTITLE AND fsTYPE = 'D'



										
	
		--SELECT TOP 4
		--	fnBOOK_T_ID, fsTITLE, fsGROUP, fsTYPE ,fsPATH_TYPE ,fsFOLDER, fsPROFILE_NAME, fsBEG_TIME, fsEND_TIME, fsWIDTH, fsHEIGHT, fsWATERMARK, fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY, 
		
		--	_sTYPE = (CASE WHEN (fsTYPE = '') THEN '(未選擇)' ELSE ISNULL(T.fsNAME, '錯誤代碼: '+fsTYPE) END),
		--	_sPATH_TYPE = (CASE WHEN (fsPATH_TYPE = '') THEN '(未選擇)' ELSE ISNULL(P.fsNAME, '錯誤代碼: '+fsPATH_TYPE) END)
		--FROM
		--	tbmBOOKING_T
		--	LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK004') AS T ON (fsTYPE = T.fsCODE)			
		--	LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK003') AS P ON (fsPATH_TYPE = P.fsCODE)						

		--WHERE 
		--	fsTITLE = @fsTITLE
		--ORDER BY
		--	fsTITLE
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




