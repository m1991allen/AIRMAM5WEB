


-- =============================================
-- 描述:	取出入庫項目路徑
-- 記錄:	<2011/11/16><Dennis.Wen><新增本預存>
--			<2011/11/24><Dennis.Wen><調整為透過函數去組路徑>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_ARC_FILE_PATH]
	@SUBJECT_ID	VARCHAR(12),
	@FILE_NO	VARCHAR(16),
	@FILE_TYPE	VARCHAR(10),
	@_sTYPE		VARCHAR(1)
AS
BEGIN

	--SELECT
	--	@SUBJECT_ID	= '123',
	--	@FILE_NO	= '20111116_1234567',
	--	@FILE_TYPE	= 'WMV',
	--	@_sTYPE		= 'V'
	
 	SET NOCOUNT ON;

	--BEGIN TRY
	--	DECLARE
	--		@fsFILE_PATH	NVARCHAR(100)
			
	--	SET @fsFILE_PATH = dbo.fnGET_MEDIA_PATH() + @SUBJECT_ID + '\'

	--	IF (@_sTYPE<>'')
	--	BEGIN
	--		SET @fsFILE_PATH = @fsFILE_PATH + @_sTYPE + '\'
	--	END 

	--	SELECT FILE_PATH = @fsFILE_PATH
	--END TRY
	--BEGIN CATCH
	--	SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	--END CATCH
	
	SELECT FILE_PATH = dbo.fnGET_ARC_FILE_PATH(@SUBJECT_ID,@FILE_NO,@FILE_TYPE,@_sTYPE)
END




