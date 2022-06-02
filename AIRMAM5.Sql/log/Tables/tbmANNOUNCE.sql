CREATE TABLE [log].[tbmANNOUNCE] (
    [fnID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [fnANN_ID]       BIGINT         NOT NULL,
    [fsTITLE]        NVARCHAR (50)  NOT NULL,
    [fsCONTENT]      NVARCHAR (MAX) NOT NULL,
    [fdSDATE]        DATETIME       NOT NULL,
    [fdEDATE]        DATETIME       NULL,
    [fsTYPE]         CHAR (1)       NOT NULL,
    [fnORDER]        INT            NOT NULL,
    [fsGROUP_LIST]   VARCHAR (500)  NOT NULL,
    [fsIS_HIDDEN]    CHAR (10)      NOT NULL,
    [fsDEPT]         NVARCHAR (50)  NOT NULL,
    [fsNOTE]         NVARCHAR (MAX) NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    [fcMODE]         CHAR (1)       NOT NULL,
    [fdOP_DATE]      DATETIME       CONSTRAINT [DF_tbmANNOUNCE_fdOP_DATE] DEFAULT (getdate()) NULL,
    [fsOP_BY]        VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmANNOUNCE] PRIMARY KEY CLUSTERED ([fnID] ASC)
) TEXTIMAGE_ON [PRIMARY];


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [log].[tbmANNOUNCE]([fnANN_ID] ASC);

