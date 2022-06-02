CREATE TABLE [log].[tbmSYNONYMS] (
    [fnID]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [fnINDEX_ID]     BIGINT          NOT NULL,
    [fsTEXT_LIST]    NVARCHAR (4000) NOT NULL,
    [fsTYPE]         VARCHAR (10)    NULL,
    [fsNOTE]         NVARCHAR (MAX)  NULL,
    [fdCREATED_DATE] DATETIME        NOT NULL,
    [fsCREATED_BY]   VARCHAR (50)    NOT NULL,
    [fdUPDATED_DATE] DATETIME        NULL,
    [fsUPDATED_BY]   VARCHAR (50)    NULL,
    [fcMODE]         CHAR (1)        NOT NULL,
    [fdOP_DATE]      DATETIME        CONSTRAINT [DF_tbmSYNONYMS_fdOP_DATE] DEFAULT (getdate()) NULL,
    [fsOP_BY]        VARCHAR (50)    NULL,
    CONSTRAINT [PK_tbmSYNONYMS] PRIMARY KEY CLUSTERED ([fnID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [log].[tbmSYNONYMS]([fnINDEX_ID] ASC);

