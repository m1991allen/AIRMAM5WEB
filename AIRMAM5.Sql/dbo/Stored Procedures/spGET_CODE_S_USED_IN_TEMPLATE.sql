
-- =============================================
-- 描述:	判斷自訂代碼群組 是否有被使用在樣板裡
-- 記錄:	<2012/04/02><Mihsiu.Chiu><新增本預存>
--      	<2012/08/30><Mihsiu.Chiu><修改@fsCODE_ID => varchar(20)>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_CODE_S_USED_IN_TEMPLATE]
	@fsCODE_ID			VARCHAR(20)
AS
BEGIN	
	SELECT 
			RESULT = 
				CASE 
					WHEN COUNT(fnTEMP_ID) > 0 THEN 'Y' 
					ELSE 'N' 
				END 
		FROM 
			tbmTEMPLATE_FIELDS 
		WHERE 
			fsCODE_ID = @fsCODE_ID
END

