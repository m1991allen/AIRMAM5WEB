

-- =============================================
-- 描述:	新增 ARC_VIDEO_D 入庫項目-影片明細檔 資料
-- 記錄:	<2011/09/15><Eric.Huang><新增本預存>
--    	<2012/05/21><Dennis.Wen><一堆欄位調整>
--      <2013/08/22><Albert.Chen><修改fsTYPE由varchar(1)改為varchar(10)>
--      <2013/10/21><Albert.Chen><加上seqno <> 0 的,都加上跟fnseqno=0一樣的fsTYPE 只有EP才這樣>
-- 記錄:	<2014/04/15><Albert.Chen><調整欄位fsDESCRIPTION長度為NVARCHAR(MAX)>
-- 記錄:<2014/08/21><Eric.Huang><新增 fsKEYWORD>
-- 記錄:<2016/11/15><David.Sin><調整欄位>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_ARC_VIDEO_D]
		
	@fsFILE_NO			VARCHAR(16),
	@fsDESCRIPTION		NVARCHAR(MAX),
	@fdBEG_TIME			DECIMAL(13,3),
	@fdEND_TIME			DECIMAL(13,3),
	@fsCREATED_BY		VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
	    
		BEGIN TRANSACTION

		DECLARE @fnSEQ_NO_T INT = (SELECT ISNULL(MAX(fnSEQ_NO),0) FROM tbmARC_VIDEO_D WHERE fsFILE_NO = @fsFILE_NO)


		INSERT
			tbmARC_VIDEO_D
			([fsFILE_NO],[fnSEQ_NO],[fsDESCRIPTION],[fdBEG_TIME],[fdEND_TIME],[fdCREATED_DATE],[fsCREATED_BY])
		VALUES
			(@fsFILE_NO, (@fnSEQ_NO_T + 1),@fsDESCRIPTION,@fdBEG_TIME,@fdEND_TIME,GETDATE(),@fsCREATED_BY)

		COMMIT

		SELECT RESULT = ''
		
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



