

-- =============================================
-- 描述:	取出全部 DIRECTORIES主檔資料&其主題檔
-- 記錄:	<2011/09/29><Dennis.Wen><新增本預存>
-- 記錄:	<2013/10/02><Eric.Huang><由Declare @xxx table 改為 create table #xxx , 速度變快很多!>
-- 記錄:	<2013/10/02><Eric.Huang><#tbResult 加入幾個INDEX>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_DIRECTORIES_AND_SUBJECTS_BY_AUTH]

AS
BEGIN
	SET NOCOUNT ON;

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
		
	--CREATE INDEX IDX_RESULT111 ON #tbTemp(fnPARENT_ID)
	--CREATE INDEX IDX_RESULT222 ON #tbTemp(fnORDER)

	-- 2015/06/17 Eric ++ 把SUBJECT放到TMP TABLE 裡
	--CREATE	TABLE #tbSubject(
	--	[fsSUBJ_ID]			[varchar](12),	[fsTITLE]			[nvarchar](50),	[fsDESCRIPTION]		[nvarchar](200),	[fnDIR_ID]	[bigint],
	--	[fsGROUPS]			[nvarchar](500),[fdCREATED_DATE]	[datetime])

	--CREATE INDEX IDX_RESULT19 ON #tbSubject(fnDIR_ID)

	--INSERT #tbSubject SELECT	[fsSUBJ_ID],	[fsTITLE],	[fsDESCRIPTION],	[fnDIR_ID],		[fsGROUPS],		[fdCREATED_DATE]
	--FROM tbmSUBJECT 
	-- 2015/06/17 Eric ++ 把SUBJECT放到TMP TABLE 裡
		
	/*先取出所有的群組頭設定至#tbResult中*/
	SELECT	[fnDIR_ID],			[fsNAME],			[fnPARENT_ID],		[fsDESCRIPTION],		[fsDIRTYPE],
			[fnORDER] ,			[fnTEMP_ID_SUBJECT],[fnTEMP_ID_VIDEO],	[fnTEMP_ID_AUDIO],	[fnTEMP_ID_PHOTO],
			[fnTEMP_ID_DOC] ,	[fsADMIN_GROUP],	[fsADMIN_USER],		[fsSHOWTYPE],
			[fdCREATED_DATE],	[fsCREATED_BY],		[fdUPDATED_DATE],	[fsUPDATED_BY],
			 _index = 1, _level = 0	
	INTO #temp FROM tbmDIRECTORIES WHERE ([fnDIR_ID] = 1)
	INSERT #tbResult SELECT * FROM #temp
	DROP TABLE #temp
	
	
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
	
	WHILE (EXISTS(SELECT * FROM #tbOriginal) AND @CNT < 10)
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
					--INSERT #tbResult SELECT @fnDIR_ID,			@fsNAME	,			@fnPARENT_ID,		@fsDESCRIPTION,
					--						@fsDIRTYPE,			@fnORDER,			@fnTEMP_ID_SUBJECT,	@fnTEMP_ID_VIDEO,
					--						@fnTEMP_ID_AUDIO,	@fnTEMP_ID_PHOTO,	@fnTEMP_ID_DOC,		@fsADMIN_GROUP,
					--						@fsADMIN_USER,		@fsSHOWTYPE,		@fdCREATED_DATE,	@fsCREATED_BY,
					--						@fdUPDATED_DATE,	@fsUPDATED_BY,		@_index,			@level

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
		
	SELECT

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
		
	--LEFT JOIN #tbSubject AS SUBJ ON (DIR.fnDIR_ID = SUBJ.fnDIR_ID)
	LEFT JOIN tbmSUBJECT AS SUBJ ON (DIR.fnDIR_ID = SUBJ.fnDIR_ID)
	ORDER BY _index , SUBJ.fdCREATED_DATE
END





