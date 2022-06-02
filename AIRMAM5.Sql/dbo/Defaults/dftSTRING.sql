CREATE DEFAULT [dbo].[dftSTRING]
    AS '';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[tblWORK].[fsRESULT]';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[tblWORK].[fsNOTE]';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[tblWORK].[_ITEM_TYPE]';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[tblWORK].[_ITEM_ID]';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[tblWORK].[_sDESCRIPTION]';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[tblWORK].[_SM_VOLUME_NAME]';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[t_tbmARC_INDEX].[fsSTATUS]';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[tbmARC_VIDEO_K].[fsTITLE]';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[tbmARC_VIDEO_K].[fsFILE_PATH]';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[tbmARC_VIDEO_K].[fsFILE_SIZE]';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[tbmARC_VIDEO_K].[fsFILE_TYPE]';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftSTRING]', @objname = N'[dbo].[tbmARC_VIDEO_K].[fsCREATED_BY]';

