CREATE TABLE [dbo].[xtblARC_CLK] (
    [fnARC_CLK_ID]   BIGINT        IDENTITY (1, 1) NOT NULL,
    [fsTYPE]         VARCHAR (50)  NOT NULL,
    [fsFILE_NO]      VARCHAR (16)  NOT NULL,
    [fsSUBJECT_ID]   VARCHAR (12)  NOT NULL,
    [fsFROM]         VARCHAR (10)  NOT NULL,
    [fdCREATED_DATE] DATETIME      NOT NULL,
    [fsCREATED_BY]   NVARCHAR (50) NOT NULL,
    [fdUPDATED_DATE] DATETIME      NOT NULL,
    [fsUPDATED_BY]   NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_tblARC_CLK] PRIMARY KEY CLUSTERED ([fnARC_CLK_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'工作註冊ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xtblARC_CLK', @level2type = N'COLUMN', @level2name = N'fnARC_CLK_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'媒資檔類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xtblARC_CLK', @level2type = N'COLUMN', @level2name = N'fsTYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xtblARC_CLK', @level2type = N'COLUMN', @level2name = N'fsFILE_NO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'點閱時所在主題', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xtblARC_CLK', @level2type = N'COLUMN', @level2name = N'fsSUBJECT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'點閱來源', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xtblARC_CLK', @level2type = N'COLUMN', @level2name = N'fsFROM';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xtblARC_CLK', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xtblARC_CLK', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xtblARC_CLK', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xtblARC_CLK', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';

