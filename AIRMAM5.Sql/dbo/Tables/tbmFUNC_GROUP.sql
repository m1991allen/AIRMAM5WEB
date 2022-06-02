﻿CREATE TABLE [dbo].[tbmFUNC_GROUP] (
    [fsFUNC_ID]      VARCHAR (50) NOT NULL,
    [fsGROUP_ID]     VARCHAR (50) NOT NULL,
    [fdCREATED_DATE] DATETIME     NOT NULL,
    [fsCREATED_BY]   VARCHAR (50) NOT NULL,
    [fdUPDATED_DATE] DATETIME     NULL,
    [fsUPDATED_BY]   VARCHAR (50) NULL,
    CONSTRAINT [PK_tbmFUNC_GROUP] PRIMARY KEY CLUSTERED ([fsFUNC_ID] ASC, [fsGROUP_ID] ASC)
);

