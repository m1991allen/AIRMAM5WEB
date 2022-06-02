


-- =============================================
-- 描述:	取出 ARC_VIDEO 自訂欄位資料
-- 記錄:	<2012/02/29><Eric.Huang><新增本預存(原本命名錯誤，故修正)>
--    	<2012/04/11><Mihsiu.Chiu><修改-串代碼資料表時，空值者為未選擇>
--      	<2012/08/30><Mihsiu.Chiu><修改@fsCODE_ID & @fsCODE_CTRL => varchar(20)>
--		<2013/03/15><Dennis.Wen><取出各欄位職的部分不再join主檔>
--      <2013/05/21><Albert.Chen><修正UniCode的問題>
--      <2014/05/13><Dennis.Wen><取出各欄位時依照樣辦order排序>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ARC_VIDEO_ATTRIBUTE]
	@fsFILE_NO	VARCHAR(16)
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @tbResult TABLE(fsFIELD			VARCHAR(50),
								fsFIELD_NAME	NVARCHAR(50),
								fsFIELD_TYPE	VARCHAR(50),
								fnORDER			INT,		--      <2014/05/13><Dennis.Wen>
								fsCODE_ID		VARCHAR(20),
								fsCODE_CTRL		VARCHAR(20),
								fsVALUE			NVARCHAR(MAX),
								fsISNULLABLE	VARCHAR(1),
								fnCODE_CNT		INT
								)

		/*取出此媒體檔所屬的主題檔*/
		DECLARE @fsSUBJECT_ID VARCHAR(12) = (	SELECT fsSUBJECT_ID 
												FROM tbmARC_VIDEO
												WHERE (fsFILE_NO = @fsFILE_NO)	)

		/*取出此主題檔所屬的目錄中, 此媒體檔套用的樣板編號*/
		DECLARE @TEMP_ID INT = (SELECT fnTEMP_ID_VIDEO
								FROM tbmDIRECTORIES
								WHERE (fnDIR_ID = (	SELECT fnDIR_ID 
													FROM tbmSUBJECT
													WHERE (fsSUBJ_ID = @fsSUBJECT_ID) ))	)
													
		/*依據樣板編號取出該樣版有設定的自訂欄位(ORDER BY fnORDER, fsFIELD_NAME)*/
		INSERT
			@tbResult	
		SELECT
			fsFIELD, fsFIELD_NAME, fsFIELD_TYPE, fnORDER, fsCODE_ID, fsCODE_CTRL, '', fsISNULLABLE,fnCODE_CNT
		FROM
			tbmTEMPLATE_FIELDS
		WHERE
			(fnTEMP_ID = @TEMP_ID)
		ORDER BY
			fnORDER, fsFIELD_NAME

		/**/	
		DECLARE CSR1 CURSOR FOR SELECT fsFIELD, fsFIELD_TYPE, fsCODE_ID, fsCODE_CTRL,fnCODE_CNT FROM @tbResult
		DECLARE @fsFIELD		VARCHAR(50),
				@fsFIELD_TYPE	VARCHAR(50),
				@fsCODE_ID		VARCHAR(20),
				@fsCODE_CTRL	VARCHAR(20),
				@_sVALUE_0		NVARCHAR(MAX) = '',
				@_sVALUE		NVARCHAR(MAX) = '',
				@SQL			NVARCHAR(500),
				@fnCODE_CNT		INT

		OPEN CSR1
			FETCH NEXT FROM CSR1 INTO @fsFIELD, @fsFIELD_TYPE, @fsCODE_ID, @fsCODE_CTRL,@fnCODE_CNT
			WHILE (@@FETCH_STATUS=0)
				BEGIN 
				---------開始處理每一筆資料
					/*先把指定欄所存的內容取出到參數: 串SQL語法 => 執行結果取回變數@_sVALUE*/

					--SET @SQL = 'SELECT @value = ' + @fsFIELD + ' 
					--			FROM   tbmARC_VIDEO ARC_VDO 
					--				LEFT JOIN tbmARC_VIDEO_D ARC_VDO_D
					--				ON     (ARC_VDO.fsFILE_NO   = ARC_VDO_D.fsFILE_NO AND 
					--						ARC_VDO_D.fnSEQ_NO = 0)
									        
					--				WHERE ARC_VDO.fsFILE_NO = ''' + @fsFILE_NO + ''''


					--<2013/03/15><Dennis.Wen>
					SET @SQL = 'SELECT @value = ' + @fsFIELD + ' 
								FROM   tbmARC_VIDEO ARC_VDO
								WHERE ARC_VDO.fsFILE_NO = ''' + @fsFILE_NO + ''' '


					--START 2013/05/21 Albert.Chen 修正UniCode的問題		
					--EXEC sp_executesql @SQL, N'@value VARCHAR(MAX) OUTPUT', @_sVALUE_0 OUTPUT
					EXEC sp_executesql @SQL, N'@value NVARCHAR(MAX) OUTPUT', @_sVALUE_0 OUTPUT  
					--END   2013/05/21 Albert.Chen  修正UniCode的問題		
					
					/*根據欄位資料型態(@fsFIELD_TYPE)決定@_sVALUE*/
					IF (@fsFIELD_TYPE = 'CODE')
						BEGIN
							if (@_sVALUE_0 = '' or @_sVALUE_0 = NULL) begin
								--set @_sVALUE = '(未選擇)';
								set @_sVALUE = '';
							end
							else begin	
								IF (@fnCODE_CNT = 1) /*下拉選單:單選*/
									BEGIN
										SET @_sVALUE = ISNULL((SELECT fsNAME 
														FROM tbzCODE 
														WHERE fsCODE_ID = @fsCODE_ID AND fsCODE = @_sVALUE_0),'錯誤的代碼:"' + @_sVALUE_0 + '"')
									END
								ELSE IF (@fnCODE_CNT = 0) /*勾選清單:複選*/
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
				FETCH NEXT FROM CSR1 INTO @fsFIELD, @fsFIELD_TYPE, @fsCODE_ID, @fsCODE_CTRL,@fnCODE_CNT
				END
		Close CSR1
		
		SELECT * FROM @tbResult ORDER BY fnORDER--      <2014/05/13><Dennis.Wen>
END






