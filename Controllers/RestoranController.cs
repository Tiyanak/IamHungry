using IamHungry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IamHungry.Controllers
{
    public class RestoranController : Controller
    {

        FDBContext fdb = new FDBContext();
        // GET: Restoran
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UrediRestoran()
        {
            GradDropDownList();
            KvardDropDownList();
            return View();
        }

        [HttpPost]
        public ActionResult UrediRestoran(Restoran restoran)
        {
            if (Session["UserId"] != null)
            {
                restoran.RestId = Session["UserId"].ToString();
            }
            if (ModelState.IsValid)
            {
                Restoran r = fdb.Restoran.Find(restoran.RestId);

                r.ImeRest = restoran.ImeRest;
                if(restoran.KucniBroj != null)
                {
                    r.KucniBroj = restoran.KucniBroj;
                }
               
                r.KvartId = restoran.KvartId;
                r.PostBroj = restoran.PostBroj;

                if(restoran.Passw != null)
                {
                    r.Passw = restoran.Passw;
                }
               
                if(restoran.Ulica != null)
                {
                    r.Ulica = restoran.Ulica;
                }
                
                if(restoran.email != null)
                {
                    r.email = restoran.email;
                }
                
                if(restoran.Telefon != null)
                {
                    r.Telefon = restoran.Telefon;
                }
                
                if(restoran.Mobitel != null)
                {
                    r.Mobitel = restoran.Mobitel;
                }
               
                if(restoran.VlasnikId != null)
                {
                    r.VlasnikId = restoran.VlasnikId;
                }
                

                fdb.SaveChanges();
               
                return RedirectToAction("Index", "Home");
            }

            ViewBag.EditRestMessage = "Niste uspjeli promijeniti podatke o restoranu!"; 
            return View();
        }

        public ActionResult UrediDostavu()
        {
            string rest = Session["UserId"].ToString();
            var dost = from d in fdb.Dostava where d.RestId == rest select d;
            return View(dost);
        }

        public ActionResult IzmijeniDostavu(string idD)
        {
            Dostava d = new Dostava();
            d.DostavaId = idD;
            d.RestId = Session["UserId"].ToString();
            ViewBag.GradD = new SelectList(fdb.Grad, "PostBroj", "ImeGrada");
            ViewBag.KvartD = new SelectList(fdb.Kvart, "KvartId", "ImeKvarta");
            return View(d);
        }

        [HttpPost]
        public ActionResult IzmijeniDostavu(Dostava d)
        {
            var dost = fdb.Dostava.Find(d.DostavaId);
            if (ModelState.IsValid)
            {
                if(d.vrijeme != null)
                {
                    dost.vrijeme = d.vrijeme;
                    dost.PostBroj = d.PostBroj;
                    dost.KvartId = d.KvartId;
                }
                fdb.SaveChanges();
                return RedirectToAction("UrediDostavu");
            }
            ViewBag.FAILAZDOSTAVA = "Greska pri izmijeni dostave!";
            return View(d);
        }

        public ActionResult IzbrisiDostavu(string idD)
        {
            var d = fdb.Dostava.Find(idD);
            fdb.Dostava.Remove(d);
            fdb.SaveChanges();
            return RedirectToAction("UrediDostavu");
        }

        public ActionResult DodajDostavu()
        {
            GradDropDownList();
            KvardDropDownList();

            Dostava d = new Dostava();
            d.RestId = Session["UserId"].ToString();
            return View(d);
        }

        [HttpPost]
        public ActionResult DodajDostavu(Dostava d)
        {
            d.DostavaId = d.RestId.Substring(0, 5).ToString() + d.KvartId.Substring(0, 5).ToString();
            if (ModelState.IsValid)
            {
                fdb.Dostava.Add(d);
                fdb.SaveChanges();
                return RedirectToAction("UrediDostavu");
            }
            ViewBag.FAILDOSTAVA = "Greska pri unosu nove dostave!";
            return View(d);
        }

        public ActionResult RadnoVrijeme()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RadnoVrijeme(RadnoVrijeme rv)
        {
            if (Session["UserId"] != null)
            {
                rv.RestId = Session["UserId"].ToString();
                rv.RestRadId = rv.RestId.Substring(0, 5).ToString() + rv.dan.ToString();
            }
            if (ModelState.IsValid)
            {
                fdb.RadnoVrijeme.Add(rv);
                fdb.SaveChanges();
                ViewBag.RadnoVrijeme = "Azurirali ste radno vrijeme za " + rv.dan.ToString() + " na: " + rv.VrijemeOd.ToString() + " - " + rv.VrijemeDo.ToString();
                return View();
            }
            return View();
        }

        private void GradDropDownList(object selectedGrad = null)
        {
            ViewBag.Grad2 = new SelectList(fdb.Grad, "PostBroj", "ImeGrada", selectedGrad);
        }
        private void KvardDropDownList(object selectedKvart = null)
        {
            ViewBag.Kvart = new SelectList(fdb.Kvart, "KvartId", "ImeKvarta", selectedKvart);
        }
    }
}