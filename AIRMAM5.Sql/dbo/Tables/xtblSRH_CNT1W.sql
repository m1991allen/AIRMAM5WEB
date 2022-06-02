﻿CREATE TABLE [dbo].[xtblSRH_CNT1W] (
    [fdSDATE]        DATE           NOT NULL,
    [fdEDATE]        DATE           NOT NULL,
    [fsKEYWORD]      NVARCHAR (100) NOT NULL,
    [fnCOUNT]        INT            NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_tbl_SRH_CNT1W] PRIMARY KEY CLUSTERED ([fdEDATE] ASC, [fsKEYWORD] ASC) ON [PRIMARY]
) ON [PRIMARY];

