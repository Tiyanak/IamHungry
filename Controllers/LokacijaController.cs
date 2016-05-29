using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using IamHungry.Models;

namespace IamHungry.Controllers
{
    public class LokacijaController : Controller
    {
        FDBContext fdb = new FDBContext();
        // GET: Lokacija
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NoviGrad()
        {
            ViewBag.zupanija = new SelectList(fdb.Zupanija, "ZupId", "ZupIme");
            return View();
        }

        [HttpPost]
        public ActionResult NoviGrad(Grad grad)
        {
            if (ModelState.IsValid)
            {
                Kvart nk = new Kvart();
                nk.PostBroj = grad.PostBroj;
                nk.ImeKvarta = grad.ImeGrada;
                nk.KvartId = "" + nk.ImeKvarta.Substring(0, 3) + "" + nk.PostBroj.ToString();
                fdb.Grad.Add(grad);
                fdb.Kvart.Add(nk);
                fdb.SaveChanges();
                return RedirectToAction("RegisterRestorana", "Account");
            }
            ViewBag.FAILGRAD = "Greska prilikom unosa novog grada!";
            return View();
        }

        public ActionResult NoviKvart()
        {
            ViewBag.gradNK = new SelectList(fdb.Grad, "PostBroj", "ImeGrada");
            return View();
        }

        [HttpPost]
        public ActionResult NoviKvart(Kvart kvart)
        {
            if (ModelState.IsValid)
            {
                kvart.KvartId = "" + kvart.ImeKvarta.Substring(0, 3) + "" + kvart.PostBroj.ToString();
                fdb.Kvart.Add(kvart);
                fdb.SaveChanges();
                return RedirectToAction("RegisterRestorana", "Account");
            }
            ViewBag.FAILKVART = "Greska prilikom unosa novog kvarta!";
            return View();
        }
    }
}