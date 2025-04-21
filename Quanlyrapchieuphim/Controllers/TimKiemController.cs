using Quanlyrapchieuphim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quanlyrapchieuphim.Controllers
{
    public class TimKiemController : Controller
    {
        // GET: TimKiem
        private QLRapChieuPhimEntities db = new QLRapChieuPhimEntities();
        public List<Phim> Search(string query)
        {
            query = query.ToLower();

       
            int? idPhim = db.Phims
                             .Where(p => p.TenPhim.ToLower() == query)
                             .Select(p => p.IDPhim)
                             .FirstOrDefault();

        
            if (idPhim != null)
            {
              
                var suatchieu = db.SuatChieux.Where(p => p.IDPhim == idPhim).ToList();

              
                if (suatchieu.Any())
                {
                 
                    return db.Phims
                            .Where(p => p.TenPhim.ToLower().Contains(query))
                            .ToList();
                }
            }

          
            return new List<Phim>();
        }
        public List<Phim> SearchTheoLoai(string genre)
        {
            return db.Phims
                     .Where(p => p.TheLoai.ToLower().Contains(genre.ToLower()))
                     .ToList();
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TimKiemTheoTenPhim(string tenPhim)
        {

            var searchResult = Search(tenPhim);
            return View(searchResult);
        }
     
        [HttpGet]
        public ActionResult GetGenres()
        {
            var genres = new string[] { "Hành động", "Tình cảm", "Drama", "Kinh dị", "Hài hước" };
            return Json(genres, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TimKiemTheoTheLoai(string genre)
        {
            var searchResult = SearchTheoLoai(genre);
            return View(searchResult);
        }
    }
}