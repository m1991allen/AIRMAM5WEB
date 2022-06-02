﻿


-- =============================================
-- 描述:	新增 ARC_PHOTO 入庫項目-圖片檔 資料
-- 記錄:	<2011/09/15><Eric.Huang><新增本預存>
--      	<2011/11/17><Eric.Huang><新增欄位>
--			<2019/05/30><David.Sin><增加參數fnPRE_ID，若大於0則使用預編詮釋資料>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_ARC_PHOTO]

	@fsFILE_NO			VARCHAR(16),
	@fsTITLE			NVARCHAR(100) = '',
	@fsDESCRIPTION		NVARCHAR(MAX) = '',
	@fsSUBJECT_ID		VARCHAR(12),
	@fsFILE_STATUS		VARCHAR(1),
	@fnFILE_SECRET		SMALLINT = 0,
	@fsFILE_TYPE		VARCHAR(10) = '',
	@fsFILE_TYPE_H		VARCHAR(10) = '',
	@fsFILE_TYPE_L		VARCHAR(10) = '',		
	@fsFILE_SIZE		NVARCHAR(50) = '',
	@fsFILE_SIZE_H		NVARCHAR(50) = '',
	@fsFILE_SIZE_L		NVARCHAR(50) = '',		
	@fsFILE_PATH		NVARCHAR(100) = '',
	@fsFILE_PATH_H		NVARCHAR(100) = '',
	@fsFILE_PATH_L		NVARCHAR(100) = '',	
	@fxMEDIA_INFO		NVARCHAR(MAX) = '',		
	@fnWIDTH			INT = 0,
	@fnHEIGHT			INT = 0,
	@fnXDPI				INT = 0,
	@fnYDPI				INT = 0,
	@fsCAMERA_MAKE		NVARCHAR(100) = '',
	@fsCAMERA_MODEL		NVARCHAR(100) = '',
	@fsFOCAL_LENGTH		NVARCHAR(100) = '',
	@fsEXPOSURE_TIME	NVARCHAR(100) = '',
	@fsAPERTURE			NVARCHAR(100) = '',
	@fnISO				INT = 0,
	@fnPRE_ID			BIGINT = 0,
	@fsATTRIBUTE1		NVARCHAR(MAX) = '',
	@fsATTRIBUTE2		NVARCHAR(MAX) = '',
	@fsATTRIBUTE3		NVARCHAR(MAX) = '',
	@fsATTRIBUTE4		NVARCHAR(MAX) = '',
	@fsATTRIBUTE5		NVARCHAR(MAX) = '',
	@fsATTRIBUTE6		NVARCHAR(MAX) = '',
	@fsATTRIBUTE7		NVARCHAR(MAX) = '',
	@fsATTRIBUTE8		NVARCHAR(MAX) = '',
	@fsATTRIBUTE9		NVARCHAR(MAX) = '',
	@fsATTRIBUTE10		NVARCHAR(MAX) = '',
	@fsATTRIBUTE11		NVARCHAR(MAX) = '',
	@fsATTRIBUTE12		NVARCHAR(MAX) = '',
	@fsATTRIBUTE13		NVARCHAR(MAX) = '',
	@fsATTRIBUTE14		NVARCHAR(MAX) = '',
	@fsATTRIBUTE15		NVARCHAR(MAX) = '',
	@fsATTRIBUTE16		NVARCHAR(MAX) = '',
	@fsATTRIBUTE17		NVARCHAR(MAX) = '',
	@fsATTRIBUTE18		NVARCHAR(MAX) = '',
	@fsATTRIBUTE19		NVARCHAR(MAX) = '',
	@fsATTRIBUTE20		NVARCHAR(MAX) = '',
	@fsATTRIBUTE21		NVARCHAR(MAX) = '',
	@fsATTRIBUTE22		NVARCHAR(MAX) = '',
	@fsATTRIBUTE23		NVARCHAR(MAX) = '',
	@fsATTRIBUTE24		NVARCHAR(MAX) = '',
	@fsATTRIBUTE25		NVARCHAR(MAX) = '',
	@fsATTRIBUTE26		NVARCHAR(MAX) = '',
	@fsATTRIBUTE27		NVARCHAR(MAX) = '',
	@fsATTRIBUTE28		NVARCHAR(MAX) = '',
	@fsATTRIBUTE29		NVARCHAR(MAX) = '',
	@fsATTRIBUTE30		NVARCHAR(MAX) = '',
	@fsATTRIBUTE31		NVARCHAR(MAX) = '',
	@fsATTRIBUTE32		NVARCHAR(MAX) = '',
	@fsATTRIBUTE33		NVARCHAR(MAX) = '',
	@fsATTRIBUTE34		NVARCHAR(MAX) = '',
	@fsATTRIBUTE35		NVARCHAR(MAX) = '',
	@fsATTRIBUTE36		NVARCHAR(MAX) = '',
	@fsATTRIBUTE37		NVARCHAR(MAX) = '',
	@fsATTRIBUTE38		NVARCHAR(MAX) = '',
	@fsATTRIBUTE39		NVARCHAR(MAX) = '',
	@fsATTRIBUTE40		NVARCHAR(MAX) = '',
	@fsATTRIBUTE41		NVARCHAR(MAX) = '',
	@fsATTRIBUTE42		NVARCHAR(MAX) = '',
	@fsATTRIBUTE43		NVARCHAR(MAX) = '',
	@fsATTRIBUTE44		NVARCHAR(MAX) = '',
	@fsATTRIBUTE45		NVARCHAR(MAX) = '',
	@fsATTRIBUTE46		NVARCHAR(MAX) = '',
	@fsATTRIBUTE47		NVARCHAR(MAX) = '',
	@fsATTRIBUTE48		NVARCHAR(MAX) = '',
	@fsATTRIBUTE49		NVARCHAR(MAX) = '',
	@fsATTRIBUTE50		NVARCHAR(MAX) = '',
	@fsATTRIBUTE51		NVARCHAR(MAX) = '',
	@fsATTRIBUTE52		NVARCHAR(MAX) = '',
	@fsATTRIBUTE53		NVARCHAR(MAX) = '',
	@fsATTRIBUTE54		NVARCHAR(MAX) = '',
	@fsATTRIBUTE55		NVARCHAR(MAX) = '',
	@fsATTRIBUTE56		NVARCHAR(MAX) = '',
	@fsATTRIBUTE57		NVARCHAR(MAX) = '',
	@fsATTRIBUTE58		NVARCHAR(MAX) = '',
	@fsATTRIBUTE59		NVARCHAR(MAX) = '',
	@fsATTRIBUTE60		NVARCHAR(MAX) = '',
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
			tbmARC_PHOTO
			(fsFILE_NO,     fsTITLE,	    fsDESCRIPTION,  fnFILE_SECRET,	 fsSUBJECT_ID,    fsFILE_STATUS,  fsFILE_TYPE ,  
			 fsFILE_TYPE_H, fsFILE_TYPE_L,  fsFILE_SIZE,    fsFILE_SIZE_H,   fsFILE_SIZE_L,  fsFILE_PATH ,  
			 fsFILE_PATH_H, fsFILE_PATH_L,  fxMEDIA_INFO,	fnWIDTH ,   	 fnHEIGHT,	     fnXDPI,	fnYDPI,	
			 fsCAMERA_MAKE, fsCAMERA_MODEL, fsFOCAL_LENGTH, fsEXPOSURE_TIME, fsAPERTURE,     fnISO, fdCREATED_DATE,  fsCREATED_BY,
			 fsATTRIBUTE1,  fsATTRIBUTE2,   fsATTRIBUTE3,   fsATTRIBUTE4,    fsATTRIBUTE5,   fsATTRIBUTE6,
			 fsATTRIBUTE7,  fsATTRIBUTE8,   fsATTRIBUTE9,   fsATTRIBUTE10,   fsATTRIBUTE11,  fsATTRIBUTE12,
			 fsATTRIBUTE13, fsATTRIBUTE14,  fsATTRIBUTE15,  fsATTRIBUTE16,   fsATTRIBUTE17,  fsATTRIBUTE18,
			 fsATTRIBUTE19, fsATTRIBUTE20,  fsATTRIBUTE21,  fsATTRIBUTE22,   fsATTRIBUTE23,  fsATTRIBUTE24,
			 fsATTRIBUTE25, fsATTRIBUTE26,  fsATTRIBUTE27,  fsATTRIBUTE28,   fsATTRIBUTE29,  fsATTRIBUTE30,
			 fsATTRIBUTE31, fsATTRIBUTE32,  fsATTRIBUTE33,  fsATTRIBUTE34,   fsATTRIBUTE35,  fsATTRIBUTE36,
			 fsATTRIBUTE37, fsATTRIBUTE38,  fsATTRIBUTE39,  fsATTRIBUTE40,   fsATTRIBUTE41,  fsATTRIBUTE42,
			 fsATTRIBUTE43, fsATTRIBUTE44,  fsATTRIBUTE45,  fsATTRIBUTE46,   fsATTRIBUTE47,  fsATTRIBUTE48,
			 fsATTRIBUTE49, fsATTRIBUTE50,  fsATTRIBUTE51,  fsATTRIBUTE52,	 fsATTRIBUTE53,  fsATTRIBUTE54,  
			 fsATTRIBUTE55, fsATTRIBUTE56,  fsATTRIBUTE57,  fsATTRIBUTE58,   fsATTRIBUTE59,  fsATTRIBUTE60)
			 
		VALUES
		
			(@fsFILE_NO,     @fsTITLE,        @fsDESCRIPTION,  @fnFILE_SECRET,		@fsSUBJECT_ID,		@fsFILE_STATUS, @fsFILE_TYPE ,  
			 @fsFILE_TYPE_H, @fsFILE_TYPE_L,  @fsFILE_SIZE,    @fsFILE_SIZE_H,		@fsFILE_SIZE_L, @fsFILE_PATH ,  
			 @fsFILE_PATH_H, @fsFILE_PATH_L,  @fxMEDIA_INFO,   @fnWIDTH ,   		@fnHEIGHT,	    @fnXDPI,	@fnYDPI,
			 @fsCAMERA_MAKE, @fsCAMERA_MODEL, @fsFOCAL_LENGTH, @fsEXPOSURE_TIME,	@fsAPERTURE,    @fnISO,		GETDATE(), @fsCREATED_BY,
			 @fsATTRIBUTE1,  @fsATTRIBUTE2,   @fsATTRIBUTE3,   @fsATTRIBUTE4,		@fsATTRIBUTE5,  @fsATTRIBUTE6,
			 @fsATTRIBUTE7,  @fsATTRIBUTE8,   @fsATTRIBUTE9,   @fsATTRIBUTE10,		@fsATTRIBUTE11, @fsATTRIBUTE12,
			 @fsATTRIBUTE13, @fsATTRIBUTE14,  @fsATTRIBUTE15,  @fsATTRIBUTE16,		@fsATTRIBUTE17, @fsATTRIBUTE18,
			 @fsATTRIBUTE19, @fsATTRIBUTE20,  @fsATTRIBUTE21,  @fsATTRIBUTE22,		@fsATTRIBUTE23, @fsATTRIBUTE24,
			 @fsATTRIBUTE25, @fsATTRIBUTE26,  @fsATTRIBUTE27,  @fsATTRIBUTE28,		@fsATTRIBUTE29, @fsATTRIBUTE30,
			 @fsATTRIBUTE31, @fsATTRIBUTE32,  @fsATTRIBUTE33,  @fsATTRIBUTE34,		@fsATTRIBUTE35, @fsATTRIBUTE36,
			 @fsATTRIBUTE37, @fsATTRIBUTE38,  @fsATTRIBUTE39,  @fsATTRIBUTE40,		@fsATTRIBUTE41, @fsATTRIBUTE42,
			 @fsATTRIBUTE43, @fsATTRIBUTE44,  @fsATTRIBUTE45,  @fsATTRIBUTE46,		@fsATTRIBUTE47, @fsATTRIBUTE48,
			 @fsATTRIBUTE49, @fsATTRIBUTE50,  @fsATTRIBUTE51,  @fsATTRIBUTE52,		@fsATTRIBUTE53, @fsATTRIBUTE54,  
			 @fsATTRIBUTE55, @fsATTRIBUTE56,  @fsATTRIBUTE57,  @fsATTRIBUTE58,		@fsATTRIBUTE59, @fsATTRIBUTE60)
		
		END
		ELSE
		BEGIN
			--判斷此預編用的樣板是否與此聲音樣板相同
			IF((SELECT [fnTEMP_ID_PHOTO] FROM [dbo].[tbmDIRECTORIES] WHERE fnDIR_ID = (SELECT fnDIR_ID FROM tbmSUBJECT WHERE fsSUBJ_ID = @fsSUBJECT_ID)) = 
				(SELECT fnTEMP_ID FROM tbmARC_PRE WHERE fnPRE_ID = @fnPRE_ID))
			BEGIN
				INSERT
					tbmARC_PHOTO
					(fsFILE_NO,     fsTITLE,	    fsDESCRIPTION,  fnFILE_SECRET,		fsSUBJECT_ID,    fsFILE_STATUS,  fsFILE_TYPE ,  
					 fsFILE_TYPE_H, fsFILE_TYPE_L,  fsFILE_SIZE,    fsFILE_SIZE_H,   fsFILE_SIZE_L,  fsFILE_PATH ,  
					 fsFILE_PATH_H, fsFILE_PATH_L,  fxMEDIA_INFO,	fnWIDTH ,   	 fnHEIGHT,	     fnXDPI,	fnYDPI,	
					 fsCAMERA_MAKE, fsCAMERA_MODEL, fsFOCAL_LENGTH, fsEXPOSURE_TIME, fsAPERTURE,     fnISO, fdCREATED_DATE,  fsCREATED_BY,
					 fsATTRIBUTE1,  fsATTRIBUTE2,   fsATTRIBUTE3,   fsATTRIBUTE4,    fsATTRIBUTE5,   fsATTRIBUTE6,
					 fsATTRIBUTE7,  fsATTRIBUTE8,   fsATTRIBUTE9,   fsATTRIBUTE10,   fsATTRIBUTE11,  fsATTRIBUTE12,
					 fsATTRIBUTE13, fsATTRIBUTE14,  fsATTRIBUTE15,  fsATTRIBUTE16,   fsATTRIBUTE17,  fsATTRIBUTE18,
					 fsATTRIBUTE19, fsATTRIBUTE20,  fsATTRIBUTE21,  fsATTRIBUTE22,   fsATTRIBUTE23,  fsATTRIBUTE24,
					 fsATTRIBUTE25, fsATTRIBUTE26,  fsATTRIBUTE27,  fsATTRIBUTE28,   fsATTRIBUTE29,  fsATTRIBUTE30,
					 fsATTRIBUTE31, fsATTRIBUTE32,  fsATTRIBUTE33,  fsATTRIBUTE34,   fsATTRIBUTE35,  fsATTRIBUTE36,
					 fsATTRIBUTE37, fsATTRIBUTE38,  fsATTRIBUTE39,  fsATTRIBUTE40,   fsATTRIBUTE41,  fsATTRIBUTE42,
					 fsATTRIBUTE43, fsATTRIBUTE44,  fsATTRIBUTE45,  fsATTRIBUTE46,   fsATTRIBUTE47,  fsATTRIBUTE48,
					 fsATTRIBUTE49, fsATTRIBUTE50,  fsATTRIBUTE51,  fsATTRIBUTE52,	 fsATTRIBUTE53,  fsATTRIBUTE54,  
					 fsATTRIBUTE55, fsATTRIBUTE56,  fsATTRIBUTE57,  fsATTRIBUTE58,   fsATTRIBUTE59,  fsATTRIBUTE60)
				SELECT
					 @fsFILE_NO,     fsTITLE,        fsDESCRIPTION,  0,		@fsSUBJECT_ID,		@fsFILE_STATUS, @fsFILE_TYPE ,  
					 @fsFILE_TYPE_H, @fsFILE_TYPE_L,  @fsFILE_SIZE,    @fsFILE_SIZE_H,		@fsFILE_SIZE_L, @fsFILE_PATH ,  
					 @fsFILE_PATH_H, @fsFILE_PATH_L,  @fxMEDIA_INFO,   @fnWIDTH ,   		@fnHEIGHT,	    @fnXDPI,	@fnYDPI,
					 @fsCAMERA_MAKE, @fsCAMERA_MODEL, @fsFOCAL_LENGTH, @fsEXPOSURE_TIME,	@fsAPERTURE,    @fnISO,		GETDATE(), @fsCREATED_BY,
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
				
				SELECT RESULT = 'ERROR:此圖片需要的樣板與預編詮釋資料的樣板不符'

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




