﻿CREATE TABLE [dbo].[xtbmARC_SET] (
    [fsTYPE]         VARCHAR (1)    NOT NULL,
    [fsNAME]         NVARCHAR (5)   NOT NULL,
    [fsTYPE_I]       VARCHAR (1000) NOT NULL,
    [fsTYPE_O]       VARCHAR (500)  NOT NULL,
    [fsTYPE_S]       VARCHAR (10)   NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   NVARCHAR (50)  NOT NULL,
    [fdUPDATED_DATE] DATETIME       NOT NULL,
    [fsUPDATED_BY]   NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_tbmARC_SET] PRIMARY KEY CLUSTERED ([fsTYPE] ASC) ON [PRIMARY]
) ON [PRIMARY];

