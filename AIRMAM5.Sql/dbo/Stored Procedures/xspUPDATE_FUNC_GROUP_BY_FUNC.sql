

-- =============================================
-- 描述:	修改FUNC_GROUP主檔資料
-- 記錄:	<2011/09/01><Mihsiu.Chiu><修改預存>
--			<2012/01/05><Mihsiu.Chiu><新增fsSET>
--			<2012/06/29><Mihsiu.Chiu><修改同時增修上層作業權限>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_FUNC_GROUP_BY_FUNC]
	@fsFUNC_ID		VARCHAR(50),
	@fsGROUP_ID		VARCHAR(50),
	@fsUPDATED_BY	NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		INSERT INTO [log].[tbmFUNC_GROUP]
		SELECT *,'U',@fsUPDATED_BY FROM tbmFUNC_GROUP WHERE (fsFUNC_ID = @fsFUNC_ID) AND (fsGROUP_ID = @fsGROUP_ID)

		UPDATE
			tbmFUNC_GROUP
		SET			
			fdUPDATED_DATE = GETDATE(), 
			fsUPDATED_BY = @fsUPDATED_BY
		WHERE 
			(fsFUNC_ID = @fsFUNC_ID) AND (fsGROUP_ID = @fsGROUP_ID)
		
		COMMIT

		--20120628	**add begin
		declare @rc int;
		select @rc = COUNT(*) from tbmFUNC_GROUP WHERE (fsFUNC_ID = @fsFUNC_ID) AND (fsGROUP_ID = @fsGROUP_ID)
		if (@rc > 0 ) begin			
			--取得其上層ID
			declare @sParent varchar(50);
			
			select @sParent = fsPARENT_ID from tbmFUNCTIONS where fsFUNC_ID =@fsFUNC_ID;
				
			if (@sParent <> 'frm00') begin
				select [fsFUNC_ID],	[fsGROUP_ID] into #temp from dbo.tbmFUNC_GROUP 
				where fsGROUP_ID = @fsGROUP_ID and fsFUNC_ID in (select fsFUNC_ID from tbmFUNCTIONS where fsPARENT_ID = @sParent)		
				
				
				if (EXISTS(select * from tbmFUNC_GROUP where fsFUNC_ID = @sParent and fsGROUP_ID = @fsGROUP_ID)) begin
					
					--上層資料存在，就進行修改			
						   
					update tbmFUNC_GROUP
					set fdUPDATED_DATE = GETDATE(), 
						fsUPDATED_BY = @fsUPDATED_BY
					where fsFUNC_ID = @sParent and fsGROUP_ID = @fsGROUP_ID;
				end
				else begin
					--insert上層
					
					INSERT
						tbmFUNC_GROUP
						(fsFUNC_ID, fsGROUP_ID,fdCREATED_DATE, fsCREATED_BY)
					VALUES
						(@sParent, @fsGROUP_ID, GETDATE(), @fsUPDATED_BY)
				end
				drop table #temp;
			end;				
			--20120628	**add end	
		
		end
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END

