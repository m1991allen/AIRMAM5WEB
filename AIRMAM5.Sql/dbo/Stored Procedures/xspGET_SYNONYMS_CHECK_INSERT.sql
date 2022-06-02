

-- =============================================
-- 描述:	新增SYNO主檔資料前的檢查
-- 記錄:	<2012/05/28><Dennis.Wen><新增預存>
-- 空格表示可以正常新增, 非空格之字串為錯誤訊息
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_SYNONYMS_CHECK_INSERT]
	@fsTEXT_LIST	NVARCHAR(400)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE
		@CNT	INT				= 0,
		@idx	INT				= 0,
		@fsTEXT	NVARCHAR(50)	= '',
		@RESULT	NVARCHAR(500)	= ''
		
	SET @CNT = LEN(REPLACE(@fsTEXT_LIST,';',';;')) - LEN(@fsTEXT_LIST)

	/*跑回圈檢查每一個字串是否已經在別組詞組中出現*/
	WHILE(@idx < @CNT)
	BEGIN
		SET @fsTEXT = dbo.fnGET_ITEM_BY_INDEX(@fsTEXT_LIST, @idx)
		
		IF EXISTS(SELECT * FROM tbmSYNONYMS WHERE ((';' + fsTEXT_LIST) LIKE ('%;'+ @fsTEXT +';%')))
		BEGIN
			SET @RESULT = @RESULT + @fsTEXT + ';'
		END

		SET @idx = @idx + 1
	END

	/*若字串非空表示有檢查到落在他組詞組的情況*/
	IF (@RESULT <> '')
		BEGIN
			SET @RESULT = 'ERROR:詞彙已存在其他同義詞組中:' + @RESULT
		END
	--ELSE
	--	BEGIN
	--/*若字串為空表示沒有重複情況,開始新增到資料庫*/
	--		INSERT
	--			tbmSYNONYMS
	--			(fsTEXT_LIST, fsTYPE, fsNOTE, fdALTERD_DATE, fsALTERD_BY)
	--		VALUES
	--			(@fsTEXT_LIST, @fsTYPE, @fsNOTE, GETDATE(), @fsALTERD_BY)
	--	END 

	SELECT RESULT = @RESULT
END



