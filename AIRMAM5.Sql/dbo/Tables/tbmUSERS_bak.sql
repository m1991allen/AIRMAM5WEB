CREATE TABLE [dbo].[tbmUSERS_bak] (
    [fnUSER_ID]             BIGINT         IDENTITY (1, 1) NOT NULL,
    [fsLOGIN_ID]            VARCHAR (50)   NOT NULL,
    [fsPASSWORD]            VARCHAR (255)  NOT NULL,
    [fsNAME]                NVARCHAR (50)  NOT NULL,
    [fsENAME]               VARCHAR (50)   NULL,
    [fsTITLE]               NVARCHAR (50)  NULL,
    [fsDEPT_ID]             VARCHAR (10)   NULL,
    [fsEMAIL]               VARCHAR (50)   NULL,
    [fsPHONE]               VARCHAR (20)   NULL,
    [fsDESCRIPTION]         NVARCHAR (MAX) NULL,
    [fsFILE_SECRET]         VARCHAR (30)   NOT NULL,
    [fsBOOKING_TARGET_PATH] VARCHAR (500)  NULL,
    [fsIS_ACTIVE]           CHAR (1)       NOT NULL,
    [fdCREATED_DATE]        DATETIME       NOT NULL,
    [fsCREATED_BY]          VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE]        DATETIME       NULL,
    [fsUPDATED_BY]          VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmUSERS] PRIMARY KEY CLUSTERED ([fnUSER_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號ID
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fnUSER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登入ID
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fsLOGIN_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'密碼
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fsPASSWORD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顯示名稱
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fsNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'英文名稱
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fsENAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'職稱
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fsTITLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所屬單位
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fsDEPT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'電子郵件
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fsEMAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fsDESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'作用中
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fsIS_ACTIVE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmUSERS_bak', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';

