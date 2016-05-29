using IamHungry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace IamHungry.Controllers
{
    public class JelaController : Controller
    {

        FDBContext fdb = new FDBContext();
        // GET: Jela
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UrediMeni()
        {
            string rest = Session["UserId"].ToString();
            var meni = from m in fdb.Meni where m.MeniId == rest select m;
            return View(meni);
        }

        public ActionResult IzmijeniMeni(string idM, string idJ)
        {
            Meni meni = new Meni();
            meni.MeniId = idM;
            meni.JelaId = idJ;
            return View(meni);
        }

        [HttpPost]
        public ActionResult IzmijeniMeni(Meni meni)
        {
            var m = fdb.Meni.Find(meni.MeniId, meni.JelaId);
            if (ModelState.IsValid)
            {
                if (meni.cijena != null)
                {
                    m.cijena = meni.cijena;
                }
                if (meni.ImeJela != null)
                {
                    m.ImeJela = meni.ImeJela;
                }
                if (meni.OpisJela != null)
                {
                    m.OpisJela = meni.OpisJela;
                }
                fdb.SaveChanges();
                ViewBag.DIDIZMIJENIMENI = "Uspjesno ste izmijenili jelo";
                return RedirectToAction("UrediMeni");
            }
            ViewBag.FAILIZMIJENIMENI = "Greska pri izmijeni jela!";
            return View(meni);
        }


        public ActionResult UrediDnevniMeni()
        {
            string dm = Session["UserId"].ToString();
            var dmeni = from m in fdb.DnevniMeni where m.MeniId == dm select m;
            return View(dmeni);
        }

        public ActionResult IzmijeniDnevniMeni(string idM, string idJ, string idD)
        {
            DnevniMeni dmeni = new DnevniMeni();
            dmeni.MeniId = idM;
            dmeni.JelaId = idJ;
            dmeni.dan = idD;
            return View(dmeni);
        }

        [HttpPost]
        public ActionResult IzmijeniDnevniMeni(DnevniMeni dm)
        {
            var dmeni = fdb.DnevniMeni.Find(dm.MeniId, dm.JelaId, dm.dan);
            if (ModelState.IsValid)
            {
                if(dm.cijena != null)
                {
                    dmeni.cijena = dm.cijena;
                }
                if(dm.dan != null)
                {
                    dmeni.dan = dm.dan;
                }
                if(dm.ImeJela != null)
                {
                    dmeni.ImeJela = dm.ImeJela;
                }
                if(dm.OpisJela != null)
                {
                    dmeni.OpisJela = dm.OpisJela;
                }
                ViewBag.SUCCIZMIJENADM = "Uspjesno ste promijenili jelo.";
                fdb.SaveChanges();
                return RedirectToAction("UrediDnevniMeni");
            }
            ViewBag.FAILIZMIJENADM = "Greska pri promijeni jela!";
            return View(dm);
        }

        public ActionResult IzbrisiDnevniMeni(string idM, string idJ, string idD)
        {
            var dm = fdb.DnevniMeni.Find(idM, idJ, idD);
            fdb.DnevniMeni.Remove(dm);
            fdb.SaveChanges();
            return RedirectToAction("UrediDnevniMeni");
        }

        public ActionResult NovoMeniJelo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NovoMeniJelo(Meni jelo)
        {
            if(Session["UserId"] != null)
            {
                jelo.MeniId = Session["UserId"].ToString();
            }
            if (ModelState.IsValid)
            {
                
                Random rnd = new Random();
                int r = rnd.Next(0, 9999);
                jelo.JelaId = "" + jelo.ImeJela.Substring(0, 3) + "" + r.ToString();
                fdb.Meni.Add(jelo);
                fdb.SaveChanges();
                return RedirectToAction("UrediMeni", "Jela");
            }
            return View();
        }

        public ActionResult IzbrisiMeni(string idM, string idJ)
        {
            var jelo = fdb.Meni.Find(idM, idJ);
            fdb.Meni.Remove(jelo);
            fdb.SaveChanges();
            return RedirectToAction("UrediMeni");
        }

        public ActionResult NovoDnevniMeniJelo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NovoDnevniMeniJelo(DnevniMeni jelo)
        {
            if (Session["UserId"] != null)
            {
                jelo.MeniId = Session["UserId"].ToString();
            }
            if (ModelState.IsValid)
            {
                Random rnd = new Random();
                int r = rnd.Next(0, 9999);
                jelo.JelaId = "" + jelo.ImeJela.Substring(0, 3) + "" + r.ToString();
                fdb.DnevniMeni.Add(jelo);
                fdb.SaveChanges();
                return RedirectToAction("UrediDnevniMeni", "Jela");
            }
            return View();
        }

       

        private void MeniDropDownList(object selectedMeni = null)
        {

            ViewBag.MeniJela = new SelectList(fdb.Meni, "JelaId", "ImeJela", selectedMeni);
        }

        private void DnevniMeniDropDownList(object selectedDnevniMeni = null)
        {
            ViewBag.DnevniMeni = new SelectList(fdb.DnevniMeni, "JelaId", "ImeJela", selectedDnevniMeni);
        }

        private void DanDropDownList()
        {
            List<string> dan = new List<string>();
            dan.Add("Ponedjeljak");
            dan.Add("Utorak");
            dan.Add("Srijeda");
            dan.Add("Četvrtak");
            dan.Add("Petak");
            dan.Add("Subota");
            dan.Add("Nedjelja");
         
            ViewBag.DanLista = new SelectList(dan);
        }

 
    }
}