CREATE TABLE [dbo].[tbmREMINDERS] (
    [fnRMD_ID]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [fsTITLE]        NVARCHAR (50)  NOT NULL,
    [fsCONTENT]      NVARCHAR (MAX) NULL,
    [fdDDATE]        DATETIME       NOT NULL,
    [fsTYPE]         CHAR (1)       NOT NULL,
    [fsTO_UID]       VARCHAR (50)   NOT NULL,
    [fsSTATUS]       CHAR (1)       CONSTRAINT [DF_tbmREMINDERS_fsSTATUS] DEFAULT ('1') NOT NULL,
    [fnORDER]        INT            NOT NULL,
    [fsNOTE]         NVARCHAR (MAX) NOT NULL,
    [fsCHECKSTATUS]  CHAR (1)       CONSTRAINT [DF_tbmREMINDERS_fsCheckStatus] DEFAULT ('N') NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NOT NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NOT NULL,
    CONSTRAINT [PK_tbmREMINDERS] PRIMARY KEY CLUSTERED ([fnRMD_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_]
    ON [dbo].[tbmREMINDERS]([fdDDATE] ASC, [fsTYPE] ASC, [fsTO_UID] ASC, [fsSTATUS] ASC, [fsCHECKSTATUS] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'提醒事項ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fnRMD_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'提醒事項標題
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fsTITLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'提醒事項內容
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fsCONTENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預計完成日
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fdDDATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'提醒事項分類
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fsTYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'負責人員
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fsTO_UID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'提醒事項狀態
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fsSTATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顯示順序
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fnORDER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fsNOTE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmREMINDERS', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';

