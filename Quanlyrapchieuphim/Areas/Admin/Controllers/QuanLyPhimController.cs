using Quanlyrapchieuphim.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quanlyrapchieuphim.Areas.Admin.Controllers
{
    public class QuanLyPhimController : Controller
    {
        // GET: Admin/QuanLyPhim
        private QLRapChieuPhimEntities db = new QLRapChieuPhimEntities();
        public ActionResult Index()
        {
            var data = db.Phims.ToList();
            return View(data);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Phim phim, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Thêm thông tin phim vào cơ sở dữ liệu

                    db.Phims.Add(phim);
                    db.SaveChanges();

                    // Kiểm tra xem có tệp ảnh được tải lên không
                    if (file != null && file.ContentLength > 0)
                    {
                        // Lưu tệp ảnh vào thư mục trên server
                        string fileName = Path.GetFileName(file.FileName);
                        string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                        file.SaveAs(path);

                        // Lưu tên tệp ảnh vào đối tượng phim
                        phim.TenAnh = fileName;
                        db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    }

                    // Chuyển hướng đến trang danh sách phim sau khi đã tạo thành công
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Lỗi: " + ex.Message;
                    return View();
                }
            }

            // Nếu dữ liệu không hợp lệ, trả về lại view với dữ liệu nhập vào
            return View(phim);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = db.Phims.SingleOrDefault(n => n.IDPhim == id);
            if (data == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(data);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Phim phim)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = db.Phims.SingleOrDefault(n => n.IDPhim == id);
                    if (data == null)
                    {
                        return HttpNotFound();
                    }
                    data.TenPhim = phim.TenPhim;
                    data.TenAnh = phim.TenAnh;
                    data.TheLoai = phim.TheLoai;
                    data.ThoiLuong = phim.ThoiLuong;
                    data.KhoiChieu = phim.KhoiChieu;
                    data.NamSanXuat = phim.NamSanXuat;
                    data.DaoDien = phim.DaoDien;
                    data.DienVien = phim.DienVien;
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
            return View(phim);
        }




        public class PhimController : Controller
        {
            private QLRapChieuPhimEntities db = new QLRapChieuPhimEntities();


            public ActionResult Delete(int id)
            {
                var phim = db.Phims.Find(id);
                if (phim == null)
                {
                    return HttpNotFound();
                }
                return View(phim);
            }

            [HttpPost, ActionName("Delete")]
            public ActionResult DeleteConfirm(int id)
            {
                var data = db.Phims.SingleOrDefault(n => n.IDPhim == id);
                if (data == null)
                {
                    Response.StatusCode = 404;
                    return null; // Handle the case when the record is not found
                }

                try
                {
                    db.Phims.Remove(data);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the changes: " + ex.Message);
                    return View();
                }

                return RedirectToAction("Index");
            }
            public ActionResult test()
            {
                var data = db.Phims;
                return View(data);
            }

        }
    }
}