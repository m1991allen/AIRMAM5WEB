using AIRMAM5.DBEntity.Models.TemplateFields;

namespace AIRMAM5.DBEntity.Models.ArcPre
{
    /// <summary>
    /// 預編詮釋自訂欄位(樣板欄位)內容 (變動欄位)。 繼承參考 <see cref="TemplateFieldsModel"/>
    /// </summary>
    public class ArcPreAttributeModel : TemplateFieldsModel
    {
        /// <summary>
        /// 預編詮釋自訂欄位(樣板欄位)內容 (變動欄位)
        /// </summary>
        public ArcPreAttributeModel() { FieldValue = string.Empty; }

        /// <summary>
        /// 預編詮釋自訂欄位(樣板欄位)內容 (變動欄位)
        /// </summary>
        /// <param name="field">fsFIELD 欄位代號, EX: fsATTRIBUTE1, fsATTRIBUTE2,.... </param>
        /// <param name="fvalue"> 欄位內容值, EX: 亞洲, 陳大明,... </param>
        /// <param name="fname">fsFIELD_NAME 欄位名稱表示, EX: 地區,導演,... </param>
        public ArcPreAttributeModel(string field, string fvalue, string fname)
        {
            this.Field = field;
            this.FieldValue = fvalue;
            this.FieldName = fname;
        }

        /// <summary>
        /// 欄位值(如為CODE,即為中文值, 代碼原始值則為[fsFIELD_VALUE])
        /// </summary>
        public string FieldValue { get; set; } = string.Empty;

        /// <summary>
        /// 預編詮釋自訂欄位(樣板欄位)內容(變動欄位), 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: 
        ///     <para> 1.樣板自訂欄位.預存 : <see cref="spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result"/> </para>
        ///     <para> 2.媒資檔案-自訂欄位.預存 : <see cref="spGET_ARC_VIDEO_ATTRIBUTE_Result"/>, <see cref="spGET_ARC_AUDIO_ATTRIBUTE_Result"/>,... </para>
        ///     <para> 3.刪除紀錄-自訂欄位.預存 : <see cref="sp_t_GET_ARC_VIDEO_ATTRIBUTE_Result"/>, <see cref="sp_t_GET_ARC_AUDIO_ATTRIBUTE_Result"/>,... </para>
        /// </typeparam>
        /// <param name="m"></param>
        /// <param name="fileCattegory">媒資檔案類別: V,A,P,D  <see cref="FileTypeEnum"/> </param>
        /// <returns></returns>
        public ArcPreAttributeModel FormatConversion<T>(T m, Enums.FileTypeEnum fileCattegory)
        {
            var _properties = typeof(T).GetProperties();

            foreach (var p in _properties)
            {
                var _val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fnTEMP_ID")
                {
                    int.TryParse(_val.ToString(), out int v);
                    this.fnTEMP_ID = v;
                }
                if (p.Name == "fsFIELD") this.Field = _val.ToString();
                if (p.Name == "fsFIELD_NAME") this.FieldName = _val.ToString();
                if (p.Name == "fsFIELD_TYPE") this.FieldType = _val.ToString();
                if (p.Name == "fsDESCRIPTION") this.FieldDesc = _val.ToString();
                if (p.Name == "fnFIELD_LENGTH")
                {
                    int.TryParse(_val.ToString(), out int len);
                    this.FieldLen = len;
                }
                if (p.Name == "fnORDER")
                {
                    this.FieldOrder = string.IsNullOrEmpty(_val.ToString()) ? 99 : int.Parse(_val.ToString());
                }
                if (p.Name == "fnCTRL_WIDTH")
                {
                    this.FieldWidth = string.IsNullOrEmpty(_val.ToString()) ? 50 : int.Parse(_val.ToString());
                }
                if (p.Name == "fsMULTILINE")
                {
                    this.IsMultiline = _val.ToString().ToUpper() == Enums.IsTrueFalseEnum.Y.ToString() ? true : false;
                }
                if (p.Name == "fsISNULLABLE")
                {
                    this.IsNullable = _val.ToString().ToUpper() == Enums.IsTrueFalseEnum.Y.ToString() ? true : false;
                }

                //預存:spGET_xxxxxxxx_ATTRIBUTE_RESULT 回覆欄位[fsFIELD_VALUE], 為欄位原始值。
                if (p.Name == "fsDEFAULT" || p.Name == "C_sDEFAULT" || p.Name == "fsFIELD_VALUE")
                {
                    this.FieldDef = _val.ToString();
                }
                if (p.Name == "fsCODE_ID") this.FieldCodeId = _val.ToString();
                if (p.Name == "fnCODE_CNT")
                {
                    this.FieldCodeCnt = string.IsNullOrEmpty(_val.ToString()) ? 1 : int.Parse(_val.ToString());
                }
                if (p.Name == "fsCODE_CTRL") this.FieldCodeCtrl = _val.ToString();
                if (p.Name == "fsIS_SEARCH") this.IsSearch = _val.ToString() == Enums.IsTrueFalseEnum.Y.ToString() ? true : false;

                //欄位資料值(如為CODE,即為中文值)
                if (p.Name == "fsVALUE" || p.Name == "C_VALUE") this.FieldValue = _val.ToString();
            }

            return this;
        }

    }

}
