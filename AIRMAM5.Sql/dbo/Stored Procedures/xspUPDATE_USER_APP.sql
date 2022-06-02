


-- =============================================
-- 描述:	修改USER_APP主檔資料
-- 記錄:	<2011/10/18><Eric.Huang><新增本預存>

-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_USER_APP]

	@fnUSER_A_ID	BIGINT,
	@fsLOGIN_ID		NVARCHAR(50),
	@fsPASSWORD		VARCHAR(255),
	@fsNAME			NVARCHAR(50),
	@fsENAME		VARCHAR(50),
	@fsTITLE		NVARCHAR(50),
	@fsDEPT_ID		VARCHAR(10),
	@fsEMAIL		VARCHAR(50),
	@fsPHONE		VARCHAR(20),		
	@fsDESCRIPTION	NVARCHAR(MAX),
	@fsSTATUS       VARCHAR(1),
	@fsNOTE         NVARCHAR(20),	
	@fsUPDATED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		IF (@fsPASSWORD = '')
			BEGIN
				UPDATE
					tbmUSER_APP
				SET			
					fsLOGIN_ID = @fsLOGIN_ID,
					--fsPASSWORD = @fsPASSWORD,
					fsNAME = @fsNAME,
					fsENAME = @fsENAME,
					fsTITLE = @fsTITLE,
					fsDEPT_ID = @fsDEPT_ID,
					fsEMAIL = @fsEMAIL,
					fsPHONE = @fsPHONE,
					fsDESCRIPTION = @fsDESCRIPTION,
					fsSTATUS = @fsSTATUS,	
					fsNOTE   = @fsNOTE,
					
					fdUPDATED_DATE = GETDATE(),
					fsUPDATED_BY = @fsUPDATED_BY
				WHERE
					(fnUSER_A_ID = @fnUSER_A_ID)			
			END
		ELSE
			BEGIN
				UPDATE
					tbmUSER_APP
				SET			
					fsLOGIN_ID = @fsLOGIN_ID,
					fsPASSWORD = @fsPASSWORD,
					fsNAME = @fsNAME,
					fsENAME = @fsENAME,
					fsTITLE = @fsTITLE,
					fsDEPT_ID = @fsDEPT_ID,
					fsEMAIL = @fsEMAIL,
					fsPHONE = @fsPHONE,
					fsDESCRIPTION = @fsDESCRIPTION,
					fsSTATUS = @fsSTATUS,	
					fsNOTE   = @fsNOTE,
					
					fdUPDATED_DATE = GETDATE(),
					fsUPDATED_BY = @fsUPDATED_BY
				WHERE
					(fnUSER_A_ID = @fnUSER_A_ID)				
			END
						
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




