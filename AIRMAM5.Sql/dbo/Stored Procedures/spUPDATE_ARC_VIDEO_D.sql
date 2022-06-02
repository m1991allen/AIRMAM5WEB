




-- =============================================
-- 描述:	修改 ARC_VIDEO_D 入庫項目-影片明細檔 資料
-- 記錄:	<2011/09/15><Eric.Huang><新增本預存>
--			<2011/11/09><Eric.Huang><增加一組KEY>
--      	<2011/11/17><Eric.Huang><新增欄位>
--			<2012/05/21><Dennis.Wen><一堆欄位調整>
-- 記錄:	<2014/04/15><Albert.Chen><調整欄位fsDESCRIPTION長度為NVARCHAR(MAX)>
-- 記錄:<2014/08/21><Eric.Huang><新增 fsKEYWORD>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_ARC_VIDEO_D]

	@fsFILE_NO			VARCHAR(16),
	@fnSEQ_NO			INT,
	@fsDESCRIPTION		NVARCHAR(MAX) = '',
	@fdBEG_TIME			DECIMAL(13,3),
	@fdEND_TIME			DECIMAL(13,3),
	@fsUPDATED_BY		VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		UPDATE
			tbmARC_VIDEO_D
		SET
			[fsDESCRIPTION] = @fsDESCRIPTION,
			[fdBEG_TIME] = @fdBEG_TIME,
			[fdEND_TIME] = @fdEND_TIME,
			[fdUPDATED_DATE] = GETDATE(),
			[fsUPDATED_BY] = @fsUPDATED_BY
		WHERE
			 [fsFILE_NO] = @fsFILE_NO AND [fnSEQ_NO] = @fnSEQ_NO
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END







