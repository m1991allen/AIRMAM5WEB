-- =============================================
-- Author:		<Albert.Chen>
-- Create date: <2013/09/27>
-- Description:	<媒體調用紀錄明細>
-- =============================================
CREATE PROCEDURE [dbo].[xspRPT_STD_GET_ARC_LIST_BY_DATES_06]
	@SDATE	DATE ,	        --起始日期
	@EDATE	DATE ,          --結束日期
	@UID	NVARCHAR(50) ,	--傳入空格時取所有人, 傳入字串時, 抓出帳號有包含字串的	
	@DESC	VARCHAR(1) 		--若傳入Y, 會改成用日期倒序, 否則一律用日期正序
AS
BEGIN
		--SET @SDATE = '2013/01/01'	--起始日期
		--SET @EDATE = '2013/12/31'	--結束日期
		--SET @UID=''
		--SET DESC='Y'
	SET NOCOUNT ON;

	BEGIN TRY
		IF(@DESC = 'Y')
		BEGIN
			SELECT
				系統時間 = L.fdCREATED_DATE
				,紀錄分類 = LEFT(CO.fsNAME,6)
				,操作人員 = L.fsCREATED_BY + ' (' + U.fsNAME + ')'
				,作業描述 = L.fsDESCRIPTION
				,備註 = L.fsNOTE 
			FROM
				tblLOG AS L
				LEFT JOIN tbmUSERS     AS U ON (U.fsLOGIN_ID = L.fsCREATED_BY)
				LEFT JOIN tbzCODE	AS CO ON (CO.fsCODE_ID = 'LOG001') AND (CO.fsCODE = L.fsTYPE)
			WHERE
				(L.fsCREATED_BY LIKE '%'+@UID+'%' OR @UID='')
			AND	(CONVERT(VARCHAR(10),L.fdCREATED_DATE,111) BETWEEN @SDATE AND @EDATE)
			ORDER BY
				L.fdCREATED_DATE DESC
		END
	ELSE
		BEGIN
			SELECT
				系統時間 = L.fdCREATED_DATE
				,紀錄分類 = LEFT(CO.fsNAME,6)
				,操作人員 = L.fsCREATED_BY + ' (' + U.fsNAME + ')'
				,作業描述 = L.fsDESCRIPTION
				,備註 = L.fsNOTE 
			FROM
				tblLOG AS L
				LEFT JOIN tbmUSERS     AS U ON (U.fsLOGIN_ID = L.fsCREATED_BY)
				LEFT JOIN tbzCODE	AS CO ON (CO.fsCODE_ID = 'LOG001') AND (CO.fsCODE = L.fsTYPE)
			WHERE
				(L.fsCREATED_BY LIKE '%'+@UID+'%' OR @UID='')
			AND	(CONVERT(VARCHAR(10),L.fdCREATED_DATE,111) BETWEEN @SDATE AND @EDATE)
			ORDER BY
				L.fdCREATED_DATE ASC
		END

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH 
END
