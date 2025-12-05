using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThotPlatform.Models;
using ThotPlatform.Utils;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des modules de cours
    /// </summary>
    [Authorize]
    public class ModuleController : Controller
    {
        private readonly ThotDbContext _context;

        public ModuleController()
        {
            _context = new ThotDbContext();
        }

        // GET: Module/Create?coursId=1
        public ActionResult Create(int? coursId)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            if (!coursId.HasValue)
            {
                TempData["ErrorMessage"] = "L'identifiant du cours est requis";
                return RedirectToAction("MesCours", "Tuteur");
            }

            var userId = (int)Session["UserId"];
            var cours = _context.Cours.FirstOrDefault(c => c.CoursId == coursId.Value && c.TuteurId == userId);

            if (cours == null)
            {
                TempData["ErrorMessage"] = "Cours introuvable ou vous n'etes pas autorise";
                return RedirectToAction("MesCours", "Tuteur");
            }

            ViewBag.CoursNom = cours.Nom;
            ViewBag.CoursId = coursId.Value;

            var module = new Module
            {
                CoursId = coursId.Value,
                Ordre = _context.Modules.Count(m => m.CoursId == coursId.Value) + 1
            };

            return View(module);
        }

        // POST: Module/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Module module, HttpPostedFileBase videoTheorique, HttpPostedFileBase videoDemonstrative)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var cours = _context.Cours.FirstOrDefault(c => c.CoursId == module.CoursId && c.TuteurId == userId);

            if (cours == null)
            {
                TempData["ErrorMessage"] = "Cours introuvable";
                return RedirectToAction("MesCours", "Tuteur");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Upload video theorique
                    if (videoTheorique != null && videoTheorique.ContentLength > 0)
                    {
                        var videoPath = FileUploadHelper.UploadVideo(videoTheorique, "Theorique");
                        if (videoPath != null)
                        {
                            module.VideoTheorique = videoPath;
                        }
                    }

                    // Upload video demonstrative
                    if (videoDemonstrative != null && videoDemonstrative.ContentLength > 0)
                    {
                        var videoPath = FileUploadHelper.UploadVideo(videoDemonstrative, "Demonstrative");
                        if (videoPath != null)
                        {
                            module.VideoDemonstrative = videoPath;
                        }
                    }

                    _context.Modules.Add(module);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Module cree avec succes !";
                    return RedirectToAction("Details", "Module", new { id = module.ModuleId });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Erreur lors de la creation du module : " + ex.Message;
                }
            }

            ViewBag.CoursNom = cours.Nom;
            ViewBag.CoursId = module.CoursId;
            return View(module);
        }

        // GET: Module/Details/5
        public ActionResult Details(int id)
        {
            var module = _context.Modules
                .Include(m => m.Cours)
                .Include(m => m.Ressources)
                .FirstOrDefault(m => m.ModuleId == id);

            if (module == null)
            {
                return HttpNotFound();
            }

            // Verifier si l'utilisateur a acces
            if (Session["UserType"]?.ToString() == "Tuteur")
            {
                var userId = (int)Session["UserId"];
                if (module.Cours.TuteurId != userId)
                {
                    TempData["ErrorMessage"] = "Acces non autorise";
                    return RedirectToAction("MesCours", "Tuteur");
                }
            }
            else if (Session["UserType"]?.ToString() == "Etudiant")
            {
                var userId = (int)Session["UserId"];
                var estInscrit = _context.InscriptionsCours
                    .Any(i => i.CoursId == module.CoursId && i.EtudiantId == userId);

                if (!estInscrit)
                {
                    TempData["ErrorMessage"] = "Vous devez etre inscrit au cours pour acceder a ce module";
                    return RedirectToAction("Details", "Cours", new { id = module.CoursId });
                }
            }

            return View(module);
        }

        // GET: Module/Edit/5
        public ActionResult Edit(int id)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var module = _context.Modules
                .Include(m => m.Cours)
                .FirstOrDefault(m => m.ModuleId == id && m.Cours.TuteurId == userId);

            if (module == null)
            {
                TempData["ErrorMessage"] = "Module introuvable";
                return RedirectToAction("MesCours", "Tuteur");
            }

            ViewBag.CoursNom = module.Cours.Nom;
            return View(module);
        }

        // POST: Module/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Module module, HttpPostedFileBase videoTheorique, HttpPostedFileBase videoDemonstrative)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var existingModule = _context.Modules
                .Include(m => m.Cours)
                .FirstOrDefault(m => m.ModuleId == module.ModuleId && m.Cours.TuteurId == userId);

            if (existingModule == null)
            {
                TempData["ErrorMessage"] = "Module introuvable";
                return RedirectToAction("MesCours", "Tuteur");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Upload nouvelle video theorique si fournie
                    if (videoTheorique != null && videoTheorique.ContentLength > 0)
                    {
                        var videoPath = FileUploadHelper.UploadVideo(videoTheorique, "Theorique");
                        if (videoPath != null)
                        {
                            existingModule.VideoTheorique = videoPath;
                        }
                    }

                    // Upload nouvelle video demonstrative si fournie
                    if (videoDemonstrative != null && videoDemonstrative.ContentLength > 0)
                    {
                        var videoPath = FileUploadHelper.UploadVideo(videoDemonstrative, "Demonstrative");
                        if (videoPath != null)
                        {
                            existingModule.VideoDemonstrative = videoPath;
                        }
                    }

                    // Mettre a jour les autres proprietes
                    existingModule.Titre = module.Titre;
                    existingModule.Description = module.Description;
                    existingModule.Ordre = module.Ordre;
                    existingModule.DureeMinutes = module.DureeMinutes;
                    existingModule.ContenuTexte = module.ContenuTexte;
                    existingModule.EstPublie = module.EstPublie;

                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Module modifie avec succes !";
                    return RedirectToAction("Details", new { id = module.ModuleId });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Erreur lors de la modification : " + ex.Message;
                }
            }

            ViewBag.CoursNom = existingModule.Cours.Nom;
            return View(module);
        }

        // GET: Module/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var module = _context.Modules
                .Include(m => m.Cours)
                .Include(m => m.Ressources)
                .Include(m => m.Quiz)
                .FirstOrDefault(m => m.ModuleId == id && m.Cours.TuteurId == userId);

            if (module == null)
            {
                TempData["ErrorMessage"] = "Module introuvable";
                return RedirectToAction("MesCours", "Tuteur");
            }

            try
            {
                var coursId = module.CoursId;
                
                // Supprimer les ressources du module
                var ressources = _context.RessourcesModules.Where(r => r.ModuleId == id).ToList();
                foreach (var ressource in ressources)
                {
                    _context.RessourcesModules.Remove(ressource);
                }
                
                // Supprimer les quiz du module
                var quizzes = _context.Quizs.Where(q => q.ModuleId == id).ToList();
                foreach (var quiz in quizzes)
                {
                    _context.Quizs.Remove(quiz);
                }
                
                // Supprimer le module
                _context.Modules.Remove(module);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Module supprime avec succes";
                return RedirectToAction("DetailsCours", "Tuteur", new { id = coursId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression : " + ex.Message;
                var coursId = module?.CoursId ?? 0;
                return RedirectToAction("DetailsCours", "Tuteur", new { id = coursId });
            }
        }

        // POST: Module/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirm(int id)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var module = _context.Modules
                .Include(m => m.Cours)
                .FirstOrDefault(m => m.ModuleId == id && m.Cours.TuteurId == userId);

            if (module == null)
            {
                TempData["ErrorMessage"] = "Module introuvable";
                return RedirectToAction("MesCours", "Tuteur");
            }

            try
            {
                var coursId = module.CoursId;
                _context.Modules.Remove(module);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Module supprime avec succes";
                return RedirectToAction("DetailsCours", "Tuteur", new { id = coursId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression : " + ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }

        // POST: Module/AddRessource
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRessource(int moduleId, string titre, TypeRessource type, HttpPostedFileBase fichier)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return Json(new { success = false, message = "Non autorise" });
            }

            var userId = (int)Session["UserId"];
            var module = _context.Modules
                .Include(m => m.Cours)
                .FirstOrDefault(m => m.ModuleId == moduleId && m.Cours.TuteurId == userId);

            if (module == null)
            {
                return Json(new { success = false, message = "Module introuvable" });
            }

            if (fichier != null && fichier.ContentLength > 0)
            {
                try
                {
                    var filePath = FileUploadHelper.UploadFile(fichier, "Ressources");
                    if (filePath != null)
                    {
                        var ressource = new RessourceModule
                        {
                            ModuleId = moduleId,
                            Titre = titre,
                            Type = type,
                            CheminFichier = filePath
                        };

                        _context.RessourcesModules.Add(ressource);
                        _context.SaveChanges();

                        return Json(new { success = true, message = "Ressource ajoutee avec succes" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Erreur : " + ex.Message });
                }
            }

            return Json(new { success = false, message = "Aucun fichier fourni" });
        }

        // GET: Module/DownloadRessource/5
        public ActionResult DownloadRessource(int id)
        {
            var ressource = _context.RessourcesModules
                .Include(r => r.Module.Cours)
                .FirstOrDefault(r => r.RessourceModuleId == id);

            if (ressource == null)
            {
                return HttpNotFound();
            }

            // Verifier acces
            if (Session["UserType"]?.ToString() == "Etudiant")
            {
                var userId = (int)Session["UserId"];
                var estInscrit = _context.InscriptionsCours
                    .Any(i => i.CoursId == ressource.Module.CoursId && i.EtudiantId == userId);

                if (!estInscrit)
                {
                    TempData["ErrorMessage"] = "Acces non autorise";
                    return RedirectToAction("Index", "Home");
                }
            }

            var filePath = Server.MapPath(ressource.CheminFichier);
            if (System.IO.File.Exists(filePath))
            {
                var fileName = Path.GetFileName(filePath);
                return File(filePath, "application/octet-stream", fileName);
            }

            TempData["ErrorMessage"] = "Fichier introuvable";
            return RedirectToAction("Details", new { id = ressource.ModuleId });
        }

        // POST: Module/MarquerComplete
        [HttpPost]
        public ActionResult MarquerComplete(int moduleId)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return Json(new { success = false, message = "Non autorise" });
            }

            var userId = (int)Session["UserId"];
            var module = _context.Modules.Find(moduleId);

            if (module == null)
            {
                return Json(new { success = false, message = "Module introuvable" });
            }

            // Verifier inscription
            var inscription = _context.InscriptionsCours
                .FirstOrDefault(i => i.CoursId == module.CoursId && i.EtudiantId == userId);

            if (inscription == null)
            {
                return Json(new { success = false, message = "Non inscrit au cours" });
            }

            // Verifier si deja marque comme complete
            var progression = _context.ProgressionsModules
                .FirstOrDefault(p => p.ModuleId == moduleId && p.InscriptionId == inscription.InscriptionId);

            if (progression == null)
            {
                progression = new ProgressionModule
                {
                    InscriptionId = inscription.InscriptionId,
                    ModuleId = moduleId,
                    EstComplete = true,
                    DateCompletion = DateTime.Now,
                    TempsPasseMinutes = 0
                };
                _context.ProgressionsModules.Add(progression);
            }
            else
            {
                progression.EstComplete = true;
                progression.DateCompletion = DateTime.Now;
            }

            _context.SaveChanges();

            // Calculer progression totale
            var totalModules = _context.Modules.Count(m => m.CoursId == module.CoursId);
            var modulesCompletes = _context.ProgressionsModules
                .Count(p => p.Inscription.CoursId == module.CoursId && 
                           p.Inscription.EtudiantId == userId && 
                           p.EstComplete);

            var pourcentage = totalModules > 0 ? (modulesCompletes * 100 / totalModules) : 0;

            return Json(new { success = true, progression = pourcentage });
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

