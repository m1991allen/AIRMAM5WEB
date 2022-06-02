

CREATE VIEW [dbo].[xvwSEARCH_SUBJECT]
AS
SELECT     
ISNULL(dbo.tbmSUBJECT.fsSUBJ_ID, '') AS fsSYS_ID, 
ISNULL(dbo.tbmSUBJECT.fsSUBJ_ID, '') AS fsSUBJ_ID, 
ISNULL(dbo.tbmSUBJECT.fsTITLE, '') AS fsTITLE, 
                      ISNULL(dbo.tbmSUBJECT.fsDESCRIPTION, '') AS fsDESCRIPTION, 
					  ISNULL(dbo.tbmSUBJECT.fsTYPE1, '') AS fsTYPE1, 
					  ISNULL(dbo.tbmSUBJECT.fsTYPE2, '') AS fsTYPE2, 
					  ISNULL(dbo.tbmSUBJECT.fsTYPE3, '') AS fsTYPE3, 
					  CONVERT(VARCHAR(10), ISNULL(dbo.tbmSUBJECT.fdCREATED_DATE, ''), 111) AS fdCREATED_DATE, 
					  ISNULL(dbo.tbmSUBJECT.fsCREATED_BY, '') AS fsCREATED_BY, 
                      CONVERT(VARCHAR(10), ISNULL(dbo.tbmSUBJECT.fdUPDATED_DATE, ''), 111) AS fdUPDATED_DATE, 
					  ISNULL(dbo.tbmSUBJECT.fsUPDATED_BY, '') 
                      AS fsUPDATED_BY, ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE1, '') AS fsATTRIBUTE1, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE2, '') AS fsATTRIBUTE2, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE3, '') AS fsATTRIBUTE3, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE4, '') AS fsATTRIBUTE4, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE5, '') AS fsATTRIBUTE5, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE6, '') AS fsATTRIBUTE6, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE7, '') AS fsATTRIBUTE7, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE8, '') AS fsATTRIBUTE8, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE9, '') AS fsATTRIBUTE9, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE10, '') AS fsATTRIBUTE10, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE11, '') AS fsATTRIBUTE11, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE12, '') AS fsATTRIBUTE12, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE13, '') AS fsATTRIBUTE13, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE14, '') AS fsATTRIBUTE14, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE15, '') AS fsATTRIBUTE15, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE16, '') AS fsATTRIBUTE16, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE17, '') AS fsATTRIBUTE17, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE18, '') AS fsATTRIBUTE18, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE19, '') AS fsATTRIBUTE19, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE20, '') AS fsATTRIBUTE20, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE21, '') AS fsATTRIBUTE21, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE22, '') AS fsATTRIBUTE22, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE23, '') AS fsATTRIBUTE23, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE24, '') AS fsATTRIBUTE24, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE25, '') AS fsATTRIBUTE25, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE26, '') AS fsATTRIBUTE26, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE27, '') AS fsATTRIBUTE27, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE28, '') AS fsATTRIBUTE28, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE29, '') AS fsATTRIBUTE29, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE30, '') AS fsATTRIBUTE30, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE31, '') AS fsATTRIBUTE31, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE32, '') AS fsATTRIBUTE32, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE33, '') AS fsATTRIBUTE33, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE34, '') AS fsATTRIBUTE34, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE35, '') AS fsATTRIBUTE35, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE36, '') AS fsATTRIBUTE36, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE37, '') AS fsATTRIBUTE37, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE38, '') AS fsATTRIBUTE38, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE39, '') AS fsATTRIBUTE39, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE40, '') AS fsATTRIBUTE40, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE41, '') AS fsATTRIBUTE41, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE42, '') AS fsATTRIBUTE42, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE43, '') AS fsATTRIBUTE43, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE44, '') AS fsATTRIBUTE44, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE45, '') AS fsATTRIBUTE45, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE46, '') AS fsATTRIBUTE46, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE47, '') AS fsATTRIBUTE47, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE48, '') AS fsATTRIBUTE48, 
                      ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE49, '') AS fsATTRIBUTE49, 
					  ISNULL(dbo.tbmSUBJECT.fsATTRIBUTE50, '') AS fsATTRIBUTE50, 
                      ISNULL(dbo.tbmDIRECTORIES.fsNAME, '') AS fsDIRECTORIES_NAME, 
					  ISNULL(dbo.tbmDIRECTORIES.fsDESCRIPTION, '') AS fsDIRECTORIES_DESCRIPTION, 
                      ISNULL(dbo.tbmDIRECTORIES.fsADMIN_GROUP, '') AS fsDIRECTORIES_ADMIN_GROUP, 
					  ISNULL(dbo.tbmDIRECTORIES.fsADMIN_USER, '') 
                      AS fsDIRECTORIES_ADMIN_USER, 'S' AS fsFILE_CATEGORY, 
                      dbo.fnGET_ARC_USED_NAME_LIST_BY_CODE_LIST(dbo.fnGET_SUBJ_USED_CODE_LIST_BY_SUBJ_ID(dbo.tbmSUBJECT.fsSUBJ_ID)) 
                      AS fsTEMPLATE_FIELD_VALUE, dbo.tbmSUBJECT.fnDIR_ID AS fsAUTHORUTY_DIR_ID,tbmDIRECTORIES.fnTEMP_ID_SUBJECT AS fnTEMP_ID
FROM         dbo.tbmSUBJECT INNER JOIN
                      dbo.tbmDIRECTORIES ON dbo.tbmSUBJECT.fnDIR_ID = dbo.tbmDIRECTORIES.fnDIR_ID


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
         Configuration = "(H (2[66] 3) )"
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
         Begin Table = "tbmSUBJECT"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 215
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbmDIRECTORIES"
            Begin Extent = 
               Top = 6
               Left = 253
               Bottom = 125
               Right = 444
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
         Width = 1500
         Width = 1500
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'xvwSEARCH_SUBJECT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'xvwSEARCH_SUBJECT';

