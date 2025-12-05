using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ThotPlatform.Models;
using ThotPlatform.ViewModels;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour les fonctionnalites tuteur
    /// </summary>
    [Authorize]
    public class TuteurController : Controller
    {
        private readonly ThotDbContext _context;

        public TuteurController()
        {
            _context = new ThotDbContext();
        }

        // GET: Tuteur
        public ActionResult Index()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var tuteur = _context.Tuteurs.Find(userId);

            if (tuteur == null)
            {
                return HttpNotFound();
            }

            // Statistiques
            ViewBag.NombreReponses = _context.Reponses.Count(r => r.TuteurId == userId);
            ViewBag.NombreCours = _context.Cours.Count(c => c.TuteurId == userId);
            ViewBag.NombreSessions = _context.SessionsClavardage.Count(s => s.TuteurId == userId);
            ViewBag.NombreRencontres = _context.RencontresPhysiques.Count(r => r.TuteurId == userId);
            ViewBag.NoteMoyenne = tuteur.NoteMoyenne;

            return View(tuteur);
        }

        // GET: Tuteur/QuestionsEnAttente
        public ActionResult QuestionsEnAttente()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            
            // Recuperer les domaines d'expertise du tuteur
            var domainesIds = _context.TuteurDomaines
                .Where(td => td.TuteurId == userId)
                .Select(td => td.DomaineId)
                .ToList();

            // Questions en attente avec ViewModel pour eviter les proxies
            var questions = _context.Questions
                .Where(q => domainesIds.Contains(q.DomaineId) && 
                           (q.Statut == StatutQuestion.EnAttente || q.Statut == StatutQuestion.EnCours))
                .OrderByDescending(q => q.EstPrioritaire)
                .ThenBy(q => q.DateCreation)
                .Select(q => new QuestionEnAttenteViewModel
                {
                    QuestionId = q.QuestionId,
                    Titre = q.Titre,
                    Contenu = q.Contenu,
                    DateCreation = q.DateCreation,
                    FichierJoint = q.FichierJoint,
                    EstPrioritaire = q.EstPrioritaire,
                    Statut = q.Statut,
                    EtudiantNomComplet = q.Etudiant.Prenom + " " + q.Etudiant.Nom,
                    EtudiantNiveau = q.Etudiant.Niveau.ToString(),
                    DomaineNom = q.Domaine.Nom
                })
                .ToList();

            return View(questions);
        }

        // GET: Tuteur/MesReponses
        public ActionResult MesReponses()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var reponses = _context.Reponses
                .Include(r => r.Question)
                .Include(r => r.Question.Etudiant)
                .Where(r => r.TuteurId == userId)
                .OrderByDescending(r => r.DateCreation)
                .ToList()
                .Select(r => new ReponseViewModel
                {
                    ReponseId = r.ReponseId,
                    Contenu = r.Contenu,
                    DateCreation = r.DateCreation,
                    EstValidee = r.EstValidee,
                    Note = r.Note,
                    QuestionTitre = r.Question.Titre,
                    QuestionContenu = r.Question.Contenu,
                    QuestionDate = r.Question.DateCreation,
                    EtudiantNom = r.Question.Etudiant.Prenom + " " + r.Question.Etudiant.Nom
                })
                .ToList();

            return View(reponses);
        }

        // GET: Tuteur/MesCours
        public ActionResult MesCours()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var cours = _context.Cours
                .Where(c => c.TuteurId == userId)
                .OrderByDescending(c => c.DateCreation)
                .Select(c => new CoursViewModel
                {
                    CoursId = c.CoursId,
                    Nom = c.Nom,
                    Code = c.Code,
                    Description = c.Description,
                    DomaineNom = c.Domaine.Nom,
                    Niveau = c.Niveau,
                    NombreModules = c.NombreModules,
                    DureeEstimeeHeures = c.DureeEstimeeHeures,
                    ImageCouverture = c.ImageCouverture,
                    EstPublie = c.EstPublie,
                    DateCreation = c.DateCreation,
                    NombreInscrits = c.NombreInscrits
                })
                .ToList();

            return View(cours);
        }

        // GET: Tuteur/DetailsCours/5
        public ActionResult DetailsCours(int id)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            
            var coursData = _context.Cours
                .Where(c => c.CoursId == id && c.TuteurId == userId)
                .Select(c => new CoursDetailsViewModel
                {
                    CoursId = c.CoursId,
                    Nom = c.Nom,
                    Code = c.Code,
                    Description = c.Description,
                    DomaineNom = c.Domaine.Nom,
                    Niveau = c.Niveau,
                    NombreModules = c.NombreModules,
                    DureeEstimeeHeures = c.DureeEstimeeHeures,
                    ImageCouverture = c.ImageCouverture,
                    EstPublie = c.EstPublie,
                    DateCreation = c.DateCreation,
                    NombreInscrits = c.NombreInscrits,
                    TuteurNomComplet = c.Tuteur.Prenom + " " + c.Tuteur.Nom,
                    TuteurEmail = c.Tuteur.Email,
                    TuteurNoteMoyenne = c.Tuteur.NoteMoyenne
                })
                .FirstOrDefault();

            if (coursData == null)
            {
                return HttpNotFound();
            }

            // Charger les modules du cours
            var modules = _context.Modules
                .Where(m => m.CoursId == id)
                .OrderBy(m => m.Ordre)
                .ToList();

            ViewBag.Modules = modules;

            return View(coursData);
        }

        // GET: Tuteur/EtudiantsInscrits/5
        public ActionResult EtudiantsInscrits(int id)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            
            // Verifier que le tuteur est proprietaire du cours
            var cours = _context.Cours.FirstOrDefault(c => c.CoursId == id && c.TuteurId == userId);
            if (cours == null)
            {
                return HttpNotFound();
            }

            // Recuperer les etudiants inscrits
            var etudiants = _context.InscriptionsCours
                .Include(i => i.Etudiant)
                .Where(i => i.CoursId == id)
                .OrderBy(i => i.Etudiant.Nom)
                .Select(i => new EtudiantCoursViewModel
                {
                    UtilisateurId = i.Etudiant.UtilisateurId,
                    Prenom = i.Etudiant.Prenom,
                    Nom = i.Etudiant.Nom,
                    Email = i.Etudiant.Email,
                    Niveau = i.Etudiant.Niveau,
                    DateInscription = i.DateInscription
                })
                .ToList();

            ViewBag.CoursNom = cours.Nom;
            ViewBag.CoursId = id;
            ViewBag.NombreEtudiants = etudiants.Count();

            return View(etudiants);
        }

        // GET: Tuteur/Disponibilite
        public ActionResult Disponibilite()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var tuteur = _context.Tuteurs.Find(userId);

            if (tuteur == null)
            {
                return HttpNotFound();
            }

            return View(tuteur);
        }

        // POST: Tuteur/UpdateDisponibilite
        [HttpPost]
        public ActionResult UpdateDisponibilite(bool? estDisponible)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var tuteur = _context.Tuteurs.Find(userId);

            if (tuteur != null)
            {
                tuteur.EstDisponible = estDisponible ?? false;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Statut de disponibilite mis a jour";
            }

            return RedirectToAction("Disponibilite");
        }

        // GET: Tuteur/MesEtudiants
        public ActionResult MesEtudiants()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            
            // Recuperer tous les etudiants inscrits aux cours du tuteur
            var inscriptions = _context.InscriptionsCours
                .Include(i => i.Etudiant)
                .Include(i => i.Cours)
                .Where(i => i.Cours.TuteurId == userId)
                .ToList();

            var etudiants = inscriptions
                .GroupBy(i => i.Etudiant.UtilisateurId)
                .Select(g => new EtudiantInscritViewModel
                {
                    UtilisateurId = g.Key,
                    Prenom = g.First().Etudiant.Prenom,
                    Nom = g.First().Etudiant.Nom,
                    Email = g.First().Etudiant.Email,
                    Niveau = g.First().Etudiant.Niveau,
                    NombreCours = g.Count(),
                    DerniereInscription = g.Max(i => i.DateInscription)
                })
                .OrderBy(e => e.Nom)
                .ToList();

            ViewBag.NombreEtudiants = etudiants.Count();
            return View(etudiants);
        }

        // GET: Tuteur/Profil
        public ActionResult Profil()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var tuteur = _context.Tuteurs
                .Include(t => t.Reponses)
                .FirstOrDefault(t => t.UtilisateurId == userId);

            if (tuteur == null)
            {
                return HttpNotFound();
            }

            return View(tuteur);
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

