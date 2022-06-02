




-- =============================================
-- 描述:	取出GROUPS主檔資料
-- 記錄:	<2012/03/30><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_GROUPS_BY_FUZZY]
	@fsGROUP_ID		varchar(20) ,
	@fsNAME			nvarchar(50) ,
	@fsDESCRIPTION	nvarchar(50) 
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			fsGROUP_ID, fsNAME, fsDESCRIPTION, fsTYPE, 
			fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE,
			fsUPDATED_BY 

		FROM
			tbmGROUPS AS GROUPS
		WHERE
			((fsGROUP_ID	LIKE  '%' + @fsGROUP_ID    + '%')  OR	(@fsGROUP_ID	= ''))     AND
			((fsNAME 		LIKE  '%' + @fsNAME		   + '%')  OR	(@fsNAME		= ''))     AND
			((fsDESCRIPTION	LIKE  '%' + @fsDESCRIPTION + '%')  OR   (@fsDESCRIPTION	= ''))		
		
		ORDER BY
			fsGROUP_ID ASC
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





