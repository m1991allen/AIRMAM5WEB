using System;
using System.IO;

namespace AIRMAM5.Utility.Common
{
    public class CommonFile
    {
        /// <summary>
        /// 刪除實體檔案
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string DeleteFile(string FilePath)
        {
            string result = "OK";

            if (string.IsNullOrEmpty(FilePath))
                result = "請傳入檔案路徑";
            else if (!File.Exists(FilePath))
                result = "檔案不存在";
            else
            {
                try
                {
                    File.Delete(FilePath);
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }

            return result;
        }
        
        /// <summary>
        /// 路徑中的檔案是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            if (File.Exists(filePath))
                return true;
            else
                return false;
        }
    }
}
