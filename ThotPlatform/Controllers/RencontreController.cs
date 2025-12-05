using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ThotPlatform.Models;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des rencontres physiques
    /// </summary>
    [Authorize]
    public class RencontreController : Controller
    {
        private readonly ThotDbContext _context;

        public RencontreController()
        {
            _context = new ThotDbContext();
        }

        // GET: Rencontre/MesRencontres
        public ActionResult MesRencontres()
        {
            var userId = (int)Session["UserId"];
            var userType = Session["UserType"]?.ToString();

            var rencontres = userType == "Etudiant"
                ? _context.RencontresPhysiques
                    .Include(r => r.Tuteur)
                    .Include(r => r.Domaine)
                    .Where(r => r.EtudiantId == userId)
                    .OrderByDescending(r => r.DateHeure)
                    .ToList()
                : _context.RencontresPhysiques
                    .Include(r => r.Etudiant)
                    .Include(r => r.Domaine)
                    .Where(r => r.TuteurId == userId)
                    .OrderByDescending(r => r.DateHeure)
                    .ToList();

            return View(rencontres);
        }

        // GET: Rencontre/Reserver
        public ActionResult Reserver()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                TempData["ErrorMessage"] = "Seuls les etudiants peuvent reserver une rencontre";
                return RedirectToAction("Index", "Home");
            }

            var userId = (int)Session["UserId"];
            var etudiant = _context.Etudiants.Find(userId);

            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom");
            ViewBag.Tuteurs = new SelectList(_context.Tuteurs.Where(t => t.EstActif), "UtilisateurId", "Prenom");
            ViewBag.TarifPreferentiel = etudiant.AbonnementActif;

            return View();
        }

        // POST: Rencontre/Reserver
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reserver(RencontrePhysique rencontre)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = (int)Session["UserId"];
            var etudiant = _context.Etudiants.Find(userId);

            if (ModelState.IsValid)
            {
                try
                {
                    rencontre.EtudiantId = userId;
                    rencontre.DateCreation = DateTime.Now;
                    rencontre.Statut = StatutRencontre.EnAttente;

                    // Appliquer le tarif preferentiel si abonne
                    if (etudiant.AbonnementActif)
                    {
                        rencontre.TarifPreferentiel = true;
                        rencontre.Tarif = rencontre.Tarif * 0.8m; // 20% de reduction
                    }

                    _context.RencontresPhysiques.Add(rencontre);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Rencontre reservee avec succes ! Le tuteur doit confirmer.";
                    return RedirectToAction("MesRencontres");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la reservation : " + ex.Message);
                }
            }

            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom", rencontre.DomaineId);
            ViewBag.Tuteurs = new SelectList(_context.Tuteurs.Where(t => t.EstActif), "UtilisateurId", "Prenom", rencontre.TuteurId);
            ViewBag.TarifPreferentiel = etudiant.AbonnementActif;

            return View(rencontre);
        }

        // GET: Rencontre/MesRencontres
        public ActionResult MesRencontres()
        {
            var userId = (int)Session["UserId"];
            var userType = Session["UserType"]?.ToString();

            var rencontres = userType == "Etudiant"
                ? _context.RencontresPhysiques
                    .Include(r => r.Tuteur)
                    .Include(r => r.Domaine)
                    .Where(r => r.EtudiantId == userId)
                    .OrderByDescending(r => r.DateHeure)
                    .ToList()
                : _context.RencontresPhysiques
                    .Include(r => r.Etudiant)
                    .Include(r => r.Domaine)
                    .Where(r => r.TuteurId == userId)
                    .OrderByDescending(r => r.DateHeure)
                    .ToList();

            return View(rencontres);
        }

        // POST: Rencontre/Confirmer
        [HttpPost]
        public ActionResult Confirmer(int id)
        {
            var userId = (int)Session["UserId"];
            var rencontre = _context.RencontresPhysiques.FirstOrDefault(r => r.RencontreId == id && r.TuteurId == userId);

            if (rencontre == null)
                return HttpNotFound();

            rencontre.Statut = StatutRencontre.Confirmee;
            rencontre.DateConfirmation = DateTime.Now;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Rencontre confirmee";
            return RedirectToAction("MesRencontres");
        }

        // POST: Rencontre/Annuler
        [HttpPost]
        public ActionResult Annuler(int id)
        {
            var userId = (int)Session["UserId"];
            var rencontre = _context.RencontresPhysiques.FirstOrDefault(r => r.RencontreId == id && (r.TuteurId == userId || r.EtudiantId == userId));

            if (rencontre == null)
                return HttpNotFound();

            rencontre.Statut = StatutRencontre.Annulee;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Rencontre annulee";
            return RedirectToAction("MesRencontres");
        }

        // POST: Rencontre/Terminer
        [HttpPost]
        public ActionResult Terminer(int id)
        {
            var userId = (int)Session["UserId"];
            var rencontre = _context.RencontresPhysiques.FirstOrDefault(r => r.RencontreId == id && r.TuteurId == userId);

            if (rencontre == null)
                return HttpNotFound();

            rencontre.Statut = StatutRencontre.Terminee;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Rencontre terminee";
            return RedirectToAction("MesRencontres");
        }

        // POST: Rencontre/Evaluer
        [HttpPost]
        public ActionResult Evaluer(int id, int note, string commentaire)
        {
            var userId = (int)Session["UserId"];
            var rencontre = _context.RencontresPhysiques.FirstOrDefault(r => r.RencontreId == id && r.EtudiantId == userId);

            if (rencontre == null)
                return HttpNotFound();

            rencontre.NoteEtudiant = note;
            rencontre.Commentaire = commentaire;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "evaluation enregistree";
            return RedirectToAction("MesRencontres");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
