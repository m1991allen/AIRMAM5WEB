

-- =============================================
-- 描述:	依照fsTYPE取出FUNCTIONS主檔資料
-- 記錄:	<2012/02/16><Mihsiu.Chiu><新增本預存>
--   		<2019/08/27><Rachel.Chung><增加排除刪除註記的資料fsTYPE='x'>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_FUNCTIONS_BY_TYPE]
	@fsLOGIN_ID		NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			* 
		FROM 
			tbmFUNCTIONS 
		WHERE
			fsTYPE <> 'X'
		AND
			fsFUNC_ID IN (
				SELECT 
					A.fsFUNC_ID
				FROM
					tbmFUNCTIONS A 
						LEFT JOIN tbmFUNC_GROUP B ON A.fsFUNC_ID = B.fsFUNC_ID
						JOIN (SELECT fsGROUP_ID FROM tbmUSER_GROUP WHERE fsUSER_ID = (SELECT fsUSER_ID FROM tbmUSERS WHERE fsLOGIN_ID = @fsLOGIN_ID)) C ON B.fsGROUP_ID = C.fsGROUP_ID
				GROUP BY
					A.fsFUNC_ID
				UNION ALL
				--有在系統管理者群組中
				SELECT 
					A.fsFUNC_ID
				FROM
					tbmFUNCTIONS A 
				WHERE
					(
						SELECT COUNT(1) FROM 
							(SELECT COL1 FROM dbo.fn_SLPIT((SELECT fsVALUE FROM tbzCONFIG WHERE (fsKEY = 'ADMIN_GROUPS')),';')) T1
								JOIN (SELECT fsGROUP_ID FROM tbmUSER_GROUP WHERE fsUSER_ID = (SELECT fsUSER_ID FROM tbmUSERS WHERE fsLOGIN_ID = @fsLOGIN_ID)) T2 ON T1.COL1 = T2.fsGROUP_ID
											
					) > 0
				GROUP BY
					A.fsFUNC_ID
				UNION ALL
				--母節點全抓
				SELECT 
					A.fsFUNC_ID
				FROM
					tbmFUNCTIONS A 
				WHERE
					fsTYPE = 'G'
				GROUP BY
					A.fsFUNC_ID
			)
		ORDER BY 
			fsPARENT_ID, fnORDER
END


