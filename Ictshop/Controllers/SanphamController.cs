using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;

namespace Ictshop.Controllers
{
    public class SanphamController : Controller
    {
        Qlbanhang db = new Qlbanhang();

        // GET: Sanpham
        public ActionResult dtiphonepartial()
        {
            var ip = db.Sanphams.Where(n=>n.Mahang==2).Take(4).ToList();
           return PartialView(ip);
        }
        public ActionResult dtsamsungpartial()
        {
            var ss = db.Sanphams.Where(n => n.Mahang == 1).Take(4).ToList();
            return PartialView(ss);
        }
        public ActionResult dtxiaomipartial()
        {
            var mi = db.Sanphams.Where(n => n.Mahang == 3).Take(4).ToList();
            return PartialView(mi);
        }
        public ActionResult ipad()
        {
            var ip = db.Sanphams.Where(n => n.Mahang == 4).Take(4).ToList();
            return PartialView(ip);
        }
        public ActionResult tabletsamsung()
        {
            var ip = db.Sanphams.Where(n => n.Mahang == 8).Take(4).ToList();
            return PartialView(ip);
        }
        public ActionResult tabletxiaomi()
        {
            var ip = db.Sanphams.Where(n => n.Mahang == 9).Take(4).ToList();
            return PartialView(ip);
        }
        public ActionResult mac()
        {
            var ip = db.Sanphams.Where(n => n.Mahang == 10).Take(4).ToList();
            return PartialView(ip);
        }
        public ActionResult dell()
        {
            var ip = db.Sanphams.Where(n => n.Mahang == 11).Take(4).ToList();
            return PartialView(ip);
        }
        public ActionResult asus()
        {
            var ip = db.Sanphams.Where(n => n.Mahang == 12).Take(4).ToList();
            return PartialView(ip);
        }
        //public ActionResult dttheohang()
        //{
        //    var mi = db.Sanphams.Where(n => n.Mahang == 5).Take(4).ToList();
        //    return PartialView(mi);
        //}
        public ActionResult xemchitiet(int Masp=0)
        {
            var chitiet = db.Sanphams.SingleOrDefault(n=>n.Masp==Masp);
            if (chitiet == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chitiet);
        }

    }

}