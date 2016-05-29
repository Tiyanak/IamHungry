using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IamHungry.Models;
using System.Data.Entity;

namespace IamHungry.Controllers
{
    public class AccountController : Controller
    {
        FDBContext fdb = new FDBContext();
        public ActionResult RegisterRestorana()
        {
            GradDropDownList();
            KvardDropDownList();
            return View();
        }

        [HttpPost]
        public ActionResult RegisterRestorana(Restoran restoran)
        {
            if (ModelState.IsValid)
            {
                Restoran r = new Restoran();
                r.RestId = restoran.RestId;
                r.ImeRest = restoran.ImeRest;
                r.PostBroj = restoran.PostBroj;
                r.Ulica = restoran.Ulica;
                r.email = restoran.email;
                r.Passw = restoran.Passw;
                r.KucniBroj = restoran.KucniBroj;
                r.Telefon = restoran.Telefon;
                r.Mobitel = restoran.Mobitel;
                r.KvartId = restoran.KvartId;
                r.MeniId = restoran.RestId;

                Rating rat = new Rating();
                rat.RestId = restoran.RestId;
                rat.jedna = 0;
                rat.dvije = 0;
                rat.tri = 0;
                rat.cetiri = 0;
                rat.pet = 0;

                List<string> dani = new List<string>(){ "Ponedjeljak", "Utorak", "Srijeda", "Cetvrtak", "Petak", "Subota", "Nedjelja" };
                
                foreach(string s in dani)
                {
                    RadnoVrijeme rv = new RadnoVrijeme();
                    rv.RestId = restoran.RestId;
                    rv.StatusRada = "Ne radimo";
                    rv.Dan = s;
                    fdb.RadnoVrijeme.Add(rv);
                }

                
                fdb.Restoran.Add(r);
                fdb.Rating.Add(rat);
                fdb.SaveChangesAsync();
                return RedirectToAction("RegisterVlasnika");
            }
            ModelState.Clear();

            ViewBag.restMessage = "Krivo ste unijeli podatke.";
            return View();
        }

        public ActionResult RegisterVlasnika()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterVlasnika(Vlasnik vlasnik)
        {
            if (ModelState.IsValid)
            {
                if (fdb.Restoran.Single(u => u.RestId == vlasnik.RestId) != null)
                {
                    fdb.Vlasnik.Add(vlasnik);
                    fdb.SaveChangesAsync();
                    ViewBag.VlasMessage = "Uspješno ste registrirali svoj restoran.";
                    return View();
                }

                ViewBag.FailMessage = "Ne postoji restoran sa navedenim OIB-om!";
                ModelState.Clear();
            }
            return View();
        }

        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(Restoran user)
        {
        
            if(fdb.Restoran.Single(u => u.ImeRest == user.ImeRest && u.Passw == user.Passw) != null)
            {
                var usr = fdb.Restoran.Single(u => u.ImeRest == user.ImeRest && u.Passw == user.Passw);
                Session["UserId"] = usr.RestId.ToString();
                Session["Username"] = usr.ImeRest.ToString();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Korisnicko ime ili password je pogresno unesen.");
            }
            
            return View("/Home/Index");
        }

        // GET: /Account/LoggedIn
        public ActionResult LoggedIn()
        {
            if(Session["UserId"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Home/Index");
            }
        }

        public ActionResult LogOut()
        {
            if(Session["UserId"] != null)
            {
                Session["UserId"] = null;
                Session["UserName"] = null;
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        private void GradDropDownList(object selectedGrad = null)
        {
            ViewBag.Grad = new SelectList(fdb.Grad, "PostBroj", "ImeGrada", selectedGrad);
        }
        private void KvardDropDownList(object selectedKvart = null)
        {
            ViewBag.Kvart = new SelectList(fdb.Kvart, "KvartId", "ImeKvarta", selectedKvart);
        }
    }
}