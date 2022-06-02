
-- =============================================
-- 描述:	取出USERS主檔資料
-- 記錄:	<2011/09/05><Dennis.Wen><新增本預存>
--			<2012/01/17><Mihsiu.Chiu><新增 _sIS_ADMINS欄位>
--			<2012/04/05><Eric.Huang><新增INSERT LOGIN登入失敗的理由：帳號已停用>
-- 記錄:    <2012/09/06><Albert.Chen><新增_sIMAGEURL,目前先寫死路逕>
-- 記錄:    <2012/09/14><<Eric.Huang><_sIMAGE_URL 不串副檔名，由Service層去讀4種副檔名中，那一種存在>
-- 記錄:    <2012/09/14><<Eric.Huang><_sIMAGE_URL 不串副檔名，由Service層去讀4種副檔名中，那一種存在>
-- 記錄:	<2012/11/12><Eric.Huang><修改本函數 EBC USE>
-- 記錄:	<2013/03/19><Eric.Huang><因應縮短mam的網址,故修改_sIMAGE_URL>
-- 記錄:	<2013/04/02><Albert.Chen><加上使用者的群組是哪類>
-- 記錄:	<2014/01/10><Eric.Huang><加入LDAP判斷式>
-- 記錄:	<2019/05/23><David.Sin><重改程式>
-- 記錄:	<2019/05/23><David.Sin><登入成功時，刪除與新增使用者可使用的節點>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_USERS_BY_PASSWORD]
	@fsLOGIN_ID		NVARCHAR(128),
	@fsPASSWORD		VARCHAR(255),
	@IP				VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;
	
	BEGIN TRY	
		
		--清除tbmUSER_DIR
		DELETE FROM tbmUSER_DIR WHERE fsLOGIN_ID = @fsLOGIN_ID

		--判斷是否需要重設密碼
		IF((SELECT COUNT(1) FROM tbmUSERS WHERE fsLOGIN_ID = @fsLOGIN_ID AND fsPASSWORD = @fsLOGIN_ID AND fsIS_ACTIVE = 'Y') +
			(SELECT COUNT(1) FROM tbmUSERS WHERE fsLOGIN_ID = @fsLOGIN_ID AND fsPASSWORD = @fsPASSWORD AND fsIS_ACTIVE = 'Y') + 
			(SELECT COUNT(1) FROM tbmUSERS WHERE fsLOGIN_ID = @fsLOGIN_ID AND fsPASSWORD = '//' AND fsIS_ACTIVE = 'Y') > 0)	
		BEGIN
			
			INSERT tblLOGIN(fsLOGIN_ID,fdSTIME, fsCREATED_BY, fdCREATED_DATE, fsNOTE )
			VALUES (@fsLOGIN_ID, GETDATE(), @IP, GETDATE(), '操作系統中')

			--寫入使用者可使用的節點
			DECLARE @fsDIRs VARCHAR(MAX) = ''

			EXEC [dbo].[spGET_USER_DIR_AUTH_WITH_OUTPUT] @fsLOGIN_ID , @fsDIRs OUTPUT

			IF(LEN(ISNULL(@fsDIRs,'')) > 0) BEGIN INSERT INTO tbmUSER_DIR VALUES(@fsLOGIN_ID,@fsDIRs,GETDATE()) END

			SELECT 
				fsUSER_ID, fsLOGIN_ID, fsPASSWORD, USERS.fsNAME, fsENAME, fsTITLE, 
				fsDEPT_ID, D.fsNAME AS _sDEPTNAME, fsEMAIL, fsPHONE, fsDESCRIPTION,fsFILE_SECRET,
				fsIS_ACTIVE, fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY,
				dbo.fnGET_STRING_MATCH(dbo.fnGET_GROUPS_BY_USER_ID(fsUSER_ID), fsVALUE) AS _sIS_ADMINS,
				_sIMAGE_URL = ''
				,ISNULL(UG.fsGROUP_ID,'') AS _sGROUP_ID
			FROM
				tbmUSERS AS USERS
					LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'DEPT001') AS D ON (USERS.fsDEPT_ID = D.fsCODE)
					LEFT JOIN (SELECT fsGROUP_ID,fsUSER_ID AS _fnUSER_ID FROM tbmUSER_GROUP) AS UG ON (USERS.fsUSER_ID = UG._fnUSER_ID)
					CROSS JOIN (select fsVALUE from tbzCONFIG where fsKEY='ADMIN_GROUPS') as A
			WHERE
				(fsLOGIN_ID = @fsLOGIN_ID)	

		END
		ELSE
		BEGIN
			INSERT tblLOGIN(fsLOGIN_ID,[fdSTIME],fsNOTE,fdCREATED_DATE,fsCREATED_BY)
			VALUES (@fsLOGIN_ID, GETDATE(),'登入失敗:帳密輸入錯誤或帳號已停用',GETDATE(), @IP)

			SELECT 
				fsUSER_ID, fsLOGIN_ID, fsPASSWORD, USERS.fsNAME, fsENAME, fsTITLE, 
				fsDEPT_ID, '' AS _sDEPTNAME, fsEMAIL, fsPHONE, fsDESCRIPTION,fsFILE_SECRET, 
				fsIS_ACTIVE, fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY,
				'' AS _sIS_ADMINS,
				_sIMAGE_URL = '',
				'' AS _sGROUP_ID
			FROM
				tbmUSERS AS USERS
					
			WHERE
				(fsLOGIN_ID = '')	
		END

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


