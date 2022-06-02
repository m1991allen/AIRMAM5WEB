CREATE TABLE [dbo].[tblLOGIN] (
    [fnLOGIN_ID]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [fsLOGIN_ID]     NVARCHAR (50)  NOT NULL,
    [fdSTIME]        DATETIME       NOT NULL,
    [fdETIME]        DATETIME       NULL,
    [fsNOTE]         NVARCHAR (200) NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    CONSTRAINT [PK_tblLOGIN] PRIMARY KEY CLUSTERED ([fnLOGIN_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [dbo].[tblLOGIN]([fsLOGIN_ID] ASC, [fdSTIME] ASC, [fdETIME] ASC, [fdCREATED_DATE] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登入記錄ID
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblLOGIN', @level2type = N'COLUMN', @level2name = N'fnLOGIN_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登入ID
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblLOGIN', @level2type = N'COLUMN', @level2name = N'fsLOGIN_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登入時間
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblLOGIN', @level2type = N'COLUMN', @level2name = N'fdSTIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登出時間
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblLOGIN', @level2type = N'COLUMN', @level2name = N'fdETIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblLOGIN', @level2type = N'COLUMN', @level2name = N'fsNOTE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblLOGIN', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblLOGIN', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblLOGIN', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tblLOGIN', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';

