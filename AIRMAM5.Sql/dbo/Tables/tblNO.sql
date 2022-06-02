CREATE TABLE [dbo].[tblNO] (
    [fsTYPE]         VARCHAR (10)  NOT NULL,
    [fsNAME]         NVARCHAR (10) NOT NULL,
    [fsHEAD]         VARCHAR (10)  NOT NULL,
    [fsBODY]         VARCHAR (10)  NOT NULL,
    [fsNO]           INT           NOT NULL,
    [fsNO_L]         INT           NOT NULL,
    [fdCREATED_DATE] DATETIME      NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)  NOT NULL,
    [fdUPDATED_DATE] DATETIME      NOT NULL,
    [fsUPDATED_BY]   VARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_tblNO] PRIMARY KEY CLUSTERED ([fsTYPE] ASC, [fsHEAD] ASC)
);

