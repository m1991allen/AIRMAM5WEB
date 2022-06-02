

-- =============================================
-- 描述:	刪除媒體檔案
-- 記錄:	<2012/05/02><Mihsiu.Chiu><新增本預存>
--			<2012/05/14><Mihsiu.Chiu><"刪暫存資料"的動作先mark >
--			<2012/09/24><Mihsiu.Chiu><處理fnRESP_ID & fnCHRO_ID>
--
-- =============================================
CREATE PROCEDURE [dbo].[sp_t_DELETE_ARC]
	@fnINDEX_ID	bigint,
	@fsDELETED_BY nvarchar(50)	--刪除人員
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY				
		/*1.修改t_tbmARC_INDEX的狀態"D"(Deleted)*/
		UPDATE 
			[dbo].[t_tbmARC_INDEX] 
		SET 
			[dbo].[t_tbmARC_INDEX].[fsSTATUS] = 'D', 
			[dbo].[t_tbmARC_INDEX].[fsUPDATED_BY] = @fsDELETED_BY, 
			[dbo].[t_tbmARC_INDEX].[fdUPDATED_DATE] = GETDATE()
		WHERE 
			[dbo].[t_tbmARC_INDEX].[fnINDEX_ID] = @fnINDEX_ID
		
		/*2.把暫存資料表中所有fnINDEX_ID=@fnINDEX_ID的資料刪除掉*/	
		DECLARE @fsTYPE VARCHAR(1) = (SELECT [dbo].[t_tbmARC_INDEX].[fsTYPE] FROM [dbo].[t_tbmARC_INDEX] WHERE [dbo].[t_tbmARC_INDEX].[fnINDEX_ID] = @fnINDEX_ID)
		
		IF (@fsTYPE = 'V')
		BEGIN

			DELETE FROM [dbo].[t_tbmARC_VIDEO_D] WHERE [dbo].[t_tbmARC_VIDEO_D].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] FROM [dbo].[t_tbmARC_VIDEO] WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID)

			DELETE FROM [dbo].[t_tbmARC_VIDEO_K] WHERE [dbo].[t_tbmARC_VIDEO_K].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] FROM [dbo].[t_tbmARC_VIDEO] WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID)

			DELETE FROM [dbo].[tbdARC_VIDEO_ATTR] WHERE [dbo].[tbdARC_VIDEO_ATTR].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] FROM [dbo].[t_tbmARC_VIDEO] WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID)

			DELETE FROM [dbo].[t_tbmARC_VIDEO] WHERE [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] FROM [dbo].[t_tbmARC_VIDEO] WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID)

		END
		ELSE IF (@fsTYPE = 'A')
		BEGIN
			
			DELETE FROM [dbo].[tbdARC_AUDIO_ATTR] WHERE [dbo].[tbdARC_AUDIO_ATTR].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] FROM [dbo].[t_tbmARC_AUDIO] WHERE [dbo].[t_tbmARC_AUDIO].[fnINDEX] = @fnINDEX_ID)

			DELETE FROM [dbo].[t_tbmARC_AUDIO_D] WHERE [dbo].[t_tbmARC_AUDIO_D].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] FROM [dbo].[t_tbmARC_AUDIO] WHERE [dbo].[t_tbmARC_AUDIO].[fnINDEX] = @fnINDEX_ID)

			DELETE FROM [dbo].[t_tbmARC_AUDIO] WHERE [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] FROM [dbo].[t_tbmARC_AUDIO] WHERE [dbo].[t_tbmARC_AUDIO].[fnINDEX] = @fnINDEX_ID)

		END
		ELSE IF (@fsTYPE = 'P')
		BEGIN
			
			DELETE FROM [dbo].[tbdARC_PHOTO_ATTR] WHERE [dbo].[tbdARC_PHOTO_ATTR].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_PHOTO].[fsFILE_NO] FROM [dbo].[t_tbmARC_PHOTO] WHERE [dbo].[t_tbmARC_PHOTO].[fnINDEX] = @fnINDEX_ID)

			DELETE FROM [dbo].[t_tbmARC_PHOTO] WHERE [dbo].[t_tbmARC_PHOTO].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_PHOTO].[fsFILE_NO] FROM [dbo].[t_tbmARC_PHOTO] WHERE [dbo].[t_tbmARC_PHOTO].[fnINDEX] = @fnINDEX_ID)

		END
		ELSE IF (@fsTYPE = 'D')
		BEGIN
			
			DELETE FROM [dbo].[tbdARC_DOC_ATTR] WHERE [dbo].[tbdARC_DOC_ATTR].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_DOC].[fsFILE_NO] FROM [dbo].[t_tbmARC_DOC] WHERE [dbo].[t_tbmARC_DOC].[fnINDEX] = @fnINDEX_ID)

			DELETE FROM [dbo].[t_tbmARC_DOC] WHERE [dbo].[t_tbmARC_DOC].[fsFILE_NO] IN (SELECT [dbo].[t_tbmARC_DOC].[fsFILE_NO] FROM [dbo].[t_tbmARC_DOC] WHERE [dbo].[t_tbmARC_DOC].[fnINDEX] = @fnINDEX_ID)

		END


		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'刪除媒體檔案', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'sp_t_DELETE_ARC';

