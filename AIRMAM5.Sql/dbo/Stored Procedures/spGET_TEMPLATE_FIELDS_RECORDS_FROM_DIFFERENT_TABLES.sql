

-- =============================================
-- 描述:	依照 TEMP_ID & field 取出 存在不同TABLE的資料筆數
--			(tbmSUBJECT/tbmARC_VIDEO_D/tbmARC_AUDIO/tbmARC_PHOTO/tbmARC_DOC) 
-- 記錄:	<2011/10/17><Mihsiu.Chiu><新增本預存>
--			<2012/04/06><Mihsiu.Chiu><修改>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_TEMPLATE_FIELDS_RECORDS_FROM_DIFFERENT_TABLES]
	@fsFIELD varchar(50),
	@fnTEMP_ID int,
	@fsTABLENAME VARCHAR(50),
	@fsTABLE VARCHAR(1)	
AS
BEGIN
 	SET NOCOUNT ON;

	declare @ssql VARCHAR(3000)
	IF (@fsTABLE = 'S') BEGIN  
		SET @ssql = 'SELECT _RecordCNT = COUNT(*) from tbmSUBJECT S, tbmDIRECTORIES AS D '
				  + 'WHERE S.fnDIR_ID = D.fnDIR_ID AND '+@fsFIELD+' <> '''' '
				  + 'AND (fnTEMP_ID_SUBJECT = ' +convert(varchar(20),@fnTEMP_ID)+')';
	END 
	ELSE BEGIN		
		SET @ssql = 'SELECT _RecordCNT = COUNT(*) from '+@fsTABLENAME +' AS A, tbmSUBJECT S, tbmDIRECTORIES AS D '
				  + 'WHERE A.fsSUBJECT_ID = S.fsSUBJ_ID AND S.fnDIR_ID = D.fnDIR_ID '
				  + 'AND A.'+@fsFIELD+' <> '''' AND '
	
		if (@fsTABLE = 'V') begin SET @ssql = @ssql + '(fnTEMP_ID_VIDEO = '+convert(varchar(20),@fnTEMP_ID)+')' end
		else if (@fsTABLE = 'A') begin SET @ssql = @ssql + '(fnTEMP_ID_AUDIO = '+convert(varchar(20),@fnTEMP_ID)+')' end
		else if (@fsTABLE = 'P') begin SET @ssql = @ssql + '(fnTEMP_ID_PHOTO = '+convert(varchar(20),@fnTEMP_ID)+')' end
		else if (@fsTABLE = 'D') begin SET @ssql = @ssql + ' (fnTEMP_ID_DOC = '+convert(varchar(20),@fnTEMP_ID)+')' end
	END

	EXEC(@ssql);
END



