CREATE TABLE [dbo].[tbmDIRECTORIES] (
    [fnDIR_ID]          BIGINT         IDENTITY (1, 1) NOT NULL,
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
    [fsADMIN_GROUP]     VARCHAR (MAX)  NULL,
    [fsADMIN_USER]      VARCHAR (MAX)  NULL,
    [fsSHOWTYPE]        CHAR (1)       NOT NULL,
    [fdCREATED_DATE]    DATETIME       NOT NULL,
    [fsCREATED_BY]      VARCHAR (50)   NOT NULL,
    [fdUPDATED_DATE]    DATETIME       NULL,
    [fsUPDATED_BY]      VARCHAR (50)   NULL,
    CONSTRAINT [PK_tbmDIRECTORIES] PRIMARY KEY CLUSTERED ([fnDIR_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_1]
    ON [dbo].[tbmDIRECTORIES]([fnPARENT_ID] ASC, [fsDIRTYPE] ASC, [fsSHOWTYPE] ASC);


GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2019/05/10>
-- Description:	<目錄異動紀錄觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigDIRECTORIES_TRAN]
ON [dbo].[tbmDIRECTORIES]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @YEAR INT
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmDIRECTORIES]
	SELECT *,'U',GETDATE(),(SELECT fsUPDATED_BY FROM INSERTED WHERE fnDIR_ID = DELETED.fnDIR_ID) 
	FROM DELETED WHERE fsUPDATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END
IF (@T_DELETE_NUM > 0 AND @T_INSERT_NUM = 0)
BEGIN

	INSERT INTO [log].[tbmDIRECTORIES]
	SELECT *,'D',GETDATE(),CAST(CONTEXT_INFO() AS VARCHAR(50)) FROM DELETED

END
IF (@T_DELETE_NUM = 0 AND @T_INSERT_NUM > 0)
BEGIN

	INSERT INTO [log].[tbmDIRECTORIES]
	SELECT *,'I',GETDATE(),fsCREATED_BY FROM INSERTED WHERE fsCREATED_BY IN (SELECT fsLOGIN_ID FROM tbmUSERS)

END

GO
-- =============================================
-- Author:		<David.Sin>
-- Create date: <2011/11/11>
-- Description:	<DIR漸進式索引觸發程序>
-- =============================================

