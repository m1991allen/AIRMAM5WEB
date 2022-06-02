﻿CREATE TABLE [dbo].[tbmARC_VIDEO] (
    [fsFILE_NO]      VARCHAR (16)    NOT NULL,
    [fsTITLE]        NVARCHAR (100)  NOT NULL,
    [fsDESCRIPTION]  NVARCHAR (MAX)  NOT NULL,
    [fsSUBJECT_ID]   VARCHAR (12)    NOT NULL,
    [fsFILE_STATUS]  CHAR (1)        NOT NULL,
    [fnFILE_SECRET]  SMALLINT        NOT NULL,
    [fsFILE_TYPE]    VARCHAR (10)    NOT NULL,
    [fsFILE_TYPE_H]  VARCHAR (10)    NOT NULL,
    [fsFILE_TYPE_L]  VARCHAR (10)    NOT NULL,
    [fsFILE_SIZE]    VARCHAR (50)    NOT NULL,
    [fsFILE_SIZE_H]  VARCHAR (50)    NOT NULL,
    [fsFILE_SIZE_L]  VARCHAR (50)    NOT NULL,
    [fsFILE_PATH]    NVARCHAR (100)  NOT NULL,
    [fsFILE_PATH_H]  NVARCHAR (100)  NOT NULL,
    [fsFILE_PATH_L]  NVARCHAR (100)  NOT NULL,
    [fxMEDIA_INFO]   NVARCHAR (MAX)  NOT NULL,
    [fsHEAD_FRAME]   VARCHAR (100)   NOT NULL,
    [fdBEG_TIME]     DECIMAL (13, 3) NOT NULL,
    [fdEND_TIME]     DECIMAL (13, 3) NOT NULL,
    [fdDURATION]     DECIMAL (13, 3) NOT NULL,
    [fsRESOL_TAG]    VARCHAR (2)     NULL,
    [fdCREATED_DATE] DATETIME        NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)    NOT NULL,
    [fdUPDATED_DATE] DATETIME        NULL,
    [fsUPDATED_BY]   VARCHAR (50)    NULL,
    [fsATTRIBUTE1]   NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE2]   NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE3]   NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE4]   NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE5]   NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE6]   NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE7]   NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE8]   NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE9]   NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE10]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE11]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE12]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE13]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE14]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE15]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE16]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE17]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE18]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE19]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE20]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE21]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE22]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE23]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE24]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE25]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE26]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE27]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE28]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE29]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE30]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE31]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE32]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE33]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE34]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE35]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE36]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE37]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE38]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE39]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE40]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE41]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE42]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE43]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE44]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE45]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE46]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE47]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE48]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE49]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE50]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE51]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE52]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE53]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE54]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE55]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE56]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE57]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE58]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE59]  NVARCHAR (MAX)  NULL,
    [fsATTRIBUTE60]  NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_tbmARC_VIDEO] PRIMARY KEY CLUSTERED ([fsFILE_NO] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [dbo].[tbmARC_VIDEO]([fsSUBJECT_ID] ASC, [fnFILE_SECRET] ASC, [fsFILE_STATUS] ASC, [fdCREATED_DATE] ASC);


GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2019/05/10>
-- Description:	<影片檔異動紀錄觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigARC_VIDEO_TRAN]
ON [dbo].[tbmARC_VIDEO]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @YEAR INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmARC_VIDEO]
	SELECT *,'U',GETDATE(),(SELECT fsUPDATED_BY FROM INSERTED WHERE fsFILE_NO = DELETED.fsFILE_NO) 
	FROM DELETED WHERE fsUPDATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM = 0)
BEGIN

	INSERT INTO [log].[tbmARC_VIDEO]
	SELECT *,'D',GETDATE(),CAST(CONTEXT_INFO() AS VARCHAR(50)) FROM DELETED

END
IF (@T_DELETE_NUM = 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmARC_VIDEO]
	SELECT *,'I',GETDATE(),fsCREATED_BY FROM INSERTED WHERE fsCREATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END

GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2011/11/11>
-- Description:	<影片檔漸進式索引觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigARC_VIDEO_INCREMENTAL]
ON [dbo].[tbmARC_VIDEO]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @YEAR INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
IF (@T_DELETE_NUM > 0)
BEGIN

	DELETE FROM [dbo].[tbdARC_VIDEO_ATTR] WHERE [fsFILE_NO] IN (SELECT fsFILE_NO FROM DELETED)
	
	INSERT INTO [dbo].[tbdARC_VIDEO_DIFFERENT](fsSYS_ID,Mode) 
		SELECT DELETED.fsFILE_NO ,3 FROM DELETED


END
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_INSERT_NUM > 0)
BEGIN
	
	INSERT INTO [dbo].[tbdARC_VIDEO_ATTR]([fsFILE_NO],[fsCODE_LIST])
		SELECT [tbmARC_VIDEO].fsFILE_NO,
			dbo.fnGET_ARC_USED_NAME_LIST_BY_CODE_LIST(dbo.fnGET_ARC_USED_CODE_LIST_BY_FILE_NO('V',dbo.tbmARC_VIDEO.fsFILE_NO))
		FROM
			[tbmARC_VIDEO]
		WHERE
			fsFILE_NO IN (SELECT fsFILE_NO FROM INSERTED)


	INSERT INTO [dbo].[tbdARC_VIDEO_DIFFERENT](fsSYS_ID,Mode) 
		SELECT fsFILE_NO ,1 FROM INSERTED
		WHERE INSERTED.fsFILE_STATUS = 'Y'

END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_VIDEO', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_VIDEO', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_VIDEO', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_VIDEO', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';
