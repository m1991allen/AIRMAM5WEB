
-- =============================================
-- 描述:	判斷CODE是否被其它table使用
-- 記錄:	<2011/08/30><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_CODE_USED]
	@TableName		varchar(50),
	@FieldName		varchar(50),
	@Value			varchar(20)
AS
BEGIN
	declare @SQL varchar(256)
	SET @SQL = 'SELECT RESULT = CASE WHEN COUNT(' + @FieldName + ') > 0 THEN ''Y'' ELSE ''N'' END 
				FROM ' + @TableName + ' WHERE ' +@FieldName + ' = '''+@Value+''''
	SET NOCOUNT ON;

	EXEC (@SQL)
END

