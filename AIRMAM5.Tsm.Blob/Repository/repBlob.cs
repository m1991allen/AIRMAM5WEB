using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AIRMAM5.Tsm.Blob.Models;

namespace AIRMAM5.Tsm.Blob.Repository
{
    public class repBlob
    {
        public List<clsFILE_STATUS_RESULT> lstFILE_STATUS_RESULT = new List<clsFILE_STATUS_RESULT>();

        //查詢檔案是否在Blob Hot
        public List<clsFILE_STATUS_RESULT> fnGET_FILE_IS_IN_BLOB_HOT(clsFILE_STATUS_ARGS clsFILE_STATUS_ARGS)
        {
            var tasks = new List<Task>();

            foreach (var item in clsFILE_STATUS_ARGS.lstFILE_TSM_PATH)
            {
                lstFILE_STATUS_RESULT.Add(new clsFILE_STATUS_RESULT() { FILE_NO = item.fsFILE_NO, FILE_STATUS = FileStatus.NotExist });

                clsGET_Blob get_blob = new clsGET_Blob(item.fsFILE_NO, item.fsFILE_TSM_PATH, new BlobCallback(BlobResultCallback));
                //Thread t = new Thread(new ThreadStart(get_s3.Go));
                //t.Start();
                tasks.Add(Task.Factory.StartNew(get_blob.Go));
                //t.Join();
            }

            Task.WaitAll(tasks.ToArray());

            return lstFILE_STATUS_RESULT;
        }

        public delegate void BlobCallback(string file_no, string status);

        public class clsGET_Blob
        {

            private string fsFILE_NO { get; set; }
            private string fsFILE_PATH { get; set; }
            private BlobCallback callback;

            public clsGET_Blob(string file_no, string file_path, BlobCallback call_back)
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

                    p.StartInfo.FileName = Properties.Settings.Default.fsAZ_CMD;
                    p.StartInfo.Arguments = @"-IBm azure.cli storage blob show --account-name " + Properties.Settings.Default.fsACCOUNT_NAME + " --account-key " + Properties.Settings.Default.fsACCOUNT_KEY + " --container-name " + Properties.Settings.Default.fsCONTAINER_NAME + " --name " + fsFILE_PATH;
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
                        if (fsRESULT.IndexOf("container") > -1)
                        {
                            clsBlob clsBlob = Newtonsoft.Json.JsonConvert.DeserializeObject<clsBlob>(fsRESULT);

                            if (clsBlob.properties.blobTier.ToLower() == "hot")
                                callback(this.fsFILE_NO, "Online");
                            if (clsBlob.properties.blobTier.ToLower() == "cool")
                                callback(this.fsFILE_NO, "Offline");
                            if (clsBlob.properties.blobTier.ToLower() == "archive")
                                callback(this.fsFILE_NO, "Offline_Deep");
                        }
                        else
                        {
                            System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + @"ERROR\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "fnGET_FILE_IS_IN_BLOB_STD 回傳結構錯誤 : " + fsRESULT + "\r\n");
                            callback(this.fsFILE_NO, "Error");
                        }
                    }
                    else
                    {
                        if (fsERROR.IndexOf("The specified blob does not exist") > -1)
                        {
                            callback(this.fsFILE_NO, "NotExist");
                        }
                        else
                        {
                            System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + @"ERROR\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "fnGET_FILE_IS_IN_BLOB_STD ERROR : " + fsERROR + "\r\n");
                            callback(this.fsFILE_NO, "Error");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + @"ERROR\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "fnGET_FILE_IS_IN_BLOB_STD ERROR : " + ex.Message + "\r\n");
                    callback(this.fsFILE_NO, "Error");
                }
            }
        }

        public void BlobResultCallback(string file_no, string status)
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

        public bool FileExists(string file_path)
        {
            try
            {
                Process p = new Process();

                p.StartInfo.FileName = Properties.Settings.Default.fsAZ_CMD;
                p.StartInfo.Arguments = @"-IBm azure.cli storage blob exists --account-name " + Properties.Settings.Default.fsACCOUNT_NAME + " --account-key " + Properties.Settings.Default.fsACCOUNT_KEY + " --container-name hvideo --name " + file_path;
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
                    clsFILE_EXIST clsFILE_EXIST = Newtonsoft.Json.JsonConvert.DeserializeObject<clsFILE_EXIST>(fsRESULT);
                    return clsFILE_EXIST.exists;
                }
                else
                {
                    System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + @"ERROR\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "FileExists ERROR : " + fsERROR + "\r\n");
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG_PATH + @"ERROR\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "FileExists ERROR : " + ex.Message + "\r\n");
            }
            return false;
        }

        public bool fnRECALL_FILE(string fsFILE_PATH)
        {
            try
            {
                Process p = new Process();

                p.StartInfo.FileName = Properties.Settings.Default.fsAZ_CMD;
                p.StartInfo.Arguments = @"-IBm azure.cli storage blob set-tier --account-name " + Properties.Settings.Default.fsACCOUNT_NAME + " --account-key " + Properties.Settings.Default.fsACCOUNT_KEY + " --container-name hvideo --name " + fsFILE_PATH + " --tier Hot --rehydrate-priority Standard";
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