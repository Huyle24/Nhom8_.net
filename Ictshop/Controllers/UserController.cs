using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ictshop.Models;
namespace Ictshop.Controllers
{
    public class UserController : Controller
    {
        Qlbanhang db = new Qlbanhang();
        // ĐĂNG KÝ
        public ActionResult Dangky()
        {
            return View();
        }


        // ĐĂNG KÝ PHƯƠNG THỨC POST
        [HttpPost]
        public ActionResult Dangky(Nguoidung nguoidung)
        {
            try
            {
                // Thêm người dùng  mới
                db.Nguoidungs.Add(nguoidung);
                // Lưu lại vào cơ sở dữ liệu
                db.SaveChanges();
                // Nếu dữ liệu đúng thì trả về trang đăng nhập
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Dangnhap");
                }
                return View("Dangky");

            }
            catch
            {
                return View();
            }
        }

        public ActionResult Dangnhap()
        {
            return View();

        }
        public ActionResult Profile()
        {
            var u = Session["use"] as Models.Nguoidung;
            // trả về trang chi tiết người dùng
            return View(u);
        }
        public ActionResult Edit()
        {  //Kiểm tra đang đăng nhập

            var u = Session["use"] as Models.Nguoidung;
            return View(u);
        }
        public ActionResult Save()
        {
            Nguoidung kh = (Nguoidung)Session["use"];
            db.SaveChanges();
            return RedirectToAction("Profile");
        }


        [HttpPost]
        public ActionResult Dangnhap(FormCollection userlog)
        {
            string userMail = userlog["userMail"].ToString();
            string password = userlog["password"].ToString();
            var islogin = db.Nguoidungs.SingleOrDefault(x => x.Email.Equals(userMail) && x.Matkhau.Equals(password));

            if (islogin != null)
            {
                if (userMail == "Admin@gmail.com")
                {
                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Admin/Home");
                }
                else
                {
                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Fail = "Đăng nhập thất bại";
                return View("Dangnhap");
            }

        }

        public ActionResult DangXuat()
        {
            Session["use"] = null;
            return RedirectToAction("Index", "Home");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNguoiDung,Hoten,Email,Dienthoai,Matkhau")] Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoidung).State = EntityState.Modified;
               db.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            return View(nguoidung);
        }
    }
}