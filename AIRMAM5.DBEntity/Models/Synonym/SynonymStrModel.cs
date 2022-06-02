using System.Text.Json.Serialization;

namespace AIRMAM5.DBEntity.Models.Synonym
{
    /// <summary>
    /// 同義詞
    /// </summary>
    public class SynonymStrModel//SynonymViewModels
    {
        /// <summary>
        /// 同義詞 字串組合  fsSYNONYM
        /// </summary>
        //[JsonPropertyName("fsSYNONYM")]
        public string fsSYNONYM { get; set; } = string.Empty;
    }

}
