

-- =============================================
-- 描述:	修改SUBTITLE_T主檔資料
-- 記錄:	<2015/08/17><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_SUBTITLE_T]
	@fnSUB_T_ID		BIGINT,
	@fsNAME			NVARCHAR(50),
	@fsDESCRIPTION	NVARCHAR(50),
	@fsCONTENT		NVARCHAR(1000),
	@fsUPDATED_BY	NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tbmSUBTITLE_T
		SET
			fsNAME = @fsNAME, 
			fsDESCRIPTION = @fsDESCRIPTION,
			fsCONTENT = @fsCONTENT, 
			fdUPDATED_DATE = GETDATE(), 
			fsUPDATED_BY = @fsUPDATED_BY
		WHERE 
			(fnSUB_T_ID = @fnSUB_T_ID)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


