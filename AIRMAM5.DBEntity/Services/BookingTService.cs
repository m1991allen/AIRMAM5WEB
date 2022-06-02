using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Material;
using AIRMAM5.DBEntity.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 調用樣板資料表 dbo.[tbmBOOKING_T]
    /// </summary>
    public class BookingTService
    {
        readonly IGenericRepository<tbmBOOKING_T> _bookingTRepository = new GenericRepository<tbmBOOKING_T>();
        readonly ISerilogService _serilogService;
        readonly ICodeService _tbzCodeService;
        //protected AIRMAM5DBEntities _db;

        public BookingTService()
        {
            //this._db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
            _tbzCodeService = new CodeService();
        }
        public BookingTService(ISerilogService serilogService, ICodeService codeService)
        {
            _serilogService = serilogService;
            _tbzCodeService = codeService;
        }

        /// <summary>
        /// 指定調用樣板id 取回資料 或不指定取回全部資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<tbmBOOKING_T> GetBy(int? id = null)
        {
            var query = _bookingTRepository.FindBy(x => id == null ? true : x.fnID == id).OrderBy(o => o.fnORDER).ToList();

            return query ?? new List<tbmBOOKING_T>();
        }

        #region ---------- DropDownList【SelectListItem】
        /// <summary>
        /// 調用原因/樣板 選單
        /// </summary>
        /// <param name="id">指定選單選定的樣板id, 選填。</param>
        /// <param name="typeEnum">轉檔類別 <see cref="BookingTranTypeEnum"/> , 選填。</param>
        /// <returns></returns>
        public List<SelectListItem> GetMaterialReson(int? id = null, BookingTranTypeEnum typeEnum = BookingTranTypeEnum.BOOKING)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst = _bookingTRepository
                //.FindBy(x => x.fcACTIVE == IsTrueFalseEnum.Y.ToString() && x.fsTRAN_TYPE != "BOOKING_BATCH")
                .FindBy(x => x.fcACTIVE == IsTrueFalseEnum.Y.ToString() && x.fsTRAN_TYPE == typeEnum.ToString())
                .OrderBy(o => o.fnORDER)
                .Select(s => new SelectListItem
                {
                    Value = s.fnID.ToString(),
                    Text = s.fsNAME,
                    Selected = id == s.fnID ? true : false
                }).ToList();

            return lst ?? new List<SelectListItem>();
        }
        #endregion

        /// <summary>
        /// 調用原因/樣板 選項資料
        /// </summary>
        /// <param name="id">指定樣板id 或 不指定,取預設第一筆樣板資料 </param>
        /// <param name="typeEnum">轉檔類別 <see cref="BookingTranTypeEnum"/> , 預設為"一般調用"。</param>
        /// <returns></returns>
        public BookingOptionModel GetBookingOption(int? id = null, BookingTranTypeEnum typeEnum = BookingTranTypeEnum.BOOKING)
        {
            BookingOptionModel get = new BookingOptionModel();

            var query = _bookingTRepository.FindBy(x => id == null ? true : x.fnID == id)
                .Where(x => x.fcACTIVE == IsTrueFalseEnum.Y.ToString())
                .OrderBy(o => o.fnORDER)
                //.Select(s => new BookingOptionModel
                //{
                //    ResonList = this.GetMaterialReson(s.fnID, typeEnum),
                //    PathList = _tbzCodeService.SpecifyCodeListItem(TbzCodeIdEnum.BOOKING_PATH.ToString(), s.fsPATH),
                //    VideoProfileList = _tbzCodeService.SpecifyCodeListItem(TbzCodeIdEnum.TRAN_PROFILE_V.ToString(), s.fsPROFILE_V),
                //    AudioProfileList = _tbzCodeService.SpecifyCodeListItem(TbzCodeIdEnum.TRAN_PROFILE_A.ToString(), s.fsPROFILE_A),
                //    WatermarkList = _tbzCodeService.SpecifyCodeListItem(TbzCodeIdEnum.WATER_MARK.ToString(), s.fsWATERMARK),
                //    DescIsNullable = s.fbDESCRIPTION == IsTrueFalseEnum.Y.ToString() ? true : false
                //})
                .FirstOrDefault();

            if (query != null)
            {
                get.ResonList = this.GetMaterialReson(query.fnID, typeEnum);
                //調用路徑 "BOOKING_PATH"
                get.PathList = _tbzCodeService.SpecifyCodeListItem(TbzCodeIdEnum.BOOKING_PATH.ToString(), query.fsPATH);
                //影片 "TRAN_PROFILE_V"
                get.VideoProfileList = _tbzCodeService.SpecifyCodeListItem(TbzCodeIdEnum.TRAN_PROFILE_V.ToString(), query.fsPROFILE_V);
                //聲音 "TRAN_PROFILE_A"
                get.AudioProfileList = _tbzCodeService.SpecifyCodeListItem(TbzCodeIdEnum.TRAN_PROFILE_A.ToString(), query.fsPROFILE_A);
                //浮水印
                get.WatermarkList = _tbzCodeService.SpecifyCodeListItem(TbzCodeIdEnum.WATER_MARK.ToString(), query.fsWATERMARK);

                get.DescIsNullable = query.fbDESCRIPTION == IsTrueFalseEnum.Y.ToString() ? true : false;
            }

            return get;
        }
    }
}
