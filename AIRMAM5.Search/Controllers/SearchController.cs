using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AIRMAM5.Search.Models;
using TS50SDK;

namespace AIRMAM5.Search.Controllers
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

            Ts5Core objTS5Core = new Ts5Core();
            TS50SDK.SearchService objSearchService = null;
            TS50SDK.ResultList objResultList = null;
            TS50SDK.SimpleCriteria objSimpleCriteria = new TS50SDK.SimpleCriteria();
            TS50SDK.FieldFilter objFieldFilter = new TS50SDK.FieldFilter();
            TS50SDK.CategoryTree tree = null;
            string fsEXPRESSION = string.Empty;
            
            try
            {
                if (!string.IsNullOrEmpty(clsPARAMETER.fsINDEX))
                {
                    //索引庫
                    objSimpleCriteria.Add(SearchCriteria.CRITERIA_DATA_SOURCE, clsPARAMETER.fsINDEX);

                    //關鍵字
                    if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD))
                    {
                        objSimpleCriteria.Add(SearchCriteria.CRITERIA_KEYWORD, clsPARAMETER.fsKEYWORD);
                    }

                    //不分全型半型
                    objSimpleCriteria.Add(SearchCriteria.CRITERIA_FORM_CONVERT, true);

                    //日期
                    if (clsPARAMETER.clsDATE != null && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fsCOLUMN) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdSDATE) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdEDATE))
                    {
                        fsEXPRESSION += objFieldFilter.SetCondition("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE + "~" + clsPARAMETER.clsDATE.fdEDATE) + "&";
                    }

                    //可查詢節點權限
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
                        {
                            fsEXPRESSION += objFieldFilter.SetCondition("$fsAUTHORUTY_DIR_ID", clsPARAMETER.fsAUTH_DIR) + "&";
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
                            fsEXPRESSION += objFieldFilter.SetCondition("$fnFILE_SECRET", clsPARAMETER.fsSECRET) + "&";
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限", Data = null }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //樣板編號
                    if (clsPARAMETER.fnTEMP_ID > 0)
                    {
                        fsEXPRESSION += objFieldFilter.SetCondition("$fnTEMP_ID", clsPARAMETER.fnTEMP_ID.ToString()) + "&";
                    }

                    //欄位檢索
                    if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
                    {
                        foreach (var item in clsPARAMETER.lstCOLUMN_SEARCH)
                        {
                            if (item.fbIS_FULLTEXT)
                                fsEXPRESSION += objFieldFilter.SetCondition(item.fsCOLUMN, item.fsVALUE) + "&";
                            else
                                fsEXPRESSION += objFieldFilter.SetCondition("$" + item.fsCOLUMN, item.fsVALUE) + "&";
                        }
                    }

                    //條件加入Filter項目
                    if (!string.IsNullOrEmpty(fsEXPRESSION))
                    {
                        fsEXPRESSION = fsEXPRESSION.Substring(0, fsEXPRESSION.Length - 1);
                        objFieldFilter.SetExpression(fsEXPRESSION);
                        objSimpleCriteria.Add(SearchCriteria.CRITERIA_FILTER, objFieldFilter);
                    }

                    //同音
                    if (clsPARAMETER.fnHOMO == 1)
                    {
                        objSimpleCriteria.Add(SearchCriteria.CRITERIA_HOMOPHONE, true);
                    }

                    //Refiner
                    if (!string.IsNullOrEmpty(clsPARAMETER.fsTREE))
                    {
                        objSimpleCriteria.Add(SearchCriteria.CRITERIA_CATEGORY, clsPARAMETER.fsTREE);
                    }

                    //檢索方式
                    objSimpleCriteria.Add(SearchCriteria.CRITERIA_SEARCH_MODE, clsPARAMETER.fnSEARCH_MODE);

                    objTS5Core.Initialize(Properties.Settings.Default.fsTS50);
                    objSearchService = objTS5Core.GetSearchService();
                    objResultList = objSearchService.SearchBySimpleCriteria(objSimpleCriteria, clsPARAMETER.fnSTART_INDEX, 1);

                    tree = objResultList.GetCategoryTree();

                    if (tree != null)
                    {
                        //subject count
                        clsCOUNT.fnSUBJECT_COUNT = (tree.GetNodeByPath("/MAM/S") == null ? 0 : tree.GetNodeByPath("/MAM/S").GetDocumentNum());
                        //video count
                        clsCOUNT.fnVIDEO_COUNT = (tree.GetNodeByPath("/MAM/V") == null ? 0 : tree.GetNodeByPath("/MAM/V").GetDocumentNum());
                        //audio count
                        clsCOUNT.fnAUDIO_COUNT = (tree.GetNodeByPath("/MAM/A") == null ? 0 : tree.GetNodeByPath("/MAM/A").GetDocumentNum());
                        //photo count
                        clsCOUNT.fnPHOTO_COUNT = (tree.GetNodeByPath("/MAM/P") == null ? 0 : tree.GetNodeByPath("/MAM/P").GetDocumentNum());
                        //doc count
                        clsCOUNT.fnDOC_COUNT = (tree.GetNodeByPath("/MAM/D") == null ? 0 : tree.GetNodeByPath("/MAM/D").GetDocumentNum());
                    }
                    else
                    {
                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "tree is null" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
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
            finally
            {
                objTS5Core = null;
                objSearchService = null;
                objResultList = null;
                tree = null;
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

            Ts5Core objTS5Core = new Ts5Core();
            TS50SDK.SearchService objSearchService = null;
            TS50SDK.ResultList objResultList = null;
            TS50SDK.SimpleCriteria objSimpleCriteria = new TS50SDK.SimpleCriteria();
            TS50SDK.FieldFilter objFieldFilter = new TS50SDK.FieldFilter();
            Order objOrder = new Order();
            string fsEXPRESSION = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(clsPARAMETER.fsINDEX))
                {
                    //索引庫
                    objSimpleCriteria.Add(SearchCriteria.CRITERIA_DATA_SOURCE, clsPARAMETER.fsINDEX);

                    //關鍵字
                    if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD))
                    {
                        objSimpleCriteria.Add(SearchCriteria.CRITERIA_KEYWORD, clsPARAMETER.fsKEYWORD);
                    }

                    //不分全型半型
                    objSimpleCriteria.Add(SearchCriteria.CRITERIA_FORM_CONVERT, true);

                    //日期
                    if (clsPARAMETER.clsDATE != null && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fsCOLUMN) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdSDATE) && !string.IsNullOrEmpty(clsPARAMETER.clsDATE.fdEDATE))
                    {
                        fsEXPRESSION += objFieldFilter.SetCondition("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE + "~" + clsPARAMETER.clsDATE.fdEDATE) + "&";
                    }

                    //可查詢節點權限
                    if (!clsPARAMETER.fbIS_ADMIN)
                    {
                        if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
                        {
                            fsEXPRESSION += objFieldFilter.SetCondition("$fsAUTHORUTY_DIR_ID", clsPARAMETER.fsAUTH_DIR) + "&";
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
                            fsEXPRESSION += objFieldFilter.SetCondition("$fnFILE_SECRET", clsPARAMETER.fsSECRET) + "&";
                        }
                        else
                        {
                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = "無檢索權限" }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                        }
                    }

                    //樣板編號
                    if (clsPARAMETER.fnTEMP_ID > 0)
                    {
                        fsEXPRESSION += objFieldFilter.SetCondition("$fnTEMP_ID", clsPARAMETER.fnTEMP_ID.ToString()) + "&";
                    }

                    //欄位檢索
                    if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
                    {
                        foreach (var item in clsPARAMETER.lstCOLUMN_SEARCH)
                        {
                            if (item.fbIS_FULLTEXT)
                                fsEXPRESSION += objFieldFilter.SetCondition(item.fsCOLUMN, item.fsVALUE) + "&";
                            else
                                fsEXPRESSION += objFieldFilter.SetCondition("$" + item.fsCOLUMN, item.fsVALUE) + "&";
                        }
                    }

                    //條件加入Filter項目
                    if (!string.IsNullOrEmpty(fsEXPRESSION))
                    {
                        fsEXPRESSION = fsEXPRESSION.Substring(0, fsEXPRESSION.Length - 1);
                        objFieldFilter.SetExpression(fsEXPRESSION);
                        objSimpleCriteria.Add(SearchCriteria.CRITERIA_FILTER, objFieldFilter);
                    }

                    //同音
                    if (clsPARAMETER.fnHOMO == 1)
                    {
                        objSimpleCriteria.Add(SearchCriteria.CRITERIA_HOMOPHONE, true);
                    }

                    //Refiner
                    if (!string.IsNullOrEmpty(clsPARAMETER.fsTREE))
                    {
                        objSimpleCriteria.Add(SearchCriteria.CRITERIA_CATEGORY, clsPARAMETER.fsTREE);
                    }

                    //排序
                    if (clsPARAMETER.lstCOLUMN_ORDER != null && clsPARAMETER.lstCOLUMN_ORDER.Count > 0)
                    {
                        foreach (var item in clsPARAMETER.lstCOLUMN_ORDER)
                        {
                            if (item.fsVALUE == "0")
                                objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_ASC);
                            else if (item.fsVALUE == "1")
                                objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_DES);
                        }
                        objSimpleCriteria.Add(SearchCriteria.CRITERIA_ORDER, objOrder);
                    }

                    //檢索方式
                    objSimpleCriteria.Add(SearchCriteria.CRITERIA_SEARCH_MODE, clsPARAMETER.fnSEARCH_MODE);

                    objTS5Core.Initialize(Properties.Settings.Default.fsTS50);
                    objSearchService = objTS5Core.GetSearchService();
                    objSearchService.SetResultMark("<<", ">>");
                    objResultList = objSearchService.SearchBySimpleCriteria(objSimpleCriteria, clsPARAMETER.fnSTART_INDEX, clsPARAMETER.fnPAGE_SIZE);

                    if (objResultList != null && objResultList.GetSize() > 0)
                    {
                        for (int i = 0; i < objResultList.GetSize(); i++)
                        {
                            lstARC.Add(new clsSEARCH_RESULT.clsARC
                            {
                                fsFILE_NO = objResultList.GetItem(i).GetFieldByName("fsFILE_NO").ToString(),
                                fsMATCH = objResultList.GetItem(i).GetContent()
                            });
                        }
                    }

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
            finally
            {
                objTS5Core = null;
                objSearchService = null;
                objResultList = null;
            }
        }

        ///// <summary>
        ///// 檢索主題
        ///// </summary>
        ///// <param name="clsPARAMETER">檢索參數</param>
        ///// <returns></returns>
        //[Route("SearchSubject")]
        //[Obsolete("已過時，請改用SearchMeta")]
        //public IHttpActionResult PostSearchSubject(clsPARAMETER clsPARAMETER)
        //{
        //    clsSEARCH_RESULT.clsSUBJ clsSUBJ = new clsSEARCH_RESULT.clsSUBJ();
        //    clsSUBJ.lstSUBJ = new List<clsSEARCH_RESULT.clsSUBJ.SUBJ>();

        //    Ts5Core objTS5Core = new Ts5Core();
        //    TS50SDK.SearchService objSearchService = null;
        //    TS50SDK.ResultList objResultList = null;
        //    TS50SDK.SimpleCriteria objSimpleCriteria = new TS50SDK.SimpleCriteria();
        //    TS50SDK.FieldFilter objFieldFilter = new TS50SDK.FieldFilter();
        //    Order objOrder = new Order();
        //    string fsEXPRESSION = string.Empty;

        //    try
        //    {
        //        //索引庫
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_DATA_SOURCE, clsPARAMETER.fsINDEX);

        //        //關鍵字
        //        if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD))
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_KEYWORD, clsPARAMETER.fsKEYWORD);
        //        }

        //        //不分全型半型
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_FORM_CONVERT, true);

        //        //日期
        //        if (clsPARAMETER.clsDATE != null)
        //        {
        //            fsEXPRESSION += objFieldFilter.SetCondition("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE + "~" + clsPARAMETER.clsDATE.fdEDATE) + "&";
        //        }

        //        //可查詢節點權限
        //        if (!clsPARAMETER.fbIS_ADMIN)
        //        {
        //            if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
        //            {
        //                fsEXPRESSION += objFieldFilter.SetCondition("$fsAUTHORUTY_DIR_ID", clsPARAMETER.fsAUTH_DIR) + "&";
        //            }
        //            else
        //            {
        //                clsSUBJ.fsRESULT = "error";
        //                clsSUBJ.fsMESSAGE = "無檢索權限";
        //                return Ok(clsSUBJ);
        //            }
        //        }

        //        //欄位檢索
        //        if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
        //        {
        //            foreach (var item in clsPARAMETER.lstCOLUMN_SEARCH)
        //            {
        //                if (item.fbIS_FULLTEXT)
        //                    fsEXPRESSION += objFieldFilter.SetCondition(item.fsCOLUMN, item.fsVALUE) + "&";
        //                else
        //                    fsEXPRESSION += objFieldFilter.SetCondition("$" + item.fsCOLUMN, item.fsVALUE) + "&";
        //            }
        //        }

        //        //條件加入Filter項目
        //        if (!string.IsNullOrEmpty(fsEXPRESSION))
        //        {
        //            fsEXPRESSION = fsEXPRESSION.Substring(0, fsEXPRESSION.Length - 1);
        //            objFieldFilter.SetExpression(fsEXPRESSION);
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_FILTER, objFieldFilter);
        //        }

        //        //同音
        //        if (clsPARAMETER.fnHOMO == 1)
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_HOMOPHONE, true);
        //        }

        //        //Refiner
        //        if (!string.IsNullOrEmpty(clsPARAMETER.fsTREE))
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_CATEGORY, clsPARAMETER.fsTREE);
        //        }

        //        //排序
        //        if (clsPARAMETER.lstCOLUMN_ORDER != null && clsPARAMETER.lstCOLUMN_ORDER.Count > 0)
        //        {
        //            foreach (var item in clsPARAMETER.lstCOLUMN_ORDER)
        //            {
        //                if (item.fsVALUE == "0")
        //                    objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_ASC);
        //                else if (item.fsVALUE == "1")
        //                    objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_DES);
        //            }
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_ORDER, objOrder);
        //        }

        //        //檢索方式
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_SEARCH_MODE, clsPARAMETER.fnSEARCH_MODE);

        //        objTS5Core.Initialize(Properties.Settings.Default.fsTS50);
        //        objSearchService = objTS5Core.GetSearchService();
        //        objResultList = objSearchService.SearchBySimpleCriteria(objSimpleCriteria, clsPARAMETER.fnSTART_INDEX, clsPARAMETER.fnPAGE_SIZE);

        //        if (objResultList != null && objResultList.GetSize() > 0)
        //        {
        //            for (int i = 0; i < objResultList.GetSize(); i++)
        //            {
        //                clsSUBJ.lstSUBJ.Add(new clsSEARCH_RESULT.clsSUBJ.SUBJ()
        //                {
        //                    fsSUBJ_ID = objResultList.GetItem(i).GetFieldByName("fsSUBJ_ID").ToString(),
        //                    //fsTITLE = objResultList.GetItem(i).GetFieldByName("fsTITLE").ToString(),
        //                    //fsDESCRIPTION = objResultList.GetItem(i).GetFieldByName("fsDESCRIPTION").ToString(),
        //                    fsMATCH = objResultList.GetItem(i).GetContent()
        //                });
        //            }
        //        }

        //        clsSUBJ.fsRESULT = "success";
        //        clsSUBJ.fsMESSAGE = string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsSUBJ.fsRESULT = "error";
        //        clsSUBJ.fsMESSAGE = ex.Message;

        //        System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-SearchCount：" + ex.Message + "\r\n");
        //    }
        //    finally
        //    {
        //        objTS5Core = null;
        //        objSearchService = null;
        //        objResultList = null;
        //    }

        //    return Ok(clsSUBJ);
        //}

        ///// <summary>
        ///// 檢索影片
        ///// </summary>
        ///// <param name="clsPARAMETER">檢索參數</param>
        ///// <returns></returns>
        //[Route("SearchVideo")]
        //[Obsolete("已過時，請改用SearchMeta")]
        //public IHttpActionResult PostSearchVideo(clsPARAMETER clsPARAMETER)
        //{
        //    clsSEARCH_RESULT.clsVIDEO clsVIDEO = new clsSEARCH_RESULT.clsVIDEO();
        //    clsVIDEO.lstVIDEO = new List<clsSEARCH_RESULT.clsVIDEO.VIDEO>();

        //    Ts5Core objTS5Core = new Ts5Core();
        //    TS50SDK.SearchService objSearchService = null;
        //    TS50SDK.ResultList objResultList = null;
        //    TS50SDK.SimpleCriteria objSimpleCriteria = new TS50SDK.SimpleCriteria();
        //    TS50SDK.FieldFilter objFieldFilter = new TS50SDK.FieldFilter();
        //    Order objOrder = new Order();
        //    string fsEXPRESSION = string.Empty;

        //    try
        //    {
        //        //索引庫
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_DATA_SOURCE, clsPARAMETER.fsINDEX);

        //        //關鍵字
        //        if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD))
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_KEYWORD, clsPARAMETER.fsKEYWORD);
        //        }

        //        //不分全型半型
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_FORM_CONVERT, true);

        //        //日期
        //        if (clsPARAMETER.clsDATE != null)
        //        {
        //            fsEXPRESSION += objFieldFilter.SetCondition("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE + "~" + clsPARAMETER.clsDATE.fdEDATE) + "&";
        //        }

        //        //可查詢節點權限
        //        if (!clsPARAMETER.fbIS_ADMIN)
        //        {
        //            if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
        //            {
        //                fsEXPRESSION += objFieldFilter.SetCondition("$fsAUTHORUTY_DIR_ID", clsPARAMETER.fsAUTH_DIR) + "&";
        //            }
        //            else
        //            {
        //                clsVIDEO.fsRESULT = "error";
        //                clsVIDEO.fsMESSAGE = "無檢索權限";
        //                return Ok(clsVIDEO);
        //            }
        //        }

        //        //可查詢機密等級
        //        if (!clsPARAMETER.fbIS_ADMIN)
        //        {
        //            if (!string.IsNullOrEmpty(clsPARAMETER.fsSECRET))
        //            {
        //                fsEXPRESSION += objFieldFilter.SetCondition("$fnFILE_SECRET", clsPARAMETER.fsSECRET) + "&";
        //            }
        //            else
        //            {
        //                clsVIDEO.fsRESULT = "error";
        //                clsVIDEO.fsMESSAGE = "無檢索權限";
        //                return Ok(clsVIDEO);
        //            }
        //        }

        //        //樣板編號
        //        if (clsPARAMETER.fnTEMP_ID > 0)
        //        {
        //            fsEXPRESSION += objFieldFilter.SetCondition("$fnTEMP_ID", clsPARAMETER.fnTEMP_ID.ToString()) + "&";
        //        }

        //        //欄位檢索
        //        if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
        //        {
        //            foreach (var item in clsPARAMETER.lstCOLUMN_SEARCH)
        //            {
        //                if (item.fbIS_FULLTEXT)
        //                    fsEXPRESSION += objFieldFilter.SetCondition(item.fsCOLUMN, item.fsVALUE) + "&";
        //                else
        //                    fsEXPRESSION += objFieldFilter.SetCondition("$" + item.fsCOLUMN, item.fsVALUE) + "&";
        //            }
        //        }

        //        //條件加入Filter項目
        //        if (!string.IsNullOrEmpty(fsEXPRESSION))
        //        {
        //            fsEXPRESSION = fsEXPRESSION.Substring(0, fsEXPRESSION.Length - 1);
        //            objFieldFilter.SetExpression(fsEXPRESSION);
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_FILTER, objFieldFilter);
        //        }

        //        //同音
        //        if (clsPARAMETER.fnHOMO == 1)
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_HOMOPHONE, true);
        //        }

        //        //Refiner
        //        if (!string.IsNullOrEmpty(clsPARAMETER.fsTREE))
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_CATEGORY, clsPARAMETER.fsTREE);
        //        }

        //        //排序
        //        if (clsPARAMETER.lstCOLUMN_ORDER != null && clsPARAMETER.lstCOLUMN_ORDER.Count > 0)
        //        {
        //            foreach (var item in clsPARAMETER.lstCOLUMN_ORDER)
        //            {
        //                if (item.fsVALUE == "0")
        //                    objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_ASC);
        //                else if (item.fsVALUE == "1")
        //                    objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_DES);
        //            }
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_ORDER, objOrder);
        //        }

        //        //檢索方式
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_SEARCH_MODE, clsPARAMETER.fnSEARCH_MODE);

        //        objTS5Core.Initialize(Properties.Settings.Default.fsTS50);
        //        objSearchService = objTS5Core.GetSearchService();
        //        objSearchService.SetResultMark("<<", ">>");
        //        objResultList = objSearchService.SearchBySimpleCriteria(objSimpleCriteria, clsPARAMETER.fnSTART_INDEX, clsPARAMETER.fnPAGE_SIZE);

        //        if (objResultList != null && objResultList.GetSize() > 0)
        //        {
        //            for (int i = 0; i < objResultList.GetSize(); i++)
        //            {
        //                clsVIDEO.lstVIDEO.Add(new clsSEARCH_RESULT.clsVIDEO.VIDEO()
        //                {
        //                    fsFILE_NO = objResultList.GetItem(i).GetFieldByName("fsFILE_NO").ToString(),
        //                    //fsTITLE = objResultList.GetItem(i).GetFieldByName("fsTITLE").ToString(),
        //                    //fsDESCRIPTION = objResultList.GetItem(i).GetFieldByName("fsDESCRIPTION").ToString(),
        //                    fsMATCH = objResultList.GetItem(i).GetContent(),
        //                    //fsHEAD_FRAME = objResultList.GetItem(i).GetFieldByName("fsHEAD_FRAME").ToString()
        //                });
        //            }
        //        }

        //        clsVIDEO.fsRESULT = "success";
        //        clsVIDEO.fsMESSAGE = string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsVIDEO.fsRESULT = "error";
        //        clsVIDEO.fsMESSAGE = ex.Message;

        //        System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-SearchCount：" + ex.Message + "\r\n");
        //    }
        //    finally
        //    {
        //        objTS5Core = null;
        //        objSearchService = null;
        //        objResultList = null;
        //    }

        //    return Ok(clsVIDEO);
        //}

        ///// <summary>
        ///// 檢索聲音
        ///// </summary>
        ///// <param name="clsPARAMETER">檢索參數</param>
        ///// <returns></returns>
        //[Route("SearchAudio")]
        //[Obsolete("已過時，請改用SearchMeta")]
        //public IHttpActionResult PostSearchAudio(clsPARAMETER clsPARAMETER)
        //{
        //    clsSEARCH_RESULT.clsAUDIO clsAUDIO = new clsSEARCH_RESULT.clsAUDIO();
        //    clsAUDIO.lstAUDIO = new List<clsSEARCH_RESULT.clsAUDIO.AUDIO>();

        //    Ts5Core objTS5Core = new Ts5Core();
        //    TS50SDK.SearchService objSearchService = null;
        //    TS50SDK.ResultList objResultList = null;
        //    TS50SDK.SimpleCriteria objSimpleCriteria = new TS50SDK.SimpleCriteria();
        //    TS50SDK.FieldFilter objFieldFilter = new TS50SDK.FieldFilter();
        //    Order objOrder = new Order();
        //    string fsEXPRESSION = string.Empty;

        //    try
        //    {
        //        //索引庫
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_DATA_SOURCE, clsPARAMETER.fsINDEX);

        //        //關鍵字
        //        if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD))
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_KEYWORD, clsPARAMETER.fsKEYWORD);
        //        }

        //        //不分全型半型
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_FORM_CONVERT, true);

        //        //日期
        //        if (clsPARAMETER.clsDATE != null)
        //        {
        //            fsEXPRESSION += objFieldFilter.SetCondition("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE + "~" + clsPARAMETER.clsDATE.fdEDATE) + "&";
        //        }

        //        //可查詢節點權限
        //        if (!clsPARAMETER.fbIS_ADMIN)
        //        {
        //            if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
        //            {
        //                fsEXPRESSION += objFieldFilter.SetCondition("$fsAUTHORUTY_DIR_ID", clsPARAMETER.fsAUTH_DIR) + "&";
        //            }
        //            else
        //            {
        //                clsAUDIO.fsRESULT = "error";
        //                clsAUDIO.fsMESSAGE = "無檢索權限";
        //                return Ok(clsAUDIO);
        //            }
        //        }

        //        //可查詢機密等級
        //        if (!clsPARAMETER.fbIS_ADMIN)
        //        {
        //            if (!string.IsNullOrEmpty(clsPARAMETER.fsSECRET))
        //            {
        //                fsEXPRESSION += objFieldFilter.SetCondition("$fnFILE_SECRET", clsPARAMETER.fsSECRET) + "&";
        //            }
        //        }

        //        //樣板編號
        //        if (clsPARAMETER.fnTEMP_ID > 0)
        //        {
        //            fsEXPRESSION += objFieldFilter.SetCondition("$fnTEMP_ID", clsPARAMETER.fnTEMP_ID.ToString()) + "&";
        //        }

        //        //欄位檢索
        //        if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
        //        {
        //            foreach (var item in clsPARAMETER.lstCOLUMN_SEARCH)
        //            {
        //                if (item.fbIS_FULLTEXT)
        //                    fsEXPRESSION += objFieldFilter.SetCondition(item.fsCOLUMN, item.fsVALUE) + "&";
        //                else
        //                    fsEXPRESSION += objFieldFilter.SetCondition("$" + item.fsCOLUMN, item.fsVALUE) + "&";
        //            }
        //        }

        //        //條件加入Filter項目
        //        if (!string.IsNullOrEmpty(fsEXPRESSION))
        //        {
        //            fsEXPRESSION = fsEXPRESSION.Substring(0, fsEXPRESSION.Length - 1);
        //            objFieldFilter.SetExpression(fsEXPRESSION);
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_FILTER, objFieldFilter);
        //        }

        //        //同音
        //        if (clsPARAMETER.fnHOMO == 1)
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_HOMOPHONE, true);
        //        }

        //        //Refiner
        //        if (!string.IsNullOrEmpty(clsPARAMETER.fsTREE))
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_CATEGORY, clsPARAMETER.fsTREE);
        //        }

        //        //排序
        //        if (clsPARAMETER.lstCOLUMN_ORDER != null && clsPARAMETER.lstCOLUMN_ORDER.Count > 0)
        //        {
        //            foreach (var item in clsPARAMETER.lstCOLUMN_ORDER)
        //            {
        //                if (item.fsVALUE == "0")
        //                    objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_ASC);
        //                else if (item.fsVALUE == "1")
        //                    objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_DES);
        //            }
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_ORDER, objOrder);
        //        }

        //        //檢索方式
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_SEARCH_MODE, clsPARAMETER.fnSEARCH_MODE);

        //        objTS5Core.Initialize(Properties.Settings.Default.fsTS50);
        //        objSearchService = objTS5Core.GetSearchService();
        //        objSearchService.SetResultMark("<<", ">>");
        //        objResultList = objSearchService.SearchBySimpleCriteria(objSimpleCriteria, clsPARAMETER.fnSTART_INDEX, clsPARAMETER.fnPAGE_SIZE);

        //        if (objResultList != null && objResultList.GetSize() > 0)
        //        {
        //            for (int i = 0; i < objResultList.GetSize(); i++)
        //            {
        //                clsAUDIO.lstAUDIO.Add(new clsSEARCH_RESULT.clsAUDIO.AUDIO()
        //                {
        //                    fsFILE_NO = objResultList.GetItem(i).GetFieldByName("fsFILE_NO").ToString(),
        //                    //fsTITLE = objResultList.GetItem(i).GetFieldByName("fsTITLE").ToString(),
        //                    //fsDESCRIPTION = objResultList.GetItem(i).GetFieldByName("fsDESCRIPTION").ToString(),
        //                    fsMATCH = objResultList.GetItem(i).GetContent()
        //                });
        //            }
        //        }

        //        clsAUDIO.fsRESULT = "success";
        //        clsAUDIO.fsMESSAGE = string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsAUDIO.fsRESULT = "error";
        //        clsAUDIO.fsMESSAGE = ex.Message;

        //        System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-SearchCount：" + ex.Message + "\r\n");
        //    }
        //    finally
        //    {
        //        objTS5Core = null;
        //        objSearchService = null;
        //        objResultList = null;
        //    }

        //    return Ok(clsAUDIO);
        //}

        ///// <summary>
        ///// 檢索圖片
        ///// </summary>
        ///// <param name="clsPARAMETER">檢索參數</param>
        ///// <returns></returns>
        //[Route("SearchPhoto")]
        //[Obsolete("已過時，請改用SearchMeta")]
        //public IHttpActionResult PostSearchPhoto(clsPARAMETER clsPARAMETER)
        //{
        //    clsSEARCH_RESULT.clsPHOTO clsPHOTO = new clsSEARCH_RESULT.clsPHOTO();
        //    clsPHOTO.lstPHOTO = new List<clsSEARCH_RESULT.clsPHOTO.PHOTO>();

        //    Ts5Core objTS5Core = new Ts5Core();
        //    TS50SDK.SearchService objSearchService = null;
        //    TS50SDK.ResultList objResultList = null;
        //    TS50SDK.SimpleCriteria objSimpleCriteria = new TS50SDK.SimpleCriteria();
        //    TS50SDK.FieldFilter objFieldFilter = new TS50SDK.FieldFilter();
        //    Order objOrder = new Order();
        //    string fsEXPRESSION = string.Empty;

        //    try
        //    {
        //        //索引庫
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_DATA_SOURCE, clsPARAMETER.fsINDEX);

        //        //關鍵字
        //        if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD))
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_KEYWORD, clsPARAMETER.fsKEYWORD);
        //        }

        //        //不分全型半型
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_FORM_CONVERT, true);

        //        //日期
        //        if (clsPARAMETER.clsDATE != null)
        //        {
        //            fsEXPRESSION += objFieldFilter.SetCondition("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE + "~" + clsPARAMETER.clsDATE.fdEDATE) + "&";
        //        }

        //        //可查詢節點權限
        //        if (!clsPARAMETER.fbIS_ADMIN)
        //        {
        //            if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
        //            {
        //                fsEXPRESSION += objFieldFilter.SetCondition("$fsAUTHORUTY_DIR_ID", clsPARAMETER.fsAUTH_DIR) + "&";
        //            }
        //            else
        //            {
        //                clsPHOTO.fsRESULT = "error";
        //                clsPHOTO.fsMESSAGE = "無檢索權限";
        //                return Ok(clsPHOTO);
        //            }
        //        }

        //        //可查詢機密等級
        //        if (!clsPARAMETER.fbIS_ADMIN)
        //        {
        //            if (!string.IsNullOrEmpty(clsPARAMETER.fsSECRET))
        //            {
        //                fsEXPRESSION += objFieldFilter.SetCondition("$fnFILE_SECRET", clsPARAMETER.fsSECRET) + "&";
        //            }
        //        }

        //        //樣板編號
        //        if (clsPARAMETER.fnTEMP_ID > 0)
        //        {
        //            fsEXPRESSION += objFieldFilter.SetCondition("$fnTEMP_ID", clsPARAMETER.fnTEMP_ID.ToString()) + "&";
        //        }

        //        //欄位檢索
        //        if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
        //        {
        //            foreach (var item in clsPARAMETER.lstCOLUMN_SEARCH)
        //            {
        //                if (item.fbIS_FULLTEXT)
        //                    fsEXPRESSION += objFieldFilter.SetCondition(item.fsCOLUMN, item.fsVALUE) + "&";
        //                else
        //                    fsEXPRESSION += objFieldFilter.SetCondition("$" + item.fsCOLUMN, item.fsVALUE) + "&";
        //            }
        //        }

        //        //條件加入Filter項目
        //        if (!string.IsNullOrEmpty(fsEXPRESSION))
        //        {
        //            fsEXPRESSION = fsEXPRESSION.Substring(0, fsEXPRESSION.Length - 1);
        //            objFieldFilter.SetExpression(fsEXPRESSION);
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_FILTER, objFieldFilter);
        //        }

        //        //同音
        //        if (clsPARAMETER.fnHOMO == 1)
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_HOMOPHONE, true);
        //        }

        //        //Refiner
        //        if (!string.IsNullOrEmpty(clsPARAMETER.fsTREE))
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_CATEGORY, clsPARAMETER.fsTREE);
        //        }

        //        //排序
        //        if (clsPARAMETER.lstCOLUMN_ORDER != null && clsPARAMETER.lstCOLUMN_ORDER.Count > 0)
        //        {
        //            foreach (var item in clsPARAMETER.lstCOLUMN_ORDER)
        //            {
        //                if (item.fsVALUE == "0")
        //                    objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_ASC);
        //                else if (item.fsVALUE == "1")
        //                    objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_DES);
        //            }
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_ORDER, objOrder);
        //        }

        //        //檢索方式
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_SEARCH_MODE, clsPARAMETER.fnSEARCH_MODE);

        //        objTS5Core.Initialize(Properties.Settings.Default.fsTS50);
        //        objSearchService = objTS5Core.GetSearchService();
        //        objSearchService.SetResultMark("<<", ">>");
        //        objResultList = objSearchService.SearchBySimpleCriteria(objSimpleCriteria, clsPARAMETER.fnSTART_INDEX, clsPARAMETER.fnPAGE_SIZE);

        //        if (objResultList != null && objResultList.GetSize() > 0)
        //        {
        //            for (int i = 0; i < objResultList.GetSize(); i++)
        //            {
        //                clsPHOTO.lstPHOTO.Add(new clsSEARCH_RESULT.clsPHOTO.PHOTO()
        //                {
        //                    fsFILE_NO = objResultList.GetItem(i).GetFieldByName("fsFILE_NO").ToString(),
        //                    //fsTITLE = objResultList.GetItem(i).GetFieldByName("fsTITLE").ToString(),
        //                    //fsDESCRIPTION = objResultList.GetItem(i).GetFieldByName("fsDESCRIPTION").ToString(),
        //                    fsMATCH = objResultList.GetItem(i).GetContent()
        //                });
        //            }
        //        }

        //        clsPHOTO.fsRESULT = "success";
        //        clsPHOTO.fsMESSAGE = string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsPHOTO.fsRESULT = "error";
        //        clsPHOTO.fsMESSAGE = ex.Message;

        //        System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-SearchCount：" + ex.Message + "\r\n");
        //    }
        //    finally
        //    {
        //        objTS5Core = null;
        //        objSearchService = null;
        //        objResultList = null;
        //    }

        //    return Ok(clsPHOTO);
        //}

        ///// <summary>
        ///// 檢索文件
        ///// </summary>
        ///// <param name="clsPARAMETER">檢索參數</param>
        ///// <returns></returns>
        //[Route("SearchDoc")]
        //[Obsolete("已過時，請改用SearchMeta")]
        //public IHttpActionResult PostSearchDoc(clsPARAMETER clsPARAMETER)
        //{
        //    clsSEARCH_RESULT.clsDOC clsDOC = new clsSEARCH_RESULT.clsDOC();
        //    clsDOC.lstDOC = new List<clsSEARCH_RESULT.clsDOC.DOC>();

        //    Ts5Core objTS5Core = new Ts5Core();
        //    TS50SDK.SearchService objSearchService = null;
        //    TS50SDK.ResultList objResultList = null;
        //    TS50SDK.SimpleCriteria objSimpleCriteria = new TS50SDK.SimpleCriteria();
        //    TS50SDK.FieldFilter objFieldFilter = new TS50SDK.FieldFilter();
        //    Order objOrder = new Order();
        //    string fsEXPRESSION = string.Empty;

        //    try
        //    {
        //        //索引庫
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_DATA_SOURCE, clsPARAMETER.fsINDEX);

        //        //關鍵字
        //        if (!string.IsNullOrEmpty(clsPARAMETER.fsKEYWORD))
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_KEYWORD, clsPARAMETER.fsKEYWORD);
        //        }

        //        //不分全型半型
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_FORM_CONVERT, true);

        //        //日期
        //        if (clsPARAMETER.clsDATE != null)
        //        {
        //            fsEXPRESSION += objFieldFilter.SetCondition("$" + clsPARAMETER.clsDATE.fsCOLUMN, clsPARAMETER.clsDATE.fdSDATE + "~" + clsPARAMETER.clsDATE.fdEDATE) + "&";
        //        }

        //        //可查詢節點權限
        //        if (!clsPARAMETER.fbIS_ADMIN)
        //        {
        //            if (!string.IsNullOrEmpty(clsPARAMETER.fsAUTH_DIR))
        //            {
        //                fsEXPRESSION += objFieldFilter.SetCondition("$fsAUTHORUTY_DIR_ID", clsPARAMETER.fsAUTH_DIR) + "&";
        //            }
        //            else
        //            {
        //                clsDOC.fsRESULT = "error";
        //                clsDOC.fsMESSAGE = "無檢索權限";
        //                return Ok(clsDOC);
        //            }
        //        }

        //        //可查詢機密等級
        //        if (!clsPARAMETER.fbIS_ADMIN)
        //        {
        //            if (!string.IsNullOrEmpty(clsPARAMETER.fsSECRET))
        //            {
        //                fsEXPRESSION += objFieldFilter.SetCondition("$fnFILE_SECRET", clsPARAMETER.fsSECRET) + "&";
        //            }
        //        }

        //        //樣板編號
        //        if (clsPARAMETER.fnTEMP_ID > 0)
        //        {
        //            fsEXPRESSION += objFieldFilter.SetCondition("$fnTEMP_ID", clsPARAMETER.fnTEMP_ID.ToString()) + "&";
        //        }

        //        //欄位檢索
        //        if (clsPARAMETER.lstCOLUMN_SEARCH != null && clsPARAMETER.lstCOLUMN_SEARCH.Count > 0)
        //        {
        //            foreach (var item in clsPARAMETER.lstCOLUMN_SEARCH)
        //            {
        //                if (item.fbIS_FULLTEXT)
        //                    fsEXPRESSION += objFieldFilter.SetCondition(item.fsCOLUMN, item.fsVALUE) + "&";
        //                else
        //                    fsEXPRESSION += objFieldFilter.SetCondition("$" + item.fsCOLUMN, item.fsVALUE) + "&";
        //            }
        //        }

        //        //條件加入Filter項目
        //        if (!string.IsNullOrEmpty(fsEXPRESSION))
        //        {
        //            fsEXPRESSION = fsEXPRESSION.Substring(0, fsEXPRESSION.Length - 1);
        //            objFieldFilter.SetExpression(fsEXPRESSION);
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_FILTER, objFieldFilter);
        //        }

        //        //同音
        //        if (clsPARAMETER.fnHOMO == 1)
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_HOMOPHONE, true);
        //        }

        //        //Refiner
        //        if (!string.IsNullOrEmpty(clsPARAMETER.fsTREE))
        //        {
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_CATEGORY, clsPARAMETER.fsTREE);
        //        }

        //        //排序
        //        if (clsPARAMETER.lstCOLUMN_ORDER != null && clsPARAMETER.lstCOLUMN_ORDER.Count > 0)
        //        {
        //            foreach (var item in clsPARAMETER.lstCOLUMN_ORDER)
        //            {
        //                if (item.fsVALUE == "0")
        //                    objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_ASC);
        //                else if (item.fsVALUE == "1")
        //                    objOrder.SetOrderType("$" + item.fsCOLUMN, OrderType.ORDER_TYPE_DES);
        //            }
        //            objSimpleCriteria.Add(SearchCriteria.CRITERIA_ORDER, objOrder);
        //        }

        //        //檢索方式
        //        objSimpleCriteria.Add(SearchCriteria.CRITERIA_SEARCH_MODE, clsPARAMETER.fnSEARCH_MODE);

        //        objTS5Core.Initialize(Properties.Settings.Default.fsTS50);
        //        objSearchService = objTS5Core.GetSearchService();
        //        objSearchService.SetResultMark("<<", ">>");
        //        objResultList = objSearchService.SearchBySimpleCriteria(objSimpleCriteria, clsPARAMETER.fnSTART_INDEX, clsPARAMETER.fnPAGE_SIZE);

        //        if (objResultList != null && objResultList.GetSize() > 0)
        //        {
        //            for (int i = 0; i < objResultList.GetSize(); i++)
        //            {
        //                clsDOC.lstDOC.Add(new clsSEARCH_RESULT.clsDOC.DOC()
        //                {
        //                    fsFILE_NO = objResultList.GetItem(i).GetFieldByName("fsFILE_NO").ToString(),
        //                    //fsTITLE = objResultList.GetItem(i).GetFieldByName("fsTITLE").ToString(),
        //                    //fsDESCRIPTION = objResultList.GetItem(i).GetFieldByName("fsDESCRIPTION").ToString(),
        //                    fsMATCH = objResultList.GetItem(i).GetContent()
        //                });
        //            }
        //        }

        //        clsDOC.fsRESULT = "success";
        //        clsDOC.fsMESSAGE = string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        clsDOC.fsRESULT = "error";
        //        clsDOC.fsMESSAGE = ex.Message;

        //        System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-SearchCount：" + ex.Message + "\r\n");
        //    }
        //    finally
        //    {
        //        objTS5Core = null;
        //        objSearchService = null;
        //        objResultList = null;
        //    }

        //    return Ok(clsDOC);
        //}

        /// <summary>
        /// 取得同義詞
        /// </summary>
        /// <param name="fsSYNONYM">同義詞彙</param>
        /// <returns></returns>
        [Route("GetSynonym")]
        public HttpResponseMessage GetSynonym(string fsSYNONYM)
        {
            List<clsSYNONYM> lstSYNONYM = new List<clsSYNONYM>();

            TermManager objTermManager = new TermManager();
            TS50SDK.Configuration objConfiguration = new TS50SDK.Configuration();
            StringList objListSynonym = new StringList();

            try
            {
                objConfiguration.Load(Properties.Settings.Default.fsTS50);
                objTermManager.Initialize(objConfiguration);

                objListSynonym = objTermManager.ListSynonym(fsSYNONYM);

                if (objListSynonym != null)
                {
                    for (int i = 0; i <= objListSynonym.GetSize() - 1; i++)
                    {
                        lstSYNONYM.Add(new clsSYNONYM() { fsSYNONYM = objListSynonym.GetElement(i) });
                    }
                }
                
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true, Data = lstSYNONYM }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-GetSynonym：" + ex.Message + "\r\n");

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            finally
            {
                objTermManager = null;
                objConfiguration = null;
                objListSynonym = null;
            }
        }

        /// <summary>
        /// 重建同義詞
        /// </summary>
        /// <param name="lstSYNONYMs">所有同義詞組</param>
        /// <returns></returns>
        [Route("RebuildSynonym")]
        public HttpResponseMessage PostRebuildSynonym(List<List<clsSYNONYM>> lstSYNONYMs)
        {
            clsRETURN clsRETURN = new clsRETURN();

            TermManager objTermManager = new TermManager();
            TS50SDK.Configuration objConfiguration = new TS50SDK.Configuration();

            try
            {
                objConfiguration.Load(Properties.Settings.Default.fsTS50);
                objTermManager.Initialize(objConfiguration);

                if (lstSYNONYMs != null && lstSYNONYMs.Count > 0)
                {
                    foreach (var item in lstSYNONYMs)
                    {
                        if (item != null && item.Count > 1)
                        {
                            for (int i = 0; i <= item.Count - 1; i++)
                            {
                                for (int j = 0; j <= item.Count - 1; j++)
                                {
                                    if (i != j)
                                    {
                                        if (!objTermManager.SetSynonym(item[i].fsSYNONYM, item[j].fsSYNONYM))
                                        {
                                            System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-RebuildSynonym：" + objTermManager.GetErrorMessage() + " 主詞：" + item[i].fsSYNONYM + "，同義詞：" + item[j].fsSYNONYM + "\r\n");
                                            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = objTermManager.GetErrorMessage() }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                                        }
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

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            finally
            {
                objTermManager = null;
                objConfiguration = null;
            }
        }

        /// <summary>
        /// 新增同義詞
        /// </summary>
        /// <param name="lstSYNONYM">單一同義詞組</param>
        /// <returns></returns>
        [Route("InsertSynonym")]
        public HttpResponseMessage PostInsertSynonym(List<clsSYNONYM> lstSYNONYM)
        {
            clsRETURN clsRETURN = new clsRETURN();

            TermManager objTermManager = new TermManager();
            TS50SDK.Configuration objConfiguration = new TS50SDK.Configuration();

            try
            {
                objConfiguration.Load(Properties.Settings.Default.fsTS50);
                objTermManager.Initialize(objConfiguration);

                if (lstSYNONYM != null && lstSYNONYM.Count > 1)
                {
                    for (int i = 0; i <= lstSYNONYM.Count - 1; i++)
                    {
                        for (int j = 0; j <= lstSYNONYM.Count - 1; j++)
                        {
                            if (i != j)
                            {
                                if (!objTermManager.SetSynonym(lstSYNONYM[i].fsSYNONYM, lstSYNONYM[j].fsSYNONYM))
                                {
                                    
                                    System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-InsertSynonym：" + objTermManager.GetErrorMessage() + " 主詞：" + lstSYNONYM[i].fsSYNONYM + "，同義詞：" + lstSYNONYM[j].fsSYNONYM + "\r\n");
                                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, Message = objTermManager.GetErrorMessage() }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                                }
                            }
                        }
                    }
                }

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            finally
            {
                objTermManager = null;
                objConfiguration = null;
            }
        }

        /// <summary>
        /// 刪除同義詞
        /// </summary>
        /// <param name="lstSYNONYM">同義詞(可為詞組或詞彙)</param>
        /// <returns></returns>
        [Route("DeleteSynonym")]
        public HttpResponseMessage PostDeleteSynonym(List<clsSYNONYM> lstSYNONYM)
        {
            clsRETURN clsRETURN = new clsRETURN();

            TermManager objTermManager = new TermManager();
            TS50SDK.Configuration objConfiguration = new TS50SDK.Configuration();
            StringList objListSynonym = new StringList();

            try
            {
                objConfiguration.Load(Properties.Settings.Default.fsTS50);
                objTermManager.Initialize(objConfiguration);

                if (lstSYNONYM != null)
                {
                    if (lstSYNONYM.Count == 1)
                    {
                        objListSynonym = objTermManager.ListSynonym(lstSYNONYM[0].fsSYNONYM);

                        for (short i = 0; i <= objListSynonym.GetSize() - 1; i++)
                        {
                            objTermManager.RemoveSynonym(lstSYNONYM[0].fsSYNONYM, objListSynonym.GetElement(i));

                            objTermManager.RemoveSynonym(objListSynonym.GetElement(i), lstSYNONYM[0].fsSYNONYM);
                        }
                    }
                    else if (lstSYNONYM.Count > 1)
                    {
                        for (int i = 0; i <= lstSYNONYM.Count - 1; i++)
                        {
                            for (int j = 0; j <= lstSYNONYM.Count - 1; j++)
                            {
                                if (i != j)
                                {
                                    if (!objTermManager.RemoveSynonym(lstSYNONYM[i].fsSYNONYM, lstSYNONYM[j].fsSYNONYM))
                                    {
                                        System.IO.File.AppendAllText(Properties.Settings.Default.fsLOG + DateTime.Now.ToString("yyyyMMdd") + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "-DeleteSynonym：" + objTermManager.GetErrorMessage() + " 主詞：" + lstSYNONYM[i].fsSYNONYM + "，同義詞：" + lstSYNONYM[j].fsSYNONYM + "\r\n");
                                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false , Message = objTermManager.GetErrorMessage() }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
                                    }
                                }
                            }
                        }
                    }
                }

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = true }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new ObjectContent<clsRETURN>(new clsRETURN() { IsSuccess = false, ErrorException = ex }, GlobalConfiguration.Configuration.Formatters.JsonFormatter) };
            }
            finally
            {
                objTermManager = null;
                objConfiguration = null;
            }
        }

    }
}
