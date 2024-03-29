﻿
-- =============================================
-- 描述:	取出DIR_PATH
-- 記錄:	<2012/01/20><Dennis.Wen><新增本函數>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_SUBJ_PATH_BY_SUBJECT_ID](
	@SUBJECT_ID VARCHAR(12)
)
RETURNS NVARCHAR(500)
AS
BEGIN  
	DECLARE @RESULT	NVARCHAR(500)

	;WITH reDIR AS
	(
	  SELECT 
		[fnDIR_ID],[fnPARENT_ID],
		CAST(CONVERT(VARCHAR(10),[fsNAME]) AS NVARCHAR(2048)) AS [Path_Name]
	  FROM tbmDIRECTORIES
	  WHERE fnDIR_ID = 1
	  UNION ALL
	  SELECT 
		D.[fnDIR_ID],D.[fnPARENT_ID],
		CAST([Path_Name] + CAST('\\' + CONVERT(NVARCHAR(100),D.[fsNAME]) AS NVARCHAR(2048)) AS NVARCHAR(2048)) AS [Path_Name]
	  FROM tbmDIRECTORIES D
	  INNER JOIN reDIR c
	    ON c.[fnDIR_ID] = D.[fnPARENT_ID]
	)
	
	SELECT @RESULT = [Path_Name] FROM reDIR WHERE [fnDIR_ID] = (SELECT fnDIR_ID FROM tbmSUBJECT WHERE fsSUBJ_ID = @SUBJECT_ID)

	SET @RESULT = @RESULT + '\\' + (SELECT fsTITLE FROM tbmSUBJECT WHERE fsSUBJ_ID = @SUBJECT_ID)
	--SET @RESULT = (SELECT [Path_Name] + ' > ' + (SELECT fsTITLE FROM tbmSUBJECT WHERE fsSUBJ_ID = @SUBJECT_ID) FROM reDIR WHERE [fnDIR_ID] = (SELECT fnDIR_ID FROM tbmSUBJECT WHERE fsSUBJ_ID = @SUBJECT_ID))

	--DECLARE
	--	@RESULT		NVARCHAR(500) = '\\',
	--	@PARENT_ID	BIGINT = -1,
	--	@fnDIR_ID	BIGINT = -1
		
	--SELECT @fnDIR_ID = SUBJ.fnDIR_ID
	--FROM tbmSUBJECT SUBJ
	--INNER JOIN tbmDIRECTORIES DIR
	--ON SUBJ.fnDIR_ID = DIR.fnDIR_ID
	--WHERE (fsSUBJ_ID = @SUBJECT_ID)
	
	--IF (@fnDIR_ID > 0)
	--	BEGIN
	--		--SELECT @RESULT = ISNULL(@RESULT + ' [[ ' + (SELECT fsTITLE FROM tbmSUBJECT WHERE (fsSUBJ_ID = @SUBJECT_ID)) + ' ]]','ERROR:錯誤的主題編號或目錄編號')
	--		SELECT @RESULT = ISNULL(@RESULT + '\\' + (SELECT fsTITLE FROM tbmSUBJECT WHERE (fsSUBJ_ID = @SUBJECT_ID)) + '\\','ERROR:錯誤的主題編號或目錄編號')
	--		WHILE(@PARENT_ID <> 0)
	--		BEGIN
	--			SELECT
	--				@RESULT		= '\\' + fsNAME + @RESULT,
	--				@PARENT_ID	= fnPARENT_ID
	--			FROM
	--				tbmDIRECTORIES
	--			WHERE
	--				(fnDIR_ID = @fnDIR_ID)	
					
	--			SET @fnDIR_ID = @PARENT_ID
	--		END			
	--	END
	--ELSE
	--	BEGIN
	--		SET @RESULT = ''
	--	END

	--SELECT @RESULT
	 
	RETURN @RESULT
END


