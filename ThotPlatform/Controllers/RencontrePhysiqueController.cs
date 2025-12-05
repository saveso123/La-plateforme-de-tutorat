using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ThotPlatform.Models;
using ThotPlatform.ViewModels;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des rencontres physiques
    /// </summary>
    [Authorize]
    public class RencontrePhysiqueController : Controller
    {
        private readonly ThotDbContext _context;

        public RencontrePhysiqueController()
        {
            _context = new ThotDbContext();
        }

        // GET: RencontrePhysique/Index
        public ActionResult Index()
        {
            if (Session["UserType"]?.ToString() == "Etudiant")
            {
                var userId = (int)Session["UserId"];
                var rencontres = _context.RencontresPhysiques
                    .Include(r => r.Tuteur)
                    .Include(r => r.Domaine)
                    .Where(r => r.EtudiantId == userId)
                    .OrderByDescending(r => r.DateCreation)
                    .Select(r => new RencontrePhysiqueViewModel
                    {
                        RencontreId = r.RencontreId,
                        DateHeure = r.DateHeure,
                        DureeHeures = r.DureeHeures,
                        Lieu = r.Lieu,
                        Tarif = r.Tarif,
                        TarifPreferentiel = r.TarifPreferentiel,
                        Statut = r.Statut,
                        NoteEtudiant = r.NoteEtudiant,
                        Commentaire = r.Commentaire,
                        DateCreation = r.DateCreation,
                        TuteurPrenom = r.Tuteur.Prenom,
                        TuteurNom = r.Tuteur.Nom,
                        TuteurEmail = r.Tuteur.Email,
                        DomaineNom = r.Domaine.Nom
                    })
                    .ToList();

                return View("IndexEtudiant", rencontres);
            }
            else if (Session["UserType"]?.ToString() == "Tuteur")
            {
                var userId = (int)Session["UserId"];
                var rencontres = _context.RencontresPhysiques
                    .Include(r => r.Etudiant)
                    .Include(r => r.Domaine)
                    .Where(r => r.TuteurId == userId)
                    .OrderByDescending(r => r.DateCreation)
                    .Select(r => new RencontrePhysiqueViewModel
                    {
                        RencontreId = r.RencontreId,
                        DateHeure = r.DateHeure,
                        DureeHeures = r.DureeHeures,
                        Lieu = r.Lieu,
                        Tarif = r.Tarif,
                        TarifPreferentiel = r.TarifPreferentiel,
                        Statut = r.Statut,
                        NoteEtudiant = r.NoteEtudiant,
                        Commentaire = r.Commentaire,
                        DateCreation = r.DateCreation,
                        EtudiantPrenom = r.Etudiant.Prenom,
                        EtudiantNom = r.Etudiant.Nom,
                        EtudiantEmail = r.Etudiant.Email,
                        DomaineNom = r.Domaine.Nom
                    })
                    .ToList();

                return View("IndexTuteur", rencontres);
            }

            return RedirectToAction("Login", "Account");
        }

        // GET: RencontrePhysique/Demander
        public ActionResult Demander()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            // Charger les domaines disponibles
            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom");

            var rencontre = new RencontrePhysique
            {
                DateHeure = DateTime.Now.AddDays(1),
                DureeHeures = 1
            };

            return View(rencontre);
        }

        // POST: RencontrePhysique/Demander
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Demander(RencontrePhysique rencontre)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var etudiant = _context.Etudiants.Find(userId);

            if (etudiant == null)
            {
                TempData["ErrorMessage"] = "Etudiant introuvable";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Trouver un tuteur disponible dans ce domaine
                    var tuteurDisponible = _context.TuteurDomaines
                        .Where(td => td.DomaineId == rencontre.DomaineId)
                        .Select(td => td.Tuteur)
                        .ToList()
                        .FirstOrDefault(t => t.EstDisponible);

                    if (tuteurDisponible == null)
                    {
                        TempData["ErrorMessage"] = "Aucun tuteur disponible dans ce domaine pour le moment";
                        ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom");
                        return View(rencontre);
                    }

                    // Verifier si l'etudiant est abonne
                    var dateUnMoisAgo = DateTime.Now.AddMonths(-1);
                    var estAbonne = _context.Transactions
                        .Any(t => t.EtudiantId == userId && 
                                 t.Type == TypeTransaction.AbonnementMensuel && 
                                 t.Statut == StatutTransaction.Completee &&
                                 t.DateTransaction > dateUnMoisAgo);

                    // Calculer le tarif
                    decimal tarif = CalculerTarif(etudiant.Niveau, estAbonne);

                    rencontre.EtudiantId = userId;
                    rencontre.TuteurId = tuteurDisponible.UtilisateurId;
                    rencontre.Tarif = tarif;
                    rencontre.TarifPreferentiel = estAbonne;
                    rencontre.Statut = StatutRencontre.EnAttente;

                    _context.RencontresPhysiques.Add(rencontre);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = $"Demande de rencontre envoyee ! Tarif : {rencontre.CoutTotal:C}";
                    return RedirectToAction("Details", new { id = rencontre.RencontreId });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Erreur : " + ex.Message;
                }
            }

            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom");
            return View(rencontre);
        }

        // GET: RencontrePhysique/Details/5
        public ActionResult Details(int id)
        {
            var rencontre = _context.RencontresPhysiques
                .Include(r => r.Etudiant)
                .Include(r => r.Tuteur)
                .Include(r => r.Domaine)
                .FirstOrDefault(r => r.RencontreId == id);

            if (rencontre == null)
            {
                return HttpNotFound();
            }

            // Verifier l'acces
            var userId = (int)Session["UserId"];
            if (rencontre.EtudiantId != userId && rencontre.TuteurId != userId)
            {
                TempData["ErrorMessage"] = "Acces non autorise";
                return RedirectToAction("Index");
            }

            // Projeter dans le ViewModel pour eviter les problemes de proxy
            var viewModel = new RencontrePhysiqueViewModel
            {
                RencontreId = rencontre.RencontreId,
                DateHeure = rencontre.DateHeure,
                DureeHeures = rencontre.DureeHeures,
                Lieu = rencontre.Lieu,
                Description = rencontre.Description,
                Tarif = rencontre.Tarif,
                TarifPreferentiel = rencontre.TarifPreferentiel,
                Statut = rencontre.Statut,
                NoteEtudiant = rencontre.NoteEtudiant,
                Commentaire = rencontre.Commentaire,
                DateCreation = rencontre.DateCreation,
                EtudiantPrenom = rencontre.Etudiant.Prenom,
                EtudiantNom = rencontre.Etudiant.Nom,
                EtudiantEmail = rencontre.Etudiant.Email,
                TuteurPrenom = rencontre.Tuteur.Prenom,
                TuteurNom = rencontre.Tuteur.Nom,
                TuteurEmail = rencontre.Tuteur.Email,
                DomaineNom = rencontre.Domaine.Nom
            };

            return View(viewModel);
        }

        // POST: RencontrePhysique/Confirmer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirmer(int id)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return Json(new { success = false, message = "Non autorise" });
            }

            var userId = (int)Session["UserId"];
            var rencontre = _context.RencontresPhysiques
                .FirstOrDefault(r => r.RencontreId == id && r.TuteurId == userId);

            if (rencontre == null)
            {
                return Json(new { success = false, message = "Rencontre introuvable" });
            }

            rencontre.Statut = StatutRencontre.Confirmee;
            rencontre.DateConfirmation = DateTime.Now;
            _context.SaveChanges();

            return Json(new { success = true, message = "Rencontre confirmee !" });
        }

        // POST: RencontrePhysique/Refuser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Refuser(int id, string raison)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return Json(new { success = false, message = "Non autorise" });
            }

            var userId = (int)Session["UserId"];
            var rencontre = _context.RencontresPhysiques
                .FirstOrDefault(r => r.RencontreId == id && r.TuteurId == userId);

            if (rencontre == null)
            {
                return Json(new { success = false, message = "Rencontre introuvable" });
            }

            rencontre.Statut = StatutRencontre.Annulee;
            rencontre.Commentaire = raison;
            _context.SaveChanges();

            return Json(new { success = true, message = "Rencontre refusee" });
        }

        // POST: RencontrePhysique/Annuler/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Annuler(int id)
        {
            var userId = (int)Session["UserId"];
            var rencontre = _context.RencontresPhysiques
                .FirstOrDefault(r => r.RencontreId == id && r.EtudiantId == userId);

            if (rencontre == null)
            {
                return Json(new { success = false, message = "Rencontre introuvable" });
            }

            if (rencontre.Statut == StatutRencontre.EnCours || rencontre.Statut == StatutRencontre.Terminee)
            {
                return Json(new { success = false, message = "Impossible d'annuler une rencontre en cours ou terminee" });
            }

            rencontre.Statut = StatutRencontre.Annulee;
            _context.SaveChanges();

            return Json(new { success = true, message = "Rencontre annulee" });
        }

        // POST: RencontrePhysique/Noter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Noter(int id, int note, string commentaire)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return Json(new { success = false, message = "Non autorise" });
            }

            var userId = (int)Session["UserId"];
            var rencontre = _context.RencontresPhysiques
                .Include(r => r.Tuteur)
                .FirstOrDefault(r => r.RencontreId == id && r.EtudiantId == userId);

            if (rencontre == null)
            {
                return Json(new { success = false, message = "Rencontre introuvable" });
            }

            if (rencontre.Statut != StatutRencontre.Terminee)
            {
                return Json(new { success = false, message = "La rencontre doit etre terminee pour etre notee" });
            }

            rencontre.NoteEtudiant = note;
            rencontre.Commentaire = commentaire;

            // Mettre a jour la note moyenne du tuteur
            var tuteur = rencontre.Tuteur;
            var notesRencontres = _context.RencontresPhysiques
                .Where(r => r.TuteurId == tuteur.UtilisateurId && r.NoteEtudiant.HasValue)
                .Select(r => r.NoteEtudiant.Value)
                .ToList();

            if (notesRencontres.Any())
            {
                tuteur.NoteMoyenne = (decimal)notesRencontres.Average();
            }

            _context.SaveChanges();

            return Json(new { success = true, message = "Merci pour votre evaluation !" });
        }

        // GET: RencontrePhysique/RechercherTuteurs
        public ActionResult RechercherTuteurs(int domaineId)
        {
            var tuteurs = _context.TuteurDomaines
                .Include(td => td.Tuteur)
                .Include(td => td.Domaine)
                .Where(td => td.DomaineId == domaineId)
                .ToList()
                .Where(td => td.Tuteur.EstDisponible)
                .Select(td => new
                {
                    tuteurId = td.TuteurId,
                    nom = td.Tuteur.Prenom + " " + td.Tuteur.Nom,
                    note = td.Tuteur.NoteMoyenne
                })
                .ToList();

            return Json(tuteurs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Calcule le tarif selon le niveau et l'abonnement
        /// </summary>
        private decimal CalculerTarif(NiveauScolaire niveau, bool estAbonne)
        {
            decimal tarifBase;

            switch (niveau)
            {
                case NiveauScolaire.Primaire:
                    tarifBase = 25m;
                    break;
                case NiveauScolaire.Secondaire:
                    tarifBase = 35m;
                    break;
                case NiveauScolaire.Collegial:
                    tarifBase = 45m;
                    break;
                default:
                    tarifBase = 30m;
                    break;
            }

            // Reduction de 20% pour les abonnes
            if (estAbonne)
            {
                tarifBase *= 0.8m;
            }

            return tarifBase;
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

