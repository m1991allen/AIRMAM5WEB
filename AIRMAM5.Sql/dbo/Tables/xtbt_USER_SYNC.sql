﻿CREATE TABLE [dbo].[xtbt_USER_SYNC] (
    [fsFTV_EMAIL]  VARCHAR (50)  NOT NULL,
    [fsEMP_NO]     VARCHAR (10)  NOT NULL,
    [fsEMP_NAME]   NVARCHAR (50) NOT NULL,
    [fsEMP_GENDER] NVARCHAR (1)  NOT NULL,
    [fsEMP_DEPT]   NVARCHAR (50) NOT NULL,
    [fsEMP_SUFFIX] NVARCHAR (20) NOT NULL
) ON [PRIMARY];

