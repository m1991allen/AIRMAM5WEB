CREATE TABLE [log].[tbmGROUPS] (
    [fnID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [fsGROUP_ID]     NVARCHAR (128) NOT NULL,
    [fsNAME]         NVARCHAR (50)  NOT NULL,
    [fsDESCRIPTION]  NVARCHAR (MAX) NULL,
    [fsTYPE]         CHAR (1)       NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    [Discriminator]  NVARCHAR (128) NOT NULL,
    [fcMODE]         CHAR (1)       NOT NULL,
    [fdOP_DATE]      DATETIME       CONSTRAINT [DF_tbmGROUPS_fdOP_DATE] DEFAULT (getdate()) NULL,
    [fsOP_BY]        VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmGROUPS] PRIMARY KEY CLUSTERED ([fnID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [log].[tbmGROUPS]([fsGROUP_ID] ASC);

