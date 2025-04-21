using Quanlyrapchieuphim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quanlyrapchieuphim.Areas.Admin.Controllers
{
    public class QuanLyLichChieuPhimController : Controller
    {
        // GET: Admin/QuanLyLichChieuPhim
        private QLRapChieuPhimEntities db = new QLRapChieuPhimEntities();
        public ActionResult Index()
        {
            var data = db.SuatChieux.ToList();
            return View(data);
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.IDPhongChieu = new SelectList(db.PhongChieux, "IDPhongChieu", "TenPhongChieu");
            ViewBag.IDPhim = new SelectList(db.Phims, "IDPhim", "TenPhim");

            return View();
        }

        [HttpPost]
        public ActionResult Create(SuatChieu suatChieu)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lịch
                var existingScreenings = db.SuatChieux
                    .Where(s => s.NgayChieu == suatChieu.NgayChieu &&
                                s.ThoiGianChieu == suatChieu.ThoiGianChieu &&
                                s.IDPhongChieu == suatChieu.IDPhongChieu)
                    .ToList();

                if (existingScreenings.Any())
                {
                    ViewBag.Message = "Bị trùng lịch";
                    ViewBag.IDPhongChieu = new SelectList(db.PhongChieux, "IDPhongChieu", "TenPhongChieu");
                    ViewBag.IDPhim = new SelectList(db.Phims, "IDPhim", "TenPhim");
                    return View();
                }

                // Lưu SuatChieu trước
                db.SuatChieux.Add(suatChieu);
                db.SaveChanges(); // Lưu để có IDSuatChieu

                // Thêm ghế ngồi cho suất chiếu
                List<ChoNgoi__SuatChieu> choNgoiList = new List<ChoNgoi__SuatChieu>();

                for (int i = 1; i <= 50; i++)
                {
                    ChoNgoi__SuatChieu a = new ChoNgoi__SuatChieu
                    {
                        IDSuatChieu = suatChieu.IDSuatChieu,
                        IDChoNgoi = i
                    };
                    choNgoiList.Add(a);
                }

                db.ChoNgoi__SuatChieu.AddRange(choNgoiList);
                db.SaveChanges(); // Lưu danh sách ghế

                // Lấy danh sách ghế vừa thêm để tạo vé
                List<Ve1> veList = new List<Ve1>();
                foreach (var choNgoi in choNgoiList)
                {
                    Ve1 ve = new Ve1
                    {
                        IDChoNgoi_SuatChieu = choNgoi.ChoNgoi_SuatChieu, // ID đã có sau khi lưu
                        GiaVe = 70000,
                        TenVe = suatChieu.TenSuatChieu
                    };
                    veList.Add(ve);
                }

                db.Ve1.AddRange(veList);
                db.SaveChanges(); // Lưu danh sách vé

                return RedirectToAction("Index");
            }

            return View(suatChieu);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            // Fetch the SuatChieu data corresponding to the given id
            var data = db.SuatChieux.SingleOrDefault(n => n.IDSuatChieu == id);
            // If data is not found, return a 404 error
            if (data == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Populate the ViewData with options for the "IDPhongChieu" dropdown list
            ViewData["IDPhongChieuOptions"] = (from s in db.PhongChieux
                                               select new SelectListItem
                                               {
                                                   Value = s.IDPhongChieu.ToString(),
                                                   Text = s.TenPhongChieu.ToString()
                                               }).ToList();

            // Populate the ViewData with options for the "IDPhim" dropdown list
            ViewData["IDPhimOptions"] = (from s in db.Phims
                                         select new SelectListItem
                                         {
                                             Value = s.IDPhim.ToString(),
                                             Text = s.TenPhim.ToString()
                                         }).ToList();
            // Return the view with the SuatChieu data
            return View(data);
        }


        [HttpPost]
        public ActionResult Edit(int id,SuatChieu suatChieu)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = db.SuatChieux.SingleOrDefault(n => n.IDSuatChieu == id);
                    if (data == null)
                    {
                        return HttpNotFound();
                    }
                    data.TenSuatChieu = suatChieu.TenSuatChieu;
                    data.ThoiGianChieu = suatChieu.ThoiGianChieu;
                    data.IDPhongChieu = suatChieu.IDPhongChieu;
                    data.IDPhim = suatChieu.IDPhim;
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
            return View(suatChieu);
        }



        [HttpGet]
        public ActionResult Delete(int id)
        {

            var data = db.SuatChieux.SingleOrDefault(n => n.IDSuatChieu == id);
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
           

            try
            {

                var data = db.SuatChieux.SingleOrDefault(n => n.IDSuatChieu == id);
                var data1 = (from s in db.ChoNgoi__SuatChieu
                             where s.IDSuatChieu == id
                             select s).ToList();
                foreach(var s in data1)
                {
                    db.ChoNgoi__SuatChieu.Remove(s);
                    db.SaveChanges();
                }
             
                if (data == null)
                {
                    Response.StatusCode = 404;
                    return null; // Handle the case when the record is not found
                }
                db.SuatChieux.Remove(data);
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