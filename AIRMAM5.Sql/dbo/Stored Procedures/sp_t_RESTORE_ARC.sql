

-- =============================================
-- 描述:	回復媒體檔案
-- 記錄:	<2012/05/02><Mihsiu.Chiu><新增本預存>
--			<2012/05/21><Dennis.Wen><一堆欄位調整>
--			<2012/09/24><Mihsiu.Chiu><修改預存 t_tbmARC_AUDIO_D新增& fnRESP_ID	fnRELA_ID	fnCHRO_ID	_sEXTRACT>
--			<2014/08/21><Eric.Huang><V/A/P/D 新增 fsKEYWORD/fsOTHINFO/fsOTHTYPE1/fsOTHTYPE2/fsOTHTYPE3>
--			<2014/08/21><Eric.Huang><V/A/P/D 新增 fsKEYWORD> ARC_VDO_D
--			<2019/07/01><David.Sin><增加判斷要還原的資料所屬的主題是否還在>
-- =============================================
CREATE PROCEDURE [dbo].[sp_t_RESTORE_ARC]
	@fnINDEX_ID	bigint,
	@fsRESTORED_BY nvarchar(50)	--執行復原人員
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY				
		
		--還原要注意tbmSUBJECT是否還存在，否則會變成孤兒
		IF ((SELECT COUNT(1) FROM [dbo].[tbmSUBJECT] WHERE [dbo].[tbmSUBJECT].[fsSUBJ_ID] IN
				(
					SELECT [dbo].[tbmSUBJECT].[fsSUBJ_ID] FROM [dbo].[t_tbmARC_VIDEO] WHERE [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] = (SELECT [dbo].[t_tbmARC_INDEX].[fsFILE_NO] FROM [dbo].[t_tbmARC_INDEX] WHERE [dbo].[t_tbmARC_INDEX].[fnINDEX_ID] = @fnINDEX_ID)
					UNION ALL
					SELECT [dbo].[tbmSUBJECT].[fsSUBJ_ID] FROM [dbo].[t_tbmARC_AUDIO] WHERE [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] = (SELECT [dbo].[t_tbmARC_INDEX].[fsFILE_NO] FROM [dbo].[t_tbmARC_INDEX] WHERE [dbo].[t_tbmARC_INDEX].[fnINDEX_ID] = @fnINDEX_ID)
					UNION ALL
					SELECT [dbo].[tbmSUBJECT].[fsSUBJ_ID] FROM [dbo].[t_tbmARC_PHOTO] WHERE [dbo].[t_tbmARC_PHOTO].[fsFILE_NO] = (SELECT [dbo].[t_tbmARC_INDEX].[fsFILE_NO] FROM [dbo].[t_tbmARC_INDEX] WHERE [dbo].[t_tbmARC_INDEX].[fnINDEX_ID] = @fnINDEX_ID)
					UNION ALL
					SELECT [dbo].[tbmSUBJECT].[fsSUBJ_ID] FROM [dbo].[t_tbmARC_DOC] WHERE [dbo].[t_tbmARC_DOC].[fsFILE_NO] = (SELECT [dbo].[t_tbmARC_INDEX].[fsFILE_NO] FROM [dbo].[t_tbmARC_INDEX] WHERE [dbo].[t_tbmARC_INDEX].[fnINDEX_ID] = @fnINDEX_ID)
				)
			) = 0)
		BEGIN
			SELECT RESULT = 'ERROR:所屬主題已不存在，無法還原!'
		END
		ELSE
		BEGIN
			DECLARE @fsTYPE CHAR(1) = (SELECT [dbo].[t_tbmARC_INDEX].[fsTYPE] FROM [dbo].[t_tbmARC_INDEX] WHERE [dbo].[t_tbmARC_INDEX].[fnINDEX_ID] = @fnINDEX_ID)

			--BEGIN TRANSACTION

			UPDATE 
				[dbo].[t_tbmARC_INDEX] 
			SET 
				[dbo].[t_tbmARC_INDEX].[fsSTATUS] = 'R',
				[dbo].[t_tbmARC_INDEX].[fsUPDATED_BY] = @fsRESTORED_BY,
				[dbo].[t_tbmARC_INDEX].[fdUPDATED_DATE] = GETDATE()
			WHERE 
				[dbo].[t_tbmARC_INDEX].[fnINDEX_ID] = @fnINDEX_ID
		
		
			IF(@fsTYPE = 'V')
			BEGIN

			
				INSERT INTO [dbo].[tbmARC_VIDEO]
				([dbo].[tbmARC_VIDEO].[fsFILE_NO], [dbo].[tbmARC_VIDEO].[fsTITLE],[dbo].[tbmARC_VIDEO].[fsDESCRIPTION], [dbo].[tbmARC_VIDEO].[fsSUBJECT_ID], [dbo].[tbmARC_VIDEO].[fsFILE_STATUS],[dbo].[tbmARC_VIDEO].[fsFILE_TYPE], 
		    				[dbo].[tbmARC_VIDEO].[fsFILE_TYPE_H], [dbo].[tbmARC_VIDEO].[fsFILE_TYPE_L], [dbo].[tbmARC_VIDEO].[fsFILE_SIZE], [dbo].[tbmARC_VIDEO].[fsFILE_SIZE_H], [dbo].[tbmARC_VIDEO].[fsFILE_SIZE_L], [dbo].[tbmARC_VIDEO].[fsFILE_PATH],
		    				[dbo].[tbmARC_VIDEO].[fsFILE_PATH_H], [dbo].[tbmARC_VIDEO].[fsFILE_PATH_L], [dbo].[tbmARC_VIDEO].[fxMEDIA_INFO],[dbo].[tbmARC_VIDEO].[fsHEAD_FRAME], [dbo].[tbmARC_VIDEO].[fdBEG_TIME], [dbo].[tbmARC_VIDEO].[fdEND_TIME],
							[dbo].[tbmARC_VIDEO].[fdDURATION],[dbo].[tbmARC_VIDEO].[fsRESOL_TAG],[dbo].[tbmARC_VIDEO].[fdCREATED_DATE], [dbo].[tbmARC_VIDEO].[fsCREATED_BY], [dbo].[tbmARC_VIDEO].[fdUPDATED_DATE], [dbo].[tbmARC_VIDEO].[fsUPDATED_BY],
							[dbo].[tbmARC_VIDEO].[fsATTRIBUTE1], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE2], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE3], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE4], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE5], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE6], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE7], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE8], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE9], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE10], 
		    				[dbo].[tbmARC_VIDEO].[fsATTRIBUTE11], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE12], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE13], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE14], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE15], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE16], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE17], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE18], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE19], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE20],
		    				[dbo].[tbmARC_VIDEO].[fsATTRIBUTE21], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE22], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE23], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE24], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE25], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE26], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE27], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE28], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE29], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE30], 
		    				[dbo].[tbmARC_VIDEO].[fsATTRIBUTE31], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE32], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE33], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE34], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE35], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE36], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE37], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE38], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE39], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE40], 
		    				[dbo].[tbmARC_VIDEO].[fsATTRIBUTE41], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE42], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE43], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE44], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE45], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE46], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE47], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE48], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE49], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE50], 
							[dbo].[tbmARC_VIDEO].[fsATTRIBUTE51], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE52], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE53], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE54], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE55], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE56], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE57], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE58], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE59], [dbo].[tbmARC_VIDEO].[fsATTRIBUTE60])	-- 2014/08/21 ERIC
				SELECT [dbo].[t_tbmARC_VIDEO].[fsFILE_NO], [dbo].[t_tbmARC_VIDEO].[fsTITLE],[dbo].[t_tbmARC_VIDEO].[fsDESCRIPTION], [dbo].[t_tbmARC_VIDEO].[fsSUBJECT_ID], [dbo].[t_tbmARC_VIDEO].[fsFILE_STATUS],[dbo].[t_tbmARC_VIDEO].[fsFILE_TYPE], 
		    				[dbo].[t_tbmARC_VIDEO].[fsFILE_TYPE_H], [dbo].[t_tbmARC_VIDEO].[fsFILE_TYPE_L], [dbo].[t_tbmARC_VIDEO].[fsFILE_SIZE], [dbo].[t_tbmARC_VIDEO].[fsFILE_SIZE_H], [dbo].[t_tbmARC_VIDEO].[fsFILE_SIZE_L], [dbo].[t_tbmARC_VIDEO].[fsFILE_PATH],
		    				[dbo].[t_tbmARC_VIDEO].[fsFILE_PATH_H], [dbo].[t_tbmARC_VIDEO].[fsFILE_PATH_L], [dbo].[t_tbmARC_VIDEO].[fxMEDIA_INFO],[dbo].[t_tbmARC_VIDEO].[fsHEAD_FRAME], [dbo].[t_tbmARC_VIDEO].[fdBEG_TIME], [dbo].[t_tbmARC_VIDEO].[fdEND_TIME],
							[dbo].[t_tbmARC_VIDEO].[fdDURATION],[dbo].[t_tbmARC_VIDEO].[fsRESOL_TAG],[dbo].[t_tbmARC_VIDEO].[fdCREATED_DATE], [dbo].[t_tbmARC_VIDEO].[fsCREATED_BY], [dbo].[t_tbmARC_VIDEO].[fdUPDATED_DATE], [dbo].[t_tbmARC_VIDEO].[fsUPDATED_BY],
							[dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE1], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE2], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE3], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE4], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE5], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE6], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE7], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE8], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE9], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE10], 
		    				[dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE11], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE12], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE13], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE14], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE15], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE16], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE17], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE18], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE19], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE20],
		    				[dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE21], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE22], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE23], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE24], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE25], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE26], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE27], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE28], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE29], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE30], 
		    				[dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE31], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE32], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE33], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE34], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE35], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE36], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE37], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE38], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE39], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE40], 
		    				[dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE41], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE42], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE43], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE44], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE45], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE46], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE47], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE48], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE49], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE50], 
							[dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE51], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE52], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE53], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE54], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE55], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE56], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE57], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE58], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE59], [dbo].[t_tbmARC_VIDEO].[fsATTRIBUTE60]
				FROM [dbo].[t_tbmARC_VIDEO]
				WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID

				INSERT INTO [dbo].[tbmARC_VIDEO_D]
					([dbo].[tbmARC_VIDEO_D].[fsFILE_NO], [dbo].[tbmARC_VIDEO_D].[fnSEQ_NO], [dbo].[tbmARC_VIDEO_D].[fsDESCRIPTION], [dbo].[tbmARC_VIDEO_D].[fdBEG_TIME], [dbo].[tbmARC_VIDEO_D].[fdEND_TIME],[dbo].[tbmARC_VIDEO_D].[fdCREATED_DATE], [dbo].[tbmARC_VIDEO_D].[fsCREATED_BY], [dbo].[tbmARC_VIDEO_D].[fdUPDATED_DATE], [dbo].[tbmARC_VIDEO_D].[fsUPDATED_BY])
				SELECT [dbo].[t_tbmARC_VIDEO_D].[fsFILE_NO], 
						[dbo].[t_tbmARC_VIDEO_D].[fnSEQ_NO], 
						[dbo].[t_tbmARC_VIDEO_D].[fsDESCRIPTION], 
						[dbo].[t_tbmARC_VIDEO_D].[fdBEG_TIME], 
						[dbo].[t_tbmARC_VIDEO_D].[fdEND_TIME],
						[dbo].[t_tbmARC_VIDEO_D].[fdCREATED_DATE], 
						[dbo].[t_tbmARC_VIDEO_D].[fsCREATED_BY], 
						[dbo].[t_tbmARC_VIDEO_D].[fdUPDATED_DATE], 
						[dbo].[t_tbmARC_VIDEO_D].[fsUPDATED_BY]
				FROM [dbo].[t_tbmARC_VIDEO_D] JOIN [dbo].[t_tbmARC_VIDEO] ON [dbo].[t_tbmARC_VIDEO_D].[fsFILE_NO] = [dbo].[t_tbmARC_VIDEO].[fsFILE_NO]
				WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID
		
		
				INSERT INTO	[dbo].[tbmARC_VIDEO_K]
				(
					[dbo].[tbmARC_VIDEO_K].[fsFILE_NO], [dbo].[tbmARC_VIDEO_K].[fsTITLE], [dbo].[tbmARC_VIDEO_K].[fsDESCRIPTION], [dbo].[tbmARC_VIDEO_K].[fsFILE_PATH], [dbo].[tbmARC_VIDEO_K].[fsFILE_SIZE], [dbo].[tbmARC_VIDEO_K].[fsFILE_TYPE], 
		    		[dbo].[tbmARC_VIDEO_K].[fdCREATED_DATE], [dbo].[tbmARC_VIDEO_K].[fsCREATED_BY], [dbo].[tbmARC_VIDEO_K].[fdUPDATED_DATE], [dbo].[tbmARC_VIDEO_K].[fsUPDATED_BY], [dbo].[tbmARC_VIDEO_K].[fsTIME]
				)
				SELECT		[dbo].[t_tbmARC_VIDEO_K].[fsFILE_NO], 
							[dbo].[t_tbmARC_VIDEO_K].[fsTITLE], 
							[dbo].[t_tbmARC_VIDEO_K].[fsDESCRIPTION], 
							[dbo].[t_tbmARC_VIDEO_K].[fsFILE_PATH], 
							[dbo].[t_tbmARC_VIDEO_K].[fsFILE_SIZE], 
							[dbo].[t_tbmARC_VIDEO_K].[fsFILE_TYPE], 
		    				[dbo].[t_tbmARC_VIDEO_K].[fdCREATED_DATE], 
							[dbo].[t_tbmARC_VIDEO_K].[fsCREATED_BY], 
							[dbo].[t_tbmARC_VIDEO_K].[fdUPDATED_DATE], 
							[dbo].[t_tbmARC_VIDEO_K].[fsUPDATED_BY], 
		    				[dbo].[t_tbmARC_VIDEO_K].[fsTIME]
				FROM [dbo].[t_tbmARC_VIDEO_K] JOIN [dbo].[t_tbmARC_VIDEO] ON [dbo].[t_tbmARC_VIDEO_K].[fsFILE_NO] = [dbo].[t_tbmARC_VIDEO].[fsFILE_NO]
				WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID
		
				DELETE FROM [dbo].[t_tbmARC_VIDEO_D] WHERE [dbo].[t_tbmARC_VIDEO_D].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] FROM [dbo].[t_tbmARC_VIDEO] WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID)
				DELETE FROM [dbo].[t_tbmARC_VIDEO_K] WHERE [dbo].[t_tbmARC_VIDEO_K].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] FROM [dbo].[t_tbmARC_VIDEO] WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID)
				DELETE FROM [dbo].[t_tbmARC_VIDEO] WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID
			END
			ELSE IF(@fsTYPE = 'A')
			BEGIN
				INSERT INTO [dbo].[tbmARC_AUDIO]
				(
					[dbo].[tbmARC_AUDIO].[fsFILE_NO],[dbo].[tbmARC_AUDIO].[fsTITLE],[dbo].[tbmARC_AUDIO].[fsDESCRIPTION], [dbo].[tbmARC_AUDIO].[fsSUBJECT_ID], [dbo].[tbmARC_AUDIO].[fsFILE_STATUS], [dbo].[tbmARC_AUDIO].[fsFILE_TYPE], [dbo].[tbmARC_AUDIO].[fsFILE_TYPE_H], [dbo].[tbmARC_AUDIO].[fsFILE_TYPE_L], 
		    		[dbo].[tbmARC_AUDIO].[fsFILE_SIZE], [dbo].[tbmARC_AUDIO].[fsFILE_SIZE_H], [dbo].[tbmARC_AUDIO].[fsFILE_SIZE_L], [dbo].[tbmARC_AUDIO].[fsFILE_PATH], [dbo].[tbmARC_AUDIO].[fsFILE_PATH_H], [dbo].[tbmARC_AUDIO].[fsFILE_PATH_L], [dbo].[tbmARC_AUDIO].[fxMEDIA_INFO], [dbo].[tbmARC_AUDIO].[fdBEG_TIME], [dbo].[tbmARC_AUDIO].[fdEND_TIME], [dbo].[tbmARC_AUDIO].[fdDURATION],
		    		[dbo].[tbmARC_AUDIO].[fsATTRIBUTE1], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE2], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE3], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE4], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE5], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE6], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE7], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE8], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE9], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE10], 
		    		[dbo].[tbmARC_AUDIO].[fsATTRIBUTE11], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE12], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE13], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE14], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE15], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE16], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE17], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE18], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE19], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE20],
		    		[dbo].[tbmARC_AUDIO].[fsATTRIBUTE21], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE22], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE23], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE24], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE25], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE26], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE27], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE28], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE29], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE30], 
		    		[dbo].[tbmARC_AUDIO].[fsATTRIBUTE31], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE32], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE33], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE34], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE35], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE36], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE37], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE38], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE39], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE40], 
		    		[dbo].[tbmARC_AUDIO].[fsATTRIBUTE41], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE42], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE43], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE44], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE45], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE46], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE47], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE48], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE49], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE50], 					
		    		[dbo].[tbmARC_AUDIO].[fsATTRIBUTE51], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE52], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE53], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE54], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE55], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE56], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE57], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE58], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE59], [dbo].[tbmARC_AUDIO].[fsATTRIBUTE60],
					[dbo].[tbmARC_AUDIO].[fdCREATED_DATE], [dbo].[tbmARC_AUDIO].[fsCREATED_BY], [dbo].[tbmARC_AUDIO].[fdUPDATED_DATE], [dbo].[tbmARC_AUDIO].[fsUPDATED_BY]
				)
				SELECT 
					[dbo].[t_tbmARC_AUDIO].[fsFILE_NO],[dbo].[t_tbmARC_AUDIO].[fsTITLE],[dbo].[t_tbmARC_AUDIO].[fsDESCRIPTION], [dbo].[t_tbmARC_AUDIO].[fsSUBJECT_ID], [dbo].[t_tbmARC_AUDIO].[fsFILE_STATUS], [dbo].[t_tbmARC_AUDIO].[fsFILE_TYPE], [dbo].[t_tbmARC_AUDIO].[fsFILE_TYPE_H], [dbo].[t_tbmARC_AUDIO].[fsFILE_TYPE_L], 
		    		[dbo].[t_tbmARC_AUDIO].[fsFILE_SIZE], [dbo].[t_tbmARC_AUDIO].[fsFILE_SIZE_H], [dbo].[t_tbmARC_AUDIO].[fsFILE_SIZE_L], [dbo].[t_tbmARC_AUDIO].[fsFILE_PATH], [dbo].[t_tbmARC_AUDIO].[fsFILE_PATH_H], [dbo].[t_tbmARC_AUDIO].[fsFILE_PATH_L], [dbo].[t_tbmARC_AUDIO].[fxMEDIA_INFO], [dbo].[t_tbmARC_AUDIO].[fdBEG_TIME], [dbo].[t_tbmARC_AUDIO].[fdEND_TIME], [dbo].[t_tbmARC_AUDIO].[fdDURATION],
		    		[dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE1], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE2], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE3], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE4], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE5], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE6], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE7], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE8], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE9], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE10], 
		    		[dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE11], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE12], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE13], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE14], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE15], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE16], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE17], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE18], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE19], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE20],
		    		[dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE21], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE22], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE23], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE24], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE25], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE26], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE27], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE28], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE29], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE30], 
		    		[dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE31], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE32], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE33], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE34], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE35], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE36], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE37], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE38], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE39], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE40], 
		    		[dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE41], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE42], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE43], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE44], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE45], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE46], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE47], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE48], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE49], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE50], 					
		    		[dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE51], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE52], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE53], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE54], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE55], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE56], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE57], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE58], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE59], [dbo].[t_tbmARC_AUDIO].[fsATTRIBUTE60],
					[dbo].[t_tbmARC_AUDIO].[fdCREATED_DATE], [dbo].[t_tbmARC_AUDIO].[fsCREATED_BY], [dbo].[t_tbmARC_AUDIO].[fdUPDATED_DATE], [dbo].[t_tbmARC_AUDIO].[fsUPDATED_BY]
				FROM [dbo].[t_tbmARC_AUDIO]
				WHERE [dbo].[t_tbmARC_AUDIO].[fnINDEX] = @fnINDEX_ID 
				
				INSERT INTO [dbo].[tbmARC_AUDIO_D]
					([dbo].[tbmARC_AUDIO_D].[fsFILE_NO], [dbo].[tbmARC_AUDIO_D].[fnSEQ_NO], [dbo].[tbmARC_AUDIO_D].[fsDESCRIPTION], [dbo].[tbmARC_AUDIO_D].[fdBEG_TIME], [dbo].[tbmARC_AUDIO_D].[fdEND_TIME],[dbo].[tbmARC_AUDIO_D].[fdCREATED_DATE], [dbo].[tbmARC_AUDIO_D].[fsCREATED_BY], [dbo].[tbmARC_AUDIO_D].[fdUPDATED_DATE], [dbo].[tbmARC_AUDIO_D].[fsUPDATED_BY])
				SELECT [dbo].[t_tbmARC_AUDIO_D].[fsFILE_NO], 
						[dbo].[t_tbmARC_AUDIO_D].[fnSEQ_NO], 
						[dbo].[t_tbmARC_AUDIO_D].[fsDESCRIPTION], 
						[dbo].[t_tbmARC_AUDIO_D].[fdBEG_TIME], 
						[dbo].[t_tbmARC_AUDIO_D].[fdEND_TIME],
						[dbo].[t_tbmARC_AUDIO_D].[fdCREATED_DATE], 
						[dbo].[t_tbmARC_AUDIO_D].[fsCREATED_BY], 
						[dbo].[t_tbmARC_AUDIO_D].[fdUPDATED_DATE], 
						[dbo].[t_tbmARC_AUDIO_D].[fsUPDATED_BY]
				FROM [dbo].[t_tbmARC_AUDIO_D] JOIN [dbo].[t_tbmARC_AUDIO] ON [dbo].[t_tbmARC_AUDIO_D].[fsFILE_NO] = [dbo].[t_tbmARC_AUDIO].[fsFILE_NO]
				WHERE [dbo].[t_tbmARC_AUDIO].[fnINDEX] = @fnINDEX_ID

				DELETE FROM [dbo].[t_tbmARC_AUDIO_D] WHERE [dbo].[t_tbmARC_AUDIO_D].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] FROM [dbo].[t_tbmARC_AUDIO] WHERE [dbo].[t_tbmARC_AUDIO].[fnINDEX] = @fnINDEX_ID)
				DELETE FROM [dbo].[t_tbmARC_AUDIO] WHERE [dbo].[t_tbmARC_AUDIO].[fnINDEX] = @fnINDEX_ID
				
			END
			ELSE IF(@fsTYPE = 'P')
			BEGIN
				INSERT INTO [dbo].[tbmARC_PHOTO]
				(
					[dbo].[tbmARC_PHOTO].[fsFILE_NO],[dbo].[tbmARC_PHOTO].[fsTITLE], [dbo].[tbmARC_PHOTO].[fsDESCRIPTION],[dbo].[tbmARC_PHOTO].[fsSUBJECT_ID], [dbo].[tbmARC_PHOTO].[fsFILE_STATUS], [dbo].[tbmARC_PHOTO].[fsFILE_TYPE], [dbo].[tbmARC_PHOTO].[fsFILE_TYPE_H], [dbo].[tbmARC_PHOTO].[fsFILE_TYPE_L], 
		    		[dbo].[tbmARC_PHOTO].[fsFILE_SIZE], [dbo].[tbmARC_PHOTO].[fsFILE_SIZE_H], [dbo].[tbmARC_PHOTO].[fsFILE_SIZE_L], [dbo].[tbmARC_PHOTO].[fsFILE_PATH], [dbo].[tbmARC_PHOTO].[fsFILE_PATH_H], [dbo].[tbmARC_PHOTO].[fsFILE_PATH_L], [dbo].[tbmARC_PHOTO].[fxMEDIA_INFO], [dbo].[tbmARC_PHOTO].[fnWIDTH], [dbo].[tbmARC_PHOTO].[fnHEIGHT], [dbo].[tbmARC_PHOTO].[fnXDPI], 
					[dbo].[tbmARC_PHOTO].[fnYDPI],[dbo].[tbmARC_PHOTO].[fsCAMERA_MAKE],[dbo].[tbmARC_PHOTO].[fsCAMERA_MODEL],[dbo].[tbmARC_PHOTO].[fsFOCAL_LENGTH],[dbo].[tbmARC_PHOTO].[fsEXPOSURE_TIME],[dbo].[tbmARC_PHOTO].[fsAPERTURE],[dbo].[tbmARC_PHOTO].[fnISO],
		    		[dbo].[tbmARC_PHOTO].[fsATTRIBUTE1], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE2], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE3], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE4], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE5], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE6], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE7], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE8], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE9], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE10], 
		    		[dbo].[tbmARC_PHOTO].[fsATTRIBUTE11], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE12], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE13], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE14], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE15], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE16], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE17], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE18], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE19], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE20],
		    		[dbo].[tbmARC_PHOTO].[fsATTRIBUTE21], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE22], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE23], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE24], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE25], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE26], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE27], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE28], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE29], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE30], 
		    		[dbo].[tbmARC_PHOTO].[fsATTRIBUTE31], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE32], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE33], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE34], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE35], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE36], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE37], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE38], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE39], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE40], 
		    		[dbo].[tbmARC_PHOTO].[fsATTRIBUTE41], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE42], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE43], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE44], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE45], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE46], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE47], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE48], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE49], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE50], 
					[dbo].[tbmARC_PHOTO].[fsATTRIBUTE51], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE52], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE53], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE54], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE55], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE56], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE57], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE58], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE59], [dbo].[tbmARC_PHOTO].[fsATTRIBUTE60], 
		    		[dbo].[tbmARC_PHOTO].[fdCREATED_DATE], [dbo].[tbmARC_PHOTO].[fsCREATED_BY], [dbo].[tbmARC_PHOTO].[fdUPDATED_DATE], [dbo].[tbmARC_PHOTO].[fsUPDATED_BY]
				)
				SELECT 
					[dbo].[t_tbmARC_PHOTO].[fsFILE_NO],[dbo].[t_tbmARC_PHOTO].[fsTITLE], [dbo].[t_tbmARC_PHOTO].[fsDESCRIPTION],[dbo].[t_tbmARC_PHOTO].[fsSUBJECT_ID], [dbo].[t_tbmARC_PHOTO].[fsFILE_STATUS], [dbo].[t_tbmARC_PHOTO].[fsFILE_TYPE], [dbo].[t_tbmARC_PHOTO].[fsFILE_TYPE_H], [dbo].[t_tbmARC_PHOTO].[fsFILE_TYPE_L], 
		    		[dbo].[t_tbmARC_PHOTO].[fsFILE_SIZE], [dbo].[t_tbmARC_PHOTO].[fsFILE_SIZE_H], [dbo].[t_tbmARC_PHOTO].[fsFILE_SIZE_L], [dbo].[t_tbmARC_PHOTO].[fsFILE_PATH], [dbo].[t_tbmARC_PHOTO].[fsFILE_PATH_H], [dbo].[t_tbmARC_PHOTO].[fsFILE_PATH_L], [dbo].[t_tbmARC_PHOTO].[fxMEDIA_INFO], [dbo].[t_tbmARC_PHOTO].[fnWIDTH], [dbo].[t_tbmARC_PHOTO].[fnHEIGHT], [dbo].[t_tbmARC_PHOTO].[fnXDPI], 
					[dbo].[t_tbmARC_PHOTO].[fnYDPI],[dbo].[t_tbmARC_PHOTO].[fsCAMERA_MAKE],[dbo].[t_tbmARC_PHOTO].[fsCAMERA_MODEL],[dbo].[t_tbmARC_PHOTO].[fsFOCAL_LENGTH],[dbo].[t_tbmARC_PHOTO].[fsEXPOSURE_TIME],[dbo].[t_tbmARC_PHOTO].[fsAPERTURE],[dbo].[t_tbmARC_PHOTO].[fnISO],
		    		[dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE1], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE2], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE3], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE4], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE5], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE6], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE7], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE8], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE9], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE10], 
		    		[dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE11], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE12], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE13], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE14], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE15], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE16], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE17], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE18], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE19], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE20],
		    		[dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE21], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE22], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE23], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE24], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE25], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE26], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE27], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE28], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE29], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE30], 
		    		[dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE31], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE32], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE33], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE34], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE35], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE36], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE37], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE38], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE39], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE40], 
		    		[dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE41], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE42], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE43], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE44], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE45], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE46], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE47], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE48], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE49], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE50], 
					[dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE51], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE52], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE53], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE54], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE55], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE56], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE57], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE58], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE59], [dbo].[t_tbmARC_PHOTO].[fsATTRIBUTE60], 
		    		[dbo].[t_tbmARC_PHOTO].[fdCREATED_DATE], [dbo].[t_tbmARC_PHOTO].[fsCREATED_BY], [dbo].[t_tbmARC_PHOTO].[fdUPDATED_DATE], [dbo].[t_tbmARC_PHOTO].[fsUPDATED_BY]
				FROM [dbo].[t_tbmARC_PHOTO]
				WHERE [dbo].[t_tbmARC_PHOTO].[fnINDEX] = @fnINDEX_ID 
		
				DELETE FROM [dbo].[t_tbmARC_PHOTO] WHERE [dbo].[t_tbmARC_PHOTO].[fnINDEX] = @fnINDEX_ID
			END
			ELSE IF(@fsTYPE = 'D')
			BEGIN
			
				INSERT INTO [dbo].[tbmARC_DOC]
				(
					[dbo].[tbmARC_DOC].[fsFILE_NO],[dbo].[tbmARC_DOC].[fsTITLE],[dbo].[tbmARC_DOC].[fsDESCRIPTION],[dbo].[tbmARC_DOC].[fsSUBJECT_ID], [dbo].[tbmARC_DOC].[fsFILE_STATUS], [dbo].[tbmARC_DOC].[fsFILE_TYPE],
		    				[dbo].[tbmARC_DOC].[fsFILE_SIZE], [dbo].[tbmARC_DOC].[fsFILE_PATH], [dbo].[tbmARC_DOC].[fxMEDIA_INFO], [dbo].[tbmARC_DOC].[fsCONTENT], [dbo].[tbmARC_DOC].[fdFILE_CREATED_DATE], [dbo].[tbmARC_DOC].[fdFILE_UPDATED_DATE], 
		    				[dbo].[tbmARC_DOC].[fsATTRIBUTE1], [dbo].[tbmARC_DOC].[fsATTRIBUTE2], [dbo].[tbmARC_DOC].[fsATTRIBUTE3], [dbo].[tbmARC_DOC].[fsATTRIBUTE4], [dbo].[tbmARC_DOC].[fsATTRIBUTE5], [dbo].[tbmARC_DOC].[fsATTRIBUTE6], [dbo].[tbmARC_DOC].[fsATTRIBUTE7], [dbo].[tbmARC_DOC].[fsATTRIBUTE8], [dbo].[tbmARC_DOC].[fsATTRIBUTE9], [dbo].[tbmARC_DOC].[fsATTRIBUTE10], 
		    				[dbo].[tbmARC_DOC].[fsATTRIBUTE11], [dbo].[tbmARC_DOC].[fsATTRIBUTE12], [dbo].[tbmARC_DOC].[fsATTRIBUTE13], [dbo].[tbmARC_DOC].[fsATTRIBUTE14], [dbo].[tbmARC_DOC].[fsATTRIBUTE15], [dbo].[tbmARC_DOC].[fsATTRIBUTE16], [dbo].[tbmARC_DOC].[fsATTRIBUTE17], [dbo].[tbmARC_DOC].[fsATTRIBUTE18], [dbo].[tbmARC_DOC].[fsATTRIBUTE19], [dbo].[tbmARC_DOC].[fsATTRIBUTE20],
		    				[dbo].[tbmARC_DOC].[fsATTRIBUTE21], [dbo].[tbmARC_DOC].[fsATTRIBUTE22], [dbo].[tbmARC_DOC].[fsATTRIBUTE23], [dbo].[tbmARC_DOC].[fsATTRIBUTE24], [dbo].[tbmARC_DOC].[fsATTRIBUTE25], [dbo].[tbmARC_DOC].[fsATTRIBUTE26], [dbo].[tbmARC_DOC].[fsATTRIBUTE27], [dbo].[tbmARC_DOC].[fsATTRIBUTE28], [dbo].[tbmARC_DOC].[fsATTRIBUTE29], [dbo].[tbmARC_DOC].[fsATTRIBUTE30], 
		    				[dbo].[tbmARC_DOC].[fsATTRIBUTE31], [dbo].[tbmARC_DOC].[fsATTRIBUTE32], [dbo].[tbmARC_DOC].[fsATTRIBUTE33], [dbo].[tbmARC_DOC].[fsATTRIBUTE34], [dbo].[tbmARC_DOC].[fsATTRIBUTE35], [dbo].[tbmARC_DOC].[fsATTRIBUTE36], [dbo].[tbmARC_DOC].[fsATTRIBUTE37], [dbo].[tbmARC_DOC].[fsATTRIBUTE38], [dbo].[tbmARC_DOC].[fsATTRIBUTE39], [dbo].[tbmARC_DOC].[fsATTRIBUTE40], 
		    				[dbo].[tbmARC_DOC].[fsATTRIBUTE41], [dbo].[tbmARC_DOC].[fsATTRIBUTE42], [dbo].[tbmARC_DOC].[fsATTRIBUTE43], [dbo].[tbmARC_DOC].[fsATTRIBUTE44], [dbo].[tbmARC_DOC].[fsATTRIBUTE45], [dbo].[tbmARC_DOC].[fsATTRIBUTE46], [dbo].[tbmARC_DOC].[fsATTRIBUTE47], [dbo].[tbmARC_DOC].[fsATTRIBUTE48], [dbo].[tbmARC_DOC].[fsATTRIBUTE49], [dbo].[tbmARC_DOC].[fsATTRIBUTE50], 
		    				[dbo].[tbmARC_DOC].[fsATTRIBUTE51], [dbo].[tbmARC_DOC].[fsATTRIBUTE52], [dbo].[tbmARC_DOC].[fsATTRIBUTE53], [dbo].[tbmARC_DOC].[fsATTRIBUTE54], [dbo].[tbmARC_DOC].[fsATTRIBUTE55], [dbo].[tbmARC_DOC].[fsATTRIBUTE56], [dbo].[tbmARC_DOC].[fsATTRIBUTE57], [dbo].[tbmARC_DOC].[fsATTRIBUTE58], [dbo].[tbmARC_DOC].[fsATTRIBUTE59], [dbo].[tbmARC_DOC].[fsATTRIBUTE60],
							[dbo].[tbmARC_DOC].[fdCREATED_DATE], [dbo].[tbmARC_DOC].[fsCREATED_BY], [dbo].[tbmARC_DOC].[fdUPDATED_DATE], [dbo].[tbmARC_DOC].[fsUPDATED_BY]
				)
				SELECT [dbo].[t_tbmARC_DOC].[fsFILE_NO],[dbo].[t_tbmARC_DOC].[fsTITLE],[dbo].[t_tbmARC_DOC].[fsDESCRIPTION],[dbo].[t_tbmARC_DOC].[fsSUBJECT_ID], [dbo].[t_tbmARC_DOC].[fsFILE_STATUS], [dbo].[t_tbmARC_DOC].[fsFILE_TYPE], 
		    				[dbo].[t_tbmARC_DOC].[fsFILE_SIZE], [dbo].[t_tbmARC_DOC].[fsFILE_PATH], [dbo].[t_tbmARC_DOC].[fxMEDIA_INFO], [dbo].[t_tbmARC_DOC].[fsCONTENT], [dbo].[t_tbmARC_DOC].[fdFILE_CREATED_DATE], [dbo].[t_tbmARC_DOC].[fdFILE_UPDATED_DATE], 
		    				[dbo].[t_tbmARC_DOC].[fsATTRIBUTE1], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE2], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE3], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE4], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE5], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE6], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE7], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE8], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE9], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE10], 
		    				[dbo].[t_tbmARC_DOC].[fsATTRIBUTE11], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE12], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE13], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE14], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE15], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE16], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE17], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE18], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE19], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE20],
		    				[dbo].[t_tbmARC_DOC].[fsATTRIBUTE21], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE22], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE23], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE24], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE25], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE26], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE27], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE28], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE29], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE30], 
		    				[dbo].[t_tbmARC_DOC].[fsATTRIBUTE31], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE32], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE33], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE34], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE35], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE36], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE37], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE38], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE39], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE40], 
		    				[dbo].[t_tbmARC_DOC].[fsATTRIBUTE41], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE42], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE43], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE44], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE45], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE46], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE47], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE48], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE49], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE50], 
		    				[dbo].[t_tbmARC_DOC].[fsATTRIBUTE51], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE52], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE53], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE54], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE55], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE56], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE57], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE58], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE59], [dbo].[t_tbmARC_DOC].[fsATTRIBUTE60],
							[dbo].[t_tbmARC_DOC].[fdCREATED_DATE], [dbo].[t_tbmARC_DOC].[fsCREATED_BY], [dbo].[t_tbmARC_DOC].[fdUPDATED_DATE], [dbo].[t_tbmARC_DOC].[fsUPDATED_BY]
				 FROM [dbo].[t_tbmARC_DOC]
				 WHERE [dbo].[t_tbmARC_DOC].[fnINDEX] = @fnINDEX_ID
		
				DELETE FROM [dbo].[t_tbmARC_DOC] WHERE [dbo].[t_tbmARC_DOC].[fnINDEX] = @fnINDEX_ID
			END
		
			--COMMIT

			SELECT RESULT = ''
		END
		
	END TRY
	BEGIN CATCH
		--ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

