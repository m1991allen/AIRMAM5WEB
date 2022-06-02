
using AIRMAM5.DBEntity.Models.ArcPre;
using AIRMAM5.DBEntity.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 預編詮釋資料表 tbmARC_PRE
    /// </summary>
    [MetadataType(typeof(tbmARC_PREMetadata))]
    public partial class tbmARC_PRE
    {
        public tbmARC_PRE()
        {
            fnPRE_ID = 0;
            fsNAME = string.Empty;
            fsTYPE = string.Empty;
            fnTEMP_ID = 0;
            fsTITLE = string.Empty;
            fsDESCRIPTION = string.Empty;
            fsHASH_TAG = string.Empty;
            fdCREATED_DATE = DateTime.Now;
            fsCREATED_BY = string.Empty;
            fdUPDATED_DATE = null;
            fsUPDATED_BY = string.Empty;
            fsATTRIBUTE1 = string.Empty;
            fsATTRIBUTE2 = string.Empty;
            fsATTRIBUTE3 = string.Empty;
            fsATTRIBUTE4 = string.Empty;
            fsATTRIBUTE5 = string.Empty;
            fsATTRIBUTE6 = string.Empty;
            fsATTRIBUTE7 = string.Empty;
            fsATTRIBUTE8 = string.Empty;
            fsATTRIBUTE9 = string.Empty;
            fsATTRIBUTE10 = string.Empty;
            fsATTRIBUTE11 = string.Empty;
            fsATTRIBUTE12 = string.Empty;
            fsATTRIBUTE13 = string.Empty;
            fsATTRIBUTE14 = string.Empty;
            fsATTRIBUTE15 = string.Empty;
            fsATTRIBUTE16 = string.Empty;
            fsATTRIBUTE17 = string.Empty;
            fsATTRIBUTE18 = string.Empty;
            fsATTRIBUTE19 = string.Empty;
            fsATTRIBUTE20 = string.Empty;
            fsATTRIBUTE21 = string.Empty;
            fsATTRIBUTE22 = string.Empty;
            fsATTRIBUTE23 = string.Empty;
            fsATTRIBUTE24 = string.Empty;
            fsATTRIBUTE25 = string.Empty;
            fsATTRIBUTE26 = string.Empty;
            fsATTRIBUTE27 = string.Empty;
            fsATTRIBUTE28 = string.Empty;
            fsATTRIBUTE29 = string.Empty;
            fsATTRIBUTE30 = string.Empty;
            fsATTRIBUTE31 = string.Empty;
            fsATTRIBUTE32 = string.Empty;
            fsATTRIBUTE33 = string.Empty;
            fsATTRIBUTE34 = string.Empty;
            fsATTRIBUTE35 = string.Empty;
            fsATTRIBUTE36 = string.Empty;
            fsATTRIBUTE37 = string.Empty;
            fsATTRIBUTE38 = string.Empty;
            fsATTRIBUTE39 = string.Empty;
            fsATTRIBUTE40 = string.Empty;
            fsATTRIBUTE41 = string.Empty;
            fsATTRIBUTE42 = string.Empty;
            fsATTRIBUTE43 = string.Empty;
            fsATTRIBUTE44 = string.Empty;
            fsATTRIBUTE45 = string.Empty;
            fsATTRIBUTE46 = string.Empty;
            fsATTRIBUTE47 = string.Empty;
            fsATTRIBUTE48 = string.Empty;
            fsATTRIBUTE49 = string.Empty;
            fsATTRIBUTE50 = string.Empty;
            fsATTRIBUTE51 = string.Empty;
            fsATTRIBUTE52 = string.Empty;
            fsATTRIBUTE53 = string.Empty;
            fsATTRIBUTE54 = string.Empty;
            fsATTRIBUTE55 = string.Empty;
            fsATTRIBUTE56 = string.Empty;
            fsATTRIBUTE57 = string.Empty;
            fsATTRIBUTE58 = string.Empty;
            fsATTRIBUTE59 = string.Empty;
            fsATTRIBUTE60 = string.Empty;
        }

        /// <summary>
        /// 新增預編詮釋資料, 包括:樣板自訂欄位資料(依欄位屬性給值).SetValue
        /// </summary>
        /// <param name="m"></param>
        public tbmARC_PRE(ArcPreModel m)
        {
            fnPRE_ID = m.fnPRE_ID;
            fsNAME = m.fsNAME;
            fsTYPE = m.fsTYPE;
            fnTEMP_ID = m.fnTEMP_ID;
            fsTITLE = m.fsTITLE ?? string.Empty;
            fsDESCRIPTION = m.fsDESCRIPTION ?? string.Empty;
            fsHASH_TAG = string.Join("^", m.HashTag.Where(s => s != "")); //自訂標籤字串陣列
            //fsHASH_TAG = m.fsHashTag;  //TIP:前端文字框的值這直接寫入 欄位資料如: 標籤^標籤^標籤, 到時開發再reviewer一次。
            fdCREATED_DATE = DateTime.Now;
            fsCREATED_BY = string.Empty;
            fdUPDATED_DATE = null;
            fsUPDATED_BY = string.Empty;

            #region 樣板自訂欄位資料(依欄位屬性給值).SetValue
            var getFields = new TemplateService().GetTemplateFieldsById(m.fnTEMP_ID);
            foreach (var f in getFields)
            {
                if (f == null) { continue; }
                string colName = f.fsFIELD;
                string colValue = m.ArcPreAttributes.FirstOrDefault(a => a.Field == f.fsFIELD).FieldValue ?? string.Empty;
                if (f.fsFIELD_TYPE.ToUpper() == "CODE")
                {
                    colValue = colValue.Replace(",", ";") + ";";
                }
                typeof(tbmARC_PRE).GetProperties().FirstOrDefault(x => x.Name == colName).SetValue(this, colValue);
            }
            #endregion

            /* 自訂欄位 fsATTRIBUTE1~60  */
            //fsATTRIBUTE1 = string.Empty;
            //fsATTRIBUTE2 = string.Empty;
            //fsATTRIBUTE3 = string.Empty;
            //fsATTRIBUTE4 = string.Empty;
            //fsATTRIBUTE5 = string.Empty;
            //fsATTRIBUTE6 = string.Empty;
            //fsATTRIBUTE7 = string.Empty;
            //fsATTRIBUTE8 = string.Empty;
            //fsATTRIBUTE9 = string.Empty;
            //fsATTRIBUTE10 = string.Empty;
            //fsATTRIBUTE11 = string.Empty;
            //fsATTRIBUTE12 = string.Empty;
            //fsATTRIBUTE13 = string.Empty;
            //fsATTRIBUTE14 = string.Empty;
            //fsATTRIBUTE15 = string.Empty;
            //fsATTRIBUTE16 = string.Empty;
            //fsATTRIBUTE17 = string.Empty;
            //fsATTRIBUTE18 = string.Empty;
            //fsATTRIBUTE19 = string.Empty;
            //fsATTRIBUTE20 = string.Empty;
            //fsATTRIBUTE21 = string.Empty;
            //fsATTRIBUTE22 = string.Empty;
            //fsATTRIBUTE23 = string.Empty;
            //fsATTRIBUTE24 = string.Empty;
            //fsATTRIBUTE25 = string.Empty;
            //fsATTRIBUTE26 = string.Empty;
            //fsATTRIBUTE27 = string.Empty;
            //fsATTRIBUTE28 = string.Empty;
            //fsATTRIBUTE29 = string.Empty;
            //fsATTRIBUTE30 = string.Empty;
            //fsATTRIBUTE31 = string.Empty;
            //fsATTRIBUTE32 = string.Empty;
            //fsATTRIBUTE33 = string.Empty;
            //fsATTRIBUTE34 = string.Empty;
            //fsATTRIBUTE35 = string.Empty;
            //fsATTRIBUTE36 = string.Empty;
            //fsATTRIBUTE37 = string.Empty;
            //fsATTRIBUTE38 = string.Empty;
            //fsATTRIBUTE39 = string.Empty;
            //fsATTRIBUTE40 = string.Empty;
            //fsATTRIBUTE41 = string.Empty;
            //fsATTRIBUTE42 = string.Empty;
            //fsATTRIBUTE43 = string.Empty;
            //fsATTRIBUTE44 = string.Empty;
            //fsATTRIBUTE45 = string.Empty;
            //fsATTRIBUTE46 = string.Empty;
            //fsATTRIBUTE47 = string.Empty;
            //fsATTRIBUTE48 = string.Empty;
            //fsATTRIBUTE49 = string.Empty;
            //fsATTRIBUTE50 = string.Empty;
            //fsATTRIBUTE51 = string.Empty;
            //fsATTRIBUTE52 = string.Empty;
            //fsATTRIBUTE53 = string.Empty;
            //fsATTRIBUTE54 = string.Empty;
            //fsATTRIBUTE55 = string.Empty;
            //fsATTRIBUTE56 = string.Empty;
            //fsATTRIBUTE57 = string.Empty;
            //fsATTRIBUTE58 = string.Empty;
            //fsATTRIBUTE59 = string.Empty;
            //fsATTRIBUTE60 = string.Empty;
        }

        /// <summary>
        /// 預編詮釋資料表 Matadata: tbmARC_PRE
        /// </summary>
        public class tbmARC_PREMetadata
        {
            [Display(Name = "預編ID")]
            public long fnPRE_ID { get; set; }

            [Required]
            [Display(Name = "預編名稱")]
            public string fsNAME { get; set; }

            [Required]
            /// <summary>
            /// 類型: S、V、A、P、D
            /// </summary>
            [Display(Name = "類型")]
            public string fsTYPE { get; set; }

            [Required]
            [Display(Name = "樣板")]
            public int fnTEMP_ID { get; set; }

            [Required]
            [Display(Name = "標題")]
            public string fsTITLE { get; set; }

            [Display(Name = "描述")]
            public string fsDESCRIPTION { get; set; }
            /// <summary>
            /// 自訂標籤_20211123ADDED
            /// </summary>
            [Display(Name = "自訂標籤")]
            public string fsHASH_TAG { get; set; }

            [Display(Name = "建立時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "最後異動時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }

            #region 自訂欄位1~60
            public string fsATTRIBUTE1 { get; set; }
            public string fsATTRIBUTE2 { get; set; }
            public string fsATTRIBUTE3 { get; set; }
            public string fsATTRIBUTE4 { get; set; }
            public string fsATTRIBUTE5 { get; set; }
            public string fsATTRIBUTE6 { get; set; }
            public string fsATTRIBUTE7 { get; set; }
            public string fsATTRIBUTE8 { get; set; }
            public string fsATTRIBUTE9 { get; set; }
            public string fsATTRIBUTE10 { get; set; }
            public string fsATTRIBUTE11 { get; set; }
            public string fsATTRIBUTE12 { get; set; }
            public string fsATTRIBUTE13 { get; set; }
            public string fsATTRIBUTE14 { get; set; }
            public string fsATTRIBUTE15 { get; set; }
            public string fsATTRIBUTE16 { get; set; }
            public string fsATTRIBUTE17 { get; set; }
            public string fsATTRIBUTE18 { get; set; }
            public string fsATTRIBUTE19 { get; set; }
            public string fsATTRIBUTE20 { get; set; }
            public string fsATTRIBUTE21 { get; set; }
            public string fsATTRIBUTE22 { get; set; }
            public string fsATTRIBUTE23 { get; set; }
            public string fsATTRIBUTE24 { get; set; }
            public string fsATTRIBUTE25 { get; set; }
            public string fsATTRIBUTE26 { get; set; }
            public string fsATTRIBUTE27 { get; set; }
            public string fsATTRIBUTE28 { get; set; }
            public string fsATTRIBUTE29 { get; set; }
            public string fsATTRIBUTE30 { get; set; }
            public string fsATTRIBUTE31 { get; set; }
            public string fsATTRIBUTE32 { get; set; }
            public string fsATTRIBUTE33 { get; set; }
            public string fsATTRIBUTE34 { get; set; }
            public string fsATTRIBUTE35 { get; set; }
            public string fsATTRIBUTE36 { get; set; }
            public string fsATTRIBUTE37 { get; set; }
            public string fsATTRIBUTE38 { get; set; }
            public string fsATTRIBUTE39 { get; set; }
            public string fsATTRIBUTE40 { get; set; }
            public string fsATTRIBUTE41 { get; set; }
            public string fsATTRIBUTE42 { get; set; }
            public string fsATTRIBUTE43 { get; set; }
            public string fsATTRIBUTE44 { get; set; }
            public string fsATTRIBUTE45 { get; set; }
            public string fsATTRIBUTE46 { get; set; }
            public string fsATTRIBUTE47 { get; set; }
            public string fsATTRIBUTE48 { get; set; }
            public string fsATTRIBUTE49 { get; set; }
            public string fsATTRIBUTE50 { get; set; }
            public string fsATTRIBUTE51 { get; set; }
            public string fsATTRIBUTE52 { get; set; }
            public string fsATTRIBUTE53 { get; set; }
            public string fsATTRIBUTE54 { get; set; }
            public string fsATTRIBUTE55 { get; set; }
            public string fsATTRIBUTE56 { get; set; }
            public string fsATTRIBUTE57 { get; set; }
            public string fsATTRIBUTE58 { get; set; }
            public string fsATTRIBUTE59 { get; set; }
            public string fsATTRIBUTE60 { get; set; }
            #endregion
        }
    }
}
