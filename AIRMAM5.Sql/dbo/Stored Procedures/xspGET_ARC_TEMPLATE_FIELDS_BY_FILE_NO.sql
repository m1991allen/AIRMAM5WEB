-- =============================================
-- 描述:	依照FILE_NO取出TEMPLATE_FIELDS資料 
-- 記錄:	<2011/12/01><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_ARC_TEMPLATE_FIELDS_BY_FILE_NO]
	@fsFILE_NO	VARCHAR(16),
	@fsTYPE	VARCHAR(1)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		declare @fnTEMP_ID INT;
		declare @sType VARCHAR(5);
		declare @sSQL NVARCHAR(3000), @sParam NVARCHAR(1000);		
		
		IF (@fsTYPE = 'V') BEGIN SET @sType = 'VIDEO' END
		ELSE IF (@fsTYPE = 'A') BEGIN SET @sType = 'AUDIO' END
		ELSE IF (@fsTYPE = 'P') BEGIN SET @sType = 'PHOTO' END
		ELSE IF (@fsTYPE = 'D') BEGIN SET @sType = 'DOC' END
		
		SET @sSQL = 'SELECT @pfnTEMP_ID = fnTEMP_ID_'+@sType+' FROM tbmDIRECTORIES WHERE fnDIR_ID = (
					 SELECT fnDIR_ID FROM tbmSUBJECT WHERE fsSUBJ_ID = (
					 SELECT fsSUBJECT_ID FROM tbmARC_'+@sType+' WHERE fsFILE_NO = '''+@fsFILE_NO+'''));';
		SET @sParam = N'@pfnTEMP_ID varchar(64) OUTPUT'
		
		EXECUTE sp_executeSQL      -- Dynamic T-SQL 取得 TEMP_ID
            @sSQL,
            @sParam,
            @pfnTEMP_ID =@fnTEMP_ID OUTPUT		
		
		EXEC	[dbo].[spGET_TEMPLATE_FIELDS_BY_TEMP_ID]
				@fnTEMP_ID;
		
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END
