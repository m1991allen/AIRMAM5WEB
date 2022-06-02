﻿CREATE TABLE [log].[tbmARC_PRE] (
    [fnID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [fnPRE_ID]       BIGINT         NOT NULL,
    [fsNAME]         NVARCHAR (50)  NOT NULL,
    [fsTYPE]         CHAR (1)       NOT NULL,
    [fnTEMP_ID]      INT            NOT NULL,
    [fsTITLE]        NVARCHAR (100) NOT NULL,
    [fsDESCRIPTION]  NVARCHAR (MAX) NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE] DATETIME       NULL,
    [fsUPDATED_BY]   VARCHAR (50)   NULL,
    [fsATTRIBUTE1]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE2]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE3]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE4]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE5]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE6]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE7]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE8]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE9]   NVARCHAR (MAX) NULL,
    [fsATTRIBUTE10]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE11]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE12]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE13]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE14]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE15]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE16]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE17]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE18]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE19]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE20]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE21]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE22]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE23]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE24]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE25]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE26]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE27]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE28]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE29]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE30]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE31]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE32]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE33]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE34]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE35]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE36]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE37]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE38]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE39]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE40]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE41]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE42]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE43]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE44]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE45]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE46]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE47]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE48]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE49]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE50]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE51]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE52]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE53]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE54]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE55]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE56]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE57]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE58]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE59]  NVARCHAR (MAX) NULL,
    [fsATTRIBUTE60]  NVARCHAR (MAX) NULL,
    [fcMODE]         CHAR (1)       NOT NULL,
    [fdOP_DATE]      DATETIME       NOT NULL,
    [fsOP_BY]        VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmARC_PRE] PRIMARY KEY CLUSTERED ([fnID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [log].[tbmARC_PRE]([fnPRE_ID] ASC);
