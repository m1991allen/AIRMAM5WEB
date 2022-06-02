﻿
-- =============================================
-- 描述:	傳入V/A/P/D回傳註冊工作項中,優先順序高的未處理工作
-- 記錄:	<2019/06/26><David.Sin><重新修改預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_WORK_COPYFILE_BY_TYPE]
	@TYPE VARCHAR(1)
AS
BEGIN
 	SET NOCOUNT ON;

	/*取得要處理的WORK*/
	DECLARE @fnWORK_ID BIGINT = 
		ISNULL(
		(
			SELECT TOP 1 fnWORK_ID 
			FROM tblWORK A JOIN tbmBOOKING B ON A.fnGROUP_ID = B.fnBOOKING_ID 
			WHERE 
				A.fdSTART_WORK_TIME <= GETDATE() AND 
				A.fsSTATUS LIKE '0%' AND 
				SUBSTRING(A.fsPARAMETERS,1,1) = @TYPE AND 
				A.fsTYPE = 'COPYFILE' AND 
				B.fsSTATUS LIKE '0%'
			ORDER BY B.fnORDER, CONVERT(INT,A.fsPRIORITY), A.fnWORK_ID
		),-1)
	

	IF(@fnWORK_ID > 0)
	BEGIN
		
		DECLARE @fsPARAMETERS VARCHAR(MAX) = (SELECT fsPARAMETERS FROM tblWORK WHERE fnWORK_ID = @fnWORK_ID)
		DECLARE @fsFILE_NO VARCHAR(16) = (SELECT dbo.fnGET_ITEM_BY_INDEX(@fsPARAMETERS,1) FROM tblWORK WHERE fnWORK_ID = @fnWORK_ID)
		DECLARE @fsPATH_TYPE VARCHAR(5) = (SELECT dbo.fnGET_ITEM_BY_INDEX(@fsPARAMETERS,2) FROM tblWORK WHERE fnWORK_ID = @fnWORK_ID)
		DECLARE @fsFOLDER VARCHAR(50) = (SELECT dbo.fnGET_ITEM_BY_INDEX(@fsPARAMETERS,3) FROM tblWORK WHERE fnWORK_ID = @fnWORK_ID)

		IF((SELECT COUNT(*) FROM tbmARC_VIDEO WHERE (@TYPE = 'V' AND fsFILE_NO = @fsFILE_NO))+
			(SELECT COUNT(*) FROM tbmARC_AUDIO WHERE (@TYPE = 'A' AND fsFILE_NO = @fsFILE_NO))+
			(SELECT COUNT(*) FROM tbmARC_PHOTO WHERE (@TYPE = 'P' AND fsFILE_NO = @fsFILE_NO))+
			(SELECT COUNT(*) FROM tbmARC_DOC WHERE (@TYPE = 'D' AND fsFILE_NO = @fsFILE_NO)) = 0)
		BEGIN
			/*若取不到媒體檔資訊,可能是被移除了,排程取消*/
			UPDATE tblWORK
			SET fsSTATUS = 'C0'
			WHERE (fnWORK_ID = @fnWORK_ID)
		END
		ELSE
		BEGIN
			
			UPDATE tblWORK
			SET fsSTATUS = '01'
			WHERE (fnWORK_ID = @fnWORK_ID)

			SELECT
				@fnWORK_ID AS WORK_ID,
				@fsFILE_NO AS FILE_NO,
				CASE
					WHEN @TYPE = 'V' THEN (SELECT fsFILE_PATH_H + fsFILE_NO + '_H.' + fsFILE_TYPE_H FROM tbmARC_VIDEO WHERE fsFILE_NO = @fsFILE_NO)
					WHEN @TYPE = 'A' THEN (SELECT fsFILE_PATH_H + fsFILE_NO + '_H.' + fsFILE_TYPE_H FROM tbmARC_AUDIO WHERE fsFILE_NO = @fsFILE_NO)
					WHEN @TYPE = 'P' THEN (SELECT fsFILE_PATH_H + fsFILE_NO + '_H.' + fsFILE_TYPE_H FROM tbmARC_PHOTO WHERE fsFILE_NO = @fsFILE_NO)
					WHEN @TYPE = 'D' THEN (SELECT fsFILE_PATH + fsFILE_NO + '.' + fsFILE_TYPE FROM tbmARC_DOC WHERE fsFILE_NO = @fsFILE_NO)
				END AS fromFILE_PATH_NAME,
				CASE
					WHEN @fsPATH_TYPE = '1' THEN 'UNC'
					WHEN @fsPATH_TYPE = '2' THEN 'FTP'
					ELSE 'UNC'
				END AS toFILE_PATH_TYPE,
				CASE
					WHEN ISNULL(U.fsBOOKING_TARGET_PATH,'') = '' THEN CODE1.fsSET
					ELSE U.fsBOOKING_TARGET_PATH
				END + B.fsCREATED_BY + @fsFOLDER + '\' AS toFILE_PATH,
				REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
					CASE
						WHEN @TYPE = 'V' THEN (SELECT fsTITLE FROM tbmARC_VIDEO WHERE fsFILE_NO = @fsFILE_NO)
						WHEN @TYPE = 'A' THEN (SELECT fsTITLE FROM tbmARC_AUDIO WHERE fsFILE_NO = @fsFILE_NO)
						WHEN @TYPE = 'P' THEN (SELECT fsTITLE FROM tbmARC_PHOTO WHERE fsFILE_NO = @fsFILE_NO)
						WHEN @TYPE = 'D' THEN (SELECT fsTITLE FROM tbmARC_DOC WHERE fsFILE_NO = @fsFILE_NO)
						ELSE @fsFILE_NO + '_' + B.fsCREATED_BY
					END,'*',''),'|',''),'\',''),':',''),'"',''),'>',''),'<',''),'?',''),'/',''
				) AS toFILE_NAME,
				A.fsSTATUS AS STATUS_0,
				'C' AS MTS_TOOL
			FROM
				tblWORK A
					JOIN tbmBOOKING B ON A.fnGROUP_ID = B.fnBOOKING_ID
					--JOIN tbmBOOKING_T C ON B.fnTEMP_ID = C.fnBOOK_T_ID
					JOIN tbmUSERS U ON B.fsCREATED_BY = U.fsLOGIN_ID
					LEFT JOIN tbzCODE CODE1 ON CODE1.fsCODE = B.fsPATH AND CODE1.fsCODE_ID = 'BOOKING_PATH'
			WHERE
				A.fnWORK_ID = @fnWORK_ID



		END

	END
END

