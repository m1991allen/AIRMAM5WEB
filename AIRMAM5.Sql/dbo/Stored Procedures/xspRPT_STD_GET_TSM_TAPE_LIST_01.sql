
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: Dennis.Wen
-- Description:	2013/10/29
-- =============================================
CREATE PROCEDURE [dbo].[xspRPT_STD_GET_TSM_TAPE_LIST_01]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [磁帶名稱]
		  ,[使用狀態]
		  --,[讀寫狀態]
		  ,[已存放資料量]
		  --,[儲存池]
		  ,[最後讀取]
		  ,[最後寫入]
		  ,[讀取錯誤]
		  ,[寫入錯誤]
		  ,[磁帶位置]
		  ,_TITLE =(CASE SUBSTRING([磁帶名稱],1,2)	WHEN 'BL' THEN '低解檔B帶'
													WHEN 'CL' THEN '低解檔C帶'
													WHEN 'BS' THEN '原始檔B帶'
													WHEN 'CS' THEN '原始檔C帶'
													WHEN 'TD' THEN 'TSMDB備份'
													WHEN 'MD' THEN 'MAMDB備份' ELSE '' END)
		  ,_TYPE = (CASE SUBSTRING([磁帶名稱],1,2)	WHEN 'BL' THEN '1'
													WHEN 'CL' THEN '2'
													WHEN 'BS' THEN '3'
													WHEN 'CS' THEN '4'
													WHEN 'TD' THEN '5'
													WHEN 'MD' THEN '6' ELSE '' END)
		  ,_LOCA = (CASE [磁帶位置]	WHEN '已下架'	THEN '1'
									WHEN '架上'		THEN '2' ELSE '' END)
		  ,_STAT = (CASE [使用狀態]	WHEN 'FULL'		THEN '1'
									WHEN 'FILLING'	THEN '2'
									WHEN 'EMPTY'	THEN '3' ELSE '' END)
	  FROM [dbo].[tbtVOLUME]
	  ORDER BY _TYPE, _LOCA, _STAT, [磁帶名稱]
END


