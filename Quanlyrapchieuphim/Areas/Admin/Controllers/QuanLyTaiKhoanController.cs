using Quanlyrapchieuphim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quanlyrapchieuphim.Areas.Admin.Controllers
{
    public class QuanLyTaiKhoanController : Controller
    {
        // GET: Admin/QuanLyTaiKhoan
        private QLRapChieuPhimEntities db= new QLRapChieuPhimEntities();
        public ActionResult Index()
        {
            var data = db.NguoiDungs;
            return View(data);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(NguoiDung user)
        {
            if (ModelState.IsValid)
            {

                db.NguoiDungs.Add(user);
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return View(user);

        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = db.NguoiDungs.SingleOrDefault(n => n.IDNguoiDung == id);
            if (data == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(data);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, NguoiDung NguoiDung)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = db.NguoiDungs.SingleOrDefault(n => n.IDNguoiDung == id);
                    if (data == null)
                    {
                        return HttpNotFound();
                    }
                    data.TenChucVu = NguoiDung.TenChucVu;
                    data.TenNguoiDung = NguoiDung.TenNguoiDung;
                    data.DiaChi = NguoiDung.DiaChi;
                    data.SoDienThoai = NguoiDung.SoDienThoai;
                    data.Email=NguoiDung.Email;
                    data.UserName=NguoiDung.UserName;
                    data.HashPass=NguoiDung.HashPass;
                    data.Quyen=NguoiDung.Quyen;
                    data.LoaiDangNhap = NguoiDung.LoaiDangNhap;

                    db.SaveChanges();
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it in a way that fits your application
                ModelState.AddModelError("", "An error occurred while saving the changes: " + ex.Message);
            }

            // If an exception occurs or ModelState is not valid, return the view with errors
            return View(NguoiDung);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {

            var data = db.NguoiDungs.SingleOrDefault(n => n.IDNguoiDung == id);
            if (data == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var data = db.NguoiDungs.SingleOrDefault(n => n.IDNguoiDung == id);
            if (data == null)
            {
                Response.StatusCode = 404;
                return null; // Handle the case when the record is not found
            }

            try
            {
                db.NguoiDungs.Remove(data);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the changes: " + ex.Message);
                return View();
            }

            return RedirectToAction("Index");
        }
    }
}