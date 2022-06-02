

-- =============================================
-- 描述:	依照Group取出FUNC_GROUP主檔資料
-- 記錄:	<2011/09/08><Mihsiu.Chiu><新增本預存 source from Dennis>
--			<2012/01/04><Mihsiu.Chiu><修改fsSET的來源為fsSET(原本來自fsUPDATE)>
--			<2012/06/06><Mihsiu.Chiu><for 611demo >
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_FUNC_GROUP_ALL_FUNCTIONS]
	@fsGROUP_ID VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		/*傳入某一個GROUP_ID之後...*/
		DECLARE	@tbOriginal TABLE(	[fsFUNC_ID]		[varchar](50),	[fsNAME]		[nvarchar](50),
									[fsDESCRIPTION]	[nvarchar](50),	[fsTYPE]		[varchar](1),
									[fnORDER]		[int],			[fsPARENT_ID]	[varchar](50))
								
		DECLARE	@tbResult TABLE(	[fsFUNC_ID]		[varchar](50),	[fsNAME]		[nvarchar](50),
									[fsDESCRIPTION]	[nvarchar](50),	[fsTYPE]		[varchar](1),
									[fnORDER]		[int],			[fsPARENT_ID]	[varchar](50),
									_index			int,			_level			int)
								
		DECLARE	@tbTemp TABLE(		[fsFUNC_ID]		[varchar](50),	[fsNAME]		[nvarchar](50),
									[fsDESCRIPTION]	[nvarchar](50),	[fsTYPE]		[varchar](1),
									[fnORDER]		[int],			[fsPARENT_ID]	[varchar](50),
									_index			int,			_level			int)
								
		SET NOCOUNT ON;
		/*先取出所有的群組頭設定至@tbResult中*/
		SELECT [fsFUNC_ID],[fsNAME],[fsDESCRIPTION],[fsTYPE],[fnORDER],[fsPARENT_ID], _index = identity(int,1,1), _level = 0			
		INTO #temp FROM tbmFUNCTIONS WHERE (fsTYPE = 'G') ORDER BY fnORDER	
		
		INSERT @tbResult SELECT * FROM #temp
		
		/*for 611demo mark begin*/
		--DROP TABLE #temp
		/*for 611demo mark end*/
		
		/*先取出所有不是群組頭的設定至@tbOriginal中*/
		INSERT @tbOriginal SELECT [fsFUNC_ID],[fsNAME],[fsDESCRIPTION],[fsTYPE],[fnORDER],[fsPARENT_ID] FROM tbmFUNCTIONS

		WHERE (fsTYPE <> 'G')	     
					
		/*為避免資料設定錯誤造成的無窮迴圈,設定門檻*/
		DECLARE @MAX INT = 10, @CNT INT = 0, @level INT = 1, @line VARCHAR(50) = '└ '
		DECLARE CSR1 CURSOR DYNAMIC FOR SELECT fsFUNC_ID, fsNAME, fsDESCRIPTION, fsTYPE, fnORDER, fsPARENT_ID
										FROM @tbTemp ORDER BY [fsPARENT_ID], [fnORDER] DESC
			
		WHILE (EXISTS(SELECT * FROM @tbOriginal) AND @CNT < 10)
		BEGIN
			DELETE FROM @tbTemp
			INSERT @tbTemp
			SELECT [fsFUNC_ID],[fsNAME],[fsDESCRIPTION],[fsTYPE],[fnORDER],[fsPARENT_ID], 0, @level
			FROM @tbOriginal WHERE fsPARENT_ID IN (SELECT fsFUNC_ID FROM @tbResult WHERE _level = @level - 1)
			ORDER BY [fsPARENT_ID], [fnORDER]  DESC
				 
			DECLARE @fsFUNC_ID		[varchar](50),
					@fsNAME			[nvarchar](50),
					@fsDESCRIPTION	[nvarchar](50),
					@fsTYPE			[varchar](1),
					@fnORDER		[int],
					@fsPARENT_ID	[varchar](50),
					@_index			int

			OPEN CSR1
				FETCH NEXT FROM CSR1
				INTO @fsFUNC_ID, @fsNAME, @fsDESCRIPTION, @fsTYPE, @fnORDER, @fsPARENT_ID
				WHILE (@@FETCH_STATUS=0)
					BEGIN 
					---------開始處理每一筆資料
						SELECT @_index = _index+1 FROM @tbResult WHERE (fsFUNC_ID = @fsPARENT_ID)
						UPDATE @tbResult SET _index = _index + 1 WHERE (_index >= @_index)

						/*塞回結果資料表*/
						INSERT @tbResult SELECT @fsFUNC_ID, @line + @fsNAME, @fsDESCRIPTION, @fsTYPE, @fnORDER, @fsPARENT_ID, @_index, @level
									
						/*已經加入結果資料表的移除*/			
						DELETE FROM @tbOriginal WHERE (fsFUNC_ID = @fsFUNC_ID)
					---------處理完畢每一筆資料
					FETCH NEXT FROM CSR1 INTO @fsFUNC_ID, @fsNAME, @fsDESCRIPTION, @fsTYPE,@fnORDER, @fsPARENT_ID
					END
			Close CSR1

			SET @line = '　' + @line
			SET @level += 1
			SET @CNT += 1
		END
		
		/*for 611demo add begin*/
		DECLARE CSR2 CURSOR FOR SELECT fsFUNC_ID, fsNAME, fsDESCRIPTION, fsTYPE, fnORDER, fsPARENT_ID
										FROM #Temp ORDER BY [fsPARENT_ID], [fnORDER] DESC
		OPEN CSR2
			FETCH NEXT FROM CSR2
			INTO @fsFUNC_ID, @fsNAME, @fsDESCRIPTION, @fsTYPE, @fnORDER, @fsPARENT_ID
			WHILE (@@FETCH_STATUS=0)
				BEGIN 
							
				FETCH NEXT FROM CSR2 INTO @fsFUNC_ID, @fsNAME, @fsDESCRIPTION, @fsTYPE,@fnORDER, @fsPARENT_ID
					
				END
						
		Close CSR2
		DEALLOCATE CSR2;
		drop table #temp
		/*for 611demo add end*/
			
		SELECT * FROM @tbResult ORDER BY _index
		--SELECT fsFUNC_ID, fsNAME, fsDESCRIPTION, fsTYPE, fnORDER, fsPARENT_ID,
		--		[fsCLASSNAME], [fsIS_HIDDEN], [fsCODE_LIST], 
		--		[fsVIEW]  FROM @tbResult ORDER BY _index
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



