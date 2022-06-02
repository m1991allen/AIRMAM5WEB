
-- =============================================
-- 描述:	傳入V/A/P/D回傳註冊工作項中,優先順序高的未處理工作
-- 記錄:	<2013/01/21><Albert.Chen><新增本預存><複製[spGET_WORK_NAS_BY_TYPE]來修改>
-- 記錄:	<2013/01/24><Albert.Chen><修改本預存><會將Status為0開頭的再次重轉,目前預設調用15小時已上,還未轉檔的>
-- 記錄:	<2013/02/26><Albert.Chen><使用正式的檔案路徑>
-- 記錄:	<2013/04/11><Albert.Chen><加上回傳低解路徑,用來檢查影片的聲音檔是否有問題>
-- 記錄:	<2013/04/16><Albert.Chen><改為以主檔排序為主>
-- 記錄:	<2013/05/10><Albert.Chen><修改本預存><不要管卡住的了,因為現在有重設轉檔功能,所以就可能兩支AP都取到>
-- 記錄:	<2013/06/03><Albert.Chen><修改本預存><當取號過就不再取號>
-- 記錄:	<2013/09/03><Albert.Chen><修改本預存><放寬@PAREMETERS到300>
-- 記錄:	<2013/11/11><Albert.Chen><修改本預存><給錯誤訊息看的,新增回傳欄位:標題(fsTitle),調用者(fsCreated_By),重設調用者(fsRebookinger)>
-- ※此預存目前只允許調用的AP會用, 其他程式叫用會影響STATUS的變化
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_WORK_AVID_BY_TYPE]
	@TYPE VARCHAR(1)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
			DECLARE
				@WORK_ID		   BIGINT,
				--START 2013/09/03 Update By Albert 放寬到300
				--@PAREMETERS      NVARCHAR(200),
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
                @TO_INTERPLAY_PATH NVARCHAR(500),          --Interplay 的URI
                @FROM_FILE_EXTION  NVARCHAR(100),          --副檔名
                @FILE_NAME         NVARCHAR(100),          --檔名(用fsTitle)
                @IS_DUPLICATE      INT,                    --下面要判斷Duplicate用
                @STATUS_WK         VARCHAR(2),
                @_nMAX_NEED_TIME   INT,                    --用來設定大於幾小時的Status=01或02之類的被搶號,卻因為程式可能掛掉後,就不再轉檔的孤立資料
                @_sTYPE            CHAR(4),
				--START 2013/04/11 加上回傳低解路徑
				@fsFROM_FILE_PATH_L NVARCHAR(600),
				--END   2013/04/11 加上回傳低解路徑
		   		--START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				@fsTITLE            NVARCHAR(100),
				@fsCREATED_BY       NVARCHAR(50),
			    @fsREBOOKINGER      NVARCHAR(50)
			    --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
                
			SELECT @FILE_NO='',@PATH_TYPE='',@BEG_TIME='',@END_TIME='',@FROM_FILE_PATH='',@TO_FILE_PATH='',@FROM_FILE_EXTION='',@FILE_NAME='',@IS_DUPLICATE=0,@STATUS_WK=''
			       ,@_nMAX_NEED_TIME=15,@_sTYPE ='AVID',@fsFROM_FILE_PATH_L=''
				   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
					,@fsTITLE='',@fsCREATED_BY='',@fsREBOOKINGER=''
			       --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
			
			--先取得CheckInAAF或CheckIn的動作
	       /*取出待處理項(STATUS=0開頭的)中優先順序高的轉檔工作WORK_ID*/

	       DECLARE icur CURSOR FOR
				     SELECT	TOP 1 WK.fnWORK_ID,WK._ITEM_SET2,WK._ITEM_SET3,WK._ITEM_ID,WK.fsSTATUS
							FROM	tblWORK AS WK
							LEFT JOIN tbmBOOKING AS BK ON (BK.fnBOOKING_ID = WK.fnGROUP_ID)
								WHERE ((WK.fsSTATUS = '00' OR WK.fsSTATUS = '30') AND 	       --9開頭表示完成 >9的也已結束 <9表示未完成
						          (WK._ITEM_TYPE =@TYPE) AND (WK.fsTYPE = @_sTYPE) AND (BK.fsSTATUS LIKE '0%'))
						           --START 這邊是取出卡住的01或02繼續轉檔
								  OR
									  ((WK.fsSTATUS LIKE '0%' AND WK.fsSTATUS <> '00') AND (WK._ITEM_TYPE =@TYPE) AND (WK.fsTYPE = @_sTYPE)
									  --START 2013/05/10 Update By Albert  不要管卡住的了,因為現在有重設轉檔功能,所以就可能兩支AP都取到
									   )--AND DateDiff(hour,WK.[fdCREATED_DATE],GETDATE())>= @_nMAX_NEED_TIME)
								      --ENd   2013/05/10 Update By Albert  不要管卡住的了,因為現在有重設轉檔功能,所以就可能兩支AP都取到
								  --END   這邊是取出卡住的01或02繼續轉檔
							      
								  --(WK.fsSTATUS DESC代表 WK.fsSTATUS = '30'的先做,因為它是Duplicate檢查完)因為取的是卡住的,所以是ASC方式
								--START 2013/04/16改為以主檔排序為主
									--ORDER BY WK.fsPRIORITY ,WK.fsSTATUS DESC ,BK.fnORDER , BK.fnBOOKING_ID, WK.fnWORK_ID

							--ORDER BY BK.fnORDER, BK.fnBOOKING_ID, WK.fsPRIORITY, WK.fnWORK_ID 
							ORDER BY BK.fnORDER, WK.fsPRIORITY, WK.fsSTATUS DESC , BK.fnBOOKING_ID, WK.fnWORK_ID 
							    --END 2013/04/16改為以主檔排序為主
			OPEN icur
	    		FETCH NEXT FROM icur INTO @WORK_ID,@BEG_TIME,@END_TIME,@FILE_NO,@STATUS_WK
	    	CLOSE icur
	    	DEALLOCATE icur	
			
		    --START 若00可能是Duplicate的資料,就不能CheckInAAF或CheckIn,會由Duplicate那支拿去
		    --只有00狀態的才要檢查,並且排除30,原因是,他原本就已經走過Duplicate後,發現找不到該筆Mobid的連結已經不存在,所以就不用再看30是否符合Duplicate的條件
		    --01或02不檢查的原因是:Duplicate只收00的,若在下面被檔掉,那麼01或02永遠不會被再轉
		    IF (@TYPE='V' AND @STATUS_WK ='00' )
		    BEGIN
    		    --判斷-->不能拿到Duplicate的				
                SET @IS_DUPLICATE= (SELECT COUNT(*)  FROM tblwork 
								    WHERE  fsTYPE = @_sTYPE AND _ITEM_TYPE='V' AND
									       _ITEM_ID = @FILE_NO AND _ITEM_SET2 = @BEG_TIME AND _ITEM_SET3 = @END_TIME AND
									        ISNULL(_ITEM_SET1,'') <> '' AND fdCREATED_DATE >= DATEADD(DAY,-3,GETDATE()) AND fsSTATUS='90'
									)
						        
				IF (@IS_DUPLICATE > 0 )
					BEGIN
					     
		        		 SET @WORK_ID =-1
					END				
			END
			--END   有Duplicate的資料,就不能CheckInAAF或CheckIn		

			--START　2013/06/03 Add By Albert 當取號過就不再取號
		    SET @WORK_ID = ISNULL((	SELECT	TOP 1 WK.fnWORK_ID
			FROM	tblWORK AS WK
			LEFT JOIN tbmBOOKING AS BK ON (BK.fnBOOKING_ID = WK.fnGROUP_ID)
			WHERE	((WK.fsSTATUS = '00') AND (WK._ITEM_TYPE =@TYPE)	--9開頭表示完成 >9的也已結束 <9表示未完成
						AND (WK.fsTYPE = @_sTYPE) AND (BK.fsSTATUS LIKE '0%'))                  --代表01或02已被拿去用的
		    ORDER BY BK.fnORDER,WK.fsPRIORITY ,WK.fsSTATUS DESC , BK.fnBOOKING_ID, WK.fnWORK_ID),-1)
			--END  　2013/06/03 Add By Albert 當取號過就不再取號
												 
			IF(@WORK_ID <> -1)
			BEGIN
				SELECT
					@TYPE	 = dbo.fnGET_ITEM_BY_INDEX(fsPARAMETERS,0),
					@FILE_NO = dbo.fnGET_ITEM_BY_INDEX(fsPARAMETERS,1),
					@BEG_TIME		= CASE WHEN @TYPE='V' THEN _ITEM_SET2 ELSE NULL END,   --影才有剪輯區間
					@END_TIME		= CASE WHEN @TYPE='V' THEN _ITEM_SET3 ELSE NULL END    --影才有剪輯區間
			       --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
					,@fsCREATED_BY = fsCREATED_BY
					,@fsREBOOKINGER=_ITEM_SET4
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
					@PATH_TYPE	    = dbo.fnGET_ITEM_BY_INDEX(@PAREMETERS, 2)
									
				/*取出來源路徑檔案名稱*/
				
				SELECT @FROM_FILE_PATH = ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H]),
				       @FROM_FILE_EXTION= [fsFILE_TYPE_H],
				       @FILE_NAME=fsTITLE,
					   --START 2013/04/11 加上回傳低解路徑
					   @fsFROM_FILE_PATH_L =([fsFILE_PATH_L] + fsFILE_NO + '_L.' + [fsFILE_TYPE_L]),
				       --END   2013/04/11 加上回傳低解路徑
					   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
					   @fsTITLE =fsTITLE
			           --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				FROM tbmARC_VIDEO
				WHERE (@TYPE = 'V') AND (fsFILE_NO = @FILE_NO)		
	 
				SELECT @FROM_FILE_PATH = ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H]),
				       @FROM_FILE_EXTION= [fsFILE_TYPE_H],
				       @FILE_NAME=fsTITLE,
					   --START 2013/04/11 加上回傳低解路徑
					   @fsFROM_FILE_PATH_L =([fsFILE_PATH_L] + fsFILE_NO + '_L.' + [fsFILE_TYPE_L]),
				       --END   2013/04/11 加上回傳低解路徑
					   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
					   @fsTITLE =fsTITLE
			           --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				FROM tbmARC_AUDIO
				WHERE (@TYPE = 'A') AND (fsFILE_NO = @FILE_NO)
	 
				SELECT @FROM_FILE_PATH = ([fsFILE_PATH_H] + fsFILE_NO + '_H.' + [fsFILE_TYPE_H]),
				       @FROM_FILE_EXTION= [fsFILE_TYPE_H],
				       @FILE_NAME=fsTITLE,
					   --START 2013/04/11 加上回傳低解路徑
					   @fsFROM_FILE_PATH_L =([fsFILE_PATH_L] + fsFILE_NO + '_L.' + [fsFILE_TYPE_L]),
				       --END   2013/04/11 加上回傳低解路徑
					   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
					   @fsTITLE =fsTITLE
			           --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				FROM tbmARC_PHOTO
				WHERE (@TYPE = 'P') AND (fsFILE_NO = @FILE_NO)	
	 		
				SELECT @FROM_FILE_PATH = ([fsFILE_PATH_2] + fsFILE_NO + '_L.' + [fsFILE_TYPE_2]),
				       @FROM_FILE_EXTION= [fsFILE_TYPE_2],
				       @FILE_NAME=fsTITLE,
					   --START 2013/04/11 加上回傳低解路徑
					   @fsFROM_FILE_PATH_L =([fsFILE_PATH_2] + fsFILE_NO + '_L.' + [fsFILE_TYPE_2]),
				       --END   2013/04/11 加上回傳低解路徑
					   --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
					   @fsTITLE =fsTITLE
			           --END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				FROM tbmARC_DOC
				WHERE (@TYPE = 'D') AND (fsFILE_NO = @FILE_NO)		
				
				/*設定檔案目的地(先從tbzCODE.fsSET取得路徑)*/
				DECLARE @CODE_SET_PATH VARCHAR(50)
				--將原本的取BOOK011方式 
				----SELECT @CODE_SET_PATH=fsSET
				----FROM tbzCODE
				----WHERE fsCODE_ID='BOOK011' AND fsCODE=@PATH_TYPE	

				--START 2013/02/26 修改是否為[財經台]推向樓層的判斷式
                SELECT @CODE_SET_PATH=ISNULL([CODE_PATH].fsSET,'') FROM [tbmUSERS]              --員工部門代碼的樓層
				LEFT JOIN [tbzCODE] AS CODE_SET ON [tbmUSERS].fsDEPT_ID =[CODE_SET].fsCODE      --員工部門代碼
				LEFT JOIN [tbzCODE] AS CODE_PATH ON [CODE_SET].fsSET =[CODE_PATH].fsCODE        --員工部門代碼
				WHERE fsLOGIN_ID=@CREATED_BY AND [CODE_PATH].fsCODE_ID='BOOK011'

				----SET @fsPATH_TYPE=ISNULL(@fsPATH_TYPE,'')
				----SET @fsSTATUS   = CASE WHEN @fsPATH_TYPE='' THEN 'E4' ELSE @fsSTATUS END    --這邊是判斷若找不道路徑,就給E4:對應不到單位
				----SET @fsPROGRESS = CASE WHEN @fsPATH_TYPE='' THEN '0%' ELSE '' END     --這邊是判斷若找不道路徑,就給0%:對應不到單位
				--END   2013/02/26 修改是否為[財經台]推向樓層的判斷式


				/*===============  下面這一段要想辦法換掉  ===============*/
				DECLARE
					@fsTYPE		VARCHAR(10),
					@fsNAME		NVARCHAR(10),
					@fsHEAD		VARCHAR(10),
					@fsBODY		VARCHAR(10),
					@fsNO_L		INT,
					@BY			VARCHAR(50)
					
				SELECT
					@fsTYPE		= 'AVID', 
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

				/*
START 複製spGET_L_NO_NEW_NO的內容來改
注意:前題是資料要有值才加號
用意:防止AVID的BaseName過大
*/

