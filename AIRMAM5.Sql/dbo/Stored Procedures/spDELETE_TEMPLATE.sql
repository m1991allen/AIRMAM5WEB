﻿



-- =============================================
-- 描述:	刪除TEMPLATE 主檔 資料
-- 記錄:	<2011/09/16><Eric.Huang><新增本預存>
--      	<2011/10/17><Mihsiu.Chiu><修改本預存>
--      	<2016/09/20><David.Sin><修改本預存><判斷此樣版有無使用，修改自spGET_DIRECTORIES_BY_TEMP_ID>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_TEMPLATE]
	@fnTEMP_ID		INT,
	@fsDELETED_BY	VARCHAR(50)
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY

		--檢查是否有DIR使用此樣版

		DECLARE @fnTEMP_ID_SUBJECT bigint = 0
		DECLARE @fnTEMP_ID_VIDEO bigint = 0
		DECLARE @fnTEMP_ID_AUDIO bigint = 0
		DECLARE @fnTEMP_ID_PHOTO bigint = 0
		DECLARE @fnTEMP_ID_DOC bigint = 0

		DECLARE @fsTABLE CHAR(1) = (SELECT [fsTABLE] FROM [dbo].[tbmTEMPLATE] WHERE [fnTEMP_ID] = @fnTEMP_ID)

		IF (@fsTABLE = 'S') BEGIN SET @fnTEMP_ID_SUBJECT = @fnTEMP_ID END
		ELSE IF (@fsTABLE = 'V') BEGIN SET @fnTEMP_ID_VIDEO = @fnTEMP_ID END
		ELSE IF (@fsTABLE = 'A') BEGIN SET @fnTEMP_ID_AUDIO = @fnTEMP_ID END
		ELSE IF (@fsTABLE = 'P') BEGIN SET @fnTEMP_ID_PHOTO = @fnTEMP_ID END
		ELSE IF (@fsTABLE = 'D') BEGIN SET @fnTEMP_ID_DOC = @fnTEMP_ID END

		IF EXISTS(SELECT TOP 10 * FROM tbmDIRECTORIES WHERE
			((@fnTEMP_ID_SUBJECT = 0) or (fnTEMP_ID_SUBJECT = @fnTEMP_ID_SUBJECT)) and
			((@fnTEMP_ID_VIDEO = 0) or (fnTEMP_ID_VIDEO = @fnTEMP_ID_VIDEO)) and
			((@fnTEMP_ID_AUDIO = 0) or (fnTEMP_ID_AUDIO = @fnTEMP_ID_AUDIO)) and
			((@fnTEMP_ID_PHOTO = 0) or (fnTEMP_ID_PHOTO = @fnTEMP_ID_PHOTO)) and
			((@fnTEMP_ID_DOC = 0) or (fnTEMP_ID_DOC = @fnTEMP_ID_DOC)) 
		)
		BEGIN
			
			SELECT RESULT = 'ERROR:此樣板已被使用，不得刪除此樣板!'

		END
		ELSE IF EXISTS(SELECT TOP 1 * FROM tbmARC_PRE WHERE fnTEMP_ID = @fnTEMP_ID)
		BEGIN
			
			SELECT RESULT = 'ERROR:此樣板已有預編詮釋資料，不得刪除此樣板欄位!'

		END
		ELSE
		BEGIN
			BEGIN TRANSACTION

			DECLARE @context_info VARBINARY(128)
			SET @context_info = CAST(@fsDELETED_BY AS VARBINARY(128))
			SET CONTEXT_INFO @context_info

			DELETE
				tbmTEMPLATE
			WHERE
				(fnTEMP_ID = @fnTEMP_ID)
			
			COMMIT

			SELECT RESULT = ''
		END
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END






