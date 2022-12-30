using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;
using Ictshop.Pay;

namespace Ictshop.Controllers
{
    public class DonhangsController : Controller
    {
        private Qlbanhang db = new Qlbanhang();

        // GET: Donhangs
        // Hiển thị danh sách đơn hàng
        public ActionResult Index()
        {
            //Kiểm tra đang đăng nhập
            if (Session["use"] == null || Session["use"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            Nguoidung kh = (Nguoidung)Session["use"];
            int maND = kh.MaNguoiDung;
            var donhangs = db.Donhangs.Include(d => d.Nguoidung).Where(d=>d.MaNguoidung == maND);
            return View(donhangs.ToList());

        }

        // GET: Donhangs/Details/5
        //Hiển thị chi tiết đơn hàng
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donhang donhang = db.Donhangs.Find(id);
            var chitiet = db.Chitietdonhangs.Include(d => d.Sanpham).Where(d=> d.Madon == id).ToList();
            if (donhang == null)
            {
                return HttpNotFound();
            }
            return View(chitiet);
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public ActionResult DetailsConfirmed(int id)
        {
            Donhang donhang = db.Donhangs.Find(id);
            Chitietdonhang chitietdonhang = db.Chitietdonhangs.SingleOrDefault(m => m.Madon == id);
            db.Chitietdonhangs.Remove(chitietdonhang);
            db.Donhangs.Remove(donhang);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
