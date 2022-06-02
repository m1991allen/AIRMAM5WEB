


-- =============================================
-- 描述:	取出 暫時刪除的-文件 自訂欄位資料
-- 記錄:	<2019/09/17><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[sp_t_GET_ARC_DOC_ATTRIBUTE]
	@fsFILE_NO	VARCHAR(16)
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @tbResult TABLE(fsFIELD			VARCHAR(50),
							fsFIELD_NAME	NVARCHAR(50),
							fsFIELD_TYPE	VARCHAR(50),
							fsCODE_ID		VARCHAR(20),
							fsCODE_CTRL		VARCHAR(20),
							fsVALUE			NVARCHAR(MAX),
							fnORDER			INT)

	/*取出此媒體檔所屬的主題檔*/
	DECLARE @fsSUBJECT_ID VARCHAR(12) = (	SELECT [dbo].[t_tbmARC_DOC].[fsSUBJECT_ID] 
											FROM [dbo].[t_tbmARC_DOC]
											WHERE ([dbo].[t_tbmARC_DOC].[fsFILE_NO] = @fsFILE_NO)	)

	/*取出此主題檔所屬的目錄中, 此媒體檔套用的樣板編號*/
	DECLARE @TEMP_ID INT = (SELECT [dbo].[tbmDIRECTORIES].[fnTEMP_ID_DOC]
							FROM [dbo].[tbmDIRECTORIES]
							WHERE ([dbo].[tbmDIRECTORIES].[fnDIR_ID] = (	SELECT [dbo].[tbmSUBJECT].[fnDIR_ID] 
												FROM [dbo].[tbmSUBJECT]
												WHERE ([dbo].[tbmSUBJECT].[fsSUBJ_ID] = @fsSUBJECT_ID) ))	)
					
	/*依據樣板編號取出該樣版有設定的自訂欄位(ORDER BY fnORDER, fsFIELD_NAME)*/
	INSERT
		@tbResult	
	SELECT
		[dbo].[tbmTEMPLATE_FIELDS].[fsFIELD], [dbo].[tbmTEMPLATE_FIELDS].[fsFIELD_NAME], [dbo].[tbmTEMPLATE_FIELDS].[fsFIELD_TYPE], [dbo].[tbmTEMPLATE_FIELDS].[fsCODE_ID], [dbo].[tbmTEMPLATE_FIELDS].[fsCODE_CTRL], '',[dbo].[tbmTEMPLATE_FIELDS].[fnORDER]
	FROM
		[dbo].[tbmTEMPLATE_FIELDS]
	WHERE
	     --START 2012/12/05 Update By Albert  修正
		 --(fnTEMP_ID = 123)
		 ([dbo].[tbmTEMPLATE_FIELDS].[fnTEMP_ID] =@TEMP_ID)
		 --END   2012/12/05
	ORDER BY
		[dbo].[tbmTEMPLATE_FIELDS].[fnORDER], [dbo].[tbmTEMPLATE_FIELDS].[fsFIELD_NAME]

	/**/	
	DECLARE CSR1 CURSOR FOR SELECT fsFIELD, fsFIELD_TYPE, fsCODE_ID, fsCODE_CTRL FROM @tbResult
	DECLARE @fsFIELD		VARCHAR(50),
			@fsFIELD_TYPE	VARCHAR(50),
			@fsCODE_ID		VARCHAR(20),
			@fsCODE_CTRL	VARCHAR(20),
			@_sVALUE_0		NVARCHAR(MAX) = '',
			@_sVALUE		NVARCHAR(MAX) = '',
			@SQL			NVARCHAR(500)

	OPEN CSR1
		FETCH NEXT FROM CSR1 INTO @fsFIELD, @fsFIELD_TYPE, @fsCODE_ID, @fsCODE_CTRL
		WHILE (@@FETCH_STATUS=0)
			BEGIN 
			---------開始處理每一筆資料
				/*先把指定欄所存的內容取出到參數: 串SQL語法 => 執行結果取回變數@_sVALUE*/
				SET @SQL = 'SELECT @value = ' + @fsFIELD + 
							' FROM t_tbmARC_DOC WHERE (fsFILE_NO = ''' + @fsFILE_NO + ''')'

			--START 2013/05/21 Albert.Chen 修正UniCode的問題		
			--EXEC sp_executesql @SQL, N'@value VARCHAR(MAX) OUTPUT', @_sVALUE_0 OUTPUT
			EXEC [dbo].[sp_executesql] @SQL, N'@value NVARCHAR(MAX) OUTPUT', @_sVALUE_0 OUTPUT  
			--END   2013/05/21 Albert.Chen  修正UniCode的問題

				/*根據欄位資料型態(@fsFIELD_TYPE)決定@_sVALUE*/
				IF (@fsFIELD_TYPE = 'CODE')
				BEGIN
					if (@_sVALUE_0 = '') begin
						set @_sVALUE = '(未選擇)';
					end
					else begin	
						IF (@fsCODE_CTRL = 'ComboBox') /*下拉選單:單選*/
							BEGIN
								SET @_sVALUE = ISNULL((SELECT [dbo].[tbzCODE].[fsNAME] 
												FROM [dbo].[tbzCODE] 
												WHERE [dbo].[tbzCODE].[fsCODE_ID] = @fsCODE_ID AND [dbo].[tbzCODE].[fsCODE] = @_sVALUE_0),'錯誤的代碼:"' + @_sVALUE_0 + '"')
							END
						ELSE IF (@fsCODE_CTRL = 'CheckBox') /*勾選清單:複選*/
							BEGIN
								SET @_sVALUE = dbo.fnGET_CODENAME_FROM_LIST(@fsCODE_ID, @_sVALUE_0)
							END			

						IF (@_sVALUE_0 <> '' AND @_sVALUE = '')
							BEGIN
								SET @_sVALUE = '錯誤的代碼:"' + @_sVALUE_0 + '"'
							END
					end
				END 
				ELSE
					BEGIN
						SET @_sVALUE = @_sVALUE_0
					END
				
				--SET @_sVALUE = (SELECT CASE	@fsFIELD_TYPE
				--					WHEN 'CODE' THEN ( SELECT CASE WHEN @_sVALUE THEN (SELECT fsNAME FROM tbzCODE WHERE fsCODE_ID = @fsCODE_ID AND fsCODE = @fsCODE_ID) ELSE  END)																
				--					WHEN 'NVARCHAR', 'INTEGER' THEN @_sVALUE
				--					WHEN 'INTEGER'	THEN @_sVALUE
				--					WHEN 'DATE'		THEN @_sVALUE
				--					WHEN 'DATETIME'	THEN @_sVALUE
				--					ELSE @_sVALUE END	)
			
				UPDATE @tbResult
				SET fsVALUE = @_sVALUE
				WHERE CURRENT OF CSR1			
			---------處理完畢每一筆資料
			FETCH NEXT FROM CSR1 INTO @fsFIELD, @fsFIELD_TYPE, @fsCODE_ID, @fsCODE_CTRL
			END
	Close CSR1

	SELECT * FROM @tbResult
END




