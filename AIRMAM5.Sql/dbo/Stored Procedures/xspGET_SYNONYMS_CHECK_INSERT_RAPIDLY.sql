

-- =============================================
-- 描述:	新增SYNO主檔資料前的檢查
-- 記錄:	<2012/05/28><Dennis.Wen><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_SYNONYMS_CHECK_INSERT_RAPIDLY]
	@fsTEXT1		NVARCHAR(50),
	@fsTEXT2		NVARCHAR(50) 
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		DECLARE
			@fnINDEX_ID_1	BIGINT = 0,
			@fnINDEX_ID_2	BIGINT = 0,
			@fsTEXT_LIST	NVARCHAR(450) = '',
			@RESULT	NVARCHAR(500)	= ''
			
		SET @fnINDEX_ID_1 = ISNULL((SELECT TOP 1 fnINDEX_ID FROM tbmSYNONYMS WHERE (';' + fsTEXT_LIST) LIKE ('%;'+ @fsTEXT1 +';%')),0)
		SET @fnINDEX_ID_2 = ISNULL((SELECT TOP 1 fnINDEX_ID FROM tbmSYNONYMS WHERE (';' + fsTEXT_LIST) LIKE ('%;'+ @fsTEXT2 +';%')),0)

		IF(@fnINDEX_ID_1 = 0 AND @fnINDEX_ID_2 = 0)			/*兩個都為0: 表兩詞彙都不屬於任何一個詞組*/
			BEGIN
				--/*新增一詞組到資料庫中, 並傳回資料列*/
				--INSERT
				--	tbmSYNONYMS
				--	(fsTEXT_LIST, fsNOTE, fdALTERD_DATE, fsALTERD_BY)
				--VALUES
				--	(@fsTEXT1+';'+@fsTEXT2+';', '快速新增', GETDATE(), @fsALTERD_BY)
					
				--SELECT * FROM tbmSYNONYMS WHERE (fnINDEX_ID = @@IDENTITY)
				
				SELECT fsTEXT_LIST = ''
			END
		ELSE IF(@fnINDEX_ID_1 > 0 AND @fnINDEX_ID_2 = 0)	/*其中一個為0: 表其中一詞彙已經屬於某一個詞組*/
			BEGIN		
				/*把@fsTEXT2加到@fsTEXT1所屬的詞組, 並傳回資料列*/
				SET @fsTEXT_LIST = (SELECT fsTEXT_LIST FROM tbmSYNONYMS WHERE (fnINDEX_ID = @fnINDEX_ID_1))
				
				IF(LEN(@fsTEXT_LIST)+LEN(@fsTEXT2)+1 > 450)
					BEGIN
						SET @RESULT = 'ERROR:修改後的詞組字串,長度超過限制.'
						
						SELECT fsTEXT_LIST = @RESULT
					END
				ELSE
					BEGIN
						--UPDATE
						--	tbmSYNONYMS
						--SET
						--	fsTEXT_LIST		= @fsTEXT_LIST + @fsTEXT2 + ';',
						--	fdUPDATED_DATE	= GETDATE(), 
						--	fsUPDATED_BY	= @fsALTERD_BY
						--WHERE
						--	(fnINDEX_ID = @fnINDEX_ID_1)
							
						SELECT * FROM tbmSYNONYMS WHERE (fnINDEX_ID = @fnINDEX_ID_1)
					END
			END
		ELSE IF(@fnINDEX_ID_1 = 0 AND @fnINDEX_ID_2 > 0)	/*其中一個為0: 表其中一詞彙已經屬於某一個詞組*/
			BEGIN
				/*把@fsTEXT1加到@fsTEXT2所屬的詞組, 並傳回資料列*/
				SET @fsTEXT_LIST = (SELECT fsTEXT_LIST FROM tbmSYNONYMS WHERE (fnINDEX_ID = @fnINDEX_ID_2))
				
				IF(LEN(@fsTEXT_LIST)+LEN(@fsTEXT1)+1 > 450)
					BEGIN
						SET @RESULT = 'ERROR:修改後的詞組字串,長度超過限制.'
						
						SELECT fsTEXT_LIST = @RESULT
					END
				ELSE
					BEGIN
						--UPDATE
						--	tbmSYNONYMS
						--SET
						--	fsTEXT_LIST		= @fsTEXT_LIST + @fsTEXT1 + ';',
						--	fdUPDATED_DATE	= GETDATE(), 
						--	fsUPDATED_BY	= @fsALTERD_BY
						--WHERE
						--	(fnINDEX_ID = @fnINDEX_ID_2)
							
						SELECT * FROM tbmSYNONYMS WHERE (fnINDEX_ID = @fnINDEX_ID_2)
					END
			END
		ELSE IF(@fnINDEX_ID_1 > 0 AND @fnINDEX_ID_2 > 0 AND @fnINDEX_ID_1 <> @fnINDEX_ID_2)	/*兩個都不為0,且不相等: 表兩個詞彙都分別屬於不同的詞組*/
			BEGIN
				/*分別屬於不同詞組, 無法新增, 傳回兩筆資料列*/
				
				SELECT * FROM tbmSYNONYMS WHERE (fnINDEX_ID IN (@fnINDEX_ID_1, @fnINDEX_ID_2))		
			END
		ELSE IF(@fnINDEX_ID_1 > 0 AND @fnINDEX_ID_2 > 0 AND @fnINDEX_ID_1 = @fnINDEX_ID_2)	/*兩個都不為0,且相等: 表兩個詞彙都屬於同一個詞組*/
			BEGIN
				/*屬於同詞組, 不用新增, 傳回資料列*/

				SELECT * FROM tbmSYNONYMS WHERE (fnINDEX_ID = @fnINDEX_ID_1)	
			END
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT fsTEXT_LIST = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



