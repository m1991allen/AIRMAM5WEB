


-- =============================================
-- 描述:	取出全部 DIRECTORIES主檔資料 BY 管理權限/使用權限		※此預存修改前請先與富元討論
-- 記錄:	<2011/10/17><Dennis.Wen><新增此版程式Logic>
--          <2011/10/17><Eric.Huang><新增此預存>
--			<2011/10/24><Dennis.Wen><比對包含的語法修正>
--			<2011/10/25><Dennis.Wen><比對包含的語法修正>
--			<2016/09/15><David.Sin><重新寫過，取代spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_PLUS_PARENT_LIMIT用>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_DIRECTORIES_ALL_BY_AUTHORITY]

	@fsGROUP_ID  VARCHAR(200),
	@fsLOGIN_ID  VARCHAR(50)
	
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY

	DECLARE	@tbOriginal TABLE(
		[fnDIR_ID]			[bigint],		[fsNAME]			[nvarchar](50),	[fnPARENT_ID]		[bigint],	[fsDESCRIPTION]		[nvarchar](50),
		[fsDIRTYPE]			[varchar](1),	[fnORDER]			[int],			[fnTEMP_ID_SUBJECT] [int],		[fnTEMP_ID_VIDEO]	[int],
		[fnTEMP_ID_AUDIO]	[int],			[fnTEMP_ID_PHOTO]	[int],			[fnTEMP_ID_DOC]		[int],		[fsADMIN_GROUP]		[varchar](500),
		[fsADMIN_USER]		[varchar](1000),[fsSHOWTYPE]		[varchar](1),	[fdCREATED_DATE]	[datetime],	[fsCREATED_BY]		[nvarchar](50),
		[fdUPDATED_DATE]	[datetime],		[fsUPDATED_BY]		[nvarchar](50))
		
	DECLARE	@tbResult TABLE(
		[fnDIR_ID]			[bigint],		[fsNAME]			[nvarchar](50),	[fnPARENT_ID]		[bigint],	[fsDESCRIPTION]		[nvarchar](50),
		[fsDIRTYPE]			[varchar](1),	[fnORDER]			[int],			[fnTEMP_ID_SUBJECT] [int],		[fnTEMP_ID_VIDEO]	[int],
		[fnTEMP_ID_AUDIO]	[int],			[fnTEMP_ID_PHOTO]	[int],			[fnTEMP_ID_DOC]		[int],		[fsADMIN_GROUP]		[varchar](500),
		[fsADMIN_USER]		[varchar](1000),[fsSHOWTYPE]		[varchar](1),	[fdCREATED_DATE]	[datetime],	[fsCREATED_BY]		[nvarchar](50),
		[fdUPDATED_DATE]	[datetime],		[fsUPDATED_BY]		[nvarchar](50),
		_index int, _level int, _ADMIN VARCHAR(1), _USER VARCHAR(1))
							
	DECLARE	@tbTemp TABLE(
		[fnDIR_ID]			[bigint],		[fsNAME]			[nvarchar](50),	[fnPARENT_ID]		[bigint],	[fsDESCRIPTION]		[nvarchar](50),
		[fsDIRTYPE]			[varchar](1),	[fnORDER]			[int],			[fnTEMP_ID_SUBJECT] [int],		[fnTEMP_ID_VIDEO]	[int],
		[fnTEMP_ID_AUDIO]	[int],			[fnTEMP_ID_PHOTO]	[int],			[fnTEMP_ID_DOC]		[int],		[fsADMIN_GROUP]		[varchar](500),
		[fsADMIN_USER]		[varchar](1000),[fsSHOWTYPE]		[varchar](1),	[fdCREATED_DATE]	[datetime],	[fsCREATED_BY]		[nvarchar](50),
		[fdUPDATED_DATE]	[datetime],		[fsUPDATED_BY]		[nvarchar](50),
		_index int, _level int)
		
	/*先取出所有的群組頭設定至@tbResult中*/
	SELECT	[fnDIR_ID],			[fsNAME],			[fnPARENT_ID],		[fsDESCRIPTION],		[fsDIRTYPE],
			[fnORDER] ,			[fnTEMP_ID_SUBJECT],[fnTEMP_ID_VIDEO],	[fnTEMP_ID_AUDIO],	[fnTEMP_ID_PHOTO],
			[fnTEMP_ID_DOC] ,	[fsADMIN_GROUP],	[fsADMIN_USER],		[fsSHOWTYPE],
			[fdCREATED_DATE],	[fsCREATED_BY],		[fdUPDATED_DATE],	[fsUPDATED_BY],
			 _index = 1, _level = 0, _ADMIN = '', _USER = ''
	INTO #temp FROM tbmDIRECTORIES WHERE ([fnDIR_ID] = 1)
	INSERT @tbResult SELECT * FROM #temp
	DROP TABLE #temp
	
	
	/*先取出所有不是群組頭的設定至@tbOriginal中*/
	INSERT @tbOriginal SELECT	[fnDIR_ID],			[fsNAME],			[fnPARENT_ID],		[fsDESCRIPTION],		[fsDIRTYPE],
								[fnORDER] ,			[fnTEMP_ID_SUBJECT],[fnTEMP_ID_VIDEO],	[fnTEMP_ID_AUDIO],	[fnTEMP_ID_PHOTO],
								[fnTEMP_ID_DOC] ,	[fsADMIN_GROUP],	[fsADMIN_USER],		[fsSHOWTYPE],
								[fdCREATED_DATE],	[fsCREATED_BY],		[fdUPDATED_DATE],	[fsUPDATED_BY] 
						FROM tbmDIRECTORIES
	WHERE ([fnDIR_ID] <> 1)	
	
