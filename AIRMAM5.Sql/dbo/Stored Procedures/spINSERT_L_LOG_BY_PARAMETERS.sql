
-- =============================================
-- 描述:	新增L_LOG主檔資料By PARAMETERS
-- 記錄:	<2012/01/31><Eric.Huang><新增本預存>
-- 記錄:	<2012/02/04><Eric.Huang><當@USER_NAME 為空時，去tbmUSERS抓USER_NAME>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_L_LOG_BY_PARAMETERS]

	--定義 tbzCODE / tblLOG用的變數
	@fsCODE_ID		VARCHAR(10),	-- tbzCODE 的 key fsCODE_ID
	@fsCODE			VARCHAR(20),	-- tbzCODE 的 key fsCODE (MSG_ID)
	@fsPARAMETERS	NVARCHAR(MAX),	
	@fsNOTE			NVARCHAR(MAX),
	@fsDATA_KEY		VARCHAR (MAX),
	@fsCREATED_BY	VARCHAR(50)
	
AS
BEGIN
	SET NOCOUNT ON;
	
	--定義 要被置換的變數
	DECLARE		@USER_ID		NVARCHAR(20);
	DECLARE		@USER_NAME		NVARCHAR(20);
	DECLARE		@DATA_TYPE		NVARCHAR(20);
	DECLARE		@RESULT			NVARCHAR(20);
	DECLARE		@TARGET			NVARCHAR(20);
	DECLARE		@WORK_PAGE		NVARCHAR(20);
	DECLARE		@CNT_V			INT;
	DECLARE		@CNT_A			INT;
	DECLARE		@CNT_P			INT;
	DECLARE		@CNT_D			INT;
	
	----定義 其它相關變數
	DECLARE		@cnt			INT;
	DECLARE		@i				INT;
	DECLARE		@strSPLIT		VARCHAR(50);
	DECLARE		@strRESULT		NVARCHAR(200);
	DECLARE		@strSET			VARCHAR(20);		
	DECLARE     @strREPLACE     VARCHAR(20);
	DECLARE		@strTYPE		NVARCHAR(10);				
	DECLARE		@strGROUP		NVARCHAR(10);						
	
	SET @strTYPE	= ''
	SET	@strGROUP   = ''
	SET	@strRESULT  = ''
	SET	@strSET		= ''
	SET @strREPLACE = ''	
	SET @i			= 0

	
		--取出LOG DESCRIPTION 的樣版
		SET @strRESULT  = (	SELECT fsNAME FROM tbzCODE	WHERE (fsCODE_ID = @fsCODE_ID) AND (fsCODE = @fsCODE)	 )
		
		--COUNT @PARAMETERS	
		SET @cnt = LEN(REPLACE(@fsPARAMETERS,';',';;')) - LEN(@fsPARAMETERS)
			
		WHILE (@cnt > @i)
		BEGIN
			--分解每個PARAMETER
			SET @strSPLIT = dbo.fnGET_ITEM_BY_INDEX(@fsPARAMETERS,@i)
			
			--將樣版的DESCRIPTION的KEYWORD置換掉(@USER_ID等...)			
			IF @strSPLIT like '@USER_ID%'		
			BEGIN
				--利用CHARINDEX 取出=在第幾個字元,再用RIGHT的方式取出=右邊的值(TOTAL長度 - '='所在的位元),並SET之
				SET  @strRESULT = REPLACE(@strRESULT, '@USER_ID'	,RIGHT(@strSPLIT,(LEN(@strSPLIT) - CHARINDEX('=',@strSPLIT)))) 
			END
			
			IF @strSPLIT like '@USER_NAME%'		
			BEGIN
				--當@USER_NAME 為空時，去tbmUSER抓抓看
				IF RIGHT(@strSPLIT,(LEN(@strSPLIT) - CHARINDEX('=',@strSPLIT))) = '' --當@USER_NAME為空
					BEGIN
						SET  @strREPLACE = dbo.fnGET_USERNAME_FROM_LIST(@fsCREATED_BY + ';') --GET USER_NAME
						SET  @strREPLACE = RTRIM(REPLACE(@strREPLACE,';',' '))				 --將 USER_NAME	的';'移除 & 移除空白						 
						SET  @strRESULT  = REPLACE(@strRESULT, '@USER_NAME',@strREPLACE)     --REPLACE USER_NAME
					END
				ELSE
					BEGIN
						SET  @strRESULT = REPLACE(@strRESULT, '@USER_NAME'	,RIGHT(@strSPLIT,(LEN(@strSPLIT) - CHARINDEX('=',@strSPLIT))))								
					END
			END
			
			IF @strSPLIT like '@DATA_TYPE%'		
			BEGIN
				SET  @strRESULT = REPLACE(@strRESULT, '@DATA_TYPE'	,RIGHT(@strSPLIT,(LEN(@strSPLIT) - CHARINDEX('=',@strSPLIT))))
			END
			
			IF @strSPLIT like '@RESULT%'		
			BEGIN
				SET  @strRESULT = REPLACE(@strRESULT, '@RESULT'		,RIGHT(@strSPLIT,(LEN(@strSPLIT) - CHARINDEX('=',@strSPLIT))))		
			END
			
			IF @strSPLIT like '@TARGET%'		
			BEGIN
				SET  @strRESULT = REPLACE(@strRESULT, '@TARGET'		,RIGHT(@strSPLIT,(LEN(@strSPLIT) - CHARINDEX('=',@strSPLIT))))
			END
			
			IF @strSPLIT like '@WORK_PAGE%'		
			BEGIN
				SET  @strRESULT = REPLACE(@strRESULT, '@WORK_PAGE'	,RIGHT(@strSPLIT,(LEN(@strSPLIT) - CHARINDEX('=',@strSPLIT))))		
			END
			
			IF @strSPLIT like '@CNT_V%'		
			BEGIN
				SET  @strRESULT = REPLACE(@strRESULT, '@CNT_V'		,RIGHT(@strSPLIT,(LEN(@strSPLIT) - CHARINDEX('=',@strSPLIT))))		
			END
			
			IF @strSPLIT like '@CNT_A%'		
			BEGIN
				SET  @strRESULT = REPLACE(@strRESULT, '@CNT_A'		,RIGHT(@strSPLIT,(LEN(@strSPLIT) - CHARINDEX('=',@strSPLIT))))
			END
			
			IF @strSPLIT like '@CNT_P%'		
			BEGIN
				SET  @strRESULT = REPLACE(@strRESULT, '@CNT_P'		,RIGHT(@strSPLIT,(LEN(@strSPLIT) - CHARINDEX('=',@strSPLIT))))								
			END
			
			IF @strSPLIT like '@CNT_D%'		
			BEGIN
				SET  @strRESULT = REPLACE(@strRESULT, '@CNT_D'		,RIGHT(@strSPLIT,(LEN(@strSPLIT) - CHARINDEX('=',@strSPLIT))))	
			END					
			
			SET @i= @i + 1

		END	
	--取出該樣版SET欄位中所放置的TYPE及GROUP供存至L_LOG
	SET @strSET     = (	SELECT fsSET  FROM tbzCODE	WHERE (fsCODE_ID = @fsCODE_ID) AND (fsCODE = @fsCODE)	 )
	--取出TYPE
	SET @strTYPE	= dbo.fnGET_ITEM_BY_INDEX(@strSET,1)
	--取出GROUP	
	SET @strGROUP   = dbo.fnGET_ITEM_BY_INDEX(@strSET,0)	
	
	--新增L_LOG RECORD
	BEGIN TRY
		INSERT
			tblLOG
			(fsTYPE, fsGROUP, fsDESCRIPTION, fsNOTE, fsDATA_KEY, fdCREATED_DATE, fsCREATED_BY)

		VALUES
			(@strTYPE, @strGROUP, @strRESULT, @fsNOTE, @fsDATA_KEY, GETDATE(), @fsCREATED_BY)		
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH		

			
END




