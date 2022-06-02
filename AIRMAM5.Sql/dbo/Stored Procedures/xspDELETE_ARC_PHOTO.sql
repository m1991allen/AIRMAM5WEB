


-- =============================================
-- 描述:	刪除ARC_PHOTO 入庫項目-圖片檔 資料
-- 記錄:	<2011/09/15><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_ARC_PHOTO]
	@fsFILE_NO	VARCHAR(16)
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		IF((SELECT COUNT(1) FROM tblWORK WHERE _ITEM_ID = @fsFILE_NO AND fsSTATUS = '90') + 
			(SELECT COUNT(1) FROM tblWORK WHERE _ITEM_ID = @fsFILE_NO AND fsSTATUS LIKE 'E%') > 0)
		BEGIN

			BEGIN TRANSACTION

			DELETE
				tbmARC_PHOTO
			WHERE
				(fsFILE_NO = @fsFILE_NO)
		
			COMMIT
			
			SELECT RESULT = ''
		END
		ELSE
		BEGIN

			SELECT RESULT = 'ERROR:檔案處理中，無法刪除!'

		END	
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





