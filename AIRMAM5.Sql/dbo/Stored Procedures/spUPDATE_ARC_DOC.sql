



-- =============================================
-- 描述:	修改 ARC_DOC 入庫項目-文件檔 資料
-- 記錄:	<2011/09/15><Eric.Huang><新增本預存>
--      	<2011/11/17><Eric.Huang><新增欄位>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_ARC_DOC]

	@fsFILE_NO				VARCHAR(16),
	@fsTITLE				NVARCHAR(100),
	@fsDESCRIPTION			NVARCHAR(MAX) = '',
	@fsATTRIBUTE1			NVARCHAR(MAX) = '',
	@fsATTRIBUTE2			NVARCHAR(MAX) = '',
	@fsATTRIBUTE3			NVARCHAR(MAX) = '',
	@fsATTRIBUTE4			NVARCHAR(MAX) = '',
	@fsATTRIBUTE5			NVARCHAR(MAX) = '',
	@fsATTRIBUTE6			NVARCHAR(MAX) = '',
	@fsATTRIBUTE7			NVARCHAR(MAX) = '',
	@fsATTRIBUTE8			NVARCHAR(MAX) = '',
	@fsATTRIBUTE9			NVARCHAR(MAX) = '',
	@fsATTRIBUTE10			NVARCHAR(MAX) = '',
	@fsATTRIBUTE11			NVARCHAR(MAX) = '',
	@fsATTRIBUTE12			NVARCHAR(MAX) = '',
	@fsATTRIBUTE13			NVARCHAR(MAX) = '',
	@fsATTRIBUTE14			NVARCHAR(MAX) = '',
	@fsATTRIBUTE15			NVARCHAR(MAX) = '',
	@fsATTRIBUTE16			NVARCHAR(MAX) = '',
	@fsATTRIBUTE17			NVARCHAR(MAX) = '',
	@fsATTRIBUTE18			NVARCHAR(MAX) = '',
	@fsATTRIBUTE19			NVARCHAR(MAX) = '',
	@fsATTRIBUTE20			NVARCHAR(MAX) = '',
	@fsATTRIBUTE21			NVARCHAR(MAX) = '',
	@fsATTRIBUTE22			NVARCHAR(MAX) = '',
	@fsATTRIBUTE23			NVARCHAR(MAX) = '',
	@fsATTRIBUTE24			NVARCHAR(MAX) = '',
	@fsATTRIBUTE25			NVARCHAR(MAX) = '',
	@fsATTRIBUTE26			NVARCHAR(MAX) = '',
	@fsATTRIBUTE27			NVARCHAR(MAX) = '',
	@fsATTRIBUTE28			NVARCHAR(MAX) = '',
	@fsATTRIBUTE29			NVARCHAR(MAX) = '',
	@fsATTRIBUTE30			NVARCHAR(MAX) = '',
	@fsATTRIBUTE31			NVARCHAR(MAX) = '',
	@fsATTRIBUTE32			NVARCHAR(MAX) = '',
	@fsATTRIBUTE33			NVARCHAR(MAX) = '',
	@fsATTRIBUTE34			NVARCHAR(MAX) = '',
	@fsATTRIBUTE35			NVARCHAR(MAX) = '',
	@fsATTRIBUTE36			NVARCHAR(MAX) = '',
	@fsATTRIBUTE37			NVARCHAR(MAX) = '',
	@fsATTRIBUTE38			NVARCHAR(MAX) = '',
	@fsATTRIBUTE39			NVARCHAR(MAX) = '',
	@fsATTRIBUTE40			NVARCHAR(MAX) = '',
	@fsATTRIBUTE41			NVARCHAR(MAX) = '',
	@fsATTRIBUTE42			NVARCHAR(MAX) = '',
	@fsATTRIBUTE43			NVARCHAR(MAX) = '',
	@fsATTRIBUTE44			NVARCHAR(MAX) = '',
	@fsATTRIBUTE45			NVARCHAR(MAX) = '',
	@fsATTRIBUTE46			NVARCHAR(MAX) = '',
	@fsATTRIBUTE47			NVARCHAR(MAX) = '',
	@fsATTRIBUTE48			NVARCHAR(MAX) = '',
	@fsATTRIBUTE49			NVARCHAR(MAX) = '',
	@fsATTRIBUTE50			NVARCHAR(MAX) = '',
	@fsATTRIBUTE51			NVARCHAR(MAX) = '',
	@fsATTRIBUTE52			NVARCHAR(MAX) = '',
	@fsATTRIBUTE53			NVARCHAR(MAX) = '',
	@fsATTRIBUTE54			NVARCHAR(MAX) = '',
	@fsATTRIBUTE55			NVARCHAR(MAX) = '',
	@fsATTRIBUTE56			NVARCHAR(MAX) = '',
	@fsATTRIBUTE57			NVARCHAR(MAX) = '',
	@fsATTRIBUTE58			NVARCHAR(MAX) = '',
	@fsATTRIBUTE59			NVARCHAR(MAX) = '',
	@fsATTRIBUTE60			NVARCHAR(MAX) = '',
	@fsUPDATED_BY		VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		--過濾特殊字元，以免調用時候檔案rename有問題
		SET @fsTITLE = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@fsTITLE,'<',''),'>',''),':',''),'"',''),'/',''),'\',''),'?',''),'|',''),'*','')

		UPDATE
			tbmARC_DOC
		SET
			fsTITLE		     = @fsTITLE,
            fsDESCRIPTION    = @fsDESCRIPTION,
            fsATTRIBUTE1     = @fsATTRIBUTE1,
            fsATTRIBUTE2     = @fsATTRIBUTE2,
            fsATTRIBUTE3     = @fsATTRIBUTE3,
            fsATTRIBUTE4     = @fsATTRIBUTE4,
            fsATTRIBUTE5     = @fsATTRIBUTE5,
            fsATTRIBUTE6     = @fsATTRIBUTE6,
            fsATTRIBUTE7     = @fsATTRIBUTE7,
            fsATTRIBUTE8     = @fsATTRIBUTE8,
            fsATTRIBUTE9     = @fsATTRIBUTE9,
            fsATTRIBUTE10    = @fsATTRIBUTE10,
            fsATTRIBUTE11    = @fsATTRIBUTE11,
            fsATTRIBUTE12    = @fsATTRIBUTE12,
            fsATTRIBUTE13    = @fsATTRIBUTE13,
            fsATTRIBUTE14    = @fsATTRIBUTE14,
            fsATTRIBUTE15    = @fsATTRIBUTE15,
            fsATTRIBUTE16    = @fsATTRIBUTE16,
            fsATTRIBUTE17    = @fsATTRIBUTE17,
            fsATTRIBUTE18    = @fsATTRIBUTE18,
            fsATTRIBUTE19    = @fsATTRIBUTE19,
            fsATTRIBUTE20    = @fsATTRIBUTE20,
            fsATTRIBUTE21    = @fsATTRIBUTE21,
            fsATTRIBUTE22    = @fsATTRIBUTE22,
            fsATTRIBUTE23    = @fsATTRIBUTE23,
            fsATTRIBUTE24    = @fsATTRIBUTE24,
            fsATTRIBUTE25    = @fsATTRIBUTE25,
            fsATTRIBUTE26    = @fsATTRIBUTE26,
            fsATTRIBUTE27    = @fsATTRIBUTE27,
            fsATTRIBUTE28    = @fsATTRIBUTE28,
            fsATTRIBUTE29    = @fsATTRIBUTE29,
            fsATTRIBUTE30    = @fsATTRIBUTE30,
            fsATTRIBUTE31    = @fsATTRIBUTE31,
            fsATTRIBUTE32    = @fsATTRIBUTE32,
            fsATTRIBUTE33    = @fsATTRIBUTE33,
            fsATTRIBUTE34    = @fsATTRIBUTE34,
            fsATTRIBUTE35    = @fsATTRIBUTE35,
            fsATTRIBUTE36    = @fsATTRIBUTE36,
            fsATTRIBUTE37    = @fsATTRIBUTE37,
            fsATTRIBUTE38    = @fsATTRIBUTE38,
            fsATTRIBUTE39    = @fsATTRIBUTE39,
            fsATTRIBUTE40    = @fsATTRIBUTE40,
            fsATTRIBUTE41    = @fsATTRIBUTE41,
            fsATTRIBUTE42    = @fsATTRIBUTE42,
            fsATTRIBUTE43    = @fsATTRIBUTE43,
            fsATTRIBUTE44    = @fsATTRIBUTE44,
            fsATTRIBUTE45    = @fsATTRIBUTE45,
            fsATTRIBUTE46    = @fsATTRIBUTE46,
            fsATTRIBUTE47    = @fsATTRIBUTE47,
            fsATTRIBUTE48    = @fsATTRIBUTE48,
            fsATTRIBUTE49    = @fsATTRIBUTE49,
            fsATTRIBUTE50    = @fsATTRIBUTE50,
			fsATTRIBUTE51    = @fsATTRIBUTE51,
            fsATTRIBUTE52    = @fsATTRIBUTE52,
            fsATTRIBUTE53    = @fsATTRIBUTE53,
            fsATTRIBUTE54    = @fsATTRIBUTE54,
            fsATTRIBUTE55    = @fsATTRIBUTE55,
            fsATTRIBUTE56    = @fsATTRIBUTE56,
            fsATTRIBUTE57    = @fsATTRIBUTE57,
            fsATTRIBUTE58    = @fsATTRIBUTE58,
            fsATTRIBUTE59    = @fsATTRIBUTE59,
            fsATTRIBUTE60    = @fsATTRIBUTE60,
			fdUPDATED_DATE 	 = GETDATE(),
			fsUPDATED_BY	 = @fsUPDATED_BY
		WHERE
			(fsFILE_NO = @fsFILE_NO)
			
		COMMIT

		SELECT RESULT = ''
	END TRY
	
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





