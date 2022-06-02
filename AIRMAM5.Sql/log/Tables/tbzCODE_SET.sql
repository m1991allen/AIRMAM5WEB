CREATE TABLE [log].[tbzCODE_SET] (
    [fnID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [fsCODE_ID]      VARCHAR (20)   NOT NULL,
    [fsTITLE]        NVARCHAR (50)  NOT NULL,
    [fsTBCOL]        VARCHAR (MAX)  NULL,
    [fsNOTE]         NVARCHAR (200) NOT NULL,
    [fsIS_ENABLED]   CHAR (1)       NOT NULL,
    [fsTYPE]         CHAR (1)       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fcMODE]         CHAR (1)       NOT NULL,
    [fdOP_DATE]      DATETIME       CONSTRAINT [DF_tbzCODE_SET_fdOP_DATE] DEFAULT (getdate()) NULL,
    [fsOP_BY]        VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbzCODE_SET] PRIMARY KEY CLUSTERED ([fnID] ASC)
) TEXTIMAGE_ON [PRIMARY];


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [log].[tbzCODE_SET]([fsCODE_ID] ASC);

