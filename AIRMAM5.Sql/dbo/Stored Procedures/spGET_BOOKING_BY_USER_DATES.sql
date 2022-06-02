

-- =============================================
-- 描述:	取出BOOKING主檔資料
-- 記錄:	<2012/04/18><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_BOOKING_BY_USER_DATES]
	@fsCREATED_BY	nvarchar(50),
	@SDATE	Date,
	@EDATE	Date
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			fnBOOKING_ID, fsREASON, fsDESCRIPTION, fnORDER, fsSTATUS,
			fsCREATED_BY, fdCREATED_DATE, fsUPDATED_BY, fdUPDATED_DATE,			
			--_sREASON = (CASE WHEN (fsREASON = '') THEN '(未選擇)' ELSE ISNULL(R.fsNAME, '錯誤代碼: '+fsREASON) END), 
			_sSTATUS = (CASE WHEN (fsSTATUS = '') THEN '(未選擇)' ELSE ISNULL(S.fsNAME, '錯誤代碼: '+ fsSTATUS) END)
		FROM
			tbmBOOKING	AS B		
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK001') AS R ON (fsREASON = R.fsCODE)			
				LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'BOOK002') AS S ON (fsSTATUS = S.fsCODE)
		WHERE
			fsCREATED_BY = @fsCREATED_BY AND
			CONVERT(varchar(10), fdCREATED_DATE, 111) >= @SDATE AND
			CONVERT(varchar(10), fdCREATED_DATE, 111) <= @EDATE
			
		ORDER BY
			fnORDER DESC, fnBOOKING_ID
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


