CREATE TABLE [dbo].[tbmUSERS] (
    [fsUSER_ID]             NVARCHAR (128)  NOT NULL,
    [fsLOGIN_ID]            NVARCHAR (256)  NOT NULL,
    [fsNAME]                NVARCHAR (50)   NULL,
    [fsENAME]               NVARCHAR (50)   NULL,
    [fsTITLE]               NVARCHAR (50)   NULL,
    [fsDEPT_ID]             NVARCHAR (10)   NULL,
    [fsDESCRIPTION]         NVARCHAR (1024) NULL,
    [fsFILE_SECRET]         NVARCHAR (30)   NULL,
    [fsBOOKING_TARGET_PATH] NVARCHAR (500)  NULL,
    [fsEMAIL]               NVARCHAR (256)  NULL,
    [fsIS_ACTIVE]           BIT             NULL,
    [fsEmailConfirmed]      BIT             NULL,
    [fsPASSWORD]            NVARCHAR (MAX)  NOT NULL,
    [fsSecurityStamp]       NVARCHAR (MAX)  NULL,
    [fsPHONE]               NVARCHAR (MAX)  NULL,
    [fbPhoneConfirmed]      BIT             NULL,
    [fbTwoFactorEnabled]    BIT             NULL,
    [fdLockoutEndDateUtc]   DATETIME        NULL,
    [fbLockoutEnabled]      BIT             NULL,
    [fnAccessFailedCount]   INT             NULL,
    [fdCREATED_DATE]        DATETIME        NULL,
    [fsCREATED_BY]          NVARCHAR (128)  NULL,
    [fdUPDATED_DATE]        DATETIME        NULL,
    [fsUPDATED_BY]          NVARCHAR (128)  NULL,
    [Discriminator]         NVARCHAR (128)  NULL,
    CONSTRAINT [PK_dbo.tbmUSERS] PRIMARY KEY CLUSTERED ([fsUSER_ID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[tbmUSERS]([fsLOGIN_ID] ASC);


GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2019/05/10>
-- Description:	<使用者異動紀錄觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigUSERS_TRAN]
ON [dbo].[tbmUSERS]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @YEAR INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmUSERS]
	SELECT *,'U',GETDATE(),(SELECT fsUPDATED_BY FROM INSERTED WHERE fsUSER_ID = DELETED.fsUSER_ID) 
	FROM DELETED WHERE fsUPDATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM = 0)
BEGIN

	INSERT INTO [log].[tbmUSERS]
	SELECT *,'D',GETDATE(),CAST(CONTEXT_INFO() AS VARCHAR(50)) FROM DELETED

END
IF (@T_DELETE_NUM = 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmUSERS]
	SELECT *,'I',GETDATE(),fsCREATED_BY FROM INSERTED WHERE fsCREATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END