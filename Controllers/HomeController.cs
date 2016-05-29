using IamHungry.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace IamHungry.Controllers
{
    public class HomeController : Controller
    {
        FDBContext fdb = new FDBContext();

        //GET: /home/pocetna
        public ActionResult Index()
        {
            
            return View();
        }

        public JsonResult getZup()
        {
            return Json(fdb.Zupanija.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getGrad(int zupid)
        {
            var gradovi = from g in fdb.Grad where g.ZupId == zupid select g;
            return Json(gradovi);
        }

        public JsonResult getKvart(int postbroj)
        {
            var kvartovi = from k in fdb.Kvart where k.PostBroj == postbroj select k;
            return Json(kvartovi);
        }
        
        // GET: /home/about
        public ActionResult About()
        {
            return View();
        }

        // GET: /home/uredivanje
        public ActionResult Uredivanje()
        {
            return View();
        }
        
        public ActionResult SearchRestorans()
        {
            var restorani = from r in fdb.Restoran select r;
            return View(restorani);
        }

        [HttpPost]
        public ActionResult SearchRestorans(Restoran r)
        {
            return View(r);
        }

        public ActionResult Meni(string idMenija)
        {

            var meni = from m in fdb.Meni where m.MeniId == idMenija select m;
            var rest = fdb.Restoran.Single(u => u.RestId == idMenija);
            ViewBag.ImeTrenRestorana = rest.ImeRest.ToString();
            return View(meni);
        }

        public ActionResult DnevniMeni(string idMenija)
        { 
            var dnevnimeni = from d in fdb.DnevniMeni where d.MeniId == idMenija select d;
            return View(dnevnimeni);
        }

        public ActionResult NarudbaMeni(string idM, string idJ, decimal? cijenaJ)
        {
            GradDropDownList();
            KvardDropDownList();
            ViewBag.ncijena = cijenaJ;
            ViewBag.nidM = idM;
            ViewBag.nidJ = idJ;
            Narudba nar = new Narudba();
            nar.RestId = idM;
            nar.ImeJela = idJ;
            nar.cijena = cijenaJ;
            return View(nar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> NarudbaMeni(Narudba nar)
        {
            if (ModelState.IsValid)
            {
                int zadnjaNar = fdb.Narudba.Max(p => p.NarId);
                nar.NarId = zadnjaNar + 1;
                fdb.Narudba.Add(nar);
                fdb.SaveChanges();
                ViewBag.Nar = "Narudba poslana, vas broj narudbe je: " + nar.NarId;

                var restoranPrimatelj = fdb.Restoran.Single(u => u.RestId == nar.RestId);
                string primatelj = restoranPrimatelj.email.ToString();
                string posiljatelj = nar.email.ToString();
                string imeNarucitelja = nar.Ime + " " + nar.Prezime;
                string adresa = "";
                if (nar.Grad.Equals(nar.ImeKvarta))
                {
                    adresa = nar.Grad + " " + nar.Ulica + " " + nar.KucniBroj;
                }
                else
                {
                    adresa = nar.Grad + " " + nar.ImeKvarta + " " + nar.Ulica + " " + nar.KucniBroj;
                }


                var body = "<p>Narucitelj: {0} </p><p>E-mail: {1}</p><p>Adresa: {2}</p><p>Telefon: {3}</p><p>Ime jela: {4}</p><p>Cijena: {5}</p><p>Vrijeme narudbe: {6}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(primatelj));
                message.From = new MailAddress(posiljatelj);
                message.Subject = "Narudba - " + nar.NarId;
                message.Body = string.Format(body, imeNarucitelja, posiljatelj, adresa, nar.Telefon.ToString(), nar.ImeJela.ToString(), nar.cijena.ToString(), nar.vrijeme.ToString());
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "i_farszky@hotmail.com",
                        Password = "Vrbovec1"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp-mail.outlook.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    return RedirectToAction("NarudbaMeni", "Home");
                }
            }
            ViewBag.FailSendNarudba = "Narudba nije poslana, kvar u sustavu";
            return View();
        }

        public ActionResult RestInfo(string idMenija)
        {
            if(Session["UserId"] != null)
            {
                idMenija = Session["UserId"].ToString();
            }
            Restoran r = fdb.Restoran.Single(t => t.RestId == idMenija);
            var rat = fdb.Rating.Single(u => u.RestId == idMenija);
            var pon = fdb.RadnoVrijeme.Single(k => k.RestId == idMenija && k.Dan.ToString() == "Ponedjeljak");
            var uto = fdb.RadnoVrijeme.Single(k => k.RestId == idMenija && k.Dan.ToString() == "Utorak");
            var sri = fdb.RadnoVrijeme.Single(k => k.RestId == idMenija && k.Dan.ToString() == "Srijeda");
            var cet = fdb.RadnoVrijeme.Single(k => k.RestId == idMenija && k.Dan.ToString() == "Cetvrtak");
            var pet = fdb.RadnoVrijeme.Single(k => k.RestId == idMenija && k.Dan.ToString() == "Petak");
            var sub = fdb.RadnoVrijeme.Single(k => k.RestId == idMenija && k.Dan.ToString() == "Subota");
            var ned = fdb.RadnoVrijeme.Single(k => k.RestId == idMenija && k.Dan.ToString() == "Nedjelja");

            int? brojglasaca = rat.jedna + rat.dvije + rat.tri + rat.cetiri + rat.pet;
            if(brojglasaca == 0 || brojglasaca == null)
            {
                brojglasaca = 1;
            }
            int? zbrojocjena = rat.jedna + rat.dvije * 2 + rat.tri * 3 + rat.cetiri * 4 + rat.pet * 5;
            double rating = (double)(zbrojocjena / brojglasaca);
            ViewBag.Rating = rating;

            if (pon.VrijemeDo != null || pon.VrijemeOd != null)
            {
                ViewBag.pon = "" + pon.VrijemeOd + " - " + pon.VrijemeDo;
            }
            else
            {
                ViewBag.pon = "Ne radimo";
            }
            if (uto.VrijemeDo != null || uto.VrijemeOd != null)
            {
                ViewBag.uto = "" + uto.VrijemeOd + " - " + uto.VrijemeDo;
            }
            else
            {
                ViewBag.uto = "Ne radimo";
            }
            if (sri.VrijemeDo != null || sri.VrijemeOd != null)
            {
                ViewBag.sri = "" + sri.VrijemeOd + " - " + sri.VrijemeDo;
            }
            else
            {
                ViewBag.sri = "Ne radimo";
            }
            if (cet.VrijemeDo != null || cet.VrijemeOd != null)
            {
                ViewBag.cet = "" + cet.VrijemeOd + " - " + cet.VrijemeDo;
            }
            else
            {
                ViewBag.cet = "Ne radimo";
            }
            if (pet.VrijemeDo != null || pet.VrijemeOd != null)
            {
                ViewBag.pet = "" + pet.VrijemeOd + " - " + pet.VrijemeDo;
            }
            else
            {
                ViewBag.pet = "Ne radimo";
            }
            if (sub.VrijemeDo != null || sub.VrijemeOd != null)
            {
                ViewBag.sub = "" + sub.VrijemeOd + " - " + sub.VrijemeDo;
            }
            else
            {
                ViewBag.sub = "Ne radimo";
            }
            if (ned.VrijemeDo != null || ned.VrijemeOd != null)
            {
                ViewBag.ned = "" + ned.VrijemeOd + " - " + ned.VrijemeDo;
            }
            else
            {
                ViewBag.ned = "Ne radimo";
            }

            return View(r);
        }


        private void GradDropDownList(object selectedGrad = null)
        {
            ViewBag.GradNar = new SelectList(fdb.Grad, "PostBroj", "ImeGrada", selectedGrad);
        }
        private void KvardDropDownList(object selectedKvart = null)
        {
            ViewBag.KvartNar = new SelectList(fdb.Kvart, "KvartId", "ImeKvarta", selectedKvart);
        }



    }
}