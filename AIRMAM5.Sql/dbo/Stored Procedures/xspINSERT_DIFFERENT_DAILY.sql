-- =============================================
-- Author:		<Dennis.Wen>
-- ALTER date: <2012/05/01>
-- Description:	<每日用來處理t_tbmINCREMENTAL中異動資料的預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_DIFFERENT_DAILY]
 
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE
		@tblTEMP1 TABLE(fnINDEX_ID	BIGINT,
						fsDATA_TYPE	VARCHAR(10),
						fsCODE_ID	VARCHAR(10),
						fsCODE		VARCHAR(20),
						fnDIR_ID	BIGINT )
						
	INSERT
		@tblTEMP1
	SELECT
		fnINDEX_ID, fsDATA_TYPE, fsCODE_ID, fsCODE, fnDIR_ID
	FROM
		t_tbmINCREMENTAL
	WHERE
		(fsSTATUS = '00') OR (fsSTATUS = '01')
	ORDER BY
		fnINDEX_ID
						 
	--	fsDATA_TYPE, fsCODE_ID, fsCODE, fnDIR_ID


	DECLARE CSR1 CURSOR FOR SELECT fnINDEX_ID, fsDATA_TYPE, fsCODE_ID, fsCODE, fnDIR_ID FROM @tblTEMP1
	DECLARE	@fnINDEX_ID		BIGINT,
			@fsDATA_TYPE	VARCHAR(10), 
			@fsCODE_ID		VARCHAR(10), 
			@fsCODE			VARCHAR(20), 
			@fnDIR_ID		BIGINT,
			@DATA_ID		VARCHAR(16)
	DECLARE	@tblTEMP2		TABLE([TYPE] VARCHAR(1), DATA_ID VARCHAR(16), CONDITION VARCHAR(10), VALUE VARCHAR(30), SEQ_NO INT)
			
	OPEN CSR1
		FETCH NEXT FROM CSR1 INTO @fnINDEX_ID, @fsDATA_TYPE, @fsCODE_ID, @fsCODE, @fnDIR_ID
		WHILE (@@FETCH_STATUS=0)
			BEGIN 
				SET @DATA_ID = ''
				
			BEGIN TRY
				---------開始處理每一筆異動資料
				--SELECT '=>', @fnINDEX_ID, @fsDATA_TYPE, @fsCODE_ID, @fsCODE, @fnDIR_ID
				
				/*檢查主題檔*/
				INSERT	@tblTEMP2
				SELECT	'S', fsSUBJ_ID, CASE WHEN ((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID)) THEN 'QUEUE' ELSE 'CODE' END
									  , CASE WHEN ((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID)) THEN CAST(@fnDIR_ID AS VARCHAR(30)) ELSE @fsCODE_ID + ':' + @fsCODE END, 0
				FROM	vwINFO_SUBJECT
				WHERE	((@fsDATA_TYPE = 'CODE') AND ([dbo].[fnGET_CHECK_CODE_USED](_sCODE_USED, @fsCODE_ID, @fsCODE) = 'Y')) OR
						((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID))
						
				/*檢查影片檔*/
				INSERT	@tblTEMP2
				SELECT	'V', fsFILE_NO, CASE WHEN ((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID)) THEN 'QUEUE' ELSE 'CODE' END
									  , CASE WHEN ((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID)) THEN CAST(@fnDIR_ID AS VARCHAR(30)) ELSE @fsCODE_ID + ':' + @fsCODE END, fnSEQ_NO
				FROM	vwINFO_VIDEO_D
				WHERE	((@fsDATA_TYPE = 'CODE') AND ([dbo].[fnGET_CHECK_CODE_USED](_sCODE_USED, @fsCODE_ID, @fsCODE) = 'Y')) OR
						((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID))
						
				/*檢查聲音檔*/
				INSERT	@tblTEMP2
				SELECT	'A', fsFILE_NO, CASE WHEN ((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID)) THEN 'QUEUE' ELSE 'CODE' END
									  , CASE WHEN ((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID)) THEN CAST(@fnDIR_ID AS VARCHAR(30)) ELSE @fsCODE_ID + ':' + @fsCODE END, 0
				FROM	vwINFO_AUDIO
				WHERE	((@fsDATA_TYPE = 'CODE') AND ([dbo].[fnGET_CHECK_CODE_USED](_sCODE_USED, @fsCODE_ID, @fsCODE) = 'Y')) OR
						((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID))
						
				/*檢查聲音檔*/
				INSERT	@tblTEMP2
				SELECT	'P', fsFILE_NO, CASE WHEN ((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID)) THEN 'QUEUE' ELSE 'CODE' END
									  , CASE WHEN ((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID)) THEN CAST(@fnDIR_ID AS VARCHAR(30)) ELSE @fsCODE_ID + ':' + @fsCODE END, 0
				FROM	vwINFO_PHOTO
				WHERE	((@fsDATA_TYPE = 'CODE') AND ([dbo].[fnGET_CHECK_CODE_USED](_sCODE_USED, @fsCODE_ID, @fsCODE) = 'Y')) OR
						((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID))
						
				/*檢查聲音檔*/
				INSERT	@tblTEMP2
				SELECT	'D', fsFILE_NO, CASE WHEN ((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID)) THEN 'QUEUE' ELSE 'CODE' END
									  , CASE WHEN ((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID)) THEN CAST(@fnDIR_ID AS VARCHAR(30)) ELSE @fsCODE_ID + ':' + @fsCODE END, 0
				FROM	vwINFO_DOC
				WHERE	((@fsDATA_TYPE = 'CODE') AND ([dbo].[fnGET_CHECK_CODE_USED](_sCODE_USED, @fsCODE_ID, @fsCODE) = 'Y')) OR
						((@fsDATA_TYPE = 'QUEUE') AND (_sDIR_ID = @fnDIR_ID))
					
				
				/*先不採以下的方式,改成全部找出來後一次處理,但這樣只能判斷全成功或全失敗...不過在這邊應該還好,只是資料新增到DIFFERENT*/
				
				--/*到這邊為止,此筆異動項的相關項目都找到了,接著要寫到DIFFERENT*/
				
				--/*到這邊為止,處理完一個異動項目了,把狀態改為'90'*/
				--UPDATE [t_tbmINCREMENTAL]
				--SET fsSTATUS = '90', fdUPDATED_DATE = GETDATE(), fsUPDATED_BY = 'SYSTEM_DAILY'
				--WHERE fnINDEX_ID = @fnINDEX_ID
				
				--/*清空暫存資料表*/
				--DELETE FROM @tblTEMP2
				
				UPDATE [t_tbmINCREMENTAL]
				SET fsSTATUS = '01', fdUPDATED_DATE = GETDATE(), fsUPDATED_BY = 'SYSTEM_DAILY'
				WHERE fnINDEX_ID = @fnINDEX_ID
			END TRY
			BEGIN CATCH
				UPDATE [t_tbmINCREMENTAL]
				SET fsSTATUS = 'E1', fdUPDATED_DATE = GETDATE(), fsUPDATED_BY = 'SYSTEM_DAILY'
				WHERE fnINDEX_ID = @fnINDEX_ID
			END CATCH
			---------處理完畢每一筆異動資料
			FETCH NEXT FROM CSR1 INTO @fnINDEX_ID, @fsDATA_TYPE, @fsCODE_ID, @fsCODE, @fnDIR_ID
			END
	Close CSR1



	/*到這邊一次處理所有的相關項目*/
	DECLARE	@tblTEMP3		TABLE([TYPE] VARCHAR(1), DATA_ID VARCHAR(16), SEQ_NO INT)
	INSERT @tblTEMP3 SELECT DISTINCT [TYPE], DATA_ID, SEQ_NO FROM @tblTEMP2

	SELECT * FROM @tblTEMP3
		
	BEGIN TRY
		DECLARE CSR2 CURSOR FOR SELECT [TYPE], DATA_ID, SEQ_NO FROM @tblTEMP3
		DECLARE @_TYPE VARCHAR(1), @_DATA_ID VARCHAR(16), @_SEQ_NO INT

		OPEN CSR2
			FETCH NEXT FROM CSR2 INTO @_TYPE, @_DATA_ID, @_SEQ_NO
			WHILE (@@FETCH_STATUS=0)
				BEGIN 
				---------開始處理每一筆資料
					--SELECT @_TYPE, @_DATA_ID, @_CONDITION, @_VALUE
					
					IF(@_TYPE = 'S')
					BEGIN
						INSERT [dbo].[tbdARC_SUBJECT_DIFFERENT](fsSYS_ID,Mode) 
						SELECT @_DATA_ID ,3 
						INSERT [dbo].[tbdARC_SUBJECT_DIFFERENT](fsSYS_ID,Mode) 
						SELECT @_DATA_ID ,1 
					END
					
					ELSE IF(@_TYPE = 'V')
					BEGIN
						INSERT [dbo].[tbdARC_VIDEO_DIFFERENT](fsSYS_ID,Mode) 
						SELECT @_DATA_ID+'_'+Convert(varchar(3),@_SEQ_NO), 3
						INSERT [dbo].[tbdARC_VIDEO_DIFFERENT](fsSYS_ID,Mode) 
						SELECT @_DATA_ID+'_'+Convert(varchar(3),@_SEQ_NO), 1			
					END
					
					ELSE IF(@_TYPE = 'A')
					BEGIN
						INSERT [dbo].[tbdARC_AUDIO_DIFFERENT](fsSYS_ID,Mode) 
						SELECT @_DATA_ID, 3
						INSERT [dbo].[tbdARC_AUDIO_DIFFERENT](fsSYS_ID,Mode) 
						SELECT @_DATA_ID, 1	
					END
					
					ELSE IF(@_TYPE = 'P')
					BEGIN
						INSERT [dbo].[tbdARC_PHOTO_DIFFERENT](fsSYS_ID,Mode) 
						SELECT @_DATA_ID, 3
						INSERT [dbo].[tbdARC_PHOTO_DIFFERENT](fsSYS_ID,Mode) 
						SELECT @_DATA_ID, 1	
					END
					
					ELSE IF(@_TYPE = 'D')
					BEGIN
						INSERT [dbo].[tbdARC_DOC_DIFFERENT](fsSYS_ID,Mode) 
						SELECT @_DATA_ID, 3
						INSERT [dbo].[tbdARC_DOC_DIFFERENT](fsSYS_ID,Mode) 
						SELECT @_DATA_ID, 1	
					END			
				---------處理完畢每一筆資料
				FETCH NEXT FROM CSR2 INTO @_TYPE, @_DATA_ID, @_SEQ_NO
				END
		Close CSR2
		
		UPDATE [t_tbmINCREMENTAL]
		SET fsSTATUS = '90', fdUPDATED_DATE = GETDATE(), fsUPDATED_BY = 'SYSTEM_DAILY'
		WHERE fnINDEX_ID IN (SELECT fnINDEX_ID FROM @tblTEMP1)
	END TRY
	BEGIN CATCH
		UPDATE [t_tbmINCREMENTAL]
		SET fsSTATUS = 'E1', fdUPDATED_DATE = GETDATE(), fsUPDATED_BY = 'SYSTEM_DAILY'
		WHERE fnINDEX_ID IN (SELECT fnINDEX_ID FROM @tblTEMP1)
	END CATCH
END

