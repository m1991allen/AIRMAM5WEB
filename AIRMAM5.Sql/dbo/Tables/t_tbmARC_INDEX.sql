CREATE TABLE [dbo].[t_tbmARC_INDEX] (
    [fnINDEX_ID]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [fsFILE_NO]      VARCHAR (16)  NOT NULL,
    [fsTYPE]         VARCHAR (4)   NOT NULL,
    [fsREASON]       NVARCHAR (50) NOT NULL,
    [fsSTATUS]       CHAR (1)      NOT NULL,
    [fdCREATED_DATE] DATETIME      NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)  NOT NULL,
    [fdUPDATED_DATE] DATETIME      NULL,
    [fsUPDATED_BY]   VARCHAR (50)  NULL,
    CONSTRAINT [PK_t_tbmARC_INDEX] PRIMARY KEY CLUSTERED ([fnINDEX_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'刪除記錄ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N't_tbmARC_INDEX', @level2type = N'COLUMN', @level2name = N'fnINDEX_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'媒體檔編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N't_tbmARC_INDEX', @level2type = N'COLUMN', @level2name = N'fsFILE_NO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'媒體檔類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N't_tbmARC_INDEX', @level2type = N'COLUMN', @level2name = N'fsTYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'刪除說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N't_tbmARC_INDEX', @level2type = N'COLUMN', @level2name = N'fsREASON';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狀態', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N't_tbmARC_INDEX', @level2type = N'COLUMN', @level2name = N'fsSTATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N't_tbmARC_INDEX', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N't_tbmARC_INDEX', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N't_tbmARC_INDEX', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N't_tbmARC_INDEX', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';

