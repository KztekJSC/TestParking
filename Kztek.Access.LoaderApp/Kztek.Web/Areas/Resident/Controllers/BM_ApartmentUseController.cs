using Kztek.Model.CustomModel;
using Kztek.Model.CustomModel.iParking;
using Kztek.Model.Models;
using Kztek.Service.Admin;
using Kztek.Web.Attributes;
using Kztek.Web.Core.Extensions;
using Kztek.Web.Core.Functions;
using Kztek.Web.Core.Helpers;
using Kztek.Web.Core.Models;
using Kztek.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kztek.Web.Areas.Resident.Controllers
{
    public class BM_ApartmentUseController : Controller
    {
        private IBM_ApartmentUseService _BM_ApartmentUseService;
        private ItblSystemConfigService _tblSystemConfigService;
        private ItblCardService _tblCardService;


        public BM_ApartmentUseController(IBM_ApartmentUseService _BM_ApartmentUseService,
            ItblSystemConfigService _tblSystemConfigService, ItblCardService _tblCardService)
        {
            this._BM_ApartmentUseService = _BM_ApartmentUseService;
            this._tblSystemConfigService = _tblSystemConfigService;
            this._tblCardService = _tblCardService;
        }

        #region Danh sách

        [CheckSessionLogin]
        [CheckAuthorize]
        public ActionResult Index(string key = "", int page = 1, string chkExport = "0", string selectedId = "")
        {

            //if (chkExport.Equals("1"))
            //{
            //    var listExcel = _BM_ApartmentUseService.ExcelAllByFirst(key, str, customerstatus);

            //    //Xuất file theo format
            //    PK_CustomerMapFormatCell(listExcel, "Danh_sách_khách_hàng", "Sheet1", "", "Danh sách khách hàng", "");

            //    return RedirectToAction("Index", new { key = key, customergroup = customergroup, customerstatus = customerstatus, page = page });
            //}

            var pageSize = 20;

            var list = _BM_ApartmentUseService.GetPagingByFirst(key, page, pageSize);

            var gridModel = PageModelCustom<BM_ApartmentUse>.GetPage(list, page, pageSize);

            ViewBag.keyValue = key;

            return View(gridModel);
        }

        //Excell
        //private void PK_CustomerMapFormatCell(List<BM_ApartmentUse_Excel> listData, string filename = "", string sheetname = "", string comments = "", string titleSheet = "", string titleTime = "")
        //{
        //    //Nội dung đầu trang
        //    var user = GetCurrentUser.GetUser();

        //    var timeSearch = "";

        //    if (!string.IsNullOrWhiteSpace(titleTime))
        //    {
        //        timeSearch = "Từ " + titleTime.Split(new[] { '-' })[0] + " đến " + titleTime.Split(new[] { '-' })[1];
        //    }

        //    var dtHeader = _tblSystemConfigService.getHeaderExcel(titleSheet, timeSearch, user.Username);

        //    //Header
        //    var listColumn = new List<SelectListModel>();
        //    listColumn.Add(new SelectListModel { ItemText = "STT", ItemValue = "NumberRow" });
        //    listColumn.Add(new SelectListModel { ItemText = "Mã khách hàng", ItemValue = "CustomerCode" });
        //    listColumn.Add(new SelectListModel { ItemText = "Khách hàng", ItemValue = "CustomerName" });
        //    listColumn.Add(new SelectListModel { ItemText = "Chứng minh thư", ItemValue = "CustomerIdentify" });
        //    listColumn.Add(new SelectListModel { ItemText = "Địa chỉ", ItemValue = "CustomerAddress" });
        //    listColumn.Add(new SelectListModel { ItemText = "Số điện thoại", ItemValue = "CustomerMobile" });
        //    listColumn.Add(new SelectListModel { ItemText = "Nhóm khách hàng", ItemValue = "CustomerGroupName" });
        //    listColumn.Add(new SelectListModel { ItemText = "Mã thẻ (Số thẻ)", ItemValue = "Cards" });
        //    listColumn.Add(new SelectListModel { ItemText = "Biển số", ItemValue = "Plates" });
        //    listColumn.Add(new SelectListModel { ItemText = "Hoạt động", ItemValue = "Active" });

        //    //Chuyển dữ liệu về datatable
        //    DataTable dt = listData.ToDataTableNullable();

        //    //Xuất file
        //    ExportFile(dt, listColumn, dtHeader, filename, sheetname, comments);
        //}

        //private void ExportFile(DataTable list = null, List<SelectListModel> listTitle = null, DataTable dtHeader = null, string filename = "", string sheetname = "", string comments = "")
        //{
        //    // Gọi lại hàm để tạo file excel
        //    var stream = FunctionHelper.WriteToExcel(null, list, listTitle, dtHeader, sheetname, comments);
        //    // Tạo buffer memory strean để hứng file excel
        //    var buffer = stream as MemoryStream;
        //    // Đây là content Type dành cho file excel, còn rất nhiều content-type khác nhưng cái này mình thấy okay nhất
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    // Dòng này rất quan trọng, vì chạy trên firefox hay IE thì dòng này sẽ hiện Save As dialog cho người dùng chọn thư mục để lưu
        //    // File name của Excel này là ExcelDemo
        //    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.xlsx", filename, DateTime.Now.ToString("yyyyMMdd")));
        //    // Lưu file excel của chúng ta như 1 mảng byte để trả về response
        //    Response.BinaryWrite(buffer.ToArray());
        //    // Send tất cả ouput bytes về phía clients
        //    Response.Flush();
        //    Response.End();
        //}
        #endregion

        #region Thêm mới
        [CheckSessionLogin]
        [CheckAuthorize]
        [HttpGet]
        public ActionResult Create(string key = "", int page = 1)
        {
            ViewBag.keyValue = key;
            return View();
        }

        [HttpPost]
        public ActionResult Create(BM_ApartmentUse obj, bool SaveAndCountinue = false, string key = "")
        {
            ViewBag.keyValue = key;

            //Kiểm tra
            if (!ModelState.IsValid)
            {
                return View(obj);
            }


            //Gán giá trị
            obj.Id = Guid.NewGuid().ToString();
            obj.DateCreated = DateTime.UtcNow;

            //Thực hiện thêm mới
            var result = _BM_ApartmentUseService.Create(obj);
            if (result.isSuccess)
            {
                WriteLog.Write(result, GetCurrentUser.GetUser(), obj.Id.ToString(), obj.Name, "BM_ApartmentUse", ConstField.ResidentCode, ActionConfigO.Create);

                if (SaveAndCountinue)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction("Create", new { key = key });
                }

                return RedirectToAction("Index", new { key = key });
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(obj);
            }
        }
        #endregion

        #region Cập nhật
        [CheckSessionLogin]
        [CheckAuthorize]
        [HttpGet]
        public ActionResult Update(string id, string key = "", string residentGroup = "", int page = 1)
        {
            ViewBag.keyValue = key;
            ViewBag.ResidentGroupValue = residentGroup;
            ViewBag.PN = page;

            var obj = _BM_ApartmentUseService.GetById(id);

            return View(obj);
        }

        [HttpPost]
        public ActionResult Update(BM_ApartmentUse obj, string key = "", int page = 1)
        {
            ViewBag.keyValue = key;
            ViewBag.PN = page;

            //Kiểm tra
            var oldObj = _BM_ApartmentUseService.GetById(obj.Id);
            if (oldObj == null)
            {
                ViewBag.Error = "Bản ghi không tồn tại";
                return View(obj);
            }

            if (!ModelState.IsValid)
            {
                return View(oldObj);
            }

            //Gán giá trị
            oldObj.Name = obj.Name;
            oldObj.Ordering = obj.Ordering;

            //Thực hiện cập nhật
            var result = _BM_ApartmentUseService.Update(oldObj);
            if (result.isSuccess)
            {
                WriteLog.Write(result, GetCurrentUser.GetUser(), obj.Id.ToString(), obj.Name, "BM_ApartmentUse", ConstField.ResidentCode, ActionConfigO.Update);

                return RedirectToAction("Index", new { page = page, key = key });
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(oldObj);
            }
        }
        #endregion

        #region Xóa
        public JsonResult Delete(string id)
        {
            var obj = new BM_ApartmentUse();

            var result = _BM_ApartmentUseService.DeleteById(id, ref obj);
            if (result.isSuccess)
            {
                WriteLog.Write(result, GetCurrentUser.GetUser(), obj.Id.ToString(), obj.Name, "BM_ApartmentUse", ConstField.ResidentCode, ActionConfigO.Delete);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Upload file
        private void UploadFile(HttpPostedFileBase fileUpload)
        {
            if (fileUpload != null)
            {
                string error = "";

                var url = ConfigurationManager.AppSettings["FileUploadAvatar"];

                Common.UploadFile(out error, Server.MapPath(url), fileUpload);
            }
        }
        #endregion

    }
}