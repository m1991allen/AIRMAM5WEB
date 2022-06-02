
-- =============================================
-- 描述:	檢查AVID Duplicate
-- 記錄:	<2013/01/22><Albert.Chen><新增本預存><複製[spGET_WORK_NAS_BY_TYPE]來修改>
-- 記錄:	<2013/03/11><Albert.Chen><修改本預存><修改推送到AVID的要由_A改為_@ ,並且修改STD_AN_BK AP的流程(效能問題),修改的原因是在ISIS的檔案根本就不需要再經由"檢查磁帶的AP">
-- 記錄:	<2013/03/12><Albert.Chen><修改本預存><修改為每次在SP上跑5筆,所以也可能一次取回四筆要Duplicate的(Status=02),另一筆是不需要Duplicate(Status=00)>
-- 記錄:	<2013/04/16><Albert.Chen><改為以主檔排序為主>
-- 記錄:	<2013/11/11><Albert.Chen><修改本預存><給錯誤訊息看的,新增回傳欄位:標題(fsTitle),調用者(fsCreated_By),重設調用者(fsRebookinger)>
-- ※此預存目前只允許調用的AP會用

--判斷哪些要進行CheckInAAF動作
--			         1._ITEM_TYPE='V'       (必須是影片)
--			         2.Status=00
--				     3.取完後,更新Status=02 (代表Duplicate拿走)
--				     4.fsTYPE = 'AVID'

--Duplicate判斷條件：
--                   1.FileNo(同影片的意思)
--                   2.起迄時間(In,Out)點 =>_Item_Set2 ,_Item_Set3     Nvarchar(50)       <==存精準度xxx.yyy秒
--                   3.Mobid是否存在      =>_Item_Set1                 Nvarchar(255)      <==看都是68,給255
--                   4.三天內的
--                   5.狀態要成功(Status=90)

