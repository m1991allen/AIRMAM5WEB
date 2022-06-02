

-- =============================================
-- 描述:	更新Work主檔資料
-- 記錄:	<2013/01/23><Albert.Chen><新增本預存>
--          <2013/02/18><Albert.Chen><修改本預存><spUPDATE_WORK_PRIORITY_BY_AVID_AND_NAS_AND_BOOKING_AND_COPYFILE更名為[spUPDATE_WORK_PRIORITY_BY_AVID_AND_NAS_AND_BOOKING_AND_COPYFILE_ADMIN]>
-- ※本預存僅供轉檔AP使用
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_WORK_PRIORITY_BY_AVID_AND_NAS_AND_BOOKING_AND_COPYFILE_ADMIN]
	@fnWORK_ID		bigint,	
	@fsPRIORITY		nvarchar(50),
	@fsUPDATED_BY	nvarchar(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SET NOCOUNT ON;
			
		UPDATE
			tblWORK
		SET
			fsPRIORITY		= @fsPRIORITY	,
			fsUPDATED_BY	= @fsUPDATED_BY ,
			fdUPDATED_DATE	= GETDATE()     
		WHERE
			(fnWORK_ID	= @fnWORK_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



