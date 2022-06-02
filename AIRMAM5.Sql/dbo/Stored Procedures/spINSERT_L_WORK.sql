

-- =============================================
-- 描述:	新增L_WORK主檔資料
-- 記錄:	<2011/11/22><Mihsiu.Chiu><新增預存>
--      <2013/09/03><Albert.Chen><修改本預存><@fsPARAMETERS放寬到300>
--      <2016/11/18><David.Sin><修改本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_L_WORK]
	@fsTYPE        VARCHAR(50),
	@fsPARAMETERS		NVARCHAR(300),
	@fsSTATUS		varchar(2),
	@fsPROGRESS		nvarchar(50),
	@fsPRIORITY		varchar(1),
	@fsRESULT		nvarchar(100),
	@fsNOTE		    NVARCHAR(200),
	@fsCREATED_BY	VARCHAR(50),

	@_ITEM_ID		VARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tblWORK
			(fsTYPE, fsPARAMETERS, fsSTATUS, fsPROGRESS, fsPRIORITY,fdSTART_WORK_TIME, fsRESULT, fsNOTE, 
			 fdCREATED_DATE, fsCREATED_BY,_ITEM_ID)
		VALUES
			(@fsTYPE, @fsPARAMETERS, @fsSTATUS, @fsPROGRESS,@fsPRIORITY, GETDATE(),@fsRESULT, @fsNOTE,
			GETDATE(), @fsCREATED_BY,@_ITEM_ID)
			
		-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
		SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