DECLARE	@fsTYPE_GETNO		VARCHAR(10)= 'AVID',
    	@fsNAME_GETNO		NVARCHAR(10)= 'AVID的BaseName流水號',
    	@fsHEAD_GETNO		VARCHAR(10)=  '',  --用月日來組  '0223',
    	@fsBODY_GETNO		VARCHAR(10)='',
		@fsNO_L_GETNO		INT= 5,
    	@BY_GETNO	    	VARCHAR(50)= 'STD_AN_BK',
        @_sMONTH_GETNO      VARCHAR(2) = CONVERT(VARCHAR(2),DATEPART(MM,GETDATE())),  --組@fsHEAD用
        @_sDAY_GETNO        VARCHAR(2) = CONVERT(VARCHAR(2),DATEPART(DD,GETDATE())),  --組@fsHEAD用
		@fsNEW_NO           VARCHAR(9) =''

--START 組@fsHEAD用

SET @_sMONTH_GETNO = REPLICATE('0', (2-LEN(@_sMONTH_GETNO))) +  CONVERT(NVARCHAR, @_sMONTH_GETNO) 
SET @_sDAY_GETNO   = REPLICATE('0', (2-LEN(@_sDAY_GETNO))) +  CONVERT(NVARCHAR, @_sDAY_GETNO)
SET @fsHEAD_GETNO  = @_sMONTH_GETNO + @_sDAY_GETNO
--END   組@fsHEAD用

