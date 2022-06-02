









CREATE VIEW [dbo].[vwSEARCH_DOC]
AS
SELECT     
	dbo.tbmARC_DOC.fsFILE_NO AS fsSYS_ID, 
	dbo.tbmARC_DOC.fsFILE_NO, 
	ISNULL(dbo.tbmARC_DOC.fsTITLE, '') AS fsTITLE, 
	ISNULL(dbo.tbmARC_DOC.fsDESCRIPTION, '') AS fsDESCRIPTION, 
	dbo.tbmARC_DOC.fnFILE_SECRET AS fnFILE_SECRET,
	CONVERT(VARCHAR(10), ISNULL(dbo.tbmARC_DOC.fdCREATED_DATE, ''), 111) AS fdCREATED_DATE, 
	ISNULL(dbo.tbmARC_DOC.fsCREATED_BY, '') AS fsCREATED_BY, 
    CONVERT(VARCHAR(10), ISNULL(dbo.tbmARC_DOC.fdUPDATED_DATE, ''), 111) AS fdUPDATED_DATE, 
	ISNULL(dbo.tbmARC_DOC.fsUPDATED_BY, '') AS fsUPDATED_BY, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE1, '') AS fsATTRIBUTE1, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE2, '') AS fsATTRIBUTE2, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE3, '') AS fsATTRIBUTE3, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE4, '') AS fsATTRIBUTE4, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE5, '') AS fsATTRIBUTE5, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE6, '') AS fsATTRIBUTE6, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE7, '') AS fsATTRIBUTE7, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE8, '') AS fsATTRIBUTE8, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE9, '') AS fsATTRIBUTE9, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE10, '') AS fsATTRIBUTE10, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE11, '') AS fsATTRIBUTE11, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE12, '') AS fsATTRIBUTE12, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE13, '') AS fsATTRIBUTE13, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE14, '') AS fsATTRIBUTE14, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE15, '') AS fsATTRIBUTE15, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE16, '') AS fsATTRIBUTE16, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE17, '') AS fsATTRIBUTE17, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE18, '') AS fsATTRIBUTE18, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE19, '') AS fsATTRIBUTE19, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE20, '') AS fsATTRIBUTE20, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE21, '') AS fsATTRIBUTE21, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE22, '') AS fsATTRIBUTE22, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE23, '') AS fsATTRIBUTE23, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE24, '') AS fsATTRIBUTE24, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE25, '') AS fsATTRIBUTE25, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE26, '') AS fsATTRIBUTE26, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE27, '') AS fsATTRIBUTE27, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE28, '') AS fsATTRIBUTE28, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE29, '') AS fsATTRIBUTE29, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE30, '') AS fsATTRIBUTE30, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE31, '') AS fsATTRIBUTE31, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE32, '') AS fsATTRIBUTE32, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE33, '') AS fsATTRIBUTE33, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE34, '') AS fsATTRIBUTE34, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE35, '') AS fsATTRIBUTE35, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE36, '') AS fsATTRIBUTE36, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE37, '') AS fsATTRIBUTE37, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE38, '') AS fsATTRIBUTE38, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE39, '') AS fsATTRIBUTE39, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE40, '') AS fsATTRIBUTE40, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE41, '') AS fsATTRIBUTE41, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE42, '') AS fsATTRIBUTE42, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE43, '') AS fsATTRIBUTE43, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE44, '') AS fsATTRIBUTE44, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE45, '') AS fsATTRIBUTE45, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE46, '') AS fsATTRIBUTE46, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE47, '') AS fsATTRIBUTE47, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE48, '') AS fsATTRIBUTE48, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE49, '') AS fsATTRIBUTE49, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE50, '') AS fsATTRIBUTE50, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE51, '') AS fsATTRIBUTE51, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE52, '') AS fsATTRIBUTE52, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE53, '') AS fsATTRIBUTE53, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE54, '') AS fsATTRIBUTE54, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE55, '') AS fsATTRIBUTE55, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE56, '') AS fsATTRIBUTE56, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE57, '') AS fsATTRIBUTE57, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE58, '') AS fsATTRIBUTE58, 
    ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE59, '') AS fsATTRIBUTE59, 
	ISNULL(dbo.tbmARC_DOC.fsATTRIBUTE60, '') AS fsATTRIBUTE60, 
    ISNULL(dbo.tbmARC_DOC.fsCONTENT, '') AS fsCONTENT, 
	ISNULL(dbo.tbmDIRECTORIES.fsNAME, '') AS fsDIRECTORIES_NAME, 
    ISNULL(dbo.tbmDIRECTORIES.fsDESCRIPTION, '') AS fsDIRECTORIES_DESCRIPTION, 
	ISNULL(dbo.tbmDIRECTORIES.fsADMIN_GROUP, '') AS fsDIRECTORIES_ADMIN_GROUP, 
	ISNULL(dbo.tbmDIRECTORIES.fsADMIN_USER, '') AS fsDIRECTORIES_ADMIN_USER, 
	ISNULL(dbo.tbmSUBJECT.fsTYPE1, '') AS fsSUBJECT_TYPE1, 
	ISNULL(dbo.tbmSUBJECT.fsTYPE2, '') AS fsSUBJECT_TYPE2, 
	ISNULL(dbo.tbmSUBJECT.fsTYPE3, '') AS fsSUBJECT_TYPE3, 
    'D' AS fsFILE_CATEGORY, 
	 [tbdARC_DOC_ATTR].fsCODE_LIST AS fsTEMPLATE_FIELD_VALUE,
	 dbo.fnGET_DIR_ID_BY_FILE_NO('D', dbo.tbmARC_DOC.fsFILE_NO) AS fsAUTHORUTY_DIR_ID,
	 tbmDIRECTORIES.fnTEMP_ID_DOC AS fnTEMP_ID
FROM         dbo.tbmARC_DOC INNER JOIN
                      dbo.tbmSUBJECT ON dbo.tbmARC_DOC.fsSUBJECT_ID = dbo.tbmSUBJECT.fsSUBJ_ID INNER JOIN
                      dbo.tbmDIRECTORIES ON dbo.tbmSUBJECT.fnDIR_ID = dbo.tbmDIRECTORIES.fnDIR_ID INNER JOIN
					  [dbo].[tbdARC_DOC_ATTR] ON tbmARC_DOC.fsFILE_NO = [tbdARC_DOC_ATTR].fsFILE_NO

WHERE
		fsFILE_STATUS = 'Y'







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
         Begin Table = "tbmARC_DOC"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 242
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "tbmSUBJECT"
            Begin Extent = 
               Top = 6
               Left = 280
               Bottom = 125
               Right = 457
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vwSEARCH_DOC';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vwSEARCH_DOC';

