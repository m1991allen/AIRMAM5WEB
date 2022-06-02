CREATE TABLE [dbo].[tbzMACHINE] (
    [FSSERVER_NAME]       VARCHAR (255)  NOT NULL,
    [FSSERVER_IP]         VARCHAR (255)  NOT NULL,
    [FSGROUP]             VARCHAR (50)   NULL,
    [FSMAX_CPU]           INT            NULL,
    [FSMONITOR_HD]        VARCHAR (128)  NULL,
    [FSMAX_HD]            VARCHAR (50)   NULL,
    [FSMAX_MEMORY]        INT            NULL,
    [FSMAX_LAN]           INT            NULL,
    [FSNETWORK_INTERFACE] VARCHAR (500)  NULL,
    [FSMAX_WEB]           INT            NULL,
    [FSWEB_NAME]          NVARCHAR (255) NULL,
    [FCAVAILABLE]         CHAR (1)       NULL,
    [FCHAS_NOTIFY]        CHAR (1)       NULL,
    [FSTYPE]              VARCHAR (10)   NULL,
    CONSTRAINT [PK_TBMachine] PRIMARY KEY CLUSTERED ([FSSERVER_NAME] ASC)
);

