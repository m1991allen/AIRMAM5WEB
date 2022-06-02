
-- =============================================
-- 描述:	熱門點閱 各統計資料表SQL排程用SP
-- 記錄:	<2011/12/14><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_L_SRH_MULTI_TABLE_BY_TYPES]
	
	@fsTYPE         VARCHAR(50)
	
AS
BEGIN
 	SET NOCOUNT ON;
	BEGIN TRY

	DECLARE @fdSDATE	DATE
	DECLARE @fdEDATE	DATE
	
    DECLARE @fsCREATED_BY NVARCHAR(50)
    
    SET @fsCREATED_BY = 'SYSTEM'
	--SET @fsTYPE       = '3D'
	--SET @fdSDATE	= DATEADD(dd,-3,GETDATE())
	--SET @fdEDATE	= GETDATE()
		
		IF (@fsTYPE = '3D')
		BEGIN
			SET @fdSDATE = DATEADD(DD,-3,GETDATE())
			SET @fdEDATE = DATEADD(DD,-1,GETDATE())		
		END
		ELSE IF (@fsTYPE = '1W')
		BEGIN
			SET @fdSDATE = DATEADD(DD,-7,GETDATE())
			SET @fdEDATE = DATEADD(DD,-1,GETDATE())		
		END
		ELSE IF (@fsTYPE = '1M')
		BEGIN
			SET @fdSDATE = DATEADD(DD,-30,GETDATE())
			SET @fdEDATE = DATEADD(DD,-1,GETDATE())		
		END
		ELSE IF (@fsTYPE = '3M')
		BEGIN
			SET @fdSDATE = DATEADD(DD,-90,GETDATE())
			SET @fdEDATE = DATEADD(DD,-1,GETDATE())		
		END
		ELSE IF (@fsTYPE = '1Y')
		BEGIN
			SET @fdSDATE = DATEADD(DD,-365,GETDATE())
			SET @fdEDATE = DATEADD(DD,-1,GETDATE())		
		END						
		
		--DECLARE CSR5 CURSOR FOR SELECT fsKEYWORD, COUNT(fsKEYWORD) AS CNT_KEYWORD FROM tblSRH_KW WHERE (fdCREATED_DATE >= @fdSDATE) AND (fdCREATED_DATE <= @fdEDATE) GROUP BY fsKEYWORD													
		--下一行的做法才能確保沒有時跟分的問題 ref=> http://havebb.com/b/post/tsql-get-date-part.aspx
		DECLARE CSR5 CURSOR FOR SELECT fsKEYWORD, COUNT(fsKEYWORD) AS CNT_KEYWORD FROM tblSRH_KW WHERE DATEADD(dd, 0, DATEDIFF(dd, 0, fdCREATED_DATE)) BETWEEN @fdSDATE AND @fdEDATE GROUP BY fsKEYWORD															

		DECLARE @KEYWORD NVARCHAR(20), @CNT INT
		
		OPEN CSR5
		FETCH NEXT FROM CSR5 INTO @KEYWORD, @CNT
		WHILE (@@FETCH_STATUS=0)
			BEGIN 
			---------開始處理每一筆資料
	
	
				----目前不知如何合併 ~,~
				
				IF	    (@fsTYPE = '3D') 
				BEGIN	
					--IF EXISTS()
					IF EXISTS(	SELECT * FROM [dbo].[tblSRH_CNT3D] WHERE fdEDATE = @fdEDATE AND fsKEYWORD = @KEYWORD )
						BEGIN
							DELETE FROM [dbo].[tblSRH_CNT3D] WHERE fdEDATE = @fdEDATE AND fsKEYWORD = @KEYWORD
							--新增資料
							INSERT INTO [dbo].[tblSRH_CNT3D](fdSDATE, fdEDATE, fsKEYWORD, fnCOUNT, fdCREATED_DATE, fsCREATED_BY) VALUES( @fdSDATE, @fdEDATE, @KEYWORD, @CNT, GETDATE(), @fsCREATED_BY)
						END
					ELSE
						BEGIN
							--新增資料
							INSERT INTO [dbo].[tblSRH_CNT3D](fdSDATE, fdEDATE, fsKEYWORD, fnCOUNT, fdCREATED_DATE, fsCREATED_BY) VALUES( @fdSDATE, @fdEDATE, @KEYWORD, @CNT, GETDATE(), @fsCREATED_BY)
						END
				END


				IF	    (@fsTYPE = '1W') 
				BEGIN	
					--IF EXISTS()
					IF EXISTS(	SELECT * 
								FROM [dbo].[tblSRH_CNT1W] 
								WHERE fdEDATE = @fdEDATE AND fsKEYWORD = @KEYWORD
							  )
						BEGIN
							DELETE FROM [dbo].[tblSRH_CNT1W] WHERE fdEDATE = @fdEDATE AND fsKEYWORD = @KEYWORD
							--新增資料
							INSERT INTO [dbo].[tblSRH_CNT1W](fdSDATE, fdEDATE, fsKEYWORD, fnCOUNT, fdCREATED_DATE, fsCREATED_BY) VALUES( @fdSDATE, @fdEDATE, @KEYWORD, @CNT, GETDATE(), @fsCREATED_BY)
						END
					ELSE
						BEGIN
							--新增資料
							INSERT INTO [dbo].[tblSRH_CNT1W](fdSDATE, fdEDATE, fsKEYWORD, fnCOUNT, fdCREATED_DATE, fsCREATED_BY) VALUES( @fdSDATE, @fdEDATE, @KEYWORD, @CNT, GETDATE(), @fsCREATED_BY)
						END
				END

				
				IF	    (@fsTYPE = '1M') 
				BEGIN	
					--IF EXISTS()
					IF EXISTS(	SELECT * 
								FROM [dbo].[tblSRH_CNT1M] 
								WHERE fdEDATE = @fdEDATE AND fsKEYWORD = @KEYWORD
							  )
						BEGIN
							DELETE FROM [dbo].[tblSRH_CNT1M] WHERE fdEDATE = @fdEDATE AND fsKEYWORD = @KEYWORD
							--新增資料
							INSERT INTO [dbo].[tblSRH_CNT1M](fdSDATE, fdEDATE, fsKEYWORD, fnCOUNT, fdCREATED_DATE, fsCREATED_BY) VALUES( @fdSDATE, @fdEDATE, @KEYWORD, @CNT, GETDATE(), @fsCREATED_BY)
						END
					ELSE
						BEGIN
							--新增資料
							INSERT INTO [dbo].[tblSRH_CNT1M](fdSDATE, fdEDATE, fsKEYWORD, fnCOUNT, fdCREATED_DATE, fsCREATED_BY) VALUES( @fdSDATE, @fdEDATE, @KEYWORD, @CNT, GETDATE(), @fsCREATED_BY)
						END
				END


				IF	    (@fsTYPE = '3M') 
				BEGIN	
					--IF EXISTS()
					IF EXISTS(	SELECT * 
								FROM [dbo].[tblSRH_CNT3M] 
								WHERE fdEDATE = @fdEDATE AND fsKEYWORD = @KEYWORD
							  )
						BEGIN
							DELETE FROM [dbo].[tblSRH_CNT3M] WHERE fdEDATE = @fdEDATE AND fsKEYWORD = @KEYWORD
							--新增資料
							INSERT INTO [dbo].[tblSRH_CNT3M](fdSDATE, fdEDATE, fsKEYWORD, fnCOUNT, fdCREATED_DATE, fsCREATED_BY) VALUES( @fdSDATE, @fdEDATE, @KEYWORD, @CNT, GETDATE(), @fsCREATED_BY)
						END
					ELSE
						BEGIN
							--新增資料
							INSERT INTO [dbo].[tblSRH_CNT3M](fdSDATE, fdEDATE, fsKEYWORD, fnCOUNT, fdCREATED_DATE, fsCREATED_BY) VALUES( @fdSDATE, @fdEDATE, @KEYWORD, @CNT, GETDATE(), @fsCREATED_BY)
						END
				END


				IF	    (@fsTYPE = '1Y') 
				BEGIN	
					--IF EXISTS()
					IF EXISTS(	SELECT * 
								FROM [dbo].[tblSRH_CNT1Y] 
								WHERE fdEDATE = @fdEDATE AND fsKEYWORD = @KEYWORD
							  )
						BEGIN
							DELETE FROM [dbo].[tblSRH_CNT1Y] WHERE fdEDATE = @fdEDATE AND fsKEYWORD = @KEYWORD
							--新增資料
							INSERT INTO [dbo].[tblSRH_CNT1Y](fdSDATE, fdEDATE, fsKEYWORD, fnCOUNT, fdCREATED_DATE, fsCREATED_BY) VALUES( @fdSDATE, @fdEDATE, @KEYWORD, @CNT, GETDATE(), @fsCREATED_BY)
						END
					ELSE
						BEGIN
							--新增資料
							INSERT INTO [dbo].[tblSRH_CNT1Y](fdSDATE, fdEDATE, fsKEYWORD, fnCOUNT, fdCREATED_DATE, fsCREATED_BY) VALUES( @fdSDATE, @fdEDATE, @KEYWORD, @CNT, GETDATE(), @fsCREATED_BY)
						END
				END

			---------處理完畢每一筆資料
			FETCH NEXT FROM CSR5 INTO @KEYWORD, @CNT
			END
		CLOSE CSR5
		DEALLOCATE CSR5
	    
		
	END TRY
	
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
		CLOSE CSR5
		DEALLOCATE CSR5
	END CATCH
END