/*<<CheckPount1>>*/
		
	/*為避免資料設定錯誤造成的無窮迴圈,設定門檻*/
	DECLARE @MAX INT = 30, @CNT INT = 0, @level INT = 1, @line VARCHAR(50) = '└ '
	DECLARE CSR1 CURSOR DYNAMIC FOR SELECT [fnDIR_ID],			[fsNAME],			[fnPARENT_ID],		[fsDESCRIPTION],		[fsDIRTYPE],
											[fnORDER] ,			[fnTEMP_ID_SUBJECT],[fnTEMP_ID_VIDEO],	[fnTEMP_ID_AUDIO],	[fnTEMP_ID_PHOTO],
											[fnTEMP_ID_DOC] ,	[fsADMIN_GROUP],	[fsADMIN_USER],		[fsSHOWTYPE],
											[fdCREATED_DATE],	[fsCREATED_BY],		[fdUPDATED_DATE],	[fsUPDATED_BY]
									FROM @tbTemp ORDER BY [fnPARENT_ID], [fnORDER] DESC, [fsNAME] DESC
									
	--
	
	WHILE (EXISTS(SELECT * FROM @tbOriginal) AND @CNT < 30)
	BEGIN
		DELETE FROM @tbTemp
		INSERT @tbTemp
		SELECT [fnDIR_ID],			[fsNAME],[fnPARENT_ID],		[fsDESCRIPTION],		[fsDIRTYPE],
				[fnORDER] ,			[fnTEMP_ID_SUBJECT],		[fnTEMP_ID_VIDEO],	[fnTEMP_ID_AUDIO],	[fnTEMP_ID_PHOTO],
				[fnTEMP_ID_DOC] ,	[fsADMIN_GROUP],			[fsADMIN_USER],		[fsSHOWTYPE],
				[fdCREATED_DATE],	[fsCREATED_BY],				[fdUPDATED_DATE],	[fsUPDATED_BY],
				0, @level
				
		FROM @tbOriginal WHERE fnPARENT_ID IN (SELECT [fnDIR_ID] FROM @tbResult WHERE _level = @level - 1)
		ORDER BY [fnPARENT_ID], [fnORDER]  DESC, [fsNAME] 
			 
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
					SELECT @_index = _index+1 FROM @tbResult WHERE (fnDIR_ID = @fnPARENT_ID)
					UPDATE @tbResult SET _index = _index + 1 WHERE (_index >= @_index)
					 		
