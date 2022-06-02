


-- =============================================
-- 描述:	取出USER_A 登入帳號資料
-- 記錄:	<2011/09/05><Eric.Huang><新增本預存>
-- 記錄:	<2014/01/10><Eric.Huang><加入LDAP判斷式>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_USER_APP_BY_PASSWORD]
	@fsLOGIN_ID	NVARCHAR(50),
	@fsPASSWORD	VARCHAR(255),
	@IP	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY	

		-- 2014/01/10 --
		--IF EXISTS(	SELECT	fnUSER_A_ID 
		--			FROM	tbmUSER_APP
		--			WHERE	(fsLOGIN_ID = @fsLOGIN_ID) AND
		--					(fsPASSWORD = @fsPASSWORD)	)

		IF EXISTS(	SELECT	fnUSER_A_ID 
					FROM	tbmUSER_APP
					WHERE	(fsLOGIN_ID = @fsLOGIN_ID) AND
							(
							  (fsPASSWORD = @fsPASSWORD) OR
  							  (fsPASSWORD = '//' )
							))
			BEGIN
			
				SELECT 
					fnUSER_A_ID, fsLOGIN_ID, fsPASSWORD, USER_A.fsNAME, fsENAME, fsTITLE, 
					fsDEPT_ID, D.fsNAME AS _sDEPTNAME, fsEMAIL, fsPHONE, fsDESCRIPTION, 
					fsSTATUS, U.fsNAME AS _sSTATUSNAME,fsNOTE, 
					fdCREATED_DATE, fsCREATED_BY, fdUPDATED_DATE, fsUPDATED_BY
					
				FROM
					tbmUSER_APP AS USER_A
					LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'DEPT001') AS D ON (USER_A.fsDEPT_ID = D.fsCODE)
					LEFT JOIN (select fsCODE, fsNAME from tbzCODE where fsCODE_ID = 'USER002') AS U ON (USER_A.fsSTATUS = U.fsCODE)		
				WHERE
				    -- 2014/01/10 --
					--(fsLOGIN_ID = @fsLOGIN_ID) AND
					--(fsPASSWORD = @fsPASSWORD)			
					-- 2014/01/10 ++
					(fsLOGIN_ID = @fsLOGIN_ID) AND
					((fsPASSWORD = @fsPASSWORD ) OR (fsPASSWORD = '//'))
			END

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


