


-- =============================================
-- 描述:	依照TEMP_ID取出有用到的CODE主檔資料  BY TEMP_ID AND PROG_ID FTV USE
-- 記錄:	<2016/03/17><Dennis.Wen><新增本預存>

-- =============================================
CREATE PROCEDURE [dbo].[xspGET_CODE_BY_TEMP_ID_AND_PROG_ID_FTV]
	@TEMP_ID	int,
	@FILE_NO	VARCHAR(16)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN
 		SET NOCOUNT ON;

		BEGIN TRY

			--DECLARE @fsTABLE VARCHAR(1) = (SELECT TOP 1 fsTABLE FROM [tbmTEMPLATE] WHERE (fnTEMP_ID = @TEMP_ID))

			IF (@TEMP_ID <> 26)
				BEGIN
		
					SELECT
						CODE.*
						--,t_USING_CNT = dbo.fn_t_GET_USING_CNT_BY_CODE2(CODE.fsCODE_ID, CODE.fsCODE, @fsTABLE, @TEMP_ID)
					FROM
						tbzCODE AS CODE
					WHERE
						CODE.fsCODE_ID IN (	SELECT
												DISTINCT fsCODE_ID
											FROM
												tbmTEMPLATE_FIELDS
											WHERE
												(fnTEMP_ID = @TEMP_ID) AND (fsCODE_ID <> ''))
					ORDER BY
						fsCODE_ID, fnORDER, fsCODE

				END
			ELSE	
				--用FILE_NO取SUBJ_NO去找PROG_ID
				BEGIN
					DECLARE @SUBJ_ID VARCHAR(16) = (SELECT fsSUBJECT_ID FROM tbmARC_VIDEO WHERE fsFILE_NO = @FILE_NO )
					DECLARE @PROG_ID VARCHAR(16) = (SELECT fsPROG_ID FROM [dbo].[tbx_TR_MAPPING_SUBJ] WHERE fsSUBJ_ID = @SUBJ_ID)
					CREATE TABLE #tbORI (
								[fsCODE_ID] [varchar](10) ,			[fsCODE] [varchar](20) ,			[fsNAME] [nvarchar](200) ,			[fsENAME] [varchar](200) ,
								[fnORDER] [int] ,					[fsSET] [varchar](50) ,				[fsNOTE] [nvarchar](200) ,			[fsIS_ENABLED] [varchar](1) ,
								[fsTYPE] [varchar](1) ,				[fdCREATED_DATE] [datetime] ,		[fsCREATED_BY] [nvarchar](20) ,		[fdUPDATED_DATE] [datetime] ,
								[fsUPDATED_BY] [nvarchar](20))

					INSERT #tbORI	 

					SELECT
						CODE.*
					FROM
						tbzCODE AS CODE
					WHERE
						CODE.fsCODE_ID IN (	SELECT
												DISTINCT fsCODE_ID
											FROM
												tbmTEMPLATE_FIELDS
											WHERE
												(fnTEMP_ID = @TEMP_ID) AND (fsCODE_ID <> '') AND (fsCODE_ID <> 'PROG_TYPE'))
											ORDER BY
												fsCODE_ID, fnORDER, fsCODE

					INSERT #tbORI
					SELECT 'PROG_TYPE', '-', '- 普通版', '', 99, '', '', 'Y', 'C', '1911-01-01','admin','1911-01-01',''
					
					INSERT #tbORI
					SELECT 'PROG_TYPE', fsPROG_VER_ID, fsPROG_VER_ID + ' ' + fsPROG_VER_NAME, '', 99, '', fsMEMO, 'Y', 'C', fdCREATED_DATE,'admin',fdUPDATED_DATE,''
					FROM [dbo].[tbx_TR_VIDEO_PROG_VER]
					WHERE fsPROG_ID = @PROG_ID

					SELECT * FROM #tbORI
				END

			
		END TRY
		BEGIN CATCH
			SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
		END CATCH
	END
END




