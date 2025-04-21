using Quanlyrapchieuphim.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quanlyrapchieuphim.Areas.Admin.Controllers
{
    public class BaoCaoThongKeController : Controller
    {
        // GET: Admin/BaoCaoThongKe
        private QLRapChieuPhimEntities db= new QLRapChieuPhimEntities();
        [HttpGet]
        public ActionResult Index()
        {
            var data=(from s in db.HoaDons
                     orderby s.NgayTao descending
                     select s).ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult Index(DateTime? startDate)
        {
            Session["date"]=startDate;
            var data = db.HoaDons.AsQueryable();
            if (startDate.HasValue)
            {
                DateTime startDateValue = startDate.Value.Date;
                data = data.Where(h => DbFunctions.TruncateTime(h.NgayTao) == startDateValue);
                
            }
            double tongtien = 0;
            foreach (var item in data)
            {
                tongtien += item.TongTien.Value; // Assuming TongTien is the property representing the amount
            }
            ViewBag.TongTien = tongtien;
            return View(data.ToList());
        }
        [HttpGet]
        public ActionResult ThongKeTheoNgay()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ThongKeThangNam()
        {
            var data = (from s in db.HoaDons
                        orderby s.NgayTao descending
                        select s).ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult ThongKeThangNam(DateTime? startDate, DateTime? endDate)
        {
            Session["startDate"] = startDate;
            Session["endDate"] = endDate;

            var data = db.HoaDons.AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                data = data.Where(h => h.NgayTao >= startDate && h.NgayTao <= endDate);
            }
            else if (startDate.HasValue)
            {
                data = data.Where(h => h.NgayTao == startDate);
            }

            double tongtien = data.Sum(item => item.TongTien.Value); // Calculate total amount using LINQ Sum() method
            ViewBag.TongTien = tongtien;

            return View(data.ToList());
        }
    }
}