﻿

-- =============================================
-- 描述:	新增 ARC_VIDEO_K 入庫項目-影片關鍵影格檔 資料
-- 記錄:	<2011/11/25><Dennis.Wen><新增本預存>
--			<2012/05/21><Dennis.Wen><一堆欄位調整>
-- ※讓轉檔AP可一次新增大量關鍵影格資料
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_ARC_VIDEO_K_BATCH]		
	@fsFILE_NO			VARCHAR(16),
	@TIME_ALL			VARCHAR(MAX),
	@fsFILE_PATH		NVARCHAR(100),
	@FILE_SIZE_ALL		NVARCHAR(MAX),
	@fsFILE_TYPE		VARCHAR(10), 
	@fsCREATED_BY		NVARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		--SELECT
		--	@fsFILE_NO		= 'TEST',
		--	@TIME_CODE_ALL	= '00000010;00000020;00000030;00000040;00000050;00000100;00000110;00000120;00000130;00000140;00000150;00000200;00000210;00000220;00000230;00000240;00000250;00000300;00000310;00000320;00000330;00000340;00000350;00000400;00000410;00000420;00000430;00000440;00000450;00000500;00000510;00000520;00000530;00000540;00000550;00000600;00000610;00000620;00000630;00000640;00000650;00000700;00000710;00000720;00000730;00000740;00000750;00000800;00000810;00000820;00000830;00000840;00000850;00000900;00000910;00000920;00000930;00000940;00000950;00001000;',
		--	@fsFILE_PATH	= '\\172.20.142.152\Media\k1\V\',
		--	@FILE_SIZE_ALL	= 'SIZE0010;SIZE0020;SIZE0030;SIZE0040;SIZE0050;SIZE0100;SIZE0110;SIZE0120;SIZE0110;SIZE0120;SIZE0110;SIZE0200;SIZE0210;SIZE0220;SIZE0210;SIZE0220;SIZE0210;SIZE0300;SIZE0310;SIZE0320;SIZE0030;SIZE0040;SIZE0050;SIZE0400;SIZE0410;SIZE0420;SIZE0110;SIZE0120;SIZE0110;SIZE0500;SIZE0510;SIZE0520;SIZE0210;SIZE0220;SIZE0210;SIZE0600;SIZE0610;SIZE0620;SIZE0030;SIZE0040;SIZE0050;SIZE0700;SIZE0710;SIZE0720;SIZE0110;SIZE0120;SIZE0110;SIZE0800;SIZE0810;SIZE0820;SIZE0210;SIZE0220;SIZE0210;SIZE0900;SIZE0910;SIZE0920;SIZE0030;SIZE0040;SIZE0050;SIZE1000;',
		--	@fsFILE_TYPE	= 'jpg', 
		--	@fsCREATED_BY	= 'Dennis.Wen'
		
		-----
		
		DECLARE @i INT = 0, @j INT = 0, @COUNT INT = LEN(@TIME_ALL) - LEN(REPLACE(@TIME_ALL,';',''))
		DECLARE @TIME_CODE_0 VARCHAR(11) = '', @TIME_CODE VARCHAR(11) = '', @FILE_SIZE NVARCHAR(50)
		/*NEW*/DECLARE @idx1 INT = 0, @idx2 INT = 0
					
		WHILE(@i < @COUNT)
		BEGIN
			SELECT
				@TIME_CODE_0 = dbo.fnGET_ITEM_BY_INDEX(@TIME_ALL,@i),
				@FILE_SIZE = dbo.fnGET_ITEM_BY_INDEX(@FILE_SIZE_ALL,@i)
			
			--/*NEW*/
			--/*取出第一個;位置*/
			--SELECT	@idx1 = CHARINDEX(';', @TIME_ALL),				
			--		@idx2 = CHARINDEX(';', @FILE_SIZE_ALL)
				
			--/*把第一個分號前的資料取出*/
			--SELECT
			--	@TIME_CODE_0 = SUBSTRING(@TIME_ALL, 1, @idx1-1),
			--	@FILE_SIZE = SUBSTRING(@FILE_SIZE_ALL, 1, @idx2-1)
				
			--/*把第一個分號前的資料移除*/
			--IF(@i < @COUNT-1)
			--BEGIN
			--	SELECT
			--		@TIME_ALL = SUBSTRING(@TIME_ALL, @idx1 + 1, LEN(@TIME_ALL) - @idx1),
			--		@FILE_SIZE_ALL = SUBSTRING(@FILE_SIZE_ALL, @idx2 + 1, LEN(@FILE_SIZE_ALL) - @idx2)						
			--END

			--SET @TIME_CODE = SUBSTRING(@TIME_CODE_0,1,2) + ':' +
			--				SUBSTRING(@TIME_CODE_0,3,2) + ':' +
			--				SUBSTRING(@TIME_CODE_0,5,2) + ';' +
			--				SUBSTRING(@TIME_CODE_0,7,2)
			
			SET @TIME_CODE = @TIME_CODE_0--LEFT(@TIME_CODE_0,6) + '.' + SUBSTRING(@TIME_CODE_0,7,3)
			
			--若紀錄已存在就不新增了，因為有可能是置換狀況下，不會刪除關鍵影格資料，只刪除體檔案

			IF ((SELECT COUNT(fsTIME) FROM tbmARC_VIDEO_K WHERE fsFILE_NO = @fsFILE_NO AND fsTIME = @TIME_CODE) = 0)
			BEGIN
				--若是第二張，則新增至Video當代表影格
				IF(@i = 1) 
				BEGIN 
					--INSERT tbmARC_VIDEO_K([fsFILE_NO],[fsTITLE],[fsDESCRIPTION],[fsFILE_PATH],[fsFILE_SIZE],[fsFILE_TYPE],[fcHEAD_FRAME],[fdCREATED_DATE],[fsCREATED_BY],[fdUPDATED_DATE],[fsUPDATED_BY],[fsTIME])
					--SELECT @fsFILE_NO, '關鍵影格:TIME = ' + LEFT(@TIME_CODE_0,6) + '.' + SUBSTRING(@TIME_CODE_0,7,3),'',@fsFILE_PATH, @FILE_SIZE, @fsFILE_TYPE,'Y',GETDATE(), @fsCREATED_BY, '1900/01/01', '', @TIME_CODE

					INSERT tbmARC_VIDEO_K([fsFILE_NO],[fsTITLE],[fsDESCRIPTION],[fsFILE_PATH],[fsFILE_SIZE],[fsFILE_TYPE],[fcHEAD_FRAME],[fdCREATED_DATE],[fsCREATED_BY],[fdUPDATED_DATE],[fsUPDATED_BY],[fsTIME])
					SELECT @fsFILE_NO, '','',@fsFILE_PATH, @FILE_SIZE, @fsFILE_TYPE,'Y',GETDATE(), @fsCREATED_BY, '1900/01/01', '', @TIME_CODE

					UPDATE tbmARC_VIDEO SET fsHEAD_FRAME = @fsFILE_NO + '_' + @TIME_CODE + '.jpg' WHERE fsFILE_NO = @fsFILE_NO 
				END
				ELSE
				BEGIN
					INSERT tbmARC_VIDEO_K([fsFILE_NO],[fsTITLE],[fsDESCRIPTION],[fsFILE_PATH],[fsFILE_SIZE],[fsFILE_TYPE],[fcHEAD_FRAME],[fdCREATED_DATE],[fsCREATED_BY],[fdUPDATED_DATE],[fsUPDATED_BY],[fsTIME])
					SELECT @fsFILE_NO, '','',@fsFILE_PATH, @FILE_SIZE, @fsFILE_TYPE,'N',GETDATE(), @fsCREATED_BY, '1900/01/01', '', @TIME_CODE
				END
			END

			SET @j = @j + @@ROWCOUNT

			--下一個
			SET @i = @i + 1
		END
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @j

		--DECLARE @i INT = 0, @j INT = 0, @COUNT INT = LEN(@TIME_ALL) - LEN(REPLACE(@TIME_ALL,';',''))
		--DECLARE @TIME_CODE_0 VARCHAR(11) = '', @TIME_CODE VARCHAR(11) = '', @FILE_SIZE NVARCHAR(50)

		--WHILE(@i<@COUNT)
		--BEGIN
		--	SELECT
		--		@TIME_CODE_0 = dbo.fnGET_ITEM_BY_INDEX(@TIME_ALL,@i),	--HHmmssff
		--		@FILE_SIZE = dbo.fnGET_ITEM_BY_INDEX(@FILE_SIZE_ALL,@i)

		--	SET @TIME_CODE = SUBSTRING(@TIME_CODE_0,1,2) + ':' +
		--					SUBSTRING(@TIME_CODE_0,3,2) + ':' +
		--					SUBSTRING(@TIME_CODE_0,5,2) + ';' +
		--					SUBSTRING(@TIME_CODE_0,7,2)

		--	INSERT tbmARC_VIDEO_K
		--	SELECT @fsFILE_NO, @TIME_CODE, '關鍵影格:TIME_CODE = ' + @TIME_CODE, '轉檔過程產生的關鍵影格',
		--			@fsFILE_PATH, @FILE_SIZE, @fsFILE_TYPE,
		--			GETDATE(), @fsCREATED_BY, '1900/01/01', ''
					
		--	SET @j = @j + @@ROWCOUNT

		--	--下一個
		--	SET @i = @i + 1
		--END
			
		---- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		--SELECT RESULT = @j
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


