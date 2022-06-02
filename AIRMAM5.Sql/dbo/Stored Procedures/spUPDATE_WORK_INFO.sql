

-- =============================================
-- 描述:	修改NEWS主檔資料
-- 記錄:	<2011/11/24><Dennis.Wen><新增本預存>
-- ※本預存僅供轉檔AP使用
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_WORK_INFO]
	@fnWORK_ID		bigint,	
	@fsSTATUS		varchar(2),		/*傳入空白則不更新*/
	@fsPROGRESS		nvarchar(50),	/*傳入空白則不更新*/
	@fdSTIME		datetime,		/*傳入1900/01/01則不更新*/
	@fdETIME		datetime,		/*傳入1900/01/01則不更新*/
	@fsRESULT		nvarchar(100),	/*傳入空白則不更新*/
	@fsNOTE			nvarchar(500),	/*傳入空白則不更新*/	
	@_SM_VOLUME_NAME	nvarchar(50),	/*傳入空白則不更新*/	
	@fsUPDATED_BY	nvarchar(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		--DECLARE
		--	@fnWORK_ID		bigint,	
		--	@fsSTATUS		varchar(2),		/*傳入空白則不更新*/
		--	@fsPROGRESS		nvarchar(50),	/*傳入空白則不更新*/
		--	@fdSTIME		datetime,		/*傳入1900/01/01則不更新*/
		--	@fdETIME		datetime,		/*傳入1900/01/01則不更新*/
		--	@fsRESULT		nvarchar(100),	/*傳入空白則不更新*/
		--	@fsNOTE			nvarchar(500),	/*傳入空白則不更新*/	
		--	@fsUPDATED_BY	nvarchar(50)
			
		-------

		--SELECT
		--	@fnWORK_ID		= 35,	
		--	@fsSTATUS		= '05',
		--	@fsPROGRESS		= '進度進度',
		--	@fdSTIME		= '2011/11/24 01:02:03',
		--	@fdETIME		= '2011/11/24 05:06:07',
		--	@fsRESULT		= '阿捏',
		--	@fsNOTE			= '甘後',	
		--	@fsUPDATED_BY	= 'APAPAP'

		-------
		
		SET NOCOUNT ON;
	
		DECLARE
			@STATUS		varchar(2),		
			@PROGRESS	nvarchar(50),
			@STIME		datetime,		
			@ETIME		datetime,		
			@RESULT		nvarchar(100),
			@NOTE		nvarchar(500),
			@VOLUME_NAME nvarchar(50) 
			
		SELECT
			@STATUS		= CASE WHEN (@fsSTATUS = '')			THEN fsSTATUS ELSE @fsSTATUS END,
			@PROGRESS	= CASE WHEN (@fsPROGRESS = '')			THEN fsPROGRESS ELSE @fsPROGRESS END,	
			@STIME		= CASE WHEN (@fdSTIME	 = '1900/01/01') THEN fdSTIME ELSE @fdSTIME END,
			@ETIME		= CASE WHEN (@fdETIME	 = '1900/01/01') THEN fdETIME ELSE @fdETIME END,
			@RESULT		= CASE WHEN (@fsRESULT	 = '')			THEN fsRESULT ELSE @fsRESULT END,
			--@NOTE		= CASE WHEN (@fsNOTE	 = '')			THEN fsNOTE ELSE @fsNOTE END
			@VOLUME_NAME= CASE WHEN (@_SM_VOLUME_NAME	 = '')			THEN _SM_VOLUME_NAME ELSE @_SM_VOLUME_NAME END
		FROM
			tblWORK
		WHERE
			(fnWORK_ID	= @fnWORK_ID)
			
			
		UPDATE
			tblWORK
		SET
			fsSTATUS		= @STATUS	,
			fsPROGRESS		= @PROGRESS	,
			fdSTIME			= @STIME	,	
			fdETIME			= @ETIME	,	
			fsRESULT		= @RESULT	,	
			--fsNOTE			= @NOTE		,
			_SM_VOLUME_NAME	= @VOLUME_NAME,
			
			fsUPDATED_BY	= @fsUPDATED_BY,
			fdUPDATED_DATE	= GETDATE()
		WHERE
			(fnWORK_ID	= @fnWORK_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



