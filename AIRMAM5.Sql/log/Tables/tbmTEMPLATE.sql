CREATE TABLE [log].[tbmTEMPLATE] (
    [fnID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [fnTEMP_ID]      INT            NOT NULL,
    [fsNAME]         NVARCHAR (50)  NOT NULL,
    [fsTABLE]        CHAR (1)       NOT NULL,
    [fsDESCRIPTION]  NVARCHAR (MAX) NOT NULL,
    [fcIS_SEARCH]    CHAR (1)       NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    [fcMODE]         CHAR (1)       NOT NULL,
    [fdOP_DATE]      DATETIME       CONSTRAINT [DF_tbmTEMPLATE_fdOP_DATE] DEFAULT (getdate()) NULL,
    [fsOP_BY]        VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmTEMPLATE] PRIMARY KEY CLUSTERED ([fnID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [log].[tbmTEMPLATE]([fnTEMP_ID] ASC);

