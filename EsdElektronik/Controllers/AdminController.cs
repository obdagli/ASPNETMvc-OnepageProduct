using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EsdElektronik.Models.Entity;
using ImageResizer;
using System.Data.Entity;
using EsdElektronik.Models;

namespace EsdElektronik.Controllers
{

    public class AdminController : Controller
    {
        esdelektronikEntities esd = new esdelektronikEntities();
        // GET: Admin
        public ActionResult Index()
        {

            return View();
        }
        #region Urun
        [HttpGet]
        public ActionResult Urunler()
        {
            var values = esd.tbl_urunler.Include(m => m.tbl_kategori).ToList();
            return View(values);
        }
        [HttpGet]
        public ActionResult Urunekle()
        {
            List<SelectListItem> degerler = (from i in esd.tbl_kategori.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.KategoriAd,
                                                 Value = i.KategoriId.ToString()
                                             }).ToList();
            ViewBag.query = degerler;
            return View();
        }
        [HttpPost]
        public ActionResult Urunekle(tbl_urunler urun, short UrunStok)
        {

            string fileName = Path.GetFileNameWithoutExtension(Request.Files[0].FileName);
            string extension = Path.GetExtension(Request.Files[0].FileName);
            string path = "~/Images/" + fileName + extension;
            Request.Files[0].SaveAs(Server.MapPath(path));
            urun.UrunFoto = "/EsdElektronik/Images/" + fileName + extension;

            ResizeSettings rs = new ResizeSettings
            {
                Width = 450,
                Height = 300,
                Format = "png"
            };
            ImageBuilder.Current.Build(path, path, rs);
            var test = esd.tbl_kategori.Where(m => m.KategoriId == urun.tbl_kategori.KategoriId).FirstOrDefault();
            urun.tbl_kategori = test;
            esd.tbl_urunler.Add(urun);
            tbl_stok addstok = new tbl_stok();
            addstok.UrunId = urun.id;
            addstok.StokSayi = UrunStok;
            esd.tbl_stok.Add(addstok);
            esd.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Urunsil(int id)
        {
            var find = esd.tbl_urunler.Find(id);
            var find_stok = esd.tbl_stok.First(x => x.tbl_urunler.id == id);
            esd.tbl_stok.Remove(find_stok);
            esd.tbl_urunler.Remove(find);
            esd.SaveChanges();
            return RedirectToAction("Urunler");
        }
        [HttpGet]
        public ActionResult Urunguncelle(int id)
        {
            List<SelectListItem> degerler = (from i in esd.tbl_kategori.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.KategoriAd,
                                                 Value = i.KategoriId.ToString()
                                             }).ToList();
            ViewBag.query = degerler;
            var temp = esd.tbl_urunler.Find(id);
            return View(temp);
        }
        [HttpPost]
        public ActionResult Urunguncelle(tbl_urunler objurun)
        {
            var temp = esd.tbl_urunler.Find(objurun.id);
            temp.UrunAciklama = objurun.UrunAciklama;
            temp.UrunAd = objurun.UrunAd;
            temp.UrunFiyat = objurun.UrunFiyat;
            temp.UrunKod = objurun.UrunKod;
            var tempkat = esd.tbl_kategori.Where(m => m.KategoriId == objurun.UrunKategori).FirstOrDefault();
            temp.UrunKategori = tempkat.KategoriId;
            if (objurun.UrunFoto != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(Request.Files[0].FileName);
                string extension = Path.GetExtension(Request.Files[0].FileName);
                string path = "~/Images/" + fileName + extension;
                Request.Files[0].SaveAs(Server.MapPath(path));
                temp.UrunFoto = "/EsdElektronik/Images/" + fileName + extension;

                ResizeSettings rs = new ResizeSettings
                {
                    Width = 450,
                    Height = 300,
                    Format = "png"
                };
                ImageBuilder.Current.Build(path, path, rs);
            }
            
            esd.SaveChanges();
            return RedirectToAction("Urunler", "Admin");
        }
        #endregion
        #region Kategori
        public ActionResult Kategoriler()
        {
            var value = esd.tbl_kategori.ToList();
            return View(value);
        }
        [HttpGet]
        public ActionResult Kategoriekle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Kategoriekle(tbl_kategori objk)
        {
            esd.tbl_kategori.Add(objk);
            esd.SaveChanges();
            return RedirectToAction("Kategoriekle", "Admin");
        }
        [HttpGet]
        public ActionResult Kategoriguncelle(int id)
        {
            var val = esd.tbl_kategori.Find(id);
            return View(val);
        }
        [HttpPost]
        public ActionResult Kategoriguncelle(tbl_kategori objkat)
        {
            var temp = esd.tbl_kategori.Find(objkat.KategoriId);
            temp.KategoriAd = objkat.KategoriAd;
            esd.SaveChanges();
            return RedirectToAction("Kategoriler", "Admin");
        }
        public ActionResult Kategorisil(int id)
        {
            foreach (var item in esd.tbl_urunler)
            {
                var temp = esd.tbl_urunler.Where(m => m.tbl_kategori.KategoriId == item.UrunKategori);
                if (temp != null)
                {
                    esd.tbl_urunler.Remove(item);
                }
            }
            var tempkat = esd.tbl_kategori.Find(id);
            esd.tbl_kategori.Remove(tempkat);
            return RedirectToAction("Kategoriler", "Admin");
        }
        #endregion
        #region Stok
        public ActionResult Stoklar()
        {
            var val = esd.tbl_stok.Include(x=>x.tbl_urunler);
            return View(val);
        }

        [HttpGet]
        public ActionResult Stokguncelle(int id)
        {
            var val = esd.tbl_stok.Find(id);
            return View(val);
        }        
        [HttpPost]
        public ActionResult Stokguncelle(tbl_stok objstok)
        {
            var temp = esd.tbl_stok.Find(objstok.id);
            temp.UrunId = objstok.UrunId;
            temp.StokSayi = objstok.StokSayi;
            esd.SaveChanges();
            return RedirectToAction("Stoklar", "Admin");
        }
        public ActionResult Stoksil(int id)
        {
            var find = esd.tbl_stok.Find(id);
            esd.tbl_stok.Remove(find);
            esd.SaveChanges();
            return RedirectToAction("Stoklar", "Admin");
        }
        #endregion
        #region Slider
        public ActionResult Sliderlist()
        {
            var val = esd.tbl_slider.ToList();
            return View(val);
        }
        #endregion

    }
}