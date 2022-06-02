using AIRMAM5.Tsm.S3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AIRMAM5.Tsm.S3.Repository
{
    public class repS3
    {
        public List<clsFILE_STATUS_RESULT> lstFILE_STATUS_RESULT = new List<clsFILE_STATUS_RESULT>();

        //查詢檔案是否在S3標準
        public List<clsFILE_STATUS_RESULT> fnGET_FILE_IS_IN_S3_STD(clsFILE_STATUS_ARGS clsFILE_STATUS_ARGS)
        {
            var tasks = new List<Task>();

            foreach (var item in clsFILE_STATUS_ARGS.lstFILE_TSM_PATH)
            {
                lstFILE_STATUS_RESULT.Add(new clsFILE_STATUS_RESULT() { FILE_NO = item.fsFILE_NO, FILE_STATUS = FileStatus.NotExist });

                clsGET_S3 get_s3 = new clsGET_S3(item.fsFILE_NO, item.fsFILE_TSM_PATH, new S3Callback(S3ResultCallback));
                //Thread t = new Thread(new ThreadStart(get_s3.Go));
                //t.Start();
                tasks.Add(Task.Factory.StartNew(get_s3.Go));
                //t.Join();
            }

            Task.WaitAll(tasks.ToArray());

            return lstFILE_STATUS_RESULT;
        }

        public void S3ResultCallback(string file_no, string status)
        {
            if (status == "Online")
                lstFILE_STATUS_RESULT.FirstOrDefault(f => f.FILE_NO == file_no).FILE_STATUS = FileStatus.Online;
            else if (status == "Offline")
                lstFILE_STATUS_RESULT.FirstOrDefault(f => f.FILE_NO == file_no).FILE_STATUS = FileStatus.Offline;
            else if (status == "Offline_Deep")
                lstFILE_STATUS_RESULT.FirstOrDefault(f => f.FILE_NO == file_no).FILE_STATUS = FileStatus.Offline_Deep;
            else if (status == "NotExist")
                lstFILE_STATUS_RESULT.FirstOrDefault(f => f.FILE_NO == file_no).FILE_STATUS = FileStatus.NotExist;
            else
                lstFILE_STATUS_RESULT.FirstOrDefault(f => f.FILE_NO == file_no).FILE_STATUS = FileStatus.Error;
        }

        public delegate void S3Callback(string file_no, string status);

        public class clsGET_S3
        {

            private string fsFILE_NO { get; set; }
            private string fsFILE_PATH { get; set; }
            private S3Callback callback;

            public clsGET_S3(string file_no, string file_path, S3Callback call_back)
            {
                this.fsFILE_NO = file_no;
                this.fsFILE_PATH = file_path;
                this.callback = call_back;
            }

            public void Go()
            {
                try
                {
                    Process p = new Process();

                    p.StartInfo.FileName = Properties.Settings.Default.fsAWS_CMD;
                    p.StartInfo.Arguments = @"s3api head-object --profile default --bucket " + Properties.Settings.Default.fsBUCKET + " --key " + fsFILE_PATH;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;

                    string fsRESULT = "", fsERROR = "";
                    p.Start();

                    fsRESULT = p.StandardOutput.ReadToEnd();
                    fsERROR = p.StandardError.ReadToEnd();

                    p.Close();
                    p.Dispose();

                    if (string.IsNullOrEmpty(fsERROR))
                    {
                        if (fsRESULT.IndexOf("AcceptRanges") > -1)
                        {
                            clsS3 clsS3 = Newtonsoft.Json.JsonConvert.DeserializeObject<clsS3>(fsRESULT);

                            if (clsS3.Restore != null && clsS3.Restore.IndexOf("false") > -1)
                            {
                                callback(this.fsFILE_NO, "Online");
                            }
                            else if(clsS3.StorageClass == null)
                            {
                                callback(this.fsFILE_NO, "Online");
                            }
                            else
                            {
                                if(clsS3.StorageClass == "GLACIER")
                                    callback(this.fsFILE_NO, "Offline");
                                else if (clsS3.StorageClass == "DEEP_ARCHIVE")
                                    callback(this.fsFILE_NO, "Offline_Deep");
                            }
                        }
                        else if (fsRESULT.IndexOf("Not Found") > -1)
                        {
                            callback(this.fsFILE_NO, "NotExist");
                        }
                        else
                        {
                            callback(this.fsFILE_NO, "Error");
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + @"ERROR\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "fnGET_FILE_IS_IN_S3_STD ERROR : " + fsERROR + "\r\n");
                        callback(this.fsFILE_NO, "Error");
                    }
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + @"ERROR\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "fnGET_FILE_IS_IN_S3_STD ERROR : " + ex.Message + "\r\n");
                    callback(this.fsFILE_NO, "Error");
                }
            }
        }

        public bool fnRECALL_FILE(string fsFILE_PATH, int fnRESERVE_DAY, string recall_mode)
        {
            try
            {
                Process p = new Process();

                p.StartInfo.FileName = Properties.Settings.Default.fsAWS_CMD;
                p.StartInfo.Arguments = @"s3api restore-object --profile default --bucket " + Properties.Settings.Default.fsBUCKET + " --key " + fsFILE_PATH + " --restore-request Days=" + fnRESERVE_DAY + ",GlacierJobParameters={\"Tier\"=\"" + recall_mode + "\"}";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                string fsRESULT = "", fsERROR = "";
                p.Start();

                fsRESULT = p.StandardOutput.ReadToEnd();
                fsERROR = p.StandardError.ReadToEnd();

                p.Close();
                p.Dispose();

                if (string.IsNullOrEmpty(fsERROR) || fsERROR.IndexOf("already in progress") > -1)
                {
                    if (string.IsNullOrEmpty(fsRESULT))
                    {
                        return true;
                    }
                    else
                    {
                        System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + @"ERROR\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "fnRECALL_FILE ERROR : " + fsRESULT + "\r\n");
                        return false;
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + @"ERROR\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "fnRECALL_FILE ERROR : " + fsERROR + "\r\n");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + @"ERROR\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "fnRECALL_FILE ERROR : " + ex.Message + "\r\n");
                return false;
            }
        }
    }
}