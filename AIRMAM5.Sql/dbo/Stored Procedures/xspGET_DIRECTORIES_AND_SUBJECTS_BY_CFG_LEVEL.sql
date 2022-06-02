


-- =============================================
-- 描述:	取出全部 DIRECTORIES主檔資料&其主題檔
-- 記錄:	<2013/10/09><Eric.Huang><新增本預存> copy from spGET_DIRECTORIES_AND_SUBJECTS_ALL
-- 記錄:	<2014/06/05><Eric.Huang><修改本預存> 當FTV時,SUBJECT 排序 ORDER BY FSATTRIBUTE1 DESC
-- 記錄:	<2015/06/18><Eric.Huang><修改本預存> 試著加速,測試中,上線中，但TEMP FOLDER 要改掉
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_DIRECTORIES_AND_SUBJECTS_BY_CFG_LEVEL]

AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DirLevel VARCHAR(10) = (SELECT fsVALUE FROM tbzCONFIG WHERE fsKEY = 'DIR_DEFAULT_LEVEL')

	CREATE	TABLE #tbOriginal(
		[fnDIR_ID]			[bigint],		[fsNAME]			[nvarchar](50),	[fnPARENT_ID]		[bigint],	[fsDESCRIPTION]		[nvarchar](50),
		[fsDIRTYPE]			[varchar](1),	[fnORDER]			[int],			[fnTEMP_ID_SUBJECT] [int],		[fnTEMP_ID_VIDEO]	[int],
		[fnTEMP_ID_AUDIO]	[int],			[fnTEMP_ID_PHOTO]	[int],			[fnTEMP_ID_DOC]		[int],		[fsADMIN_GROUP]		[varchar](500),
		[fsADMIN_USER]		[varchar](1000),[fsSHOWTYPE]		[varchar](1),	[fdCREATED_DATE]	[datetime],	[fsCREATED_BY]		[nvarchar](50),
		[fdUPDATED_DATE]	[datetime],		[fsUPDATED_BY]		[nvarchar](50))
		
	CREATE INDEX IDX_RESULT11 ON #tbOriginal(fnDIR_ID)
	CREATE INDEX IDX_RESULT22 ON #tbOriginal(fnPARENT_ID)

	CREATE	TABLE #tbResult(
		[fnDIR_ID]			[bigint],		[fsNAME]			[nvarchar](50),	[fnPARENT_ID]		[bigint],	[fsDESCRIPTION]		[nvarchar](50),
		[fsDIRTYPE]			[varchar](1),	[fnORDER]			[int],			[fnTEMP_ID_SUBJECT] [int],		[fnTEMP_ID_VIDEO]	[int],
		[fnTEMP_ID_AUDIO]	[int],			[fnTEMP_ID_PHOTO]	[int],			[fnTEMP_ID_DOC]		[int],		[fsADMIN_GROUP]		[varchar](500),
		[fsADMIN_USER]		[varchar](1000),[fsSHOWTYPE]		[varchar](1),	[fdCREATED_DATE]	[datetime],	[fsCREATED_BY]		[nvarchar](50),
		[fdUPDATED_DATE]	[datetime],		[fsUPDATED_BY]		[nvarchar](50),
		_index int, _level int)
							
	CREATE INDEX IDX_RESULT1 ON #tbResult(fnDIR_ID)
	CREATE INDEX IDX_RESULT2 ON #tbResult(fnPARENT_ID)
	--CREATE INDEX IDX_RESULT3 ON #tbResult(fsADMIN_USER)
	--CREATE INDEX IDX_RESULT4 ON #tbResult(fsADMIN_GROUP)
	
	CREATE	TABLE #tbTemp(
		[fnDIR_ID]			[bigint],		[fsNAME]			[nvarchar](50),	[fnPARENT_ID]		[bigint],	[fsDESCRIPTION]		[nvarchar](50),
		[fsDIRTYPE]			[varchar](1),	[fnORDER]			[int],			[fnTEMP_ID_SUBJECT] [int],		[fnTEMP_ID_VIDEO]	[int],
		[fnTEMP_ID_AUDIO]	[int],			[fnTEMP_ID_PHOTO]	[int],			[fnTEMP_ID_DOC]		[int],		[fsADMIN_GROUP]		[varchar](500),
		[fsADMIN_USER]		[varchar](1000),[fsSHOWTYPE]		[varchar](1),	[fdCREATED_DATE]	[datetime],	[fsCREATED_BY]		[nvarchar](50),
		[fdUPDATED_DATE]	[datetime],		[fsUPDATED_BY]		[nvarchar](50),
		_index int, _level int)
		
	CREATE INDEX IDX_RESULT111 ON #tbTemp(fnPARENT_ID)
	CREATE INDEX IDX_RESULT222 ON #tbTemp(fnORDER)
		
	/*先取出所有的群組頭設定至#tbResult中*/
	INSERT #tbResult
	SELECT	[fnDIR_ID],			[fsNAME],			[fnPARENT_ID],		[fsDESCRIPTION],		[fsDIRTYPE],
			[fnORDER] ,			[fnTEMP_ID_SUBJECT],[fnTEMP_ID_VIDEO],	[fnTEMP_ID_AUDIO],	[fnTEMP_ID_PHOTO],
			[fnTEMP_ID_DOC] ,	[fsADMIN_GROUP],	[fsADMIN_USER],		[fsSHOWTYPE],
			[fdCREATED_DATE],	[fsCREATED_BY],		[fdUPDATED_DATE],	[fsUPDATED_BY],
			 _index = 1, _level = 0	
	FROM tbmDIRECTORIES WHERE ([fnDIR_ID] = 1)
	--INSERT #tbResult SELECT * FROM #temp
	--DROP TABLE #temp
	
	
	/*先取出所有不是群組頭的設定至#tbOriginal中*/
	INSERT #tbOriginal SELECT	[fnDIR_ID],			[fsNAME],			[fnPARENT_ID],		[fsDESCRIPTION],		[fsDIRTYPE],
								[fnORDER] ,			[fnTEMP_ID_SUBJECT],[fnTEMP_ID_VIDEO],	[fnTEMP_ID_AUDIO],	[fnTEMP_ID_PHOTO],
								[fnTEMP_ID_DOC] ,	[fsADMIN_GROUP],	[fsADMIN_USER],		[fsSHOWTYPE],
								[fdCREATED_DATE],	[fsCREATED_BY],		[fdUPDATED_DATE],	[fsUPDATED_BY] 
						FROM tbmDIRECTORIES
	WHERE ([fnDIR_ID] <> 1)	
	ORDER BY fnDIR_ID
			
	/*為避免資料設定錯誤造成的無窮迴圈,設定門檻*/
	DECLARE @MAX INT = 10, @CNT INT = 0, @level INT = 1, @line VARCHAR(50) = '└ '
	DECLARE CSR1 CURSOR DYNAMIC FOR SELECT [fnDIR_ID],			[fsNAME],			[fnPARENT_ID],		[fsDESCRIPTION],		[fsDIRTYPE],
											[fnORDER] ,			[fnTEMP_ID_SUBJECT],[fnTEMP_ID_VIDEO],	[fnTEMP_ID_AUDIO],	[fnTEMP_ID_PHOTO],
											[fnTEMP_ID_DOC] ,	[fsADMIN_GROUP],	[fsADMIN_USER],		[fsSHOWTYPE],
											[fdCREATED_DATE],	[fsCREATED_BY],		[fdUPDATED_DATE],	[fsUPDATED_BY]
									FROM #tbTemp ORDER BY [fnPARENT_ID], [fnORDER] DESC
									
	--
	
	WHILE (EXISTS(SELECT * FROM #tbOriginal) AND @CNT < @DirLevel)
	BEGIN
		DELETE FROM #tbTemp
		INSERT #tbTemp
		SELECT [fnDIR_ID],			[fsNAME],[fnPARENT_ID],		[fsDESCRIPTION],		[fsDIRTYPE],
				[fnORDER] ,			[fnTEMP_ID_SUBJECT],		[fnTEMP_ID_VIDEO],	[fnTEMP_ID_AUDIO],	[fnTEMP_ID_PHOTO],
				[fnTEMP_ID_DOC] ,	[fsADMIN_GROUP],			[fsADMIN_USER],		[fsSHOWTYPE],
				[fdCREATED_DATE],	[fsCREATED_BY],				[fdUPDATED_DATE],	[fsUPDATED_BY],
				0, @level
				
		FROM #tbOriginal WHERE fnPARENT_ID IN (SELECT [fnDIR_ID] FROM #tbResult WHERE _level = @level - 1)
		ORDER BY [fnPARENT_ID], [fnORDER]  DESC
			 
		DECLARE
			@fnDIR_ID			bigint,			@fsNAME				nvarchar(50),	@fnPARENT_ID		bigint,		@fsDESCRIPTION		nvarchar(50),
			@fsDIRTYPE			varchar(1),		@fnORDER			int,			@fnTEMP_ID_SUBJECT	int,		@fnTEMP_ID_VIDEO	int,
			@fnTEMP_ID_AUDIO	int,			@fnTEMP_ID_PHOTO	int,			@fnTEMP_ID_DOC		int,		@fsADMIN_GROUP		varchar(500),
			@fsADMIN_USER		varchar(1000),	@fsSHOWTYPE			varchar(1),		@fdCREATED_DATE		datetime,	@fsCREATED_BY		nvarchar(50),
			@fdUPDATED_DATE		datetime,		@fsUPDATED_BY		nvarchar(50),	@_index			int	

		OPEN CSR1
			FETCH NEXT FROM CSR1
			INTO
				@fnDIR_ID,			@fsNAME	,			@fnPARENT_ID,		@fsDESCRIPTION,
				@fsDIRTYPE,			@fnORDER,			@fnTEMP_ID_SUBJECT,	@fnTEMP_ID_VIDEO,
				@fnTEMP_ID_AUDIO,	@fnTEMP_ID_PHOTO,	@fnTEMP_ID_DOC,		@fsADMIN_GROUP,
				@fsADMIN_USER,		@fsSHOWTYPE,		@fdCREATED_DATE,	@fsCREATED_BY,
				@fdUPDATED_DATE,	@fsUPDATED_BY	
			WHILE (@@FETCH_STATUS=0)
				BEGIN 
				---------開始處理每一筆資料
					SELECT @_index = _index+1 FROM #tbResult WHERE (fnDIR_ID = @fnPARENT_ID)
					UPDATE #tbResult SET _index = _index + 1 WHERE (_index >= @_index)
					 		
--SELECT * FROM  #tbResult

					/*塞回結果資料表*/
					INSERT #tbResult SELECT @fnDIR_ID,			@fsNAME	,			@fnPARENT_ID,		@fsDESCRIPTION,
											@fsDIRTYPE,			@fnORDER,			@fnTEMP_ID_SUBJECT,	@fnTEMP_ID_VIDEO,
											@fnTEMP_ID_AUDIO,	@fnTEMP_ID_PHOTO,	@fnTEMP_ID_DOC,		@fsADMIN_GROUP,
											@fsADMIN_USER,		@fsSHOWTYPE,		@fdCREATED_DATE,	@fsCREATED_BY,
											@fdUPDATED_DATE,	@fsUPDATED_BY,		@_index,			@level
								
					/*已經加入結果資料表的移除*/			
					DELETE FROM #tbOriginal WHERE (fnDIR_ID = @fnDIR_ID)
				---------處理完畢每一筆資料
				FETCH NEXT FROM CSR1 INTO
					@fnDIR_ID,			@fsNAME	,			@fnPARENT_ID,		@fsDESCRIPTION,
					@fsDIRTYPE,			@fnORDER,			@fnTEMP_ID_SUBJECT,	@fnTEMP_ID_VIDEO,
					@fnTEMP_ID_AUDIO,	@fnTEMP_ID_PHOTO,	@fnTEMP_ID_DOC,		@fsADMIN_GROUP,
					@fsADMIN_USER,		@fsSHOWTYPE,		@fdCREATED_DATE,	@fsCREATED_BY,
					@fdUPDATED_DATE,	@fsUPDATED_BY	
				END
		Close CSR1

		SET @line = '　' + @line
		SET @level += 1
		SET @CNT += 1
	END
		
	--SELECT * FROM #tbResult ORDER BY _index
	DECLARE @USER AS NVARCHAR(10)

	SET @USER = ISNULL((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'CUSTOMER_ID')),'')

		IF (@USER = 'FTV')
			BEGIN

				SELECT
					--TOP 1
					DIR.fnDIR_ID, DIR.fsDIRTYPE, NODATA = ISNULL(SUBJ.fsSUBJ_ID,'Y'), 
					_nVideo		= (SELECT COUNT(fsFILE_NO) FROM tbmARC_VIDEO AS V WHERE (V.fsSUBJECT_ID = SUBJ.fsSUBJ_ID)),
					_nAudio		= (SELECT COUNT(fsFILE_NO) FROM tbmARC_AUDIO AS A WHERE (A.fsSUBJECT_ID = SUBJ.fsSUBJ_ID)),
					_nPhoto		= (SELECT COUNT(fsFILE_NO) FROM tbmARC_PHOTO AS P WHERE (P.fsSUBJECT_ID = SUBJ.fsSUBJ_ID)),
					_nDocument	= (SELECT COUNT(fsFILE_NO) FROM tbmARC_DOC AS D WHERE (D.fsSUBJECT_ID = SUBJ.fsSUBJ_ID)),		
					SUBJ.fsSUBJ_ID, SUBJ.fsTITLE,  SUBJ.fsDESCRIPTION, SUBJ.fsGroups,
						_sDIR_PATH = dbo.fnGET_DIR_PATH_BY_DIR_ID(DIR.fnDIR_ID),

						-- 記錄:	<2015/06/18><Eric.Huang><修改本預存> 試著加速,測試中,未上線
						--_sDIR_PATH = (SELECT _sDIR_PATH FROM t_20150618 WHERE fnDIR_ID = DIR.fnDIR_ID),
						_sSUBJ_PATH = CASE WHEN (ISNULL(SUBJ.fsSUBJ_ID,'')='') THEN ''
										ELSE dbo.fnGET_SUBJ_PATH_BY_SUBJECT_ID(SUBJ.fsSUBJ_ID) END,

						-- 記錄:	<2015/06/18><Eric.Huang><修改本預存> 試著加速,測試中,未上線
						--_sSUBJ_PATH = CASE WHEN (ISNULL(SUBJ.fsSUBJ_ID,'')='') THEN ''
						--				ELSE (SELECT _sSUBJ_PATH FROM t_20150618_2 WHERE fsSUBJ_ID = SUBJ.fsSUBJ_ID) END,
						

						SUBJ.fdCREATED_DATE,	-- 2012/04/16 ERIC ++
						SUBJ.fsATTRIBUTE1	    -- 2014/06/05 ERIC ++
					--,SUBJ.*
				FROM
					#tbResult AS DIR
		
				LEFT JOIN tbmSUBJECT AS SUBJ ON (DIR.fnDIR_ID = SUBJ.fnDIR_ID)
				ORDER BY _index , (Select RIGHT(REPLICATE('0', 8) + CAST(SUBJ.fsATTRIBUTE1 as NVARCHAR), 8)) desc

			END
		ELSE
			BEGIN
				SELECT
					--TOP 1
					DIR.fnDIR_ID, DIR.fsDIRTYPE, NODATA = ISNULL(SUBJ.fsSUBJ_ID,'Y'), 
					_nVideo		= (SELECT COUNT(fsFILE_NO) FROM tbmARC_VIDEO AS V WHERE (V.fsSUBJECT_ID = SUBJ.fsSUBJ_ID)),
					_nAudio		= (SELECT COUNT(fsFILE_NO) FROM tbmARC_AUDIO AS A WHERE (A.fsSUBJECT_ID = SUBJ.fsSUBJ_ID)),
					_nPhoto		= (SELECT COUNT(fsFILE_NO) FROM tbmARC_PHOTO AS P WHERE (P.fsSUBJECT_ID = SUBJ.fsSUBJ_ID)),
					_nDocument	= (SELECT COUNT(fsFILE_NO) FROM tbmARC_DOC AS D WHERE (D.fsSUBJECT_ID = SUBJ.fsSUBJ_ID)),		
					SUBJ.fsSUBJ_ID, SUBJ.fsTITLE,  SUBJ.fsDESCRIPTION, SUBJ.fsGroups,
						_sDIR_PATH = dbo.fnGET_DIR_PATH_BY_DIR_ID(DIR.fnDIR_ID),
						_sSUBJ_PATH = CASE WHEN (ISNULL(SUBJ.fsSUBJ_ID,'')='') THEN ''
										ELSE dbo.fnGET_SUBJ_PATH_BY_SUBJECT_ID(SUBJ.fsSUBJ_ID) END,
						SUBJ.fdCREATED_DATE	-- 2012/04/16 ERIC ++
					--,SUBJ.*
				FROM
					#tbResult AS DIR
		
				LEFT JOIN tbmSUBJECT AS SUBJ ON (DIR.fnDIR_ID = SUBJ.fnDIR_ID)
				ORDER BY _index , SUBJ.fsSUBJ_ID desc
			END

		DROP TABLE #tbOriginal			
		DROP TABLE #tbTemp
		DROP TABLE #tbResult	
END






