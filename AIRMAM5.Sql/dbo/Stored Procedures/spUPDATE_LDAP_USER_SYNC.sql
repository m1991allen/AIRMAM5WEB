


-- =============================================
-- 描述:	LDAP USER SYNC
-- 記錄:	<2014/02/11><Eric.Huang><新增本預存>
-- 記錄:	<2014/04/15><Eric.Huang><修改本預存,將USER都加入LDAP_USERS GROUP>
-- 記錄:	<2014/05/12><Eric.Huang><修改本預存,當tbmUSERS有資料,但tbt_USER_SYNC有MAIL,沒有NAME時,代表LDAP帳號已被刪除,故停用此MAM帳號>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_LDAP_USER_SYNC]
	
AS
BEGIN
 	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @fsFTV_EMAIL	VARCHAR(50)
		DECLARE @fsEMP_NO		VARCHAR(10)
		DECLARE @fsEMP_NAME		NVARCHAR(50)
		DECLARE @fsEMP_GENDER	NVARCHAR(1)
		DECLARE @fsEMP_DEPT		NVARCHAR(50)
		DECLARE @fsEMP_SUFFIX	NVARCHAR(20)
	
		DECLARE @tmpPASSWORD    VARCHAR(255)
		DECLARE @newUserID      INT

		DECLARE @LDAP_GROUP   NVARCHAR(50)
		DECLARE @fsCREATED_BY NVARCHAR(50)
		SET @LDAP_GROUP = 'LDAP_USERS'
		SET @fsCREATED_BY = 'LDAP_SYNC'

		DECLARE CSR5 CURSOR FOR SELECT fsFTV_EMAIL, fsEMP_NO, fsEMP_NAME, fsEMP_GENDER, fsEMP_DEPT, fsEMP_SUFFIX FROM tbt_USER_SYNC

		OPEN CSR5
		FETCH NEXT FROM CSR5 INTO @fsFTV_EMAIL,@fsEMP_NO,@fsEMP_NAME,@fsEMP_GENDER,@fsEMP_DEPT,@fsEMP_SUFFIX
		WHILE (@@FETCH_STATUS=0)
			BEGIN 
			---------開始處理每一筆資料
				IF (@fsEMP_NAME <> '')				
				BEGIN
						--1. list有(也有EMP_NAME的) MAM沒有 => 新增帳號
					IF NOT EXISTS(SELECT * FROM [dbo].[tbmUSERS] WHERE fsLOGIN_ID = @fsFTV_EMAIL)
						BEGIN
							INSERT INTO tbmUSERS(fsLOGIN_ID,fsPASSWORD,fsNAME,fsDEPT_ID,fsEMAIL,fsDESCRIPTION,fsIS_ACTIVE,fsTYPE,fdCREATED_DATE,fsCREATED_BY) 
									VALUES(@fsFTV_EMAIL,'//', @fsEMP_NAME, '', @fsFTV_EMAIL + '@ftv.com.tw','','Y','O', GETDATE(), @fsCREATED_BY)
							SELECT @newUserID = @@IDENTITY
							INSERT	tbmUSER_GROUP
									(fnUSER_ID, fsGROUP_ID, fdCREATED_DATE, fsCREATED_BY)
								VALUES
									(@newUserID, @LDAP_GROUP, GETDATE(), @fsCREATED_BY)
						END
					ELSE
						BEGIN
					
							SET @tmpPASSWORD = (SELECT fsPASSWORD FROM [dbo].[tbmUSERS] WHERE fsLOGIN_ID = @fsFTV_EMAIL)

							IF (@tmpPASSWORD = '//')
								BEGIN
									--2. list有(也有EMP_NAME的) MAM有 密碼// => UPDATE名稱
									UPDATE tbmUSERS SET fsNAME = @fsEMP_NAME WHERE fsLOGIN_ID = @fsFTV_EMAIL
								END
							ELSE
								BEGIN
									--3.list有(也有EMP_NAME的) MAM有 密碼非// => 通知信發給管理者
									
									/*發通知信*/
									BEGIN
										DECLARE @tableHTML NVARCHAR(MAX),@WHO VARCHAR(MAX)

										SET @tableHTML = 
														N'LDAP 同步AIRMAM帳號中,發現異常' + N'</tr>' + 'LDAP帳號 ' + @fsFTV_EMAIL + ' 與MAM帳號發生衝突!'
			
										--SELECT @tableHTML	 
											SET @WHO = (select TOP 1 fsVALUE from tbzCONFIG WHERE fsKEY='MAIL_LIST_PROC')

											EXEC msdb.dbo.sp_send_dbmail @recipients = @WHO,
												  @profile_name  = 'FTV MAIL',
												  @subject = 'LDAP同步AIRMAM帳號作業中,發現異常',
												  @body = @tableHTML,
												  @body_format = 'HTML' ;
									END
								END																														
						END	
				END
			
			---------處理完畢每一筆資料
			FETCH NEXT FROM CSR5 INTO @fsFTV_EMAIL,@fsEMP_NO,@fsEMP_NAME,@fsEMP_GENDER,@fsEMP_DEPT,@fsEMP_SUFFIX
			END
		CLOSE CSR5
		DEALLOCATE CSR5

	END TRY
	
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
		CLOSE CSR5
		DEALLOCATE CSR5
	END CATCH


	BEGIN TRY
	
		DECLARE @fsLOGIN_ID		VARCHAR(255)
		DECLARE @fsPASSWORD		VARCHAR(255)
		DECLARE @fsNAME			NVARCHAR(50)

		DECLARE CSR6 CURSOR FOR SELECT fsLOGIN_ID, fsPASSWORD, fsNAME FROM tbmUSERS
	

		OPEN CSR6
		FETCH NEXT FROM CSR6 INTO @fsLOGIN_ID,@fsPASSWORD,@fsNAME
		WHILE (@@FETCH_STATUS=0)
			BEGIN 
			---------開始處理每一筆資料

				--4.list沒有		 MAM有 密碼// => 停用
					IF NOT EXISTS(SELECT * FROM tbt_USER_SYNC WHERE fsFTV_EMAIL = @fsLOGIN_ID AND fsEMP_NAME <> '')
						BEGIN
							IF (@fsPASSWORD = '//')
							BEGIN
								UPDATE tbmUSERS SET fsIS_ACTIVE = 'N',
								fdUPDATED_DATE 	 = GETDATE(),
								fsUPDATED_BY	 = 'LDAP_SYNC',
								fsDESCRIPTION    = 'LDAP帳號已被刪除'
								WHERE fsLOGIN_ID = @fsLOGIN_ID
							END
						END

			---------處理完畢每一筆資料
			FETCH NEXT FROM CSR6 INTO @fsLOGIN_ID, @fsPASSWORD, @fsNAME
			END
		CLOSE CSR6
		DEALLOCATE CSR6

		END TRY

		BEGIN CATCH
			CLOSE CSR6
			DEALLOCATE CSR6
		END CATCH
END







