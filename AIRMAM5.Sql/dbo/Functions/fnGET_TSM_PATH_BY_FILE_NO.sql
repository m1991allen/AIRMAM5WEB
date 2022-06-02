
-- =============================================
-- 描述:	組合TSM路徑 FOR EBC
-- 記錄:	<2012/02/19><Eric.Huang><新增本函數>
-- 記錄:	<2013/09/14><Eric.Huang><修改本函數-分類為 EBC & EP>
-- =============================================
CREATE FUNCTION [dbo].[fnGET_TSM_PATH_BY_FILE_NO](
	@FILE_NO VARCHAR(16)
)
returns varchar(100)
as
begin   
	DECLARE @strRESULT VARCHAR(200) = ''
	DECLARE @TMP VARCHAR(200)

	SET @TMP = (SELECT fsFILE_PATH_H from dbo.tbmARC_VIDEO where fsFILE_NO = @FILE_NO) + @FILE_NO + '_H.' +(SELECT fsFILE_TYPE_H from dbo.tbmARC_VIDEO where fsFILE_NO = @FILE_NO)

	-- 2013/09/13 ++
	DECLARE @USER VARCHAR(20)
	SET @USER = ISNULL((SELECT TOP 1 fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'CUSTOMER_ID')),'')

	IF (@USER = 'EBC')
		BEGIN
			SET @TMP = REPLACE(@TMP,'\\MAM\MAMDFS\HVIDEO','')
			SET @TMP = REPLACE(@TMP,'\','/')
			SET @strRESULT = @TMP
		END
	ELSE IF (@USER = 'EP')
		BEGIN
			SET @TMP = REPLACE(@TMP,'\\172.30.101.51\HVIDEO','')
			SET @TMP = REPLACE(@TMP,'\','/')
			SET @strRESULT = @TMP
		END
	ELSE IF (@USER = 'FTV')
		BEGIN
			SET @TMP = REPLACE(@TMP,'\\AMS01\MAM_HVIDEO','')
			SET @TMP = REPLACE(@TMP,'\','/')
			SET @strRESULT = @TMP
		END

	return @strRESULT
end