--SELECT * FROM  @tbResult

					/*塞回結果資料表*/
					INSERT @tbResult SELECT @fnDIR_ID,			@fsNAME	,			@fnPARENT_ID,		@fsDESCRIPTION,
											@fsDIRTYPE,			@fnORDER,			@fnTEMP_ID_SUBJECT,	@fnTEMP_ID_VIDEO,
											@fnTEMP_ID_AUDIO,	@fnTEMP_ID_PHOTO,	@fnTEMP_ID_DOC,		@fsADMIN_GROUP,
											@fsADMIN_USER,		@fsSHOWTYPE,		@fdCREATED_DATE,	@fsCREATED_BY,
											@fdUPDATED_DATE,	@fsUPDATED_BY,		@_index,			@level, '', ''
											
								
					/*已經加入結果資料表的移除*/			
					DELETE FROM @tbOriginal WHERE (fnDIR_ID = @fnDIR_ID)
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
	
	/*到目前為止@@tbResult應該有所選節點以上與以下的所有目錄*/
	
/*<<CheckPount2>>*/
	
	-----
	DECLARE @UID BIGINT = (SELECT TOP 1 fnUSER_ID FROM tbmUSERS WHERE fsLOGIN_ID = @fsLOGIN_ID)
	-----
	SET NOCOUNT ON;

	DECLARE CSR2 CURSOR FOR SELECT fnDIR_ID, fsADMIN_GROUP, fsADMIN_USER, fsSHOWTYPE FROM @tbResult  ORDER BY _index
	DECLARE @c2fnDIR_ID BIGINT, @c2fsADMIN_GROUP VARCHAR(500), @c2fsADMIN_USER VARCHAR(1000), @c2fsSHOWTYPE VARCHAR(1)

	OPEN CSR2
		FETCH NEXT FROM CSR2 INTO @c2fnDIR_ID, @c2fsADMIN_GROUP, @c2fsADMIN_USER, @c2fsSHOWTYPE
		WHILE (@@FETCH_STATUS=0)
			BEGIN 
			---------開始處理每一筆資料
				/*_ADMIN是大寫Y的表示其於Directories設定為管理群組或管理帳號*/
				--IF((@c2fsADMIN_USER LIKE '%'+@fsLOGIN_ID+';%') OR (dbo.fnGET_STRING_MATCH(@fsGROUP_ID,@c2fsADMIN_GROUP) = 'Y'))
				IF(((';'+@c2fsADMIN_USER) LIKE '%;'+@fsLOGIN_ID+';%' AND @fsLOGIN_ID <> '') OR (dbo.fnGET_STRING_MATCH(@fsGROUP_ID,@c2fsADMIN_GROUP) = 'Y'))
				BEGIN
					UPDATE @tbResult
					SET _ADMIN = 'Y'
					WHERE (fnDIR_ID = @c2fnDIR_ID)
				END
				
				/*檢查每一筆在tbmDIR_GROUP與tbmDIR_USER是否有設定,有存在設定表示有使用權限*/
				--IF(	(EXISTS(SELECT * FROM tbmDIR_GROUP WHERE @fsGROUP_ID LIKE '%'+fnGROUP_ID+';%' AND fnDIR_ID = @c2fnDIR_ID) OR
				IF(	(EXISTS(SELECT * FROM tbmDIR_GROUP WHERE (';'+@fsGROUP_ID) LIKE '%;'+fnGROUP_ID+';%' AND fnDIR_ID = @c2fnDIR_ID) OR
					EXISTS(SELECT * FROM tbmDIR_USER WHERE fnUSER_ID = @UID AND fnDIR_ID = @c2fnDIR_ID)) AND @c2fsSHOWTYPE <> 'H')
				BEGIN
					UPDATE @tbResult
					SET _USER = 'Y'
					WHERE (fnDIR_ID = @c2fnDIR_ID)
				END
			FETCH NEXT FROM CSR2 INTO @c2fnDIR_ID, @c2fsADMIN_GROUP, @c2fsADMIN_USER, @c2fsSHOWTYPE
			END
	Close CSR2	

	DECLARE @i INT = 0, @j INT = (SELECT COUNT(*) FROM @tbResult)
	--SELECT fnDIR_ID FROM @tbResult WHERE _ADMIN = 'Y'
	WHILE(@i < @j)
	BEGIN
		/*_ADMIN 往下, 父階層是'Y'或'y'就'y'*/
		UPDATE
			@tbResult 		
		SET
			_ADMIN = 'y'
		WHERE
			((fnPARENT_ID IN (SELECT fnDIR_ID FROM @tbResult WHERE _ADMIN = 'Y')) AND _ADMIN = '') 
		
		/*_USER 往下, 父階層是'Y'或'y'就'y'*/
		UPDATE
			@tbResult	
		SET
			_USER = 'y'
		WHERE
			((fnPARENT_ID IN (SELECT fnDIR_ID FROM @tbResult WHERE _USER = 'Y')) AND _USER = '')	
				
		SET @i = @i + 1;		
	END
	
	SET @i = 1;
	WHILE(@i < @j)
	BEGIN
		/*_USER 往上, 子階層需掛而本身未設定使用, 則是'@'*/
		/*_USER 往上, 子階層需掛而本身有管理權限, 則是'y'*/
		UPDATE
			@tbResult	
		SET
			_USER = CASE WHEN _ADMIN = '' AND _USER = '' THEN '@' ELSE '' END
		WHERE
			((fnDIR_ID IN (SELECT fnPARENT_ID FROM @tbResult WHERE _USER <> ''  OR _ADMIN<>'')) 
			AND _USER = '' AND fsSHOWTYPE <> 'H')
			
		SET @i = @i + 1;
	END

-----

	--SET @i = 1;
	
	--WHILE(@i < @j)
	--BEGIN
	--	UPDATE
	--		@tbResult
	--	SET
	--		_USER = 'y'
	--	WHERE
	--		((fnDIR_ID IN (SELECT fnPARENT_ID FROM @tbResult WHERE _USER <> '' OR _ADMIN <> '')) AND _USER = '' AND _ADMIN = '')
	--		/*OR
	--		((fnPARENT_ID IN (SELECT fnDIR_ID FROM @tbResult WHERE _USER <> '' AND _USER <> '@')) AND _USER = '')*/
			
	--	UPDATE
	--		@tbResult
	--	SET
	--		_USER = 'y'
	--	WHERE
	--		/*((fnDIR_ID IN (SELECT fnPARENT_ID FROM @tbResult WHERE _USER <> '' OR _ADMIN <> '')) AND _USER = '' AND _ADMIN = '')
	--		OR*/
	--		((fnPARENT_ID IN (SELECT fnDIR_ID FROM @tbResult WHERE _USER <> '' AND _USER <> '@')) AND _USER = '')		
			
	--	SET @i = @i + 1;
	--END

-----
		
	SELECT _ADMIN, _USER, idx = _index,
		* , 
		_SUBJECT_NAME = ISNULL(TEMP_S.fsNAME, ''), 
		_VIDEO_NAME   = ISNULL(TEMP_V.fsNAME, ''), 
		_AUDIO_NAME   = ISNULL(TEMP_A.fsNAME, ''), 
		_PHOTO_NAME   = ISNULL(TEMP_P.fsNAME, ''), 
		_DOC_NAME     = ISNULL(TEMP_D.fsNAME, ''),
		_sGROUP_NAME_LIST = dbo.fnGET_GROUPNAME_FROM_LIST(fsADMIN_GROUP),
		_sUSER_NAME_LIST  = dbo.fnGET_USERNAME_FROM_LIST(fsADMIN_USER),
			_sDIR_PATH = dbo.fnGET_DIR_PATH_BY_DIR_ID(fnDIR_ID)

	FROM @tbResult 
		LEFT JOIN tbmTEMPLATE AS TEMP_S ON (TEMP_S.fnTEMP_ID = fnTEMP_ID_SUBJECT)		
		LEFT JOIN tbmTEMPLATE AS TEMP_V ON (TEMP_V.fnTEMP_ID = fnTEMP_ID_VIDEO)
		LEFT JOIN tbmTEMPLATE AS TEMP_A ON (TEMP_A.fnTEMP_ID = fnTEMP_ID_AUDIO)	
	
		LEFT JOIN tbmTEMPLATE AS TEMP_P ON (TEMP_P.fnTEMP_ID = fnTEMP_ID_PHOTO)
		LEFT JOIN tbmTEMPLATE AS TEMP_D ON (TEMP_D.fnTEMP_ID = fnTEMP_ID_DOC)
	
	WHERE _ADMIN <> '' or _USER <> ''
		
	ORDER BY _index
	
END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



