CREATE TABLE [log].[tbmTEMPLATE_FIELDS] (
    [fnID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [fnTEMP_ID]      INT            NOT NULL,
    [fsFIELD]        VARCHAR (50)   NOT NULL,
    [fsFIELD_NAME]   NVARCHAR (50)  NOT NULL,
    [fsFIELD_TYPE]   VARCHAR (50)   NOT NULL,
    [fnFIELD_LENGTH] INT            NULL,
    [fsDESCRIPTION]  NVARCHAR (MAX) NULL,
    [fnORDER]        INT            NOT NULL,
    [fnCTRL_WIDTH]   INT            NULL,
    [fsMULTILINE]    CHAR (1)       NULL,
    [fsISNULLABLE]   CHAR (10)      NULL,
    [fsDEFAULT]      NVARCHAR (50)  NULL,
    [fsCODE_ID]      VARCHAR (20)   NULL,
    [fnCODE_CNT]     INT            NULL,
    [fsCODE_CTRL]    VARCHAR (20)   NULL,
    [fsIS_SEARCH]    CHAR (1)       NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    [fcMODE]         CHAR (1)       NOT NULL,
    [fdOP_DATE]      DATETIME       CONSTRAINT [DF_tbmTEMPLATE_FIELDS_fdOP_DATE] DEFAULT (getdate()) NULL,
    [fsOP_BY]        VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmTEMPLATE_FIELDS] PRIMARY KEY CLUSTERED ([fnID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [log].[tbmTEMPLATE_FIELDS]([fnTEMP_ID] ASC, [fsFIELD] ASC);

