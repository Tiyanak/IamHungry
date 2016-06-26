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
            var meni = from m in fdb.JelaUMeniju where m.RestId == rest select m;
            return View(meni);
        }

        public ActionResult IzmijeniMeni(string idM, string idJ)
        {
            JelaUMeniju meni = new JelaUMeniju();
            meni.RestId = idM;
            meni.JeloId = idJ;
            return View(meni);
        }

        [HttpPost]
        public ActionResult IzmijeniMeni(JelaUMeniju meni)
        {
            var m = fdb.JelaUMeniju.Find(meni.RestId, meni.JeloId);
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
            var dmeni = from m in fdb.JelaUDnevnomMeniju where m.RestId == dm select m;
            return View(dmeni);
        }

        public ActionResult IzmijeniDnevniMeni(string idRDM)
        {
            JelaUDnevnomMeniju dmeni = new JelaUDnevnomMeniju();
            var jelo = fdb.JelaUDnevnomMeniju.Find(idRDM);
            dmeni.RestDnevnoJeloId = jelo.RestDnevnoJeloId.ToString();
            return View(dmeni);
        }

        [HttpPost]
        public ActionResult IzmijeniDnevniMeni(JelaUDnevnomMeniju dm)
        {
            var dmeni = fdb.JelaUDnevnomMeniju.Find(dm.RestDnevnoJeloId);
            if (ModelState.IsValid)
            {
                if(dm.cjena != null)
                {
                    dmeni.cjena = dm.cjena;
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

        public ActionResult IzbrisiDnevniMeni(string idRDM)
        {
            var dm = fdb.JelaUDnevnomMeniju.Find(idRDM);
            fdb.JelaUDnevnomMeniju.Remove(dm);
            fdb.SaveChanges();
            return RedirectToAction("UrediDnevniMeni");
        }

        public ActionResult NovoMeniJelo()
        {
            JelaUMeniju jelo = new JelaUMeniju();
            return View(jelo);
        }

        [HttpPost]
        public ActionResult NovoMeniJelo(JelaUMeniju jelo)
        {
            if(Session["UserId"] != null)
            {
                jelo.RestId = Session["UserId"].ToString();
            }
            if (ModelState.IsValid)
            {
                
                Random rnd = new Random();
                int r = rnd.Next(0, 9999);
                jelo.JeloId = "" + jelo.ImeJela.Substring(0, 5) + "" + r.ToString();
                fdb.JelaUMeniju.Add(jelo);
                fdb.SaveChanges();
                return RedirectToAction("UrediMeni", "Jela");
            }
            return View();
        }

        public ActionResult IzbrisiMeni(string idM, string idJ)
        {
            var jelo = fdb.JelaUMeniju.Find(idM, idJ);
            fdb.JelaUMeniju.Remove(jelo);
            fdb.SaveChanges();
            return RedirectToAction("UrediMeni");
        }

        public ActionResult NovoDnevniMeniJelo()
        {
            JelaUDnevnomMeniju jelo = new JelaUDnevnomMeniju();
            return View(jelo);
        }

        [HttpPost]
        public ActionResult NovoDnevniMeniJelo(JelaUDnevnomMeniju jelo)
        {
            if (Session["UserId"] != null)
            {
                jelo.RestId = Session["UserId"].ToString();
                jelo.RestDnevnoJeloId = jelo.RestId.Substring(0, 4).ToString() + jelo.dan.ToString() + jelo.ImeJela.Substring(0, 5).ToString();
            }
            if (ModelState.IsValid)
            {
                fdb.JelaUDnevnomMeniju.Add(jelo);
                fdb.SaveChanges();
                return RedirectToAction("UrediDnevniMeni", "Jela");
            }
            return View();
        }

       

        private void MeniDropDownList(object selectedMeni = null)
        {

            ViewBag.MeniJela = new SelectList(fdb.JelaUMeniju, "JelaId", "ImeJela", selectedMeni);
        }

        private void DnevniMeniDropDownList(object selectedDnevniMeni = null)
        {
            ViewBag.DnevniMeni = new SelectList(fdb.JelaUDnevnomMeniju, "RestDnevnoJeloId", "ImeJela", selectedDnevniMeni);
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