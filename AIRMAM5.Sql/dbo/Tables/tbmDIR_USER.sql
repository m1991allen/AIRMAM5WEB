﻿CREATE TABLE [dbo].[tbmDIR_USER] (
    [fnDIR_ID]        BIGINT         NOT NULL,
    [fsLOGIN_ID]      NVARCHAR (256) NOT NULL,
    [fsLIMIT_SUBJECT] VARCHAR (10)   NOT NULL,
    [fsLIMIT_VIDEO]   VARCHAR (10)   NOT NULL,
    [fsLIMIT_AUDIO]   VARCHAR (10)   NOT NULL,
    [fsLIMIT_PHOTO]   VARCHAR (10)   NOT NULL,
    [fsLIMIT_DOC]     VARCHAR (10)   NOT NULL,
    [fdCREATED_DATE]  DATETIME       NOT NULL,
    [fsCREATED_BY]    VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE]  DATETIME       NULL,
    [fsUPDATED_BY]    VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmDIR_USER] PRIMARY KEY CLUSTERED ([fnDIR_ID] ASC, [fsLOGIN_ID] ASC)
);


GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2019/05/10>
-- Description:	<目錄使用者異動紀錄觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigDIR_USER_TRAN]
ON [dbo].[tbmDIR_USER]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @YEAR INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmDIR_USER]
	SELECT *,'U',GETDATE(),(SELECT fsUPDATED_BY FROM INSERTED WHERE fnDIR_ID = DELETED.fnDIR_ID AND fsLOGIN_ID = DELETED.fsLOGIN_ID) 
	FROM DELETED WHERE fsUPDATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM = 0)
BEGIN

	INSERT INTO [log].[tbmDIR_USER]
	SELECT *,'D',GETDATE(),CAST(CONTEXT_INFO() AS VARCHAR(50)) FROM DELETED

END
IF (@T_DELETE_NUM = 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmDIR_USER]
	SELECT *,'I',GETDATE(),fsCREATED_BY FROM INSERTED WHERE fsCREATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END