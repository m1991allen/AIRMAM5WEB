





-- =============================================
-- 描述:	修改 TEMPLATE_FIELDS 主檔 資料
-- 記錄:	<2011/09/16><Eric.Huang><新增本預存>
--      	<2011/10/03><Eric.Huang><修改欄位>
--      	<2011/10/17><Mihsiu.Chiu><修改>
--      	<2012/08/30><Mihsiu.Chiu><修改@fsCODE_ID & @fsCODE_CTRL => varchar(20)>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_TEMPLATE_FIELDS]

	@fnTEMP_ID   		INT,
	@fsFIELD			VARCHAR(50),
	@fsFIELD_NAME		NVARCHAR(50),
	@fsFIELD_TYPE		VARCHAR(50),
	@fnFIELD_LENGTH		INT,
	@fsDESCRIPTION		NVARCHAR(MAX),
	@fnORDER			INT,
	@fnCTRL_WIDTH		INT,
	@fsMULTILINE		CHAR(1),
	@fsISNULLABLE		CHAR(1),
	@fsDEFAULT			NVARCHAR(50),
	@fsCODE_ID			VARCHAR(20),
	@fnCODE_CNT			INT,
	@fsCODE_CTRL		VARCHAR(20),
	@fsIS_SEARCH		CHAR(1),
	@fsUPDATED_BY		VARCHAR(50)


AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION


		UPDATE
			tbmTEMPLATE_FIELDS
		SET		
			fsFIELD_NAME	= @fsFIELD_NAME,
			fsFIELD_TYPE	= @fsFIELD_TYPE,
			fnFIELD_LENGTH	= @fnFIELD_LENGTH,
			fsDESCRIPTION	= @fsDESCRIPTION,
			fnORDER			= @fnORDER,
			fnCTRL_WIDTH	= @fnCTRL_WIDTH,
			fsMULTILINE		= @fsMULTILINE,
			fsISNULLABLE	= @fsISNULLABLE,
			fsDEFAULT		= @fsDEFAULT,
			fsCODE_ID		= @fsCODE_ID,
			fnCODE_CNT		= @fnCODE_CNT,
			fsCODE_CTRL		= @fsCODE_CTRL,
			fsIS_SEARCH	= @fsIS_SEARCH,
            fdUPDATED_DATE  = GETDATE(),
            fsUPDATED_BY    = @fsUPDATED_BY
			
		WHERE
			(fnTEMP_ID   = @fnTEMP_ID) AND
			(fsFIELD     = @fsFIELD) 

		COMMIT	
		SELECT RESULT = ''
	END TRY
	
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END








