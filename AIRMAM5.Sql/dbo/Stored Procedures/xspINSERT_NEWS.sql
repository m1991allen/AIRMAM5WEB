

-- =============================================
-- 描述:	新增NEWS主檔資料
-- 記錄:	<2011/08/09><Dennis.Wen><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_NEWS]
	@fsTITLE	NVARCHAR(50),
	@fsCONTENT	NVARCHAR(MAX),
	@fdDATE		DATE,
	@fsDEPT			NVARCHAR(50),
	@fsNOTE			NVARCHAR(100),
	@fsALTERD_BY	VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbmNEWS
			(fsTITLE, fsCONTENT, fdDATE, fsDEPT, fsNOTE, fdALTERD_DATE, fsALTERD_BY)
		VALUES
			(@fsTITLE, @fsCONTENT, @fdDATE, @fsDEPT, @fsNOTE, GETDATE(), @fsALTERD_BY)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