/*還沒有此筆設定時先新增*/
		IF NOT EXISTS(SELECT * FROM tblNO WHERE (fsTYPE = @fsTYPE_GETNO) AND (fsHEAD = @fsHEAD_GETNO))
		BEGIN
			BEGIN TRY
				INSERT	tblNO
				SELECT	@fsTYPE_GETNO, @fsNAME_GETNO, @fsHEAD_GETNO, @fsBODY_GETNO, 0, @fsNO_L_GETNO, GETDATE(), @BY_GETNO, '1900/01/01', ''
			END TRY
			BEGIN CATCH
			END CATCH
		END

		DECLARE	@strRESULT_GETNO	VARCHAR(100) = '',
				@intPRESENT_GETNO	INT,
				@intNEW_GETNO	INT,
				@blGETOK_GETNO	BIT = 0,
				@dateTIME_GETNO	DATETIME

		WHILE(@blGETOK_GETNO = 0)
		BEGIN
			SET @dateTIME_GETNO = GETDATE(); --先取出目前資料庫中的資料時間
			SET @intPRESENT_GETNO =  (SELECT fsNO FROM tblNO WHERE (fsTYPE = @fsTYPE_GETNO) AND (fsHEAD = @fsHEAD_GETNO))
			SET @intNEW_GETNO = @intPRESENT_GETNO + 1
				/*若是影音圖文,流水號再加一*/
				IF(@fsTYPE_GETNO='ARC')
				BEGIN
					SET @intNEW_GETNO = @intNEW_GETNO + 1
				END
			SET @strRESULT_GETNO = CAST(@intNEW_GETNO AS VARCHAR(100))
			
			--WHILE(LEN(@strRESULT) < @fsNO_L)	
			--BEGIN
			--	SET @strRESULT = '0' + @strRESULT --依照長度來補0
			--END	
			
			/*修改回資料庫同時,檢查是否資料庫中的時間是比我取號時舊的資料*/
			UPDATE	tblNO
			SET		fsNO = @intNEW_GETNO, fsUPDATED_BY = @BY_GETNO
			WHERE	(fsTYPE = @fsTYPE_GETNO) AND (fsHEAD = @fsHEAD_GETNO) AND 
					(fdCREATED_DATE <= @dateTIME_GETNO) AND (fdUPDATED_DATE <= @dateTIME_GETNO)	
			
			IF (@@ROWCOUNT > 0)
				BEGIN
					/*確實有修改到比較舊的資料時*/
					SET @blGETOK_GETNO = 1
				END
		END

		SET @strRESULT_GETNO = '00000000000000000000000000000000000000000000000000' + CAST(@intNEW_GETNO AS VARCHAR(50)) 
		SET @strRESULT_GETNO = SUBSTRING(@strRESULT_GETNO, LEN(@strRESULT_GETNO)-@fsNO_L_GETNO+1, @fsNO_L_GETNO) 

		SET @fsNEW_NO = @fsHEAD_GETNO + @fsBODY_GETNO + @strRESULT_GETNO
		--SELECT @fsNEW_NO
		--SELECT @fsNEW_NO
