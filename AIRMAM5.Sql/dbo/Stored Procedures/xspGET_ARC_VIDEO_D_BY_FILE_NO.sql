﻿

-- =============================================
-- 描述:	取出ARC_VIDEO_D 入庫項目-影片明細檔 資料
-- 記錄:	<2012/02/29>Dennis.Wen><新增本預存> 
--		<2012/05/21><Dennis.Wen><一堆欄位調整>
--		<2012/07/24><Eric.Huang><新增欄位 fnRESP_ID>
-- 記錄:<2014/08/21><Eric.Huang><新增 fsKEYWORD>
-- 記錄:<2016/11/14><David.Sin><調整欄位>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_ARC_VIDEO_D_BY_FILE_NO]
	@fsFILE_NO	VARCHAR(16)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 

			fsFILE_NO,fnSEQ_NO,fsDESCRIPTION,fdBEG_TIME, fdEND_TIME,
			fdCREATED_DATE,fsCREATED_BY , fdUPDATED_DATE, fsUPDATED_BY
			--fsFILE_NO, fsSUBJECT_ID, fsTYPE, fsID, fnEPISODE, fsSUPERVISOR, fsTITLE, fsDESCRIPTION,
			--fsFILE_STATUS, fsOLD_FILE_NAME, fsCHANGE_FILE_NO, fnDIR_ID, fsCHANNEL_ID, fsARC_ID, 
			--/*fsBEG_TIMECODE, fsEND_TIMECODE,*/ fsFROM, fnSEQ_NO, fsKEYFRAME_NO,--201205一堆欄位調整
		 --   fsATTRIBUTE1, fsATTRIBUTE2, fsATTRIBUTE3, 
			--fsATTRIBUTE4, fsATTRIBUTE5, fsATTRIBUTE6, fsATTRIBUTE7, fsATTRIBUTE8, fsATTRIBUTE9, fsATTRIBUTE10,
			--fsATTRIBUTE11, fsATTRIBUTE12, fsATTRIBUTE13, fsATTRIBUTE14, fsATTRIBUTE15, fsATTRIBUTE16, fsATTRIBUTE17,
			--fsATTRIBUTE18, fsATTRIBUTE19, fsATTRIBUTE20, fsATTRIBUTE21, fsATTRIBUTE22, fsATTRIBUTE23, fsATTRIBUTE24,
			--fsATTRIBUTE25, fsATTRIBUTE26, fsATTRIBUTE27, fsATTRIBUTE28, fsATTRIBUTE29, fsATTRIBUTE30, fsATTRIBUTE31,
			--fsATTRIBUTE32, fsATTRIBUTE33, fsATTRIBUTE34, fsATTRIBUTE35, fsATTRIBUTE36, fsATTRIBUTE37, fsATTRIBUTE38, 
			--fsATTRIBUTE39, fsATTRIBUTE40, fsATTRIBUTE41, fsATTRIBUTE42, fsATTRIBUTE43, fsATTRIBUTE44, fsATTRIBUTE45,
			--fsATTRIBUTE46, fsATTRIBUTE47, fsATTRIBUTE48, fsATTRIBUTE49, fsATTRIBUTE50, fsGROUPS, fdCREATED_DATE,
			--fsCREATED_BY , fdUPDATED_DATE, fsUPDATED_BY,
			
			--_sDIR_PATH = dbo.fnGET_DIR_PATH_BY_SUBJECT_ID(fsSUBJECT_ID),
			--_sSUBJ_PATH = dbo.fnGET_SUBJ_PATH_BY_SUBJECT_ID(fsSUBJECT_ID)
			
			--,fdBEG_TIME, fdEND_TIME, fdDURATION --201205一堆欄位調整
			--,_sVIDEO_MAX_TIME = (SELECT fdDURATION FROM tbmARC_VIDEO WHERE (fsFILE_NO = @fsFILE_NO))
			--,fnRESP_ID
			--,fsKEYWORD	--2014/08/21 eric

		FROM
			tbmARC_VIDEO_D
			
		WHERE
			(fsFILE_NO = @fsFILE_NO) AND [fnSEQ_NO] > 0
		ORDER BY 
			fdBEG_TIME, fdEND_TIME DESC, fsFILE_NO

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


