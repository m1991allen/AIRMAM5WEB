CREATE TABLE [dbo].[tbmARC_PHOTO] (
    [fsFILE_NO]       VARCHAR (16)   NOT NULL,
    [fsTITLE]         NVARCHAR (100) NOT NULL,
    [fsDESCRIPTION]   NVARCHAR (MAX) NOT NULL,
    [fsSUBJECT_ID]    VARCHAR (12)   NOT NULL,
    [fsFILE_STATUS]   CHAR (1)       NOT NULL,
    [fnFILE_SECRET]   SMALLINT       NOT NULL,
    [fsFILE_TYPE]     VARCHAR (10)   NOT NULL,
    [fsFILE_TYPE_H]   VARCHAR (10)   NOT NULL,
    [fsFILE_TYPE_L]   VARCHAR (10)   NOT NULL,
    [fsFILE_SIZE]     VARCHAR (50)   NOT NULL,
    [fsFILE_SIZE_H]   VARCHAR (50)   NOT NULL,
    [fsFILE_SIZE_L]   VARCHAR (50)   NOT NULL,
    [fsFILE_PATH]     NVARCHAR (100) NOT NULL,
    [fsFILE_PATH_H]   NVARCHAR (100) NOT NULL,
    [fsFILE_PATH_L]   NVARCHAR (100) NOT NULL,
    [fxMEDIA_INFO]    NVARCHAR (MAX) NULL,
    [fnWIDTH]         INT            NULL,
    [fnHEIGHT]        INT            NULL,
    [fnXDPI]          INT            NULL,
    [fnYDPI]          INT            NULL,
    [fsCAMERA_MAKE]   NVARCHAR (100) NULL,
    [fsCAMERA_MODEL]  NVARCHAR (100) NULL,
    [fsFOCAL_LENGTH]  NVARCHAR (100) NULL,
    [fsEXPOSURE_TIME] NVARCHAR (100) NULL,
    [fsAPERTURE]      NVARCHAR (100) NULL,
    [fnISO]           INT            NULL,
    [fdCREATED_DATE]  DATETIME       NOT NULL,
    [fsCREATED_BY]    VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE]  DATETIME       NULL,
    [fsUPDATED_BY]    VARCHAR (50)   NULL,
    [fsATTRIBUTE1]    NVARCHAR (MAX) NULL,
    [fsATTRIBUTE2]    NVARCHAR (MAX) NULL,
    [fsATTRIBUTE3]    NVARCHAR (MAX) NULL,
    [fsATTRIBUTE4]    NVARCHAR (MAX) NULL,
    [fsATTRIBUTE5]    NVARCHAR (MAX) NULL,
    [fsATTRIBUTE6]    NVARCHAR (MAX) NULL,
    [fsATTRIBUTE7]    NVARCHAR (MAX) NULL,
    [fsATTRIBUTE8]    NVARCHAR (MAX) NULL,
    [fsATTRIBUTE9]    NVARCHAR (MAX) NULL,
    [fsATTRIBUTE10]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE11]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE12]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE13]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE14]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE15]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE16]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE17]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE18]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE19]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE20]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE21]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE22]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE23]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE24]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE25]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE26]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE27]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE28]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE29]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE30]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE31]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE32]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE33]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE34]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE35]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE36]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE37]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE38]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE39]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE40]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE41]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE42]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE43]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE44]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE45]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE46]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE47]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE48]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE49]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE50]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE51]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE52]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE53]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE54]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE55]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE56]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE57]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE58]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE59]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE60]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_tbmARC_PHOTO] PRIMARY KEY CLUSTERED ([fsFILE_NO] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [dbo].[tbmARC_PHOTO]([fsSUBJECT_ID] ASC, [fnFILE_SECRET] ASC, [fsFILE_STATUS] ASC, [fdCREATED_DATE] ASC);


GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2019/05/10>
-- Description:	<圖片檔異動紀錄觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigARC_PHOTO_TRAN]
ON [dbo].[tbmARC_PHOTO]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @YEAR INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
BEGIN

	INSERT INTO [log].[tbmARC_PHOTO]
	SELECT *,'U',GETDATE(),(SELECT fsUPDATED_BY FROM INSERTED WHERE fsFILE_NO = DELETED.fsFILE_NO) 
	FROM DELETED
	WHERE fsUPDATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM = 0)
