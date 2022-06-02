CREATE TABLE [dbo].[tbmSUBJECT] (
    [fsSUBJ_ID]      VARCHAR (12)   NOT NULL,
    [fsTITLE]        NVARCHAR (100) NOT NULL,
    [fsDESCRIPTION]  NVARCHAR (MAX) NOT NULL,
    [fnDIR_ID]       BIGINT         NOT NULL,
    [fsTYPE1]        NVARCHAR (10)  NOT NULL,
    [fsTYPE2]        NVARCHAR (10)  NOT NULL,
    [fsTYPE3]        NVARCHAR (10)  NOT NULL,
    [fsATTRIBUTE1]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE2]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE3]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE4]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE5]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE6]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE7]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE8]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE9]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE10]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE11]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE12]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE13]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE14]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE15]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE16]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE17]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE18]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE19]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE20]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE21]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE22]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE23]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE24]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE25]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE26]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE27]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE28]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE29]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE30]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE31]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE32]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE33]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE34]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE35]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE36]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE37]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE38]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE39]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE40]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE41]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE42]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE43]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE44]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE45]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE46]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE47]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE48]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE49]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE50]  NVARCHAR (MAX) NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmSUBJECT] PRIMARY KEY CLUSTERED ([fsSUBJ_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [dbo].[tbmSUBJECT]([fnDIR_ID] ASC);


GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2011/11/11>
-- Description:	<SUBJECT漸進式索引觸發程序>
-- 2013/07/30 Eric.Huang REMOVE ==>INSERT INTO [dbo].[tbdARC_SUBJECT_DIFFERENT_2]
-- =============================================

CREATE TRIGGER [dbo].[trigSUBJECT_INCREMENTAL]
ON [dbo].[tbmSUBJECT]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
IF (@T_DELETE_NUM <> 0)
BEGIN	
	INSERT INTO [dbo].[tbdARC_SUBJECT_DIFFERENT](fsSYS_ID,Mode) 
		SELECT fsSUBJ_ID ,3 FROM DELETED

	INSERT INTO [dbo].[tbdARC_VIDEO_DIFFERENT](fsSYS_ID,Mode)
		SELECT V.fsFILE_NO, 3 FROM tbmARC_VIDEO V JOIN DELETED D ON V.fsSUBJECT_ID = D.fsSUBJ_ID

	INSERT INTO [dbo].[tbdARC_AUDIO_DIFFERENT](fsSYS_ID,Mode)
		SELECT A.fsFILE_NO, 3 FROM tbmARC_AUDIO A JOIN DELETED D ON A.fsSUBJECT_ID = D.fsSUBJ_ID

	INSERT INTO [dbo].[tbdARC_PHOTO_DIFFERENT](fsSYS_ID,Mode)
		SELECT P.fsFILE_NO, 3 FROM tbmARC_PHOTO P JOIN DELETED D ON P.fsSUBJECT_ID = D.fsSUBJ_ID

	INSERT INTO [dbo].[tbdARC_DOC_DIFFERENT](fsSYS_ID,Mode)
		SELECT DOC.fsFILE_NO, 3 FROM tbmARC_DOC DOC JOIN DELETED D ON DOC.fsSUBJECT_ID = D.fsSUBJ_ID
END		
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_INSERT_NUM <> 0)
BEGIN	
	INSERT INTO [dbo].[tbdARC_SUBJECT_DIFFERENT](fsSYS_ID,Mode) 
		SELECT fsSUBJ_ID ,1 FROM INSERTED

	INSERT INTO [dbo].[tbdARC_VIDEO_DIFFERENT](fsSYS_ID,Mode)
		SELECT V.fsFILE_NO, 1 FROM tbmARC_VIDEO V JOIN INSERTED I ON V.fsSUBJECT_ID = I.fsSUBJ_ID WHERE V.fsFILE_STATUS = 'Y'

	INSERT INTO [dbo].[tbdARC_AUDIO_DIFFERENT](fsSYS_ID,Mode)
		SELECT A.fsFILE_NO, 1 FROM tbmARC_AUDIO A JOIN INSERTED I ON A.fsSUBJECT_ID = I.fsSUBJ_ID WHERE A.fsFILE_STATUS = 'Y'

	INSERT INTO [dbo].[tbdARC_PHOTO_DIFFERENT](fsSYS_ID,Mode)
		SELECT P.fsFILE_NO, 1 FROM tbmARC_PHOTO P JOIN INSERTED I ON P.fsSUBJECT_ID = I.fsSUBJ_ID WHERE P.fsFILE_STATUS = 'Y'

	INSERT INTO [dbo].[tbdARC_DOC_DIFFERENT](fsSYS_ID,Mode)
		SELECT DOC.fsFILE_NO, 1 FROM tbmARC_DOC DOC JOIN INSERTED I ON DOC.fsSUBJECT_ID = I.fsSUBJ_ID WHERE DOC.fsFILE_STATUS = 'Y'
END

GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2019/05/10>
-- Description:	<主題異動紀錄觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigSUBJECT_TRAN]
ON [dbo].[tbmSUBJECT]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @YEAR INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmSUBJECT]
	SELECT *,'U',GETDATE(),(SELECT fsUPDATED_BY FROM INSERTED WHERE fsSUBJ_ID = DELETED.fsSUBJ_ID) 
	FROM DELETED WHERE fsUPDATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM = 0)
BEGIN

	INSERT INTO [log].[tbmSUBJECT]
	SELECT *,'D',GETDATE(),CAST(CONTEXT_INFO() AS VARCHAR(50)) FROM DELETED

END
IF (@T_DELETE_NUM = 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmSUBJECT]
	SELECT *,'I',GETDATE(),fsCREATED_BY FROM INSERTED WHERE fsCREATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'目錄ID
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fnDIR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位1', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位2', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位3', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE3';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位4', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE4';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位5', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE5';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位6', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE6';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位7', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE7';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位8', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE8';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位9', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE9';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位10', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE10';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位11', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE11';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位12', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE12';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位13', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE13';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位14', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE14';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位15', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE15';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位16', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE16';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位17', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE17';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位18', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE18';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位19', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE19';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位20', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE20';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位21', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE21';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位22', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE22';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位23', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE23';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位24', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE24';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位25', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE25';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位26', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE26';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位27', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE27';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位28', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE28';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位29', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE29';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位30', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE30';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位31', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE31';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位32', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE32';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位33', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE33';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位34', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE34';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位35', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE35';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位36', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE36';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位37', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE37';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位38', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE38';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位39', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE39';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位40', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE40';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位41', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE41';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位42', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE42';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位43', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE43';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位44', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE44';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位45', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE45';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位46', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE46';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位47', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE47';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位48', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE48';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位49', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE49';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位50', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsATTRIBUTE50';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmSUBJECT', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';

