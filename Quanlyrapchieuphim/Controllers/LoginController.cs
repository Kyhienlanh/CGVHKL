using Quanlyrapchieuphim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quanlyrapchieuphim.Helper;
using static Quanlyrapchieuphim.Helper.sendmail;
namespace Quanlyrapchieuphim.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        private QLRapChieuPhimEntities db = new QLRapChieuPhimEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult test() 
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {

            var user = db.NguoiDungs.FirstOrDefault(u => u.Email == Username && u.HashPass == Password && u.XacNhanEmail == "true");

            if (user != null)
            {
                if (user.Quyen =="1")
                {
                    Session["TaiKhoan"] = user.Email;
                    return RedirectToAction("index", "QuanLyPhim", new { area = "Admin" });

                }
                else
                {
                    Session["TaiKhoan"] = user.Email;
                    string a = Session["ReturnUrl"]as string;
                    if (a == null)
                    {
                        return RedirectToAction("index", "Home");
                    }
                    return Redirect(a);
                }

                
            }
            else
            {
           
                return RedirectToAction("Index");
            }
        }
        public ActionResult delete()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("index","Home");
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection form)
        {
            Random random = new Random();
            int randomNumber = random.Next(100000, 999999);
            string Username = form["Username"];
            string MatKhau = form["MatKhau"];
            string NhapLaiMatKhau = form["NhapLaiMatKhau"];
            string sdt = form["sdt"];
            string Email = form["email"];
            NguoiDung nguoiDung = new NguoiDung
            {
                TenNguoiDung = Username,
                SoDienThoai = sdt,
                Email = Email,
                UserName = Username,
                HashPass = MatKhau,
                MaXacNhan = randomNumber.ToString(),
            };
            db.NguoiDungs.Add(nguoiDung);
            db.SaveChanges();

           
            Session["IDNguoiDung"] = nguoiDung.IDNguoiDung;

            SendMail.SendMail123(Email, "Mã xác nhận ", randomNumber.ToString(), null);

            Session["MaXacNhan"] = randomNumber.ToString();
            return View();
        }

        [HttpGet]
        public ActionResult confirmEmail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult confirmEmail(FormCollection form)
        {
            string MaXacNhan = form["MaXacNhan"];
            string data = Session["MaXacNhan"] as string;
          
            if (Session["IDNguoiDung"] is int id)
            {
                var a = db.NguoiDungs.FirstOrDefault(u => u.IDNguoiDung == id);
                if (MaXacNhan == data)
                {
                    a.XacNhanEmail = "true";
                    db.SaveChanges();

                    return RedirectToAction("index","home");
                }
                else
                {
                 
                    ViewBag.Message = "Mã xác nhận không đúng. Vui lòng kiểm tra lại.";
                    return View();
                }
            }
            else
            {
                // Handle the case where IDNguoiDung is not set or cannot be parsed to an integer
                ViewBag.Message = "Lỗi xác nhận ID người dùng.";
                return View();
            }
        }


    }
}