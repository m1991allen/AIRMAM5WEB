

-- =============================================
-- 描述:	修改USER_G主檔資料 BY Transaction
-- 記錄:	<2011/09/08><Eric.Huang><新增本預存>
-- 記錄:	<2011/09/09><Eric.Huang><修改本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_USER_GROUP_BY_TRANS]
	@fnUSER_ID		BIGINT ,	
	@GROUP_LIST		VARCHAR(500) ,
	@fsCREATED_BY	nvarchar(50)
AS

	BEGIN TRANSACTION;

	DECLARE @cnt INT, @idx INT, @i INT, @result NVARCHAR(4000), @errcnt int=0
	SELECT @cnt = 0, @idx = 0, @i = 0, @result = ''
 	SELECT @cnt = LEN(REPLACE(@GROUP_LIST,';',';;')) - LEN(@GROUP_LIST)
 	
 	--TODO: DELETE SETTINGS OF THIS @LOGIN_ID
	DELETE
			tbmUSER_GROUP
		WHERE
			(fnUSER_ID		= @fnUSER_ID )
	BEGIN TRY

	 	WHILE(@i < @cnt)
		
		BEGIN
			Declare @Code nvarchar(20)
			Declare @Name nvarchar(100)

			SELECT @Code = dbo.fnGET_ITEM_BY_INDEX(@GROUP_LIST,@i)
			
			--TODO: INSERT EACH SETTING OF THIS @LOGIN_ID		
			INSERT
			tbmUSER_GROUP
			(fnUSER_ID, fsGROUP_ID, fdCREATED_DATE, fsCREATED_BY)
			VALUES
			(@fnUSER_ID, @Code, GETDATE(), @fsCREATED_BY)
			
			SET @i = @i + 1
		END
	END TRY
	
	BEGIN CATCH
		SET @errcnt = @errcnt + 1
	END CATCH
	
	IF (@errcnt = 0)
		BEGIN
			COMMIT TRANSACTION;
			SELECT RESULT = @cnt;			
		END
	ELSE
		BEGIN
			ROLLBACK TRANSACTION;
			SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE();
		END


