



-- =============================================
-- 描述:	新增spUPDATE_L_WORK_PRIORITY
-- 記錄:	<2019/05/09><David.Sin><新增預存>
--      <2019/09/12><Rachel.Chung><Modified: 修改回傳型態, int--> string>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_L_WORK_PRIORITY]
	@fnWORK_ID			BIGINT,
	@fsPRIORITY			VARCHAR(1),
	@fsCREATED_BY		VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		UPDATE
			tblWORK
		SET
			fsPRIORITY		= @fsPRIORITY,
			fdUPDATED_DATE 	 = GETDATE(),
			fsUPDATED_BY	 = @fsCREATED_BY
		WHERE
			(fnWORK_ID = @fnWORK_ID)

		--SELECT RESULT = @@ROWCOUNT
		SELECT CONVERT(varchar(6), @@ROWCOUNT)

		COMMIT
		
		
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END
