﻿








/*============Lucene使用========================*/


CREATE VIEW [dbo].[vwSEARCH_PHOTO_L]
AS
SELECT
	*,
	fsFILE_NO + ' ' + 
	fsTITLE + ' ' +
	fsDESCRIPTION + ' ' +
	fsCREATED_BY + ' ' +
	fsUPDATED_BY + ' ' +
	fsATTRIBUTE1 + ' ' +
	fsATTRIBUTE2 + ' ' +
	fsATTRIBUTE3 + ' ' +
	fsATTRIBUTE4 + ' ' +
	fsATTRIBUTE5 + ' ' +
	fsATTRIBUTE6 + ' ' +
	fsATTRIBUTE7 + ' ' +
	fsATTRIBUTE8 + ' ' +
	fsATTRIBUTE9 + ' ' +
	fsATTRIBUTE10 + ' ' +
	fsATTRIBUTE11 + ' ' +
	fsATTRIBUTE12 + ' ' +
	fsATTRIBUTE13 + ' ' +
	fsATTRIBUTE14 + ' ' +
	fsATTRIBUTE15 + ' ' +
	fsATTRIBUTE16 + ' ' +
	fsATTRIBUTE17 + ' ' +
	fsATTRIBUTE18 + ' ' +
	fsATTRIBUTE19 + ' ' +
	fsATTRIBUTE20 + ' ' +
	fsATTRIBUTE21 + ' ' +
	fsATTRIBUTE22 + ' ' +
	fsATTRIBUTE23 + ' ' +
	fsATTRIBUTE24 + ' ' +
	fsATTRIBUTE25 + ' ' +
	fsATTRIBUTE26 + ' ' +
	fsATTRIBUTE27 + ' ' +
	fsATTRIBUTE28 + ' ' +
	fsATTRIBUTE29 + ' ' +
	fsATTRIBUTE30 + ' ' +
	fsATTRIBUTE31 + ' ' +
	fsATTRIBUTE32 + ' ' +
	fsATTRIBUTE33 + ' ' +
	fsATTRIBUTE34 + ' ' +
	fsATTRIBUTE35 + ' ' +
	fsATTRIBUTE36 + ' ' +
	fsATTRIBUTE37 + ' ' +
	fsATTRIBUTE38 + ' ' +
	fsATTRIBUTE39 + ' ' +
	fsATTRIBUTE40 + ' ' +
	fsATTRIBUTE41 + ' ' +
	fsATTRIBUTE42 + ' ' +
	fsATTRIBUTE43 + ' ' +
	fsATTRIBUTE44 + ' ' +
	fsATTRIBUTE45 + ' ' +
	fsATTRIBUTE46 + ' ' +
	fsATTRIBUTE47 + ' ' +
	fsATTRIBUTE48 + ' ' +
	fsATTRIBUTE49 + ' ' +
	fsATTRIBUTE50 + ' ' +
	fsATTRIBUTE51 + ' ' +
	fsATTRIBUTE52 + ' ' +
	fsATTRIBUTE53 + ' ' +
	fsATTRIBUTE54 + ' ' +
	fsATTRIBUTE55 + ' ' +
	fsATTRIBUTE56 + ' ' +
	fsATTRIBUTE57 + ' ' +
	fsATTRIBUTE58 + ' ' +
	fsATTRIBUTE59 + ' ' +
	fsATTRIBUTE60 + ' ' +
	fsDIRECTORIES_NAME + ' ' +
	fsDIRECTORIES_DESCRIPTION + ' ' +
	fsTEMPLATE_FIELD_VALUE AS fsCONTENT_ALL
