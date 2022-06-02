﻿CREATE TABLE [dbo].[xtbzDEPT] (
    [fsDEPT_ID]      VARCHAR (10)   NOT NULL,
    [fsNAME]         NVARCHAR (50)  NOT NULL,
    [fsPARENT_ID]    VARCHAR (10)   NOT NULL,
    [fnORDER]        INT            NOT NULL,
    [fsNOTE]         NVARCHAR (200) NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   NVARCHAR (50)  NOT NULL,
    [fdUPDATED_DATE] DATETIME       NOT NULL,
    [fsUPDATED_BY]   NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_TBDEPT] PRIMARY KEY CLUSTERED ([fsDEPT_ID] ASC)
);