BEGIN

	INSERT INTO [log].[tbmARC_PHOTO]
	SELECT *,'D',GETDATE(),CAST(CONTEXT_INFO() AS VARCHAR(50)) FROM DELETED --WHERE fsUPDATED_BY <> 'STD_MTS_UL'

END
IF (@T_DELETE_NUM = 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmARC_PHOTO]
	SELECT *,'I',GETDATE(),fsCREATED_BY FROM INSERTED WHERE fsCREATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END
GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2011/11/11>
-- Description:	<圖片檔漸進式索引觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigARC_PHOTO_INCREMENTAL]
ON dbo.tbmARC_PHOTO
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
IF (@T_DELETE_NUM > 0)
BEGIN
	DELETE FROM [dbo].[tbdARC_PHOTO_ATTR]
	WHERE fsFILE_NO IN (SELECT fsFILE_NO FROM DELETED)
	
	INSERT INTO [dbo].[tbdARC_PHOTO_DIFFERENT](fsSYS_ID,Mode) 
		SELECT fsFILE_NO ,3 FROM DELETED

END
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_INSERT_NUM > 0)
BEGIN
	
	INSERT INTO [dbo].[tbdARC_PHOTO_ATTR]([fsFILE_NO],[fsCODE_LIST])
	SELECT [tbmARC_PHOTO].fsFILE_NO,
		dbo.fnGET_ARC_USED_NAME_LIST_BY_CODE_LIST(dbo.fnGET_ARC_USED_CODE_LIST_BY_FILE_NO('P',dbo.[tbmARC_PHOTO].fsFILE_NO))
	FROM
		[tbmARC_PHOTO] JOIN (SELECT fsFILE_NO FROM INSERTED) AS T ON
		[tbmARC_PHOTO].fsFILE_NO = T.fsFILE_NO

	INSERT INTO [dbo].[tbdARC_PHOTO_DIFFERENT](fsSYS_ID,Mode) 
		SELECT fsFILE_NO ,1 FROM INSERTED WHERE fsFILE_STATUS = 'Y'
END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsFILE_NO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'標題', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsTITLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsDESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'主題編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsSUBJECT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原始檔案類型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsFILE_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'高解檔案類型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsFILE_TYPE_H';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'低解檔案類型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsFILE_TYPE_L';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原始檔案大小', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsFILE_SIZE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'高解檔案大小', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsFILE_SIZE_H';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'低解檔案大小', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsFILE_SIZE_L';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原始檔案路徑', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsFILE_PATH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'高解檔案路徑', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsFILE_PATH_H';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'低解檔案路徑', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsFILE_PATH_L';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'MediaInfo', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fxMEDIA_INFO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寬', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fnWIDTH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'高', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fnHEIGHT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'圖片X解析度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fnXDPI';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'圖片Y解析度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fnYDPI';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'相機廠牌', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsCAMERA_MAKE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'相機型號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsCAMERA_MODEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'焦距', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsFOCAL_LENGTH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'曝光時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsEXPOSURE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'光圈', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsAPERTURE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ISO', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fnISO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位1', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位2', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位3', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE3';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位4', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE4';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位5', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE5';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位6', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE6';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位7', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE7';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位8', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE8';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位9', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE9';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位10', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE10';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位11', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE11';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位12', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE12';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位13', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE13';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位14', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE14';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位15', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE15';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位16', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE16';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位17', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE17';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位18', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE18';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位19', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE19';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位20', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE20';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位21', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE21';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位22', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE22';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位23', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE23';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位24', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE24';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位25', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE25';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位26', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE26';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位27', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE27';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位28', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE28';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位29', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE29';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位30', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE30';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位31', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE31';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位32', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE32';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位33', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE33';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位34', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE34';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位35', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE35';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位36', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE36';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位37', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE37';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位38', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE38';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位39', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE39';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位40', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE40';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位41', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE41';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位42', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE42';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位43', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE43';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位44', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE44';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位45', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE45';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位46', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE46';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位47', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE47';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位48', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE48';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位49', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE49';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位50', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE50';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位51', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE51';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位52', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE52';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位53', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE53';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位54', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE54';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位55', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE55';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位56', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE56';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位57', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE57';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位58', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE58';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位59', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE59';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位60', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmARC_PHOTO', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE60';

