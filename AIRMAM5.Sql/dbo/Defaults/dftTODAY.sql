CREATE DEFAULT [dbo].[dftTODAY]
    AS GETDATE();


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftTODAY]', @objname = N'[dbo].[tbmARC_VIDEO_K].[fdCREATED_DATE]';

