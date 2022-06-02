


-- =============================================
-- 描述:	取出RESOLUTION主檔資料
-- 記錄:	<2012/04/26><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_RESOLUTION_NEW]
	@fnRESOL_ID	INT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
	
		DECLARE @i INT,
	            @cnt INT,
		        @fsRADIO VARCHAR(20),
		        @strTMP			NVARCHAR(50),	 --自行運用	        	        	        
		        @strSPLIT		NVARCHAR(200),
		        @strRESULT		NVARCHAR(MAX)	 --顯示結果
		        
		SET @strRESULT = ''
		SET @i = 0

--		SET @fsRADIO = (SELECT fsRATIO FROM tbzRESOLUTION WHERE fnRESOL_ID = @fnRESOL_ID)
		SET @fsRADIO = '4:3;5:4;15;'
	    SELECT @cnt = LEN(REPLACE(@fsRADIO,';',';;')) - LEN(@fsRADIO)
	    
	    WHILE (@cnt > @i)
		BEGIN
			SET @strSPLIT = dbo.fnGET_ITEM_BY_INDEX(@fsRADIO,@i)
			SET @strTMP = isnull((SELECT fsNAME from dbo.tbzCODE WHERE fsCODE_ID = 'RESOL003' AND fsCODE = @strSPLIT),'00')
			SET @i= @i + 1
			SET @strRESULT = @strRESULT + @strTMP+'/' 			
		END
		
	    --SELECT @strRESULT
	    
		SELECT 
			fnRESOL_ID, fsRATIO, fsNAME, fsWIDTH, fsHEIGHT, fdCREATED_DATE, fsCREATED_BY, 
			fdUPDATED_DATE, fsUPDATED_BY,@strRESULT
		FROM
			tbzRESOLUTION
		WHERE
			(fnRESOL_ID = @fnRESOL_ID)
		ORDER BY
			fnRESOL_ID DESC
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

