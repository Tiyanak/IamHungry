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
                r.KucniBroj = restoran.KucniBroj;
                r.KvartId = restoran.KvartId;
                r.PostBroj = restoran.PostBroj;
                r.Passw = restoran.Passw;
                r.Ulica = restoran.Ulica;
                r.email = restoran.email;
                r.MeniId = restoran.RestId;
                r.Telefon = restoran.Telefon;
                r.Mobitel = restoran.Mobitel;

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

        public ActionResult IzmijeniDostavu(string idR, string idK, int idG)
        {
            Dostava d = new Dostava();
            d.RestId = idR;
            d.PostBroj = idG;
            d.KvartId = idK;
            return View(d);
        }

        [HttpPost]
        public ActionResult IzmijeniDostavu(Dostava d)
        {
            var dost = fdb.Dostava.Find(d.RestId, d.KvartId, d.PostBroj);
            if (ModelState.IsValid)
            {
                if(d.vrijeme != null)
                {
                    dost.vrijeme = d.vrijeme;
                }
                fdb.SaveChanges();
                return RedirectToAction("UrediDostavu");
            }
            ViewBag.FAILAZDOSTAVA = "Greska pri izmijeni dostave!";
            return View(d);
        }

        public ActionResult IzbrisiDostavu(string idR, string idK, int idG)
        {
            var d = fdb.Dostava.Find(idR, idK, idG);
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
            }
            if (ModelState.IsValid)
            {
                var rad = fdb.RadnoVrijeme.Find(rv.RestId, rv.Dan);
                rad.StatusRada = rv.StatusRada;
                rad.VrijemeOd = rv.VrijemeOd;
                rad.VrijemeDo = rv.VrijemeDo;
                fdb.SaveChanges();
                ViewBag.RadnoVrijeme = "Azurirali ste radno vrijeme za " + rad.Dan.ToString() + " na: " + rad.VrijemeOd.ToString() + " - " + rad.VrijemeDo.ToString();
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