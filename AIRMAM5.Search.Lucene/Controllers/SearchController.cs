using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml;
using AIRMAM5.Search.Lucene.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Microsoft.VisualBasic;
using NChinese.Phonetic;

namespace AIRMAM5.Search.Lucene.Controllers
{
    [RoutePrefix("Search")]
    public class SearchController : ApiController
    {
        /// <summary>
        /// 檢索各類型筆數
        /// </summary>
        /// <param name="clsPARAMETER">檢索參數</param>
        /// <returns></returns>
        [Route("SearchCount")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostSearchCount(clsPARAMETER clsPARAMETER)
        {
            clsSEARCH_RESULT.clsCOUNT clsCOUNT = new clsSEARCH_RESULT.clsCOUNT();
            clsCOUNT.fnSUBJECT_COUNT = 0;
            clsCOUNT.fnVIDEO_COUNT = 0;
            clsCOUNT.fnAUDIO_COUNT = 0;
            clsCOUNT.fnPHOTO_COUNT = 0;
            clsCOUNT.fnDOC_COUNT = 0;

            try
            {
                if (!string.IsNullOrEmpty(clsPARAMETER.fsINDEX))
                {
                    Analyzer analyzer = new StandardAnalyzer(global::Lucene.Net.Util.Version.LUCENE_30);

                    //搜尋物件集合
                    BooleanQuery bq = new BooleanQuery();

                    //filter 篩選器(Dir & Secret用)
                    BooleanFilter filter = null;
                    
                    //關鍵字
                    if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD) && !string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD.Replace(" ", "")))
                    {
                        clsPARAMETER.fsKEYWORD = ReplaceWord(clsPARAMETER.fsKEYWORD);

                        string fsKEYWORD = Regex.Replace(clsPARAMETER.fsKEYWORD, "\\s+", " ").Replace(" ", "AND").Replace("&", "AND").Replace("|", "OR").Replace("!", "NOT").Replace("AND", " AND ").Replace("OR", " OR ").Replace("NOT", " NOT ");
                        //fsKEYWORD = Strings.StrConv(fsKEYWORD.TrimStart().TrimEnd(), VbStrConv.TraditionalChinese, 0x0804);

                        //查詢模式(先找出同義詞，再看條件是否轉成同音，若是非漢字就不會轉了)
                        if (clsPARAMETER.fnSEARCH_MODE == 2)
                        {
                            //取得同義詞方法中就順便轉同音
                            fsKEYWORD = GetSynonymKeyword(fsKEYWORD, clsPARAMETER.fnHOMO);
                        }
                        else
                        {
                            if (clsPARAMETER.fnHOMO == 1)
                            {
                                fsKEYWORD = GetHomophone(fsKEYWORD);
                            }
                        }


                        QueryParser qp = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, (clsPARAMETER.fnHOMO == 0 ? "fsCONTENT_ALL" : "HOMOPHONE"), analyzer);
                        qp.AllowLeadingWildcard = true;
                        BooleanClause bc = new BooleanClause(qp.Parse(fsKEYWORD), Occur.MUST);
                        bq.Add(bc);
                    }

