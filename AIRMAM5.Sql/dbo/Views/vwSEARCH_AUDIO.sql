





CREATE VIEW [dbo].[vwSEARCH_AUDIO]
AS
SELECT     
	dbo.tbmARC_AUDIO.fsFILE_NO AS fsSYS_ID, 
	dbo.tbmARC_AUDIO.fsFILE_NO,
	ISNULL(dbo.tbmARC_AUDIO.fsTITLE, '') AS fsTITLE, 
	ISNULL(dbo.tbmARC_AUDIO.fsDESCRIPTION, '') AS fsDESCRIPTION,
	dbo.tbmARC_AUDIO.fnFILE_SECRET AS fnFILE_SECRET,
	ISNULL(dbo.tbmARC_AUDIO.fsALBUM, '') AS fsALBUM,			
	ISNULL(dbo.tbmARC_AUDIO.fsALBUM_TITLE, '') AS fsALBUM_TITLE,  
	ISNULL(dbo.tbmARC_AUDIO.fsALBUM_ARTISTS, '') AS fsALBUM_ARTISTS, 
	ISNULL(dbo.tbmARC_AUDIO.fsALBUM_PERFORMERS, '') AS fsALBUM_PERFORMERS,
	ISNULL(dbo.tbmARC_AUDIO.fsALBUM_COMPOSERS, '') AS fsALBUM_COMPOSERS,
	ISNULL(dbo.tbmARC_AUDIO.fnALBUM_YEAR, 0) AS fnALBUM_YEAR,
	ISNULL(dbo.tbmARC_AUDIO.fsALBUM_COPYRIGHT, '') AS fsALBUM_COPYRIGHT,
	ISNULL(dbo.tbmARC_AUDIO.fsALBUM_GENRES, '') AS fsALBUM_GENRES, 
	ISNULL(dbo.tbmARC_AUDIO.fsALBUM_COMMENT, '') AS fsALBUM_COMMENT,
    CONVERT(VARCHAR(10), ISNULL(dbo.tbmARC_AUDIO.fdCREATED_DATE, ''), 111) AS fdCREATED_DATE, 
	ISNULL(dbo.tbmARC_AUDIO.fsCREATED_BY, '') AS fsCREATED_BY, 
	CONVERT(VARCHAR(10), ISNULL(dbo.tbmARC_AUDIO.fdUPDATED_DATE, ''), 111) AS fdUPDATED_DATE, 
    ISNULL(dbo.tbmARC_AUDIO.fsUPDATED_BY, '') AS fsUPDATED_BY, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE1, '') AS fsATTRIBUTE1, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE2, '') AS fsATTRIBUTE2, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE3, '') AS fsATTRIBUTE3, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE4, '') AS fsATTRIBUTE4, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE5, '') AS fsATTRIBUTE5, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE6, '') AS fsATTRIBUTE6, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE7, '') AS fsATTRIBUTE7, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE8, '') AS fsATTRIBUTE8, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE9, '') AS fsATTRIBUTE9, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE10, '') AS fsATTRIBUTE10, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE11, '') AS fsATTRIBUTE11, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE12, '') AS fsATTRIBUTE12, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE13, '') AS fsATTRIBUTE13, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE14, '') AS fsATTRIBUTE14, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE15, '') AS fsATTRIBUTE15, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE16, '') AS fsATTRIBUTE16, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE17, '') AS fsATTRIBUTE17, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE18, '') AS fsATTRIBUTE18, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE19, '') AS fsATTRIBUTE19, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE20, '') AS fsATTRIBUTE20, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE21, '') AS fsATTRIBUTE21, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE22, '') AS fsATTRIBUTE22, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE23, '') AS fsATTRIBUTE23, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE24, '') AS fsATTRIBUTE24, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE25, '') AS fsATTRIBUTE25, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE26, '') AS fsATTRIBUTE26, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE27, '') AS fsATTRIBUTE27, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE28, '') AS fsATTRIBUTE28, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE29, '') AS fsATTRIBUTE29, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE30, '') AS fsATTRIBUTE30, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE31, '') AS fsATTRIBUTE31, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE32, '') AS fsATTRIBUTE32, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE33, '') AS fsATTRIBUTE33, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE34, '') AS fsATTRIBUTE34, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE35, '') AS fsATTRIBUTE35, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE36, '') AS fsATTRIBUTE36, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE37, '') AS fsATTRIBUTE37, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE38, '') AS fsATTRIBUTE38, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE39, '') AS fsATTRIBUTE39, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE40, '') AS fsATTRIBUTE40, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE41, '') AS fsATTRIBUTE41, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE42, '') AS fsATTRIBUTE42, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE43, '') AS fsATTRIBUTE43, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE44, '') AS fsATTRIBUTE44, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE45, '') AS fsATTRIBUTE45, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE46, '') AS fsATTRIBUTE46, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE47, '') AS fsATTRIBUTE47, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE48, '') AS fsATTRIBUTE48, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE49, '') AS fsATTRIBUTE49, 
    ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE50, '') AS fsATTRIBUTE50, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE51, '') AS fsATTRIBUTE51, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE52, '') AS fsATTRIBUTE52, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE53, '') AS fsATTRIBUTE53, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE54, '') AS fsATTRIBUTE54, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE55, '') AS fsATTRIBUTE55, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE56, '') AS fsATTRIBUTE56, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE57, '') AS fsATTRIBUTE57, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE58, '') AS fsATTRIBUTE58, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE59, '') AS fsATTRIBUTE59, 
	ISNULL(dbo.tbmARC_AUDIO.fsATTRIBUTE60, '') AS fsATTRIBUTE60, 
	ISNULL(dbo.tbmDIRECTORIES.fsNAME, '') AS fsDIRECTORIES_NAME, 
    ISNULL(dbo.tbmDIRECTORIES.fsDESCRIPTION, '') AS fsDIRECTORIES_DESCRIPTION, 
	ISNULL(dbo.tbmDIRECTORIES.fsADMIN_GROUP, '') AS fsDIRECTORIES_ADMIN_GROUP, 
	ISNULL(dbo.tbmDIRECTORIES.fsADMIN_USER, '') AS fsDIRECTORIES_ADMIN_USER, 
	ISNULL(dbo.tbmSUBJECT.fsTYPE1, '') AS fsSUBJECT_TYPE1, 
	ISNULL(dbo.tbmSUBJECT.fsTYPE2, '') AS fsSUBJECT_TYPE2, 
	ISNULL(dbo.tbmSUBJECT.fsTYPE3, '') AS fsSUBJECT_TYPE3, 
	ISNULL((SELECT [dbo].[tbmARC_AUDIO_D].[fsDESCRIPTION] + ' ' FROM [dbo].[tbmARC_AUDIO_D] WHERE [dbo].[tbmARC_AUDIO_D].[fsFILE_NO] = dbo.tbmARC_AUDIO.fsFILE_NO AND [dbo].[tbmARC_AUDIO_D].[fnSEQ_NO] > 0 FOR XML PATH('')),'') AS fsARC_AUDIO_D_DESCRIPTION,
    'A' AS fsFILE_CATEGORY,
    [dbo].[tbdARC_AUDIO_ATTR].[fsCODE_LIST] AS fsTEMPLATE_FIELD_VALUE,
	dbo.fnGET_DIR_ID_BY_FILE_NO('A', dbo.tbmARC_AUDIO.fsFILE_NO) AS fsAUTHORUTY_DIR_ID,
	[dbo].[tbmDIRECTORIES].[fnTEMP_ID_AUDIO] AS fnTEMP_ID