FROM
(
	SELECT     
		dbo.tbmARC_PHOTO.fsFILE_NO AS fsSYS_ID, 
		dbo.tbmARC_PHOTO.fsFILE_NO,
		ISNULL(dbo.tbmARC_PHOTO.fsTITLE, '') AS fsTITLE, 
	    ISNULL(dbo.tbmARC_PHOTO.fsDESCRIPTION, '') AS fsDESCRIPTION, 
		dbo.tbmARC_PHOTO.fnFILE_SECRET AS fnFILE_SECRET,
		CONVERT(VARCHAR(10), ISNULL(dbo.tbmARC_PHOTO.fdCREATED_DATE, ''), 111) AS fdCREATED_DATE, 
	    ISNULL(dbo.tbmARC_PHOTO.fsCREATED_BY, '') AS fsCREATED_BY, 
		CONVERT(VARCHAR(10), ISNULL(dbo.tbmARC_PHOTO.fdUPDATED_DATE, ''), 111) AS fdUPDATED_DATE, 
		ISNULL(dbo.tbmARC_PHOTO.fsUPDATED_BY, '') AS fsUPDATED_BY, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE1, '') AS fsATTRIBUTE1, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE2, '') AS fsATTRIBUTE2, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE3, '') AS fsATTRIBUTE3, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE4, '') AS fsATTRIBUTE4, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE5, '') AS fsATTRIBUTE5, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE6, '') AS fsATTRIBUTE6, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE7, '') AS fsATTRIBUTE7, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE8, '') AS fsATTRIBUTE8, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE9, '') AS fsATTRIBUTE9, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE10, '') AS fsATTRIBUTE10, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE11, '') AS fsATTRIBUTE11, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE12, '') AS fsATTRIBUTE12, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE13, '') AS fsATTRIBUTE13, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE14, '') AS fsATTRIBUTE14, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE15, '') AS fsATTRIBUTE15, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE16, '') AS fsATTRIBUTE16, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE17, '') AS fsATTRIBUTE17, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE18, '') AS fsATTRIBUTE18, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE19, '') AS fsATTRIBUTE19, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE20, '') AS fsATTRIBUTE20, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE21, '') AS fsATTRIBUTE21, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE22, '') AS fsATTRIBUTE22, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE23, '') AS fsATTRIBUTE23, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE24, '') AS fsATTRIBUTE24, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE25, '') AS fsATTRIBUTE25, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE26, '') AS fsATTRIBUTE26, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE27, '') AS fsATTRIBUTE27, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE28, '') AS fsATTRIBUTE28, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE29, '') AS fsATTRIBUTE29, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE30, '') AS fsATTRIBUTE30, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE31, '') AS fsATTRIBUTE31, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE32, '') AS fsATTRIBUTE32, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE33, '') AS fsATTRIBUTE33, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE34, '') AS fsATTRIBUTE34, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE35, '') AS fsATTRIBUTE35, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE36, '') AS fsATTRIBUTE36, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE37, '') AS fsATTRIBUTE37, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE38, '') AS fsATTRIBUTE38, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE39, '') AS fsATTRIBUTE39, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE40, '') AS fsATTRIBUTE40, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE41, '') AS fsATTRIBUTE41, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE42, '') AS fsATTRIBUTE42, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE43, '') AS fsATTRIBUTE43, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE44, '') AS fsATTRIBUTE44, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE45, '') AS fsATTRIBUTE45, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE46, '') AS fsATTRIBUTE46, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE47, '') AS fsATTRIBUTE47, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE48, '') AS fsATTRIBUTE48, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE49, '') AS fsATTRIBUTE49, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE50, '') AS fsATTRIBUTE50,
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE51, '') AS fsATTRIBUTE51, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE52, '') AS fsATTRIBUTE52, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE53, '') AS fsATTRIBUTE53, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE54, '') AS fsATTRIBUTE54, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE55, '') AS fsATTRIBUTE55, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE56, '') AS fsATTRIBUTE56, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE57, '') AS fsATTRIBUTE57, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE58, '') AS fsATTRIBUTE58, 
		ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE59, '') AS fsATTRIBUTE59, 
	    ISNULL(dbo.tbmARC_PHOTO.fsATTRIBUTE60, '') AS fsATTRIBUTE60,  
		ISNULL(dbo.tbmDIRECTORIES.fsNAME, '') AS fsDIRECTORIES_NAME, 
	    ISNULL(dbo.tbmDIRECTORIES.fsDESCRIPTION, '') AS fsDIRECTORIES_DESCRIPTION, 
		ISNULL(dbo.tbmDIRECTORIES.fsADMIN_GROUP, '') AS fsDIRECTORIES_ADMIN_GROUP, 
		ISNULL(dbo.tbmDIRECTORIES.fsADMIN_USER, '') AS fsDIRECTORIES_ADMIN_USER, 
		ISNULL(dbo.tbmSUBJECT.fsTYPE1, '') AS fsSUBJECT_TYPE1, 
		ISNULL(dbo.tbmSUBJECT.fsTYPE2, '') AS fsSUBJECT_TYPE2, 
		ISNULL(dbo.tbmSUBJECT.fsTYPE3, '') AS fsSUBJECT_TYPE3, 
	    'P' AS fsFILE_CATEGORY,
	    [tbdARC_PHOTO_ATTR].fsCODE_LIST AS fsTEMPLATE_FIELD_VALUE,
		dbo.fnGET_DIR_ID_BY_FILE_NO('P', dbo.tbmARC_PHOTO.fsFILE_NO) AS fsAUTHORUTY_DIR_ID,
		tbmDIRECTORIES.fnTEMP_ID_PHOTO AS fnTEMP_ID
	FROM         dbo.tbmARC_PHOTO INNER JOIN
	                      dbo.tbmSUBJECT ON dbo.tbmARC_PHOTO.fsSUBJECT_ID = dbo.tbmSUBJECT.fsSUBJ_ID INNER JOIN
	                      dbo.tbmDIRECTORIES ON dbo.tbmSUBJECT.fnDIR_ID = dbo.tbmDIRECTORIES.fnDIR_ID INNER JOIN
						  [dbo].[tbdARC_PHOTO_ATTR] ON tbmARC_PHOTO.fsFILE_NO = [tbdARC_PHOTO_ATTR].fsFILE_NO
	WHERE
		fsFILE_STATUS = 'Y'
) T	






