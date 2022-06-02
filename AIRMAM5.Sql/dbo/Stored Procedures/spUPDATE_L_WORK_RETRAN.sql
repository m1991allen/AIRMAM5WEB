

-- =============================================
-- 描述:	新增spUPDATE_RETRAN
-- 記錄:	<2019/03/12><David.Sin><新增預存>
--			<2019/09/12><Rachel.Chung><Modified: 修改回傳型態, int--> string>
--			<2019/09/16><David.Sin><重新整理>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_L_WORK_RETRAN]
	@fnWORK_ID		BIGINT,
	@fsCREATED_BY	VARCHAR(50)
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
			
			BEGIN TRANSACTION

			UPDATE
				tblWORK
			SET
				fsSTATUS = '00',
				fsPROGRESS = '0',
				fdUPDATED_DATE = GETDATE(),
				fsUPDATED_BY = @fsCREATED_BY
			WHERE
				fnWORK_ID = @fnWORK_ID

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
					fsFILE_TYPE		= '',
					fsFILE_SIZE		= '',
					fsFILE_PATH 	 = '',
					fdUPDATED_DATE	 = '1900-01-01',
					fsUPDATED_BY	 = '',
					fsCONTENT        = ''

				WHERE
					(fsFILE_NO = @FILE_NO)		
			END

			COMMIT

			SELECT 'Y' AS RESULT
		END
		ELSE
		BEGIN
			SELECT 'N' AS RESULT
		END
   
 
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END
