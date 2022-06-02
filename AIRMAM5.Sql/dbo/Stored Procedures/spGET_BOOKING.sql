

-- =============================================
-- 描述:	取出BOOKING主檔資料
-- 記錄:	<2012/04/18><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_BOOKING]
	@fnBOOKING_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			fnBOOKING_ID, fsTEMP_ID, fsREASON, fsDESCRIPTION, fsFOLDER, fnORDER, fsSTATUS,
			fsCREATED_BY, fdCREATED_DATE, fsUPDATED_BY, fdUPDATED_DATE,
			_sREASON = (CASE WHEN (fsREASON = '') THEN '(未選擇)' ELSE ISNULL(R.fsNAME, '錯誤代碼: '+fsREASON) END), 
			_sSTATUS = (CASE WHEN (fsSTATUS = '') THEN '(未選擇)' ELSE ISNULL(S.fsNAME, '錯誤代碼: '+fsSTATUS) END),
			_sTEMP = dbo.fnGET_BOOKING_T_NAME_FROM_LIST(fsTEMP_ID)
		FROM
			tbmBOOKING	AS B		
			LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK001') AS R ON (fsREASON = R.fsCODE)			
			LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK002') AS S ON (fsSTATUS = S.fsCODE)						
		WHERE
			(fnBOOKING_ID = @fnBOOKING_ID)
		ORDER BY
			fnBOOKING_ID DESC
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


