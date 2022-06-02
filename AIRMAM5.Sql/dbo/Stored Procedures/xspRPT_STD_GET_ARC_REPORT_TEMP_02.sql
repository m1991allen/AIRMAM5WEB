

-- =============================================
-- 描述:	暫時產生報表用2
-- 記錄:	<2013/10/25><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspRPT_STD_GET_ARC_REPORT_TEMP_02]
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
	DECLARE @tblRESULT TABLE(_NAME NVARCHAR(50), _DESCRIPTION NVARCHAR(50), _TOTAL  NVARCHAR(50))

---

/*影片備份統計*/
INSERT	@tblRESULT
SELECT '影片備份統計',
'截至: 2013/10/25',
'筆數: ' + '12479'

/*圖片備份統計*/
INSERT	@tblRESULT
SELECT '圖片備份統計',
'截至: 2013/10/25',
'筆數: ' + '396574'

/*聲音備份統計*/
INSERT	@tblRESULT
SELECT '聲音備份統計',
'截至: 2013/10/25',
'筆數: ' + '189'

---

SELECT * FROM @tblRESULT

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



