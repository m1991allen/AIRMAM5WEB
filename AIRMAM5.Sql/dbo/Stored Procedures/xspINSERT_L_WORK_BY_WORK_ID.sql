


-- =============================================
-- 描述:	新增 L_WORK_BY_PARAMETERS主檔資料
-- 記錄:	<2012/09/25><Albert.Chen><新增預存>
-- 記錄:	<2013/05/15><Eric.Huang><修改預存 fsTYPE 加入DAILY_ITP>
-- 記錄:	<2013/09/03><Albert.Chen><修改本預存><@fsPARAMETERS放寬到300>
-- 記錄:	<2016/09/07><David.Sin><修改本預存><直接傳入fnWORK_ID，再去查@fsPARAMETERS>
-- 記錄:	<2016/09/07><David.Sin><修改本預存><把高解欄位設定為空值取消，重轉只針對低解與KF>
-- 記錄:	<2019/03/12><David.Sin><修改本預存><重傳改成Update原有work，不新增work>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_L_WORK_BY_WORK_ID]
	@fnWORK_ID BIGINT,
	@fsCREATED_BY	varchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		DECLARE @fsPARAMETERS NVARCHAR(300) = (SELECT [fsPARAMETERS] FROM [dbo].[tblWORK] WHERE [fnWORK_ID] = @fnWORK_ID)

	    --fsTYPE是判斷影音圖文用
	    DECLARE @fsTYPE   char(1)= dbo.fnGET_ITEM_BY_INDEX(@fsPARAMETERS,0)
		DECLARE @FILE_NO  varchar(16) = dbo.fnGET_ITEM_BY_INDEX(@fsPARAMETERS,1)
		DECLARE @fsSTATUS varchar(2)
		
	  ----先判斷此筆資料是否正在轉檔
		SET @fsSTATUS=(SELECT  TOP 1 fsSTATUS 
		               FROM tblWORK 
	                   WHERE fnWORK_ID = @fnWORK_ID)
	  --在大於9的狀態才可以使用轉檔
	  IF (SUBSTRING(@fsSTATUS,1,1) = '9' OR SUBSTRING(@fsSTATUS,1,1) = 'E')
	     BEGIN
			UPDATE
				tblWORK
			SET
				fsSTATUS = '00',
				fsPROGRESS = '0',
				fdUPDATED_DATE = GETDATE(),
				fsUPDATED_BY = @fsCREATED_BY
			WHERE
				fnWORK_ID = @fnWORK_ID
			--INSERT
			--	tblWORK
			--	(fsTYPE, fsPARAMETERS,
			--	 fdCREATED_DATE, fsCREATED_BY)
			--VALUES
			--	('TRANSCODE', @fsPARAMETERS,
			--	GETDATE(), @fsCREATED_BY)
		
			-- 新增完畢時, 取回新增資料的識別編號,將值放到FILE_NO
			
			--Start進行更新動作
			IF (@fsTYPE = 'V')
			BEGIN
			
				-- 刪除該FILE_NO 的KEYFRAME
				DELETE
					tbmARC_VIDEO_K
				WHERE
					(fsFILE_NO = @FILE_NO)		
			
				-- UPDATE tbmARC_VIDEO TO 轉檔前的狀態
				UPDATE
					tbmARC_VIDEO
				SET
					fsFILE_TYPE_L    = '',
					fsFILE_SIZE_L    = '',
					fsFILE_PATH_L    = '',
					fdUPDATED_DATE	 = '1900-01-01',
					fsUPDATED_BY	 = '',
					fdBEG_TIME		 = 0 ,
					fdEND_TIME		 = 0 ,
					fdDURATION		 = 0 
				WHERE
					(fsFILE_NO = @FILE_NO)

			END
		
			ELSE IF (@fsTYPE = 'A')
				BEGIN
					-- UPDATE tbmARC_AUDIO TO 轉檔前的狀態
					UPDATE
						tbmARC_AUDIO
					SET
						fsFILE_TYPE_L    = '',    
						fsFILE_SIZE_L    = '',   
						fsFILE_PATH_L    = '',                       
       					fdUPDATED_DATE	 = '1900-01-01',
						fsUPDATED_BY	 = '',
						fdBEG_TIME		= 0  ,
						fdEND_TIME		= 0  ,
						fdDURATION		= 0  
					WHERE
						(fsFILE_NO = @FILE_NO)	
				END
					
			ELSE IF (@fsTYPE = 'P')
				BEGIN
					-- UPDATE tbmARC_PHOTO TO 轉檔前的狀態
					UPDATE
						tbmARC_PHOTO
					SET
						fsFILE_TYPE_L	= '',  
						fsFILE_SIZE_L	= '',  
						fsFILE_PATH_L	= '',
						fdUPDATED_DATE	= '1900-01-01',
						fsUPDATED_BY	= ''
					
					WHERE
						(fsFILE_NO = @FILE_NO)
				END
				
			ELSE IF (@fsTYPE = 'D')
				BEGIN
					-- UPDATE tbmARC_DOC TO 轉檔前的狀態(含EXTRACT欄位)
					UPDATE
						tbmARC_DOC
					SET
						fsFILE_TYPE_2	 = '',
						fsFILE_SIZE_2	 = '',
						fsFILE_PATH_2 	 = '',
						fdUPDATED_DATE	 = '1900-01-01',
						fsUPDATED_BY	 = '',
						fsCONTENT        = ''
					
					WHERE
						(fsFILE_NO = @FILE_NO)		
				END
			
			DECLARE @RESULT INT
			SET @RESULT = @@ROWCOUNT
			IF (@RESULT<>1)
			   BEGIN
				  --取不到媒體檔資訊取消排程
				  UPDATE tblWORK SET fsSTATUS='C1' WHERE fnWORK_ID=@@IDENTITY
			   END
			SELECT @RESULT	
         END
      --End更新動作
	  ELSE      -- 1~8不能轉
	     BEGIN
	       SELECT 'N'
	     END
    
		
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END
