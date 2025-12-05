using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ThotPlatform.Models;
using ThotPlatform.ViewModels;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour les fonctionnalites etudiant
    /// </summary>
    [Authorize]
    public class EtudiantController : Controller
    {
        private readonly ThotDbContext _context;

        public EtudiantController()
        {
            _context = new ThotDbContext();
        }

        // GET: Etudiant
        public ActionResult Index()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var etudiant = _context.Etudiants.Find(userId);

            if (etudiant == null)
            {
                return HttpNotFound();
            }

            // Statistiques completes pour le dashboard
            ViewBag.CoursInscrits = _context.InscriptionsCours.Count(i => i.EtudiantId == userId);
            ViewBag.QuestionsEnCours = _context.Questions.Count(q => q.EtudiantId == userId && q.Statut == StatutQuestion.EnCours);
            ViewBag.NombreQuestions = _context.Questions.Count(q => q.EtudiantId == userId);
            ViewBag.NombreSessionsClavardage = _context.SessionsClavardage.Count(s => s.EtudiantId == userId);
            ViewBag.NombreRencontres = _context.RencontresPhysiques.Count(r => r.EtudiantId == userId);
            ViewBag.EstAbonne = etudiant.AbonnementActif;

            // Utiliser la vue DashboardComplet
            return View("DashboardComplet", etudiant);
        }

        // GET: Etudiant/MesQuestions
        public ActionResult MesQuestions()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var questions = _context.Questions
                .Include(q => q.Domaine)
                .Include(q => q.Reponses)
                .Where(q => q.EtudiantId == userId)
                .OrderByDescending(q => q.DateCreation)
                .ToList();

            return View(questions);
        }

        // GET: Etudiant/MesCours
        public ActionResult MesCours()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var inscriptions = _context.InscriptionsCours
                .Include(i => i.Cours)
                .Include(i => i.Cours.Tuteur)
                .Include(i => i.Cours.Domaine)
                .Include(i => i.ProgressionsModules)
                .Where(i => i.EtudiantId == userId)
                .OrderByDescending(i => i.DateInscription)
                .ToList();

            return View(inscriptions);
        }

        // GET: Etudiant/VoirCours/5
        public ActionResult VoirCours(int id, int? moduleId = null)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            
            // Verifier l'inscription
            var inscription = _context.InscriptionsCours
                .Include(i => i.Cours)
                .Include(i => i.ProgressionsModules)
                .FirstOrDefault(i => i.CoursId == id && i.EtudiantId == userId);

            if (inscription == null)
            {
                TempData["ErrorMessage"] = "Vous devez etre inscrit a ce cours pour y acceder";
                return RedirectToAction("Details", "Cours", new { id });
            }

            var viewModel = new CoursEtudiantViewModel
            {
                Cours = inscription.Cours,
                Inscription = inscription,
                Modules = _context.Modules
                    .Include(m => m.Ressources)
                    .Include(m => m.Quiz)
                    .Include(m => m.Quiz.Select(q => q.Questions))
                    .Where(m => m.CoursId == id && m.EstPublie)
                    .OrderBy(m => m.Ordre)
                    .ToList(),
                Progressions = inscription.ProgressionsModules.ToList(),
                TentativesQuiz = _context.TentativesQuiz
                    .Where(t => t.EtudiantId == userId)
                    .ToList()
            };

            // Determiner le module actuel
            if (moduleId.HasValue)
            {
                viewModel.ModuleActuel = viewModel.Modules.FirstOrDefault(m => m.ModuleId == moduleId.Value);
            }
            else
            {
                // Premier module non termine ou premier module
                var moduleNonTermine = viewModel.Modules.FirstOrDefault(m => 
                    !viewModel.Progressions.Any(p => p.ModuleId == m.ModuleId && p.EstComplete));
                viewModel.ModuleActuel = moduleNonTermine ?? viewModel.Modules.FirstOrDefault();
            }

            return View(viewModel);
        }

        // GET: Etudiant/VoirModule
        public ActionResult VoirModule(int coursId, int moduleId)
        {
            return RedirectToAction("VoirCours", new { id = coursId, moduleId });
        }

        // POST: Etudiant/MarquerModuleComplete
        [HttpPost]
        public JsonResult MarquerModuleComplete(int moduleId)
        {
            try
            {
                var userId = (int)Session["UserId"];
                var module = _context.Modules.Find(moduleId);
                
                if (module == null)
                    return Json(new { success = false, message = "Module introuvable" });

                var inscription = _context.InscriptionsCours
                    .FirstOrDefault(i => i.CoursId == module.CoursId && i.EtudiantId == userId);

                if (inscription == null)
                    return Json(new { success = false, message = "Inscription introuvable" });

                var progression = _context.ProgressionsModules
                    .FirstOrDefault(p => p.InscriptionId == inscription.InscriptionId && p.ModuleId == moduleId);

                if (progression == null)
                {
                    progression = new ProgressionModule
                    {
                        InscriptionId = inscription.InscriptionId,
                        ModuleId = moduleId,
                        DateDebut = DateTime.Now
                    };
                    _context.ProgressionsModules.Add(progression);
                }

                progression.EstComplete = true;
                progression.DateCompletion = DateTime.Now;
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Etudiant/MettreAJourProgression
        [HttpPost]
        public JsonResult MettreAJourProgression(int moduleId, int tempsVisionne)
        {
            try
            {
                var userId = (int)Session["UserId"];
                var module = _context.Modules.Find(moduleId);
                
                if (module == null)
                    return Json(new { success = false });

                var inscription = _context.InscriptionsCours
                    .FirstOrDefault(i => i.CoursId == module.CoursId && i.EtudiantId == userId);

                if (inscription == null)
                    return Json(new { success = false });

                var progression = _context.ProgressionsModules
                    .FirstOrDefault(p => p.InscriptionId == inscription.InscriptionId && p.ModuleId == moduleId);

                if (progression == null)
                {
                    progression = new ProgressionModule
                    {
                        InscriptionId = inscription.InscriptionId,
                        ModuleId = moduleId,
                        DateDebut = DateTime.Now,
                        TempsPasseMinutes = tempsVisionne
                    };
                    _context.ProgressionsModules.Add(progression);
                }
                else
                {
                    progression.TempsPasseMinutes = Math.Max(progression.TempsPasseMinutes, tempsVisionne);
                }

                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        // GET: Etudiant/TelechargerRessource/5
        public ActionResult TelechargerRessource(int id)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var ressource = _context.RessourcesModules
                .Include(r => r.Module)
                .FirstOrDefault(r => r.RessourceModuleId == id);

            if (ressource == null)
                return HttpNotFound();

            var userId = (int)Session["UserId"];
            var inscription = _context.InscriptionsCours
                .FirstOrDefault(i => i.CoursId == ressource.Module.CoursId && i.EtudiantId == userId);

            if (inscription == null)
            {
                TempData["ErrorMessage"] = "Vous devez etre inscrit au cours pour telecharger cette ressource";
                return RedirectToAction("Index");
            }

            var filePath = Server.MapPath(ressource.CheminFichier);
            if (!System.IO.File.Exists(filePath))
            {
                TempData["ErrorMessage"] = "Fichier introuvable";
                return RedirectToAction("VoirCours", new { id = ressource.Module.CoursId });
            }

            var fileName = Path.GetFileName(filePath);
            var contentType = "application/octet-stream";

            return File(filePath, contentType, fileName);
        }

        // GET: Etudiant/MonAbonnement
        public ActionResult MonAbonnement()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var etudiant = _context.Etudiants.Find(userId);

            if (etudiant == null)
            {
                return HttpNotFound();
            }

            return View(etudiant);
        }

        // GET: Etudiant/Profil
        public ActionResult Profil()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var etudiant = _context.Etudiants.Find(userId);

            if (etudiant == null)
            {
                return HttpNotFound();
            }

            return View(etudiant);
        }

        // GET: Etudiant/Quiz/5
        public ActionResult Quiz(int id)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var cours = _context.Cours
                .Include(c => c.Modules)
                .FirstOrDefault(c => c.CoursId == id);

            if (cours == null)
                return HttpNotFound();

            // Verifier l'inscription
            var inscription = _context.InscriptionsCours
                .FirstOrDefault(i => i.CoursId == id && i.EtudiantId == userId);

            if (inscription == null)
            {
                TempData["ErrorMessage"] = "Vous devez etre inscrit a ce cours";
                return RedirectToAction("MesCours", "Etudiant");
            }

            // Recuperer les quiz des modules du cours
            var quiz = _context.Quizs
                .Include(q => q.Module)
                .Include(q => q.Module.Cours)
                .Include(q => q.Questions)
                .Where(q => q.Module.CoursId == id && q.EstPublie)
                .OrderBy(q => q.Titre)
                .ToList();

            ViewBag.CoursId = id;
            ViewBag.CoursNom = cours.Nom;

            return View(quiz);
        }

        // GET: Etudiant/QuizModule/5
        public ActionResult QuizModule(int moduleId, int coursId)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var module = _context.Modules
                .Include(m => m.Cours)
                .Include(m => m.Quiz)
                .FirstOrDefault(m => m.ModuleId == moduleId && m.CoursId == coursId);

            if (module == null)
                return HttpNotFound();

            // Verifier l'inscription
            var inscription = _context.InscriptionsCours
                .FirstOrDefault(i => i.CoursId == coursId && i.EtudiantId == userId);

            if (inscription == null)
            {
                TempData["ErrorMessage"] = "Vous devez etre inscrit a ce cours";
                return RedirectToAction("MesCours", "Etudiant");
            }

            // Recuperer les quiz du module
            var quiz = module.Quiz
                .Where(q => q.EstPublie)
                .OrderBy(q => q.Titre)
                .ToList();

            ViewBag.ModuleId = moduleId;
            ViewBag.ModuleNom = module.Titre;
            ViewBag.CoursId = coursId;
            ViewBag.CoursNom = module.Cours.Nom;

            return View("Quiz", quiz);
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

