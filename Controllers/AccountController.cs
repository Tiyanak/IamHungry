using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IamHungry.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;

namespace IamHungry.Controllers
{
    public class AccountController : Controller
    {
        FDBContext fdb = new FDBContext();
        [HttpGet]
        public ActionResult RegisterRestorana()
        {
            if(Session["RestReg"] != null)
            {
                ViewBag.RestoranExist = Session["RestReg"].ToString();
                Session["RestReg"] = null;
            }
            GradDropDownList();
            KvardDropDownList();
            return View();
        }

        [HttpPost]
        public ActionResult RegisterRestorana(Restoran restoran)
        {
            restoran.VlasnikId = Session["VlasnikId"].ToString();


            if (ModelState.IsValidField("RestId") && ModelState.IsValidField("ImeRest") && ModelState.IsValidField("Ulica") && ModelState.IsValidField("email") && ModelState.IsValidField("Passw"))
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
                r.VlasnikId = restoran.VlasnikId;

                Session["VlasnikId"] = null;

                Rating rat = new Rating();
                rat.RestId = restoran.RestId;
                rat.jedna = 0;
                rat.dvije = 0;
                rat.tri = 0;
                rat.cetiri = 0;
                rat.pet = 0;
                rat.ocjena = 1.0;

               
                fdb.Restoran.Add(r);
                fdb.Rating.Add(rat);
                fdb.SaveChangesAsync();
                Session["VlasnikId"] = null;
                Session["RestReg"] = "Uspješno ste registrirali restoran " + restoran.ImeRest.ToString();
                return RedirectToAction("RegisterRestorana");
            }
            ModelState.Clear();

            var v = fdb.Vlasnik.Find(Session["VlasnikId"].ToString());
            fdb.Vlasnik.Remove(v);
            fdb.SaveChanges();
            Session["VlasnikId"] = null;
            ViewBag.restMessage = "Krivo ste unijeli podatke.";
            return RedirectToAction("RegisterRestorana");
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
                Session["VlasnikId"] = vlasnik.VlasnikId.ToString();
                fdb.Vlasnik.Add(vlasnik);
                fdb.SaveChanges();
               
                return RedirectToAction("RegisterRestorana");
            }

            ModelState.Clear();
            
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
           
            using(FDBContext fdb2 = new FDBContext())
            {
                var usr = fdb2.Restoran.SingleOrDefault(u => u.ImeRest == user.ImeRest && u.Passw == user.Passw);
                if(usr != null)
                {
                    Session["UserId"] = usr.RestId.ToString();
                    Session["Username"] = usr.ImeRest.ToString();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Korisničko ime ili lozinka su pogrešni!");
                }
            }
            return View();
           
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