CREATE TRIGGER [dbo].[trigDIRECTORIES_INCREMENTAL]
ON [dbo].[tbmDIRECTORIES]
FOR INSERT, UPDATE, DELETE
AS
DECLARE @INSERTCOUNT INT, @DELETECOUNT INT, @T_INSERT_NUM INT, @T_DELETE_NUM INT
DECLARE @TABLE_VIDEO TABLE(fsSYS_ID VARCHAR(20),fnYEAR INT)
DECLARE @TABLE_AUDIO TABLE(fsSYS_ID VARCHAR(20),fnYEAR INT)
DECLARE @TABLE_PHOTO TABLE(fsSYS_ID VARCHAR(20),fnYEAR INT)
DECLARE @TABLE_DOC TABLE(fsSYS_ID VARCHAR(20),fnYEAR INT)
DECLARE @TABLE_SUBJECT TABLE(fsSYS_ID VARCHAR(20))
IF @@ROWCOUNT = 0 RETURN
SET @T_DELETE_NUM = (SELECT COUNT(*) FROM DELETED)
IF (@T_DELETE_NUM > 0)
BEGIN
	
	DELETE FROM @TABLE_VIDEO
	DELETE FROM @TABLE_AUDIO
	DELETE FROM @TABLE_PHOTO
	DELETE FROM @TABLE_DOC
	DELETE FROM @TABLE_SUBJECT

	--先取得要修改的SVAPD
	INSERT INTO @TABLE_VIDEO 
		SELECT 
			[tbmARC_VIDEO].[fsFILE_NO],YEAR([tbmARC_VIDEO].[fdCREATED_DATE])
		FROM 
			[dbo].[tbmARC_VIDEO] JOIN 
				(SELECT [tbmSUBJECT].[fsSUBJ_ID] 
					FROM [dbo].[tbmSUBJECT] JOIN [dbo].[tbmDIRECTORIES] ON 
						[tbmSUBJECT].[fnDIR_ID] = [tbmDIRECTORIES].[fnDIR_ID] 
					WHERE [tbmDIRECTORIES].[fnDIR_ID] IN (SELECT fnDIR_ID FROM DELETED)
				) AS [TBSUBJECT]
			 ON [tbmARC_VIDEO].[fsSUBJECT_ID] = [TBSUBJECT].[fsSUBJ_ID]
		
	INSERT INTO @TABLE_AUDIO 
		SELECT 
			[tbmARC_AUDIO].[fsFILE_NO],YEAR([tbmARC_AUDIO].[fdCREATED_DATE])
		FROM 
			[dbo].[tbmARC_AUDIO] JOIN 
				(SELECT [tbmSUBJECT].[fsSUBJ_ID] 
					FROM [dbo].[tbmSUBJECT] JOIN [dbo].[tbmDIRECTORIES] ON 
						[tbmSUBJECT].[fnDIR_ID] = [tbmDIRECTORIES].[fnDIR_ID] 
					WHERE [tbmDIRECTORIES].[fnDIR_ID] IN (SELECT fnDIR_ID FROM DELETED)
				) AS [TBSUBJECT]
			 ON [tbmARC_AUDIO].[fsSUBJECT_ID] = [TBSUBJECT].[fsSUBJ_ID]
	
	INSERT INTO @TABLE_PHOTO 
		SELECT 
			[tbmARC_PHOTO].[fsFILE_NO],YEAR([tbmARC_PHOTO].[fdCREATED_DATE])
		FROM 
			[dbo].[tbmARC_PHOTO] JOIN 
				(SELECT [tbmSUBJECT].[fsSUBJ_ID] 
					FROM [dbo].[tbmSUBJECT] JOIN [dbo].[tbmDIRECTORIES] ON 
						[tbmSUBJECT].[fnDIR_ID] = [tbmDIRECTORIES].[fnDIR_ID] 
					WHERE [tbmDIRECTORIES].[fnDIR_ID] IN (SELECT fnDIR_ID FROM DELETED)
				) AS [TBSUBJECT]
			 ON [tbmARC_PHOTO].[fsSUBJECT_ID] = [TBSUBJECT].[fsSUBJ_ID]
	
	INSERT INTO @TABLE_DOC 
		SELECT 
			[tbmARC_DOC].[fsFILE_NO],YEAR([tbmARC_DOC].[fdCREATED_DATE])
		FROM 
			[dbo].[tbmARC_DOC] JOIN 
				(SELECT [tbmSUBJECT].[fsSUBJ_ID] 
					FROM [dbo].[tbmSUBJECT] JOIN [dbo].[tbmDIRECTORIES] ON 
						[tbmSUBJECT].[fnDIR_ID] = [tbmDIRECTORIES].[fnDIR_ID] 
					WHERE [tbmDIRECTORIES].[fnDIR_ID] IN (SELECT fnDIR_ID FROM DELETED)
				) AS [TBSUBJECT]
			 ON [tbmARC_DOC].[fsSUBJECT_ID] = [TBSUBJECT].[fsSUBJ_ID]
	
	INSERT INTO @TABLE_SUBJECT
		SELECT 
			[tbmSUBJECT].[fsSUBJ_ID]
		FROM
			[dbo].[tbmSUBJECT] JOIN [dbo].[tbmDIRECTORIES] ON 
						[tbmSUBJECT].[fnDIR_ID] = [tbmDIRECTORIES].[fnDIR_ID] 
		WHERE 
			[tbmDIRECTORIES].[fnDIR_ID] IN (SELECT fnDIR_ID FROM DELETED)

	--開始新增至異動表
	IF EXISTS(SELECT fsSYS_ID FROM @TABLE_VIDEO)
	BEGIN
		INSERT INTO [dbo].[tbdARC_VIDEO_DIFFERENT](fsSYS_ID,Mode) 
			SELECT fsSYS_ID,3 FROM @TABLE_VIDEO
	END

	IF EXISTS(SELECT fsSYS_ID FROM @TABLE_AUDIO)
	BEGIN
		INSERT INTO [dbo].[tbdARC_AUDIO_DIFFERENT](fsSYS_ID,Mode) 
			SELECT fsSYS_ID,3 FROM @TABLE_AUDIO
	END

	IF EXISTS(SELECT fsSYS_ID FROM @TABLE_PHOTO)
	BEGIN
		INSERT INTO [dbo].[tbdARC_PHOTO_DIFFERENT](fsSYS_ID,Mode) 
			SELECT fsSYS_ID,3 FROM @TABLE_PHOTO
	END

	IF EXISTS(SELECT fsSYS_ID FROM @TABLE_DOC)
	BEGIN
		INSERT INTO [dbo].[tbdARC_DOC_DIFFERENT](fsSYS_ID,Mode) 
			SELECT fsSYS_ID,3 FROM @TABLE_DOC
	END

	IF EXISTS(SELECT fsSYS_ID FROM @TABLE_SUBJECT)
	BEGIN
		INSERT INTO [dbo].[tbdARC_SUBJECT_DIFFERENT](fsSYS_ID,Mode) 
			SELECT fsSYS_ID,3 FROM @TABLE_SUBJECT
	END

