

-- =============================================
-- 描述:	新增 TSM VOLUME 資料
-- 記錄:	<2013/10/30><Albert.Chen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_TSM_TBTVOLUME]

            @fsTYPE_NAME          AS VARCHAR(50),
            @fsUSE_STATUS         AS VARCHAR(50),
            @fsREAD_WRITE_STATUS  AS VARCHAR(50),
            @fnDATA_QUANTIT       AS VARCHAR(50),
            @fsSTORE_POOL         AS VARCHAR(50),
            @fdLAST_READ          AS VARCHAR(50),
            @fdLAST_WRITE         AS VARCHAR(50),
            @fnREAD_ERROR_COUNT   AS VARCHAR(50),
            @fnWIRTE_ERROR_COUNT  AS VARCHAR(50),
            @fsTAPE_POSITION      AS VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY	
	        IF (@fdLAST_READ = 1900/01/01)
			  BEGIN
			      SET @fdLAST_READ = NULL
			  END
			IF (@fdLAST_WRITE = 1900/01/01)
			  BEGIN
			      SET @fdLAST_WRITE = NULL
			  END

			INSERT tbtVOLUME
				(	[磁帶名稱], [使用狀態], [讀寫狀態], [已存放資料量] , [儲存池], [最後讀取],	
					[最後寫入], [讀取錯誤], [寫入錯誤], [磁帶位置]	)
			VALUES
				(	@fsTYPE_NAME, @fsUSE_STATUS, @fsREAD_WRITE_STATUS, @fnDATA_QUANTIT, @fsSTORE_POOL, @fdLAST_READ,
					@fdLAST_WRITE, @fnREAD_ERROR_COUNT, @fnWIRTE_ERROR_COUNT, @fsTAPE_POSITION	)			
		
		SELECT RESULT = ''		
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + ERROR_MESSAGE()
	END CATCH
END




