using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ThotPlatform.Models;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des quiz
    /// </summary>
    [Authorize]
    public class QuizController : Controller
    {
        private readonly ThotDbContext _context;

        public QuizController()
        {
            _context = new ThotDbContext();
        }

        // GET: Quiz/CommencerQuiz/5
        public ActionResult CommencerQuiz(int id)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var quiz = _context.Quizs
                .Include(q => q.Module)
                .Include(q => q.Questions)
                .FirstOrDefault(q => q.QuizId == id);

            if (quiz == null || !quiz.EstPublie)
                return HttpNotFound();

            var userId = (int)Session["UserId"];

            // Verifier l'inscription au cours
            var inscription = _context.InscriptionsCours
                .FirstOrDefault(i => i.CoursId == quiz.Module.CoursId && i.EtudiantId == userId);

            if (inscription == null)
            {
                TempData["ErrorMessage"] = "Vous devez etre inscrit au cours pour passer ce quiz";
                return RedirectToAction("Index", "Etudiant");
            }

            // Verifier le nombre de tentatives
            var nombreTentatives = _context.TentativesQuiz
                .Count(t => t.QuizId == id && t.EtudiantId == userId);

            if (nombreTentatives >= quiz.NombreTentativesAutorisees)
            {
                TempData["ErrorMessage"] = "Vous avez atteint le nombre maximum de tentatives pour ce quiz";
                return RedirectToAction("VoirCours", "Etudiant", new { id = quiz.Module.CoursId });
            }

            // Creer une nouvelle tentative
            var tentative = new TentativeQuiz
            {
                QuizId = id,
                EtudiantId = userId,
                DateDebut = DateTime.Now,
                EstCompletee = false
            };

            _context.TentativesQuiz.Add(tentative);
            _context.SaveChanges();

            return RedirectToAction("PasserQuiz", new { id = tentative.TentativeId });
        }

        // GET: Quiz/PasserQuiz/5
        public ActionResult PasserQuiz(int id)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var tentative = _context.TentativesQuiz
                .Include(t => t.Quiz)
                .Include(t => t.Quiz.Module)
                .Include(t => t.Quiz.Questions)
                .Include(t => t.Quiz.Questions.Select(q => q.Choix))
                .Include(t => t.Reponses)
                .FirstOrDefault(t => t.TentativeId == id && t.EtudiantId == userId);

            if (tentative == null)
                return HttpNotFound();

            if (tentative.EstCompletee)
            {
                return RedirectToAction("ResultatQuiz", new { id });
            }

            // Melanger les questions si necessaire
            var questions = tentative.Quiz.OrdreAleatoire
                ? tentative.Quiz.Questions.OrderBy(q => Guid.NewGuid()).ToList()
                : tentative.Quiz.Questions.OrderBy(q => q.Ordre).ToList();

            ViewBag.Questions = questions;
            
            // Calculer le temps restant
            var tempsEcoule = (DateTime.Now - tentative.DateDebut).TotalMinutes;
            var dureeQuiz = tentative.Quiz.DureeLimiteMinutes ?? 60; // 60 minutes par defaut
            var tempsRestant = Math.Max(0, dureeQuiz - (int)tempsEcoule);
            ViewBag.TempsRestant = tempsRestant;

            return View(tentative);
        }

        // GET: Quiz/ContinuerQuiz/5
        public ActionResult ContinuerQuiz(int id)
        {
            var userId = (int)Session["UserId"];
            var tentative = _context.TentativesQuiz
                .FirstOrDefault(t => t.QuizId == id && t.EtudiantId == userId && !t.EstCompletee);

            if (tentative == null)
            {
                return RedirectToAction("CommencerQuiz", new { id });
            }

            return RedirectToAction("PasserQuiz", new { id = tentative.TentativeId });
        }

        // POST: Quiz/SoumettreReponse
        [HttpPost]
        public JsonResult SoumettreReponse(int tentativeId, int questionId, int? choixId, string reponseTexte)
        {
            try
            {
                var userId = (int)Session["UserId"];
                var tentative = _context.TentativesQuiz
                    .FirstOrDefault(t => t.TentativeId == tentativeId && t.EtudiantId == userId);

                if (tentative == null || tentative.EstCompletee)
                    return Json(new { success = false, message = "Tentative invalide" });

                var question = _context.QuestionsQuiz
                    .Include(q => q.Choix)
                    .FirstOrDefault(q => q.QuestionQuizId == questionId);

                if (question == null)
                    return Json(new { success = false, message = "Question invalide" });

                // Verifier si une reponse existe deja
                var reponseExistante = _context.ReponsesQuiz
                    .FirstOrDefault(r => r.TentativeId == tentativeId && r.QuestionQuizId == questionId);

                if (reponseExistante != null)
                {
                    // Mettre a jour la reponse
                    reponseExistante.ChoixId = choixId;
                    reponseExistante.ReponseTexte = reponseTexte;
                }
                else
                {
                    // Creer une nouvelle reponse
                    var reponse = new ReponseQuiz
                    {
                        TentativeId = tentativeId,
                        QuestionQuizId = questionId,
                        ChoixId = choixId,
                        ReponseTexte = reponseTexte
                    };
                    _context.ReponsesQuiz.Add(reponse);
                }

                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Quiz/SoumettreQuiz
        [HttpPost]
        public ActionResult SoumettreQuiz(int id)
        {
            var userId = (int)Session["UserId"];
            var tentative = _context.TentativesQuiz
                .Include(t => t.Quiz)
                .Include(t => t.Quiz.Questions)
                .Include(t => t.Quiz.Questions.Select(q => q.Choix))
                .Include(t => t.Reponses)
                .FirstOrDefault(t => t.TentativeId == id && t.EtudiantId == userId);

            if (tentative == null)
                return HttpNotFound();

            if (tentative.EstCompletee)
                return RedirectToAction("ResultatQuiz", new { id });

            // Calculer le score
            int totalPoints = 0;
            int pointsObtenus = 0;

            foreach (var question in tentative.Quiz.Questions)
            {
                totalPoints += question.Points;

                var reponse = tentative.Reponses.FirstOrDefault(r => r.QuestionQuizId == question.QuestionQuizId);
                if (reponse != null)
                {
                    bool estCorrecte = false;

                    switch (question.Type)
                    {
                        case TypeQuestionQuiz.ChoixMultiple:
                        case TypeQuestionQuiz.VraiFaux:
                            var choixCorrect = question.Choix.FirstOrDefault(c => c.EstCorrect);
                            if (choixCorrect != null && reponse.ChoixId == choixCorrect.ChoixId)
                            {
                                estCorrecte = true;
                                pointsObtenus += question.Points;
                            }
                            break;

                        case TypeQuestionQuiz.ReponseCourte:
                        case TypeQuestionQuiz.ReponseLongue:
                            // Pour les reponses libres, on ne peut pas corriger automatiquement
                            // Le tuteur devra corriger manuellement
                            break;
                    }

                    reponse.EstCorrecte = estCorrecte;
                    reponse.PointsObtenus = estCorrecte ? question.Points : 0;
                }
            }

            // Mettre a jour la tentative
            tentative.DateFin = DateTime.Now;
            tentative.EstCompletee = true;
            tentative.NoteObtenue = totalPoints > 0 ? (int)((pointsObtenus * 100.0) / totalPoints) : 0;
            tentative.EstReussie = tentative.NoteObtenue >= tentative.Quiz.NotePassage;

            _context.SaveChanges();

            return RedirectToAction("ResultatQuiz", new { id });
        }

        // GET: Quiz/ResultatQuiz/5
        public ActionResult ResultatQuiz(int id)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var tentative = _context.TentativesQuiz
                .Include(t => t.Quiz)
                .Include(t => t.Quiz.Module)
                .Include(t => t.Quiz.Questions)
                .Include(t => t.Quiz.Questions.Select(q => q.Choix))
                .Include(t => t.Reponses)
                .Include(t => t.Reponses.Select(r => r.Choix))
                .FirstOrDefault(t => t.TentativeId == id && t.EtudiantId == userId);

            if (tentative == null)
                return HttpNotFound();

            if (!tentative.EstCompletee)
                return RedirectToAction("PasserQuiz", new { id });

            return View(tentative);
        }

        // GET: Quiz/Index (Tuteur)
        public ActionResult Index()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var quizList = _context.Quizs
                .Include(q => q.Module)
                .Include(q => q.Module.Cours)
                .Where(q => q.Module.Cours.TuteurId == userId)
                .OrderByDescending(q => q.DateCreation)
                .ToList();

            // Charger les questions et tentatives pour chaque quiz
            foreach (var quiz in quizList)
            {
                // Creer une nouvelle instance de contexte pour eviter les problemes de cache
                using (var context = new ThotDbContext())
                {
                    var questionsCount = context.QuestionsQuiz.Count(q => q.QuizId == quiz.QuizId);
                    var tentativesCount = context.TentativesQuiz.Count(t => t.QuizId == quiz.QuizId);
                    
                    // Creer les collections avec les bonnes donnees
                    quiz.Questions = context.QuestionsQuiz.Where(q => q.QuizId == quiz.QuizId).ToList();
                    quiz.Tentatives = context.TentativesQuiz.Where(t => t.QuizId == quiz.QuizId).ToList();
                }
            }

            return View(quizList);
        }

        // GET: Quiz/Details/5 (Tuteur)
        public ActionResult Details(int id)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var quiz = _context.Quizs
                .Include(q => q.Module)
                .Include(q => q.Module.Cours)
                .Include(q => q.Questions)
                .Include(q => q.Questions.Select(qq => qq.Choix))
                .Include(q => q.Tentatives)
                .FirstOrDefault(q => q.QuizId == id && q.Module.Cours.TuteurId == userId);

            if (quiz == null)
                return HttpNotFound();

            // Charger les etudiants pour chaque tentative
            foreach (var tentative in quiz.Tentatives)
            {
                if (tentative.EtudiantId > 0)
                {
                    tentative.Etudiant = _context.Etudiants.FirstOrDefault(e => e.UtilisateurId == tentative.EtudiantId);
                }
            }

            return View(quiz);
        }

        // GET: Quiz/CorrigerTentative/5 (Tuteur)
        public ActionResult CorrigerTentative(int id)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var tentative = _context.TentativesQuiz
                .Include(t => t.Quiz)
                .Include(t => t.Quiz.Module)
                .Include(t => t.Quiz.Module.Cours)
                .Include(t => t.Quiz.Questions)
                .Include(t => t.Etudiant)
                .Include(t => t.Reponses)
                .FirstOrDefault(t => t.TentativeId == id && t.Quiz.Module.Cours.TuteurId == userId);

            if (tentative == null)
                return HttpNotFound();

            // Charger les choix pour chaque question
            foreach (var question in tentative.Quiz.Questions)
            {
                question.Choix = _context.ChoixReponses
                    .Where(c => c.QuestionQuizId == question.QuestionQuizId)
                    .ToList();
            }

            // Charger les choix pour chaque reponse
            foreach (var reponse in tentative.Reponses)
            {
                if (reponse.ChoixId.HasValue)
                {
                    reponse.Choix = _context.ChoixReponses
                        .FirstOrDefault(c => c.ChoixId == reponse.ChoixId);
                }
            }

            return View(tentative);
        }

        // POST: Quiz/EnregistrerCorrection
        [HttpPost]
        public JsonResult EnregistrerCorrection(int reponseQuizId, int pointsAccordes)
        {
            try
            {
                var userId = (int)Session["UserId"];
                var reponse = _context.ReponsesQuiz
                    .Include(r => r.Tentative)
                    .Include(r => r.Tentative.Quiz)
                    .Include(r => r.Tentative.Quiz.Module)
                    .Include(r => r.Tentative.Quiz.Module.Cours)
                    .FirstOrDefault(r => r.ReponseQuizId == reponseQuizId);

                if (reponse == null || reponse.Tentative.Quiz.Module.Cours.TuteurId != userId)
                    return Json(new { success = false, message = "Acces refuse" });

                reponse.PointsAccordes = pointsAccordes;
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Quiz/FinalisierCorrection
        [HttpPost]
        public ActionResult FinalisierCorrection(int tentativeId)
        {
            try
            {
                var userId = (int)Session["UserId"];
                var tentative = _context.TentativesQuiz
                    .Include(t => t.Quiz)
                    .Include(t => t.Quiz.Module)
                    .Include(t => t.Quiz.Module.Cours)
                    .Include(t => t.Quiz.Questions)
                    .Include(t => t.Reponses)
                    .FirstOrDefault(t => t.TentativeId == tentativeId && t.Quiz.Module.Cours.TuteurId == userId);

                if (tentative == null)
                    return HttpNotFound();

                // Recalculer la note avec les points accordes pour les reponses libres
                int totalPoints = tentative.Quiz.Questions.Sum(q => q.Points);
                int pointsObtenus = tentative.Reponses.Sum(r => r.PointsObtenus);

                tentative.NoteObtenue = totalPoints > 0 ? (int)((pointsObtenus * 100.0) / totalPoints) : 0;
                tentative.EstReussie = tentative.NoteObtenue >= tentative.Quiz.NotePassage;
                tentative.EstCorrigee = true;

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Correction finalisee avec succes";
                return RedirectToAction("Details", new { id = tentative.QuizId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de la finalisation : " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Quiz/SelectModule (Tuteur)
        public ActionResult SelectModule()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var modules = _context.Modules
                .Include(m => m.Cours)
                .Where(m => m.Cours.TuteurId == userId)
                .OrderBy(m => m.Cours.Nom)
                .ThenBy(m => m.Ordre)
                .ToList();

            return View(modules);
        }

        // GET: Quiz/Create (Tuteur)
        public ActionResult Create(int moduleId)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var module = _context.Modules
                .Include(m => m.Cours)
                .FirstOrDefault(m => m.ModuleId == moduleId && m.Cours.TuteurId == userId);

            if (module == null)
                return HttpNotFound();

            var quiz = new Quiz { ModuleId = moduleId };
            ViewBag.ModuleNom = module.Titre;
            ViewBag.CoursNom = module.Cours.Nom;
            ViewBag.ModuleId = moduleId;

            return View("CreateQuiz", quiz);
        }

        // POST: Quiz/Create (Tuteur)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Quiz quiz)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var module = _context.Modules
                .Include(m => m.Cours)
                .FirstOrDefault(m => m.ModuleId == quiz.ModuleId && m.Cours.TuteurId == userId);

            if (module == null)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    quiz.DateCreation = DateTime.Now;
                    _context.Quizs.Add(quiz);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Quiz cree avec succes";
                    return RedirectToAction("Details", new { id = quiz.QuizId });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Erreur lors de la creation : " + ex.Message;
                }
            }

            ViewBag.ModuleNom = module.Titre;
            ViewBag.CoursNom = module.Cours.Nom;
            return View("CreateQuiz", quiz);
        }

        // GET: Quiz/AddQuestion/5 (Tuteur)
        public ActionResult AddQuestion(int quizId)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var quiz = _context.Quizs
                .Include(q => q.Module)
                .Include(q => q.Module.Cours)
                .FirstOrDefault(q => q.QuizId == quizId && q.Module.Cours.TuteurId == userId);

            if (quiz == null)
                return HttpNotFound();

            var question = new QuestionQuiz { QuizId = quizId, Ordre = quiz.Questions.Count + 1 };
            ViewBag.QuizTitre = quiz.Titre;
            ViewBag.QuizId = quizId;

            return View(question);
        }

        // POST: Quiz/AddQuestion (Tuteur)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuestion(QuestionQuiz question, string[] choixTexte, bool[] choixCorrect)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var quiz = _context.Quizs
                .Include(q => q.Module)
                .Include(q => q.Module.Cours)
                .FirstOrDefault(q => q.QuizId == question.QuizId && q.Module.Cours.TuteurId == userId);

            if (quiz == null)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Ajouter la question
                    _context.QuestionsQuiz.Add(question);
                    _context.SaveChanges();

                    // Ajouter les choix de reponses
                    if (choixTexte != null)
                    {
                        for (int i = 0; i < choixTexte.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(choixTexte[i]))
                            {
                                var choix = new ChoixReponse
                                {
                                    QuestionQuizId = question.QuestionQuizId,
                                    Texte = choixTexte[i],
                                    EstCorrect = choixCorrect != null && i < choixCorrect.Length && choixCorrect[i],
                                    Ordre = i + 1
                                };
                                _context.ChoixReponses.Add(choix);
                            }
                        }
                        _context.SaveChanges();
                    }

                    TempData["SuccessMessage"] = "Question ajoutee avec succes";
                    return RedirectToAction("Details", new { id = question.QuizId });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Erreur lors de l'ajout : " + ex.Message;
                }
            }

            ViewBag.QuizTitre = quiz.Titre;
            ViewBag.QuizId = question.QuizId;
            return View(question);
        }

        // GET: Quiz/EditQuestion/5 (Tuteur)
        public ActionResult EditQuestion(int? questionId)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var question = _context.QuestionsQuiz
                .Include(q => q.Quiz)
                .Include(q => q.Quiz.Module)
                .Include(q => q.Quiz.Module.Cours)
                .Include(q => q.Choix)
                .FirstOrDefault(q => q.QuestionQuizId == questionId && q.Quiz.Module.Cours.TuteurId == userId);

            if (question == null)
                return HttpNotFound();

            ViewBag.QuizTitre = question.Quiz.Titre;
            ViewBag.QuizId = question.QuizId;

            return View(question);
        }

        // POST: Quiz/EditQuestion (Tuteur)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuestion(QuestionQuiz question, string[] choixTexte, bool[] choixCorrect)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var existingQuestion = _context.QuestionsQuiz
                .Include(q => q.Quiz)
                .Include(q => q.Quiz.Module)
                .Include(q => q.Quiz.Module.Cours)
                .Include(q => q.Choix)
                .FirstOrDefault(q => q.QuestionQuizId == question.QuestionQuizId && q.Quiz.Module.Cours.TuteurId == userId);

            if (existingQuestion == null)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    existingQuestion.Texte = question.Texte;
                    existingQuestion.Points = question.Points;

                    // Supprimer les reponses associees aux anciens choix
                    var reponseAssociees = _context.ReponsesQuiz
                        .Where(r => r.Tentative.QuizId == existingQuestion.QuizId && 
                                   r.QuestionQuizId == existingQuestion.QuestionQuizId)
                        .ToList();
                    _context.ReponsesQuiz.RemoveRange(reponseAssociees);
                    _context.SaveChanges();

                    // Supprimer les anciens choix
                    _context.ChoixReponses.RemoveRange(existingQuestion.Choix);
                    _context.SaveChanges();

                    // Ajouter les nouveaux choix
                    if (choixTexte != null)
                    {
                        for (int i = 0; i < choixTexte.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(choixTexte[i]))
                            {
                                var choix = new ChoixReponse
                                {
                                    QuestionQuizId = question.QuestionQuizId,
                                    Texte = choixTexte[i],
                                    EstCorrect = choixCorrect != null && i < choixCorrect.Length && choixCorrect[i],
                                    Ordre = i + 1
                                };
                                _context.ChoixReponses.Add(choix);
                            }
                        }
                    }

                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Question modifiee avec succes";
                    return RedirectToAction("Details", new { id = question.QuizId });
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Erreur lors de la modification : " + ex.Message;
                }
            }

            ViewBag.QuizTitre = existingQuestion.Quiz.Titre;
            ViewBag.QuizId = question.QuizId;
            return View(question);
        }

        // GET: Quiz/DeleteQuestion/5 (Tuteur)
        public ActionResult DeleteQuestion(int questionId)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            var question = _context.QuestionsQuiz
                .Include(q => q.Quiz)
                .Include(q => q.Quiz.Module)
                .Include(q => q.Quiz.Module.Cours)
                .FirstOrDefault(q => q.QuestionQuizId == questionId && q.Quiz.Module.Cours.TuteurId == userId);

            if (question == null)
                return HttpNotFound();

            try
            {
                var quizId = question.QuizId;
                _context.QuestionsQuiz.Remove(question);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Question supprimee avec succes";
                return RedirectToAction("Details", new { id = quizId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression : " + ex.Message;
                return RedirectToAction("Details", new { id = question.QuizId });
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

