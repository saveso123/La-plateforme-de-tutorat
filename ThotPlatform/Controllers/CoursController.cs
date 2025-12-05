using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThotPlatform.Models;
using ThotPlatform.Utils;
using ThotPlatform.ViewModels;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des cours E-learning
    /// </summary>
    public class CoursController : Controller
    {
        private readonly ThotDbContext _context;

        public CoursController()
        {
            _context = new ThotDbContext();
        }

        // GET: Cours
        public ActionResult Index()
        {
            var cours = _context.Cours
                .Include(c => c.Tuteur)
                .Include(c => c.Domaine)
                .Where(c => c.EstPublie)
                .OrderByDescending(c => c.DateCreation)
                .ToList();

            return View(cours);
        }

        // GET: Cours/Details/5
        public ActionResult Details(int id)
        {
            var coursData = _context.Cours
                .Where(c => c.CoursId == id && c.EstPublie)
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

            // Verifier si l'etudiant est inscrit
            if (Session["UserId"] != null && Session["UserType"]?.ToString() == "Etudiant")
            {
                var userId = (int)Session["UserId"];
                coursData.EstInscrit = _context.InscriptionsCours
                    .Any(i => i.CoursId == id && i.EtudiantId == userId);
            }

            return View(coursData);
        }

        // GET: Cours/Create
        [Authorize]
        public ActionResult Create()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                TempData["ErrorMessage"] = "Seuls les tuteurs peuvent creer des cours";
                return RedirectToAction("Index");
            }

            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom");
            return View();
        }

        // POST: Cours/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(Cours cours, HttpPostedFileBase image)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var userId = (int)Session["UserId"];
                cours.TuteurId = userId;
                cours.DateCreation = DateTime.Now;
                cours.DateModification = DateTime.Now;
                cours.EstPublie = false;
                cours.NombreInscrits = 0;

                // Upload de l'image de couverture
                if (image != null && image.ContentLength > 0)
                {
                    try
                    {
                        cours.ImageCouverture = FileUploadHelper.UploadImage(image, "Cours");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Erreur lors de l'upload de l'image: " + ex.Message);
                        ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom");
                        return View(cours);
                    }
                }

                _context.Cours.Add(cours);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Le cours a ete cree avec succes !";
                return RedirectToAction("Edit", new { id = cours.CoursId });
            }

            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom");
            return View(cours);
        }

        // GET: Cours/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Details", new { id });
            }

            var cours = _context.Cours.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }

            var userId = (int)Session["UserId"];
            if (cours.TuteurId != userId)
            {
                TempData["ErrorMessage"] = "Vous n'etes pas autorise a modifier ce cours";
                return RedirectToAction("Details", new { id });
            }

            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom", cours.DomaineId);
            return View(cours);
        }

        // POST: Cours/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(Cours cours, HttpPostedFileBase image)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Details", new { id = cours.CoursId });
            }

            var existingCours = _context.Cours.Find(cours.CoursId);
            if (existingCours == null)
            {
                return HttpNotFound();
            }

            var userId = (int)Session["UserId"];
            if (existingCours.TuteurId != userId)
            {
                return RedirectToAction("Details", new { id = cours.CoursId });
            }

            // Retirer la validation pour les champs non modifiables
            ModelState.Remove("Code");
            ModelState.Remove("TuteurId");
            ModelState.Remove("DateCreation");
            ModelState.Remove("NombreModules");
            ModelState.Remove("ImageCouverture");
            ModelState.Remove("NombreInscrits");
            ModelState.Remove("NoteMoyenne");

            if (ModelState.IsValid)
            {
                try
                {
                    existingCours.Nom = cours.Nom;
                    existingCours.Description = cours.Description;
                    existingCours.DomaineId = cours.DomaineId;
                    existingCours.Niveau = cours.Niveau;
                    existingCours.DureeEstimeeHeures = cours.DureeEstimeeHeures;
                    existingCours.EstPublie = cours.EstPublie;
                    existingCours.DateModification = DateTime.Now;

                    // Upload de la nouvelle image
                    if (image != null && image.ContentLength > 0)
                    {
                        try
                        {
                            // Supprimer l'ancienne image
                            if (!string.IsNullOrEmpty(existingCours.ImageCouverture))
                            {
                                FileUploadHelper.DeleteFile(existingCours.ImageCouverture);
                            }
                            existingCours.ImageCouverture = FileUploadHelper.UploadImage(image, "Cours");
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Erreur lors de l'upload de l'image: " + ex.Message);
                            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom", cours.DomaineId);
                            return View(existingCours);
                        }
                    }

                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Le cours a ete modifie avec succes !";
                    return RedirectToAction("Details", new { id = cours.CoursId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la modification: " + ex.Message);
                }
            }

            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom", cours.DomaineId);
            return View(existingCours);
        }

        // GET: Cours/MesCours
        [Authorize]
        public ActionResult MesCours()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Index");
            }

            var userId = (int)Session["UserId"];
            var inscriptions = _context.InscriptionsCours
                .Include(i => i.Cours)
                .Include(i => i.Cours.Tuteur)
                .Include(i => i.Cours.Domaine)
                .Where(i => i.EtudiantId == userId)
                .OrderByDescending(i => i.DateDerniereActivite ?? i.DateInscription)
                .ToList();

            return View(inscriptions);
        }

        // POST: Cours/Inscrire/5
        [HttpPost]
        [Authorize]
        public JsonResult Inscrire(int id)
        {
            try
            {
                if (Session["UserType"]?.ToString() != "Etudiant")
                {
                    return Json(new { success = false, message = "Seuls les etudiants peuvent s'inscrire aux cours" });
                }

                var userId = (int)Session["UserId"];
                
                // Verifier si deja inscrit
                var inscriptionExistante = _context.InscriptionsCours
                    .FirstOrDefault(i => i.CoursId == id && i.EtudiantId == userId);

                if (inscriptionExistante != null)
                {
                    return Json(new { success = false, message = "Vous etes deja inscrit a ce cours" });
                }

                var cours = _context.Cours.Find(id);
                if (cours == null || !cours.EstPublie)
                {
                    return Json(new { success = false, message = "Cours introuvable" });
                }

                // Creer l'inscription
                var inscription = new InscriptionCours
                {
                    CoursId = id,
                    EtudiantId = userId,
                    DateInscription = DateTime.Now
                };

                _context.InscriptionsCours.Add(inscription);
                
                // Incrementer le nombre d'inscrits
                cours.NombreInscrits++;
                
                _context.SaveChanges();

                return Json(new { success = true, message = "Inscription reussie !" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erreur: " + ex.Message });
            }
        }

        // POST: Cours/Desinscrire/5
        [HttpPost]
        [Authorize]
        public JsonResult Desinscrire(int id)
        {
            try
            {
                if (Session["UserType"]?.ToString() != "Etudiant")
                {
                    return Json(new { success = false, message = "Action non autorisee" });
                }

                var userId = (int)Session["UserId"];
                
                var inscription = _context.InscriptionsCours
                    .FirstOrDefault(i => i.CoursId == id && i.EtudiantId == userId);

                if (inscription == null)
                {
                    return Json(new { success = false, message = "Vous n'etes pas inscrit a ce cours" });
                }

                _context.InscriptionsCours.Remove(inscription);
                
                // Decrementer le nombre d'inscrits
                var cours = _context.Cours.Find(id);
                if (cours != null && cours.NombreInscrits > 0)
                {
                    cours.NombreInscrits--;
                }
                
                _context.SaveChanges();

                return Json(new { success = true, message = "Desinscription reussie" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erreur: " + ex.Message });
            }
        }

        // GET: Cours/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Index");
            }

            var userId = (int)Session["UserId"];
            var cours = _context.Cours.FirstOrDefault(c => c.CoursId == id && c.TuteurId == userId);

            if (cours == null)
            {
                TempData["ErrorMessage"] = "Cours introuvable";
                return RedirectToAction("MesCours", "Tuteur");
            }

            try
            {
                // Supprimer les modules du cours (avec leurs dependances)
                var modules = _context.Modules.Where(m => m.CoursId == id).ToList();
                foreach (var module in modules)
                {
                    // Supprimer les progressions des modules
                    var progressions = _context.ProgressionsModules.Where(p => p.ModuleId == module.ModuleId).ToList();
                    foreach (var progression in progressions)
                    {
                        _context.ProgressionsModules.Remove(progression);
                    }

                    // Supprimer les ressources du module
                    var ressources = _context.RessourcesModules.Where(r => r.ModuleId == module.ModuleId).ToList();
                    foreach (var ressource in ressources)
                    {
                        _context.RessourcesModules.Remove(ressource);
                    }

                    // Supprimer les quiz du module (avec leurs dependances)
                    var quizzes = _context.Quizs.Where(q => q.ModuleId == module.ModuleId).ToList();
                    foreach (var quiz in quizzes)
                    {
                        // Supprimer les tentatives du quiz
                        var tentatives = _context.TentativesQuiz.Where(t => t.QuizId == quiz.QuizId).ToList();
                        foreach (var tentative in tentatives)
                        {
                            // Supprimer les reponses de la tentative
                            var reponses = _context.ReponsesQuiz.Where(r => r.TentativeId == tentative.TentativeId).ToList();
                            foreach (var reponse in reponses)
                            {
                                _context.ReponsesQuiz.Remove(reponse);
                            }
                            _context.TentativesQuiz.Remove(tentative);
                        }

                        // Supprimer les questions du quiz
                        var questions = _context.QuestionsQuiz.Where(q => q.QuizId == quiz.QuizId).ToList();
                        foreach (var question in questions)
                        {
                            // Supprimer les choix de reponse
                            var choix = _context.ChoixReponses.Where(c => c.QuestionQuizId == question.QuestionQuizId).ToList();
                            foreach (var ch in choix)
                            {
                                _context.ChoixReponses.Remove(ch);
                            }
                            _context.QuestionsQuiz.Remove(question);
                        }

                        _context.Quizs.Remove(quiz);
                    }

                    // Supprimer le module
                    _context.Modules.Remove(module);
                }

                // Supprimer les inscriptions au cours
                var inscriptions = _context.InscriptionsCours.Where(i => i.CoursId == id).ToList();
                foreach (var inscription in inscriptions)
                {
                    _context.InscriptionsCours.Remove(inscription);
                }

                // Supprimer le cours
                _context.Cours.Remove(cours);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Cours supprime avec succes";
                return RedirectToAction("MesCours", "Tuteur");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression : " + ex.Message;
                return RedirectToAction("MesCours", "Tuteur");
            }
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

