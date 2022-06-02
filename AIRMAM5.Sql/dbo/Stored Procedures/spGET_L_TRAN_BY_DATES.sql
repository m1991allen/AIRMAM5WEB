

-- =============================================
-- 描述:	依照異動日期取出L_TRAN主檔資料
-- 記錄:	<2019/03/13><David.Sin><新增本預存>
-- 記錄:	<2019/06/03><David.Sin><直接從log & t_取出異動資料>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_L_TRAN_BY_DATES]
	@fdDATE1	VARCHAR(10),
	@fdDATE2	VARCHAR(10)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			T.fnID,
			T.fsTABLE,
			T.fsTABLE_NAME,
			T.fsFILE_NO,
			T.fsTITLE,
			T.fdACTION_DATE,
			T.fsACTION_BY,
			ISNULL(fsACTION_BY_NAME,'') AS fsACTION_BY_NAME,
			fsACTION
		FROM
		(	--新增的
			SELECT
				[tbmARC_VIDEO].fnID,
				'tbmARC_VIDEO' AS fsTABLE,
				'影片' AS fsTABLE_NAME,
				[tbmARC_VIDEO].fsFILE_NO,
				[tbmARC_VIDEO].fsTITLE,
				[tbmARC_VIDEO].fdOP_DATE AS fdACTION_DATE,
				[tbmARC_VIDEO].fsOP_BY AS fsACTION_BY,
				USERS_CRT.fsNAME AS fsACTION_BY_NAME,
				CASE [tbmARC_VIDEO].fcMODE
					WHEN 'I' THEN '新增' 
					WHEN 'U' THEN '修改'
					WHEN 'D' THEN '刪除'
				END AS fsACTION
			FROM
				[log].[tbmARC_VIDEO]
					LEFT JOIN tbmUSERS USERS_CRT ON [tbmARC_VIDEO].fsOP_BY = USERS_CRT.fsLOGIN_ID
			WHERE
				(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_VIDEO].fdOP_DATE,111) >= @fdDATE1) AND
				(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_VIDEO].fdOP_DATE,111) <= @fdDATE2)
			UNION ALL
			SELECT
				[tbmARC_AUDIO].fnID,
				'tbmARC_AUDIO' AS fsTABLE,
				'聲音' AS fsTABLE_NAME,
				[tbmARC_AUDIO].fsFILE_NO,
				[tbmARC_AUDIO].fsTITLE,
				[tbmARC_AUDIO].fdOP_DATE AS fdACTION_DATE,
				[tbmARC_AUDIO].fsOP_BY AS fsACTION_BY,
				USERS_CRT.fsNAME AS fsACTION_BY_NAME,
				CASE [tbmARC_AUDIO].fcMODE
					WHEN 'I' THEN '新增' 
					WHEN 'U' THEN '修改'
					WHEN 'D' THEN '刪除'
				END AS fsACTION
			FROM
				[log].[tbmARC_AUDIO]
					LEFT JOIN tbmUSERS USERS_CRT ON [tbmARC_AUDIO].fsOP_BY = USERS_CRT.fsLOGIN_ID
			WHERE
				(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_AUDIO].fdOP_DATE,111) >= @fdDATE1) AND
				(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_AUDIO].fdOP_DATE,111) <= @fdDATE2)
			UNION ALL
			SELECT
				[tbmARC_PHOTO].fnID,
				'tbmARC_PHOTO' AS fsTABLE,
				'圖片' AS fsTABLE_NAME,
				[tbmARC_PHOTO].fsFILE_NO,
				[tbmARC_PHOTO].fsTITLE,
				[tbmARC_PHOTO].fdOP_DATE AS fdACTION_DATE,
				[tbmARC_PHOTO].fsOP_BY AS fsACTION_BY,
				USERS_CRT.fsNAME AS fsACTION_BY_NAME,
				CASE [tbmARC_PHOTO].fcMODE
					WHEN 'I' THEN '新增' 
					WHEN 'U' THEN '修改'
					WHEN 'D' THEN '刪除'
				END AS fsACTION
			FROM
				[log].[tbmARC_PHOTO]
					LEFT JOIN tbmUSERS USERS_CRT ON [tbmARC_PHOTO].fsOP_BY = USERS_CRT.fsLOGIN_ID
			WHERE
				(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_PHOTO].fdOP_DATE,111) >= @fdDATE1) AND
				(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_PHOTO].fdOP_DATE,111) <= @fdDATE2)
			UNION ALL
			SELECT
				[tbmARC_DOC].fnID,
				'tbmARC_DOC' AS fsTABLE,
				'文件' AS fsTABLE_NAME,
				[tbmARC_DOC].fsFILE_NO,
				[tbmARC_DOC].fsTITLE,
				[tbmARC_DOC].fdOP_DATE AS fdACTION_DATE,
				[tbmARC_DOC].fsOP_BY AS fsACTION_BY,
				USERS_CRT.fsNAME AS fsACTION_BY_NAME,
				CASE [tbmARC_DOC].fcMODE
					WHEN 'I' THEN '新增' 
					WHEN 'U' THEN '修改'
					WHEN 'D' THEN '刪除'
				END AS fsACTION
			FROM
				[log].[tbmARC_DOC]
					LEFT JOIN tbmUSERS USERS_CRT ON [tbmARC_DOC].fsOP_BY = USERS_CRT.fsLOGIN_ID
			WHERE
				(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_DOC].fdOP_DATE,111) >= @fdDATE1) AND
				(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_DOC].fdOP_DATE,111) <= @fdDATE2)
			UNION ALL
			SELECT
				[tbmSUBJECT].fnID,
				'tbmSUBJECT' AS fsTABLE,
				'主題' AS fsTABLE_NAME,
				[tbmSUBJECT].fsSUBJ_ID AS fsFILE_NO,
				[tbmSUBJECT].fsTITLE,
				[tbmSUBJECT].fdOP_DATE AS fdACTION_DATE,
				[tbmSUBJECT].fsOP_BY AS fsACTION_BY,
				USERS_CRT.fsNAME AS fsACTION_BY_NAME,
				CASE [tbmSUBJECT].fcMODE
					WHEN 'I' THEN '新增' 
					WHEN 'U' THEN '修改'
					WHEN 'D' THEN '刪除'
				END AS fsACTION
			FROM
				[log].[tbmSUBJECT]
					LEFT JOIN tbmUSERS USERS_CRT ON [tbmSUBJECT].fsOP_BY = USERS_CRT.fsLOGIN_ID
			WHERE
				(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmSUBJECT].fdOP_DATE,111) >= @fdDATE1) AND
				(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmSUBJECT].fdOP_DATE,111) <= @fdDATE2)
			--UNION ALL
			----修改的
			--SELECT
			--	[tbmARC_VIDEO].fnID,
			--	'tbmARC_VIDEO' AS fsTABLE,
			--	'影片' AS fsTABLE_NAME,
			--	[tbmARC_VIDEO].fsFILE_NO,
			--	[tbmARC_VIDEO].fsTITLE,
			--	[tbmARC_VIDEO].fdOP_DATE AS fdACTION_DATE,
			--	[tbmARC_VIDEO].fsOP_BY AS fsACTION_BY,
			--	USERS_UPD.fsNAME AS fsACTION_BY_NAME,
			--	'修改' AS fsACTION
			--FROM
			--	[log].[tbmARC_VIDEO]
			--		LEFT JOIN tbmUSERS USERS_UPD ON [tbmARC_VIDEO].fsOP_BY = USERS_UPD.fsLOGIN_ID
			--WHERE
			--	[tbmARC_VIDEO].fcMODE = 'U' AND
			--	(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_VIDEO].fdOP_DATE,111) >= @fdDATE1) AND
			--	(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_VIDEO].fdOP_DATE,111) <= @fdDATE2)
			--UNION ALL
			--SELECT
			--	[tbmARC_AUDIO].fnID,
			--	'tbmARC_AUDIO' AS fsTABLE,
			--	'聲音' AS fsTABLE_NAME,
			--	[tbmARC_AUDIO].fsFILE_NO,
			--	[tbmARC_AUDIO].fsTITLE,
			--	[tbmARC_AUDIO].fdOP_DATE AS fdACTION_DATE,
			--	[tbmARC_AUDIO].fsOP_BY AS fsACTION_BY,
			--	USERS_UPD.fsNAME AS fsACTION_BY_NAME,
			--	'修改' AS fsACTION
			--FROM
			--	[log].[tbmARC_AUDIO]
			--		LEFT JOIN tbmUSERS USERS_UPD ON [tbmARC_AUDIO].fsOP_BY = USERS_UPD.fsLOGIN_ID
			--WHERE
			--	[tbmARC_AUDIO].fcMODE = 'U' AND
			--	(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_AUDIO].fdOP_DATE,111) >= @fdDATE1) AND
			--	(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_AUDIO].fdOP_DATE,111) <= @fdDATE2)
			--UNION ALL
			--SELECT
			--	[tbmARC_PHOTO].fnID,
			--	'tbmARC_PHOTO' AS fsTABLE,
			--	'圖片' AS fsTABLE_NAME,
			--	[tbmARC_PHOTO].fsFILE_NO,
			--	[tbmARC_PHOTO].fsTITLE,
			--	[tbmARC_PHOTO].fdOP_DATE AS fdACTION_DATE,
			--	[tbmARC_PHOTO].fsOP_BY AS fsACTION_BY,
			--	USERS_UPD.fsNAME AS fsACTION_BY_NAME,
			--	'修改' AS fsACTION
			--FROM
			--	[log].[tbmARC_PHOTO]
			--		LEFT JOIN tbmUSERS USERS_UPD ON [tbmARC_PHOTO].fsOP_BY = USERS_UPD.fsLOGIN_ID
			--WHERE
			--	[tbmARC_PHOTO].fcMODE = 'U' AND
			--	(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_PHOTO].fdOP_DATE,111) >= @fdDATE1) AND
			--	(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_PHOTO].fdOP_DATE,111) <= @fdDATE2)
			--UNION ALL
			--SELECT
			--	[tbmARC_DOC].fnID,
			--	'tbmARC_DOC' AS fsTABLE,
			--	'文件' AS fsTABLE_NAME,
			--	[tbmARC_DOC].fsFILE_NO,
			--	[tbmARC_DOC].fsTITLE,
			--	[tbmARC_DOC].fdOP_DATE AS fdACTION_DATE,
			--	[tbmARC_DOC].fsOP_BY AS fsACTION_BY,
			--	USERS_UPD.fsNAME AS fsACTION_BY_NAME,
			--	'修改' AS fsACTION
			--FROM
			--	[log].[tbmARC_DOC]
			--		LEFT JOIN tbmUSERS USERS_UPD ON [tbmARC_DOC].fsOP_BY = USERS_UPD.fsLOGIN_ID
			--WHERE
			--	[tbmARC_DOC].fcMODE = 'U' AND
			--	(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_DOC].fdOP_DATE,111) >= @fdDATE1) AND
			--	(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_DOC].fdOP_DATE,111) <= @fdDATE2)
			----刪除的
			--UNION ALL
			--SELECT
			--	[tbmARC_VIDEO].fnID,
			--	'tbmARC_VIDEO' AS fsTABLE,
			--	'影片' AS fsTABLE_NAME,
			--	[tbmARC_VIDEO].fsFILE_NO,
			--	[tbmARC_VIDEO].fsTITLE,
			--	[tbmARC_VIDEO].fdOP_DATE AS fdACTION_DATE,
			--	[tbmARC_VIDEO].fsOP_BY AS fsACTION_BY,
			--	USERS_UPD.fsNAME AS fsACTION_BY_NAME,
			--	'刪除' AS fsACTION
			--FROM
			--	[log].[tbmARC_VIDEO]
			--		LEFT JOIN tbmUSERS USERS_UPD ON [tbmARC_VIDEO].fsOP_BY = USERS_UPD.fsLOGIN_ID
			--WHERE
			--	[tbmARC_VIDEO].fcMODE = 'D' AND
			--	(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_VIDEO].fdOP_DATE,111) >= @fdDATE1) AND
			--	(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_VIDEO].fdOP_DATE,111) <= @fdDATE2)
			--UNION ALL
			--SELECT
			--	[tbmARC_AUDIO].fnID,
			--	'tbmARC_AUDIO' AS fsTABLE,
			--	'聲音' AS fsTABLE_NAME,
			--	[tbmARC_AUDIO].fsFILE_NO,
			--	[tbmARC_AUDIO].fsTITLE,
			--	[tbmARC_AUDIO].fdOP_DATE AS fdACTION_DATE,
			--	[tbmARC_AUDIO].fsOP_BY AS fsACTION_BY,
			--	USERS_CRT.fsNAME AS fsACTION_BY_NAME,
			--	'刪除' AS fsACTION
			--FROM
			--	[log].[tbmARC_AUDIO]
			--		LEFT JOIN tbmUSERS USERS_CRT ON [tbmARC_AUDIO].fsOP_BY = USERS_CRT.fsLOGIN_ID
			--WHERE
			--	[tbmARC_AUDIO].fcMODE = 'D' AND
			--	(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_AUDIO].fdOP_DATE,111) >= @fdDATE1) AND
			--	(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_AUDIO].fdOP_DATE,111) <= @fdDATE2)
			--UNION ALL
			--SELECT
			--	[tbmARC_PHOTO].fnID,
			--	'tbmARC_PHOTO' AS fsTABLE,
			--	'圖片' AS fsTABLE_NAME,
			--	[tbmARC_PHOTO].fsFILE_NO,
			--	[tbmARC_PHOTO].fsTITLE,
			--	[tbmARC_PHOTO].fdOP_DATE AS fdACTION_DATE,
			--	[tbmARC_PHOTO].fsOP_BY AS fsACTION_BY,
			--	USERS_CRT.fsNAME AS fsACTION_BY_NAME,
			--	'刪除' AS fsACTION
			--FROM
			--	[log].[tbmARC_PHOTO]
			--		LEFT JOIN tbmUSERS USERS_CRT ON [tbmARC_PHOTO].fsOP_BY = USERS_CRT.fsLOGIN_ID
			--WHERE
			--	[tbmARC_PHOTO].fcMODE = 'D' AND
			--	(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_PHOTO].fdOP_DATE,111) >= @fdDATE1) AND
			--	(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_PHOTO].fdOP_DATE,111) <= @fdDATE2)
			--UNION ALL
			--SELECT
			--	[tbmARC_DOC].fnID,
			--	'tbmARC_DOC' AS fsTABLE,
			--	'文件' AS fsTABLE_NAME,
			--	[tbmARC_DOC].fsFILE_NO,
			--	[tbmARC_DOC].fsTITLE,
			--	[tbmARC_DOC].fdOP_DATE AS fdACTION_DATE,
			--	[tbmARC_DOC].fsOP_BY AS fsACTION_BY,
			--	USERS_CRT.fsNAME AS fsACTION_BY_NAME,
			--	'刪除' AS fsACTION
			--FROM
			--	[log].[tbmARC_DOC]
			--		LEFT JOIN tbmUSERS USERS_CRT ON [tbmARC_DOC].fsOP_BY = USERS_CRT.fsLOGIN_ID
			--WHERE
			--	[tbmARC_DOC].fcMODE = 'D' AND
			--	(@fdDATE1 = '' OR CONVERT(VARCHAR(10),[tbmARC_DOC].fdOP_DATE,111) >= @fdDATE1) AND
			--	(@fdDATE2 = '' OR CONVERT(VARCHAR(10),[tbmARC_DOC].fdOP_DATE,111) <= @fdDATE2)
		) T
		ORDER BY
			T.fdACTION_DATE DESC
END


