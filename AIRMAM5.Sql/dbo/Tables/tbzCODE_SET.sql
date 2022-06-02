CREATE TABLE [dbo].[tbzCODE_SET] (
    [fsCODE_ID]      VARCHAR (20)   NOT NULL,
    [fsTITLE]        NVARCHAR (50)  NOT NULL,
    [fsTBCOL]        VARCHAR (MAX)  NULL,
    [fsNOTE]         NVARCHAR (200) NOT NULL,
    [fsIS_ENABLED]   CHAR (1)       NOT NULL,
    [fsTYPE]         CHAR (1)       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    CONSTRAINT [PK_tbzCODE_SET] PRIMARY KEY CLUSTERED ([fsCODE_ID] ASC)
);


GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2019/05/10>
-- Description:	<代碼明細紀錄觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigCODE_SET_TRAN]
ON [dbo].[tbzCODE_SET]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @YEAR INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbzCODE_SET]
	SELECT *,'U',GETDATE(),(SELECT fsUPDATED_BY FROM INSERTED WHERE fsCODE_ID = DELETED.fsCODE_ID) 
	FROM DELETED WHERE fsUPDATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM = 0)
BEGIN

	INSERT INTO [log].[tbzCODE_SET]
	SELECT *,'D',GETDATE(),CAST(CONTEXT_INFO() AS VARCHAR(50)) FROM DELETED

END
IF (@T_DELETE_NUM = 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbzCODE_SET]
	SELECT *,'I',GETDATE(),fsCREATED_BY FROM INSERTED WHERE fsCREATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代碼群組編號
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbzCODE_SET', @level2type = N'COLUMN', @level2name = N'fsCODE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代碼群組名稱
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbzCODE_SET', @level2type = N'COLUMN', @level2name = N'fsTITLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'影響的資料表資料列
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbzCODE_SET', @level2type = N'COLUMN', @level2name = N'fsTBCOL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'註記
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbzCODE_SET', @level2type = N'COLUMN', @level2name = N'fsNOTE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否可選
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbzCODE_SET', @level2type = N'COLUMN', @level2name = N'fsIS_ENABLED';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代碼設定分類', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbzCODE_SET', @level2type = N'COLUMN', @level2name = N'fsTYPE';

