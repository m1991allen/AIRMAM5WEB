﻿CREATE TABLE [dbo].[xtbtVALUE] (
    [guid]  UNIQUEIDENTIFIER CONSTRAINT [DF_tbtVALUE_guid] DEFAULT (newid()) NOT NULL,
    [value] NVARCHAR (50)    NOT NULL,
    CONSTRAINT [PK_tbtVALUE] PRIMARY KEY CLUSTERED ([guid] ASC) ON [PRIMARY]
) ON [PRIMARY];

