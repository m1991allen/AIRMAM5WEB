



-- =============================================
-- 描述:	修改T_USER_SYNC主檔資料
-- 記錄:	<2014/02/10><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_T_USER_SYNC]
	@fsFTV_EMAIL	VARCHAR(50),
	@fsEMP_NO		VARCHAR(10),
	@fsEMP_NAME		NVARCHAR(50),
	@fsEMP_GENDER	NVARCHAR(1),
	@fsEMP_DEPT		NVARCHAR(50),
	@fsEMP_SUFFIX	NVARCHAR(20)

AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN
			UPDATE
				tbt_USER_SYNC
			SET			
				fsEMP_NAME   = @fsEMP_NAME,
				fsEMP_GENDER = @fsEMP_GENDER,
				fsEMP_DEPT   = @fsEMP_DEPT,
				fsEMP_SUFFIX = @fsEMP_SUFFIX
				
			WHERE
				(fsEMP_NO = @fsEMP_NO)			
		END
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





