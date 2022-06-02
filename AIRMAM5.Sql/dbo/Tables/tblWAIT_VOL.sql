CREATE TABLE [dbo].[tblWAIT_VOL] (
    [fnWAIT_ID]      BIGINT       IDENTITY (1, 1) NOT NULL,
    [fsVOL_ID]       VARCHAR (50) NOT NULL,
    [fnWORK_ID]      BIGINT       NOT NULL,
    [fsSTATUS]       VARCHAR (2)  NOT NULL,
    [fdCREATED_DATE] DATETIME     NOT NULL,
    [fsCREATED_BY]   VARCHAR (50) NOT NULL,
    [fdUPDATED_DATE] DATETIME     NOT NULL,
    [fsUPDATED_BY]   VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_tblWAIT_VOL] PRIMARY KEY CLUSTERED ([fnWAIT_ID] ASC)
);


GO
-- =============================================
-- Author:		Dennis.Wen
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[trigWAIT_VOL_INSERT]
   ON dbo.tblWAIT_VOL
   AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON;

 	BEGIN TRY
		----------

		DECLARE	@fsVOL_ID VARCHAR(50) = '', @fnWAIT_ID bigint, @fnWORK_ID bigint 
		
		SELECT @fsVOL_ID = fsVOL_ID, @fnWAIT_ID = fnWAIT_ID, @fnWORK_ID = fnWORK_ID FROM INSERTED 

		INSERT [dbo].[tblMESSAGE]
			([fsFROM_ID], [fsTO_ID], [fsINFO], [_fnWAIT_ID], [_fnWORK_ID])
		VALUES
			('@SYSTEM', '@ADMINISTRATORS', '磁帶待上架通知: 請放入磁帶編號為' + @fsVOL_ID + '的磁帶至磁帶櫃中.', @fnWAIT_ID, @fnWORK_ID)

		INSERT [dbo].[tblMESSAGE]
			([fsFROM_ID], [fsTO_ID], [fsINFO], [_fnWAIT_ID], [_fnWORK_ID])
		VALUES
			('@SYSTEM', '@BIG_LIB', '磁帶待上架通知: 請放入磁帶編號為' + @fsVOL_ID + '的磁帶至磁帶櫃中.', @fnWAIT_ID, @fnWORK_ID)

		INSERT [dbo].[tblMESSAGE]
			([fsFROM_ID], [fsTO_ID], [fsINFO], [_fnWAIT_ID], [_fnWORK_ID])
		VALUES
			('@SYSTEM', '@FILE_LIB', '磁帶待上架通知: 請放入磁帶編號為' + @fsVOL_ID + '的磁帶至磁帶櫃中.', @fnWAIT_ID, @fnWORK_ID)

		----------
	END TRY
	BEGIN CATCH

	END CATCH 
END
