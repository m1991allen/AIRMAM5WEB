CREATE TABLE [log].[tbmUSERS] (
    [fnID]                  BIGINT          IDENTITY (1, 1) NOT NULL,
    [fsUSER_ID]             NVARCHAR (128)  NOT NULL,
    [fsLOGIN_ID]            NVARCHAR (256)  NOT NULL,
    [fsNAME]                NVARCHAR (50)   NULL,
    [fsENAME]               NVARCHAR (50)   NULL,
    [fsTITLE]               NVARCHAR (50)   NULL,
    [fsDEPT_ID]             NVARCHAR (10)   NULL,
    [fsDESCRIPTION]         NVARCHAR (1024) NULL,
    [fsFILE_SECRET]         NVARCHAR (30)   NULL,
    [fsBOOKING_TARGET_PATH] NVARCHAR (500)  NULL,
    [fsEMAIL]               NVARCHAR (256)  NULL,
    [fsIS_ACTIVE]           BIT             NULL,
    [fsEmailConfirmed]      BIT             NULL,
    [fsPASSWORD]            NVARCHAR (MAX)  NULL,
    [fsSecurityStamp]       NVARCHAR (MAX)  NULL,
    [fsPHONE]               NVARCHAR (MAX)  NULL,
    [fbPhoneConfirmed]      BIT             NULL,
    [fbTwoFactorEnabled]    BIT             NULL,
    [fdLockoutEndDateUtc]   DATETIME        NULL,
    [fbLockoutEnabled]      BIT             NULL,
    [fnAccessFailedCount]   INT             NULL,
    [fdCREATED_DATE]        DATETIME        NULL,
    [fsCREATED_BY]          NVARCHAR (128)  NULL,
    [fdUPDATED_DATE]        DATETIME        NULL,
    [fsUPDATED_BY]          NVARCHAR (128)  NULL,
    [Discriminator]         NVARCHAR (128)  NULL,
    [fcMODE]                CHAR (1)        NOT NULL,
    [fdOP_DATE]             DATETIME        CONSTRAINT [DF_tbmUSERS_fdOP_DATE] DEFAULT (getdate()) NULL,
    [fsOP_BY]               VARCHAR (50)    NULL,
    CONSTRAINT [PK_tbmUSERS] PRIMARY KEY CLUSTERED ([fnID] ASC)
) TEXTIMAGE_ON [PRIMARY];


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [log].[tbmUSERS]([fsUSER_ID] ASC);