END		
SET @T_INSERT_NUM = (SELECT COUNT(*) FROM INSERTED)
IF (@T_INSERT_NUM > 0)
BEGIN

	DELETE FROM @TABLE_VIDEO
	DELETE FROM @TABLE_AUDIO
	DELETE FROM @TABLE_PHOTO
	DELETE FROM @TABLE_DOC
	DELETE FROM @TABLE_SUBJECT

	--先取得要修改的SVAPD
	INSERT INTO @TABLE_VIDEO 
		SELECT 
			[tbmARC_VIDEO].[fsFILE_NO],YEAR([tbmARC_VIDEO].[fdCREATED_DATE])
		FROM 
			[dbo].[tbmARC_VIDEO] JOIN 
				(SELECT [tbmSUBJECT].[fsSUBJ_ID] 
					FROM [dbo].[tbmSUBJECT] JOIN [dbo].[tbmDIRECTORIES] ON 
						[tbmSUBJECT].[fnDIR_ID] = [tbmDIRECTORIES].[fnDIR_ID] 
					WHERE [tbmDIRECTORIES].[fnDIR_ID] IN (SELECT fnDIR_ID FROM INSERTED)
				) AS [TBSUBJECT]
			 ON [tbmARC_VIDEO].[fsSUBJECT_ID] = [TBSUBJECT].[fsSUBJ_ID] AND [tbmARC_VIDEO].[fsFILE_STATUS] = 'Y'
		
	INSERT INTO @TABLE_AUDIO 
		SELECT 
			[tbmARC_AUDIO].[fsFILE_NO],YEAR([tbmARC_AUDIO].[fdCREATED_DATE])
		FROM 
			[dbo].[tbmARC_AUDIO] JOIN 
				(SELECT [tbmSUBJECT].[fsSUBJ_ID] 
					FROM [dbo].[tbmSUBJECT] JOIN [dbo].[tbmDIRECTORIES] ON 
						[tbmSUBJECT].[fnDIR_ID] = [tbmDIRECTORIES].[fnDIR_ID] 
					WHERE [tbmDIRECTORIES].[fnDIR_ID] IN (SELECT fnDIR_ID FROM INSERTED)
				) AS [TBSUBJECT]
			 ON [tbmARC_AUDIO].[fsSUBJECT_ID] = [TBSUBJECT].[fsSUBJ_ID] AND [tbmARC_AUDIO].[fsFILE_STATUS] = 'Y'
	
	INSERT INTO @TABLE_PHOTO 
		SELECT 
			[tbmARC_PHOTO].[fsFILE_NO],YEAR([tbmARC_PHOTO].[fdCREATED_DATE])
		FROM 
			[dbo].[tbmARC_PHOTO] JOIN 
				(SELECT [tbmSUBJECT].[fsSUBJ_ID] 
					FROM [dbo].[tbmSUBJECT] JOIN [dbo].[tbmDIRECTORIES] ON 
						[tbmSUBJECT].[fnDIR_ID] = [tbmDIRECTORIES].[fnDIR_ID] 
					WHERE [tbmDIRECTORIES].[fnDIR_ID] IN (SELECT fnDIR_ID FROM INSERTED)
				) AS [TBSUBJECT]
			 ON [tbmARC_PHOTO].[fsSUBJECT_ID] = [TBSUBJECT].[fsSUBJ_ID] AND [tbmARC_PHOTO].[fsFILE_STATUS] = 'Y'
	
	INSERT INTO @TABLE_DOC 
		SELECT 
			[tbmARC_DOC].[fsFILE_NO],YEAR([tbmARC_DOC].[fdCREATED_DATE])
		FROM 
			[dbo].[tbmARC_DOC] JOIN 
				(SELECT [tbmSUBJECT].[fsSUBJ_ID] 
					FROM [dbo].[tbmSUBJECT] JOIN [dbo].[tbmDIRECTORIES] ON 
						[tbmSUBJECT].[fnDIR_ID] = [tbmDIRECTORIES].[fnDIR_ID] 
					WHERE [tbmDIRECTORIES].[fnDIR_ID] IN (SELECT fnDIR_ID FROM INSERTED)
				) AS [TBSUBJECT]
			 ON [tbmARC_DOC].[fsSUBJECT_ID] = [TBSUBJECT].[fsSUBJ_ID] AND [tbmARC_DOC].[fsFILE_STATUS] = 'Y'
	
	INSERT INTO @TABLE_SUBJECT
		SELECT 
			[tbmSUBJECT].[fsSUBJ_ID]
		FROM
			[dbo].[tbmSUBJECT] JOIN [dbo].[tbmDIRECTORIES] ON 
						[tbmSUBJECT].[fnDIR_ID] = [tbmDIRECTORIES].[fnDIR_ID] 
		WHERE 
			[tbmDIRECTORIES].[fnDIR_ID] IN (SELECT fnDIR_ID FROM INSERTED)

	--開始新增至異動表
	IF EXISTS(SELECT fsSYS_ID FROM @TABLE_VIDEO)
	BEGIN
		INSERT INTO [dbo].[tbdARC_VIDEO_DIFFERENT](fsSYS_ID,Mode) 
			SELECT fsSYS_ID,1 FROM @TABLE_VIDEO
	END

	IF EXISTS(SELECT fsSYS_ID FROM @TABLE_AUDIO)
	BEGIN
		INSERT INTO [dbo].[tbdARC_AUDIO_DIFFERENT](fsSYS_ID,Mode) 
			SELECT fsSYS_ID,1 FROM @TABLE_AUDIO
	END

	IF EXISTS(SELECT fsSYS_ID FROM @TABLE_PHOTO)
	BEGIN
		INSERT INTO [dbo].[tbdARC_PHOTO_DIFFERENT](fsSYS_ID,Mode) 
			SELECT fsSYS_ID,1 FROM @TABLE_PHOTO
	END

	IF EXISTS(SELECT fsSYS_ID FROM @TABLE_DOC)
	BEGIN
		INSERT INTO [dbo].[tbdARC_DOC_DIFFERENT](fsSYS_ID,Mode) 
			SELECT fsSYS_ID,1 FROM @TABLE_DOC
	END

	IF EXISTS(SELECT fsSYS_ID FROM @TABLE_SUBJECT)
	BEGIN
		INSERT INTO [dbo].[tbdARC_SUBJECT_DIFFERENT](fsSYS_ID,Mode) 
			SELECT fsSYS_ID,1 FROM @TABLE_SUBJECT
	END
END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'目錄ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fnDIR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'目錄名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fsNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'父階目錄ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fnPARENT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'目錄描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fsDESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'目錄類型(取代QUEUE)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fsDIRTYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'同一父階中顯示順序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fnORDER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位樣板ID_主檔', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fnTEMP_ID_SUBJECT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位樣板ID_影
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fnTEMP_ID_VIDEO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位樣板ID_音
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fnTEMP_ID_AUDIO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位樣板ID_圖
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fnTEMP_ID_PHOTO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自訂欄位樣板ID_文
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fnTEMP_ID_DOC';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fdCREATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建檔人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fsCREATED_BY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fdUPDATED_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新人員帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'tbmDIRECTORIES', @level2type = N'COLUMN', @level2name = N'fsUPDATED_BY';

