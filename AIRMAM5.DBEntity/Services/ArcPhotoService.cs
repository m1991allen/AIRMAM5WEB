using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Common;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.SearchResponse;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Subject;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 圖片主檔 tbmARC_PHOTO
    /// </summary>
    public class ArcPhotoService
    {
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult();
        private readonly IGenericRepository<tbmARC_PHOTO> _arcPhotoRepository = new GenericRepository<tbmARC_PHOTO>();

        public ArcPhotoService()
        {
            //_db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }

        public ArcPhotoService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// 取 tbmARC_PHOTO 入庫項目-圖片檔 資料 【EF dbo.tbmARC_PHOTO】
        /// </summary>
        /// <returns></returns>
        public tbmARC_PHOTO GetByFileno(string fileno)
        {
            return _arcPhotoRepository.Get(x => x.fsFILE_NO == fileno);
        }

        /// <summary>
        /// 多筆檔案編號 取 tbmARC_PHOTO 入庫項目-圖片檔 資料
        /// </summary>
        /// <param name="fnos"></param>
        /// <returns></returns>
        public List<tbmARC_PHOTO> GetByFileNos(string[] fnos)
        {
            var _nos = string.Join(";", fnos);
            var query = _arcPhotoRepository.FindBy(x => SqlFunctions.CharIndex(x.fsFILE_NO, _nos) > 0).ToList();

            return query;
        }

        /// <summary>
        /// 依 SUBJ_ID取 ARC_PHOTO 入庫項目-圖片檔 資料 【spGET_ARC_PHOTO_BY_SUBJ_ID】
        /// </summary>
        /// <param name="subjid">主題Id [fsSUBJ_ID] </param>
        /// <param name="fileno">檔案編號 [fsFILE_NO], 預設值= null </param>
        public List<spGET_ARC_PHOTO_BY_SUBJ_ID_Result> GetArcPhotoBySubjectId (string subjid, string fileno = null)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_PHOTO_BY_SUBJ_ID(subjid).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ARC_PHOTO_BY_SUBJ_ID_Result>();

                query = query.Where(x => fileno == null ? true : x.fsFILE_NO == fileno).ToList();

                return query;
            }
        }

        /// <summary>
        /// 依 SUBJ_DI+ FILE_NO 取回 tbmARC_PHOTO 資料內容欄位 【dbo.spGET_ARC_PHOTO】
        /// <para> TIP: 預存語法是 SELECT * from tbmARC_PHOTO ,回傳結果會因為資料表新增/刪除影響, 要注意。 </para>
        /// </summary>
        /// <param name="subjid">主題編號 fsSUBJECT_ID (允訐空值查詢) </param>
        /// <param name="fileno">檔案編號 fsFILE_NO (允訐空值查詢) </param>
        /// <returns></returns>
        public List<spGET_ARC_PHOTO_Result> GetArcPhotoByIdFile(string subjid, string fileno)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_PHOTO(fileno, subjid).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ARC_PHOTO_Result>();

                return query;
            }
        }

        /// <summary>
        /// 依 FILE_NO 取回 tbmARC_PHOTO 自訂欄位 【dbo.spGET_ARC_PHOTO_ATTRIBUTE】
        /// </summary>
        /// <param name="fileno">檔案編號 fsFILE_NO </param>
        /// <returns></returns>
        public List<spGET_ARC_PHOTO_ATTRIBUTE_Result> GetPhotoAttrByFile(string fileno)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_ARC_PHOTO_ATTRIBUTE(fileno).DefaultIfEmpty().ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_ARC_PHOTO_ATTRIBUTE_Result>();

                return query;
            }
        }

        /// <summary>
        /// 依 FILE_NO 取 圖片-EXIF資訊
        /// </summary>
        /// <param name="fileno">檔案編號 fsFILE_NO </param>
        /// <returns></returns>
        public SearchResponsePhotoModel.ExifInfo GetPhotoExif(string fileno)
        {
            var query = _arcPhotoRepository.FindBy(x => x.fsFILE_NO == fileno)
                .Select(s => new SearchResponsePhotoModel.ExifInfo
                {
                    PhotoWidth = s.fnWIDTH ?? 0,
                    PhotoHeight = s.fnHEIGHT ?? 0,
                    PhotoXdpi = s.fnXDPI ?? 0,
                    PhotoYdpi = s.fnYDPI ?? 0,
                    CameraMake = s.fsCAMERA_MAKE,
                    CameraModel = s.fsCAMERA_MODEL,
                    FocalLength = s.fsFOCAL_LENGTH,
                    ExposureTime = s.fsEXPOSURE_TIME,
                    Aperture = s.fsAPERTURE,
                    ISO = s.fnISO ?? 0
                }).ToList();

            if (query == null || query.FirstOrDefault() == null)
                return new SearchResponsePhotoModel.ExifInfo();

            return query.FirstOrDefault();
        }

        #region ---------- CURD 【tbmARC_PHOTO】: 新 修 刪
        /// <summary>
        /// 新建存檔 tbmARC_PHOTO: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="arePreId"></param>
        /// <returns></returns>
        public VerifyResult CreateBy(tbmARC_PHOTO rec, int arePreId = 0)
        {
            try
            {
                //non-Use -- EXECUTE dbo.spINSERT_ARC_PHOTO --✘✘✘
                using (_db = new AIRMAM5DBEntities())
                {
                    //TODO:當有指定"預編詮釋"資料ID, 要檢查此預編的樣板和Photo的樣板是否相同
                    if (arePreId > 0)
                    {
                        #region 【判斷選定的預編樣板是否與此圖片樣板相同】
                        var _subj = _db.tbmSUBJECT.Where(x => x.fsSUBJ_ID == rec.fsSUBJECT_ID).FirstOrDefault();
                        var _direc = _db.tbmDIRECTORIES.Where(x => x.fnDIR_ID == _subj.fnDIR_ID).FirstOrDefault();
                        var _arcpre = _db.tbmARC_PRE.Where(x => x.fnPRE_ID == arePreId).FirstOrDefault();

                        if (_direc.fnTEMP_ID_PHOTO != _arcpre.fnTEMP_ID)
                        {
                            result.IsSuccess = true;
                            result.Message = "此圖片需要的樣板與預編詮釋資料的樣板不符.";
                            result.Data = rec;
                            return result;
                        }
                        //else
                        //{
                        //    //取出預編資料寫入rec
                        //    var arcPre = _db.tbmARC_PRE.Where(x => x.fnPRE_ID == arePreId).FirstOrDefault();
                        //    rec.fsTITLE = arcPre.fsTITLE;
                        //    rec.fsDESCRIPTION = arcPre.fsDESCRIPTION;
                        //    rec.fsATTRIBUTE1 = arcPre.fsATTRIBUTE1;
                        //    rec.fsATTRIBUTE2 = arcPre.fsATTRIBUTE2;
                        //      ....
                        //    rec.fsATTRIBUTE59 = arcPre.fsATTRIBUTE59;
                        //    rec.fsATTRIBUTE60 = arcPre.fsATTRIBUTE60;
                        //}
                        #endregion

                        rec = new ArcPreService().UseArcPreAttribute(arePreId, rec);
                    }

                    _arcPhotoRepository.Create(rec);

                    result.IsSuccess = true;
                    result.Message = "圖片檔案已新增.";
                    result.Data = rec;
                }
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcPhotoService",
                    Method = "[CreateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"圖片檔案新增失敗【{ex.Message}】 ")
                });
                #endregion
                result.IsSuccess = false;
                result.Message = string.Format($"圖片檔案新增失敗【{ex.Message}】");
            }
            return result;
        }

        /// <summary>
        /// '單筆/多筆批次'修改更新 媒資圖片 tbmARC_PHOTO: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateMultiple(FormCollection form, string updateby)
        {
            try
            {
                string[] _fileno = form["fsFILE_NO"].ToString().Split(new char[] { ',' });
                //↑form裡面的fsFILE_NO 在批次修改裡,代表前端回傳的多批檔案編號.(請以逗號(,)為分隔符號)

                string _subjId = form["SubjectId"].ToString();
                //取 目錄節點資料.影片的樣板id
                int _photoTempid = new DirectoriesService().GetDirBySubjId(_subjId).fnTEMP_ID_PHOTO;

                List<tbmARC_PHOTO> getPhoto = new List<tbmARC_PHOTO>();
                using (_db = new AIRMAM5DBEntities())
                {
                    getPhoto = _db.tbmARC_PHOTO.Where(x => _fileno.Contains(x.fsFILE_NO)).ToList();

                    getPhoto.ForEach(x => {
                        x.fsTITLE = string.IsNullOrEmpty(form["Title_IsEdit"]) ? x.fsTITLE
                                    : (form["Title_IsEdit"].ToString().ToUpper() == "ON" ? form["Title"].ToString() : x.fsTITLE);
                        x.fsDESCRIPTION = string.IsNullOrEmpty(form["Description_IsEdit"]) ? x.fsDESCRIPTION
                                    : (form["Description_IsEdit"].ToString().ToUpper() == "ON" ? form["Description"].ToString() : x.fsDESCRIPTION);
                        x.fnFILE_SECRET = string.IsNullOrEmpty(form["FileSecret_IsEdit"]) ? x.fnFILE_SECRET
                                    : (form["FileSecret_IsEdit"].ToString().ToUpper() == "ON" ? short.Parse(form["FileSecret"].ToString()) : x.fnFILE_SECRET);
                        x.fdUPDATED_DATE = DateTime.Now;
                        x.fsUPDATED_BY = updateby;
                        /* added_20210909_版權 */
                        x.fsLICENSE = string.IsNullOrEmpty(form["LicenseCode_IsEdit"]) ? x.fsLICENSE
                                    : (form["LicenseCode_IsEdit"].ToString().ToUpper() == "ON" ? form["LicenseCode"].ToString() : x.fsLICENSE);
                        /* added_20211122_自訂標籤HashTag */
                        x.fsHASH_TAG = string.IsNullOrEmpty(form["HashTag_IsEdit"]) ? x.fsHASH_TAG
                                    : (form["HashTag_IsEdit"].ToString().ToUpper() == "ON" ? form["HashTag"].ToString() : x.fsHASH_TAG);

                        //樣板自訂欄位資料(依欄位屬性給值).SetValue 
                        //Tips_20200409:增加參數判斷批次修改"接口名_IsEdit=on",預設=false
                        new TemplateService().AttriFieldsSetValue<tbmARC_PHOTO>(_photoTempid, x, form, true);
                    });

                    _db.SaveChanges();
                }

                result.IsSuccess = true;
                result.Message = "媒資圖片已修改更新.";

                var _p = this.GetArcPhotoByIdFile(_subjId, _fileno[0]).FirstOrDefault();
                var _data = getPhoto.Select(s => new SubjectFileMetaViewModel().FormatConversion(_p, FileTypeEnum.P)).ToList();
                result.Data = _data;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"媒資圖片修改失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcPhotoService",
                    Method = "[UpdateMultiple]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = "FormCollection", Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"媒資圖片修改失敗【{ex.Message}】 ")
                });
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 單筆修改 媒資圖片 tbmARC_PHOTO: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateBy(tbmARC_PHOTO rec)
        {
            try
            {
                //non-USE : [spUPDATE_ARC_PHOTO_CHANGE]--✘✘✘
                rec.fdUPDATED_DATE = DateTime.Now;
                _arcPhotoRepository.Update(rec);

                result.IsSuccess = true;
                result.Message = "圖片資料修改成功.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"圖片資料修改失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcPhotoService",
                    Method = "[UpdateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"圖片資料修改失敗【{ex.Message}】 ")
                });
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 檔案搬移 - '單筆/多筆批次'修改更新 媒資圖片 tbmARC_PHOTO: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateByMove(List<tbmARC_PHOTO> rec)
        {
            try
            {
                _arcPhotoRepository.UpdateMultiple(rec);

                result.IsSuccess = true;
                result.Message = "圖片檔案搬移更新完成.";
                //TODO:搬移更新後,暫不回傳Data內容
                result.Data = new { };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"圖片檔案搬移更新失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ArcPhotoService",
                    Method = "[UpdateByMove]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"圖片檔案搬移更新失敗【{ex.Message}】 ")
                });
                #endregion
            }

            return result;
        }
        #endregion
    }
}
