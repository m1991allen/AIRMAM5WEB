

-- =============================================
-- 描述:	取出ANNOUNCE主檔資料
-- 記錄:	<2011/09/24><Mihsiu.Chiu><新增本預存>
--      	<2012/03/03><Eric.Huang><修改WHERE判斷式的日期部份(原先的方法只判斷到DATE,沒有判斷到 時跟分)>
--			<2012/03/26><Mihsiu.Chiu><修改 order by &  判斷上架期間日期方式>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ANNOUNCE_PUBLIC]
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			ANN.fnANN_ID,
			ANN.fsTITLE,
			ANN.fsCONTENT,
			ANN.fdSDATE,
			ANN.fdEDATE,
			ANN.fsTYPE, 
			ANN.fnORDER,
			ANN.fsGROUP_LIST,
			ANN.fsIS_HIDDEN,
			ANN.fsDEPT,
			ANN.fsNOTE,
			ANN.fdCREATED_DATE,
			ANN.fsCREATED_BY,
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			ANN.fdUPDATED_DATE,
			ANN.fsUPDATED_BY,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
			CASE 
				WHEN ANN.fsTYPE = '' THEN '(未選擇)' 
				ELSE ISNULL(CODE1.fsNAME, '錯誤代碼: '+ ANN.fsTYPE) 
			END AS fsTYPE_NAME,
			CASE 
				WHEN ANN.fsDEPT = '' THEN '(未選擇)' 
				ELSE ISNULL(CODE2.fsNAME, '錯誤代碼: '+ ANN.fsDEPT) 
			END AS fsDEPT_NAME
		FROM
			tbmANNOUNCE AS ANN 
				LEFT JOIN tbzCODE AS CODE1 ON ANN.fsTYPE = CODE1.fsCODE AND CODE1.fsCODE_ID = 'ANN001'
				LEFT JOIN tbzCODE AS CODE2 ON ANN.fsDEPT = CODE2.fsCODE AND CODE2.fsCODE_ID = 'DEPT001'
				LEFT JOIN tbmUSERS USERS_CRT ON ANN.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON ANN.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE
			(ANN.fsGROUP_LIST = '') AND 
			(ANN.fsIS_HIDDEN = 'N') AND 
			(fdSDATE <= GETDATE()) AND
			(fdEDATE IS NULL OR fdEDATE >= GETDATE())
			  
		ORDER BY
			ANN.[fnORDER],ANN.[fdSDATE] DESC
END



