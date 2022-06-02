





-- =============================================
-- 描述:	傳入V/A/P/D回傳註冊工作項中,優先順序高的未處理工作
-- 記錄:	<2011/11/24><Dennis.Wen><新增本預存>	
--	    <2011/11/24><Dennis.Wen>不轉出xps
--		<2013//02/26><Dennis.Wen><dk>
--		<2013//02/27><Eric.Huang><修改高解低解路徑>
--		<2013//05/16><Eric.Huang><EBC 當檔案類型是'V' ,當fsTYPE是'DAILY_ITP'時 FILE_FROM的檔案取用高解檔!>
--		<2013/08/13><Eric.Huang><EP 當檔案類型是'V/A/P' ,當fsTYPE是'TRANSCODE'時 FILE_FROM的檔案取用高解檔!>
--		<2013/08/28><Eric.Huang><EP 當檔案類型是'V'時,判斷fsRESOL_TAG 為HD or SD , 從tbzCONFIG給不同PROFILE>
--      <2013/09/03><Albert.Chen><修改本預存><@PAREMETERS放寬到300>
--      <2013/09/11><Eric.Huang><修改本預存><KEYFRAME_RULE1 => KEYFRAME_RULE>
-- 記錄:	<2013/11/19><Eric.Huang><修改本預存><新增 MTS_TOOL 因應後續單一客戶多重轉檔工具的可能性>
-- 記錄:	<2014/04/10><Eric.Huang><修改本預存><當客戶為FTV時,上傳檔案時,不轉高解檔>
-- 記錄:	<2014/06/09><Eric.Huang><修改本預存><當客戶為FTV時,DOC的原始檔沒有_H>
-- 記錄:	<2015/01/21><Eric.Huang><修改本預存><ams01=>172.20.144.87>
-- 記錄:	<2015/02/12><Eric.Huang><從2015/02/13開始,低解/keyframe 存到ams01\newmedia\MAM_MEDIA\>
-- 記錄:	<2016/08/04><Eric.Huang><FOR 立法院 USER MEDIALOOKS>
 -- ※此預存目前只允許轉檔的AP會用, 其他程式叫用會影響STATUS的變化
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_WORK_TRANSCODE_BY_TYPE]
	@TYPE VARCHAR(1)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY

		--EXEC [dbo].[sp_x_UPDATE_VP_WORK_C]
		--EXEC [dbo].[sp_x_UPDATE_TR_WORK_C]
	
		--DECLARE
		--	@TYPE VARCHAR(1)
			
		-------

		--SET @TYPE = 'V'

		-------

			DECLARE
				@WORK_ID	BIGINT,
				--START 2013/09/03 Update By Albert 放寬到300
				--@PAREMETERS		NVARCHAR(200),
				@PAREMETERS		NVARCHAR(300),
				--END   2013/09/03 Update By Albert 放髖到300
				@FILE_NO	VARCHAR(16),
				@SUBJECT_ID	VARCHAR(12),
				
				@FILE_PATH		NVARCHAR(100),
				@FILE_TYPE_H	VARCHAR(500),
				@FILE_TYPE_L	VARCHAR(500),
				@KF_PATH		NVARCHAR(100),
				@THUMM_PATH		NVARCHAR(100),
				@FILE_KF_CONFIG	nvarchar(500),

				@STATUS_0	VARCHAR(2),
				@TRANS_TO	VARCHAR(10),
				@FILE_TYPE  VARCHAR(20),

				@FILE_PATH_V_H	VARCHAR(200),
				@FILE_PATH_V_L	VARCHAR(200),
				@FILE_PATH_V_K  VARCHAR(200),
				@FILE_PATH_V_T  VARCHAR(200),
				@CUSTOMER_ID	VARCHAR(100)
				
				
				SET @CUSTOMER_ID = (SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE fsKEY = 'CUSTOMER_ID')



			/*取出待處理項(STATUS=0開頭的)中優先順序高的轉檔工作WORK_ID*/
			SET @WORK_ID = ISNULL((	SELECT	TOP 1 fnWORK_ID
									FROM	tblWORK
									WHERE	(fsSTATUS LIKE '0%') AND (fsPARAMETERS LIKE @TYPE + '%')	--9開頭表示完成 >9的也已結束 <9表示未完成
									         AND (fsTYPE IN ('TRANSCODE','DAILY_ITP','VP_YOUTUBE','TR_VIDEO','MAT'))
									ORDER BY fsPRIORITY, fnWORK_ID ),-1)

		--SELECT @WORK_ID

			/*依據WORK_ID取回相關參數*/
			SELECT @PAREMETERS = fsPARAMETERS, @STATUS_0 = fsSTATUS, @FILE_TYPE = fsTYPE
			FROM tblWORK WHERE (fnWORK_ID = @WORK_ID)
			
			/*將狀態修改為01*/
			UPDATE	tblWORK
			SET		fsSTATUS = '01'
			WHERE	(fnWORK_ID = @WORK_ID)

		--SELECT @PAREMETERS

			/*由參數解析為要處理的FILE_NO與SUBJ_ID*/
			SELECT
				@FILE_NO	= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 1),	
				@SUBJECT_ID	= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 2),
				@TRANS_TO	= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 3)
				
		--SELECT @FILE_NO, @SUBJECT_ID
				
			/*@FILE_PATH*/
			SELECT @FILE_PATH = dbo.fnGET_ARC_FILE_PATH(@SUBJECT_ID,@FILE_NO,'',@TYPE)
			SELECT @KF_PATH = CASE	WHEN (@TYPE = 'V') THEN dbo.fnGET_ARC_FILE_PATH(@SUBJECT_ID,@FILE_NO,'','K')
									WHEN (@TYPE = 'P') THEN dbo.fnGET_ARC_FILE_PATH(@SUBJECT_ID,@FILE_NO,'','T')
									ELSE '' END
			SET @THUMM_PATH = REPLACE(dbo.fnGET_ARC_FILE_PATH(@SUBJECT_ID,@FILE_NO,'','T'),'\P\','\V\')

			-- 2013/02/27 ++ eric 取EBC 高低解正確路徑
			SELECT @FILE_PATH_V_H	= dbo.fnGET_ARC_FILE_PATH(@SUBJECT_ID,@FILE_NO,'','H')
			SELECT @FILE_PATH_V_L	= dbo.fnGET_ARC_FILE_PATH(@SUBJECT_ID,@FILE_NO,'','V')
			SELECT @FILE_PATH_V_K   = dbo.fnGET_ARC_FILE_PATH(@SUBJECT_ID,@FILE_NO,'','K')
			
			SELECT @FILE_PATH_V_T   = @FILE_PATH_V_L + 'thumbnail' + '\'

			---- 2015/02/12 ERIC EDIT ------------------------------------START------------------------------------------------
			--IF (@FILE_NO >= '20150213_0000000')
			--	BEGIN
			--		SELECT @FILE_PATH_V_T   = REPLACE(@FILE_PATH_V_L + 'thumbnail' + '\','MAM_MEDIA','NEWMEDIA\MAM_MEDIA')
			--	END
			--ELSE
			--	BEGIN
			--		SELECT @FILE_PATH_V_T   = @FILE_PATH_V_L + 'thumbnail' + '\'
			--	END
			-- 2015/02/12 ERIC EDIT -------------------------------------END-------------------------------------------------

			--SELECT @FILE_PATH_V_T   = @FILE_PATH_V_L + 'thumbnail' + '\'
			
			--DECLARE @YYYY VARCHAR(4) = (select substring(@FILE_NO,1,4)),
							--@MM   VARCHAR(2) = (select substring(@FILE_NO,5,2)),
							--@DD   VARCHAR(2) = (select substring(@FILE_NO,7,2))
			-- 2013/02/27 -- eric 取EBC 高低解正確路徑
			--SET @FILE_PATH_V_H = @FILE_PATH_V_H + @YYYY + '\' + @MM + '\' + @DD + '\'
			--SET @FILE_PATH_V_L = @FILE_PATH_V_L + @YYYY + '\' + @MM + '\' + @DD + '\'
			--SET @FILE_PATH_V_K = @FILE_PATH_V_K + @YYYY + '\' + @MM + '\' + @DD + '\' + @FILE_NO + '\'
			--SET @FILE_PATH_V_T = @FILE_PATH_V_L + 'thumbnail' + '\'




			--dbo.fnGET_ARC_FILE_PATH(@SUBJECT_ID,@FILE_NO,'','K')
			
		--SELECT @FILE_PATH, @KF_PATH

			-- 2013/08/27 Eric ++				
			IF ('EP' = (SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE fsKEY = 'CUSTOMER_ID'))

				BEGIN
					DECLARE @RESOL_TAG VARCHAR(2)
					IF (@TYPE= 'V')
						BEGIN

							SET @RESOL_TAG = (SELECT fsRESOL_TAG from tbmARC_VIDEO WHERE fsFILE_NO = @FILE_NO)

							IF (@RESOL_TAG = 'SD')
								BEGIN
									SET @FILE_TYPE_H = (SELECT	TOP 1 fsVALUE FROM	tbzCONFIG WHERE	fsKEY = 'TRANSCODE_VDO_SD_EP')
								END

							ELSE
								BEGIN
									SET @FILE_TYPE_H = (SELECT	TOP 1 fsVALUE FROM	tbzCONFIG WHERE	fsKEY = 'TRANSCODE_VDO_HD_EP')
								END
						END

					ELSE
						BEGIN

							SET @FILE_TYPE_H = (SELECT	TOP 1 fsVALUE
												FROM	tbzCONFIG
												WHERE	(@TYPE = 'A' AND fsKEY = 'TRANSCODE_ADO_H') OR
														(@TYPE = 'P' AND fsKEY = 'TRANSCODE_PHO_H') OR (@TYPE = 'D' AND fsKEY = 'TRANSCODE_DOC_H') )
						END
					
					/*從[tbzCONFIG]資料表取回高低解的副檔名*/
					
				END
			-- 2013/08/27 Eric ++				

			ELSE

				BEGIN

					/*從[tbzCONFIG]資料表取回高低解的副檔名*/
					SET @FILE_TYPE_H = (SELECT	TOP 1 fsVALUE
										FROM	tbzCONFIG
										WHERE	(@TYPE = 'V' AND fsKEY = 'TRANSCODE_VDO_H') OR (@TYPE = 'A' AND fsKEY = 'TRANSCODE_ADO_H') OR
												(@TYPE = 'P' AND fsKEY = 'TRANSCODE_PHO_H') OR (@TYPE = 'D' AND fsKEY = 'TRANSCODE_DOC_H') )

				END

				SET @FILE_TYPE_L = (SELECT	TOP 1 fsVALUE
									FROM	tbzCONFIG
									WHERE	(@TYPE = 'V' AND fsKEY = 'TRANSCODE_VDO_L') OR (@TYPE = 'A' AND fsKEY = 'TRANSCODE_ADO_L') OR
											(@TYPE = 'P' AND fsKEY = 'TRANSCODE_PHO_L') OR (@TYPE = 'D' AND fsKEY = 'TRANSCODE_DOC_L') )

				SELECT @FILE_KF_CONFIG = ISNULL((SELECT	TOP 1 fsVALUE
												FROM	tbzCONFIG
												WHERE	/*(@TYPE = 'V' AND fsKEY = 'TRANSCODE_VDO_K') OR*/ (@TYPE = 'P' AND fsKEY = 'TRANSCODE_PHO_T') ),'')
											+ 
										 ISNULL((SELECT	TOP 1 fsVALUE
											FROM	tbzCONFIG
											WHERE	(@TYPE = 'V' AND fsKEY = 'KEYFRAME_RULE')),'')

				--SELECT @FILE_TYPE_H, @FILE_TYPE_L
		
				/*若取不到媒體檔資訊,可能是被移除了,排程取消*/
				IF(((SELECT COUNT(*) FROM tbmARC_VIDEO WHERE (@TYPE = 'V' AND fsFILE_NO = @FILE_NO))+
					(SELECT COUNT(*) FROM tbmARC_AUDIO WHERE (@TYPE = 'A' AND fsFILE_NO = @FILE_NO))+
					(SELECT COUNT(*) FROM tbmARC_PHOTO WHERE (@TYPE = 'P' AND fsFILE_NO = @FILE_NO))+
					(SELECT COUNT(*) FROM tbmARC_DOC WHERE (@TYPE = 'D' AND fsFILE_NO = @FILE_NO)) = 0))
				BEGIN
					UPDATE tblWORK
					SET fsSTATUS = 'C0'
					WHERE (fnWORK_ID = @WORK_ID)
				END
		
				
				/*取回各欄位*/
				--SELECT	WORK_ID = @WORK_ID,														--<=後續更新進度與狀態用
				--		FILE_NO = fsFILE_NO,													--<=後續更新媒體檔路徑用
				--		FILE_FROM = ISNULL(fsFILE_PATH + @FILE_NO + '.' + fsFILE_TYPE,''),		--<=來源檔, 使用者上傳的檔案
				--		FILE_H_TO = ISNULL(@FILE_PATH + @FILE_NO + '_H.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_H,0),''),	--<=轉出的高解檔路徑與檔名
				--		FILE_H_CONFIG = ISNULL(@FILE_TYPE_H,''),								--<=[tbzCONFIG]中對此高解檔的設定
				--		FILE_L_TO = ISNULL(@FILE_PATH + @FILE_NO + '_L.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_L,0),''),	--<=轉出的低解檔路徑與檔名
				--		FILE_L_CONFIG = ISNULL(@FILE_TYPE_L,''),								--<=[tbzCONFIG]中對此高解檔的設定
				--		KF_PATH = @KF_PATH,														--<=轉出的關鍵影格路徑與檔名
				--		STATUS_0 = @STATUS_0,
				--		TRANS_TO = 'HLK'
				--FROM	tbmARC_VIDEO
				--WHERE	(@TYPE = 'V' AND fsFILE_NO = @FILE_NO) OR
				--		(@TYPE = 'A' AND fsFILE_NO = @FILE_NO) OR
				--		(@TYPE = 'P' AND fsFILE_NO = @FILE_NO) OR
				--		(@TYPE = 'D' AND fsFILE_NO = @FILE_NO)
			
				--<2013//05/16><Eric.Huang><EBC 當檔案類型是'V' ,當fsTYPE是'DAILY_ITP'時 FILE_FROM的檔案取用高解檔!>
				IF (@FILE_TYPE = 'DAILY_ITP')
					BEGIN

									/*取回各欄位*/
							SELECT	WORK_ID = @WORK_ID,														--<=後續更新進度與狀態用
									FILE_NO = fsFILE_NO,													--<=後續更新媒體檔路徑用
									FILE_FROM = ISNULL(fsFILE_PATH + @FILE_NO + '_H.' + fsFILE_TYPE,''),		--<=來源檔, 使用者上傳的檔案

									-- 2013/02/27 -- eric
									--FILE_H_TO = ISNULL(@FILE_PATH + @FILE_NO + '_H.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_H,0),''),	--<=轉出的高解檔路徑與檔名
									-- 2013/02/27 ++ eric
									FILE_H_TO = ISNULL(@FILE_PATH_V_H + @FILE_NO + '_H.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_H,0),''),	--<=轉出的高解檔路徑與檔名

									FILE_H_CONFIG = ISNULL(@FILE_TYPE_H,''),								--<=[tbzCONFIG]中對此高解檔的設定
									-- 2013/02/27 -- eric
									--FILE_L_TO = ISNULL(@FILE_PATH + @FILE_NO + '_L.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_L,0),''),	--<=轉出的低解檔路徑與檔名
									-- 2013/02/27 ++ eric
									FILE_L_TO = ISNULL(@FILE_PATH_V_L + @FILE_NO + '_L.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_L,0),''),	--<=轉出的低解檔路徑與檔名

									FILE_L_CONFIG = ISNULL(@FILE_TYPE_L,''),								--<=[tbzCONFIG]中對此高解檔的設定

									-- 2013/02/27 -- eric
									--KF_PATH = CASE WHEN (@TRANS_TO LIKE '%K%') THEN @KF_PATH ELSE '' END,	--<=轉出的關鍵影格路徑與檔名
									-- 2013/02/27 ++ eric
									KF_PATH = ISNULL(@FILE_PATH_V_K,''),	--<=轉出的關鍵影格路徑與檔名

									-- 2013/02/27 -- eric
									--THUMB_PATH = @THUMM_PATH + @FILE_NO + '_thumb.jpg',
									-- 2013/02/27 ++ eric
									THUMB_PATH = @FILE_PATH_V_T + @FILE_NO + '_thumb.jpg',

									FILE_KF_CONFIG = @FILE_KF_CONFIG, 
									STATUS_0 = @STATUS_0,
									TRANS_TO = @TRANS_TO 
									-- 2013/11/19 新增 
									,MTS_TOOL            = 'M'
									--,MTS_TOOL            = 'C'
									--,MTS_TOOL            = 'F'	
									-- 2013/11/19 新增 
							FROM	tbmARC_VIDEO
							WHERE	(@TYPE = 'V' AND fsFILE_NO = @FILE_NO)
						UNION
							SELECT	WORK_ID = @WORK_ID,														--<=後續更新進度與狀態用
									FILE_NO = fsFILE_NO,													--<=後續更新媒體檔路徑用
									FILE_FROM = ISNULL(fsFILE_PATH + @FILE_NO + '.' + fsFILE_TYPE,''),		--<=來源檔, 使用者上傳的檔案
									FILE_H_TO = ISNULL(@FILE_PATH + @FILE_NO + '_H.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_H,0),''),	--<=轉出的高解檔路徑與檔名
									FILE_H_CONFIG = ISNULL(@FILE_TYPE_H,''),								--<=[tbzCONFIG]中對此高解檔的設定
									FILE_L_TO = ISNULL(@FILE_PATH + @FILE_NO + '_L.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_L,0),''),	--<=轉出的低解檔路徑與檔名
									FILE_L_CONFIG = ISNULL(@FILE_TYPE_L,''),								--<=[tbzCONFIG]中對此高解檔的設定
									KF_PATH = '',														--<=轉出的關鍵影格路徑與檔名
									THUMB_PATH = '',
									FILE_KF_CONFIG = '', 
									STATUS_0 = @STATUS_0,
									TRANS_TO = @TRANS_TO
					  				-- 2013/11/19 新增 
									,MTS_TOOL            = 'M'
									--,MTS_TOOL            = 'F'	
									-- 2013/11/19 新增 )
							FROM	tbmARC_AUDIO
							WHERE	(@TYPE = 'A' AND fsFILE_NO = @FILE_NO)
						UNION
							SELECT	WORK_ID = @WORK_ID,														--<=後續更新進度與狀態用
									FILE_NO = fsFILE_NO,													--<=後續更新媒體檔路徑用
									FILE_FROM = ISNULL(fsFILE_PATH + @FILE_NO + '.' + fsFILE_TYPE,''),		--<=來源檔, 使用者上傳的檔案
									FILE_H_TO = ISNULL(@FILE_PATH + @FILE_NO + '_H.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_H,0),''),	--<=轉出的高解檔路徑與檔名
									FILE_H_CONFIG = ISNULL(@FILE_TYPE_H,''),								--<=[tbzCONFIG]中對此高解檔的設定
									FILE_L_TO = ISNULL(@FILE_PATH + @FILE_NO + '_L.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_L,0),''),	--<=轉出的低解檔路徑與檔名
									FILE_L_CONFIG = ISNULL(@FILE_TYPE_L,''),								--<=[tbzCONFIG]中對此高解檔的設定
									KF_PATH = CASE WHEN (@TRANS_TO LIKE '%T%') THEN @KF_PATH ELSE '' END,	--<=轉出的關鍵影格路徑與檔名
									THUMB_PATH = '',
									FILE_KF_CONFIG = @FILE_KF_CONFIG, 
									STATUS_0 = @STATUS_0,
									TRANS_TO = @TRANS_TO
									-- 2013/11/19 新增 
									,MTS_TOOL            = 'M'
									--,MTS_TOOL            = 'F'	
									-- 2013/11/19 新增 
							FROM	tbmARC_PHOTO
							WHERE	(@TYPE = 'P' AND fsFILE_NO = @FILE_NO)
						UNION
							SELECT	WORK_ID = @WORK_ID,														--<=後續更新進度與狀態用
									FILE_NO = fsFILE_NO,													--<=後續更新媒體檔路徑用
									FILE_FROM = ISNULL(fsFILE_PATH + @FILE_NO + '.' + fsFILE_TYPE,''),		--<=來源檔, 使用者上傳的檔案
									FILE_H_TO = ISNULL(@FILE_PATH + @FILE_NO + '_H.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_H,0),''),	--<=轉出的高解檔路徑與檔名
									FILE_H_CONFIG = ISNULL(@FILE_TYPE_H,''),								--<=[tbzCONFIG]中對此高解檔的設定
									FILE_L_TO = ISNULL(@FILE_PATH + @FILE_NO + '_L.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_L,0),''),	--<=轉出的低解檔路徑與檔名
									FILE_L_CONFIG = ISNULL(@FILE_TYPE_L,''),								--<=[tbzCONFIG]中對此高解檔的設定
									KF_PATH = '',															--<=轉出的關鍵影格路徑與檔名
									THUMB_PATH = '',
									FILE_KF_CONFIG = '', 
									STATUS_0 = @STATUS_0,													
									--TRANS_TO = @TRANS_TO													-- 2012/03/21 -- 不轉出xps
									TRANS_TO = REPLACE(@TRANS_TO,'H','') 									-- 2012/03/21 -- 不轉出xps		
									-- 2013/11/19 新增 
									,MTS_TOOL            = 'M'
									--,MTS_TOOL            = 'F'	
									-- 2013/11/19 新增 		
							FROM	tbmARC_DOC
							WHERE	(@TYPE = 'D' AND fsFILE_NO = @FILE_NO)
					END
				ELSE IF (@FILE_TYPE = 'MAT')
					BEGIN
							/*取回各欄位*/
							SELECT	WORK_ID = @WORK_ID,														--<=後續更新進度與狀態用
									FILE_NO = fsFILE_NO,													--<=後續更新媒體檔路徑用
									---- 2013/08/13 -- eric ++
									--FILE_FROM = ISNULL(fsFILE_PATH + @FILE_NO + '_H.' + fsFILE_TYPE,''),		--<=來源檔, 使用者上傳的檔案
									---- 2013/08/13 -- eric --

									--FILE_FROM = ISNULL(fsFILE_PATH + @FILE_NO + '.' + fsFILE_TYPE,''),		--<=來源檔, 使用者上傳的檔案
									--FILE_H_TO = ISNULL(@FILE_PATH + @FILE_NO + '_H.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_H,0),''),	--<=轉出的高解檔路徑與檔名

									---- 2014/04/10 -- eric ++
									FILE_FROM = IIF(@CUSTOMER_ID = 'FTV', ISNULL(fsFILE_PATH + @FILE_NO + '.' + fsFILE_TYPE,''), ISNULL(fsFILE_PATH + @FILE_NO + '.' + fsFILE_TYPE,'')),
									FILE_H_TO = IIF(@CUSTOMER_ID = 'FTV', ISNULL(@FILE_PATH + @FILE_NO + '_H.' + fsFILE_TYPE,''), ISNULL(@FILE_PATH + @FILE_NO + '_H.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_H,0),'')),

									FILE_H_CONFIG = ISNULL(@FILE_TYPE_H,''),								--<=[tbzCONFIG]中對此高解檔的設定
									FILE_L_TO = ISNULL(@FILE_PATH + @FILE_NO + '_L.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_L,0),''),	--<=轉出的低解檔路徑與檔名
									FILE_L_CONFIG = ISNULL(@FILE_TYPE_L,''),								--<=[tbzCONFIG]中對此高解檔的設定
									KF_PATH = '',															--<=轉出的關鍵影格路徑與檔名
									THUMB_PATH = '',
									FILE_KF_CONFIG = '', 
									STATUS_0 = @STATUS_0,													
									--TRANS_TO = @TRANS_TO													-- 2012/03/21 -- 不轉出xps
									TRANS_TO = REPLACE(@TRANS_TO,'H','') 									-- 2012/03/21 -- 不轉出xps					
									-- 2013/11/19 新增 
									,MTS_TOOL            = 'M'
									--,MTS_TOOL            = 'F'	
									-- 2013/11/19 新增 
							FROM	tbmARC_DOC
							WHERE	(@TYPE = 'D' AND fsFILE_NO = @FILE_NO)
					END
				ELSE
					BEGIN

									/*取回各欄位*/
							SELECT	WORK_ID = @WORK_ID,														--<=後續更新進度與狀態用
									FILE_NO = fsFILE_NO,													--<=後續更新媒體檔路徑用
									
									---- 2014/04/10 -- eric ++
									FILE_FROM = ISNULL(UPPER(fsFILE_PATH) + @FILE_NO + '_H.' + fsFILE_TYPE,''),
									FILE_H_TO = ISNULL(UPPER(@FILE_PATH_V_H) + @FILE_NO + '_H.' + fsFILE_TYPE,''),
									
									FILE_H_CONFIG = ISNULL(@FILE_TYPE_H,''),								--<=[tbzCONFIG]中對此高解檔的設定
									
									FILE_L_TO = ISNULL(@FILE_PATH_V_L + @FILE_NO + '_L.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_L,0),''),	--<=轉出的低解檔路徑與檔名

									FILE_L_CONFIG = ISNULL(@FILE_TYPE_L,''),								--<=[tbzCONFIG]中對此高解檔的設定

									KF_PATH = ISNULL(@FILE_PATH_V_K,''),	--<=轉出的關鍵影格路徑與檔名

									THUMB_PATH = @FILE_PATH_V_T + @FILE_NO + '_thumb.jpg',

									FILE_KF_CONFIG = @FILE_KF_CONFIG, 
									STATUS_0 = @STATUS_0,
									TRANS_TO = @TRANS_TO
									-- 2013/11/19 新增 
						    		,MTS_TOOL            = 'M'
							FROM	tbmARC_VIDEO
							WHERE	(@TYPE = 'V' AND fsFILE_NO = @FILE_NO)
						UNION
							SELECT	WORK_ID = @WORK_ID,														--<=後續更新進度與狀態用
									FILE_NO = fsFILE_NO,													--<=後續更新媒體檔路徑用
									FILE_FROM = ISNULL(fsFILE_PATH + @FILE_NO + '_H.' + fsFILE_TYPE,''),
									FILE_H_TO = ISNULL(@FILE_PATH + @FILE_NO + '_H.' + fsFILE_TYPE,''),	--<=轉出的高解檔路徑與檔名


									FILE_H_CONFIG = ISNULL(@FILE_TYPE_H,''),								--<=[tbzCONFIG]中對此高解檔的設定
									FILE_L_TO = ISNULL(@FILE_PATH + @FILE_NO + '_L.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_L,0),''),	--<=轉出的低解檔路徑與檔名
									FILE_L_CONFIG = ISNULL(@FILE_TYPE_L,''),								--<=[tbzCONFIG]中對此高解檔的設定
									KF_PATH = '',														--<=轉出的關鍵影格路徑與檔名
									THUMB_PATH = '',
									FILE_KF_CONFIG = '', 
									STATUS_0 = @STATUS_0,
									TRANS_TO = @TRANS_TO
									-- 2013/11/19 新增 
									,MTS_TOOL            = 'M'
									--,MTS_TOOL            = 'F'	
									-- 2013/11/19 新增 
							FROM	tbmARC_AUDIO
							WHERE	(@TYPE = 'A' AND fsFILE_NO = @FILE_NO)
						UNION
							SELECT	WORK_ID = @WORK_ID,														--<=後續更新進度與狀態用
									FILE_NO = fsFILE_NO,													--<=後續更新媒體檔路徑用

									---- 2014/04/10 -- eric ++
									FILE_FROM = ISNULL(fsFILE_PATH + @FILE_NO + '_H.' + fsFILE_TYPE,''),
									FILE_H_TO = ISNULL(@FILE_PATH + @FILE_NO + '_H.' + fsFILE_TYPE,''),	--<=轉出的高解檔路徑與檔名


									FILE_H_CONFIG = ISNULL(@FILE_TYPE_H,''),								--<=[tbzCONFIG]中對此高解檔的設定
									FILE_L_TO = ISNULL(@FILE_PATH + @FILE_NO + '_L.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_L,0),''),	--<=轉出的低解檔路徑與檔名
									FILE_L_CONFIG = ISNULL(@FILE_TYPE_L,''),								--<=[tbzCONFIG]中對此高解檔的設定
									KF_PATH = CASE WHEN (@TRANS_TO LIKE '%T%') THEN @KF_PATH ELSE '' END,	--<=轉出的關鍵影格路徑與檔名
									THUMB_PATH = '',
									FILE_KF_CONFIG = @FILE_KF_CONFIG, 
									STATUS_0 = @STATUS_0,
									TRANS_TO = @TRANS_TO
					    			-- 2013/11/19 新增 
						    		,MTS_TOOL            = 'M'
									--  ,MTS_TOOL            = 'F'	
						    		-- 2013/11/19 新增 
							FROM	tbmARC_PHOTO
							WHERE	(@TYPE = 'P' AND fsFILE_NO = @FILE_NO)
						UNION
							SELECT	WORK_ID = @WORK_ID,														--<=後續更新進度與狀態用
									FILE_NO = fsFILE_NO,													--<=後續更新媒體檔路徑用
									
									---- 2014/06/09 -- eric ++
									FILE_FROM = ISNULL(fsFILE_PATH + @FILE_NO + '.' + fsFILE_TYPE,''),		--<=來源檔, 使用者上傳的檔案
									---- 2014/04/10 -- eric ++
									--FILE_FROM = IIF(@CUSTOMER_ID = 'FTV', ISNULL(fsFILE_PATH + @FILE_NO + '_H.' + fsFILE_TYPE,''), ISNULL(fsFILE_PATH + @FILE_NO + '.' + fsFILE_TYPE,'')),
									FILE_H_TO = ISNULL(@FILE_PATH + @FILE_NO + '.' + fsFILE_TYPE,''),

									FILE_H_CONFIG = ISNULL(@FILE_TYPE_H,''),								--<=[tbzCONFIG]中對此高解檔的設定
									FILE_L_TO = ISNULL(@FILE_PATH + @FILE_NO + '_L.' + dbo.fnGET_ITEM_BY_INDEX(@FILE_TYPE_L,0),''),	--<=轉出的低解檔路徑與檔名
									FILE_L_CONFIG = ISNULL(@FILE_TYPE_L,''),								--<=[tbzCONFIG]中對此高解檔的設定
									KF_PATH = '',															--<=轉出的關鍵影格路徑與檔名
									THUMB_PATH = '',
									FILE_KF_CONFIG = '', 
									STATUS_0 = @STATUS_0,													
									--TRANS_TO = @TRANS_TO													-- 2012/03/21 -- 不轉出xps
									TRANS_TO = REPLACE(@TRANS_TO,'H','') 									-- 2012/03/21 -- 不轉出xps					
									-- 2013/11/19 新增 
									,MTS_TOOL            = 'M'
									--,MTS_TOOL            = 'F'	
									-- 2013/11/19 新增 
							FROM	tbmARC_DOC
							WHERE	(@TYPE = 'D' AND fsFILE_NO = @FILE_NO)
					END

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



