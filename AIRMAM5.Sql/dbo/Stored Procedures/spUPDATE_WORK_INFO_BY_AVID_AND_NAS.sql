

-- =============================================
-- 描述:	更新Work主檔資料
-- 記錄:	<2013/01/22><Albert.Chen><新增本預存><複製[spUPDATE_WORK_INFO>
--          <2013/01/23><Albert.Chen><修改本預存><不把詳細訊息更新到fsNOTE>
-- ※本預存僅供轉檔AP使用
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_WORK_INFO_BY_AVID_AND_NAS]
	@fnWORK_ID		bigint,	
	@fsSTATUS		varchar(2),		/*傳入空白則不更新*/
	@fsPROGRESS		nvarchar(50),	/*傳入空白則不更新*/
	@fdSTIME		datetime,		/*傳入1900/01/01則不更新*/
	@fdETIME		datetime,		/*傳入1900/01/01則不更新*/
	@fsRESULT		nvarchar(100),	/*傳入例外訊息*/
	@fsUPDATED_BY	nvarchar(50),
	@fsMOB_ID       nvarchar(255)   /*傳入空白則不更新*/
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SET NOCOUNT ON;
	
		DECLARE
			@STATUS		varchar(2),		
			@PROGRESS	nvarchar(50),
			@STIME		datetime,		
			@ETIME		datetime,		
			@RESULT		nvarchar(100),
			@NOTE		nvarchar(500),
			@MOB_ID     nvarchar(500),
			@CODE_NAME  NVARCHAR(200)
		
		SET @RESULT=''   --防止傳入NULL
		
		SELECT
			@STATUS		= CASE WHEN (@fsSTATUS = '')			THEN fsSTATUS ELSE @fsSTATUS END,
			@PROGRESS	= CASE WHEN (@fsPROGRESS = '')			THEN fsPROGRESS ELSE @fsPROGRESS END,	
			@STIME		= CASE WHEN (@fdSTIME	 = '1900/01/01') THEN fdSTIME ELSE @fdSTIME END,
			@ETIME		= CASE WHEN (@fdETIME	 = '1900/01/01') THEN fdETIME ELSE @fdETIME END,
			@RESULT		= @fsRESULT ,
			@MOB_ID     = CASE WHEN (@fsMOB_ID   ='')             THEN NULL ELSE @fsMOB_ID END
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
			fsUPDATED_BY	= @fsUPDATED_BY,
			fdUPDATED_DATE	= GETDATE(),
			_ITEM_SET1      = @MOB_ID
		WHERE
			(fnWORK_ID	= @fnWORK_ID)
			
		SELECT RESULT = CAST(@@IDENTITY AS VARCHAR(30))
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



