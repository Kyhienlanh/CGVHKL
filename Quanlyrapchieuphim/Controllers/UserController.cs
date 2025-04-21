using Quanlyrapchieuphim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quanlyrapchieuphim.Controllers
{
    public class UserController : Controller
    {
        private QLRapChieuPhimEntities db = new QLRapChieuPhimEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TaiKhoan()
        {
            var email = Session["TaiKhoan"] as string;
            if (email == null)
            {
                return RedirectToAction("Login", "Home"); // Chưa đăng nhập, chuyển hướng đến login
            }

            var data = db.NguoiDungs.FirstOrDefault(d => d.Email == email);
            return View(data);
        }

        public ActionResult Lichsugiaodich()
        {
            var email = Session["TaiKhoan"] as string;
            if (email == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int? IDNguoiDung = db.NguoiDungs.Where(s => s.Email == email)
                                           .Select(s => s.IDNguoiDung)
                                           .FirstOrDefault();

            if (IDNguoiDung == null)
            {
                return View(new List<HoaDon>());
            }

            var data = db.Ve1.Where(s => s.TrangThai == "Da Thanh Toan" && s.IDNguoiDung == IDNguoiDung)
                             .Select(s => s.IDVe)
                             .ToList();

            if (!data.Any())
            {
                return View(new List<HoaDon>());
            }

            var invoiceData = db.HoaDons.Where(s => data.Contains(s.IDVe ?? 0))
                                        .ToList();

            return View(invoiceData);
        }
    }
}