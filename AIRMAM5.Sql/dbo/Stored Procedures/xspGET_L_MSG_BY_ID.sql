
-- =============================================
-- 描述:	取出通訊資料
-- 記錄:	<2013/05/08><Dennis.Wen><新增本預存>
-- 記錄:	<2015/07/27><Eric.Huang><修改本預存-加入傳給群組成員的功能!>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_L_MSG_BY_ID]
	@UID		NVARCHAR(50)

AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY	--DECLARE @UID		NVARCHAR(50) = 'admin'
		---------------

		DECLARE @RESULT TABLE ([fsMSG_ID] VARCHAR(36), [fsFROM_ID] NVARCHAR(50), [fsTO_ID] NVARCHAR(50), [fsINFO] NVARCHAR(500), [fdCREATED_DATE] DATETIME,
								_sFROM_ID_NAME NVARCHAR(50), _sTO_ID_NAME NVARCHAR(50))
								 


		DECLARE @ISADMIN VARCHAR(1) = '', @fnUSER_ID BIGINT = 0, @GROUPS VARCHAR(500) = ''
		SET @fnUSER_ID = ISNULL((SELECT TOP 1 fnUSER_ID FROM [dbo].[tbmUSERS] WHERE [fsLOGIN_ID] = @UID), 0)
		IF EXISTS(SELECT * FROM [dbo].[tbmUSER_GROUP] WHERE [fnUSER_ID]=@fnUSER_ID AND [fsGROUP_ID]='Administrators')
		BEGIN 
			SET @ISADMIN = 'Y'
		END

		-- 2015/07/27 ERIC ++ ---------------------------------------------------------
		DECLARE @group_result varchar(1000) = ''
		DECLARE @i int
		DECLARE @user_group_id VARCHAR(50)
		DECLARE @numrows int
		DECLARE @user_group_table TABLE (
		idx smallint Primary Key IDENTITY(1,1)
		, user_group_id VARCHAR(50)
		)

		INSERT @user_group_table
		SELECT distinct fsGROUP_ID FROM [tbmUSER_GROUP] 
		WHERE fnUSER_ID = @fnUSER_ID

		SET @i = 1
		SET @numrows = (SELECT COUNT(0) FROM @user_group_table)

		IF @numrows > 0
		WHILE (@i <= (SELECT MAX(idx) FROM @user_group_table))
		BEGIN

			SET @user_group_id = (SELECT user_group_id FROM @user_group_table WHERE idx = @i)
			SET @group_result += '@' + @user_group_id + ';'
			SET @i = @i + 1
		END

		-- 2015/07/27 ERIC ++ ---------------------------------------------------------


		DECLARE @DELETE_CNT INT = 0
		SET @DELETE_CNT = (SELECT COUNT(*) FROM [dbo].[t_tbmARC_INDEX] WHERE [fsSTATUS] = '') 

		IF((@ISADMIN = 'Y') AND (@DELETE_CNT <> 0))
		BEGIN
			INSERT @RESULT
			SELECT 'DELETE_ARC', '@SYSTEM', '', '目前有' + CAST(@DELETE_CNT AS VARCHAR(5)) + '筆媒體資產的刪除事件尚未被審核, 請管理員至"媒體資產刪除管理"作業中通過或還原這些刪除事件.', GETDATE(), '<系統通知>', (SELECT TOP 1 USERS2.fsNAME FROM [tbmUSERS] AS USERS2 WHERE (USERS2.fsLOGIN_ID = @UID))
		END
		--------------- 

		UPDATE	[tblMESSAGE]
		SET		_OK = 'N'
		--WHERE	(fsTO_ID = @UID) AND (_OK = '') 
		-- 2015/07/27 ERIC ++
		WHERE	((fsTO_ID = @UID) OR (@group_result LIKE '%' + fsTO_ID +'%')) AND (_OK = '') 


		INSERT
			@RESULT
		SELECT 
			MSG.fsMSG_ID, MSG.fsFROM_ID, MSG.fsTO_ID, MSG.fsINFO, MSG.fdCREATED_DATE,
			_sFROM_ID_NAME = (CASE WHEN (MSG.fsFROM_ID = '@SYSTEM') THEN '<系統通知>' ELSE USERS1.fsNAME END), --USERS1.fsNAME,
			_sTO_ID_NAME = (CASE WHEN (MSG.fsTO_ID = '全部使用者') THEN '' ELSE USERS2.fsNAME END)
		FROM
			[tblMESSAGE] AS MSG
			LEFT JOIN [tbmUSERS] AS USERS1 ON (USERS1.fsLOGIN_ID = MSG.fsFROM_ID)
			LEFT JOIN [tbmUSERS] AS USERS2 ON (USERS2.fsLOGIN_ID = @UID)
		WHERE
			((MSG.fsTO_ID = @UID) AND (_OK = 'N')) OR --這個人
			((MSG.fsTO_ID = '') AND (_OK = '') AND (DATEDIFF(DAY, MSG.fdCREATED_DATE, GETDATE()) < 3)) OR --所有人
			((@ISADMIN = 'Y') AND (MSG.fsTO_ID = '@ADMINISTRATORS') AND (_OK = '') AND (DATEDIFF(DAY, MSG.fdCREATED_DATE, GETDATE()) < 3)) --管理者 
			-- 2015/07/27 ERIC ++ ---------------------------------------------------------
			OR	((@group_result LIKE '%' + MSG.fsTO_ID +'%') AND (_OK = 'N'))  --此群組
			-- 2015/07/27 ERIC ++ ---------------------------------------------------------

		--ORDER BY
		--	MSG.fdCREATED_DATE DESC --這邊要改成全部通知的類別要限制時間

		---------------

		SELECT * FROM @RESULT ORDER BY fdCREATED_DATE DESC 

		---------------

		UPDATE	[tblMESSAGE]
		SET		_OK = 'Y'
		--WHERE	(fsTO_ID = @UID) AND (_OK = 'N') 
		-- 2015/07/27 ERIC ++
		WHERE	((fsTO_ID = @UID)OR (@group_result LIKE '%' + fsTO_ID +'%')) AND (_OK = 'N') 
			
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


