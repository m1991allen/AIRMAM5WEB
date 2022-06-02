

-- =============================================
-- 描述:	重設調用使用
-- 記錄:	<2019/03/12><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_REBOOKING]
	@fnWORK_ID		BIGINT,	
	@fsUPDATED_BY	NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsSTATUS VARCHAR(2) = (SELECT fsSTATUS FROM tblWORK WHERE fnWORK_ID = @fnWORK_ID)
	IF (@fsSTATUS LIKE '9%' OR @fsSTATUS LIKE 'E%')
	BEGIN
		
		--先更新WORK
		UPDATE
			tblWORK
		SET
			fsSTATUS = CASE WHEN W._APPROVE_STATUS = '_C' OR W._APPROVE_STATUS IS NULL THEN '00' ELSE W._APPROVE_STATUS END,
			fsPRIORITY = '',
			fdSTIME = '',
			fdETIME = '',
			fsRESULT = '',
			fdUPDATED_DATE = GETDATE(),
			fsUPDATED_BY = @fsUPDATED_BY
		FROM
			tblWORK W 
		WHERE
			fnWORK_ID = @fnWORK_ID AND
			fnWORK_ID = W.fnWORK_ID

		--再更新BOOKING
		UPDATE
			tbmBOOKING
		SET
			fsSTATUS = '00',
			fdUPDATED_DATE = GETDATE(),
			fsUPDATED_BY = @fsUPDATED_BY
		WHERE
			fnBOOKING_ID = (SELECT fnGROUP_ID FROM tblWORK WHERE fnWORK_ID = @fnWORK_ID)

		SELECT RESULT = ''
	END
	ELSE
	BEGIN
		
		SELECT RESULT = 'ERROR:檔案正在轉檔中，無法重設調用'

	END
	--DECLARE @_nBOOKING_ID BIGINT,
 --           @_sTYPE       VARCHAR(50),
	--		@_ITEM_TYPE   VARCHAR(20),
	--		@_sSTATUS     VARCHAR(2)
	--BEGIN TRY
	--	SET NOCOUNT ON;

	--    SELECT @_nBOOKING_ID=fnGROUP_ID,@_sTYPE=fsTYPE,@_ITEM_TYPE=_ITEM_TYPE FROM tblWORK WHERE (fnWORK_ID	= @fnWORK_ID)

	--	IF (@_sTYPE='AVID')
	--	    IF (@_ITEM_TYPE ='V')
	--		  SET  @_sSTATUS='_@'
	--		ELSE
	--		  SET  @_sSTATUS='00'
	--	ELSE IF (@_sTYPE ='NAS')
	--			IF (@_ITEM_TYPE ='V')
	--			  SET  @_sSTATUS='_C'
	--			ELSE
	--			  SET  @_sSTATUS='00'
	--	ELSE  --MTS(BOOKING OR COPYFILE)
	--	    IF (@_ITEM_TYPE ='V')
	--		  SET  @_sSTATUS='_C'
	--		ELSE
	--		  SET  @_sSTATUS='00'


	--	UPDATE
	--		tblWORK
	--	SET
	--	    fsSTATUS        = @_sSTATUS,
	--		fsPROGRESS      = '0%',
	--		_ITEM_SET1      = null,          --這樣AVID才能重新CheckIn(而不是被Duplicate)
	--		_sDESCRIPTION   = '',			 --這樣TMS才會重新處理 2014/04/21
	--		fsUPDATED_BY	= @fsUPDATED_BY ,
	--		fdUPDATED_DATE	= GETDATE()     
	--	WHERE
	--		(fnWORK_ID	= @fnWORK_ID)

	--	UPDATE
	--		tbmBOOKING 
	--	SET
	--	    fsSTATUS        = '00',
	--		fsUPDATED_BY	= @fsUPDATED_BY ,
	--		fdUPDATED_DATE	= GETDATE()     
	--	WHERE
	--		(fnBOOKING_ID	= @_nBOOKING_ID)
			
	--	SELECT RESULT = @@ROWCOUNT
	--END TRY
	--BEGIN CATCH
	--	SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	--END CATCH
END



