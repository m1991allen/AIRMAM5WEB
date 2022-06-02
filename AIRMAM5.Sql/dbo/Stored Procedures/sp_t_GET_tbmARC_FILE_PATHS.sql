

-- =============================================
-- 描述:	取出影音圖文的路徑檔名
-- 記錄:	<2019/08/06><David.Sin><重構本預存>
-- =============================================
CREATE PROCEDURE [dbo].[sp_t_GET_tbmARC_FILE_PATHS]
	@fnINDEX_ID	bigint,
	@fsTYPE		varchar(1),
	@fsFILE_NO	varchar(16)	
AS
BEGIN
 	SET NOCOUNT ON;

	----- 2012/11/12 eric start -----				
	DECLARE @FILE_PATH_PT NVARCHAR(100) = ISNULL((SELECT TOP 1 [dbo].[tbzCONFIG].[fsVALUE] FROM [dbo].[tbzCONFIG] WHERE ([dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_FOLDER_P')),'') + 'thumbnail\';		 
	----- 2012/11/12 eric end -----				
	
	BEGIN TRY

		IF (@fsTYPE = 'V')
		BEGIN
			SELECT [dbo].[t_tbmARC_VIDEO].[fsFILE_PATH]+[dbo].[t_tbmARC_VIDEO].[fsFILE_NO]+'.'+[dbo].[t_tbmARC_VIDEO].[fsFILE_TYPE] FILE_PATH
			FROM [dbo].[t_tbmARC_VIDEO]
			WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] = @fsFILE_NO
			UNION ALL 
			SELECT [dbo].[t_tbmARC_VIDEO].[fsFILE_PATH_H]+[dbo].[t_tbmARC_VIDEO].[fsFILE_NO]+'_H.'+[dbo].[t_tbmARC_VIDEO].[fsFILE_TYPE_H] FILE_PATH
			FROM [dbo].[t_tbmARC_VIDEO]
			WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] = @fsFILE_NO
			UNION ALL 
			SELECT [dbo].[t_tbmARC_VIDEO].[fsFILE_PATH_L]+[dbo].[t_tbmARC_VIDEO].[fsFILE_NO]+'_L.'+[dbo].[t_tbmARC_VIDEO].[fsFILE_TYPE_L] FILE_PATH
			FROM [dbo].[t_tbmARC_VIDEO]
			WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] = @fsFILE_NO	
			UNION ALL 
			SELECT [dbo].[t_tbmARC_VIDEO].[fsFILE_PATH_L]+'thumbnail\'+[dbo].[t_tbmARC_VIDEO].[fsFILE_NO]+'_thumb.jpg' FILE_PATH
			FROM [dbo].[t_tbmARC_VIDEO]
			WHERE [dbo].[t_tbmARC_VIDEO].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] = @fsFILE_NO
			UNION ALL 
			SELECT [dbo].[t_tbmARC_VIDEO_K].[fsFILE_PATH] + [dbo].[t_tbmARC_VIDEO_K].[fsTIME] + '.' + [dbo].[t_tbmARC_VIDEO_K].[fsFILE_TYPE] FILE_PATH
			FROM [dbo].[t_tbmARC_VIDEO_K]
			WHERE [dbo].[t_tbmARC_VIDEO_K].[fsFILE_NO] = @fsFILE_NO				
		END
		ELSE IF (@fsTYPE = 'A')
		BEGIN
			SELECT [dbo].[t_tbmARC_AUDIO].[fsFILE_PATH]+[dbo].[t_tbmARC_AUDIO].[fsFILE_NO]+'.'+[dbo].[t_tbmARC_AUDIO].[fsFILE_TYPE] FILE_PATH
			FROM [dbo].[t_tbmARC_AUDIO]
			WHERE [dbo].[t_tbmARC_AUDIO].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] = @fsFILE_NO
			UNION ALL 
			SELECT [dbo].[t_tbmARC_AUDIO].[fsFILE_PATH_H]+[dbo].[t_tbmARC_AUDIO].[fsFILE_NO]+'_H.'+[dbo].[t_tbmARC_AUDIO].[fsFILE_TYPE_H] FILE_PATH
			FROM [dbo].[t_tbmARC_AUDIO]
			WHERE [dbo].[t_tbmARC_AUDIO].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] = @fsFILE_NO
			UNION ALL 
			SELECT [dbo].[t_tbmARC_AUDIO].[fsFILE_PATH_L]+[dbo].[t_tbmARC_AUDIO].[fsFILE_NO]+'_L.'+[dbo].[t_tbmARC_AUDIO].[fsFILE_TYPE_L] FILE_PATH
			FROM [dbo].[t_tbmARC_AUDIO]
			WHERE [dbo].[t_tbmARC_AUDIO].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] = @fsFILE_NO
		END
		ELSE IF (@fsTYPE = 'P')
		BEGIN			
			SELECT [dbo].[t_tbmARC_PHOTO].[fsFILE_PATH]+[dbo].[t_tbmARC_PHOTO].[fsFILE_NO]+'.'+[dbo].[t_tbmARC_PHOTO].[fsFILE_TYPE]  FILE_PATH
			FROM [dbo].[t_tbmARC_PHOTO]
			WHERE [dbo].[t_tbmARC_PHOTO].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_PHOTO].[fsFILE_NO] = @fsFILE_NO
			UNION ALL 
			SELECT [dbo].[t_tbmARC_PHOTO].[fsFILE_PATH_H]+[dbo].[t_tbmARC_PHOTO].[fsFILE_NO]+'_H.'+[dbo].[t_tbmARC_PHOTO].[fsFILE_TYPE_H]  FILE_PATH
			FROM [dbo].[t_tbmARC_PHOTO]
			WHERE [dbo].[t_tbmARC_PHOTO].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_PHOTO].[fsFILE_NO] = @fsFILE_NO
			UNION ALL 
			SELECT [dbo].[t_tbmARC_PHOTO].[fsFILE_PATH_L]+[dbo].[t_tbmARC_PHOTO].[fsFILE_NO]+'_L.'+[dbo].[t_tbmARC_PHOTO].[fsFILE_TYPE_L] FILE_PATH
			FROM [dbo].[t_tbmARC_PHOTO]
			WHERE [dbo].[t_tbmARC_PHOTO].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_PHOTO].[fsFILE_NO] = @fsFILE_NO
			UNION ALL 
			SELECT @FILE_PATH_PT + [dbo].[t_tbmARC_PHOTO].[fsFILE_NO]+'_thumb.jpg' FILE_PATH
			FROM [dbo].[t_tbmARC_PHOTO]		
			WHERE [dbo].[t_tbmARC_PHOTO].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_PHOTO].[fsFILE_NO] = @fsFILE_NO
		END
		ELSE IF (@fsTYPE = 'D')
		BEGIN
			SELECT [dbo].[t_tbmARC_DOC].[fsFILE_PATH] + [dbo].[t_tbmARC_DOC].[fsFILE_NO] + '.' + [dbo].[t_tbmARC_DOC].[fsFILE_TYPE] FILE_PATH
			FROM [dbo].[t_tbmARC_DOC]
			WHERE [dbo].[t_tbmARC_DOC].[fnINDEX] = @fnINDEX_ID and [dbo].[t_tbmARC_DOC].[fsFILE_NO] = @fsFILE_NO
			--declare @type3 as varchar(10);
			--set @type3 = (SELECT fsFILE_TYPE_2 FROM t_tbmARC_DOC WHERE fnINDEX = @fnINDEX_ID and fsFILE_NO = @fsFILE_NO		)
			--if (@type3 = '') 
			--begin
			--	SELECT fsFILE_PATH+fsFILE_NO+'.'+fsFILE_TYPE FILE_PATH
			--	FROM t_tbmARC_DOC
			--	WHERE fnINDEX = @fnINDEX_ID and fsFILE_NO = @fsFILE_NO
			--end
			--else begin
			--	SELECT fsFILE_PATH+fsFILE_NO+'.'+fsFILE_TYPE FILE_PATH
			--	FROM t_tbmARC_DOC
			--	WHERE fnINDEX = @fnINDEX_ID and fsFILE_NO = @fsFILE_NO
			--	union 
			--	SELECT fsFILE_PATH_2+fsFILE_NO+'_L.'+fsFILE_TYPE_2 FILE_PATH
			--	FROM t_tbmARC_DOC
			--	WHERE fnINDEX = @fnINDEX_ID and fsFILE_NO = @fsFILE_NO
			--end
		END		
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



