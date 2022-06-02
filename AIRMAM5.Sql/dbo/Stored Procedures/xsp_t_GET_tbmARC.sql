

-- =============================================
-- 描述:	取出t_tbmARC_INDEX主檔資料
-- 記錄:	<2012/04/30><Mihsiu.Chiu><新增本預存>
--			<2012/07/06><Mihsiu.Chiu><for english version>
-- =============================================
CREATE PROCEDURE [dbo].[xsp_t_GET_tbmARC]
	@fnINDEX_ID		bigint
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			fnINDEX_ID, fsFILE_NO, T.fsTYPE, fsREASON, fsSTATUS,
			_sSTATUS = (CASE WHEN (fsSTATUS = '') THEN 'Temporarily Deleted' --暫刪除
						ELSE ISNULL(C.fsNAME, '錯誤代碼: '+fsSTATUS) END),
			_sTYPE = (CASE WHEN (fsTYPE = '') THEN '(未選擇)' 
						ELSE ISNULL(P.fsNAME, '錯誤代碼: '+fsTYPE) END),
			fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY
		FROM
			t_tbmARC_INDEX AS T
			LEFT JOIN (SELECT fsCODE, fsNAME from tbzCODE where fsCODE_ID='ARC006') C on T.fsSTATUS = C.fsCODE
			LEFT JOIN (SELECT fsCODE, fsNAME from tbzCODE where fsCODE_ID='ARC004') P on T.fsTYPE = P.fsCODE
		WHERE
			fnINDEX_ID		= @fnINDEX_ID
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


