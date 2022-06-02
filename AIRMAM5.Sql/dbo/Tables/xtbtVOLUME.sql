CREATE TABLE [dbo].[xtbtVOLUME] (
    [磁帶名稱]   VARCHAR (50)    CONSTRAINT [DF_tbtVOLUME_磁帶名稱] DEFAULT ('') NOT NULL,
    [使用狀態]   VARCHAR (50)    NOT NULL,
    [讀寫狀態]   VARCHAR (50)    NOT NULL,
    [已存放資料量] DECIMAL (10, 3) CONSTRAINT [DF_tbtVOLUME_已存放資料量] DEFAULT ((0)) NOT NULL,
    [儲存池]    VARCHAR (50)    NOT NULL,
    [最後讀取]   DATETIME        NULL,
    [最後寫入]   DATETIME        NULL,
    [讀取錯誤]   INT             CONSTRAINT [DF_tbtVOLUME_讀取錯誤] DEFAULT ((0)) NOT NULL,
    [寫入錯誤]   INT             NOT NULL,
    [磁帶位置]   NVARCHAR (10)   NOT NULL,
    CONSTRAINT [PK_tbtVOLUME] PRIMARY KEY CLUSTERED ([磁帶名稱] ASC)
);

