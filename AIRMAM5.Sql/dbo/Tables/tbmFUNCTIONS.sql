CREATE TABLE [dbo].[tbmFUNCTIONS] (
    [fsFUNC_ID]        VARCHAR (50)   NOT NULL,
    [fsNAME]           NVARCHAR (50)  NOT NULL,
    [fsDESCRIPTION]    NVARCHAR (MAX) NOT NULL,
    [fsTYPE]           CHAR (1)       NOT NULL,
    [fnORDER]          INT            NOT NULL,
    [fsICON]           VARCHAR (50)   NOT NULL,
    [fsPARENT_ID]      VARCHAR (50)   NOT NULL,
    [fsHEADER]         NVARCHAR (20)  NOT NULL,
    [fsCONTROLLER]     VARCHAR (100)  NULL,
    [fsACTION]         VARCHAR (100)  NULL,
    [fsIS_MULTI_SHEET] BIT            NOT NULL,
    [fdCREATED_DATE]   DATETIME       NOT NULL,
    [fsCREATED_BY]     VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE]   DATETIME       NULL,
    [fsUPDATED_BY]     VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmFUNCTIONS] PRIMARY KEY CLUSTERED ([fsFUNC_ID] ASC)
);