-- =============================================
CREATE PROCEDURE [dbo].[xspGET_WORK_AVID_BY_DUPLICATE]
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
			DECLARE
				@FILE_NO		      VARCHAR(16),	
				@BEG_TIME		      VARCHAR(20),
				@END_TIME		      VARCHAR(20),
				@InterplayFolderURI   NVARCHAR(500),
				@WORK_ID              BIGINT,
				@ISOK                 CHAR(1),
				@WK_STATUS            VARCHAR(2),
				@TITLE                NVARCHAR(100),
				@DO_WORK_COUNT        INT,
				@DO_WORK_MAX_COUNT    INT,
				--START 2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				@fsCREATED_BY         NVARCHAR(50),
			    @fsREBOOKINGER        NVARCHAR(50)
			    --END   2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)

			SELECT @FILE_NO='',@BEG_TIME='',@END_TIME='',@InterplayFolderURI='',@WORK_ID=-1,@ISOK=0,@WK_STATUS='',
			       @DO_WORK_COUNT=0,     --處理筆數的起點
			       @DO_WORK_MAX_COUNT=5  --設定每次處理五筆
		     	   --START 2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				   ,@fsCREATED_BY='',@fsREBOOKINGER=''
			       --END   2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
			
			
			--再去迴圈比對,若他是Duplicate的條件,就回傳
	    	DECLARE @tbResult TABLE(
	    	                         --fsMOB_ID NVARCHAR(255)
	    	                         fsInterplayAssetURI   NVARCHAR(600),
	    	                         fsInterplayFolderURI  NVARCHAR(500),
	    	                         fnWORK_ID             BIGINT,
									 fsTITLE               NVARCHAR(100),
							    --START 2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
								     fsFILE_NO	             VARCHAR(16),
							      	 fsCREATED_BY            NVARCHAR(50),
								     fsREBOOKINGER           NVARCHAR(50)
								--END   2013/11/11 加上 標題(fsTITLE),調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
	    	                       )
			WHILE (@DO_WORK_COUNT < @DO_WORK_MAX_COUNT)
			BEGIN	    
				
	    	DECLARE icur CURSOR FOR
 		    --取出要CheckInAAF的資料
 		    --優先順序高的先
			  --START 2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
			      --SELECT TOP 1 WK._ITEM_ID,WK._ITEM_SET2,WK._ITEM_SET3,dbo.fnGET_AVID_OUTPUT_PATH2(WK.fnWORK_ID ,WK.fsCREATED_BY, WK.fdCREATED_DATE) AS fsInterplayFolderURI,WK.fnWORK_ID,WK.fsSTATUS AS WK_STATUS,ARC_VDO.fsTITLE 
			    SELECT TOP 1 WK._ITEM_ID,WK._ITEM_SET2,WK._ITEM_SET3,dbo.fnGET_AVID_OUTPUT_PATH2(WK.fnWORK_ID ,WK.fsCREATED_BY, WK.fdCREATED_DATE) AS fsInterplayFolderURI,WK.fnWORK_ID,WK.fsSTATUS AS WK_STATUS,ARC_VDO.fsTITLE 
			                   ,WK.fsCREATED_BY ,WK._ITEM_SET4 
		      --END   2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
				FROM	tblWORK AS WK
				LEFT JOIN tbmBOOKING AS BK ON (BK.fnBOOKING_ID = WK.fnGROUP_ID)
				LEFT JOIN tbmARC_VIDEO AS ARC_VDO ON (ARC_VDO.fsFILE_NO = WK._ITEM_ID )
				WHERE (WK.fsSTATUS = '_@' OR WK.fsSTATUS = '00') AND (WK._ITEM_TYPE ='V')	          --9開頭表示完成 >9的也已結束 <9表示未完成
					  AND (WK.fsTYPE = 'AVID') AND (BK.fsSTATUS LIKE '0%')	 
			   --START 2013/04/16改為以主檔排序為主        
				--ORDER BY WK.fsSTATUS ,WK.fsPRIORITY ,BK.fnORDER, BK.fnBOOKING_ID, WK.fnWORK_ID        --第一個ORDER BY WK.fsSTATUS 會讓_@的先檢查,(若沒這樣做,則必須有人把00搶走才能動作,就會卡住_@的流程)
				  ORDER BY WK.fsSTATUS ,BK.fnORDER, WK.fsPRIORITY, BK.fnBOOKING_ID, WK.fnWORK_ID         --第一個ORDER BY WK.fsSTATUS 會讓_@的先檢查,(若沒這樣做,則必須有人把00搶走才能動作,就會卡住_@的流程)
	    	   --END 2013/04/16改為以主檔排序為主
	    		OPEN icur
			    	--START 2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
					  --FETCH NEXT FROM icur INTO @FILE_NO,@BEG_TIME,@END_TIME,@InterplayFolderURI,@WORK_ID,@WK_STATUS,@TITLE
					  FETCH NEXT FROM icur INTO @FILE_NO,@BEG_TIME,@END_TIME,@InterplayFolderURI,@WORK_ID,@WK_STATUS,@TITLE,@fsCREATED_BY,@fsREBOOKINGER
				    --END   2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
		    	    WHILE @@FETCH_STATUS=0
    			BEGIN
    			            SET @ISOK=0
							SET @ISOK =(SELECT TOP 1 '1' 
							           FROM tblwork 
									   WHERE  fsTYPE = 'AVID' AND _ITEM_TYPE='V' AND
									   _ITEM_ID = @FILE_NO AND _ITEM_SET2 = @BEG_TIME AND _ITEM_SET3 = @END_TIME AND
									   ISNULL(_ITEM_SET1,'') <> '' AND fdCREATED_DATE >= DATEADD(DAY,-3,GETDATE()) AND fsSTATUS='90'	
									   )	   
						  --有找到資料的才刷新
							IF (@ISOK ='1')
								BEGIN
								       --刷新狀態為Duplicate=02
									   UPDATE tblwork SET fsSTATUS='02' WHERE fnWORK_ID=@WORK_ID
								END		
							ELSE 
								BEGIN
								       IF (@WK_STATUS ='_@')
									      BEGIN
						     		           UPDATE tblwork SET fsSTATUS='_C' WHERE fnWORK_ID=@WORK_ID   --找不到Duplicate,就把Status=_C,回到檢查檔案在哪裡
									      END
								END

							IF (@@ROWCOUNT=1)  --代表有更新狀態才會進行Insert到要取值的地方(防止搶號)
							BEGIN
								--再倒入tbResult
			                 	--START 2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
    				    		   --INSERT INTO @tbResult(fsInterplayAssetURI,fsInterplayFolderURI,fnWORK_ID,fsTITLE) 
							       --SELECT TOP 1 dbo.fnGET_AVID_OUTPUT_PATH3(_ITEM_SET1) AS fsInterplayAssetURI,@InterplayFolderURI AS fsInterplayFolderURI,@WORK_ID AS fnWork_ID_NEW,@TITLE
								INSERT INTO @tbResult(fsInterplayAssetURI,fsInterplayFolderURI,fnWORK_ID,fsTITLE,fsFILE_NO,fsCREATED_BY,fsREBOOKINGER) 
    				    				   SELECT TOP 1 dbo.fnGET_AVID_OUTPUT_PATH3(_ITEM_SET1) AS fsInterplayAssetURI,@InterplayFolderURI AS fsInterplayFolderURI,@WORK_ID AS fnWork_ID_NEW,@TITLE,@FILE_NO,@fsCREATED_BY,@fsREBOOKINGER
								--START 2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
										   FROM tblwork 
										   WHERE  fsTYPE = 'AVID' AND _ITEM_TYPE='V' AND
										   _ITEM_ID = @FILE_NO AND _ITEM_SET2 = @BEG_TIME AND _ITEM_SET3 = @END_TIME AND
										   ISNULL(_ITEM_SET1,'') <> '' AND fdCREATED_DATE >= DATEADD(DAY,-3,GETDATE()) AND fsSTATUS='90'
							END	   
			       --START 2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
					  -- FETCH NEXT FROM icur INTO @FILE_NO,@BEG_TIME,@END_TIME,@InterplayFolderURI,@WORK_ID,@WK_STATUS,@TITLE
					  FETCH NEXT FROM icur INTO @FILE_NO,@BEG_TIME,@END_TIME,@InterplayFolderURI,@WORK_ID,@WK_STATUS,@TITLE,@fsCREATED_BY,@fsREBOOKINGER
				   --END   2013/11/11 加上 調用者(fsCREATED_BY),重設調用者(fsREBOOKINGER)
    			END
	    		CLOSE icur
	    		DEALLOCATE icur
				SET @DO_WORK_COUNT = @DO_WORK_COUNT + 1

			END

	        SELECT * FROM @tbResult
	    	
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

