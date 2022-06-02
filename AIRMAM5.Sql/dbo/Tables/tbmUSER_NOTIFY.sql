﻿CREATE TABLE [dbo].[tbmUSER_NOTIFY] (
    [fsUSER_ID]      NVARCHAR (128) NOT NULL,
    [fnNOTIFY_ID]    BIGINT         NOT NULL,
    [fbIS_READ]      BIT            CONSTRAINT [DF_tbmUSER_NOTIFY_fbIS_READ] DEFAULT ((0)) NOT NULL,
    [fbIS_DELETE]    BIT            CONSTRAINT [DF_tbmUSER_NOTIFY_fbIS_DELETE] DEFAULT ((0)) NOT NULL,
    [fdREAD_TIME]    DATETIME       NULL,
    [fdDELETE_TIME]  DATETIME       NULL,
    [fdCREATED_DATE] DATETIME       CONSTRAINT [DF_tbmUSER_NOTIFY_fdCREATED_DATE] DEFAULT (getdate()) NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmUSER_NOTIFY] PRIMARY KEY CLUSTERED ([fsUSER_ID] ASC, [fnNOTIFY_ID] ASC)
);
