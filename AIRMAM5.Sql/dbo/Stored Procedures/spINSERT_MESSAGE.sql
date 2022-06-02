

-- =============================================
-- 描述:	新增MESSAGE
-- 記錄:	<2015/07/30><Dennis.Wen><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_MESSAGE]
	@fsFROM_ID		NVARCHAR(50),
	@fsINFO			NVARCHAR(500),
	@_TO_USER		NVARCHAR(50),
	@_TO_GROUP		NVARCHAR(50) 
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

---
		--SELECT	@fsFROM_ID		= 'dennis.wen',
		--		@fsINFO			= 'DENNIS:測試訊息測試訊息',
		--		@_TO_USER		= 'aaa',
		--		@_TO_GROUP		= 'Administrators' 
	---
 

		/*變數宣告*/
		DECLARE @i INT = 0, @CNT INT = 0, @CREATED_DATE DATETIME = GETDATE()
		DECLARE @GROUP_ID NVARCHAR(100) = (@fsFROM_ID + ':' + CONVERT(VARCHAR(19), @CREATED_DATE,120))

		CREATE TABLE #tblTO_ID_LIST(
			TO_ID	NVARCHAR(50)
		)

		/*塞個別帳號*/
		IF(@_TO_USER <> '')
			INSERT #tblTO_ID_LIST SELECT @_TO_USER

		/*塞群組帳號*/
		IF(@_TO_GROUP <> '')
			INSERT #tblTO_ID_LIST SELECT [fsLOGIN_ID] FROM [dbo].[tbmUSERS] WHERE ([fnUSER_ID] IN (SELECT [fnUSER_ID] FROM [dbo].[tbmUSER_GROUP] WHERE [fsGROUP_ID] = @_TO_GROUP))

		--SELECT * FROM #tblTO_ID_LIST

		/*新增MESSAGE*/
		INSERT [dbo].[tblMESSAGE] 
		SELECT NEWID(), @fsFROM_ID, TO_ID, @fsINFO, @CREATED_DATE, '', 0, 0, @GROUP_ID
		FROM #tblTO_ID_LIST

		---

		/*移除暫時資料表*/
		DROP TABLE #tblTO_ID_LIST
---	

		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

