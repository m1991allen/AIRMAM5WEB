using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;

namespace AIRMAM5.Search.Lucene.Models
{
    /// <summary>
    /// 取得索引庫相關資訊
    /// </summary>
    public class clsINDEX_INFO
    {
        private string IndexName { get; set; }
        private string IndexInfoFile { get; set; }
        private string ActiveIndexFolder { get; set; }
        private string SynonymFolder { get; set; }
        private XmlDocument xmlDOC { get; set; }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="fsINDEX_INFO_FILE">索引庫資訊檔案位置</param>
        /// <param name="fsINDEX_NAME">索引庫名稱</param>
        public clsINDEX_INFO(string fsINDEX_INFO_FILE, string fsINDEX_NAME)
        {
            this.IndexName = fsINDEX_NAME;
            this.IndexInfoFile = fsINDEX_INFO_FILE;

            if (!System.IO.File.Exists(fsINDEX_INFO_FILE))
                throw new FileNotFoundException("設定檔不存在");

            //載入設定檔
            xmlDOC = new XmlDocument();
            xmlDOC.Load(this.IndexInfoFile);
        }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="fsINDEX_INFO_FILE"></param>
        public clsINDEX_INFO(string fsINDEX_INFO_FILE)
        {
            this.IndexInfoFile = fsINDEX_INFO_FILE;

            if (!System.IO.File.Exists(fsINDEX_INFO_FILE))
                throw new FileNotFoundException("設定檔不存在");

            //載入設定檔
            xmlDOC = new XmlDocument();
            xmlDOC.Load(this.IndexInfoFile);
        }

        /// <summary>
        /// 取得目前使用中的索引庫
        /// </summary>
        /// <returns></returns>
        public string GetActiveIndexFolder()
        {
            //string fsINDEX_PATH = Properties.Settings.Default.fsYOUTUBE_INDEX + "FTSearch" + Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(Properties.Settings.Default.fsYOUTUBE_INDEX, "*.idx")[0]) + "\\";
            var Nodes = xmlDOC.SelectNodes("/Indexs/Index");

            if (Nodes != null && Nodes.Count > 0)
            {
                foreach (XmlNode item in Nodes)
                {
                    if(item.SelectSingleNode("Name").InnerText.ToLower() == this.IndexName.ToLower())
                    {
                        string fsFILE_PATH = item.SelectSingleNode("IndexDir").InnerText + IndexName;

                        if(System.IO.Directory.Exists(fsFILE_PATH))
                            this.ActiveIndexFolder = fsFILE_PATH + @"\" + "FTSearch" + Path.GetFileNameWithoutExtension(System.IO.Directory.GetFiles(fsFILE_PATH, "*.idx")[0]) + "\\";

                        break;
                    }
                }
            }
            else
                throw new Exception("找不到索引庫資訊");

            return ActiveIndexFolder;
        }

        /// <summary>
        /// 取得同義詞索引庫
        /// </summary>
        /// <returns></returns>
        public string GetSynonymFolder()
        {
            var Nodes = xmlDOC.SelectNodes("/Indexs/Synonym");

            if (Nodes != null && Nodes.Count > 0)
            {
                this.SynonymFolder = Nodes[0].SelectSingleNode("SynonymDir").InnerText + Nodes[0].SelectSingleNode("Name").InnerText;

            }
            else
                throw new Exception("找不到同義詞庫資訊");
            
            return SynonymFolder;
        }
    }
}