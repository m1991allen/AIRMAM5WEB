﻿

-- =============================================
-- 描述:	新增 ARC_VIDEO 入庫項目-影片檔主檔 資料
-- 記錄:	<2011/09/15><Eric.Huang><新增本預存>
-- 記錄:	<2011/09/16><Eric.Huang><將50個自訂欄位拿掉>
-- 記錄:	<2011/11/17><Eric.Huang><新增欄位>
--			<2012/05/21><Dennis.Wen><一堆欄位調整>
--			<2013/08/22><Albert.Chen><修改fsTYPE由varchar(1)改為varchar(10)>
--			<2014/04/15><Albert.Chen><調整欄位fsDESCRIPTION長度為NVARCHAR(MAX)>
--			<2014/08/21><Eric.Huang><新增 fsKEYWORD/fsOTHINFO/fsOTHTYPE1/fsOTHTYPE2/fsOTHTYPE3>
--			<2016/11/15><David.Sin><調整欄位>
--			<2019/05/30><David.Sin><增加參數fnPRE_ID，若大於0則使用預編詮釋資料>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_ARC_VIDEO]

	@fsFILE_NO			VARCHAR(16),
	@fsTITLE			NVARCHAR(100) = '',
	@fsDESCRIPTION		NVARCHAR(MAX) = '',
	@fsSUBJECT_ID		VARCHAR(12),
	@fsFILE_STATUS		CHAR(1),
	@fnFILE_SECRET		SMALLINT = 0,
	@fsFILE_TYPE		VARCHAR(10) = '',	
	@fsFILE_TYPE_H		VARCHAR(10) = '',
	@fsFILE_TYPE_L		VARCHAR(10) = '',
	@fsFILE_SIZE		VARCHAR(50) = '',
	@fsFILE_SIZE_H		VARCHAR(50) = '',
	@fsFILE_SIZE_L		VARCHAR(50) = '',
	@fsFILE_PATH		NVARCHAR(100) = '',			
	@fsFILE_PATH_H		NVARCHAR(100) = '',
	@fsFILE_PATH_L		NVARCHAR(100) = '',
	@fxMEDIA_INFO		NVARCHAR(MAX) = '',
	@fdBEG_TIME			DECIMAL(13,3) = 0,
	@fdEND_TIME			DECIMAL(13,3) = 0,
	@fdDURATION			DECIMAL(13,3) = 0,
	@fsRESOL_TAG		VARCHAR(2),
	@fnPRE_ID			BIGINT = 0,
	@fsCREATED_BY		VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		--過濾特殊字元，以免調用時候檔案rename有問題
		SET @fsTITLE = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@fsTITLE,'<',''),'>',''),':',''),'"',''),'/',''),'\',''),'?',''),'|',''),'*','')

		IF(@fnPRE_ID = 0)
		BEGIN
			INSERT
				tbmARC_VIDEO
				(fsFILE_NO,fsTITLE, fsDESCRIPTION,fsSUBJECT_ID,fsFILE_STATUS,fnFILE_SECRET,fsFILE_TYPE,fsFILE_TYPE_H,
					fsFILE_TYPE_L,fsFILE_SIZE,fsFILE_SIZE_H, fsFILE_SIZE_L, fsFILE_PATH,fsFILE_PATH_H,	fsFILE_PATH_L, 
					fxMEDIA_INFO,fsHEAD_FRAME,fdCREATED_DATE, fsCREATED_BY,fdBEG_TIME, fdEND_TIME, fdDURATION,fsRESOL_TAG)			 	 
			 
			VALUES
		
				(@fsFILE_NO,@fsTITLE, @fsDESCRIPTION,@fsSUBJECT_ID,@fsFILE_STATUS,@fnFILE_SECRET,@fsFILE_TYPE,@fsFILE_TYPE_H,
					@fsFILE_TYPE_L,@fsFILE_SIZE,@fsFILE_SIZE_H, @fsFILE_SIZE_L, @fsFILE_PATH,@fsFILE_PATH_H,	@fsFILE_PATH_L, 
					@fxMEDIA_INFO,'', GETDATE(), @fsCREATED_BY,@fdBEG_TIME, @fdEND_TIME, @fdDURATION, @fsRESOL_TAG)

		END
		ELSE
		BEGIN
			--判斷此預編用的樣板是否與此聲音樣板相同
			IF((SELECT [fnTEMP_ID_VIDEO] FROM [dbo].[tbmDIRECTORIES] WHERE fnDIR_ID = (SELECT fnDIR_ID FROM tbmSUBJECT WHERE fsSUBJ_ID = @fsSUBJECT_ID)) = 
				(SELECT fnTEMP_ID FROM tbmARC_PRE WHERE fnPRE_ID = @fnPRE_ID))
			BEGIN
				INSERT
					tbmARC_VIDEO
					(fsFILE_NO,fsTITLE, fsDESCRIPTION,fsSUBJECT_ID,fsFILE_STATUS,fnFILE_SECRET,fsFILE_TYPE,fsFILE_TYPE_H,
					 fsFILE_TYPE_L,fsFILE_SIZE,fsFILE_SIZE_H, fsFILE_SIZE_L, fsFILE_PATH,fsFILE_PATH_H,	fsFILE_PATH_L, 
					 fxMEDIA_INFO,fsHEAD_FRAME,fdCREATED_DATE, fsCREATED_BY,fdBEG_TIME, fdEND_TIME, fdDURATION,fsRESOL_TAG,
					 fsATTRIBUTE1,  fsATTRIBUTE2,  fsATTRIBUTE3,  fsATTRIBUTE4,  fsATTRIBUTE5,  fsATTRIBUTE6,
					 fsATTRIBUTE7,  fsATTRIBUTE8,  fsATTRIBUTE9,  fsATTRIBUTE10, fsATTRIBUTE11, fsATTRIBUTE12,
					 fsATTRIBUTE13, fsATTRIBUTE14, fsATTRIBUTE15, fsATTRIBUTE16, fsATTRIBUTE17, fsATTRIBUTE18,
					 fsATTRIBUTE19, fsATTRIBUTE20, fsATTRIBUTE21, fsATTRIBUTE22, fsATTRIBUTE23, fsATTRIBUTE24,
					 fsATTRIBUTE25, fsATTRIBUTE26, fsATTRIBUTE27, fsATTRIBUTE28, fsATTRIBUTE29, fsATTRIBUTE30,
					 fsATTRIBUTE31, fsATTRIBUTE32, fsATTRIBUTE33, fsATTRIBUTE34, fsATTRIBUTE35, fsATTRIBUTE36,
					 fsATTRIBUTE37, fsATTRIBUTE38, fsATTRIBUTE39, fsATTRIBUTE40, fsATTRIBUTE41, fsATTRIBUTE42,
					 fsATTRIBUTE43, fsATTRIBUTE44, fsATTRIBUTE45, fsATTRIBUTE46, fsATTRIBUTE47, fsATTRIBUTE48,
					 fsATTRIBUTE49, fsATTRIBUTE50, fsATTRIBUTE51, fsATTRIBUTE52, fsATTRIBUTE53, fsATTRIBUTE54, 
					 fsATTRIBUTE55, fsATTRIBUTE56, fsATTRIBUTE57, fsATTRIBUTE58, fsATTRIBUTE59, fsATTRIBUTE60)
				SELECT
					 @fsFILE_NO,fsTITLE, fsDESCRIPTION,@fsSUBJECT_ID,@fsFILE_STATUS,@fnFILE_SECRET,@fsFILE_TYPE,@fsFILE_TYPE_H,
					 @fsFILE_TYPE_L,@fsFILE_SIZE,@fsFILE_SIZE_H, @fsFILE_SIZE_L, @fsFILE_PATH,@fsFILE_PATH_H,	@fsFILE_PATH_L, 
					 @fxMEDIA_INFO,'', GETDATE(), @fsCREATED_BY,@fdBEG_TIME, @fdEND_TIME, @fdDURATION, @fsRESOL_TAG,
					 fsATTRIBUTE1,  fsATTRIBUTE2,  fsATTRIBUTE3,  fsATTRIBUTE4,  fsATTRIBUTE5,  fsATTRIBUTE6,
					 fsATTRIBUTE7,  fsATTRIBUTE8,  fsATTRIBUTE9,  fsATTRIBUTE10, fsATTRIBUTE11, fsATTRIBUTE12,
					 fsATTRIBUTE13, fsATTRIBUTE14, fsATTRIBUTE15, fsATTRIBUTE16, fsATTRIBUTE17, fsATTRIBUTE18,
					 fsATTRIBUTE19, fsATTRIBUTE20, fsATTRIBUTE21, fsATTRIBUTE22, fsATTRIBUTE23, fsATTRIBUTE24,
					 fsATTRIBUTE25, fsATTRIBUTE26, fsATTRIBUTE27, fsATTRIBUTE28, fsATTRIBUTE29, fsATTRIBUTE30,
					 fsATTRIBUTE31, fsATTRIBUTE32, fsATTRIBUTE33, fsATTRIBUTE34, fsATTRIBUTE35, fsATTRIBUTE36,
					 fsATTRIBUTE37, fsATTRIBUTE38, fsATTRIBUTE39, fsATTRIBUTE40, fsATTRIBUTE41, fsATTRIBUTE42,
					 fsATTRIBUTE43, fsATTRIBUTE44, fsATTRIBUTE45, fsATTRIBUTE46, fsATTRIBUTE47, fsATTRIBUTE48,
					 fsATTRIBUTE49, fsATTRIBUTE50, fsATTRIBUTE51, fsATTRIBUTE52, fsATTRIBUTE53, fsATTRIBUTE54, 
					 fsATTRIBUTE55, fsATTRIBUTE56, fsATTRIBUTE57, fsATTRIBUTE58, fsATTRIBUTE59, fsATTRIBUTE60
				FROM
					tbmARC_PRE
				WHERE
					fnPRE_ID = @fnPRE_ID

			END
			ELSE
			BEGIN
				
				SELECT RESULT = 'ERROR:此影片需要的樣板與預編詮釋資料的樣板不符'

			END
		END


		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



