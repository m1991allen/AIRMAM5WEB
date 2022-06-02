-- =============================================
-- Author:		<Albert.Chen>
-- Create date: <2013/09/27>
-- Description:	<查詢紀錄明細表>
-- =============================================
CREATE PROCEDURE [dbo].[xspRPT_STD_GET_ARC_LIST_BY_DATES_03]
	@SDATE		DATE,
	@EDATE		DATE,
	@NAME	NVARCHAR(50)
AS
BEGIN
		--DECLARE @SDATE	DATE
		--DECLARE @EDATE	DATE
		--SET @SDATE	 = '2013/01/01'	--起始日期
		--SET @EDATE	 = '2014/12/31'	--結束日期
		--DECLARE @NAME NVARCHAR(50) = 'er'		         --查詢人員 (帳號或姓名的部分字串都可以)
	SET NOCOUNT ON;

	BEGIN TRY
	SELECT
		查詢時間 = SH.fdCREATED_DATE,
		查詢人員 = ISNULL(SH.fsCREATED_BY + ' (' + U.fsNAME + ')', '') ,
		查詢字串 = fsSTATEMENT 
	FROM
		tblSRH AS SH
		LEFT JOIN tbmUSERS	AS U ON (U.fsLOGIN_ID = SH.fsCREATED_BY)
	WHERE
		(fsSTATEMENT <> '')
	AND	(CONVERT(VARCHAR(10),SH.fdCREATED_DATE,111) BETWEEN @SDATE AND @EDATE)
	AND (@NAME='' OR (ISNULL(SH.fsCREATED_BY + ' (' + U.fsNAME + ')', '') LIKE '%'+@NAME+'%'))
	ORDER BY SH.fdCREATED_DATE, SH.fsSTATEMENT

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH 
END
