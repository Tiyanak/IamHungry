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
            ViewBag.gradLista = new SelectList(fdb.Grad, "PostBroj", "ImeGrada");
            ViewBag.kvartLista = new SelectList(fdb.Kvart, "KvartId", "ImeKvarta");
            ViewBag.zupLista = new SelectList(fdb.Zupanija, "ZupId", "ZupIme");

            var r = from k in fdb.Restoran select k;

            List<Restoran> racR = new List<Restoran>(10);
            foreach(var i in r)
            {
                Rating rest = fdb.Rating.Find(i.RestId);

                int? brojglasaca = rest.jedna + rest.dvije + rest.tri + rest.cetiri + rest.pet;
                if (brojglasaca == 0 || brojglasaca == null)
                {
                    brojglasaca = 1;
                }
                int? zbrojocjena = rest.jedna + rest.dvije * 2 + rest.tri * 3 + rest.cetiri * 4 + rest.pet * 5;
                double? rating = (double) zbrojocjena/brojglasaca;
                double rat = (double)rating;
                rat = System.Math.Round(rat, 2);
                rest.ocjena = rat;
            }
            fdb.SaveChanges();
            foreach (var i in r)
            {
                if(racR.Count < 10)
                {
                    racR.Add(i);
                }else
                {
                    double? min = 5.0;
                    Restoran minRest = new Restoran();
                    foreach(var k in racR)
                    {
                        if(k.Rating.ocjena < min)
                        {
                            min = k.Rating.ocjena;
                            minRest = k;
                        }
                    }
                    if(minRest.Rating.ocjena >= i.Rating.ocjena)
                    {
                        continue;
                    }else
                    {
                        racR.Remove(minRest);
                        racR.Add(i);
                    }
                }
            }
            
            return View(racR);
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
        
        [HttpGet]
        public ActionResult SearchRestorans()
        {
            var r = from k in fdb.Restoran select k;
            return View(r);
        }

        [HttpPost]
        public ActionResult SearchRestorans(Restoran r)
        {
            return View(r);
        }

        public ActionResult Meni(string idMenija)
        {

            var meni = from m in fdb.JelaUMeniju where m.RestId == idMenija select m;
            var rest = fdb.Restoran.Single(u => u.RestId == idMenija);
            ViewBag.ImeTrenRestorana = rest.ImeRest.ToString();
            return View(meni);
        }

        public ActionResult DnevniMeni(string idMenija)
        { 
            var dnevnimeni = from d in fdb.JelaUDnevnomMeniju where d.RestId == idMenija select d;
            return View(dnevnimeni);
        }

        public ActionResult NarudbaMeni(string idM, string idJ, decimal? cijenaJ)
        {
            if(Session["NarPos"] != null)
            {
                ViewBag.NarPos = Session["NarPos"];
                Session["NarPos"] = null;
            }
            GradDropDownList();
            KvardDropDownList();
            ViewBag.ncijena = cijenaJ;
            ViewBag.nidM = idM;
            ViewBag.nidJ = idJ;
            Narudzba nar = new Narudzba();
            nar.RestId = idM;
            nar.ImeJela = idJ;
            nar.cijena = cijenaJ;
            return View(nar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> NarudbaMeni(Narudzba nar)
        {
            if (ModelState.IsValid)
            {
                int zadnjaNar = fdb.Narudzba.Max(p => p.NarId);
                nar.NarId = zadnjaNar + 1;
                fdb.Narudzba.Add(nar);
                fdb.SaveChanges();

                if (nar.email == null)
                {
                    nar.email = "korisnikNemaEmail@noEmail.com";
                }
                if(nar.ImeKvarta == null)
                {
                    nar.ImeKvarta = "";
                }
                if(nar.Telefon == null)
                {
                    nar.Telefon = "";
                }

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


                var body = "<p>Narucitelj: {0} </p><p>E-mail: {1}</p><p>Adresa: {2}</p><p>Telefon: {3}</p><p>Ime jela: {4}</p><p>Cijena: {5}</p><p>Vrijeme narudbe: {6}</p><p>Količina: {7}";
                var message = new MailMessage();
                message.To.Add(new MailAddress(primatelj));
                message.From = new MailAddress(posiljatelj);
                message.Subject = "Narudba - " + nar.NarId;
                message.Body = string.Format(body, imeNarucitelja, posiljatelj, adresa, nar.Telefon.ToString(), nar.ImeJela.ToString(), nar.cijena.ToString(), nar.vrijeme.ToString(), nar.kolicina);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "i_farszky@hotmail.com",
                        Password = "Vrbovec22"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp-mail.outlook.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    Session["RestRat"] = nar.RestId.ToString();
                    Session["NarId"] = nar.NarId.ToString();
                    return RedirectToAction("NarudbaPoslano");
                }
            }
            ViewBag.FailSendNarudba = "Narudba nije poslana, krivo ste unijeli podatke.";
            return View();
        }

        public ActionResult NarudbaPoslano()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NarudbaPoslano(string submit)
        {
            String restoran = "";
            if(Session["RestRat"] != null)
            {
                restoran = Session["RestRat"].ToString();
                Session["RestRat"] = null;
                Session["NarId"] = null;
            }
            string button = submit;
            var rat = fdb.Rating.Find(restoran);
            int oc = 0;
            switch (button)
            {
                case "1":
                    oc = Int32.Parse(rat.jedna.ToString());
                    oc = oc + 1;
                    rat.jedna = oc;
                    break;
                case "2":
                    oc = Int32.Parse(rat.dvije.ToString());
                    oc = oc + 1;
                    rat.dvije = oc;
                    break;
                case "3":
                    oc = Int32.Parse(rat.tri.ToString());
                    oc = oc + 1;
                    rat.tri = oc;
                    break;
                case "4":
                    oc = Int32.Parse(rat.cetiri.ToString());
                    oc = oc + 1;
                    rat.cetiri = oc;
                    break;
                case "5":
                    oc = Int32.Parse(rat.pet.ToString());
                    oc = oc + 1;
                    rat.pet = oc;
                    break;
                default:
                    break;
            }
            fdb.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult NarudbaDnevniMeni(string idDJ)
        {
            GradDropDownList();
            KvardDropDownList();
            Narudzba nar = new Narudzba();
            var jelo = fdb.JelaUDnevnomMeniju.Find(idDJ);
            nar.RestId = jelo.RestId;
            nar.ImeJela = jelo.ImeJela;
            nar.cijena = jelo.cjena;
            return View(nar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> NarudbaDnevniMeni(Narudzba nar)
        {
            if (ModelState.IsValid)
            {
                int zadnjaNar = fdb.Narudzba.Max(p => p.NarId);
                nar.NarId = zadnjaNar + 1;
                fdb.Narudzba.Add(nar);
                fdb.SaveChanges();

                if (nar.email == null)
                {
                    nar.email = "korisnikNemaEmail@noEmail.com";
                }
                if (nar.ImeKvarta == null)
                {
                    nar.ImeKvarta = "";
                }
                if (nar.Telefon == null)
                {
                    nar.Telefon = "";
                }

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


                var body = "<p>Narucitelj: {0} </p><p>E-mail: {1}</p><p>Adresa: {2}</p><p>Telefon: {3}</p><p>Ime jela: {4}</p><p>Cijena: {5}</p><p>Vrijeme narudbe: {6}</p><p>Količina: {7}";
                var message = new MailMessage();
                message.To.Add(new MailAddress(primatelj));
                message.From = new MailAddress(posiljatelj);
                message.Subject = "Narudba - " + nar.NarId;
                message.Body = string.Format(body, imeNarucitelja, posiljatelj, adresa, nar.Telefon.ToString(), nar.ImeJela.ToString(), nar.cijena.ToString(), nar.vrijeme.ToString(), nar.kolicina);
                message.IsBodyHtml = true;
           
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "i_farszky@hotmail.com",
                        Password = "Vrbovec22"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp-mail.outlook.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    Session["RestRat"] = nar.RestId.ToString();
                    Session["NarId"] = nar.NarId.ToString();
                    return RedirectToAction("NarudbaPoslano");
                }
            }
            ViewBag.FailSendNarudba = "Narudba nije poslana, krivo ste unijeli podatke.";
            return View();
        }

        public ActionResult RestInfo(string idMenija)
        {
            if(Session["UserId"] != null)
            {
                idMenija = Session["UserId"].ToString();
            }
            Restoran r = fdb.Restoran.Single(t => t.RestId == idMenija);

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