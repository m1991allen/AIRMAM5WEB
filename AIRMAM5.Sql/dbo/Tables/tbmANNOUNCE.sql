CREATE TABLE [dbo].[tbmANNOUNCE] (
    [fnANN_ID]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [fsTITLE]        NVARCHAR (50)  NOT NULL,
    [fsCONTENT]      NVARCHAR (MAX) NOT NULL,
    [fdSDATE]        DATETIME       NOT NULL,
    [fdEDATE]        DATETIME       NULL,
    [fsTYPE]         CHAR (1)       NOT NULL,
    [fnORDER]        INT            NOT NULL,
    [fsGROUP_LIST]   VARCHAR (500)  NULL,
    [fsIS_HIDDEN]    CHAR (1)       NOT NULL,
    [fsDEPT]         NVARCHAR (50)  NOT NULL,
    [fsNOTE]         NVARCHAR (MAX) NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmANNOUNCE] PRIMARY KEY CLUSTERED ([fnANN_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [dbo].[tbmANNOUNCE]([fdSDATE] ASC, [fdEDATE] ASC, [fsTYPE] ASC, [fsGROUP_LIST] ASC, [fsIS_HIDDEN] ASC, [fsDEPT] ASC);


GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2019/05/10>
-- Description:	<公告異動紀錄觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigANNOUNCE_TRAN]
ON [dbo].[tbmANNOUNCE]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @YEAR INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmANNOUNCE]
	SELECT *,'U',GETDATE(),(SELECT fsUPDATED_BY FROM INSERTED WHERE fnANN_ID = DELETED.fnANN_ID)
	FROM DELETED

END

ELSE IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM = 0)
BEGIN
	
	INSERT INTO [log].[tbmANNOUNCE]
	SELECT *,'D',GETDATE(),CAST(CONTEXT_INFO() AS VARCHAR(50))
	FROM DELETED

END
IF (@T_DELETE_NUM = 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmANNOUNCE]
	SELECT *,'I',GETDATE(),fsCREATED_BY FROM INSERTED WHERE fsCREATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'系統公告ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fnANN_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'公告標題', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fsTITLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'公告內容', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fsCONTENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'公告上架日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fdSDATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'公告下架日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fdEDATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'公告分類', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fsTYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顯示順序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fnORDER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'公告群組', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fsGROUP_LIST';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'隱藏', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fsIS_HIDDEN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'發佈單位', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fsDEPT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fsNOTE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmANNOUNCE', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';

