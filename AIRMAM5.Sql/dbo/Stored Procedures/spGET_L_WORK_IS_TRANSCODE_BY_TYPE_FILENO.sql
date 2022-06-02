



-- =============================================
-- 描述:	取出 L_WORK 主檔資料 -檢查是否進行轉檔狀態 for 刪除檔案時判斷
-- 記錄:	<2012/05/08><Mihsiu.Chiu><新增本預存>
-- 記錄:	<2012/09/26><Eric.Huang><新增一個SWITCH變數判斷來借用此SP>
-- 記錄:	<2013/02/28><Eric.Huang><為了不要讓INGEST的影片在讀取WORK時會找不到資料,故設定將2013年起的媒體資料才要檢查WORK>
-- 記錄:	<2013/06/13><Eric.Huang><為了不要讓INGEST的影片在讀取WORK時會找不到資料,故設定將20130705之後的媒體資料才要檢查WORK>
-- 記錄:	<2013/11/22><Eric.Huang><為了不要讓INGEST的影片在讀取WORK時會找不到資料,故設定將20140501之後的媒體資料才要檢查WORK>
-- 記錄:	<2014/05/01><Eric.Huang><判斷fsTYPE多一種 VP_YOUTUBE>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_L_WORK_IS_TRANSCODE_BY_TYPE_FILENO]
	@TYPE	 varchar(1),
	@FILE_NO varchar(16),
	@SWITCH  varchar(16)
AS
BEGIN
 	SET NOCOUNT ON;

	IF (@SWITCH = 'DELETE')
		BEGIN
			SELECT RESULT = (Case COUNT(*) when 0 then 'N' else 'Y' end)

			FROM
				tblWORK 
			WHERE
				fsTYPE IN ('TRANSCODE', 'VP_YOUTUBE', 'TR_VIDEO','DAILY_ITP')
				and fsPARAMETERS like @TYPE+';'+@FILE_NO+';%'
				and (substring(fsSTATUS,1,1) >= '1' and substring(fsSTATUS,1,1) <= '8')
		END
	ELSE
		BEGIN
		
			DECLARE	@tbOriginal TABLE(
					[fnWORK_ID]	    [varchar](50),
					[fsPARAMETERS]	[varchar](50),
					[fsSTATUS]	    [varchar](2)
					)
		
			IF (CAST(SUBSTRING(@FILE_NO,1,8) AS INT) > 20140501)
				BEGIN

					SELECT	TOP 1 fnWORK_ID,fsPARAMETERS,fsSTATUS
					INTO #temp 
					FROM tblWORK 
					WHERE ( (fsTYPE     = 'TRANSCODE' OR fsTYPE     = 'DAILY_ITP' OR fsTYPE     = 'VP_YOUTUBE' OR fsTYPE     = 'TR_VIDEO')
						AND _ITEM_TYPE = @TYPE 
						AND _ITEM_ID   = @FILE_NO)
				
					ORDER BY fnWORK_ID DESC
					INSERT @tbOriginal SELECT * FROM #temp
					DROP TABLE #temp	

					SELECT RESULT = (SELECT fsSTATUS FROM @tbOriginal)
				END
			ELSE

				BEGIN

					SELECT RESULT = '90'
				END

				

			
			--SELECT * FROM @tbOriginal

			--SELECT RESULT = (Case COUNT(*) when 1 then 'N' else 'Y' end), fsSTATUS
			--FROM
			--@tbOriginal 
						----WHERE (substring(fsSTATUS,1,1) IN ('9','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'))
			--   WHERE PATINDEX('%[A-Z,a-z,9]%', (substring(fsSTATUS,1,1)))=1
			
			
		END
	
	
END





