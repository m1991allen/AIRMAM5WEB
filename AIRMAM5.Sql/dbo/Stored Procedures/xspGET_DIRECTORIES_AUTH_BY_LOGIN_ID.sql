



-- =============================================
-- 描述:	取出全部 DIRECTORIES主檔資料 BY LOGIN ID 的權限
-- 記錄:	<2015/07/09><Eric.Huang><新增>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_DIRECTORIES_AUTH_BY_LOGIN_ID]

	@fsLOGIN_ID			VARCHAR(50)
	
AS
BEGIN
	SET NOCOUNT ON;
	
	BEGIN TRY

	DECLARE @fnUSER_ID INT = ISNULL((SELECT TOP 1 [fnUSER_ID] FROM [dbo].[tbmUSERS] WHERE ([fsLOGIN_ID] = @fsLOGIN_ID)), -1)
	DECLARE @fsUSER_GROUPS NVARCHAR(500) = dbo.fnGET_GROUPS_BY_USER_ID(@fnUSER_ID)
	--@fsUSER_GROUPS為此帳號的所有系統群組

	IF(@fnUSER_ID = -1)
		BEGIN
			SELECT 'ERROR:找不到這一個帳號.'
		END
	ELSE
		BEGIN

			---

			CREATE TABLE #tblRESULT ( LV0_NAME NVARCHAR(50), LV1_NAME NVARCHAR(50), LV2_NAME NVARCHAR(50), LV3_NAME NVARCHAR(50), LV4_NAME NVARCHAR(50), 
									  LV5_NAME NVARCHAR(50), LV6_NAME NVARCHAR(50), LV7_NAME NVARCHAR(50), LV8_NAME NVARCHAR(50), LV9_NAME NVARCHAR(50), 
									LV0_ORDER INT, LV1_ORDER INT, LV2_ORDER INT, LV3_ORDER INT, LV4_ORDER INT, 
									LV5_ORDER INT, LV6_ORDER INT, LV7_ORDER INT, LV8_ORDER INT, LV9_ORDER INT, 
									LV0_ID INT, LV1_ID INT, LV2_ID INT, LV3_ID INT, LV4_ID INT, 
									LV5_ID INT, LV6_ID INT, LV7_ID INT, LV8_ID INT, LV9_ID INT, _DIRTYPE VARCHAR(1), _DIRID BIGINT, _DIRPID BIGINT,  _SUBJLIST NVARCHAR(MAX), _DIRNAME NVARCHAR(50),
									_ADMIN VARCHAR(5), _檢視 VARCHAR(5), _主題 VARCHAR(5), _影片 VARCHAR(5), _聲音 VARCHAR(5), _圖片 VARCHAR(5), _文件 VARCHAR(5), _顯示 VARCHAR(5))

			CREATE TABLE #tblQUEUE (ID INT, DIR_ID INT, _LIST VARCHAR(50), _DIRTYPE VARCHAR(1), _DIRID BIGINT, _DIRPID BIGINT, _DIRNAME NVARCHAR(50)) 

			-----

			INSERT #tblQUEUE 
			SELECT ROW_NUMBER() OVER(ORDER BY fnDIR_ID), fnDIR_ID, dbo.fnGET_DIR_PARENT_LIST(fnDIR_ID), fsDIRTYPE, fnDIR_ID, fnPARENT_ID, fsNAME
			FROM tbmDIRECTORIES AS DD 
			--WHERE fnDIR_ID NOT IN (select distinct [fnPARENT_ID] from [dbo].[tbmDIRECTORIES])

			-----

			INSERT	#tblRESULT
			SELECT	D0.fsNAME, 
					D1.fsNAME, 
					D2.fsNAME, 
					D3.fsNAME, 
					D4.fsNAME,
					D5.fsNAME, 
					D6.fsNAME, 
					D7.fsNAME, 
					D8.fsNAME, 
					D9.fsNAME,
					D0.fnORDER,	D1.fnORDER,	D2.fnORDER,	D3.fnORDER, D4.fnORDER,
					D5.fnORDER, D6.fnORDER,	D7.fnORDER,	D8.fnORDER, D9.fnORDER,
					[dbo].[fnGET_ITEM_BY_INDEX](_LIST, 0),
					[dbo].[fnGET_ITEM_BY_INDEX](_LIST, 1),
					[dbo].[fnGET_ITEM_BY_INDEX](_LIST, 2),
					[dbo].[fnGET_ITEM_BY_INDEX](_LIST, 3),
					[dbo].[fnGET_ITEM_BY_INDEX](_LIST, 4),
					[dbo].[fnGET_ITEM_BY_INDEX](_LIST, 5),
					[dbo].[fnGET_ITEM_BY_INDEX](_LIST, 6),
					[dbo].[fnGET_ITEM_BY_INDEX](_LIST, 7),
					[dbo].[fnGET_ITEM_BY_INDEX](_LIST, 8),
					[dbo].[fnGET_ITEM_BY_INDEX](_LIST, 9),
					_DIRTYPE, _DIRID, _DIRPID,  '', _DIRNAME,
					'', '', '', '', '', '', '', ''
			FROM	#tblQUEUE AS Q
					LEFT JOIN tbmDIRECTORIES AS D0 ON (D0.fnDIR_ID = [dbo].[fnGET_ITEM_BY_INDEX](_LIST, 0))
					LEFT JOIN tbmDIRECTORIES AS D1 ON (D1.fnDIR_ID = [dbo].[fnGET_ITEM_BY_INDEX](_LIST, 1))
					LEFT JOIN tbmDIRECTORIES AS D2 ON (D2.fnDIR_ID = [dbo].[fnGET_ITEM_BY_INDEX](_LIST, 2))
					LEFT JOIN tbmDIRECTORIES AS D3 ON (D3.fnDIR_ID = [dbo].[fnGET_ITEM_BY_INDEX](_LIST, 3))
					LEFT JOIN tbmDIRECTORIES AS D4 ON (D4.fnDIR_ID = [dbo].[fnGET_ITEM_BY_INDEX](_LIST, 4))
					LEFT JOIN tbmDIRECTORIES AS D5 ON (D5.fnDIR_ID = [dbo].[fnGET_ITEM_BY_INDEX](_LIST, 5))
					LEFT JOIN tbmDIRECTORIES AS D6 ON (D6.fnDIR_ID = [dbo].[fnGET_ITEM_BY_INDEX](_LIST, 6))
					LEFT JOIN tbmDIRECTORIES AS D7 ON (D7.fnDIR_ID = [dbo].[fnGET_ITEM_BY_INDEX](_LIST, 7))
					LEFT JOIN tbmDIRECTORIES AS D8 ON (D8.fnDIR_ID = [dbo].[fnGET_ITEM_BY_INDEX](_LIST, 8))
					LEFT JOIN tbmDIRECTORIES AS D9 ON (D9.fnDIR_ID = [dbo].[fnGET_ITEM_BY_INDEX](_LIST, 9))

			-----
   
			/*檢查所有目錄 若帳號或群組有管理權限 => 「管理」欄標記為 "A"*/
			UPDATE	#tblRESULT
			SET		_ADMIN = 'A',
					_顯示 = 'Y'
			WHERE	(dbo.fnGET_STRING_MATCH(
						(SELECT TOP 1 [fsADMIN_GROUP] FROM [dbo].[tbmDIRECTORIES] WHERE ([fnDIR_ID] = _DIRID)), @fsUSER_GROUPS
					) = 'Y') OR 
					(dbo.fnGET_STRING_MATCH(
						(SELECT TOP 1 [fsADMIN_USER] FROM [dbo].[tbmDIRECTORIES] WHERE ([fnDIR_ID] = _DIRID)), (@fsLOGIN_ID + ';')
					) = 'Y')


			/*檢查所有「管理」為"A"目錄以下的目錄, 若「管理」欄為空 => 「管理」欄標記為 "a"*/
			DECLARE @i INT = 0

			WHILE(@i < 10)
			BEGIN
				UPDATE	A
				SET		_ADMIN = 'a',
						_顯示 = 'Y'
				FROM	#tblRESULT AS A
					LEFT JOIN #tblRESULT AS B ON (B._DIRID = A._DIRPID)
				WHERE	(B._ADMIN = 'A') AND (A._ADMIN = '')

				SET @i += 1
			END


			/*檢查所有目錄 若帳號或群組有檢視或編輯權限 => 「檢視」欄標記為 "V"
														   其他「_」開頭欄標記權限VIUD*/
			UPDATE	#tblRESULT
			SET		_主題 = dbo.fnGET_USER_DIR_LIMIT(@fsLOGIN_ID, _DIRID, 'S'),
					_影片 = dbo.fnGET_USER_DIR_LIMIT(@fsLOGIN_ID, _DIRID, 'V'),
					_聲音 = dbo.fnGET_USER_DIR_LIMIT(@fsLOGIN_ID, _DIRID, 'A'),
					_圖片 = dbo.fnGET_USER_DIR_LIMIT(@fsLOGIN_ID, _DIRID, 'P'),
					_文件 = dbo.fnGET_USER_DIR_LIMIT(@fsLOGIN_ID, _DIRID, 'D')

			UPDATE	#tblRESULT
			SET		_檢視 = 'V',
					_顯示 = 'Y'
			WHERE	((_主題+_影片+_聲音+_圖片+_文件)<>'')


			/*檢查所有「檢視」為"V"目錄以下的目錄, 若「檢視」欄為空 => 「檢視」欄標記為 "v"*/
			SET @i = 0

			WHILE(@i < 10)
			BEGIN
				UPDATE	A
				SET		_檢視 = 'v',
						_主題 = LOWER(B._主題),
						_影片 = LOWER(B._影片),
						_聲音 = LOWER(B._聲音),
						_圖片 = LOWER(B._圖片),
						_文件 = LOWER(B._文件),
						_顯示 = 'Y'
				FROM	#tblRESULT AS A
					LEFT JOIN #tblRESULT AS B ON (B._DIRID = A._DIRPID)
				WHERE	(B._檢視 = 'V') AND (A._檢視 = '')

				SET @i += 1
			END


			/*由下往上檢查, 若因為下層有需要掛的而只好顯示的節點標為y*/
			SET @i = 0

			WHILE(@i < 10)
			BEGIN
				UPDATE	A
				SET		_顯示 = 'y'
				FROM	#tblRESULT AS A
					LEFT JOIN #tblRESULT AS B ON (B._DIRPID = A._DIRID)
				WHERE	(B._顯示 = 'Y') AND (A._顯示 = '')

				SET @i += 1
			END
		 
			-----
 
			SELECT * 
			FROM #tblRESULT 
			WHERE _顯示 <> ''
			ORDER BY 
				LV0_ORDER, LV0_NAME, 
				LV1_ORDER, LV1_NAME, 
				LV2_ORDER, LV2_NAME, 
				LV3_ORDER, LV3_NAME, 
				LV4_ORDER, LV4_NAME 
				--LV0_NAME, LV1_NAME, LV2_NAME, LV3_NAME, LV4_NAME,
				--LV5_NAME, LV6_NAME, LV7_NAME, LV8_NAME, LV9_NAME 

			drop table #tblRESULT
			drop table #tblQUEUE
			-----
		END   

END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



