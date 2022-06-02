//==============================================================
//<2016/11/17><David.Sin><新增本程式><檔案MediaInfo擷取>
//==============================================================
using System;
using System.Configuration;
using System.Diagnostics;

namespace AIRMAM5.FileUpload.Common
{
    /// <summary>
    /// 檔案MediaInfo擷取
    /// </summary>
    public class clsMEDIA_INFO
    {

        public static string fnGET_FILE_MEDIA_INFO(string fsFILE_PATH)
        {
            string fsRESULT = string.Empty;
            string fsERROR = string.Empty;
            Process p = new Process();
            p.StartInfo.FileName = ConfigurationManager.AppSettings["fsMEDIA_INFO"].ToString();//Properties.Settings.Default.fsMEDIA_INFO;
            p.StartInfo.Arguments = @"--Output=XML """ + fsFILE_PATH + @"""";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

            try
            {
                p.Start();

                fsRESULT = p.StandardOutput.ReadToEnd();
                fsERROR = p.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(fsERROR))
                {
                    fsRESULT = "ERROR:" + fsERROR;
                }
            }
            catch (Exception ex)
            {
                fsRESULT = "ERROR:" + ex.ToString();
            }
            finally
            {
                p.Close();
            }

            return fsRESULT;
        }
    }
}