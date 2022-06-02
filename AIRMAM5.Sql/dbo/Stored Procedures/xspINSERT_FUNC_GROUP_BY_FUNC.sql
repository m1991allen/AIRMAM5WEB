

-- =============================================
-- 描述:	新增FUNC_GROUP主檔資料
-- 記錄:	<2011/09/01><Mihsiu.Chiu><新增預存>
--			<2012/01/05><Mihsiu.Chiu><新增fsSET>
--			<2012/06/29><Mihsiu.Chiu><修改同時增修上層作業權限>
-- =============================================
CREATE PROCEDURE [dbo].[xspINSERT_FUNC_GROUP_BY_FUNC]
	@fsFUNC_ID		VARCHAR(50),
	@fsGROUP_ID		VARCHAR(50),
	@fsCREATED_BY	NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT
			tbmFUNC_GROUP
			(fsFUNC_ID, fsGROUP_ID,fdCREATED_DATE, fsCREATED_BY)
		VALUES
			(@fsFUNC_ID, @fsGROUP_ID,GETDATE(), @fsCREATED_BY)
			 
	--20120628	**add begin
	declare @rc int;
	select @rc = COUNT(*) from tbmFUNC_GROUP WHERE (fsFUNC_ID = @fsFUNC_ID) AND (fsGROUP_ID = @fsGROUP_ID)
	if (@rc > 0 ) begin
		
		--取得其上層ID
		declare @sParent varchar(50);
		select @sParent = fsPARENT_ID from tbmFUNCTIONS where fsFUNC_ID =@fsFUNC_ID;
		if (@sParent <> 'frm00')
		begin
			select [fsFUNC_ID],	[fsGROUP_ID] into #temp from dbo.tbmFUNC_GROUP 
				where fsGROUP_ID = @fsGROUP_ID and fsFUNC_ID in (select fsFUNC_ID from tbmFUNCTIONS where fsPARENT_ID = @sParent)		
				
				
			if (EXISTS(select * from tbmFUNC_GROUP where fsFUNC_ID = @sParent and fsGROUP_ID = @fsGROUP_ID)) begin
				--上層資料存在，就進行修改
				update tbmFUNC_GROUP
					set 
						fdUPDATED_DATE = GETDATE(), 
						fsUPDATED_BY = @fsCREATED_BY
					where fsFUNC_ID = @sParent and fsGROUP_ID = @fsGROUP_ID;
			end
			else begin
				--insert上層
				
				INSERT
					tbmFUNC_GROUP
					(fsFUNC_ID, fsGROUP_ID, fdCREATED_DATE, fsCREATED_BY)
				VALUES
					(@sParent, @fsGROUP_ID, GETDATE(), @fsCREATED_BY)
			end
			
			drop table #temp;
		end;
		--20120628	**add end
	end		
	-- 新增完畢時, 取回新增資料的識別編號, 或OK字樣
	SELECT RESULT = @@IDENTITY
	END TRY
	BEGIN CATCH
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

