

-- =============================================
-- 描述:	取出MATERIAL主檔資料-BY_MARKED_BY FOR EP 
-- 記錄:	<2013/08/16><Albert.Chen><新增本預存><複製spGET_MATERIAL_BY_MARKED_BY_EBC來改的>
--      <2013/09/14><Albert.Chen><修改><依照spGET_WORK_BOOKING_BY_TYPE欄位樣版設定,的方式取值>
--      <2013/10/12><Albert.Chen><修改><加上圖片寬高回去給浮水印判斷用>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_MATERIAL_BY_MARKED_BY_EP]
	@fsMARKED_BY	nvarchar(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
	--START 2013/09/14 Add By Albert 依照spGET_WORK_BOOKING_BY_TYPE欄位樣版設定,的方式取值
	DECLARE @BOOKING_PATH        VARCHAR(300)
	SET @BOOKING_PATH = ISNULL((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'EP_BOOKING_PATH_FROM_LVM')),'')
	--END   2013/09/14 Add By Albert 依照spGET_WORK_BOOKING_BY_TYPE欄位樣版設定,的方式取值

		SELECT 
			fnMATERIAL_ID, fsMARKED_BY, fsTYPE, fsSUB_TYPE, M.fsFILE_NO, fsGROUP,
			fsTITLE, fsDESCRIPTION, fsNOTE, fsPARAMETER,
			fsCREATED_BY, fdCREATED_DATE, fsUPDATED_BY, fdUPDATED_DATE,
			_sTYPE = (CASE WHEN (fsTYPE = '') THEN '(未選擇)' ELSE ISNULL(T.fsNAME, '錯誤代碼: '+fsTYPE) END), 
			_sSUB_TYPE = (CASE WHEN (fsSUB_TYPE = '') THEN '(未選擇)' ELSE ISNULL(S.fsNAME, '錯誤代碼: '+fsSUB_TYPE) END),
			--_sVIDEO_MAX_TIME = (case when (fsTYPE = 'V') then datediff(ms,convert(datetime,Replace(V.fsBEG_TIMECODE,';',':')),convert(datetime, Replace(V.fsEND_TIMECODE,';',':'))) else '' end)			
			_sVIDEO_MAX_TIME = CASE WHEN (fsTYPE = 'V') THEN CAST(V.fdDURATION AS VARCHAR(10))
									WHEN (fsTYPE = 'A') THEN CAST(A.fdDURATION AS VARCHAR(10))
									ELSE '' END
			
								/*(case when (fsTYPE = 'V') then (cast(substring(V.fsEND_TIMECODE,1,2) as int)*60*60 +
								CAST(substring(V.fsEND_TIMECODE,4,2) as int)*60 + 
								cast(substring(V.fsEND_TIMECODE,7,2) as int) +
								cast(substring(V.fsEND_TIMECODE,10,2) as float)/30)  -
								(cast(substring(V.fsBEG_TIMECODE,1,2) as int)*60*60 +
							     CAST(substring(V.fsBEG_TIMECODE,4,2) as int)*60 + 
							     cast(substring(V.fsBEG_TIMECODE,7,2) as int) +
							     cast(substring(V.fsBEG_TIMECODE,10,2) as float)/30)
							     else '' end)*/
			,_sFILE_URL	= dbo.fnGET_FILE_URL_BY_TYPE_AND_FILE_NO(fsTYPE,M.fsFILE_NO,'L')
			--START 2013/04/27 Update By Albert 使用fxMEDIA_INFO來判斷是否為手動上傳或每日新聞(若為true代表是手動上傳)
			,_sIS_MANUALLY_UPLOAD=CASE WHEN ISNULL(V.fxMEDIA_INFO,'')='' THEN 'false' ELSE 'true' END
			--END   2013/04/27 Upate By Albert  使用fxMEDIA_INFO來判斷是否為手動上傳或每日新聞(若為true代表是手動上傳)
			,_sFILE_PATH_H = (V.[fsFILE_PATH_H] + V.fsFILE_NO + '_H.' + V.[fsFILE_TYPE_H])

			--START 2013/09/14 Add By Albert 依照spGET_WORK_BOOKING_BY_TYPE欄位樣版設定,的方式取值
			---         SET @TMP=VDO.fsFILE_PATH_H + VDO.fsFILE_NO + '_H.' + VDO.fsFILE_TYPE_H
			----        SET @TMPCNT = (SELECT CHARINDEX ('\HVIDEO',@TMP))
			,_sFromFILE_PATH_NAME=
			---SELECT @TMP = ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H]) FROM tbmARC_VIDEO WHERE fsFILE_NO = VDO.fs	
			CASE WHEN CHARINDEX ('\HVIDEO',V.fsFILE_PATH_H + V.fsFILE_NO + '_H.' + V.fsFILE_TYPE_H)>0
			THEN
			     --SET @TMP2 = SUBSTRING(@TMP,1,@TMPCNT)
		         --SET @fromFILE_PATH_NAME = REPLACE (@TMP, @TMP2, @BOOKING_PATH)
			     REPLACE (V.fsFILE_PATH_H + V.fsFILE_NO + '_H.' + V.fsFILE_TYPE_H, SUBSTRING(V.fsFILE_PATH_H + V.fsFILE_NO + '_H.' + V.fsFILE_TYPE_H,1,CHARINDEX ('\HVIDEO',V.fsFILE_PATH_H + V.fsFILE_NO + '_H.' + V.fsFILE_TYPE_H)), @BOOKING_PATH)
			ELSE
	    	----		SELECT @fromFILE_PATH_NAME = ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H])
			----		FROM tbmARC_VIDEO
			----		WHERE (fsTYPE = 'V') AND (fsFILE_NO = @FILE_NO)		
                 V.fsFILE_PATH_H + V.fsFILE_NO + '_H.' + V.fsFILE_TYPE_H
			END
			--END   2013/09/14 Add By Albert 依照欄位樣版設定
        	--START 2013/10/12 Add By Albert 加上圖片寬高回去給浮水印判斷用
			,P.fnWIDTH  AS _sPHOTO_WIDTH
			,p.fnHEIGHT  AS _sPHOTO_HEIGHT
			--END   2013/10/12 Add By Albert 加上圖片寬高回去給浮水印判斷用
		FROM
			tbmMATERIAL	AS M		
			LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'MTRL001') AS T ON (fsTYPE = T.fsCODE)			
			LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'MTRL002') AS S ON (fsSUB_TYPE = S.fsCODE)						
			--LEFT JOIN (select fsBEG_TIMECODE, fsFILE_NO, fsEND_TIMECODE  from tbmARC_VIDEO) as V on (M.fsTYPE = 'V' and M.fsFILE_NO = V.fsFILE_NO)
			--START 2013/04/27 Update By Albert 使用fxMEDIA_INFO來判斷是否為手動上傳或每日新聞(若為true代表是手動上傳)
				--LEFT JOIN (select fsFILE_NO, fdDURATION  from tbmARC_VIDEO) as V on (M.fsTYPE = 'V' and M.fsFILE_NO = V.fsFILE_NO)
			LEFT JOIN (select fsFILE_NO, fdDURATION,fxMEDIA_INFO,fsFILE_PATH_H,fsFILE_TYPE_H  from tbmARC_VIDEO) as V on (M.fsTYPE = 'V' and M.fsFILE_NO = V.fsFILE_NO)
			--END   2013/04/27 Upate By Albert  使用fxMEDIA_INFO來判斷是否為手動上傳或每日新聞(若為true代表是手動上傳)
			LEFT JOIN (select fsFILE_NO, fdDURATION  from tbmARC_AUDIO) as A on (M.fsTYPE = 'A' and M.fsFILE_NO = A.fsFILE_NO)
			--START 2013/10/12 Add By Albert 加上圖片寬高回去給浮水印判斷用
			LEFT JOIN (select fsFILE_NO, fnWIDTH, fnHEIGHT  from tbmARC_PHOTO) as P on (M.fsTYPE = 'P' and M.fsFILE_NO = P.fsFILE_NO)
			--END   2013/10/12 Add By Albert 加上圖片寬高回去給浮水印判斷用
		WHERE
			(fsMARKED_BY = @fsMARKED_BY) and (fsTYPE in ('V','A','P','D'))
		ORDER BY
			fnMATERIAL_ID DESC
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


