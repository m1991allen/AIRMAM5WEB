﻿


-- =============================================
-- 描述:	刪除SRH主檔資料
-- 記錄:	<2011/12/13><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_L_SRH]
	@fnSRH_ID		BIGINT
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tblSRH
		WHERE
			(fnSRH_ID		= @fnSRH_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





