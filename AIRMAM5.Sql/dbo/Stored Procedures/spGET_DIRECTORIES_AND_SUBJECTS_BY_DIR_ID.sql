

-- =============================================
-- 描述:	取出全部 DIRECTORIES主檔資料&其主題檔 BY DIR_ID
-- 記錄:	<2013/10/09><Eric.Huang><新增>
-- 記錄:	<2014/10/16><Eric.Huang><當使用者為FTV時,要取回SUBJ fsATTRIBUTE1的欄位(集數用)>
-- 記錄:	<2016/09/14><David.Sin><修改只取需要的欄位>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID]

		@fnDIR_ID	            BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	
		
	SELECT
		tbmSUBJECT.fsSUBJ_ID, 
		tbmSUBJECT.fsTITLE,  
		tbmSUBJECT.fsDESCRIPTION,
		_nVideo		= (SELECT COUNT(fsFILE_NO) FROM tbmARC_VIDEO AS V WHERE (V.fsSUBJECT_ID = tbmSUBJECT.fsSUBJ_ID)),
		_nAudio		= (SELECT COUNT(fsFILE_NO) FROM tbmARC_AUDIO AS A WHERE (A.fsSUBJECT_ID = tbmSUBJECT.fsSUBJ_ID)),
		_nPhoto		= (SELECT COUNT(fsFILE_NO) FROM tbmARC_PHOTO AS P WHERE (P.fsSUBJECT_ID = tbmSUBJECT.fsSUBJ_ID)),
		_nDocument	= (SELECT COUNT(fsFILE_NO) FROM tbmARC_DOC AS D WHERE (D.fsSUBJECT_ID = tbmSUBJECT.fsSUBJ_ID))	
	
	FROM
		tbmSUBJECT 
			JOIN tbmDIRECTORIES ON tbmSUBJECT.fnDIR_ID = tbmDIRECTORIES.fnDIR_ID
	WHERE
		tbmDIRECTORIES.fnDIR_ID = @fnDIR_ID
	ORDER BY 
		tbmSUBJECT.fdCREATED_DATE
	
END





