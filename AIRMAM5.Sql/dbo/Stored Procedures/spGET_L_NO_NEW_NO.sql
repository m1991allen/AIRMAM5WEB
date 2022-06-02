


-- =============================================
-- 描述:	取出L_NO主檔資料
-- 記錄:	<2011/11/13><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_L_NO_NEW_NO]
	@fsTYPE		VARCHAR(10),
	@fsNAME		NVARCHAR(10),
	@fsHEAD		VARCHAR(10),
	@fsBODY		VARCHAR(10),
	@fsNO_L		INT,
	@BY			VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

		declare @START_NO int = 0

		if(@fsTYPE = 'VP_YOUTUBE')
		BEGIN
			set @START_NO = 10000
		END
		else if(@fsTYPE = 'TR_VIDEO')
		BEGIN
			set @START_NO = 20000
			--set @fsHEAD = REPLACE(@fsHEAD, '2014', '2222')
		END

		/*還沒有此筆設定時先新增*/
		IF NOT EXISTS(SELECT * FROM tblNO WHERE (fsTYPE = @fsTYPE) AND (fsHEAD = @fsHEAD))
		BEGIN
			BEGIN TRY
				INSERT	tblNO
				SELECT	@fsTYPE, @fsNAME, @fsHEAD, @fsBODY, @START_NO, @fsNO_L, GETDATE(), @BY, '1900/01/01', ''
			END TRY
			BEGIN CATCH
			END CATCH
		END

		DECLARE	@strRESULT	VARCHAR(100) = '',
				@intPRESENT	INT,
				@intNEW	INT,
				@blGETOK	BIT = 0,
				@dateTIME	DATETIME

		WHILE(@blGETOK = 0)
		BEGIN
			SET @dateTIME = GETDATE(); --先取出目前資料庫中的資料時間
			SET @intPRESENT =  (SELECT fsNO FROM tblNO WHERE (fsTYPE = @fsTYPE) AND (fsHEAD = @fsHEAD))
			SET @intNEW = @intPRESENT + 1
				/*若是影音圖文,流水號再加一*/
				IF(@fsTYPE='ARC')
				BEGIN
					SET @intNEW = @intNEW + 1
				END
			SET @strRESULT = CAST(@intNEW AS VARCHAR(100))
			
			--WHILE(LEN(@strRESULT) < @fsNO_L)	
			--BEGIN
			--	SET @strRESULT = '0' + @strRESULT --依照長度來補0
			--END	
			
			/*修改回資料庫同時,檢查是否資料庫中的時間是比我取號時舊的資料*/
			UPDATE	tblNO
			SET		fsNO = @intNEW, fsUPDATED_BY = @BY
			WHERE	(fsTYPE = @fsTYPE) AND (fsHEAD = @fsHEAD) AND 
					(fdCREATED_DATE <= @dateTIME) AND (fdUPDATED_DATE <= @dateTIME)	
			
			IF (@@ROWCOUNT > 0)
				BEGIN
					/*確實有修改到比較舊的資料時*/
					SET @blGETOK = 1
				END
		END

		SET @strRESULT = '00000000000000000000000000000000000000000000000000' + CAST(@intNEW AS VARCHAR(50)) 
		SET @strRESULT = SUBSTRING(@strRESULT, LEN(@strRESULT)-@fsNO_L+1, @fsNO_L) 

		SELECT NEW_NO = @fsHEAD + @fsBODY + @strRESULT
END



