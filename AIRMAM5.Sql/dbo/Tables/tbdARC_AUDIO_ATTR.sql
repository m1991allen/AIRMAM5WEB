﻿CREATE TABLE [dbo].[tbdARC_AUDIO_ATTR] (
    [fsFILE_NO]   VARCHAR (16)   NOT NULL,
    [fsCODE_LIST] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_tbmARC_AUDIO_ATTR] PRIMARY KEY CLUSTERED ([fsFILE_NO] ASC)
);
