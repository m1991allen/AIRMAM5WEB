CREATE DEFAULT [dbo].[dftDATE]
    AS '1900/01/01';


GO
EXECUTE sp_bindefault @defname = N'[dbo].[dftDATE]', @objname = N'[dbo].[tblWORK].[fdSTIME]';

