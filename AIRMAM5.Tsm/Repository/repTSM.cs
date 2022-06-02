using AIRMAM5.Tsm.Common;
using AIRMAM5.Tsm.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace AIRMAM5.Tsm.Repository
{
    public class repTSM
    {
        static string fsPLINK = Properties.Settings.Default.fsPLINK;
        static string fsTSM_HOST = Properties.Settings.Default.fsTSM_HOST;
        static string fsTSM_USER = Properties.Settings.Default.fsTSM_USER;
        static string fsTSM_PWD = Properties.Settings.Default.fsTSM_PWD;
        static string fsTSM_ADMC_USER = Properties.Settings.Default.fsTSM_ADMC_USER;
        static string fsTSM_ADMC_PWD = Properties.Settings.Default.fsTSM_ADMC_PWD;
        static string fsTSM_PATH_TRANS_KEYWORD = Properties.Settings.Default.fsTSM_PATH_TRANS_KEYWORD;

        /// <summary>
        /// 取得檔案狀態
        /// </summary>
        /// <param name="clsFILE_STATUS_ARGS"></param>
        /// <returns></returns>
        public static List<clsFILE_STATUS_RESULT> fnGET_FILE_STATUS(clsFILE_STATUS_ARGS clsFILE_STATUS_ARGS)
        {
            List<clsFILE_STATUS_RESULT> lstFILE_STATUS = new List<clsFILE_STATUS_RESULT>();
            Process p = new Process();

            try
            {
                string fsFILE_TSM_PATH = string.Empty;
                //string fsSTART_FILE_NO = clsFILE_STATUS_ARGS.lstFILE_TSM_PATH[0].fsFILE_NO;
                //string fsEND_FILE_NO = clsFILE_STATUS_ARGS.lstFILE_TSM_PATH[clsFILE_STATUS_ARGS.lstFILE_TSM_PATH.Count() - 1].fsFILE_NO;
                foreach (var item in clsFILE_STATUS_ARGS.lstFILE_TSM_PATH)
                {
                    if(!string.IsNullOrEmpty(item.fsFILE_TSM_PATH))
                        fsFILE_TSM_PATH += item.fsFILE_TSM_PATH + " ";

                    //先把要查詢的檔案編號加入回傳陣列
                    //lstFILE_STATUS.Add(new clsFILE_STATUS_RESULT() { FILE_NO = item.fsFILE_NO, FILE_STATUS = FileStatus.Empty });
                    lstFILE_STATUS.Add(new clsFILE_STATUS_RESULT() { FILE_NO = item.fsFILE_NO, FILE_STATUS = FileStatus.NotExist });
                }

                //開始走plink
                
                p.StartInfo.FileName = fsPLINK;
                p.StartInfo.Arguments = @"" + fsTSM_USER + "@" + fsTSM_HOST + " -pw " + fsTSM_PWD + " dsmls " + fsFILE_TSM_PATH + "";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                string fsRESULT = "", fsERROR = "";
                List<string> lstRESULT = new List<string>();

                p.Start();
                p.StandardInput.WriteLine("y");
                fsRESULT = p.StandardOutput.ReadToEnd();
                fsERROR = p.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(fsERROR) && !fsERROR.Contains("The server's host key is not cached"))
                {
                    throw new Exception(fsERROR);
                }

                if (!fsRESULT.StartsWith("ERROR:"))
                {
                    lstRESULT = fsRESULT.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();

                    //fnSTART_INDEX = lstRESULT.FindIndex(f => f.Contains(fsSTART_FILE_NO));
                    //fnEND_INDEX = lstRESULT.FindIndex(f => f.Contains(fsEND_FILE_NO));

                    for (int i = 0; i < lstFILE_STATUS.Count; i++)
                    {
                        string fsTEXT = lstRESULT.Find(f => f.Contains(lstFILE_STATUS[i].FILE_NO));

                        if (!string.IsNullOrEmpty(fsTEXT))
                        {
                            //改取代的取代一下: 多個空格換一個
                            while (fsTEXT.Contains("  "))
                            {
                                fsTEXT = fsTEXT.Replace("  ", " ");
                            }
                            //把一個空格換成-分隔符號
                            fsTEXT = fsTEXT.Replace(" ", "|");
                            //改取代的取代一下: 把m (p)中空格移除
                            fsTEXT = fsTEXT.Replace("m (p)", "m(p)");

                            switch (fsTEXT.Split('|')[fsTEXT.Split('|').Count() - 2].ToLower())
                            {
                                case "m":
                                    lstFILE_STATUS[i].FILE_STATUS = FileStatus.Tape;
                                    break;
                                case "m(p)":
                                    lstFILE_STATUS[i].FILE_STATUS = FileStatus.Tape;
                                    break;
                                case "p":
                                    lstFILE_STATUS[i].FILE_STATUS = FileStatus.NearLine;
                                    break;
                                case "r":
                                    lstFILE_STATUS[i].FILE_STATUS = FileStatus.NearLine;
                                    break;
                                case "n":
                                    lstFILE_STATUS[i].FILE_STATUS = FileStatus.Error;
                                    break;
                                case "g":
                                    lstFILE_STATUS[i].FILE_STATUS = FileStatus.Processing;
                                    break;
                                default:
                                    break;
                            }
                        }
                        
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                p.Close();
                p.Dispose();
            }

            return lstFILE_STATUS;
        }

        /// <summary>
        /// 取得檔案狀態(無HSM)
        /// </summary>
        /// <param name="clsFILE_STATUS_ARGS"></param>
        /// <returns></returns>
        public static List<clsFILE_STATUS_RESULT > fnGET_FILE_STATUS_WITHOUT_HSM(clsFILE_STATUS_ARGS clsFILE_STATUS_ARGS)
        {
            List<clsFILE_STATUS_RESULT> lstFILE_STATUS = new List<clsFILE_STATUS_RESULT>();
            try
            {
                string fsFILE_TSM_PATH = string.Empty;

                foreach (var item in clsFILE_STATUS_ARGS.lstFILE_TSM_PATH)
                {
                    if (System.IO.File.Exists(item.fsFILE_TSM_PATH))
                        lstFILE_STATUS.Add(new clsFILE_STATUS_RESULT() { FILE_NO = item.fsFILE_NO, FILE_STATUS = FileStatus.NearLine });
                    else
                        lstFILE_STATUS.Add(new clsFILE_STATUS_RESULT() { FILE_NO = item.fsFILE_NO, FILE_STATUS = FileStatus.Tape });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return lstFILE_STATUS;
        }

        /// <summary>
        /// 取得目前在LIB中的磁帶資訊
        /// </summary>
        /// <returns></returns>
        public static List<clsTAPE_INFO_RESULT> fnGET_TAPE_INFO_IN_LIB()
        {
            List<clsTAPE_INFO_RESULT> lstTAPE_INFO_RESULT = new List<clsTAPE_INFO_RESULT>();
            Process p = new Process();
            try
            {
                //開始走plink
                
                p.StartInfo.FileName = fsPLINK;
                p.StartInfo.Arguments = @"" + fsTSM_USER + "@" + fsTSM_HOST + " -pw " + fsTSM_PWD + " dsmadmc -id=" + fsTSM_ADMC_USER + " -password=" + fsTSM_ADMC_PWD + " -dataonly=yes -comma RUN EBC_MAM_Q_ALL_VOLUME_IN_LIB";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                string fsRESULT = "", fsERROR = "";
                p.Start();
                p.StandardInput.WriteLine("y");
                fsRESULT = p.StandardOutput.ReadToEnd();
                fsERROR = p.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(fsERROR) && !fsERROR.Contains("The server's host key is not cached"))
                {
                    throw new Exception(fsERROR);
                }

                string[] aryResult = fsRESULT.Split('\n');

                for (int i = 0; i < aryResult.Length; i++)
                {
                    string[] aryResult2 = aryResult[i].Split(',');

                    if(aryResult2.Count() == 10)
                    {
                        lstTAPE_INFO_RESULT.Add(new clsTAPE_INFO_RESULT
                        {
                            VOL_ID = aryResult2[0].Trim(),
                            VOL_TYPE = aryResult2[1].Trim(),
                            VOL_USE_STATUS = aryResult2[2].Trim(),
                            USED_GB = (string.IsNullOrEmpty(aryResult2[3].Trim()) ? 0.0 : double.Parse(aryResult2[3].Trim())),
                            READ_DATE = aryResult2[4].Trim(),
                            WRITE_DATE = aryResult2[5].Trim(),
                            POOL_NAME = aryResult2[6].Trim(),
                            VOL_RW_STATUS = aryResult2[7].Trim(),
                            WRITE_ERRORS = aryResult2[8].Trim(),
                            READ_ERRORS = aryResult2[9].Trim()
                        });
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                p.Close();
                p.Dispose();
            }

            return lstTAPE_INFO_RESULT;
        }

        /// <summary>
        /// 取得LIN納管所有的磁帶資訊
        /// </summary>
        /// <returns></returns>
        public static List<clsTAPE_INFO_RESULT> fnGET_TAPE_INFO_ALL()
        {
            List<clsTAPE_INFO_RESULT> lstTAPE_INFO_RESULT = new List<clsTAPE_INFO_RESULT>();
            Process p = new Process();
            try
            {
                //開始走plink
                
                p.StartInfo.FileName = fsPLINK;
                p.StartInfo.Arguments = @"" + fsTSM_USER + "@" + fsTSM_HOST + " -pw " + fsTSM_PWD + " dsmadmc -id=" + fsTSM_ADMC_USER + " -password=" + fsTSM_ADMC_PWD + " -dataonly=yes -comma RUN MAM_Q_ALL_VOLUME";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                //p.StartInfo.StandardOutputEncoding = System.Text.Encoding.Default;

                string fsRESULT = "", fsERROR = "";
                p.Start();
                p.StandardInput.WriteLine("y");
                fsRESULT = p.StandardOutput.ReadToEnd();
                fsERROR = p.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(fsERROR) && !fsERROR.Contains("The server's host key is not cached"))
                {
                    throw new Exception(fsERROR);
                }
                string[] aryResult = fsRESULT.Split('\n');

                for (int i = 0; i < aryResult.Length; i++)
                {
                    string[] aryResult2 = aryResult[i].Split(',');

                    if (aryResult2.Count() == 10)
                    {
                        lstTAPE_INFO_RESULT.Add(new clsTAPE_INFO_RESULT
                        {
                            VOL_ID = aryResult2[0].Trim(),
                            VOL_TYPE = aryResult2[1].Trim(),
                            VOL_USE_STATUS = aryResult2[2].Trim(),
                            USED_GB = (string.IsNullOrEmpty(aryResult2[3].Trim()) ? 0.0 : double.Parse(aryResult2[3].Trim())),
                            READ_DATE = aryResult2[4].Trim(),
                            WRITE_DATE = aryResult2[5].Trim(),
                            POOL_NAME = aryResult2[6].Trim(),
                            VOL_RW_STATUS = aryResult2[7].Trim(),
                            WRITE_ERRORS = aryResult2[8].Trim(),
                            READ_ERRORS = aryResult2[9].Trim()
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                p.Close();
                p.Dispose();
            }

            return lstTAPE_INFO_RESULT;
        }

        /// <summary>
        /// 磁帶下架
        /// </summary>
        /// <param name="lstTAPE_NO">磁帶編號</param>
        /// <returns></returns>
        public static string fnTAPE_CHECK_OUT(List<string> lstTAPE_NO)
        {
            //string RESULT = false;
            Process p = new Process();
            try
            {
                //開始走plink
                
                p.StartInfo.FileName = fsPLINK;
                p.StartInfo.Arguments = @"" + fsTSM_USER + "@" + fsTSM_HOST + " -pw " + fsTSM_PWD + " dsmadmc -id=" + fsTSM_ADMC_USER + " -password=" + fsTSM_ADMC_PWD + " RUN MAM_CHECK_OUT_DATA_VOLUME " + string.Join(",", lstTAPE_NO);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                string fsRESULT = "", fsERROR = "";
                p.Start();
                p.StandardInput.WriteLine("y");
                fsRESULT = p.StandardOutput.ReadToEnd();
                fsERROR = p.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(fsERROR) && !fsERROR.Contains("The server's host key is not cached"))
                {
                    throw new Exception(fsERROR);
                }

                if (fsRESULT.ToLower().IndexOf("success") > -1)
                {
                    return string.Empty;
                }
                else
                {
                    return fsRESULT;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                p.Close();
                p.Dispose();
            }
        }

        /// <summary>
        /// 磁帶上架
        /// </summary>
        /// <returns></returns>
        public static string fnTAPE_CHECK_IN()
        {
            //string RESULT = false;
            Process p = new Process();
            try
            {
                //開始走plink

                p.StartInfo.FileName = fsPLINK;
                p.StartInfo.Arguments = @"" + fsTSM_USER + "@" + fsTSM_HOST + " -pw " + fsTSM_PWD + " dsmadmc -id=" + fsTSM_ADMC_USER + " -password=" + fsTSM_ADMC_PWD + " -dataonly=yes -comma RUN EBC_MAM_CHECKIN";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                string fsRESULT = "", fsERROR = "";
                p.Start();
                p.StandardInput.WriteLine("y");
                fsRESULT = p.StandardOutput.ReadToEnd();
                fsERROR = p.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(fsERROR) && !fsERROR.Contains("The server's host key is not cached"))
                {
                    throw new Exception(fsERROR);
                }

                if (fsRESULT.ToLower().IndexOf("success") > -1)
                {
                    return string.Empty;
                }
                else
                {
                    return fsRESULT;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                p.Close();
                p.Dispose();
            }
        }

        /// <summary>
        /// 檢查是否正在CHECK IN
        /// </summary>
        /// <returns></returns>
        public static bool fnCHECK_IS_CHECKIN()
        {
            bool RESULT = false;
            Process p = new Process();
            try
            {
                //開始走plink
                
                p.StartInfo.FileName = fsPLINK;
                p.StartInfo.Arguments = @"" + fsTSM_USER + "@" + fsTSM_HOST + " -pw " + fsTSM_PWD + " dsmadmc -id=" + fsTSM_ADMC_USER + " -password=" + fsTSM_ADMC_PWD + " -dataonly=yes -comma q pro";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                string fsRESULT = "", fsERROR = "";
                p.Start();
                p.StandardInput.WriteLine("y");
                fsRESULT = p.StandardOutput.ReadToEnd();
                fsERROR = p.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(fsERROR) && !fsERROR.Contains("The server's host key is not cached"))
                {
                    throw new Exception(fsERROR);
                }

                if (fsRESULT.Contains("CHECKIN LIBVOLUME"))
                {
                    RESULT = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                p.Close();
                p.Dispose();
            }
            return RESULT;
        }

        /// <summary>
        /// 檢查磁帶是否在架上
        /// </summary>
        /// <returns></returns>
        public static bool fnTAPE_IS_IN_LIB(string fsTAPE_NO)
        {
            bool RESULT = false;
            
            Process p = new Process();

            try
            {
                //開始走plink
                p.StartInfo.FileName = fsPLINK;
                p.StartInfo.Arguments = @"" + fsTSM_USER + "@" + fsTSM_HOST + " -pw " + fsTSM_PWD + " dsmadmc -id=" + fsTSM_ADMC_USER + " -password=" + fsTSM_ADMC_PWD + " -dataonly=yes -comma RUN EBC_MAM_Q_IS_VOL_IN_LIB " + fsTAPE_NO;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                string fsRESULT = "", fsERROR = "";
                p.Start();
                p.StandardInput.WriteLine("y");
                fsRESULT = p.StandardOutput.ReadToEnd();
                fsERROR = p.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(fsERROR) && !fsERROR.Contains("The server's host key is not cached"))
                {
                    throw new Exception(fsERROR);
                }

                if (fsRESULT.Contains("No match found using this criteria"))
                {
                    RESULT = false;
                }
                else if (fsRESULT.Contains(fsTAPE_NO))
                {
                    RESULT = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                p.Close();
                p.Dispose();
            }
            return RESULT;
        }

        /// <summary>
        /// 詢問目前TSM是否有傳入的作業
        /// </summary>
        /// <returns></returns>
        public static bool fnGET_TSM_HAS_WORK(string fsWORK_NAME)
        {
            Process p = new Process();

            try
            {
                //開始走plink
                p.StartInfo.FileName = fsPLINK;
                p.StartInfo.Arguments = @"" + fsTSM_USER + "@" + fsTSM_HOST + " -pw " + fsTSM_PWD + " dsmadmc -id=" + fsTSM_ADMC_USER + " -password=" + fsTSM_ADMC_PWD + " q pro";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                string fsRESULT = "", fsERROR = "";
                p.Start();
                p.StandardInput.WriteLine("y");
                fsRESULT = p.StandardOutput.ReadToEnd();
                fsERROR = p.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(fsERROR) && !fsERROR.Contains("The server's host key is not cached"))
                {
                    throw new Exception(fsERROR);
                }

                if (fsRESULT.Contains(fsWORK_NAME))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                p.Close();
                p.Dispose();
            };
        }

        ///// <summary>
        ///// 取得待上架磁帶清單
        ///// </summary>
        ///// <returns></returns>
        //public static List<clsWAIT_VOL_RESULT> fnGET_WAIT_VOL()
        //{
        //    List<clsWAIT_VOL_RESULT> lstWAIT_VOL_RESULT = new List<clsWAIT_VOL_RESULT>();

        //    try
        //    {
        //        DataTable dtWAIT_VOL = clsDB.Do_Query("spTSM_GET_L_WAIT_VOL_ACTIVE_ALL", null);

        //        if(dtWAIT_VOL != null && dtWAIT_VOL.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dtWAIT_VOL.Rows.Count; i++)
        //            {
        //                lstWAIT_VOL_RESULT.Add(new clsWAIT_VOL_RESULT()
        //                {
        //                    fnWAIT_ID = long.Parse(dtWAIT_VOL.Rows[i]["fnWAIT_ID"].ToString()),

        //                    fsVOL_ID = dtWAIT_VOL.Rows[i]["fsVOL_ID"].ToString(),
        //                    fnWORK_ID = long.Parse(dtWAIT_VOL.Rows[i]["fnWORK_ID"].ToString()),
        //                    fsCREATED_BY_NAME = dtWAIT_VOL.Rows[i]["fsCREATED_BY_NAME"].ToString(),

        //                    _sBOOKING_REASON = dtWAIT_VOL.Rows[i]["_sBOOKING_REASON"].ToString(),
        //                    _sTATUS_NAME = dtWAIT_VOL.Rows[i]["_sTATUS_NAME"].ToString(),
        //                    _sPRIORITY = dtWAIT_VOL.Rows[i]["_sPRIORITY"].ToString(),
        //                });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }

        //    return lstWAIT_VOL_RESULT;

        //}

        ///// <summary>
        ///// 更新待上架磁帶狀態
        ///// </summary>
        ///// <param name="clsUPDATE_WAIT_VOL_ARGS">待上架磁帶</param>
        ///// <returns></returns>
        //public static string fnUPDATE_WAIT_VOL(clsUPDATE_WAIT_VOL_ARGS clsUPDATE_WAIT_VOL_ARGS)
        //{
        //    string fsRESULT = string.Empty;
        //    Dictionary<string, string> dicParameters = new Dictionary<string, string>();
            
        //    dicParameters.Add("fnWAIT_ID", clsUPDATE_WAIT_VOL_ARGS.fnWAIT_ID.ToString());
        //    dicParameters.Add("fsVOL_ID", clsUPDATE_WAIT_VOL_ARGS.fsVOL_ID);
        //    dicParameters.Add("fnWORK_ID", clsUPDATE_WAIT_VOL_ARGS.fnWORK_ID.ToString());
        //    dicParameters.Add("fsSTATUS", clsUPDATE_WAIT_VOL_ARGS.fsSTATUS);
        //    dicParameters.Add("fsUPDATED_BY", clsUPDATE_WAIT_VOL_ARGS.fsUPDATED_BY);
        //    fsRESULT = clsDB.Do_Tran("spTSM_UPDATE_L_WAIT_VOL", dicParameters);

        //    return fsRESULT;
        //}

        ///// <summary>
        ///// 更新已上架更新WORK狀態(改為_U)
        ///// </summary>
        ///// <param name="clsUPDATE_L_WORK_ARGS">要更新的WORK資訊</param>
        ///// <returns></returns>
        //public static string fnUPDATE_L_WORK_STATUS(clsUPDATE_L_WORK_ARGS clsUPDATE_L_WORK_ARGS)
        //{
        //    string fsRESULT = string.Empty;
        //    Dictionary<string, string> dicParameters = new Dictionary<string, string>();
            
        //    dicParameters.Add("fnWORK_ID", clsUPDATE_L_WORK_ARGS.fnWORK_ID.ToString());
        //    dicParameters.Add("fsSTATUS", clsUPDATE_L_WORK_ARGS.fsSTATUS);
        //    dicParameters.Add("fsUPDATED_BY", clsUPDATE_L_WORK_ARGS.fsUPDATED_BY);
        //    fsRESULT = clsDB.Do_Tran("spTSM_UPDATE_L_WORK_STATUS", dicParameters);

        //    return fsRESULT;
        //}
    }
}