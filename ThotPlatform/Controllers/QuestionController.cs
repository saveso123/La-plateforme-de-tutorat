using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThotPlatform.Models;
using ThotPlatform.Utils;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des questions et reponses
    /// </summary>
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly ThotDbContext _context;

        public QuestionController()
        {
            _context = new ThotDbContext();
        }

        // GET: Question
        public ActionResult Index()
        {
            var questions = _context.Questions
                .Include(q => q.Etudiant)
                .Include(q => q.Domaine)
                .Include(q => q.Reponses)
                .Where(q => q.Statut == StatutQuestion.Repondue)
                .OrderByDescending(q => q.DateCreation)
                .Take(50)
                .ToList();

            return View(questions);
        }

        // GET: Question/Create
        public ActionResult Create()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                TempData["ErrorMessage"] = "Seuls les etudiants peuvent poser des questions";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom");
            return View();
        }

        // POST: Question/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Question question, HttpPostedFileBase fichier)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var userId = (int)Session["UserId"];
                var etudiant = _context.Etudiants.Find(userId);

                // Verifier si l'etudiant peut poser une question
                if (!etudiant.AbonnementActif)
                {
                    // Non-abonne : creer une transaction de 2$
                    var transaction = new Transaction
                    {
                        EtudiantId = userId,
                        Type = TypeTransaction.QuestionPonctuelle,
                        Montant = 2.00m,
                        Statut = StatutTransaction.EnAttente,
                        PayPalTransactionId = Guid.NewGuid().ToString(),
                        Description = "Question ponctuelle"
                    };
                    _context.Transactions.Add(transaction);
                }

                question.EtudiantId = userId;
                question.DateCreation = DateTime.Now;
                question.Statut = StatutQuestion.EnAttente;
                question.DateLimiteReponse = DateTime.Now.AddHours(2);

                // Upload du fichier si present
                if (fichier != null && fichier.ContentLength > 0)
                {
                    try
                    {
                        question.FichierJoint = FileUploadHelper.UploadFile(fichier, "Questions");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Erreur lors de l'upload du fichier: " + ex.Message);
                        ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom");
                        return View(question);
                    }
                }

                _context.Questions.Add(question);
                _context.SaveChanges();

                // TODO: Notifier les tuteurs du domaine
                // Cette fonctionnalité sera implémentée ultérieurement avec une approche asynchrone
                // pour éviter les problèmes de contexte et de performance

                TempData["SuccessMessage"] = "Votre question a ete envoyee avec succes !";
                return RedirectToAction("Details", new { id = question.QuestionId });
            }

            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom");
            return View(question);
        }

        // GET: Question/Details/5
        public ActionResult Details(int id)
        {
            var question = _context.Questions
                .Include(q => q.Etudiant)
                .Include(q => q.Domaine)
                .Include(q => q.Reponses.Select(r => r.Tuteur))
                .FirstOrDefault(q => q.QuestionId == id);
            if (question == null)
            {
                return HttpNotFound();
            }

            // Incrementer le nombre de vues
            question.NombreVues++;
            _context.SaveChanges();

            return View(question);
        }

        // GET: Question/Answer/5
        public ActionResult Answer(int id)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                TempData["ErrorMessage"] = "Seuls les tuteurs peuvent repondre aux questions";
                return RedirectToAction("Details", new { id });
            }

            var question = _context.Questions
                .Include(q => q.Etudiant)
                .Include(q => q.Domaine)
                .FirstOrDefault(q => q.QuestionId == id);
            if (question == null)
            {
                return HttpNotFound();
            }

            ViewBag.Question = question;
            return View();
        }

        // POST: Question/Answer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Answer(int id, Reponse reponse, HttpPostedFileBase fichier)
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Details", new { id });
            }

            var question = _context.Questions
                .Include(q => q.Etudiant)
                .FirstOrDefault(q => q.QuestionId == id);
            if (question == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = (int)Session["UserId"];
                reponse.QuestionId = id;
                reponse.TuteurId = userId;
                reponse.DateCreation = DateTime.Now;

                // Upload du fichier si present
                if (fichier != null && fichier.ContentLength > 0)
                {
                    try
                    {
                        reponse.FichierJoint = FileUploadHelper.UploadFile(fichier, "Reponses");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Erreur lors de l'upload du fichier: " + ex.Message);
                        ViewBag.Question = question;
                        return View(reponse);
                    }
                }

                _context.Reponses.Add(reponse);
                
                // Mettre a jour le statut de la question
                question.Statut = StatutQuestion.Repondue;
                
                // Calculer le delai de reponse
                var delaiReponse = DateTime.Now - question.DateCreation;
                
                // Si la reponse est validee, archiver automatiquement dans la FAQ
                if (reponse.EstValidee)
                {
                    question.Statut = StatutQuestion.Resolue;
                }
                
                _context.SaveChanges();

                // Notifier l'etudiant
                var tuteur = _context.Tuteurs.Find(userId);
                EmailHelper.SendAnswerNotificationAsync(
                    question.Etudiant.Email, 
                    question.Titre, 
                    tuteur.NomComplet
                );

                TempData["SuccessMessage"] = $"Votre reponse a ete publiee avec succes ! Delai de reponse : {delaiReponse.TotalMinutes:F0} minutes";
                return RedirectToAction("Details", new { id });
            }

            ViewBag.Question = question;
            return View(reponse);
        }

        // GET: Question/QuestionsUrgentes (pour tuteurs)
        public ActionResult QuestionsUrgentes()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = (int)Session["UserId"];
            
            // Recuperer les domaines du tuteur
            var domainesIds = _context.TuteurDomaines
                .Where(td => td.TuteurId == userId)
                .Select(td => td.DomaineId)
                .ToList();

            // Questions urgentes (proche de la limite de 2h)
            var maintenant = DateTime.Now;
            var questionsUrgentes = _context.Questions
                .Include(q => q.Etudiant)
                .Include(q => q.Domaine)
                .Where(q => domainesIds.Contains(q.DomaineId) && 
                           q.Statut == StatutQuestion.EnAttente &&
                           q.DateLimiteReponse <= maintenant.AddMinutes(30)) // Moins de 30 min restantes
                .OrderBy(q => q.DateLimiteReponse)
                .ToList();

            return View(questionsUrgentes);
        }

        // GET: Question/VerifierDelais (methode utilitaire)
        public JsonResult VerifierDelais()
        {
            var questionsEnRetard = _context.Questions
                .Where(q => q.Statut == StatutQuestion.EnAttente && 
                           q.DateLimiteReponse < DateTime.Now)
                .ToList();

            foreach (var question in questionsEnRetard)
            {
                question.EstPrioritaire = true;
            }

            _context.SaveChanges();

            return Json(new { 
                success = true, 
                questionsEnRetard = questionsEnRetard.Count 
            }, JsonRequestBehavior.AllowGet);
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

