CREATE TABLE [log].[tbmDIRECTORIES] (
    [fnID]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [fnDIR_ID]          BIGINT         NOT NULL,
    [fsNAME]            NVARCHAR (50)  NOT NULL,
    [fnPARENT_ID]       BIGINT         NOT NULL,
    [fsDESCRIPTION]     NVARCHAR (MAX) NOT NULL,
    [fsDIRTYPE]         CHAR (1)       NOT NULL,
    [fnORDER]           INT            NOT NULL,
    [fnTEMP_ID_SUBJECT] INT            NOT NULL,
    [fnTEMP_ID_VIDEO]   INT            NOT NULL,
    [fnTEMP_ID_AUDIO]   INT            NOT NULL,
    [fnTEMP_ID_PHOTO]   INT            NOT NULL,
    [fnTEMP_ID_DOC]     INT            NOT NULL,
    [fsADMIN_GROUP]     VARCHAR (500)  NOT NULL,
    [fsADMIN_USER]      VARCHAR (900)  NOT NULL,
    [fsSHOWTYPE]        CHAR (1)       NOT NULL,
    [fdCREATED_DATE]    DATETIME       NOT NULL,
    [fsCREATED_BY]      VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE]    DATETIME       NULL,
    [fsUPDATED_BY]      VARCHAR (50)   NULL,
    [fcMODE]            CHAR (1)       NOT NULL,
    [fdOP_DATE]         DATETIME       CONSTRAINT [DF_tbmDIRECTORIES_fdOP_DATE] DEFAULT (getdate()) NULL,
    [fsOP_BY]           VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmDIRECTORIES] PRIMARY KEY CLUSTERED ([fnID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [log].[tbmDIRECTORIES]([fnDIR_ID] ASC);

