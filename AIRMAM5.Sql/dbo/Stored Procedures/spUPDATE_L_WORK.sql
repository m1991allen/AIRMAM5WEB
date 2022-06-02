

-- =============================================
-- 描述:	修改spUPDATE_L_WORK主檔資料
-- 記錄:	<2011/11/23><Mihsiu.Chiu><新增本預存>
-- 記錄:	<2013/09/03><Albert.Chen><修改本預存><@fsPARAMETERS放寬到300>
--		<2016/11/18><David.Sin><修改SP>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_L_WORK]
	@fnWORK_ID BIGINT,
	@fsSTATUS		varchar(2),
	@fsPROGRESS		nvarchar(50),
	@fsPRIORITY		varchar(1),
	@fdSTIME	    DATETIME,
	@fdETIME	    DATETIME,
	@fsRESULT		nvarchar(100),	
	@fsNOTE		    NVARCHAR(200),
	@fsUPDATED_BY	NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tblWORK
		SET
			fsSTATUS		= @fsSTATUS,		
			fsPROGRESS		= @fsPROGRESS,
			fsPRIORITY		= @fsPRIORITY,
			fdSTIME			= @fdSTIME,
			fdETIME			= @fdETIME,
			fsRESULT		= @fsRESULT,
			fsNOTE			= @fsNOTE,
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fnWORK_ID = @fnWORK_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



