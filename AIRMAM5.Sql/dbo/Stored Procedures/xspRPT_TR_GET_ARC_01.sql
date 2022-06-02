
-- =============================================
-- Author:		<Dennis.Wen>
-- =============================================
CREATE PROCEDURE [dbo].[xspRPT_TR_GET_ARC_01]
	@SDATE		DATE,
	@EDATE		DATE,
	@QUERY_BY	NVARCHAR(50)
AS
BEGIN
	--DECLARE
	--	@SDATE	DATE,
	--	@EDATE	DATE

	-------

	--SELECT
	--	@SDATE	= '2012/12/01',
	--	@EDATE	= '2012/12/31' 
	SET NOCOUNT ON;

	BEGIN TRY
		SELECT
			COL1 = '\\Ams01\mam_media\V\L\2222\22\22\',
			COL2 = '\\Ams01\mam_media\V\L\2222\22\22\22222222_0020001_L.mp4',
			KF1 = 'http://airmam//media/V/K/2222/22/22/22222222_0020001/22222222_0020001_000000040.jpg',
			KF2 = 'http://airmam//media/V/K/2222/22/22/22222222_0020001/22222222_0020001_000020040.jpg',
			KF3 = 'http://airmam//media/V/K/2222/22/22/22222222_0020001/22222222_0020001_000040040.jpg'
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH 
END


