

-- =============================================
-- 描述:	新增L_LOGIN主檔資料
-- 記錄:	<2011/08/23><Mihsiu.Chiu><新增預存
--          <2011/09/05><Eric.Huang><新增fsCheckStatus欄位>
--          <2019/09/06><David.Sin><修正參數>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_L_LOGIN]
	@fsLOGIN_ID	    NVARCHAR(128),
	@fdSTIME	    DATETIME,
	@fdETIME	    DATETIME = NULL,
	@fsNOTE		    NVARCHAR(200) = '',
	@fsCREATED_BY	VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tblLOGIN
			(fsLOGIN_ID, fdSTIME, fdETIME, fsNOTE,
			 fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsLOGIN_ID, @fdSTIME, @fdETIME, @fsNOTE,
			GETDATE(), @fsCREATED_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = IDENT_CURRENT('tblLOGIN')
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


