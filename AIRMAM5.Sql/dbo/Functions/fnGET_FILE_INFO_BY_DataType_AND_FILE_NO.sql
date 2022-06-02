
-- =============================================
-- 描述:	取出作縮圖路徑
-- 記錄:	<2011/11/01><Dennis.Wen><新增本函數>
-- 記錄:	<2013/12/03><Albert.Chen><修改本函數><關鍵影格的顯示文字>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_FILE_INFO_BY_DataType_AND_FILE_NO](
	@DataType	VARCHAR(1),
	@fsFILE_NO	VARCHAR(25)
)
returns nvarchar(500)
as
begin   
	DECLARE @_sFILE_INFO VARCHAR(500)

	SELECT
		@_sFILE_INFO = CASE @DataType 
						WHEN 'V' THEN '這是影片檔: ' + @fsFILE_NO
						WHEN 'A' THEN '這是聲音檔: ' + @fsFILE_NO
						WHEN 'P' THEN '這是圖片檔: ' + @fsFILE_NO
						WHEN 'D' THEN '這是文件檔: ' + @fsFILE_NO
						--START 2013/12/03 Update By Albert 關鍵影格編號
						--WHEN 'K' THEN '這是關鍵影格: ' + @fsFILE_NO
						WHEN 'K' THEN '關鍵影格編號: ' + @fsFILE_NO
						--END   2013/12/03 Update By Albert 關鍵影格編號
						WHEN 'S' THEN '這是主題檔: ' + @fsFILE_NO END
 
return @_sFILE_INFO 
end


