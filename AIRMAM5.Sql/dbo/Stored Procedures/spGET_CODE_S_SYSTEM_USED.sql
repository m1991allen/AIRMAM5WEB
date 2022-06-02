
-- =============================================
-- 描述:	判斷一般代碼群組是否被其它table使用
-- 記錄:	<2012/04/06><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_CODE_S_SYSTEM_USED]
	@TableName		varchar(50),
	@FieldName		varchar(50)
AS
BEGIN
	declare @SQL varchar(256)
	SET @SQL = 'SELECT RESULT = CASE WHEN COUNT(' + @FieldName + ') > 0 THEN ''Y'' ELSE ''N'' END 
				FROM ' + @TableName 
	SET NOCOUNT ON;

	EXEC (@SQL)
END

