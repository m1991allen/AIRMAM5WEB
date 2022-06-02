using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using Newtonsoft.Json;
using Serilog;
using System;

namespace AIRMAM5.DBEntity.Services
{
    public class SerilogService : ISerilogService
    {
        private ILogger _logger;
        //private string _showDebug = ConfigurationManager.AppSettings["ShowDebugLog"].ToString();
        //private string _showInfo = ConfigurationManager.AppSettings["ShowInfoLog"].ToString();

        public SerilogService()
        {
            _logger = Log.Logger;
        }

        /// <summary>
        /// 記錄字串 Serilog
        /// </summary>
        /// <param name="logstr"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool SerilogWriter(string logstr, SerilogLevelEnum level)
        {
            switch (level)
            {
                case SerilogLevelEnum.Verbose:
                    _logger.Verbose(logstr);
                    break;

                case SerilogLevelEnum.Debug:
                    _logger.Debug(logstr);
                    break;

                case SerilogLevelEnum.Warning:
                    _logger.Warning(logstr);
                    break;

                case SerilogLevelEnum.Error:
                    _logger.Error(logstr);
                    break;

                case SerilogLevelEnum.Fatal:
                    _logger.Fatal(logstr);
                    break;

                case SerilogLevelEnum.Information:
                default:
                    _logger.Information(logstr);
                    break;

            }
            return true;
        }

        /// <summary>
        /// 儲存 Seroilog
        /// </summary>
        /// <param name="input">要存的參數</param>
        /// <returns></returns>
        public bool SerilogWriter(SerilogInputModel input)
        {
            try
            {
                string con = string.IsNullOrWhiteSpace(input.Controller) ? "UnKnowController" : input.Controller
                    , method = string.IsNullOrWhiteSpace(input.Method) ? "UnknowMethod" : input.Method
                    , strval = input.LogString ?? "";
                string serilogString = string.Format("{0}_{1}_{2}: \n {3}", con, method, strval, JsonConvert.SerializeObject(input.Input));

                if (!string.IsNullOrEmpty(input.ErrorMessage))
                {
                    serilogString = string.Format("{0}_{1}_{2}: \n 【{3}】 \n {4}", con, method, strval, input.ErrorMessage, JsonConvert.SerializeObject(input.Input));
                }

                switch (input.EventLevel)
                {
                    case SerilogLevelEnum.Verbose:
                        _logger.Verbose(serilogString);
                        break;

                    case SerilogLevelEnum.Debug:
                        _logger.Debug(serilogString);
                        break;

                    case SerilogLevelEnum.Warning:
                        _logger.Warning(serilogString);
                        break;

                    case SerilogLevelEnum.Error:
                        _logger.Error(serilogString);
                        break;

                    case SerilogLevelEnum.Fatal:
                        _logger.Fatal(serilogString);
                        break;

                    case SerilogLevelEnum.Information:
                    default:
                        _logger.Information(serilogString);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("SerilogService: {0} \n {1}", ex.Message.ToString(), JsonConvert.SerializeObject(input.Input));
                _logger.Error("SerilogService: {0} \n {1}", ex.ToString(), JsonConvert.SerializeObject(input.Input));
                //return false;
                //throw ex;
            }
            return true;
        }

        /// <summary>
        /// Writer/Save log text. Tag:[Verbose]
        /// </summary>
        /// <param name="tagstr"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool WriterText(string tagstr, object input)
        {
            string serilogString = string.Format("{0}: \n {1}", tagstr, JsonConvert.SerializeObject(input));
            _logger.Verbose(serilogString);

            return true;
        }

    }
}
