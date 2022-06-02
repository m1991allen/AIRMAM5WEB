

-- =============================================
-- 描述:	重設調用使用
-- 記錄:	<2013/05/01><Albert.Chen><新增本預存>
-- 記錄:	<2014/04/17><Eric.Huang><修改本預存 清除 _sDESCRIPTION>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_RE_TRANS_CODE]
	@fnWORK_ID		BIGINT,	
	@fsUPDATED_BY	NVARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;
	DECLARE @_nBOOKING_ID BIGINT,
            @_sTYPE       VARCHAR(50),
			@_ITEM_TYPE   VARCHAR(20),
			@_sSTATUS     VARCHAR(2)
	BEGIN TRY
		SET NOCOUNT ON;

	    SELECT @_nBOOKING_ID=fnGROUP_ID,@_sTYPE=fsTYPE,@_ITEM_TYPE=_ITEM_TYPE FROM tblWORK WHERE (fnWORK_ID	= @fnWORK_ID)

		IF (@_sTYPE='AVID')
		    IF (@_ITEM_TYPE ='V')
			  SET  @_sSTATUS='_@'
			ELSE
			  SET  @_sSTATUS='00'
		ELSE IF (@_sTYPE ='NAS')
				IF (@_ITEM_TYPE ='V')
				  SET  @_sSTATUS='_C'
				ELSE
				  SET  @_sSTATUS='00'
		ELSE  --MTS(BOOKING OR COPYFILE)
		    IF (@_ITEM_TYPE ='V')
			  SET  @_sSTATUS='_C'
			ELSE
			  SET  @_sSTATUS='00'


		UPDATE
			tblWORK
		SET
		    fsSTATUS        = @_sSTATUS,
			fsPROGRESS      = '0%',
			_ITEM_SET1      = null,          --這樣AVID才能重新CheckIn(而不是被Duplicate)
			_sDESCRIPTION   = '',			 --這樣TMS才會重新處理 2014/04/21
			fsUPDATED_BY	= @fsUPDATED_BY ,
			fdUPDATED_DATE	= GETDATE()     
		WHERE
			(fnWORK_ID	= @fnWORK_ID)

		UPDATE
			tbmBOOKING 
		SET
		    fsSTATUS        = '00',
			fsUPDATED_BY	= @fsUPDATED_BY ,
			fdUPDATED_DATE	= GETDATE()     
		WHERE
			(fnBOOKING_ID	= @_nBOOKING_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



