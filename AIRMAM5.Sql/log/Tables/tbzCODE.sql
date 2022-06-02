CREATE TABLE [log].[tbzCODE] (
    [fnID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [fsCODE_ID]      VARCHAR (20)   NOT NULL,
    [fsCODE]         VARCHAR (50)   NOT NULL,
    [fsNAME]         NVARCHAR (200) NOT NULL,
    [fsENAME]        VARCHAR (200)  NULL,
    [fnORDER]        INT            NOT NULL,
    [fsSET]          VARCHAR (50)   NULL,
    [fsNOTE]         NVARCHAR (200) NULL,
    [fsIS_ENABLED]   CHAR (1)       NOT NULL,
    [fsTYPE]         CHAR (1)       NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    [fcMODE]         CHAR (1)       NOT NULL,
    [fdOP_DATE]      DATETIME       CONSTRAINT [DF_tbzCODE_fdOP_DATE] DEFAULT (getdate()) NULL,
    [fsOP_BY]        VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbzCODE] PRIMARY KEY CLUSTERED ([fnID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [log].[tbzCODE]([fsCODE_ID] ASC, [fsCODE] ASC);

