CREATE TABLE [dbo].[xt_tblARC_CNT1Y] (
    [fnINDEX_ID]     BIGINT        NOT NULL,
    [fdSDATE]        DATE          NOT NULL,
    [fdEDATE]        DATE          NOT NULL,
    [fsTYPE]         VARCHAR (1)   NOT NULL,
    [fsIDENTIFIER]   VARCHAR (16)  NOT NULL,
    [fnCOUNT]        INT           NOT NULL,
    [fdCREATED_DATE] DATETIME      NOT NULL,
    [fsCREATED_BY]   NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_t_tblARC_CNT1Y] PRIMARY KEY CLUSTERED ([fdEDATE] ASC, [fsIDENTIFIER] ASC, [fnINDEX_ID] ASC) ON [PRIMARY]
) ON [PRIMARY];


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'資料起始日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xt_tblARC_CNT1Y', @level2type = N'COLUMN', @level2name = N'fdSDATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'資料結束日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xt_tblARC_CNT1Y', @level2type = N'COLUMN', @level2name = N'fdEDATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'統計數字', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xt_tblARC_CNT1Y', @level2type = N'COLUMN', @level2name = N'fnCOUNT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xt_tblARC_CNT1Y', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'xt_tblARC_CNT1Y', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';

