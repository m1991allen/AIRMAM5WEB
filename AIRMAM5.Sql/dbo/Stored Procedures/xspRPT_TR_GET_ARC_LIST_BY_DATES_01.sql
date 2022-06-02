﻿

-- =============================================
-- Author:		<Dennis.Wen>
-- =============================================
CREATE PROCEDURE [dbo].[xspRPT_TR_GET_ARC_LIST_BY_DATES_01]
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
		DECLARE @RESULT TABLE(
			檔案編號	VARCHAR(16),
			新增時間	DATETIME,
			新增人員	NVARCHAR(50),
			處理備註	NVARCHAR(50),

			影片標題	NVARCHAR(50),
			影片分類	NVARCHAR(10),
			影片語言	NVARCHAR(10),
			影片備註	VARCHAR(200),
			影帶條碼	VARCHAR(20),

			節目編號	VARCHAR(20),
			節目名稱	NVARCHAR(50),
			節目集數	NVARCHAR(10),
			節目段落	VARCHAR(10),
			轉檔編號	VARCHAR(10),
			轉檔時間	DATETIME,

			影片描述	NVARCHAR(MAX),
		    關鍵字詞	NVARCHAR(500),
			字幕參考	NVARCHAR(500),

			圖片URL		NVARCHAR(500)
)

		INSERT @RESULT
		SELECT
			檔案編號 = '20041008_0000001',
			新增時間 = '2014/10/08 13:14:15',
			新增人員 = 'Dennis.Wen',
			處理備註 = '處理備註處理備註處理備註',

			影片標題 = '保重保重保重保重',
			影片分類 = '劈哩趴啦',
			影片語言 = '台語',
			影片備註 = '*媽媽請您也保重',
			影帶條碼 = 'L0000862',

			節目編號 = 'G1997006',
			節目名稱 = '媽媽請您也保重',
			節目集數 = '135',
			節目段落 = '1',
			轉檔編號 = '13579',
			轉檔時間 = '2014/10/08 13:19:27',

			影片描述 = '媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重',
		    關鍵字詞 = '媽媽;保重;',
			字幕參考 = '媽媽請您也保重媽媽請您也保重媽媽請您也保重...',

			圖片URL = 'http://airmam//media/V/K/1010/10/00/10101000001_0001/10101000001_0001_000030000.jpg'

		INSERT @RESULT
		SELECT
			檔案編號 = '20041008_0000002',
			新增時間 = '2014/10/08 13:16:09',
			新增人員 = 'Dennis.Wen',
			處理備註 = '處理備註處理備註處理備註',

			影片標題 = '保重保重保重保重',
			影片分類 = '劈哩趴啦',
			影片語言 = '台語',
			影片備註 = '*媽媽請您也保重',
			影帶條碼 = 'L0000862',

			節目編號 = 'G1997006',
			節目名稱 = '媽媽請您也保重',
			節目集數 = '135',
			節目段落 = '2',
			轉檔編號 = '13579',
			轉檔時間 = '2014/10/08 13:19:27',

			影片描述 = '媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重',
		    關鍵字詞 = '媽媽;保重;',
			字幕參考 = '媽媽請您也保重媽媽請您也保重媽媽請您也保重...',

			圖片URL = 'http://airmam//media/V/K/1010/10/00/10101000001_0001/10101000001_0001_000130000.jpg'

		INSERT @RESULT
		SELECT
			檔案編號 = '20041008_0000003',
			新增時間 = '2014/10/08 13:16:09',
			新增人員 = 'Dennis.Wen',
			處理備註 = '處理備註處理備註處理備註',

			影片標題 = '保重保重保重保重',
			影片分類 = '劈哩趴啦',
			影片語言 = '台語',
			影片備註 = '*媽媽請您也保重',
			影帶條碼 = 'L0000862',

			節目編號 = 'G1997006',
			節目名稱 = '媽媽請您也保重',
			節目集數 = '135',
			節目段落 = '2',
			轉檔編號 = '13579',
			轉檔時間 = '2014/10/08 13:19:27',

			影片描述 = '媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重',
		    關鍵字詞 = '媽媽;保重;',
			字幕參考 = '媽媽請您也保重媽媽請您也保重媽媽請您也保重...',

			圖片URL = 'http://airmam//media/V/K/1010/10/00/10101000001_0001/10101000001_0001_000230000.jpg'

		INSERT @RESULT
		SELECT
			檔案編號 = '20041008_0000004',
			新增時間 = '2014/10/08 13:16:09',
			新增人員 = 'Dennis.Wen',
			處理備註 = '處理備註處理備註處理備註',

			影片標題 = '保重保重保重保重',
			影片分類 = '劈哩趴啦',
			影片語言 = '台語',
			影片備註 = '*媽媽請您也保重',
			影帶條碼 = 'L0000862',

			節目編號 = 'G1997006',
			節目名稱 = '媽媽請您也保重',
			節目集數 = '135',
			節目段落 = '2',
			轉檔編號 = '13579',
			轉檔時間 = '2014/10/08 13:19:27',

			影片描述 = '媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重媽媽請您也保重',
		    關鍵字詞 = '媽媽;保重;',
			字幕參考 = '媽媽請您也保重媽媽請您也保重媽媽請您也保重...',

			圖片URL = 'http://airmam//media/V/K/1010/10/00/10101000001_0001/10101000001_0001_000330000.jpg'


		SELECT * FROM @RESULT


	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH 
END



