﻿-- =============================================
-- 描述:	取出調用申請中狀態為_C的資料
-- 記錄:	<2012/10/30><Dennis.Wen><新增本預存>
-- 記錄:	<2013/02/19><Eric.Huang><修改本預存將FILE_PATH改為正確的路徑>
-- 記錄:	<2013/12/10><Eric.Huang><因應EBC需要加入流程:DAILY重轉時，採用TRANSCODE流程，故將TYPE為TRANSCODE加入到TMS流程中>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_WORK_BOOKING_COPYFILE_STATUS_C]
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY

		DECLARE @USER VARCHAR(20)
		DECLARE @FILE_SPACE_NAME VARCHAR(100)
		SET @USER = ISNULL((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'CUSTOMER_ID')),'')

		IF (@USER = 'EBC')
			BEGIN
				SET @FILE_SPACE_NAME = '/HVIDEO'
			END
		ELSE IF (@USER = 'EP')
			BEGIN
				SET @FILE_SPACE_NAME = '/1_HVIDEO'
			END
		ELSE IF (@USER = 'FTV')
			BEGIN
				SET @FILE_SPACE_NAME = '/HVideo'
			END
		ELSE
			BEGIN
				SET @FILE_SPACE_NAME = '/HVIDEO'
			END


		SELECT
			fnWORK_ID, fnGROUP_ID, fsTYPE, fsPARAMETERS, WK.fsSTATUS,
			fsPROGRESS, fsPRIORITY, fdSTIME, fdETIME, fsRESULT,
			fsNOTE, WK.fdCREATED_DATE, WK.fsCREATED_BY, WK.fdUPDATED_DATE, WK.fsUPDATED_BY,
			_ITEM_TYPE, _ITEM_ID, _sDESCRIPTION,

			_SM_FILE_PATH = @FILE_SPACE_NAME + (dbo.fnGET_TSM_PATH_BY_FILE_NO((select dbo.fnGET_WORK_PARAMETERS_ANALYZE_BOOKING(fsPARAMETERS,1,'N','N')))),

			--_SM_FILE_PATH = '/HVIDEO' + (dbo.fnGET_TSM_PATH_BY_FILE_NO_EBC((select dbo.fnGET_WORK_PARAMETERS_ANALYZE_BOOKING(fsPARAMETERS,1,'N','N')))),
			--_SM_FILE_PATH = CASE WHEN (fnWORK_ID = 2616) THEN '/HVideo/HVideo/20100226097/201002260970001.mpg'
			--				     WHEN (fnWORK_ID = 2617) THEN '/HVideo/HVideo/20100226097/201002260970002.mpg'
			--				     WHEN (fnWORK_ID = 2618) THEN '/HVideo/HVideo/20100226097/201002260970003.mpg'
			--				     WHEN (fnWORK_ID = 2619) THEN '/HVideo/HVideo/20100226097/201002260970004.mpg'
			--				     WHEN (fnWORK_ID = 2620) THEN '/HVideo/HVideo/20100226097/201002260970005.mpg' END,
			_SM_FILE_SPACE_NAME = @FILE_SPACE_NAME,
			_SM_FILE_NAME = (dbo.fnGET_TSM_PATH_BY_FILE_NO((select dbo.fnGET_WORK_PARAMETERS_ANALYZE_BOOKING(fsPARAMETERS,1,'N','N'))))
			--_SM_FILE_NAME  = CASE WHEN (fnWORK_ID = 2616) THEN '/HVideo/20100226097/201002260970001.mpg'
			--					  WHEN (fnWORK_ID = 2617) THEN '/HVideo/20100226097/201002260970002.mpg'
			--					  WHEN (fnWORK_ID = 2618) THEN '/HVideo/20100226097/201002260970003.mpg'
			--					  WHEN (fnWORK_ID = 2619) THEN '/HVideo/20100226097/201002260970004.mpg'
			--					  WHEN (fnWORK_ID = 2620) THEN '/HVideo/20100226097/201002260970005.mpg' END
		FROM
			tblWORK AS WK
				LEFT JOIN tbmBOOKING AS BK ON (BK.fnBOOKING_ID = WK.fnGROUP_ID)
		WHERE
			-- 2013/12/10 ERIC ++
			--(WK.fsSTATUS = '_C') AND ((WK.fsTYPE IN ('BOOKING', 'COPYFILE', 'NAS', 'AVID') AND (BK.fsSTATUS LIKE '0%')) OR (WK.fsTYPE = 'TRANSCODE'))
			-- 2013/12/10 ERIC --
			WK.fdSTART_WORK_TIME <= GETDATE() AND (WK.fsSTATUS = '_C') AND (WK.fsTYPE IN ('BOOKING', 'COPYFILE', 'NAS', 'AVID')) AND (BK.fsSTATUS LIKE '0%')
		ORDER BY
			BK.fnORDER, WK.fsPRIORITY, BK.fnBOOKING_ID, WK.fnWORK_ID
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END