FROM         dbo.tbmARC_AUDIO INNER JOIN
                      dbo.tbmSUBJECT ON dbo.tbmARC_AUDIO.fsSUBJECT_ID = dbo.tbmSUBJECT.fsSUBJ_ID INNER JOIN
                      dbo.tbmDIRECTORIES ON dbo.tbmSUBJECT.fnDIR_ID = dbo.tbmDIRECTORIES.fnDIR_ID INNER JOIN
					  [dbo].[tbdARC_AUDIO_ATTR] ON [dbo].[tbmARC_AUDIO].[fsFILE_NO] = [dbo].[tbdARC_AUDIO_ATTR].[fsFILE_NO]
WHERE
		[dbo].[tbmARC_AUDIO].[fsFILE_STATUS] = 'Y'

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4[30] 2[40] 3) )"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2[67] 3) )"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 5
   End
   Begin DiagramPane = 
      PaneHidden = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "tbmARC_AUDIO"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 225
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbmSUBJECT"
            Begin Extent = 
               Top = 6
               Left = 263
               Bottom = 125
               Right = 440
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbmDIRECTORIES"
            Begin Extent = 
               Top = 126
               Left = 38
               Bottom = 245
               Right = 229
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 2685
         Width = 1890
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      PaneHidden = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vwSEARCH_AUDIO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vwSEARCH_AUDIO';

