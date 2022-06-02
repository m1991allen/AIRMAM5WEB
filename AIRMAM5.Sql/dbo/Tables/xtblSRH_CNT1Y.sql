﻿CREATE TABLE [dbo].[xtblSRH_CNT1Y] (
    [fdSDATE]        DATE           NOT NULL,
    [fdEDATE]        DATE           NOT NULL,
    [fsKEYWORD]      NVARCHAR (100) NOT NULL,
    [fnCOUNT]        INT            NOT NULL,
    [fdCREATED_DATE] DATETIME       NOT NULL,
    [fsCREATED_BY]   NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_tblSRH_CNT1Y] PRIMARY KEY CLUSTERED ([fdEDATE] ASC, [fsKEYWORD] ASC) ON [PRIMARY]
) ON [PRIMARY];

