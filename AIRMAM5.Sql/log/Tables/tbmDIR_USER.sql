﻿CREATE TABLE [log].[tbmDIR_USER] (
    [fnID]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [fnDIR_ID]        BIGINT         NOT NULL,
    [fsLOGIN_ID]      NVARCHAR (256) NOT NULL,
    [fsLIMIT_SUBJECT] VARCHAR (10)   NOT NULL,
    [fsLIMIT_VIDEO]   VARCHAR (10)   NOT NULL,
    [fsLIMIT_AUDIO]   VARCHAR (10)   NOT NULL,
    [fsLIMIT_PHOTO]   VARCHAR (10)   NOT NULL,
    [fsLIMIT_DOC]     VARCHAR (10)   NOT NULL,
    [fdCREATED_DATE]  DATETIME       NOT NULL,
    [fsCREATED_BY]    VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE]  DATETIME       NULL,
    [fsUPDATED_BY]    VARCHAR (50)   NULL,
    [fcMODE]          CHAR (1)       NOT NULL,
    [fdOP_DATE]       DATETIME       CONSTRAINT [DF_tbmDIR_USER_fdOP_DATE] DEFAULT (getdate()) NULL,
    [fsOP_BY]         VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmDIR_USER] PRIMARY KEY CLUSTERED ([fnID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [log].[tbmDIR_USER]([fnDIR_ID] ASC, [fsLOGIN_ID] ASC);

