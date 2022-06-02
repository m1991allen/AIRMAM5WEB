
-- =============================================
-- 描述:	傳入V/A/P/D回傳註冊工作項中,優先順序高的未處理工作
-- 記錄:	<2013/01/14><Albert.Chen><新增本預存><複製[spGET_WORK_BOOKING_BY_TYPE]來修改>
-- 記錄:	<2013/01/24><Albert.Chen><修改本預存><會將Status為0開頭的再次重轉,目前預設調用15小時已上,還未轉檔的>
-- 記錄:	<2013/02/26><Albert.Chen><修改本預存><使用正式的檔案路徑>
-- 記錄:	<2013/03/11><Albert.Chen><修改本預存><傳入HOST,並且使用此來當作寫入路徑>
-- 記錄:	<2013/04/11><Albert.Chen><加上回傳低解路徑,用來檢查影片的聲音檔是否有問題>
-- 記錄:	<2013/04/16><Albert.Chen><改為以主檔排序為主>
-- 記錄:	<2013/04/22><Albert.Chen><修改本預存><判斷是選擇高解格式或原始格式>
-- 記錄:	<2013/05/01><Albert.Chen><修改本預存><不要管卡住的了,因為現在有重設轉檔功能,所以就可能兩支AP都取到>
-- 記錄:	<2013/05/01><Albert.Chen><修改本預存><原始檔Ingest的會有_H路徑>
-- 記錄:	<2013/09/03><Albert.Chen><修改本預存><@PAREMETERS放寬到300>
-- 記錄:	<2013/11/11><Albert.Chen><修改本預存><給錯誤訊息看的,新增回傳欄位:標題(fsTitle),調用者(fsCreated_By),重設調用者(fsRebookinger)>
-- ※此預存目前只允許調用的AP會用, 其他程式叫用會影響STATUS的變化>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_WORK_NAS_BY_TYPE]
	@TYPE VARCHAR(1),
	@HOST NVARCHAR(255)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
			DECLARE
				@WORK_ID		   BIGINT,
				--START 2013/09/03 Update By Albert 放寬到300
				--@PAREMETERS		NVARCHAR(200),
				@PAREMETERS		   NVARCHAR(300),
				--END   2013/09/03 Update By Albert 放髖到300
				@STATUS_0		   VARCHAR(2),
				@CREATED_BY		   NVARCHAR(50),
				@CREATED_DATE      DATETIME,
				@FILE_NO		   VARCHAR(16),	
				@PATH_TYPE	       VARCHAR(1),
				@BEG_TIME		   VARCHAR(20),
				@END_TIME		   VARCHAR(20),
				@FROM_FILE_PATH    NVARCHAR(600),
				@TO_FILE_PATH      NVARCHAR(500),          --fnGET_NAS_OUTPUT_PATH1
                @FROM_FILE_EXTION  NVARCHAR(100),          --副檔名
                @FILE_NAME         NVARCHAR(100),          --檔名(用fsTitle)
                @_nMAX_NEED_TIME   INT,                    --用來設定大於幾小時的Status=01或02之類的被搶號,卻因為程式可能掛掉後,就不再轉檔的孤立資料
				@_sTYPE            CHAR(3),
		    	--START 2013/04/11 加上回傳低解路徑
				@fsFROM_FILE_PATH_L NVARCHAR(600),
				--END   2013/04/11 加上回傳低解路徑
				--START 2013/04/22 判斷是選擇高解格式或原始格式
				@_sFILE_TYPE        CHAR(1),
				--END   2013/04/22 判斷是選擇高解格式或原始格式
				@ARC_VIDEO_CREATED_DATE DATETIME,
				--START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				@fsTITLE            NVARCHAR(100),
				@fsCREATED_BY       NVARCHAR(50),
			    @fsREBOOKINGER      NVARCHAR(50)
			    --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				
			SELECT @FILE_NO='',@PATH_TYPE='',@BEG_TIME='',@END_TIME='',@FROM_FILE_PATH='',@TO_FILE_PATH='',@FROM_FILE_EXTION='',@FILE_NAME=''
	              ,@_nMAX_NEED_TIME=15,@_sTYPE='NAS'
	   		      --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				  ,@fsTITLE='',@fsCREATED_BY='',@fsREBOOKINGER=''
			      --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
	              
			/*取出待處理項(STATUS=0開頭的)中優先順序高的轉檔工作WORK_ID*/
			SET @WORK_ID = ISNULL((	SELECT	TOP 1 WK.fnWORK_ID
					FROM	tblWORK AS WK
					LEFT JOIN tbmBOOKING AS BK ON (BK.fnBOOKING_ID = WK.fnGROUP_ID)
					WHERE	((WK.fsSTATUS = '00') AND (WK._ITEM_TYPE =@TYPE)	--9開頭表示完成 >9的也已結束 <9表示未完成
					         AND (WK.fsTYPE = @_sTYPE) AND (BK.fsSTATUS LIKE '0%'))
			     --START 2013/05/01 Update By Albert 不要管卡住的了,因為現在有重設轉檔功能,所以就可能兩支AP都取到
					  --START 這邊是取出卡住的01或02繼續轉檔
					----  OR
						  ----((WK.fsSTATUS LIKE '0%' AND WK.fsSTATUS <> '00') AND (WK._ITEM_TYPE =@TYPE) AND (WK.fsTYPE = @_sTYPE)
						  ----AND DateDiff(hour,WK.[fdCREATED_DATE],GETDATE())>= @_nMAX_NEED_TIME)
					  --END   這邊是取出卡住的01或02繼續轉檔
			    --END 2013/05/01 Update By Albert 不要管卡住的了,因為現在有重設轉檔功能,所以就可能兩支AP都取到
			      
					--ORDER BY BK.fnORDER, BK.fnBOOKING_ID, 
					--START 2013/04/16改為以主檔排序為主  
					--ORDER BY WK.fsPRIORITY ,WK.fsSTATUS DESC ,BK.fnORDER , BK.fnBOOKING_ID, WK.fnWORK_ID),-1)
					  ORDER BY BK.fnORDER,WK.fsPRIORITY ,WK.fsSTATUS DESC , BK.fnBOOKING_ID, WK.fnWORK_ID),-1)
					--END 2013/04/16改為以主檔排序為主

				  --------START 這邊是取出卡住的01或02繼續轉檔
      ------              (CASE WHEN DateDiff(hour,WK.[fdCREATED_DATE],GETDATE())>= @_nMAX_NEED_TIME
      ------                AND WK.fsSTATUS LIKE '0%' AND WK.fsSTATUS <> '00' 
      ------                AND (WK._ITEM_TYPE =@TYPE) AND (WK.fsTYPE = @_sTYPE)
      ------                THEN 0 ELSE WK.fsPRIORITY END)
				  --------END   這邊是取出卡住的01或02繼續轉檔
					----, WK.fnWORK_ID ),-1)
						 
			IF(@WORK_ID <> -1)
			BEGIN
				SELECT
					@TYPE	 = dbo.fnGET_ITEM_BY_INDEX(fsPARAMETERS,0),
					@FILE_NO = dbo.fnGET_ITEM_BY_INDEX(fsPARAMETERS,1),
					@BEG_TIME		= CASE WHEN @TYPE='V' THEN _ITEM_SET2 ELSE NULL END,   --影才有剪輯區間
					@END_TIME		= CASE WHEN @TYPE='V' THEN _ITEM_SET3 ELSE NULL END    --影才有剪輯區間
				   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
					,@fsCREATED_BY=fsCREATED_BY,
					@fsREBOOKINGER=_ITEM_SET4
			       --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				FROM
					tblWORK
				WHERE
					(fnWORK_ID = @WORK_ID)			
						
				/*若取不到媒體檔資訊,可能是被移除了,排程取消*/
				IF(((SELECT COUNT(*) FROM tbmARC_VIDEO WHERE (@TYPE = 'V' AND fsFILE_NO = @FILE_NO))+
				   (SELECT COUNT(*) FROM tbmARC_AUDIO WHERE (@TYPE = 'A' AND fsFILE_NO = @FILE_NO))+
				   (SELECT COUNT(*) FROM tbmARC_PHOTO WHERE (@TYPE = 'P' AND fsFILE_NO = @FILE_NO))+
				   (SELECT COUNT(*) FROM tbmARC_DOC WHERE (@TYPE = 'D' AND fsFILE_NO = @FILE_NO)) = 0))
				BEGIN
					UPDATE tblWORK
					SET fsSTATUS = 'C0'
					WHERE (fnWORK_ID = @WORK_ID)
					
					SET @WORK_ID = -1
				END				
			  		
				/*依據WORK_ID取回相關參數*/
				SELECT @PAREMETERS = fsPARAMETERS, @STATUS_0 = fsSTATUS, @CREATED_BY = fsCREATED_BY, @CREATED_DATE = fdCREATED_DATE 
				FROM tblWORK WHERE (fnWORK_ID = @WORK_ID)			
				
				/*將狀態修改為01*/
				UPDATE	tblWORK
				SET		fsSTATUS = '01'
				WHERE	(fnWORK_ID = @WORK_ID) 
				
				/*由參數解析為要處理的FILE_NO與SUBJ_ID*/
				SELECT
					@FILE_NO		= dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 1),
					@PATH_TYPE	    = dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 2),
				    --START 2013/04/22 判斷是選擇高解格式或原始格式
				    @_sFILE_TYPE    = dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 5)
					--END   2013/04/22 判斷是選擇高解格式或原始格式
					
				----/*依照不同@PATH_TYPE呼叫不同預存組路徑*/
				----SET @toFILE_PATH = CASE @PATH_TYPE
				----					WHEN '1' THEN ('\' + REPLACE(dbo.fnGET_BOOKING_OUTPUT_PATH1(@TYPE, @CREATED_BY, @FOLDER),'\\','\'))
				----					WHEN '2' THEN (dbo.fnGET_BOOKING_OUTPUT_PATH2(@TYPE, @CREATED_BY, @FOLDER))
				----					ELSE '' END		
									
				----SET @toFILE_PATH_TYPE = CASE @PATH_TYPE
				----					WHEN '1' THEN 'UNC'
				----					WHEN '2' THEN 'FTP'
				----					ELSE '' END	 
									
				/*取出來源路徑檔案名稱*/
				--影片
				IF (@_sFILE_TYPE='1')
				    --原始檔案
					BEGIN
						SELECT
						       --START 2013/05/03 Ingest的會有_H路徑
							   @FROM_FILE_PATH=
							   CASE WHEN ISNULL(fxMEDIA_INFO,'')=''
							   THEN
									 ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H])
							   ELSE
								     --手動上傳(就不用H)
								     ([fsFILE_PATH] + fsFILE_NO + '.' + [fsFILE_TYPE])
							   END,
							   --END   2013/05/03 Ingest的會有_H路徑
							   @FROM_FILE_EXTION= [fsFILE_TYPE],
							   @FILE_NAME=fsTITLE,
							   --START 2013/04/11 加上回傳低解路徑
							   @fsFROM_FILE_PATH_L =([fsFILE_PATH_L] + fsFILE_NO + '_L.' + [fsFILE_TYPE_L]),
							   --END   2013/04/11 加上回傳低解路徑
							   @ARC_VIDEO_CREATED_DATE= fdCREATED_DATE
						FROM tbmARC_VIDEO
						WHERE (@TYPE = 'V') AND (fsFILE_NO = @FILE_NO)		
					END
				ELSE
				    --高解檔案
					BEGIN
					   	SELECT @FROM_FILE_PATH = ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H]),
							   @FROM_FILE_EXTION= [fsFILE_TYPE_H],
							   @FILE_NAME=fsTITLE,
							   --START 2013/04/11 加上回傳低解路徑
							   @fsFROM_FILE_PATH_L =([fsFILE_PATH_L] + fsFILE_NO + '_L.' + [fsFILE_TYPE_L]),
							   --END   2013/04/11 加上回傳低解路徑
							   @ARC_VIDEO_CREATED_DATE= fdCREATED_DATE
							   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
							  ,@fsTITLE = fsTITLE
							   --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
						FROM tbmARC_VIDEO
						WHERE (@TYPE = 'V') AND (fsFILE_NO = @FILE_NO)	
					END
                
				--聲音
				IF (@_sFILE_TYPE='1')
				    --原始檔案
					BEGIN
						SELECT @FROM_FILE_PATH = ([fsFILE_PATH] + fsFILE_NO + '.' + [fsFILE_TYPE]),
							   @FROM_FILE_EXTION= [fsFILE_TYPE],
							   @FILE_NAME=fsTITLE,
							   --START 2013/04/11 加上回傳低解路徑
							   @fsFROM_FILE_PATH_L =([fsFILE_PATH_L] + fsFILE_NO + '_L.' + [fsFILE_TYPE_L]),
							   --END   2013/04/11 加上回傳低解路徑
							   @ARC_VIDEO_CREATED_DATE= fdCREATED_DATE
			     			   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
							  ,@fsTITLE = fsTITLE
							   --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
						FROM tbmARC_AUDIO
						WHERE (@TYPE = 'A') AND (fsFILE_NO = @FILE_NO)
				    END
				ELSE
				    --高解檔案
					BEGIN
						SELECT @FROM_FILE_PATH = ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H]),
							   @FROM_FILE_EXTION= [fsFILE_TYPE_H],
							   @FILE_NAME=fsTITLE,
							   --START 2013/04/11 加上回傳低解路徑
							   @fsFROM_FILE_PATH_L =([fsFILE_PATH_L] + fsFILE_NO + '_L.' + [fsFILE_TYPE_L]),
							   --END   2013/04/11 加上回傳低解路徑
							   @ARC_VIDEO_CREATED_DATE= fdCREATED_DATE
							   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
							  ,@fsTITLE = fsTITLE
							   --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
						FROM tbmARC_AUDIO
						WHERE (@TYPE = 'A') AND (fsFILE_NO = @FILE_NO)
				    END

	            --圖片
				IF (@_sFILE_TYPE='1')
				    --原始檔案
					BEGIN
						SELECT @FROM_FILE_PATH = ([fsFILE_PATH] + fsFILE_NO + '.' + [fsFILE_TYPE]),
							   @FROM_FILE_EXTION= [fsFILE_TYPE],
							   @FILE_NAME=fsTITLE,
							   --START 2013/04/11 加上回傳低解路徑
							   @fsFROM_FILE_PATH_L =([fsFILE_PATH_L] + fsFILE_NO + '_L.' + [fsFILE_TYPE_L]),
							   --END   2013/04/11 加上回傳低解路徑
							   @ARC_VIDEO_CREATED_DATE= fdCREATED_DATE
							   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
							  ,@fsTITLE = fsTITLE
							   --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
						FROM tbmARC_PHOTO
						WHERE (@TYPE = 'P') AND (fsFILE_NO = @FILE_NO)	
					END
				ELSE 
				    --高解檔案
					BEGIN
						SELECT @FROM_FILE_PATH = ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H]),
							   @FROM_FILE_EXTION= [fsFILE_TYPE_H],
							   @FILE_NAME=fsTITLE,
							   --START 2013/04/11 加上回傳低解路徑
							   @fsFROM_FILE_PATH_L =([fsFILE_PATH_L] + fsFILE_NO + '_L.' + [fsFILE_TYPE_L]),
							   --END   2013/04/11 加上回傳低解路徑
							   @ARC_VIDEO_CREATED_DATE= fdCREATED_DATE
							   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
							  ,@fsTITLE = fsTITLE
							   --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
						FROM tbmARC_PHOTO
						WHERE (@TYPE = 'P') AND (fsFILE_NO = @FILE_NO)	
					END
	 		
			    --文件
				IF (@_sFILE_TYPE='1')
				    --原始檔案
					BEGIN
						SELECT @FROM_FILE_PATH = ([fsFILE_PATH] + fsFILE_NO + '.' + [fsFILE_TYPE]) ,
							   @FROM_FILE_EXTION= [fsFILE_TYPE],
							   @FILE_NAME=fsTITLE,
							   --START 2013/04/11 加上回傳低解路徑
							   @fsFROM_FILE_PATH_L =([fsFILE_PATH_2] + fsFILE_NO + '_L.' + [fsFILE_TYPE_2]),
							   --END   2013/04/11 加上回傳低解路徑
							   @ARC_VIDEO_CREATED_DATE= fdCREATED_DATE
							   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
							  ,@fsTITLE = fsTITLE
							   --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
						FROM tbmARC_DOC
						WHERE (@TYPE = 'D') AND (fsFILE_NO = @FILE_NO)	
					END	
				ELSE 
				    --高解檔案
					BEGIN
						SELECT @FROM_FILE_PATH = ([fsFILE_PATH] + fsFILE_NO + '.' + [fsFILE_TYPE]),
							   @FROM_FILE_EXTION= [fsFILE_TYPE],
							   @FILE_NAME=fsTITLE,
							   --START 2013/04/11 加上回傳低解路徑
							   @fsFROM_FILE_PATH_L =([fsFILE_PATH_2] + fsFILE_NO + '_L.' + [fsFILE_TYPE_2]),
							   --END   2013/04/11 加上回傳低解路徑
							   @ARC_VIDEO_CREATED_DATE= fdCREATED_DATE
							   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
							  ,@fsTITLE = fsTITLE
							   --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
						FROM tbmARC_DOC
						WHERE (@TYPE = 'D') AND (fsFILE_NO = @FILE_NO)	
					END
				
				/*設定檔案目的地(先從tbzCODE.fsSET取得路徑)*/
				DECLARE @CODE_SET_PATH VARCHAR(50)
				--START 2013/03/11 Update By Albert 傳入HOST,並且使用此來當作寫入路徑
				--SELECT @CODE_SET_PATH= fsSET
				SELECT @CODE_SET_PATH= @HOST + fsSET
				--END   2013/03/11 Update By Albert 
				FROM tbzCODE
				WHERE fsCODE_ID='BOOK010' AND fsCODE=@PATH_TYPE
				
				SET @TO_FILE_PATH=dbo.fnGET_NAS_OUTPUT_PATH1(@CODE_SET_PATH, @CREATED_BY, @CREATED_DATE)
			

				/*===============  下面這一段要想辦法換掉  ===============*/
				DECLARE
					@fsTYPE		VARCHAR(10),
					@fsNAME		NVARCHAR(10),
					@fsHEAD		VARCHAR(10),
					@fsBODY		VARCHAR(10),
					@fsNO_L		INT,
					@BY			VARCHAR(50)
					
				SELECT
					@fsTYPE		= 'NAS', 
					@fsNAME		= '借調媒體檔', 
					@fsHEAD		= CONVERT(char(8), getdate(), 112), 
					@fsBODY		= '_', 
					@fsNO_L		= 5, 
					@BY			= @CREATED_BY

				/*還沒有此筆設定時先新增*/
				IF NOT EXISTS(SELECT * FROM tblNO WHERE (fsTYPE = @fsTYPE) AND (fsHEAD = @fsHEAD))
				BEGIN
					BEGIN TRY
						INSERT	tblNO
						SELECT	@fsTYPE, @fsNAME, @fsHEAD, @fsBODY, 0, @fsNO_L, GETDATE(), @BY, '1900/01/01', ''
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
					SET @strRESULT = CAST(@intNEW AS VARCHAR(100)) 
					
					/*修改回資料庫同時,檢查是否資料庫中的時間是比我取號時舊的資料*/
					UPDATE	tblNO
					SET		fsNO = @intNEW
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

				------SET @toFILE_NAME = @fsHEAD + @fsBODY + @strRESULT
					
				
				------/*===============  上面這一段要想辦法換掉  ===============*/
				
				------SET @WM_PATH = dbo.fnGET_MEDIA_PATH() + 'wm.png'	-- 2012/10/18
			
			    --START 2013/02/26 改為 使用正式的檔案路徑
				------START 暫時
			    ----  	IF (@TYPE='V')
			    ----  	BEGIN
				----  SET @FROM_FILE_PATH='C:\Test\XDCAM_TC.mxf'
				----  SET @FROM_FILE_EXTION='mxf'
				----END
				------ELSE IF (@TYPE='D')
				------BEGIN
				------   SET @FROM_FILE_PATH='C:\Test\test2.txt'
				------    SET @FROM_FILE_EXTION='txt'
				------END
				------END   暫時		
				--END 2013/02/26 改為 使用正式的檔案路徑
				
				SELECT
					fnWORK_ID	    = @WORK_ID,
			    	fsFILE_NAME     = @FILE_NAME,
					fsTYPE		    = @TYPE,
					fsFILE_NO		= @FILE_NO,
					fsPATH_TYPE	    = @PATH_TYPE,
					fsBEG_TIME		= @BEG_TIME,
					fsEND_TIME		= @END_TIME,
					fsFROM_FILE_PATH= @FROM_FILE_PATH,
					fsTO_FILE_PATH  = @TO_FILE_PATH,
					fsFROM_FILE_EXTION     = @FROM_FILE_EXTION,
					--START 2013/04/11 加上回傳低解路徑
					fsFROM_FILE_PATH_L  =  @fsFROM_FILE_PATH_L,
					--END   2013/04/11 加上回傳低解路徑
					--START 2013/04/22 加上回傳判斷是選擇高解格式或原始格式
					fsFILE_TYPE  =  @_sFILE_TYPE,
					--END 2013/04/22 加上回傳判斷是選擇高解格式或原始格式
					fdARC_VIDEO_CREATED_DATE =@ARC_VIDEO_CREATED_DATE,
				    --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				    fsTITLE                = @fsTITLE,
					fsCREATED_BY           = @fsCREATED_BY,
					fsREBOOKINGER          = @fsREBOOKINGER
					--END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
			END 
			IF  (@blGETOK IS NULL)
			BEGIN 
			  DECLARE @tbResult TABLE(
			                    fnWORK_ID			      BIGINT,
			                  	fsFILE_NAME               NVARCHAR(100),
								fsTYPE	                  VARCHAR(1),
								fsFILE_NO	              VARCHAR(16),
								fsPATH_TYPE		          VARCHAR(1),
								fsBEG_TIME	         	  VARCHAR(20),
								fsEND_TIME	              VARCHAR(20),
								fsFROM_FILE_PATH          NVARCHAR(600),
								fsTO_FILE_PATH            NVARCHAR(500),
								fsFROM_FILE_EXTION        NVARCHAR(100),
					            --START 2013/04/11 加上回傳低解路徑
								fsFROM_FILE_PATH_L        VARCHAR(600),
								--END   2013/04/11 加上回傳低解路徑
								--START 2013/04/22 加上回傳判斷是選擇高解格式或原始格式
								fsFILE_TYPE               CHAR(1),
								--END 2013/04/22 加上回傳判斷是選擇高解格式或原始格式
								fdARC_VIDEO_CREATED_DATE  DATETIME,
					     		--START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
								fsTITLE                   NVARCHAR(100),
								fsCREATED_BY              NVARCHAR(50),
								fsREBOOKINGER             NVARCHAR(50)
								--END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
								)
								
		       SELECT * FROM @tbResult
			END
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

