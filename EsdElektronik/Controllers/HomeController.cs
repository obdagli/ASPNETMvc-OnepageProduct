using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EsdElektronik.Models.Entity;
namespace EsdElektronik.Controllers
{
    public class HomeController : Controller
    {
        esdelektronikEntities esd = new esdelektronikEntities();
        public ActionResult Index(int? id)
        {
            var values2 = esd.tbl_kategori.ToList();
            ViewBag.Categories = values2;
            if (id != null)
            {
                var x = esd.tbl_urunler.Where(d => d.UrunKategori == id).ToList();
                return View(x);
            }           
      
            var values = esd.tbl_urunler.ToList();
            return View(values);
        }
        public ActionResult Kats()
        {
            var values = esd.tbl_kategori.ToList();
            
            return View(values);
        }

  
    }
}