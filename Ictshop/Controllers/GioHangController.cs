using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Net;
using Ictshop.Pay;

namespace Ictshop.Controllers
{
    public class GioHangController : Controller
    {
        Qlbanhang db = new Qlbanhang();
        // GET: GioHang
      
        //Lấy giỏ hàng 
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì mình tiến hành khởi tao list giỏ hàng (sessionGioHang)
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        //Thêm giỏ hàng
        public ActionResult ThemGioHang(int iMasp, string strURL)
        {
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMasp);
            if ( sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy ra session giỏ hàng
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sp này đã tồn tại trong session[giohang] chưa
            GioHang sanpham = lstGioHang.Find(n => n.iMasp == iMasp);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMasp);
                //Add sản phẩm mới thêm vào list
                lstGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }
        //Cập nhật giỏ hàng 
        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            //Kiểm tra masp
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp== iMaSP);
            //Nếu get sai masp thì sẽ trả về trang lỗi 404
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sp có tồn tại trong session["GioHang"]
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoLuong"].ToString());

            }
            return RedirectToAction("GioHang");
        }
        //Xóa giỏ hàng
        public ActionResult XoaGioHang(int iMaSP)
        {
            //Kiểm tra masp
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp== iMaSP);
            //Nếu get sai masp thì sẽ trả về trang lỗi 404
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMasp == iMaSP);

            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang");
        }
        //Xây dựng trang giỏ hàng
        public ActionResult GioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }
        //Tính tổng số lượng và tổng tiền
        //Tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        //Tính tổng thành tiền
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.ThanhTien);
            }
            return dTongTien;

        }
        //tạo partial giỏ hàng
        public ActionResult GioHangPartial()
        {
            if (TongSoLuong() == 0)
            {
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        //Xây dựng 1 view cho người dùng chỉnh sửa giỏ hàng
        public ActionResult SuaGioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);

        }
        public ActionResult Voucher()
        {
        
            return View();

        }


        #region // Mới hoàn thiện
        //Xây dựng chức năng đặt hàng
        [HttpPost]
        public ActionResult DatHang()
        {
            //Kiểm tra đăng đăng nhập
            if (Session["use"] == null || Session["use"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            //Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            //Thêm đơn hàng
            Donhang ddh = new Donhang();
            Nguoidung kh = (Nguoidung)Session["use"];
            List<GioHang> gh = LayGioHang();
            ddh.MaNguoidung = kh.MaNguoiDung;
            ddh.Ngaydat = DateTime.Now;
            ddh.Tinhtrang = "dsadasd";
            Console.WriteLine(ddh);
            db.Donhangs.Add(ddh);
            db.SaveChanges();
            //Thêm chi tiết đơn hàng
            foreach (var item in gh)
            {
                Chitietdonhang ctDH = new Chitietdonhang();
                decimal thanhtien =  item.iSoLuong * (decimal) item.dDonGia;
                ctDH.Madon = ddh.Madon;
                ctDH.Masp = item.iMasp;
                ctDH.Soluong = item.iSoLuong;
                ctDH.Dongia = (decimal)item.dDonGia;
                ctDH.Thanhtien = (decimal) thanhtien;
                db.Chitietdonhangs.Add(ctDH);
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Donhangs");
        }
        //Thanh Toan
        public ActionResult ThanhToan()
        {
            if (Session["use"] == null || Session["use"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }


            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        Donhang ddh = new Donhang();
                        Nguoidung kh = (Nguoidung)Session["use"];
                        List<GioHang> gh = LayGioHang();
                        ddh.MaNguoidung = kh.MaNguoiDung;
                        ddh.Ngaydat = DateTime.Now;
                        ddh.Tinhtrang = "Đã thanh toán";
                        Console.WriteLine(ddh);
                        db.Donhangs.Add(ddh);
                        db.SaveChanges();
                        //Thêm chi tiết đơn hàng
                        foreach (var item in gh)
                        {
                            Chitietdonhang ctDH = new Chitietdonhang();
                            decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                            ctDH.Madon = ddh.Madon;
                            ctDH.Masp = item.iMasp;
                            ctDH.Soluong = item.iSoLuong;
                            ctDH.Dongia = (decimal)item.dDonGia;
                            ctDH.Thanhtien = (decimal)thanhtien;
                            db.Chitietdonhangs.Add(ctDH);
                        }
                        db.SaveChanges();
                        return RedirectToAction("Index", "Donhangs");
                        ViewBag.Message = "Thanh toán thành công hóa đơn ";
                    }
                    else
                    {
                        //Thanh toán thành công
                        Donhang ddh = new Donhang();
                        Nguoidung kh = (Nguoidung)Session["use"];
                        List<GioHang> gh = LayGioHang();
                        ddh.MaNguoidung = kh.MaNguoiDung;
                        ddh.Ngaydat = DateTime.Now;
                        ddh.Tinhtrang = "Chưa thanh toán";
                        Console.WriteLine(ddh);
                        db.Donhangs.Add(ddh);
                        db.SaveChanges();
                        //Thêm chi tiết đơn hàng
                        foreach (var item in gh)
                        {
                            Chitietdonhang ctDH = new Chitietdonhang();
                            decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                            ctDH.Madon = ddh.Madon;
                            ctDH.Masp = item.iMasp;
                            ctDH.Soluong = item.iSoLuong;
                            ctDH.Dongia = (decimal)item.dDonGia;
                            ctDH.Thanhtien = (decimal)thanhtien;
                            db.Chitietdonhangs.Add(ctDH);
                        }
                        db.SaveChanges();
                        return RedirectToAction("Index", "Donhangs");
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                      
                    }
                }
                else
                {  //Thanh toán thành công
                    Donhang ddh = new Donhang();
                    Nguoidung kh = (Nguoidung)Session["use"];
                    List<GioHang> gh = LayGioHang();
                    ddh.MaNguoidung = kh.MaNguoiDung;
                    ddh.Ngaydat = DateTime.Now;
                    ddh.Tinhtrang = "Xãy ra lỗi";
                    Console.WriteLine(ddh);
                    db.Donhangs.Add(ddh);
                    db.SaveChanges();
                    //Thêm chi tiết đơn hàng
                    foreach (var item in gh)
                    {
                        Chitietdonhang ctDH = new Chitietdonhang();
                        decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                        ctDH.Madon = ddh.Madon;
                        ctDH.Masp = item.iMasp;
                        ctDH.Soluong = item.iSoLuong;
                        ctDH.Dongia = (decimal)item.dDonGia;
                        ctDH.Thanhtien = (decimal)thanhtien;
                        db.Chitietdonhangs.Add(ctDH);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", "Donhangs");
                  
                }
            }

            return View();

        }
        public ActionResult Payment()
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", "1000000"); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);

        }

        #endregion

    }
}