                    //日期
                    if (clsPARAMETER.clsDATE != null && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fsCOLUMN) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdSDATE) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdEDATE))
                    {
                        TermRangeQuery trq = new TermRangeQuery("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE.Replace("/",""), clsPARAMETER.clsDATE.fdEDATE.Replace("/", ""), true, true);
                        BooleanClause bc = new BooleanClause(trq, Occur.MUST);
                        bq.Add(bc);
                    }

                    //可查詢節點權限
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
                        {
                            string[] lstDIR_ID = clsPARAMETER.fsAUTH_DIR.Split(';');
                            FieldCacheTermsFilter f = new FieldCacheTermsFilter("$fsAUTHORUTY_DIR_ID", lstDIR_ID);
                            if (filter == null) filter = new BooleanFilter();
                            filter.Add(new FilterClause(f, Occur.MUST));
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限", Data = null }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //可查詢機密等級
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsSECRET))
                        {
                            string[] lstSECRET = clsPARAMETER.fsSECRET.Split(';');
                            FieldCacheTermsFilter f = new FieldCacheTermsFilter("$fnFILE_SECRET", lstSECRET);
                            if (filter == null) filter = new BooleanFilter();
                            filter.Add(new FilterClause(f, Occur.MUST));
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限", Data = null }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //樣板編號
                    if (clsPARAMETER.fnTEMP_ID > 0)
                    {
                        BooleanClause bc = new BooleanClause(new TermQuery(new Term("$fnTEMP_ID", clsPARAMETER.fnTEMP_ID.ToString())), Occur.MUST);
                        bq.Add(bc);
                    }

                    //欄位檢索
                    if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
                    {
                        foreach (var col_search in clsPARAMETER.lstCOLUMN_SEARCH)
                        {
                            if (col_search.fbIS_FULLTEXT)
                            {
                                QueryParser qp = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, col_search.fsCOLUMN, analyzer);
                                BooleanClause bc = new BooleanClause(qp.Parse(col_search.fsVALUE), Occur.MUST);
                                bq.Add(bc);
                            }
                            else
                            {
                                //是否為區間查詢
                                if (col_search.fsVALUE.IndexOf("~") > -1)
                                {
                                    TermRangeQuery trq = new TermRangeQuery("$" + col_search.fsCOLUMN, col_search.fsVALUE.Split('~')[0].Replace("/", ""), col_search.fsVALUE.Split('~')[1].Replace("/", ""), true, true);
                                    BooleanClause bc = new BooleanClause(trq, Occur.MUST);
                                    bq.Add(bc);
                                }
                                else
                                {
                                    BooleanClause bc = new BooleanClause(new TermQuery(new Term("$" + col_search.fsCOLUMN, col_search.fsVALUE)), Occur.MUST);
                                    bq.Add(bc);
                                }
                            }
                        }
                    }

                    foreach (var item in Properties.Settings.Default.fsINDEX.Split(','))
                    {
                        //索引庫
                        clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                        string active_folder = indexinfo.GetActiveIndexFolder();

                        if (System.IO.Directory.Exists(active_folder))
                        {
                            IndexReader indexReader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true);
                            IndexSearcher indexSearcher = new IndexSearcher(indexReader);

                            //開始檢索
                            TopDocs result = indexSearcher.Search(bq, filter == null ? null : filter, indexReader.MaxDoc);

                            if (result != null)
                            {
                                if (item.ToLower().IndexOf("video") > -1 && clsPARAMETER.fsINDEX.ToLower().IndexOf("video") > -1)
                                    clsCOUNT.fnVIDEO_COUNT += result.TotalHits;
                                else if (item.ToLower().IndexOf("audio") > -1 && clsPARAMETER.fsINDEX.ToLower().IndexOf("audio") > -1)
                                    clsCOUNT.fnAUDIO_COUNT += result.TotalHits;
                                else if (item.ToLower().IndexOf("photo") > -1 && clsPARAMETER.fsINDEX.ToLower().IndexOf("photo") > -1)
                                    clsCOUNT.fnPHOTO_COUNT += result.TotalHits;
                                else if (item.ToLower().IndexOf("doc") > -1 && clsPARAMETER.fsINDEX.ToLower().IndexOf("doc") > -1)
                                    clsCOUNT.fnDOC_COUNT += result.TotalHits;
                                else if (item.ToLower().IndexOf("subject") > -1 && clsPARAMETER.fsINDEX.ToLower().IndexOf("subject") > -1)
                                    clsCOUNT.fnSUBJECT_COUNT += result.TotalHits;
                            }
                            else
                            {
                                indexReader.Dispose();
                                indexSearcher.Dispose();

                                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "result is null" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                            }

                            indexReader.Dispose();
                            indexSearcher.Dispose();
                        }
                    }
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "請輸入要檢索的索引庫" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = clsCOUNT }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-SearchCount：" + ex.Message + "\r\n");

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 檢索各類型
        /// </summary>
        /// <param name="clsPARAMETER">檢索參數</param>
        /// <returns></returns>
        [Route("SearchMeta")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostSearchMeta(clsPARAMETER clsPARAMETER)
        {
            List<clsSEARCH_RESULT.clsARC> lstARC = new List<clsSEARCH_RESULT.clsARC>();
            List<string> lstINDEX_V = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Video") > -1).ToList();
            List<string> lstINDEX_A = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Audio") > -1).ToList();
            List<string> lstINDEX_P = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Photo") > -1).ToList();
            List<string> lstINDEX_D = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Doc") > -1).ToList();
            try
            {
                if (!string.IsNullOrEmpty(clsPARAMETER.fsINDEX))
                {
                    Analyzer analyzer = new StandardAnalyzer(global::Lucene.Net.Util.Version.LUCENE_30);

                    //搜尋物件集合
                    BooleanQuery bq = new BooleanQuery();

                    //filter 篩選器(Dir & Secret用)
                    BooleanFilter filter = null;

                    //排序
                    SortField[] sortfield = null;

                    //關鍵字
                    if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD) && !string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD.Replace(" ", "")))
                    {
                        clsPARAMETER.fsKEYWORD = ReplaceWord(clsPARAMETER.fsKEYWORD);

                        string fsKEYWORD = Regex.Replace(clsPARAMETER.fsKEYWORD, "\\s+", " ").Replace(" ", "AND").Replace("&", "AND").Replace("|", "OR").Replace("!", "NOT").Replace("AND", " AND ").Replace("OR", " OR ").Replace("NOT", " NOT ");
                        //fsKEYWORD = Strings.StrConv(fsKEYWORD.TrimStart().TrimEnd(), VbStrConv.TraditionalChinese, 0x0804);

                        //查詢模式
                        if (clsPARAMETER.fnSEARCH_MODE == 2)
                        {
                            fsKEYWORD = GetSynonymKeyword(fsKEYWORD, clsPARAMETER.fnHOMO);
                        }
                        else
                        {
                            if (clsPARAMETER.fnHOMO == 1)
                            {
                                fsKEYWORD = GetHomophone(fsKEYWORD);
                            }
                        }

                        QueryParser qp = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, (clsPARAMETER.fnHOMO == 0 ? "fsCONTENT_ALL" : "HOMOPHONE"), analyzer);
                        qp.AllowLeadingWildcard = true;
                        BooleanClause bc = new BooleanClause(qp.Parse(fsKEYWORD), Occur.MUST);
                        bq.Add(bc);
                    }

                    //日期
                    if (clsPARAMETER.clsDATE != null && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fsCOLUMN) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdSDATE) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdEDATE))
                    {
                        TermRangeQuery trq = new TermRangeQuery("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE.Replace("/", ""), clsPARAMETER.clsDATE.fdEDATE.Replace("/", ""), true, true);
                        BooleanClause bc = new BooleanClause(trq, Occur.MUST);
                        bq.Add(bc);
                    }

                    //可查詢節點權限
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
                        {
                            string[] lstDIR_ID = clsPARAMETER.fsAUTH_DIR.Split(';');
                            FieldCacheTermsFilter f = new FieldCacheTermsFilter("$fsAUTHORUTY_DIR_ID", lstDIR_ID);
                            if (filter == null) filter = new BooleanFilter();
                            filter.Add(new FilterClause(f, Occur.MUST));
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //可查詢機密等級
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsSECRET))
                        {
                            string[] lstSECRET = clsPARAMETER.fsSECRET.Split(';');
                            FieldCacheTermsFilter f = new FieldCacheTermsFilter("$fnFILE_SECRET", lstSECRET);
                            if (filter == null) filter = new BooleanFilter();
                            filter.Add(new FilterClause(f, Occur.MUST));
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //樣板編號
                    if (clsPARAMETER.fnTEMP_ID > 0)
                    {
                        BooleanClause bc = new BooleanClause(new TermQuery(new Term("$fnTEMP_ID", clsPARAMETER.fnTEMP_ID.ToString())), Occur.MUST);
                        bq.Add(bc);
                    }

                    //欄位檢索
                    if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
                    {
                        foreach (var col_search in clsPARAMETER.lstCOLUMN_SEARCH)
                        {
                            if (col_search.fbIS_FULLTEXT)
                            {
                                QueryParser qp = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, col_search.fsCOLUMN, analyzer);
                                BooleanClause bc = new BooleanClause(qp.Parse(col_search.fsVALUE), Occur.MUST);
                                bq.Add(bc);
                            }
                            else
                            {
                                //是否為區間查詢
                                if (col_search.fsVALUE.IndexOf("~") > -1)
                                {
                                    TermRangeQuery trq = new TermRangeQuery("$" + col_search.fsCOLUMN, col_search.fsVALUE.Split('~')[0].Replace("/", ""), col_search.fsVALUE.Split('~')[1].Replace("/", ""), true, true);
                                    BooleanClause bc = new BooleanClause(trq, Occur.MUST);
                                    bq.Add(bc);
                                }
                                else
                                {
                                    BooleanClause bc = new BooleanClause(new TermQuery(new Term("$" + col_search.fsCOLUMN, col_search.fsVALUE)), Occur.MUST);
                                    bq.Add(bc);
                                }
                            }
                        }
                    }

                    //排序
                    if (clsPARAMETER.lstCOLUMN_ORDER != null && clsPARAMETER.lstCOLUMN_ORDER.Count > 0)
                    {
                        sortfield = new SortField[clsPARAMETER.lstCOLUMN_ORDER.Count];
                        for (int i = 0; i < clsPARAMETER.lstCOLUMN_ORDER.Count; i++)
                        {
                            if (clsPARAMETER.lstCOLUMN_ORDER[i].fsVALUE == "1")
                                sortfield[i] = new SortField("$" + clsPARAMETER.lstCOLUMN_ORDER[i].fsCOLUMN, SortField.STRING, false);
                            else
                                sortfield[i] = new SortField("$" + clsPARAMETER.lstCOLUMN_ORDER[i].fsCOLUMN, SortField.STRING, true);
                        }
                    }

                    //索引庫
                    List<IndexReader> indexReader = new List<IndexReader>(); ;

                    if (clsPARAMETER.fsINDEX.ToLower().Contains("video"))
                    {
                        foreach (var item in lstINDEX_V)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }
                    else if (clsPARAMETER.fsINDEX.ToLower().Contains("audio"))
                    {
                        foreach (var item in lstINDEX_A)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }
                    else if (clsPARAMETER.fsINDEX.ToLower().Contains("photo"))
                    {
                        foreach (var item in lstINDEX_P)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }
                    else if (clsPARAMETER.fsINDEX.ToLower().Contains("doc"))
                    {
                        foreach (var item in lstINDEX_D)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }

                    MultiReader readers = new MultiReader(indexReader.ToArray());
                    //IndexReader indexReader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true);
                    IndexSearcher indexSearcher = new IndexSearcher(readers);

                    //開始檢索
                    TopDocs result = indexSearcher.Search(bq, filter == null ? null : filter, readers.MaxDoc, (sortfield == null ? null : new Sort(sortfield)));

                    var hits = result.ScoreDocs;

                    if (result != null)
                    {
                        if (clsPARAMETER.fnSTART_INDEX + clsPARAMETER.fnPAGE_SIZE - 1 >= result.TotalHits)
                        {
                            clsPARAMETER.fnPAGE_SIZE = clsPARAMETER.fnSTART_INDEX + (result.TotalHits - clsPARAMETER.fnSTART_INDEX);
                        }
                        else
                        {
                            clsPARAMETER.fnPAGE_SIZE = clsPARAMETER.fnSTART_INDEX + clsPARAMETER.fnPAGE_SIZE - 1;
                        }

                        string[] lstKEYWORD = (string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD) ? null : clsPARAMETER.fsKEYWORD.Replace("\\s+", " ").Replace(" ", "AND").Replace("&", "AND").Replace("|", "OR").Replace("!", "NOT").Replace("AND", "|").Replace("OR", "|").Replace("NOT", "|").Split('|'));

                        SimpleHTMLFormatter formatter = new SimpleHTMLFormatter("<<", ">>");
                        QueryScorer scorer = new QueryScorer(bq, "fsCONTENT_ALL");
                        var highlighter = new Highlighter(formatter, scorer);


                        for (int i = clsPARAMETER.fnSTART_INDEX - 1; i < clsPARAMETER.fnPAGE_SIZE; i++)
                        {
                            var documentFromSearcher = indexSearcher.Doc(hits[i].Doc);

                            List<string> lstContent = indexSearcher.Doc(hits[i].Doc).Get("fsCONTENT_ALL").ToString().Split(' ').ToList();
                            string content = string.Join(" ", lstContent.Where(s => !string.IsNullOrEmpty(s)));

                            string fsCONTENT_ALL = string.Empty;

                            if (string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD))
                            {
                                fsCONTENT_ALL = GetContent(content);
                            }
                            else if (lstKEYWORD.Where(w => content.IndexOf(w) > -1).Count() == 0)
                            {
                                fsCONTENT_ALL = GetContent(content);
                            }
                            else
                            {
                                if (content.Length > 100)
                                    highlighter.TextFragmenter = new SimpleFragmenter(100);

                                fsCONTENT_ALL = highlighter.GetBestFragment(analyzer, "fsCONTENT_ALL", content);
                            }


                            //string fsCONTENT_ALL = (string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD) ? GetContent(documentFromSearcher.Get("fsCONTENT_ALL").Replace(" ", "")) : GetContent(documentFromSearcher.Get("fsCONTENT_ALL").Replace(" ", ""), lstKEYWORD[0]));

                            //if (lstKEYWORD != null && lstKEYWORD.Count() > 0)
                            //{
                            //    foreach (var item in lstKEYWORD)
                            //    {
                            //        fsCONTENT_ALL = fsCONTENT_ALL.Replace(item.Replace("*", ""), "<<" + item.Replace("*", "") + ">>");
                            //    }
                            //}


                            lstARC.Add(new clsSEARCH_RESULT.clsARC()
                            {
                                fsFILE_NO = documentFromSearcher.Get("fsFILE_NO"),
                                fsMATCH = fsCONTENT_ALL
                            });
                        }
                    }
                    else
                    {
                        readers.Dispose();
                        indexSearcher.Dispose();

                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "result is null" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                    }

                    readers.Dispose();
                    indexSearcher.Dispose();

                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = lstARC }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "請輸入要檢索的索引庫" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }


            }
            catch (Exception ex)
            {

                System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-SearchMeta：" + ex.Message + "\r\n");

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 檢索各類型For資料匯出
        /// </summary>
        /// <param name="clsPARAMETER">檢索參數</param>
        /// <returns></returns>
        [Route("SearchMetaExport")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostSearchMetaExport(clsPARAMETER clsPARAMETER)
        {
            List<clsSEARCH_RESULT.clsARC> lstARC = new List<clsSEARCH_RESULT.clsARC>();
            List<string> lstINDEX_V = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Video") > -1).ToList();
            List<string> lstINDEX_A = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Audio") > -1).ToList();
            List<string> lstINDEX_P = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Photo") > -1).ToList();
            List<string> lstINDEX_D = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Doc") > -1).ToList();
            try
            {
                if (!string.IsNullOrEmpty(clsPARAMETER.fsINDEX))
                {
                    Analyzer analyzer = new StandardAnalyzer(global::Lucene.Net.Util.Version.LUCENE_30);

                    //搜尋物件集合
                    BooleanQuery bq = new BooleanQuery();

                    //filter 篩選器(Dir & Secret用)
                    BooleanFilter filter = null;

                    //排序
                    SortField[] sortfield = null;

                    //關鍵字
                    if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD) && !string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD.Replace(" ", "")))
                    {
                        clsPARAMETER.fsKEYWORD = ReplaceWord(clsPARAMETER.fsKEYWORD);

                        string fsKEYWORD = Regex.Replace(clsPARAMETER.fsKEYWORD, "\\s+", " ").Replace(" ", "AND").Replace("&", "AND").Replace("|", "OR").Replace("!", "NOT").Replace("AND", " AND ").Replace("OR", " OR ").Replace("NOT", " NOT ");
                        //fsKEYWORD = Strings.StrConv(fsKEYWORD.TrimStart().TrimEnd(), VbStrConv.TraditionalChinese, 0x0804);

                        //查詢模式
                        if (clsPARAMETER.fnSEARCH_MODE == 2)
                        {
                            fsKEYWORD = GetSynonymKeyword(fsKEYWORD, clsPARAMETER.fnHOMO);
                        }
                        else
                        {
                            if (clsPARAMETER.fnHOMO == 1)
                            {
                                fsKEYWORD = GetHomophone(fsKEYWORD);
                            }
                        }

                        QueryParser qp = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, (clsPARAMETER.fnHOMO == 0 ? "fsCONTENT_ALL" : "HOMOPHONE"), analyzer);
                        qp.AllowLeadingWildcard = true;
                        BooleanClause bc = new BooleanClause(qp.Parse(fsKEYWORD), Occur.MUST);
                        bq.Add(bc);
                    }

                    //日期
                    if (clsPARAMETER.clsDATE != null && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fsCOLUMN) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdSDATE) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdEDATE))
                    {
                        TermRangeQuery trq = new TermRangeQuery("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE.Replace("/", ""), clsPARAMETER.clsDATE.fdEDATE.Replace("/", ""), true, true);
                        BooleanClause bc = new BooleanClause(trq, Occur.MUST);
                        bq.Add(bc);
                    }

                    //可查詢節點權限
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
                        {
                            string[] lstDIR_ID = clsPARAMETER.fsAUTH_DIR.Split(';');
                            FieldCacheTermsFilter f = new FieldCacheTermsFilter("$fsAUTHORUTY_DIR_ID", lstDIR_ID);
                            if (filter == null) filter = new BooleanFilter();
                            filter.Add(new FilterClause(f, Occur.MUST));
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //可查詢機密等級
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsSECRET))
                        {
                            string[] lstSECRET = clsPARAMETER.fsSECRET.Split(';');
                            FieldCacheTermsFilter f = new FieldCacheTermsFilter("$fnFILE_SECRET", lstSECRET);
                            if (filter == null) filter = new BooleanFilter();
                            filter.Add(new FilterClause(f, Occur.MUST));
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //樣板編號
                    if (clsPARAMETER.fnTEMP_ID > 0)
                    {
                        BooleanClause bc = new BooleanClause(new TermQuery(new Term("$fnTEMP_ID", clsPARAMETER.fnTEMP_ID.ToString())), Occur.MUST);
                        bq.Add(bc);
                    }

                    //欄位檢索
                    if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
                    {
                        foreach (var col_search in clsPARAMETER.lstCOLUMN_SEARCH)
                        {
                            if (col_search.fbIS_FULLTEXT)
                            {
                                QueryParser qp = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, col_search.fsCOLUMN, analyzer);
                                BooleanClause bc = new BooleanClause(qp.Parse(col_search.fsVALUE), Occur.MUST);
                                bq.Add(bc);
                            }
                            else
                            {
                                //是否為區間查詢
                                if (col_search.fsVALUE.IndexOf("~") > -1)
                                {
                                    TermRangeQuery trq = new TermRangeQuery("$" + col_search.fsCOLUMN, col_search.fsVALUE.Split('~')[0].Replace("/", ""), col_search.fsVALUE.Split('~')[1].Replace("/", ""), true, true);
                                    BooleanClause bc = new BooleanClause(trq, Occur.MUST);
                                    bq.Add(bc);
                                }
                                else
                                {
                                    BooleanClause bc = new BooleanClause(new TermQuery(new Term("$" + col_search.fsCOLUMN, col_search.fsVALUE)), Occur.MUST);
                                    bq.Add(bc);
                                }
                            }
                        }
                    }

                    //排序
                    if (clsPARAMETER.lstCOLUMN_ORDER != null && clsPARAMETER.lstCOLUMN_ORDER.Count > 0)
                    {
                        sortfield = new SortField[clsPARAMETER.lstCOLUMN_ORDER.Count];
                        for (int i = 0; i < clsPARAMETER.lstCOLUMN_ORDER.Count; i++)
                        {
                            if (clsPARAMETER.lstCOLUMN_ORDER[i].fsVALUE == "1")
                                sortfield[i] = new SortField("$" + clsPARAMETER.lstCOLUMN_ORDER[i].fsCOLUMN, SortField.STRING, false);
                            else
                                sortfield[i] = new SortField("$" + clsPARAMETER.lstCOLUMN_ORDER[i].fsCOLUMN, SortField.STRING, true);
                        }
                    }

                    //索引庫
                    List<IndexReader> indexReader = new List<IndexReader>(); ;

                    if (clsPARAMETER.fsINDEX.ToLower().Contains("video"))
                    {
                        foreach (var item in lstINDEX_V)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }
                    else if (clsPARAMETER.fsINDEX.ToLower().Contains("audio"))
                    {
                        foreach (var item in lstINDEX_A)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }
                    else if (clsPARAMETER.fsINDEX.ToLower().Contains("photo"))
                    {
                        foreach (var item in lstINDEX_P)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }
                    else if (clsPARAMETER.fsINDEX.ToLower().Contains("doc"))
                    {
                        foreach (var item in lstINDEX_D)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }

                    MultiReader readers = new MultiReader(indexReader.ToArray());
                    //IndexReader indexReader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true);
                    IndexSearcher indexSearcher = new IndexSearcher(readers);

                    //開始檢索
                    TopDocs result = indexSearcher.Search(bq, filter == null ? null : filter, readers.MaxDoc, (sortfield == null ? null : new Sort(sortfield)));

                    var hits = result.ScoreDocs;

                    if (result != null)
                    {
                        //if (clsPARAMETER.fnSTART_INDEX + clsPARAMETER.fnPAGE_SIZE - 1 >= result.TotalHits)
                        //{
                        //    clsPARAMETER.fnPAGE_SIZE = clsPARAMETER.fnSTART_INDEX + (result.TotalHits - clsPARAMETER.fnSTART_INDEX);
                        //}
                        //else
                        //{
                        //    clsPARAMETER.fnPAGE_SIZE = clsPARAMETER.fnSTART_INDEX + clsPARAMETER.fnPAGE_SIZE - 1;
                        //}

                        //string[] lstKEYWORD = (string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD) ? null : clsPARAMETER.fsKEYWORD.Replace(" ", "").Replace("\\s+", " ").Replace(" ", "AND").Replace("&", "AND").Replace("|", "OR").Replace("!", "NOT").Replace("AND", "|").Replace("OR", "|").Replace("NOT", "|").Split('|'));

                        for (int i = 0; i < result.TotalHits; i++)
                        {
                            lstARC.Add(new clsSEARCH_RESULT.clsARC()
                            {
                                fsFILE_NO = indexSearcher.Doc(hits[i].Doc).Get("fsFILE_NO"),
                                fsMATCH = string.Empty
                            });
                        }
                    }
                    else
                    {
                        readers.Dispose();
                        indexSearcher.Dispose();

                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "result is null" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                    }

                    readers.Dispose();
                    indexSearcher.Dispose();

                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = lstARC }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "請輸入要檢索的索引庫" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }


            }
            catch (Exception ex)
            {

                System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-SearchMetaExport：" + ex.Message + "\r\n");

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 檢索各類型For資料匯出
        /// </summary>
        /// <param name="clsPARAMETER">檢索參數</param>
        /// <returns></returns>
        [Route("SearchMetaTemplate")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostSearchMetaTemplate(clsPARAMETER clsPARAMETER)
        {
            List<clsSEARCH_RESULT.clsTEMP> lstTEMP = new List<clsSEARCH_RESULT.clsTEMP>();
            List<string> lstINDEX_V = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Video") > -1).ToList();
            List<string> lstINDEX_A = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Audio") > -1).ToList();
            List<string> lstINDEX_P = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Photo") > -1).ToList();
            List<string> lstINDEX_D = Properties.Settings.Default.fsINDEX.Split(',').ToList().Where(f => f.IndexOf("Doc") > -1).ToList();
            try
            {
                if (!string.IsNullOrEmpty(clsPARAMETER.fsINDEX))
                {
                    Analyzer analyzer = new StandardAnalyzer(global::Lucene.Net.Util.Version.LUCENE_30);

                    //搜尋物件集合
                    BooleanQuery bq = new BooleanQuery();

                    //filter 篩選器(Dir & Secret用)
                    BooleanFilter filter = null;
                    
                    //排序
                    SortField[] sortfield = null;

                    //關鍵字
                    if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD) && !string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD.Replace(" ", "")))
                    {
                        clsPARAMETER.fsKEYWORD = ReplaceWord(clsPARAMETER.fsKEYWORD);

                        string fsKEYWORD = Regex.Replace(clsPARAMETER.fsKEYWORD, "\\s+", " ").Replace(" ", "AND").Replace("&", "AND").Replace("|", "OR").Replace("!", "NOT").Replace("AND", " AND ").Replace("OR", " OR ").Replace("NOT", " NOT ");
                        //fsKEYWORD = Strings.StrConv(fsKEYWORD.TrimStart().TrimEnd(), VbStrConv.TraditionalChinese, 0x0804);

                        //查詢模式
                        if (clsPARAMETER.fnSEARCH_MODE == 2)
                        {
                            fsKEYWORD = GetSynonymKeyword(fsKEYWORD, clsPARAMETER.fnHOMO);
                        }
                        else
                        {
                            if (clsPARAMETER.fnHOMO == 1)
                            {
                                fsKEYWORD = GetHomophone(fsKEYWORD);
                            }
                        }

                        QueryParser qp = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, (clsPARAMETER.fnHOMO == 0 ? "fsCONTENT_ALL" : "HOMOPHONE"), analyzer);
                        qp.AllowLeadingWildcard = true;
                        BooleanClause bc = new BooleanClause(qp.Parse(fsKEYWORD), Occur.MUST);
                        bq.Add(bc);
                    }

                    //日期
                    if (clsPARAMETER.clsDATE != null && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fsCOLUMN) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdSDATE) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdEDATE))
                    {
                        TermRangeQuery trq = new TermRangeQuery("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE.Replace("/", ""), clsPARAMETER.clsDATE.fdEDATE.Replace("/", ""), true, true);
                        BooleanClause bc = new BooleanClause(trq, Occur.MUST);
                        bq.Add(bc);
                    }

                    //可查詢節點權限
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
                        {
                            string[] lstDIR_ID = clsPARAMETER.fsAUTH_DIR.Split(';');
                            FieldCacheTermsFilter f = new FieldCacheTermsFilter("$fsAUTHORUTY_DIR_ID", lstDIR_ID);
                            if (filter == null) filter = new BooleanFilter();
                            filter.Add(new FilterClause(f, Occur.MUST));
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //可查詢機密等級
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsSECRET))
                        {
                            string[] lstSECRET = clsPARAMETER.fsSECRET.Split(';');
                            FieldCacheTermsFilter f = new FieldCacheTermsFilter("$fnFILE_SECRET", lstSECRET);
                            if (filter == null) filter = new BooleanFilter();
                            filter.Add(new FilterClause(f, Occur.MUST));
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //樣板編號
                    if (clsPARAMETER.fnTEMP_ID > 0)
                    {
                        BooleanClause bc = new BooleanClause(new TermQuery(new Term("$fnTEMP_ID", clsPARAMETER.fnTEMP_ID.ToString())), Occur.MUST);
                        bq.Add(bc);
                    }

                    //欄位檢索
                    if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
                    {
                        foreach (var col_search in clsPARAMETER.lstCOLUMN_SEARCH)
                        {
                            if (col_search.fbIS_FULLTEXT)
                            {
                                QueryParser qp = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, col_search.fsCOLUMN, analyzer);
                                BooleanClause bc = new BooleanClause(qp.Parse(col_search.fsVALUE), Occur.MUST);
                                bq.Add(bc);
                            }
                            else
                            {
                                //是否為區間查詢
                                if (col_search.fsVALUE.IndexOf("~") > -1)
                                {
                                    TermRangeQuery trq = new TermRangeQuery("$" + col_search.fsCOLUMN, col_search.fsVALUE.Split('~')[0].Replace("/", ""), col_search.fsVALUE.Split('~')[1].Replace("/", ""), true, true);
                                    BooleanClause bc = new BooleanClause(trq, Occur.MUST);
                                    bq.Add(bc);
                                }
                                else
                                {
                                    BooleanClause bc = new BooleanClause(new TermQuery(new Term("$" + col_search.fsCOLUMN, col_search.fsVALUE)), Occur.MUST);
                                    bq.Add(bc);
                                }
                            }
                        }
                    }

                    //排序
                    if (clsPARAMETER.lstCOLUMN_ORDER != null && clsPARAMETER.lstCOLUMN_ORDER.Count > 0)
                    {
                        sortfield = new SortField[clsPARAMETER.lstCOLUMN_ORDER.Count];
                        for (int i = 0; i < clsPARAMETER.lstCOLUMN_ORDER.Count; i++)
                        {
                            if (clsPARAMETER.lstCOLUMN_ORDER[i].fsVALUE == "1")
                                sortfield[i] = new SortField("$" + clsPARAMETER.lstCOLUMN_ORDER[i].fsCOLUMN, SortField.STRING, false);
                            else
                                sortfield[i] = new SortField("$" + clsPARAMETER.lstCOLUMN_ORDER[i].fsCOLUMN, SortField.STRING, true);
                        }
                    }

                    //索引庫
                    List<IndexReader> indexReader = new List<IndexReader>(); ;

                    if (clsPARAMETER.fsINDEX.ToLower().Contains("video"))
                    {
                        foreach (var item in lstINDEX_V)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }
                    else if (clsPARAMETER.fsINDEX.ToLower().Contains("audio"))
                    {
                        foreach (var item in lstINDEX_A)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }
                    else if (clsPARAMETER.fsINDEX.ToLower().Contains("photo"))
                    {
                        foreach (var item in lstINDEX_P)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }
                    else if (clsPARAMETER.fsINDEX.ToLower().Contains("doc"))
                    {
                        foreach (var item in lstINDEX_D)
                        {
                            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                            string active_folder = indexinfo.GetActiveIndexFolder();
                            if (System.IO.Directory.Exists(active_folder))
                                indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                        }
                    }

                    MultiReader readers = new MultiReader(indexReader.ToArray());
                    //IndexReader indexReader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true);
                    IndexSearcher indexSearcher = new IndexSearcher(readers);

                    //開始檢索
                    bool flag = true;
                    string temp_id = string.Empty;

                    while (flag)
                    {
                        TopDocs result = indexSearcher.Search(bq, filter, readers.MaxDoc, (sortfield == null ? null : new Sort(sortfield)));

                        var hits = result.ScoreDocs;

                        if (result != null)
                        {
                            if (result.TotalHits > 0)
                            {
                                temp_id += indexSearcher.Doc(hits[0].Doc).Get("$fnTEMP_ID").ToString() + ';';
                                //層層加上temp_id 去 filter，直到沒有結果
                                string[] lstTEMP_ID = { indexSearcher.Doc(hits[0].Doc).Get("$fnTEMP_ID").ToString() };
                                FieldCacheTermsFilter f = new FieldCacheTermsFilter("$fnTEMP_ID", lstTEMP_ID);
                                if (filter == null) filter = new BooleanFilter();
                                filter.Add(new FilterClause(f, Occur.MUST_NOT));
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                    }

                    if (!string.IsNullOrEmpty(temp_id))
                    {
                        foreach (var item in temp_id.Substring(0, temp_id.Length - 1).Split(';'))
                        {
                            lstTEMP.Add(new clsSEARCH_RESULT.clsTEMP() { fnTEMP_ID = int.Parse(item) });
                        }
                    }

                    readers.Dispose();
                    indexSearcher.Dispose();

                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = lstTEMP.OrderBy(o => o.fnTEMP_ID) }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "請輸入要檢索的索引庫" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }


            }
            catch (Exception ex)
            {

                System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-SearchMetaTemplate：" + ex.Message + "\r\n");

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 檢索後依照欄位做成群組
        /// </summary>
        /// <param name="clsPARAMETER">檢索參數</param>
        /// <returns></returns>
        [Route("SearchGroup")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostSearchGroup(clsPARAMETER clsPARAMETER)
        {
            List<clsSEARCH_RESULT.clsGROUP> lstGROUP = new List<clsSEARCH_RESULT.clsGROUP>();
            try
            {
                if (!string.IsNullOrEmpty(clsPARAMETER.fsINDEX))
                {
                    Analyzer analyzer = new StandardAnalyzer(global::Lucene.Net.Util.Version.LUCENE_30);

                    //搜尋物件集合
                    BooleanQuery bq = new BooleanQuery();

                    //filter 篩選器(Dir & Secret用)
                    BooleanFilter filter = null;

                    //排序
                    //SortField[] sortfield = null;

                    //關鍵字
                    if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD) && !string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD.Replace(" ", "")))
                    {
                        clsPARAMETER.fsKEYWORD = ReplaceWord(clsPARAMETER.fsKEYWORD);

                        string fsKEYWORD = Regex.Replace(clsPARAMETER.fsKEYWORD, "\\s+", " ").Replace(" ", "AND").Replace("&", "AND").Replace("|", "OR").Replace("!", "NOT").Replace("AND", " AND ").Replace("OR", " OR ").Replace("NOT", " NOT ");
                        //fsKEYWORD = Strings.StrConv(fsKEYWORD.TrimStart().TrimEnd(), VbStrConv.TraditionalChinese, 0x0804);

                        //查詢模式
                        if (clsPARAMETER.fnSEARCH_MODE == 2)
                        {
                            fsKEYWORD = GetSynonymKeyword(fsKEYWORD, clsPARAMETER.fnHOMO);
                        }
                        else
                        {
                            if (clsPARAMETER.fnHOMO == 1)
                            {
                                fsKEYWORD = GetHomophone(fsKEYWORD);
                            }
                        }

                        QueryParser qp = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, (clsPARAMETER.fnHOMO == 0 ? "fsCONTENT_ALL" : "HOMOPHONE"), analyzer);
                        qp.AllowLeadingWildcard = true;
                        BooleanClause bc = new BooleanClause(qp.Parse(fsKEYWORD), Occur.MUST);
                        bq.Add(bc);
                    }

                    //日期
                    if (clsPARAMETER.clsDATE != null && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fsCOLUMN) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdSDATE) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdEDATE))
                    {
                        TermRangeQuery trq = new TermRangeQuery("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE, clsPARAMETER.clsDATE.fdEDATE, true, true);
                        BooleanClause bc = new BooleanClause(trq, Occur.MUST);
                        bq.Add(bc);
                    }

                    //可查詢節點權限
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
                        {
                            string[] lstDIR_ID = clsPARAMETER.fsAUTH_DIR.Split(';');
                            FieldCacheTermsFilter f = new FieldCacheTermsFilter("$fsAUTHORUTY_DIR_ID", lstDIR_ID);
                            if (filter == null) filter = new BooleanFilter();
                            filter.Add(new FilterClause(f, Occur.MUST));
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //可查詢機密等級
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsSECRET))
                        {
                            string[] lstSECRET = clsPARAMETER.fsSECRET.Split(';');
                            FieldCacheTermsFilter f = new FieldCacheTermsFilter("$fnFILE_SECRET", lstSECRET);
                            if (filter == null) filter = new BooleanFilter();
                            filter.Add(new FilterClause(f, Occur.MUST));
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //樣板編號
                    if (clsPARAMETER.fnTEMP_ID > 0)
                    {
                        BooleanClause bc = new BooleanClause(new TermQuery(new Term("$fnTEMP_ID", clsPARAMETER.fnTEMP_ID.ToString())), Occur.MUST);
                        bq.Add(bc);
                    }

                    //欄位檢索
                    if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
                    {
                        foreach (var col_search in clsPARAMETER.lstCOLUMN_SEARCH)
                        {
                            if (col_search.fbIS_FULLTEXT)
                            {
                                QueryParser qp = new QueryParser(global::Lucene.Net.Util.Version.LUCENE_30, col_search.fsCOLUMN, analyzer);
                                BooleanClause bc = new BooleanClause(qp.Parse(col_search.fsVALUE), Occur.MUST);
                                bq.Add(bc);
                            }
                            else
                            {
                                //是否為區間查詢
                                if (col_search.fsVALUE.IndexOf("~") > -1)
                                {
                                    TermRangeQuery trq = new TermRangeQuery("$" + col_search.fsCOLUMN, col_search.fsVALUE.Split('~')[0], col_search.fsVALUE.Split('~')[1], true, true);
                                    BooleanClause bc = new BooleanClause(trq, Occur.MUST);
                                    bq.Add(bc);
                                }
                                else
                                {
                                    BooleanClause bc = new BooleanClause(new TermQuery(new Term("$" + col_search.fsCOLUMN, col_search.fsVALUE)), Occur.MUST);
                                    bq.Add(bc);
                                }
                            }
                        }
                    }

                    //排序
                    //if (clsPARAMETER.lstCOLUMN_ORDER != null && clsPARAMETER.lstCOLUMN_ORDER.Count > 0)
                    //{
                    //    sortfield = new SortField[clsPARAMETER.lstCOLUMN_ORDER.Count];
                    //    for (int i = 0; i < clsPARAMETER.lstCOLUMN_ORDER.Count; i++)
                    //    {
                    //        if (clsPARAMETER.lstCOLUMN_ORDER[i].fsVALUE == "0")
                    //            sortfield[i] = new SortField("$" + clsPARAMETER.lstCOLUMN_ORDER[i].fsCOLUMN, SortField.STRING, false);
                    //        else
                    //            sortfield[i] = new SortField("$" + clsPARAMETER.lstCOLUMN_ORDER[i].fsCOLUMN, SortField.STRING, true);
                    //    }
                    //}

                    //索引庫
                    List<IndexReader> indexReader = new List<IndexReader>(); ;

                    foreach (var item in clsPARAMETER.fsINDEX.Split(','))
                    {
                        clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE, item);
                        string active_folder = indexinfo.GetActiveIndexFolder();
                        indexReader.Add(DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true));
                    }

                    MultiReader readers = new MultiReader(indexReader.ToArray());
                    //IndexReader indexReader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(active_folder)), true);
                    IndexSearcher indexSearcher = new IndexSearcher(readers);

                    //開始檢索
                    TopDocs result = indexSearcher.Search(bq, filter == null ? null : filter, readers.MaxDoc);

                    if (result != null)
                    {
                        var hits = result.ScoreDocs;

                        for (int i = 0; i < result.TotalHits; i++)
                        {
                            var documentFromSearcher = indexSearcher.Doc(hits[i].Doc);

                            if (lstGROUP != null && lstGROUP.FirstOrDefault(f => f.fsGROUP == documentFromSearcher.Get(clsPARAMETER.fsTREE)) == null)
                            {
                                lstGROUP.Add(new clsSEARCH_RESULT.clsGROUP()
                                {
                                    fsGROUP = documentFromSearcher.Get(clsPARAMETER.fsTREE)
                                });
                            }
                        }
                    }
                    else
                    {
                        readers.Dispose();
                        indexSearcher.Dispose();

                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "result is null" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                    }

                    readers.Dispose();
                    indexSearcher.Dispose();

                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = lstGROUP }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "請輸入要檢索的索引庫" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }


            }
            catch (Exception ex)
            {

                System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-SearchMeta：" + ex.Message + "\r\n");

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 取得同義詞
        /// </summary>
        /// <param name="clsSYNONYM">同義詞彙</param>
        /// <returns></returns>
        [Route("GetSynonym")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostGetSynonym(clsSYNONYM clsSYNONYM)
        {
            List<clsSYNONYM> lstSYNONYM = new List<clsSYNONYM>();


            try
            {
                clsINDEX_INFO synonyminfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE);
                string synonym_folder = synonyminfo.GetSynonymFolder();

                IndexReader IndexReader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(synonym_folder)), true);

                var synonymSearcher = new IndexSearcher(IndexReader);

                TopDocs resultDocs = synonymSearcher.Search(new TermQuery(new Term("fsWORD", clsSYNONYM.fsSYNONYM)), null, IndexReader.MaxDoc);

                if (resultDocs != null)
                {
                    var synonymhits = resultDocs.ScoreDocs;

                    for (int h = 0; h < resultDocs.TotalHits; h++)
                    {
                        var documentFromSearcher = synonymSearcher.Doc(synonymhits[h].Doc);
                        lstSYNONYM.Add(new clsSYNONYM() { fsSYNONYM = documentFromSearcher.Get("fsSYNONYM") });
                    }
                }

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = lstSYNONYM }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-GetSynonym：" + ex.Message + "\r\n");

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 重建同義詞
        /// </summary>
        /// <param name="lstSYNONYMs">所有同義詞組</param>
        /// <returns></returns>
        [Route("RebuildSynonym")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostRebuildSynonym(List<List<clsSYNONYM>> lstSYNONYMs)
        {

            try
            {
                //讀取XML檔案
                if (File.Exists(Properties.Settings.Default.fsSETTING_FILE))
                {
                    XmlDocument xmlDOC = new XmlDocument();
                    xmlDOC.Load(Properties.Settings.Default.fsSETTING_FILE);

                    if (xmlDOC != null && xmlDOC.SelectNodes("/Indexs/Synonym").Count > 0)
                    {
                        string fsNAME = xmlDOC.SelectNodes("/Indexs/Synonym")[0].SelectSingleNode("Name").InnerText;
                        string fsSYNONYM_DIR = xmlDOC.SelectNodes("/Indexs/Synonym")[0].SelectSingleNode("SynonymDir").InnerText;

                        //刪除目前同義詞索引庫資料夾
                        if (System.IO.Directory.Exists(fsSYNONYM_DIR + fsNAME))
                        {
                            RecycleApplicationPool();
                            System.Threading.Thread.Sleep(1000);
                            System.IO.Directory.Delete(fsSYNONYM_DIR + fsNAME, true);
                        }

                        //建立索引庫資料夾
                        System.IO.Directory.CreateDirectory(fsSYNONYM_DIR + fsNAME);


                        FSDirectory directory = FSDirectory.Open(new DirectoryInfo(fsSYNONYM_DIR + fsNAME));
                        Analyzer analyzer = new StandardAnalyzer(global::Lucene.Net.Util.Version.LUCENE_30);
                        var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

                        if (lstSYNONYMs != null && lstSYNONYMs.Count > 0)
                        {
                            //多個同義詞組
                            foreach (var item in lstSYNONYMs)
                            {
                                if (item != null && item.Count > 1)
                                {
                                    //每個詞組的同義詞要做雙向同義
                                    for (int i = 0; i <= item.Count - 1; i++)
                                    {
                                        for (int j = 0; j <= item.Count - 1; j++)
                                        {
                                            if (i != j)
                                            {
                                                var addElement = new Document();
                                                addElement.Add(new Field("fsWORD", item[i].fsSYNONYM, Field.Store.YES, Field.Index.NOT_ANALYZED));
                                                addElement.Add(new Field("fsSYNONYM", item[j].fsSYNONYM, Field.Store.YES, Field.Index.NOT_ANALYZED));
                                                writer.AddDocument(addElement);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        writer.Optimize();
                        writer.Commit();
                        writer.Dispose();
                        directory.Dispose();

                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                    }
                    else
                    {
                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "找不到同義詞資訊" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                    }
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "找不到同義詞資訊" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }

            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex, Message = ex.Message }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }

        }

        /// <summary>
        /// 新增同義詞
        /// </summary>
        /// <param name="lstSYNONYM">單一同義詞組</param>
        /// <returns></returns>
        [Route("InsertSynonym")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostInsertSynonym(List<clsSYNONYM> lstSYNONYM)
        {
            try
            {
                //讀取XML檔案
                if (File.Exists(Properties.Settings.Default.fsSETTING_FILE))
                {
                    XmlDocument xmlDOC = new XmlDocument();
                    xmlDOC.Load(Properties.Settings.Default.fsSETTING_FILE);

                    if (xmlDOC != null && xmlDOC.SelectNodes("/Indexs/Synonym").Count > 0)
                    {
                        string fsNAME = xmlDOC.SelectNodes("/Indexs/Synonym")[0].SelectSingleNode("Name").InnerText;
                        string fsSYNONYM_DIR = xmlDOC.SelectNodes("/Indexs/Synonym")[0].SelectSingleNode("SynonymDir").InnerText;

                        RecycleApplicationPool();

                        //建立索引庫資料夾
                        if (!System.IO.Directory.Exists(fsSYNONYM_DIR + fsNAME)) { }
                        System.IO.Directory.CreateDirectory(fsSYNONYM_DIR + fsNAME);

                        FSDirectory directory = FSDirectory.Open(new DirectoryInfo(fsSYNONYM_DIR + fsNAME));
                        Analyzer analyzer = new StandardAnalyzer(global::Lucene.Net.Util.Version.LUCENE_30);
                        var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

                        if (lstSYNONYM != null && lstSYNONYM.Count > 1)
                        {
                            for (int i = 0; i <= lstSYNONYM.Count - 1; i++)
                            {
                                //先刪除再建立
                                writer.DeleteDocuments(new Term("fsWORD", lstSYNONYM[i].fsSYNONYM));

                                for (int j = 0; j <= lstSYNONYM.Count - 1; j++)
                                {
                                    //同義詞為雙向關聯
                                    if (i != j)
                                    {
                                        var addElement = new Document();
                                        addElement.Add(new Field("fsWORD", lstSYNONYM[i].fsSYNONYM, Field.Store.YES, Field.Index.NOT_ANALYZED));
                                        addElement.Add(new Field("fsSYNONYM", lstSYNONYM[j].fsSYNONYM, Field.Store.YES, Field.Index.NOT_ANALYZED));
                                        writer.AddDocument(addElement);
                                    }
                                }
                            }
                        }

                        writer.Optimize();
                        writer.Dispose();
                        directory.Dispose();

                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                    }
                    else
                    {
                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "找不到同義詞資訊" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                    }
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "找不到同義詞資訊" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }


            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }

        /// <summary>
        /// 刪除同義詞
        /// </summary>
        /// <param name="lstSYNONYM">可為詞組或詞彙</param>
        /// <returns></returns>
        [Route("DeleteSynonym")]
        [ResponseType(typeof(clsRETURN))]
        public HttpResponseMessage PostDeleteSynonym(List<clsSYNONYM> lstSYNONYM)
        {
            try
            {
                //讀取XML檔案
                if (File.Exists(Properties.Settings.Default.fsSETTING_FILE))
                {
                    RecycleApplicationPool();

                    clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE);
                    string synonym_folder = indexinfo.GetSynonymFolder();

                    FSDirectory directory = FSDirectory.Open(new DirectoryInfo(synonym_folder));
                    Analyzer analyzer = new StandardAnalyzer(global::Lucene.Net.Util.Version.LUCENE_30);
                    var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

                    if (lstSYNONYM != null)
                    {
                        if (lstSYNONYM.Count == 1)
                        {
                            //刪除詞組
                            //先取得有哪些
                            IndexReader IndexReader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(synonym_folder)), true);
                            var indexSearcher = new IndexSearcher(IndexReader);
                            TopDocs resultDocs = indexSearcher.Search(new TermQuery(new Term("fsWORD", lstSYNONYM[0].fsSYNONYM)), null, IndexReader.MaxDoc);

                            if (resultDocs != null)
                            {
                                var hits = resultDocs.ScoreDocs;

                                for (int h = 0; h < resultDocs.TotalHits; h++)
                                {
                                    var documentFromSearcher = indexSearcher.Doc(hits[h].Doc);

                                    //由於同義詞定義是雙向的，所以要做兩次刪除

                                    BooleanQuery[] bq = new BooleanQuery[2];
                                    bq[0] = new BooleanQuery();
                                    bq[1] = new BooleanQuery();

                                    bq[0].Add(new TermQuery(new Term("fsWORD", lstSYNONYM[0].fsSYNONYM)), Occur.MUST);
                                    bq[0].Add(new TermQuery(new Term("fsSYNONYM", documentFromSearcher.Get("fsSYNONYM"))), Occur.MUST);

                                    writer.DeleteDocuments(bq[0]);

                                    bq[1].Add(new TermQuery(new Term("fsWORD", documentFromSearcher.Get("fsSYNONYM"))), Occur.MUST);
                                    bq[1].Add(new TermQuery(new Term("fsSYNONYM", lstSYNONYM[0].fsSYNONYM)), Occur.MUST);

                                    writer.DeleteDocuments(bq[1]);
                                }
                            }

                            IndexReader.Dispose();
                            indexSearcher.Dispose();
                        }
                        else if (lstSYNONYM.Count > 1)
                        {
                            for (int i = 0; i <= lstSYNONYM.Count - 1; i++)
                            {
                                for (int j = 0; j <= lstSYNONYM.Count - 1; j++)
                                {
                                    if (i != j)
                                    {
                                        BooleanQuery bq = new BooleanQuery();

                                        bq.Add(new TermQuery(new Term("fsWORD", lstSYNONYM[i].fsSYNONYM)), Occur.MUST);
                                        bq.Add(new TermQuery(new Term("fsSYNONYM", lstSYNONYM[j].fsSYNONYM)), Occur.MUST);

                                        writer.DeleteDocuments(bq);
                                    }
                                }
                            }
                        }
                    }

                    writer.Optimize();
                    writer.Dispose();
                    directory.Dispose();

                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "找不到同義詞資訊" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                }


            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
        }



        //============以下是共用function==================//
        /// <summary>
        /// 取代文字
        /// </summary>
        /// <param name="content">被取代文字</param>
        /// <returns></returns>
        private string ReplaceWord(string content)
        {
            XmlDocument xmlDOC = new XmlDocument();
            xmlDOC.Load(Properties.Settings.Default.fsSETTING_FILE);
            //取得取代字元
            string[] lstREPLACE_WORD = File.ReadAllLines(xmlDOC.SelectNodes("/Indexs/Setting")[0].SelectSingleNode("ReplaceWordFile").InnerText);

            string output = content;

            if (lstREPLACE_WORD != null && lstREPLACE_WORD.Count() > 0)
            {
                foreach (var item in lstREPLACE_WORD)
                {
                    output = output.Replace(item.Split('|')[0], item.Split('|')[1]);
                }
            }

            return output;
        }

        /// <summary>
        /// 反取代文字
        /// </summary>
        /// <param name="content">被取代文字</param>
        /// <returns></returns>
        private string ReverseWord(string content)
        {
            XmlDocument xmlDOC = new XmlDocument();
            xmlDOC.Load(Properties.Settings.Default.fsSETTING_FILE);
            //取得取代字元
            string[] lstREPLACE_WORD = File.ReadAllLines(xmlDOC.SelectNodes("/Indexs/Setting")[0].SelectSingleNode("ReplaceWordFile").InnerText);

            string output = content;

            if (lstREPLACE_WORD != null && lstREPLACE_WORD.Count() > 0)
            {
                foreach (var item in lstREPLACE_WORD)
                {
                    output = output.Replace(item.Split('|')[1], item.Split('|')[0]);
                }
            }

            return output;
        }

        /// <summary>
        /// 取得關鍵字前後文
        /// </summary>
        /// <param name="content">全文</param>
        /// <returns></returns>
        private string GetContent(string content)
        {
            string after_content = string.Empty;

            after_content = (content.Length > 70 ? content.Substring(0, 70) : content.Substring(0, content.Length - 1));

            return after_content;
        }

        /// <summary>
        /// 取得關鍵字前後文
        /// </summary>
        /// <param name="content">全文</param>
        /// <param name="keyword">關鍵字</param>
        /// <returns></returns>
        private string GetContent(string content, string keyword)
        {
            string after_content = string.Empty;

            if (content.IndexOf(keyword) > -1)
            {
                //關鍵字前面有多少字，最多取30個字
                int before_keyword_len = content.IndexOf(keyword);
                if (before_keyword_len >= 40) before_keyword_len = before_keyword_len - 40; else before_keyword_len = 0;

                //關鍵字後面有多少字，最多取30個字
                int after_keyword_len = content.Length - content.IndexOf(keyword) - keyword.Length;
                if (after_keyword_len >= 40) after_keyword_len = 40;

                after_content = content.Substring(before_keyword_len, (content.IndexOf(keyword) - before_keyword_len) + after_keyword_len + keyword.Length);

                //after_content = after_content.Substring(0, before_keyword_len + after_keyword_len + keyword.Length - 1);
            }
            else
            {
                after_content = (content.Length > 80 ? content.Substring(0, 80) : content.Substring(0, content.Length - 1));
            }

            return after_content;
        }

        /// <summary>
        /// 取得同義詞組合後的關鍵字
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="homophone">是否啟動同音</param>
        /// <returns></returns>
        private string GetSynonymKeyword(string keyword, int homophone = 0)
        {
            string after_synonym_keyword = keyword;

            string[] lstKEYWORD = keyword.Replace(" ", "").Replace("AND", "|").Replace("OR", "|").Replace("NOT", "|").Split('|');

            clsINDEX_INFO indexinfo = new clsINDEX_INFO(Properties.Settings.Default.fsSETTING_FILE);
            string synonym_folder = indexinfo.GetSynonymFolder();

            IndexReader IndexReader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(synonym_folder)), true);

            var indexSearcher = new IndexSearcher(IndexReader);

            //所有同義詞陣列組合成關建詞
            foreach (var item in lstKEYWORD)
            {
                TopDocs resultDocs = indexSearcher.Search(new TermQuery(new Term("fsWORD", item)), null, IndexReader.MaxDoc);
                //同義詞暫存陣列宣告
                List<string> lstKEYWORD_TEMP = new List<string>();
                if (homophone == 1) lstKEYWORD_TEMP.Add(GetHomophone(item)); else lstKEYWORD_TEMP.Add(item);

                if (resultDocs != null)
                {
                    var hits = resultDocs.ScoreDocs;

                    //把同義詞放入暫存陣列，並看條件是否需轉成同音
                    for (int h = 0; h < resultDocs.TotalHits; h++)
                    {
                        var documentFromSearcher = indexSearcher.Doc(hits[h].Doc);
                        //把所同義詞轉成同音字
                        if (homophone == 1) lstKEYWORD_TEMP.Add(GetHomophone(documentFromSearcher.Get("fsSYNONYM"))); else lstKEYWORD_TEMP.Add(documentFromSearcher.Get("fsSYNONYM"));
                    }
                }
                //從暫存陣列中組合還原要查詢的關鍵字組合(A1 OR A1_1 OR A1_2)
                after_synonym_keyword = after_synonym_keyword.Replace(item, "(" + string.Join(" OR ", lstKEYWORD_TEMP) + ")");
            }

            IndexReader.Dispose();
            indexSearcher.Dispose();

            return after_synonym_keyword;
        }

        /// <summary>
        /// 取得關鍵字轉成注音
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <returns></returns>
        private string GetHomophone(string keyword)
        {
            string after_homophone_keyword = keyword;
            string[] lstKEYWORD = keyword.Replace(" ", "").Replace("AND", "|").Replace("OR", "|").Replace("NOT", "|").Split('|');

            foreach (var item in lstKEYWORD)
            {
                //只剩下中文的部分
                string keyword_after_replace = System.Text.RegularExpressions.Regex.Replace(item, @"[^\u4e00-\u9fa5]", "");

                // 轉成注音部分
                List<string> lstHOMOPHONE = new List<string>();
                //每個字轉換，空白就是轉不過去，直接忽略
                char[] word = keyword_after_replace.ToCharArray();
                for (int i = 0; i < word.Length; i++)
                {
                    lstHOMOPHONE.Add(new ZhuyinReverseConversionProvider().Convert(word.ElementAt(i).ToString()).ElementAt(0));
                }

                ////轉成注音部分
                //string[] lstHOMOPHONE = new ZhuyinReverseConversionProvider().Convert(keyword_after_replace);

                if (lstHOMOPHONE != null && lstHOMOPHONE.Count() > 0)
                {
                    //每個中文字做取代
                    for (int h = 0; h < lstHOMOPHONE.Count(); h++)
                    {
                        if (lstHOMOPHONE[h].ToCharArray().Last() == 'ˊ' || lstHOMOPHONE[h].ToCharArray().Last() == 'ˇ' || lstHOMOPHONE[h].ToCharArray().Last() == 'ˋ' || lstHOMOPHONE[h].ToCharArray().Last() == '˙')
                        {
                            after_homophone_keyword = after_homophone_keyword.Replace(keyword_after_replace.ToCharArray()[h].ToString(), lstHOMOPHONE[h].Replace("ˊ", "二").Replace("ˇ", "三").Replace("ˋ", "四").Replace("˙", "零"));
                        }
                        else
                        {
                            after_homophone_keyword = after_homophone_keyword.Replace(keyword_after_replace.ToCharArray()[h].ToString(), lstHOMOPHONE[h] + "一");
                        }
                    }
                }
            }

            return after_homophone_keyword;
        }

        /// <summary>
        /// 回收檢索的應用程式集區
        /// </summary>
        private void RecycleApplicationPool()
        {
            Process p = new Process();
            try
            {
                p.StartInfo.FileName = @"C:\Windows\System32\inetsrv\appcmd.exe";
                p.StartInfo.Arguments = @"AIRMAM5.Search.Lucene";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;

                p.Start();

            }
            catch (Exception)
            {

                throw;
            }

        }
        //================================================//
    }


}
