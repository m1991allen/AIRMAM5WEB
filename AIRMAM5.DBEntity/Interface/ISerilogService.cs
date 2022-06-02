using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;

namespace AIRMAM5.DBEntity.Interface
{
    public interface ISerilogService
    {
        /// <summary>
        /// 記錄字串 Serilog
        /// </summary>
        /// <param name="logstr"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        bool SerilogWriter(string logstr, SerilogLevelEnum level);

        /// <summary>
        /// 儲存 Seroilog
        /// </summary>
        /// <param name="input">要存的參數</param>
        /// <returns></returns>
        bool SerilogWriter(SerilogInputModel input);

        /// <summary>
        /// Writer/Save log text. Tag:[Verbose]
        /// </summary>
        /// <param name="tagstr"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        bool WriterText(string tagstr, object input);
    }
}