/*END 複製spGET_L_NO_NEW_NO的內容來改*/

				--START 20130223採用九碼流水號,並且採用現在時間當調用時間,防止可能很多天後,才進行轉檔後,但資料夾可能監控被Purge
				--SET @TO_FILE_PATH=dbo.fnGET_AVID_OUTPUT_PATH1(@CODE_SET_PATH, @WORK_ID ,@CREATED_BY, @CREATED_DATE)
				--SET @TO_INTERPLAY_PATH=dbo.fnGET_AVID_OUTPUT_PATH2(@WORK_ID ,@CREATED_BY, @CREATED_DATE) + @fsNEW_NO + '/'  --這個是Avid用
				SET @TO_FILE_PATH=dbo.fnGET_AVID_OUTPUT_PATH1(@CODE_SET_PATH, @fsNEW_NO ,@CREATED_BY, GETDATE())
				SET @TO_INTERPLAY_PATH=dbo.fnGET_AVID_OUTPUT_PATH2(@WORK_ID ,@CREATED_BY, GETDATE())   --這個是Avid用
				--END   20130223採用九碼流水號
				
				--START 2013/02/26 改為 使用正式的檔案路徑
			 ----    	--START 暫時
			 ----    	IF (@TYPE='V')
			 ----    	BEGIN
			 ----     SET @FROM_FILE_PATH='C:\Test\XDCAM_TC.mxf'
				----  SET @FROM_FILE_EXTION='mxf'
				----END
				------ELSE IF (@TYPE='D')
				------BEGIN
				------   SET @FROM_FILE_PATH='C:\Test\test.txt'
				------    SET @FROM_FILE_EXTION='txt'
				------END
				------END   暫時	
				--END 2013/02/26 改為 使用正式的檔案路徑
				--SET @TO_FILE_PATH='C:\\Test\'
				
				SELECT
					fnWORK_ID	           = @WORK_ID,
			    	fsFILE_NAME            = @FILE_NAME,
					fsTYPE		           = @TYPE,
					fsFILE_NO	           = @FILE_NO,
					fsPATH_TYPE	           = @PATH_TYPE,
					fsBEG_TIME		       = @BEG_TIME,
					fsEND_TIME		       = @END_TIME,
					fsFROM_FILE_PATH       = @FROM_FILE_PATH,    --'D:\\AlbertFile\\123.mxf', -- @FROM_FILE_PATH
					fsTO_FILE_PATH         = @TO_FILE_PATH,      --'D:\\AlbertFile\\123\\',  --@TO_FILE_PATH
					fsTO_INTERPLAY_PATH    = @TO_INTERPLAY_PATH,
					fsFROM_FILE_EXTION     = @FROM_FILE_EXTION,  --'mxf',  --@FROM_FILE_EXTION
					fsNEW_NO               = @fsNEW_NO,
					--START 2013/04/11 加上回傳低解路徑
					fsFROM_FILE_PATH_L     = @fsFROM_FILE_PATH_L,
					--END   2013/04/11 加上回傳低解路徑
		     		--START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
					fsTITLE                = @fsTITLE,
					fsCREATED_BY           = @fsCREATED_BY,
					fsREBOOKINGER          = @fsREBOOKINGER
					--END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
			END 
			IF  (@blGETOK IS NULL)
			BEGIN 
			  DECLARE @tbResult TABLE(
			                    fnWORK_ID			BIGINT,
			                  	fsFILE_NAME             NVARCHAR(100),
								fsTYPE	                VARCHAR(1),
								fsFILE_NO	            VARCHAR(16),
								fsPATH_TYPE		        VARCHAR(1),
								fsBEG_TIME	         	VARCHAR(20),
								fsEND_TIME	         	VARCHAR(20),
								fsFROM_FILE_PATH        NVARCHAR(600),
								fsTO_FILE_PATH          NVARCHAR(500),
								fsTO_INTERPLAY_PATH     NVARCHAR(500),
								fsFROM_FILE_EXTION      NVARCHAR(100),
								fsNEW_NO                VARCHAR(9),
					            --START 2013/04/11 加上回傳低解路徑
								fsFROM_FILE_PATH_L      VARCHAR(600),
								--END   2013/04/11 加上回傳低解路徑
								--START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
								fsTITLE                 NVARCHAR(100),
								fsCREATED_BY            NVARCHAR(50),
								fsREBOOKINGER           NVARCHAR(50)
								--END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
								)
								
		       SELECT * FROM @tbResult
			END
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

