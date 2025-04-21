using PayPal.Api;
using Quanlyrapchieuphim.Helper;
using Quanlyrapchieuphim.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services.Description;
using static Quanlyrapchieuphim.Helper.sendmail;
using System.Data.Entity;


namespace Quanlyrapchieuphim.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private QLRapChieuPhimEntities db = new QLRapChieuPhimEntities();


        public ActionResult Index()
        {
            var phimList = db.Phims.ToList();
            return View(phimList);
        }
        public ActionResult PhimSapChieu() 
        {
            var now = DateTime.Now;
            var data = from s in db.Phims
                       where s.KhoiChieu > now
                       select s;
            return View(data.ToList());
        }
        public ActionResult PhimDangChieu()
        {
            var now = DateTime.Now;
            var data = from s in db.Phims
                       where s.KhoiChieu < now
                       select s;
            return View(data.ToList());

           
        }
        public ActionResult ChiTietPhim(int id)
        {
            Session["id"] = id;
           
            var data = from s in db.Phims
                       where s.IDPhim == id
                       select s;

            return View(data.ToList());
        }
        public ActionResult LichChieu()
        {
            var now = DateTime.Now.Date; // Chỉ lấy ngày, bỏ qua giờ

            if (Session["id"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int id = (int)Session["id"];
            ViewBag.Id = id;

            // Truy vấn dữ liệu
            var data = db.SuatChieux
                         .Where(s => s.IDPhim == id && DbFunctions.TruncateTime(s.NgayChieu) >= now)
                         .OrderBy(s => s.NgayChieu)
                         .ToList();

            if (!data.Any())
            {
                ViewBag.Message = "Không có suất chiếu nào khả dụng.";
            }

            return View(data);
        }


        public ActionResult ChoNgoi(int id)
        {

            if (Session["TaiKhoan"] == null)
            {
                Session["ReturnUrl"] = Request.UrlReferrer.AbsoluteUri;
                return RedirectToAction("Index", "Login");
            }
            Session["IDSuatChieu"] = id;
            var data = from s in db.ChoNgoi__SuatChieu
                       where s.IDSuatChieu == id
                       select s;
            return View(data.ToList());
        }
        public ActionResult HoaDon()
        {
            var data = from s in db.HoaDons
                       where s.IDHoaDon == 2
                       select s;
            return View(data);
        }
        
        public ActionResult test(string selectedPlace)
        {
            if(selectedPlace !=null)
            {
                Session["ChoNgoi"] = selectedPlace;
               
               return RedirectToAction("DichVu");
            }
            ViewBag.ChoNgoi = Session["ChoNgoi"] as string;
            return View();
        }
        [HttpGet]
        public ActionResult DichVu()
        {
            var data = db.DichVus;
            return View(data);
        }
        [HttpPost]
        public ActionResult DichVu(IEnumerable<int> selectedServices)
        {
           
            if (selectedServices != null && selectedServices.Any())
            {
                Session["SelectedServices"] = selectedServices.ToList();

                return RedirectToAction("TinhTien");
            }

            return View();  
        }

        public ActionResult ifdv()
        {
            List<int> selectedServices = Session["SelectedServices"] as List<int>;

            
            ViewBag.iddv = selectedServices;
            return View();
        }
        public ActionResult Xemlsgd()
        {
            if (Session["TaiKhoan"] == null)
            {
                Session["ReturnUrl"] = Request.UrlReferrer?.AbsoluteUri;
                return RedirectToAction("Index", "Login");
            }
            string mail = Session["TaiKhoan"] as string;
            int user = db.NguoiDungs.FirstOrDefault(s => s.Email == mail).IDNguoiDung;
            var data=from s in db.Ve1
                     where s.IDNguoiDung == user
                     select s;
            return View(data.ToList());
        }
        public ActionResult TinhTien()
        {
            List<int> selectedServices = Session["SelectedServices"] as List<int> ?? new List<int>();
            int idPhim = Session["id"] as int? ?? 0;
            int idSuatChieu = Session["IDSuatChieu"] as int? ?? 0;
            string Email = Session["TaiKhoan"] as string ?? "";
            int idChoNgoi;
            double tongtien = 0;
            int idve = 0;
            List<DichVu> selectedServiceObjects = new List<DichVu>();

            if (Session["ChoNgoi"] != null && int.TryParse(Session["ChoNgoi"].ToString(), out idChoNgoi))
            {
                int IDNguoiDung = db.NguoiDungs
                    .Where(s => s.Email == Email)
                    .Select(s => s.IDNguoiDung)
                    .FirstOrDefault();

                var ve = db.Ve1.FirstOrDefault(u => u.IDChoNgoi_SuatChieu == idChoNgoi);
                if (ve != null)
                {
                    ve.IDNguoiDung = IDNguoiDung;
                    tongtien += ve.GiaVe ?? 0;
                    db.SaveChanges();
                    idve = ve.IDVe;
                }
                

                var ghe = db.ChoNgoi__SuatChieu.FirstOrDefault(s => s.ChoNgoi_SuatChieu == idChoNgoi);
                ViewData["Ghe"] = ghe;
            }

            foreach (int serviceId in selectedServices)
            {
                var service = db.DichVus.FirstOrDefault(s => s.IDDichVu == serviceId);
                if (service != null)
                {
                    selectedServiceObjects.Add(service);
                    tongtien += service.GiaTien ?? 0;

                    if (idve > 0) // Chỉ thêm nếu IDVe hợp lệ
                    {
                        DV_VE a = new DV_VE { IDVe = idve, IDDichVu = serviceId };
                        db.DV_VE.Add(a);
                    }
                }
            }

            db.SaveChanges(); // Lưu thay đổi sau khi kiểm tra

            Session["TongTien"] = tongtien;
            ViewBag.message = Session["TongTien"];

            Phim phim = db.Phims.FirstOrDefault(s => s.IDPhim == idPhim);
            ViewData["Phim"] = phim ?? new Phim();
            ViewData["SelectedServices"] = selectedServiceObjects;

            return View();
        }



        public ActionResult FailureView()
        {
            return View();
        }
        public ActionResult SuccessView()
        {
           
            // Return the SuccessView view if none of the above conditions are met
            return View();
        }

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Home/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }

            //on successful payment, show success page to user.  
          


              
               

          
            int idChoNgoi = 0;
            if (Session["ChoNgoi"] != null && int.TryParse(Session["ChoNgoi"].ToString(), out idChoNgoi))
            {
                // Retrieve the ticket with the IDChoNgoi_SuatChieu matching the ChoNgoi session value
                var ve = db.Ve1.FirstOrDefault(s => s.IDChoNgoi_SuatChieu == idChoNgoi);
                var ChoNgoi_SuatChieu= db.ChoNgoi__SuatChieu.FirstOrDefault(s => s.ChoNgoi_SuatChieu == idChoNgoi);
                if (ve != null)
                {

                    // Update the ticket status to "Da Thanh Toan" (Paid)
                    ve.TrangThai = "Da Thanh Toan";
                    ChoNgoi_SuatChieu.trangthai= "Da Duoc Dat";
                    HoaDon hoaDon = new HoaDon();
                    hoaDon.IDVe = ve.IDVe;
                    hoaDon.TenHoaDon = "Ve xem phim";
                    if (Session["TongTien"] != null && float.TryParse(Session["TongTien"].ToString(), out float tongTien))
                    {
                        hoaDon.TongTien = tongTien;
                    }
                    else
                    {
                        hoaDon.TongTien = 0;
                    }
                    DateTime ngayHienTai = DateTime.Now;
                    hoaDon.NgayTao = ngayHienTai;
                    db.HoaDons.Add(hoaDon);
                    db.SaveChanges(); // Save changes to the database
                                      // Format thông tin hóa đơn thành một chuỗi văn bản
                    string hoaDonInfo = $"Tên hóa đơn: {hoaDon.TenHoaDon}\n" +
                                        $"ID Vé: {hoaDon.IDVe}\n" +
                                        $"Tổng tiền: {hoaDon.TongTien}\n" +
                                        $"Ngày tạo: {hoaDon.NgayTao}";

                    // Gửi email chứa thông tin hóa đơn
                    string recipientEmail = Session["TaiKhoan"] as string;
                    // Thay thế bằng địa chỉ email của người nhận
                    string subject = "Thông tin hóa đơn";
                    string body = hoaDonInfo;
                    string attachFile = null; // Nếu bạn muốn đính kèm file, hãy cung cấp đường dẫn đến file ở đây

                    // Gọi phương thức SendMail.SendMail123 để gửi email
                    bool emailSent = SendMail.SendMail123(recipientEmail, subject, body, attachFile);

                    // Redirect the user to the Index action of the Login controller
                    return RedirectToAction("SuccessView");
                }
                else
                {
                    // Redirect the user to the Index action if the ticket is not found
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("test");
            }


            return View("SuccessView");
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            int TotalAmount = 0; 
            if (Session["TongTien"] != null && int.TryParse(Session["TongTien"].ToString(), out int amount1))
            {
                int amount11 = amount1 / 24000;
                TotalAmount = amount11;
            }
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
          

            itemList.items.Add(new Item()
            {
                name = "Vé xem phim",
                currency = "USD",
                price = TotalAmount.ToString(),
                quantity = "1",
                sku = "vé"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = TotalAmount.ToString()
            };
            var total = (Convert.ToDouble(details.tax) + Convert.ToDouble(details.shipping) + Convert.ToDouble(details.subtotal)).ToString();
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = total.ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }







    }

}