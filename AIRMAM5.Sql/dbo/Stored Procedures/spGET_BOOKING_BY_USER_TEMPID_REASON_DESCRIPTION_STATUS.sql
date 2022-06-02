

-- =============================================
-- 描述:	取出BOOKING主檔資料
-- 記錄:	<2012/04/18><Mihsiu.Chiu><新增本預存>
--			<2012/07/03><Mihsiu.Chiu><新增欄位_sTEMP>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_BOOKING_BY_USER_TEMPID_REASON_DESCRIPTION_STATUS]
	@fsCREATED_BY	nvarchar(50),
	@fsTEMP_ID	nvarchar(20),
	@fsREASON	varchar(2),
	@fsDESCRIPTION	nvarchar(50),
	@fsSTATUS		varchar(2)
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
			((@fsCREATED_BY = '') or (fsCREATED_BY = @fsCREATED_BY))
			and ((@fsTEMP_ID = '') or (fsTEMP_ID = @fsTEMP_ID))
			and ((@fsREASON = '') or (fsREASON = @fsREASON))
			and ((@fsDESCRIPTION = '') or (fsDESCRIPTION LIKE '%'+@fsDESCRIPTION+'%'))
			and ((@fsSTATUS = '') or (fsSTATUS = @fsSTATUS))
			
		ORDER BY
			fnORDER DESC, fnBOOKING_ID
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


