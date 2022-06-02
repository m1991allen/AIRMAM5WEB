--=========================================
-- 描述:	傳入V/A/P/D回傳註冊工作項中,優先順序高的未處理工作
-- 記錄:	<2019/06/26><David.Sin><重新修改預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_WORK_BOOKING_BY_TYPE]
	@TYPE VARCHAR(1)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY

			/*取得要處理的WORK*/
			DECLARE @fnWORK_ID BIGINT = 
				ISNULL(
				(
					SELECT TOP 1 fnWORK_ID 
					FROM tblWORK A JOIN tbmBOOKING B ON A.fnGROUP_ID = B.fnBOOKING_ID 
					WHERE 
						A.fdSTART_WORK_TIME <= GETDATE() AND
						A.fsSTATUS LIKE '0%' AND 
						SUBSTRING(A.fsPARAMETERS,1,1) = @TYPE AND 
						A.fsTYPE = 'BOOKING' AND 
						B.fsSTATUS LIKE '0%'
					ORDER BY B.fnORDER, CONVERT(INT,A.fsPRIORITY), A.fnWORK_ID
				),-1)
			
			IF(@fnWORK_ID > 0)
			BEGIN
				DECLARE @fsPARAMETERS VARCHAR(MAX) = (SELECT fsPARAMETERS FROM tblWORK WHERE fnWORK_ID = @fnWORK_ID)
				DECLARE @fsFILE_NO VARCHAR(16) = (SELECT dbo.fnGET_ITEM_BY_INDEX(@fsPARAMETERS,1) FROM tblWORK WHERE fnWORK_ID = @fnWORK_ID)
				DECLARE @fsPATH_TYPE VARCHAR(5) = (SELECT dbo.fnGET_ITEM_BY_INDEX(@fsPARAMETERS,2) FROM tblWORK WHERE fnWORK_ID = @fnWORK_ID)
				DECLARE @fsFOLDER VARCHAR(50) = (SELECT dbo.fnGET_ITEM_BY_INDEX(@fsPARAMETERS,3) FROM tblWORK WHERE fnWORK_ID = @fnWORK_ID)

				IF((SELECT COUNT(*) FROM tbmARC_VIDEO WHERE (@TYPE = 'V' AND fsFILE_NO = @fsFILE_NO))+
					(SELECT COUNT(*) FROM tbmARC_AUDIO WHERE (@TYPE = 'A' AND fsFILE_NO = @fsFILE_NO))+
					(SELECT COUNT(*) FROM tbmARC_PHOTO WHERE (@TYPE = 'P' AND fsFILE_NO = @fsFILE_NO))+
					(SELECT COUNT(*) FROM tbmARC_DOC WHERE (@TYPE = 'D' AND fsFILE_NO = @fsFILE_NO)) = 0)
				BEGIN
					/*若取不到媒體檔資訊,可能是被移除了,排程取消*/
					UPDATE tblWORK
					SET fsSTATUS = 'C0'
					WHERE (fnWORK_ID = @fnWORK_ID)
				END
				ELSE
				BEGIN
					
					UPDATE tblWORK
					SET fsSTATUS = '01'
					WHERE (fnWORK_ID = @fnWORK_ID)

					SELECT
						@fnWORK_ID AS WORK_ID,
						@fsFILE_NO AS FILE_NO,
						CASE
							WHEN @TYPE = 'V' THEN (SELECT fsFILE_PATH_H + fsFILE_NO + '_H.' + fsFILE_TYPE_H FROM tbmARC_VIDEO WHERE fsFILE_NO = @fsFILE_NO)
							WHEN @TYPE = 'A' THEN (SELECT fsFILE_PATH_H + fsFILE_NO + '_H.' + fsFILE_TYPE_H FROM tbmARC_AUDIO WHERE fsFILE_NO = @fsFILE_NO)
							WHEN @TYPE = 'P' THEN (SELECT fsFILE_PATH_H + fsFILE_NO + '_H.' + fsFILE_TYPE_H FROM tbmARC_PHOTO WHERE fsFILE_NO = @fsFILE_NO)
							WHEN @TYPE = 'D' THEN (SELECT fsFILE_PATH + fsFILE_NO + '.' + fsFILE_TYPE FROM tbmARC_DOC WHERE fsFILE_NO = @fsFILE_NO)
						END AS fromFILE_PATH_NAME,
						CASE
							WHEN @fsPATH_TYPE = '1' THEN 'UNC'
							WHEN @fsPATH_TYPE = '2' THEN 'FTP'
							ELSE 'UNC'
						END AS toFILE_PATH_TYPE,
						CASE
							WHEN ISNULL(U.fsBOOKING_TARGET_PATH,'') = '' THEN CODE1.fsSET
							ELSE U.fsBOOKING_TARGET_PATH
						END + B.fsCREATED_BY + @fsFOLDER + '\' AS toFILE_PATH,
						REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
							CASE
								WHEN @TYPE = 'V' THEN (SELECT fsTITLE FROM tbmARC_VIDEO WHERE fsFILE_NO = @fsFILE_NO)
								WHEN @TYPE = 'A' THEN (SELECT fsTITLE FROM tbmARC_AUDIO WHERE fsFILE_NO = @fsFILE_NO)
								WHEN @TYPE = 'P' THEN (SELECT fsTITLE FROM tbmARC_PHOTO WHERE fsFILE_NO = @fsFILE_NO)
								WHEN @TYPE = 'D' THEN (SELECT fsTITLE FROM tbmARC_DOC WHERE fsFILE_NO = @fsFILE_NO)
								ELSE @fsFILE_NO + '_' + B.fsCREATED_BY
							END,'*',''),'|',''),'\',''),':',''),'"',''),'>',''),'<',''),'?',''),'/',''
						) AS fromFILE_PATH_NAME,
						A.fsSTATUS AS STATUS_0,
						'C' AS MTS_TOOL,
						dbo.fnGET_ITEM_BY_INDEX(A.fsPARAMETERS, 4) AS PROFILE_NAME,
						dbo.fnGET_ITEM_BY_INDEX(A.fsPARAMETERS, 5) AS BEG_TIME,
						dbo.fnGET_ITEM_BY_INDEX(A.fsPARAMETERS, 6) AS END_TIME,
						dbo.fnGET_ITEM_BY_INDEX(A.fsPARAMETERS, 7) AS WIDTH,
						dbo.fnGET_ITEM_BY_INDEX(A.fsPARAMETERS, 8) AS HEIGHT,
						dbo.fnGET_ITEM_BY_INDEX(A.fsPARAMETERS, 10) AS WATERMARK,
						dbo.fnGET_ITEM_BY_INDEX(A.fsPARAMETERS, 11) AS WATERMARK_X_OFFSET,
						dbo.fnGET_ITEM_BY_INDEX(A.fsPARAMETERS, 12) AS WATERMARK_Y_OFFSET,
						dbo.fnGET_ITEM_BY_INDEX(A.fsPARAMETERS, 13) AS WATERMARK_SCALE,
						90 AS WATERMARK_ALPHA
					FROM
						tblWORK A
							JOIN tbmBOOKING B ON A.fnGROUP_ID = B.fnBOOKING_ID
							--JOIN tbmBOOKING_T C ON B.fnTEMP_ID = C.fnBOOK_T_ID
							JOIN tbmUSERS U ON B.fsCREATED_BY = U.fsLOGIN_ID
							LEFT JOIN tbzCODE CODE1 ON CODE1.fsCODE = B.fsPATH AND CODE1.fsCODE_ID = 'BOOKING_PATH'
					WHERE
						A.fnWORK_ID = @fnWORK_ID
			END
			--DECLARE
			--	@WORK_ID		BIGINT,
			--	@PAREMETERS		NVARCHAR(200),
			--	@STATUS_0		VARCHAR(2),
			--	@CREATED_BY		NVARCHAR(50),
					
			--	@FILE_NO		VARCHAR(16),				
			--	@PATH_TYPE		VARCHAR(1),
			--	@FOLDER			VARCHAR(50),			
			--	@PROFILE_NAME	NVARCHAR(128),				
			--	@BEG_TIME		VARCHAR(20),
			--	@END_TIME		VARCHAR(20),
			--	@WIDTH			VARCHAR(10),
			--	@HEIGHT			VARCHAR(10),
				
			--	@fromFILE_PATH_NAME NVARCHAR(600),
			--	@toFILE_PATH_TYPE	NVARCHAR(10),
			--	@toFILE_PATH		NVARCHAR(500),
			--	@toFILE_NAME		NVARCHAR(100),
			--	@WM_PATH        VARCHAR(100)	-- 2012/10/18
				
			--SELECT @PROFILE_NAME = '', @BEG_TIME = '', @END_TIME = '', @WIDTH = '', @HEIGHT = '', @toFILE_PATH = '', @toFILE_NAME = ''
				
			--/*取出待處理項(STATUS=0開頭的)中優先順序高的轉檔工作WORK_ID*/
			--SET @WORK_ID = ISNULL((	SELECT	TOP 1 WK.fnWORK_ID
			--						FROM	tblWORK AS WK
			--						LEFT JOIN tbmBOOKING AS BK ON (BK.fnBOOKING_ID = WK.fnGROUP_ID)
			--						WHERE	(WK.fsSTATUS LIKE '0%') AND (WK.fsPARAMETERS LIKE 'V' + '%')	--9開頭表示完成 >9的也已結束 <9表示未完成
			--						         AND (WK.fsTYPE = 'BOOKING') AND (BK.fsSTATUS LIKE '0%')
			--						ORDER BY BK.fnORDER, BK.fnBOOKING_ID, WK.fsPRIORITY, WK.fnWORK_ID ),-1)
									 
			--IF(@WORK_ID <> -1)
			--BEGIN
			--	SELECT
			--		@TYPE	 = dbo.fnGET_ITEM_BY_INDEX(fsPARAMETERS,0),
			--		@FILE_NO = dbo.fnGET_ITEM_BY_INDEX(fsPARAMETERS,1)
					
			--	FROM
			--		tblWORK
			--	WHERE
			--		(fnWORK_ID = @WORK_ID)			
						
			--	/*若取不到媒體檔資訊,可能是被移除了,排程取消*/
			--	IF(((SELECT COUNT(*) FROM tbmARC_VIDEO WHERE (@TYPE = 'V' AND fsFILE_NO = @FILE_NO))+
			--	   (SELECT COUNT(*) FROM tbmARC_AUDIO WHERE (@TYPE = 'A' AND fsFILE_NO = @FILE_NO))+
			--	   (SELECT COUNT(*) FROM tbmARC_PHOTO WHERE (@TYPE = 'P' AND fsFILE_NO = @FILE_NO))+
			--	   (SELECT COUNT(*) FROM tbmARC_DOC WHERE (@TYPE = 'D' AND fsFILE_NO = @FILE_NO)) = 0))
			--	BEGIN
			--		UPDATE tblWORK
			--		SET fsSTATUS = 'C0'
			--		WHERE (fnWORK_ID = @WORK_ID)
					
			--		SET @WORK_ID = -1
			--	END				
			  		
			--	/*依據WORK_ID取回相關參數*/
			--	SELECT @PAREMETERS = fsPARAMETERS, @STATUS_0 = fsSTATUS, @CREATED_BY = fsCREATED_BY
			--	FROM tblWORK WHERE (fnWORK_ID = @WORK_ID)			
				
			--	/*將狀態修改為01*/
			--	UPDATE	tblWORK
			--	SET		fsSTATUS = '01'
			--	WHERE	(fnWORK_ID = @WORK_ID) 
				
			--	/*由參數解析為要處理的FILE_NO與SUBJ_ID*/
			--	SELECT
			--		@FILE_NO		= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 1),	
			--		@PATH_TYPE		= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 2),
			--		@FOLDER			= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 3),
			--		@PROFILE_NAME	= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 4),
			--		@BEG_TIME		= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 5),
			--		@END_TIME		= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 6),
			--		@WIDTH			= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 7),
			--		@HEIGHT			= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 8),
			--		@WM_PATH		=
			--						CASE dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 9)
			--							WHEN 'Y' THEN '\\172.20.144.49\mam_news\Logo\FTVLogo.png'
			--							ELSE ''
			--						END
			--	/*依照不同@PATH_TYPE呼叫不同預存組路徑*/
			--	SET @toFILE_PATH = CASE @PATH_TYPE
			--						WHEN '1' THEN ('\' + REPLACE(dbo.fnGET_BOOKING_OUTPUT_PATH1(@TYPE, @CREATED_BY, @FOLDER),'\\','\'))
			--						WHEN '2' THEN (dbo.fnGET_BOOKING_OUTPUT_PATH2(@TYPE, @CREATED_BY, @FOLDER))
			--						ELSE '' END		
									
			--	SET @toFILE_PATH_TYPE = CASE @PATH_TYPE
			--						WHEN '1' THEN 'UNC'
			--						WHEN '2' THEN 'FTP'
			--						ELSE '' END	 
									
			--	/*取出來源路徑檔案名稱*/
			--	--SELECT @fromFILE_PATH_NAME = ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H])
			--	--FROM tbmARC_VIDEO
			--	--WHERE (@TYPE = 'V') AND (fsFILE_NO = @FILE_NO)		
			--	--\ / : * ?「 < > |
			--	--SELECT @fromFILE_PATH_NAME = (REPLACE(tbmARC_VIDEO.[fsFILE_PATH_H],'\\Newsmam1\mamdfs','\\ams03') + tbmARC_VIDEO_D.fsATTRIBUTE1 + '.' + tbmARC_VIDEO.[fsFILE_TYPE_H])
			--	SELECT @fromFILE_PATH_NAME = (SELECT tbmARC_VIDEO.[fsFILE_PATH_H] + tbmARC_VIDEO_D.fsATTRIBUTE1 + '.' + tbmARC_VIDEO.[fsFILE_TYPE_H])
			--	--SELECT @fromFILE_PATH_NAME = REPLACE((SELECT tbmARC_VIDEO.[fsFILE_PATH_H] + tbmARC_VIDEO_D.fsATTRIBUTE1 + '.' + tbmARC_VIDEO.[fsFILE_TYPE_H]),'\\NEWSMAM1\MAMDFS','\\172.20.144.87')
			--	FROM tbmARC_VIDEO JOIN tbmARC_VIDEO_D ON tbmARC_VIDEO.fsFILE_NO = tbmARC_VIDEO_D.fsFILE_NO
			--	WHERE (@TYPE = 'V') AND (tbmARC_VIDEO.fsFILE_NO = @FILE_NO)		

			--	SELECT @fromFILE_PATH_NAME = ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H])
			--	FROM tbmARC_AUDIO
			--	WHERE (@TYPE = 'A') AND (fsFILE_NO = @FILE_NO)
	 
			--	SELECT @fromFILE_PATH_NAME = ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H])
			--	FROM tbmARC_PHOTO
			--	WHERE (@TYPE = 'P') AND (fsFILE_NO = @FILE_NO)	
	 		
			--	SELECT @fromFILE_PATH_NAME = ([fsFILE_PATH_2] + fsFILE_NO + '_L.' + [fsFILE_TYPE_2])
			--	FROM tbmARC_DOC
			--	WHERE (@TYPE = 'D') AND (fsFILE_NO = @FILE_NO)		
							

			--	/*===============  下面這一段要想辦法換掉  ===============*/
			--	DECLARE
			--		@fsTYPE		VARCHAR(10),
			--		@fsNAME		NVARCHAR(10),
			--		@fsHEAD		VARCHAR(10),
			--		@fsBODY		VARCHAR(10),
			--		@fsNO_L		INT,
			--		@BY			VARCHAR(50)
					
			--	SELECT
			--		@fsTYPE		= 'BOOKING', 
			--		@fsNAME		= '借調媒體檔', 
			--		@fsHEAD		= CONVERT(char(8), getdate(), 112), 
			--		@fsBODY		= '_', 
			--		@fsNO_L		= 5, 
			--		@BY			= @CREATED_BY

			--	/*還沒有此筆設定時先新增*/
			--	IF NOT EXISTS(SELECT * FROM tblNO WHERE (fsTYPE = @fsTYPE) AND (fsHEAD = @fsHEAD))
			--	BEGIN
			--		BEGIN TRY
			--			INSERT	tblNO
			--			SELECT	@fsTYPE, @fsNAME, @fsHEAD, @fsBODY, 0, @fsNO_L, GETDATE(), @BY, '1900/01/01', ''
			--		END TRY
			--		BEGIN CATCH
			--		END CATCH
			--	END

			--	DECLARE	@strRESULT	VARCHAR(100) = '',
			--			@intPRESENT	INT,
			--			@intNEW	INT,
			--			@blGETOK	BIT = 0,
			--			@dateTIME	DATETIME

			--	WHILE(@blGETOK = 0)
			--	BEGIN
			--		SET @dateTIME = GETDATE(); --先取出目前資料庫中的資料時間
			--		SET @intPRESENT =  (SELECT fsNO FROM tblNO WHERE (fsTYPE = @fsTYPE) AND (fsHEAD = @fsHEAD))
			--		SET @intNEW = @intPRESENT + 1
			--		SET @strRESULT = CAST(@intNEW AS VARCHAR(100)) 
					
			--		/*修改回資料庫同時,檢查是否資料庫中的時間是比我取號時舊的資料*/
			--		UPDATE	tblNO
			--		SET		fsNO = @intNEW
			--		WHERE	(fsTYPE = @fsTYPE) AND (fsHEAD = @fsHEAD) AND 
			--				(fdCREATED_DATE <= @dateTIME) AND (fdUPDATED_DATE <= @dateTIME)	
					
			--		IF (@@ROWCOUNT > 0)
			--			BEGIN
			--				/*確實有修改到比較舊的資料時*/
			--				SET @blGETOK = 1
			--			END
			--	END

			--	SET @strRESULT = '00000000000000000000000000000000000000000000000000' + CAST(@intNEW AS VARCHAR(50)) 
			--	SET @strRESULT = SUBSTRING(@strRESULT, LEN(@strRESULT)-@fsNO_L+1, @fsNO_L) 

			--	SET @toFILE_NAME = (SELECT REPLACE(REPLACE(REPLACE(REPLACE(fsTITLE,'"',''),'!',''),':',''),'?','') FROM tbmARC_VIDEO WHERE fsFILE_NO = @FILE_NO)--@fsHEAD + @fsBODY + @strRESULT
					
				
			--	/*===============  上面這一段要想辦法換掉  ===============*/
				
			--	--SET @WM_PATH = dbo.fnGET_MEDIA_PATH() + 'wm.png'	-- 2012/10/18
				
				

			--	SELECT
			--		WORK_ID				= @WORK_ID,
			--		FILE_NO				= @FILE_NO,
			--		fromFILE_PATH_NAME	= @fromFILE_PATH_NAME,
			--		toFILE_PATH_TYPE	= @toFILE_PATH_TYPE,
			--		toFILE_PATH			= @toFILE_PATH,	
			--		[toFILE_NAME]		= @toFILE_NAME,
			--		PROFILE_NAME		= @PROFILE_NAME,
			--		BEG_TIME			= @BEG_TIME,	
			--		END_TIME			= @END_TIME,	
			--		WIDTH				= @WIDTH,
			--		HEIGHT				= @HEIGHT,
			--		STATUS_0			= @STATUS_0,
			--		--WATERMARK			= '\\172.20.142.152\Media\wm.png',
			--		WATERMARK			= @WM_PATH,	-- 2012/10/18
			--		WATERMARK_ALPHA		= 90
			--		-- 2013/11/19 新增 
			--		,MTS_TOOL            = 'C'
			--		-- 2013/11/19 新增 
			END 
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



