
-- =============================================
-- 描述:	取出DIR_PATH
-- 記錄:	<2012/01/20><Dennis.Wen><新增本函數>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_DIR_PATH_BY_SUBJECT_ID](
	@SUBJECT_ID VARCHAR(12)
)
RETURNS NVARCHAR(500)
AS
BEGIN   
	DECLARE
		@RESULT		NVARCHAR(500) = '\\',
		@PARENT_ID	BIGINT = -1,
		@fnDIR_ID	BIGINT = -1
		
	SELECT @fnDIR_ID = fnDIR_ID
	FROM tbmSUBJECT
	WHERE (fsSUBJ_ID = @SUBJECT_ID)
		
	IF (@fnDIR_ID > 0)
		BEGIN
			WHILE(@PARENT_ID <> 0)
			BEGIN
				SELECT
					@RESULT		= '\\' + fsNAME + @RESULT,
					@PARENT_ID	= fnPARENT_ID
				FROM
					tbmDIRECTORIES
				WHERE
					(fnDIR_ID = @fnDIR_ID)	
					
				SET @fnDIR_ID = @PARENT_ID
			END
		END
	ELSE
		BEGIN
			SET @RESULT = ''
		END

	--SELECT @RESULT
	 
	RETURN @RESULT
END


