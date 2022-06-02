



-- =============================================
-- 描述:	修改BOOK_T主檔資料
-- 記錄:	<2012/04/26><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_BOOK_T]

	@fnBOOK_T_ID	BIGINT,
	@fsTITLE		nvarchar(20),
	@fsGROUP		nvarchar(50),
	@fsTYPE			varchar(1),
	@fsPATH_TYPE	varchar(1),
	@fsFOLDER		varchar(50),
	@fsPROFILE_NAME	nvarchar(128),
	@fsBEG_TIME		varchar(10),
	@fsEND_TIME		varchar(10),
	@fsWIDTH		varchar(10),
	@fsHEIGHT		varchar(10),
	@fsWATERMARK	varchar(500),
	@fsUPDATED_BY	varchar(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		UPDATE
			tbmBOOKING_T	
		SET
			fsTITLE          = @fsTITLE,       
			fsGROUP          = @fsGROUP ,      
			fsTYPE           = @fsTYPE  ,      
			fsPATH_TYPE      = @fsPATH_TYPE,   
			fsFOLDER         = @fsFOLDER    ,  
			fsPROFILE_NAME   = @fsPROFILE_NAME,
			fsBEG_TIME       = @fsBEG_TIME    ,
			fsEND_TIME       = @fsEND_TIME    ,
			fsWIDTH          = @fsWIDTH       ,
			fsHEIGHT         = @fsHEIGHT      ,
			fsWATERMARK      = @fsWATERMARK   ,
			fdUPDATED_DATE	 = GETDATE(),
			fsUPDATED_BY	 = @fsUPDATED_BY
		WHERE
			(fnBOOK_T_ID = @fnBOOK_T_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





