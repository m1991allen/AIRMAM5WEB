


-- =============================================
-- 描述:	取出我的最愛資料
-- 記錄:	<2016/10/03><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_USERS_FAVORITE]
	@fsLOGIN_ID NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
		[fsLOGIN_ID],[fsFAVORITE] 
	FROM 
		[dbo].[tbmUSER_FAVORITE]
	WHERE 
		[fsLOGIN_ID] = @fsLOGIN_ID
END



