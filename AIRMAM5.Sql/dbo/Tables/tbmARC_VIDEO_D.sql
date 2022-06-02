CREATE TABLE [dbo].[tbmARC_VIDEO_D] (
    [fsFILE_NO]      VARCHAR (16)    NOT NULL,
    [fnSEQ_NO]       INT             NOT NULL,
    [fsDESCRIPTION]  NVARCHAR (MAX)  NOT NULL,
    [fdBEG_TIME]     DECIMAL (13, 3) NOT NULL,
    [fdEND_TIME]     DECIMAL (13, 3) NOT NULL,
    [fdCREATED_DATE] DATETIME        NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)    NOT NULL,
    [fdUPDATED_DATE] DATETIME        NULL,
    [fsUPDATED_BY]   VARCHAR (50)    NULL,
    CONSTRAINT [PK_tbmARC_VIDEO_D] PRIMARY KEY CLUSTERED ([fsFILE_NO] ASC, [fnSEQ_NO] ASC)
);


GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2019/05/10>
-- Description:	<影片段落描述異動紀錄觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigARC_VIDEO_D_TRAN]
ON [dbo].[tbmARC_VIDEO_D]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @YEAR INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmARC_VIDEO_D]
	SELECT *,'U',GETDATE(),(SELECT fsUPDATED_BY FROM INSERTED WHERE fsFILE_NO = DELETED.fsFILE_NO) 
	FROM DELETED WHERE fsUPDATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM = 0)
BEGIN

	INSERT INTO [log].[tbmARC_VIDEO_D]
	SELECT *,'D',GETDATE(),CAST(CONTEXT_INFO() AS VARCHAR(50)) FROM DELETED

END
IF (@T_DELETE_NUM = 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmARC_VIDEO_D]
	SELECT *,'I',GETDATE(),fsCREATED_BY FROM INSERTED WHERE fsCREATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END
GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2011/11/11>
-- Description:	<影片檔漸進式索引觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigARC_VIDEO_D_INCREMENTAL]
ON [dbo].[tbmARC_VIDEO_D]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @YEAR INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
IF (@T_DELETE_NUM > 0)
BEGIN

	INSERT INTO [dbo].[tbdARC_VIDEO_DIFFERENT](fsSYS_ID,Mode) 
		SELECT DELETED.fsFILE_NO ,3 FROM DELETED


END
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [dbo].[tbdARC_VIDEO_DIFFERENT](fsSYS_ID,Mode) 
		SELECT INSERTED.fsFILE_NO ,1 FROM INSERTED JOIN tbmARC_VIDEO ON INSERTED.[fsFILE_NO] = tbmARC_VIDEO.[fsFILE_NO]
		WHERE tbmARC_VIDEO.fsFILE_STATUS = 'Y'

END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_VIDEO_D', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_VIDEO_D', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_VIDEO_D', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_VIDEO_D', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';

