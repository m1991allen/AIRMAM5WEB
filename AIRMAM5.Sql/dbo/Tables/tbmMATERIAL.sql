CREATE TABLE [dbo].[tbmMATERIAL] (
    [fnMATERIAL_ID]  BIGINT         IDENTITY (1, 1) NOT NULL,
    [fsMARKED_BY]    VARCHAR (50)   NOT NULL,
    [fsTYPE]         CHAR (1)       NOT NULL,
    [fsFILE_NO]      VARCHAR (16)   NOT NULL,
    [fsDESCRIPTION]  NVARCHAR (MAX) NOT NULL,
    [fsNOTE]         NVARCHAR (MAX) NOT NULL,
    [fsPARAMETER]    VARCHAR (100)  NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmMATERIAL] PRIMARY KEY CLUSTERED ([fnMATERIAL_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [dbo].[tbmMATERIAL]([fnMATERIAL_ID] ASC, [fsMARKED_BY] ASC, [fsTYPE] ASC, [fsFILE_NO] ASC, [fdCREATED_DATE] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'媒體素材ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmMATERIAL', @level2type = N'COLUMN', @level2name = N'fnMATERIAL_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'標記人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmMATERIAL', @level2type = N'COLUMN', @level2name = N'fsMARKED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'媒體類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmMATERIAL', @level2type = N'COLUMN', @level2name = N'fsTYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmMATERIAL', @level2type = N'COLUMN', @level2name = N'fsFILE_NO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'素材描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmMATERIAL', @level2type = N'COLUMN', @level2name = N'fsDESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'素材備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmMATERIAL', @level2type = N'COLUMN', @level2name = N'fsNOTE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'素材參數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmMATERIAL', @level2type = N'COLUMN', @level2name = N'fsPARAMETER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmMATERIAL', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmMATERIAL', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmMATERIAL', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmMATERIAL', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';

