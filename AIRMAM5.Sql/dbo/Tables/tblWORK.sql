CREATE TABLE [dbo].[tblWORK] (
    [fnWORK_ID]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [fnGROUP_ID]        BIGINT         CONSTRAINT [DF_tblWORK_fnGROUP_ID] DEFAULT ((0)) NOT NULL,
    [fsTYPE]            VARCHAR (50)   NOT NULL,
    [fsPARAMETERS]      NVARCHAR (300) NOT NULL,
    [fsSTATUS]          VARCHAR (2)    NOT NULL,
    [fsPROGRESS]        NVARCHAR (50)  CONSTRAINT [DF_tblWORK_fsPROGRESS] DEFAULT ('') NOT NULL,
    [fsPRIORITY]        VARCHAR (1)    CONSTRAINT [DF_tblWORK_fsPRIORITY] DEFAULT ('5') NOT NULL,
    [fdSTART_WORK_TIME] DATETIME       CONSTRAINT [DF_tblWORK_fdSTART_WORK_TIME] DEFAULT (getdate()) NOT NULL,
    [fdSTIME]           DATETIME       NOT NULL,
    [fdETIME]           DATETIME       NULL,
    [fsRESULT]          NVARCHAR (100) NOT NULL,
    [fsNOTE]            NVARCHAR (500) NOT NULL,
    [fdCREATED_DATE]    DATETIME       NOT NULL,
    [fsCREATED_BY]      VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE]    DATETIME       NULL,
    [fsUPDATED_BY]      VARCHAR (50)   NULL,
    [_ITEM_TYPE]        VARCHAR (20)   NULL,
    [_ITEM_ID]          VARCHAR (20)   NOT NULL,
    [_sDESCRIPTION]     NVARCHAR (50)  NULL,
    [_SM_VOLUME_NAME]   NVARCHAR (50)  NULL,
    [_ITEM_SET1]        NVARCHAR (500) NULL,
    [_ITEM_SET2]        NVARCHAR (50)  NULL,
    [_ITEM_SET3]        NVARCHAR (50)  NULL,
    [_ITEM_SET4]        NVARCHAR (50)  NULL,
    [_APPROVE_STATUS]   VARCHAR (2)    NULL,
    [_APPROVE_DATE]     DATETIME       NULL,
    [_APPROVE_BY]       VARCHAR (50)   NULL,
    [_ARC_TYPE]         AS             (substring([fsPARAMETERS],(1),(1))),
    CONSTRAINT [PK_tblWORK] PRIMARY KEY CLUSTERED ([fnWORK_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [dbo].[tblWORK]([fnWORK_ID] ASC, [fnGROUP_ID] ASC, [fsTYPE] ASC, [fsPARAMETERS] ASC, [fsSTATUS] ASC, [fsPRIORITY] ASC, [fdSTART_WORK_TIME] ASC, [fdCREATED_DATE] ASC, [_ITEM_ID] ASC)
    INCLUDE([fsRESULT], [fsCREATED_BY], [fsPROGRESS], [fdSTIME], [fdETIME]);


GO
-- =============================================
-- Author:		David.Sin
-- Create date: 2019/09/06
-- Description:	若全部完成時，修改調用主檔狀態完成。
-- =============================================
CREATE TRIGGER [dbo].[trigWORK_STATUS]
   ON  [dbo].[tblWORK]
   AFTER UPDATE
AS 
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)

IF(@T_DELETE_NUM > 0 AND @T_INSERT_NUM > 0)
BEGIN
	--只有調用的才需要去變更tbmBOOKING狀態
	IF((SELECT COUNT(1) FROM INSERTED WHERE fsTYPE IN ('BOOKING','COPYFILE','NAS','AVID')) > 0)
	BEGIN
		
		DECLARE @GROUP_ID BIGINT = (SELECT fnGROUP_ID FROM INSERTED)
	
		IF((SELECT COUNT(1) FROM tblWORK WHERE fsTYPE IN ('BOOKING','COPYFILE','NAS','AVID') AND fnGROUP_ID = @GROUP_ID) =
			(SELECT COUNT(1) FROM tblWORK WHERE fsTYPE IN ('BOOKING','COPYFILE','NAS','AVID') AND fnGROUP_ID = @GROUP_ID AND fsSTATUS > '00'))
		BEGIN
		
			UPDATE 
				tbmBOOKING 
			SET 
				fsSTATUS = '90',
				fdUPDATED_DATE=GETDATE(),
				fsUPDATED_BY='WORK_TRIGGER' 
			WHERE 
				fnBOOKING_ID = @GROUP_ID
		END

	END
	
END
--BEGIN	
	--SET NOCOUNT ON;
	--DECLARE @nGroup_ID as bigint, @sType as varchar(50), @sStatus as varchar(2)
	--SELECT @nGroup_ID = fnGROUP_ID, @sType = fsTYPE, @sStatus = fsSTATUS  FROM inserted
	
 --   if (@sType = 'BOOKING' or @sType = 'COPYFILE') and (@sStatus > '9') 
 --   begin
	--	DECLARE @nAll  as bigint, @nStatus as bigint
	--	SET @nAll = (select COUNT(*) from tblWORK where fnGROUP_ID = @nGroup_ID)
	--	SET @nStatus = (select COUNT(*) from tblWORK where fnGROUP_ID = @nGroup_ID AND fsSTATUS > '9')
	--	IF ( @nAll = @nStatus)
	--	BEGIN
	--		UPDATE tbmBOOKING SET fsSTATUS = '90', fdUPDATED_DATE=GETDATE(), fsUPDATED_BY='sys' WHERE fnBOOKING_ID = @nGroup_ID
	--	END
 --   end;


--END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'工作項目123', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'工作註冊ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fnWORK_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'工作類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fsTYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'工作參數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fsPARAMETERS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'工作狀態', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fsSTATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'進度說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fsPROGRESS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'優先順序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fsPRIORITY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'工作起始時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fdSTIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'工作停止時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fdETIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'工作執行結果', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fsRESULT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'工作備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fsNOTE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblWORK', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';

