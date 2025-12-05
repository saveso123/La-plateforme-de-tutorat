using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ThotPlatform.Models;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour la FAQ publique
    /// </summary>
    public class FAQController : Controller
    {
        private readonly ThotDbContext _context;

        public FAQController()
        {
            _context = new ThotDbContext();
        }

        // GET: FAQ/Index
        public ActionResult Index(string recherche = "", int? domaineId = null)
        {
            // Recuperer toutes les questions resolues (avec au moins une reponse validee)
            var questionsQuery = _context.Questions
                .Include(q => q.Etudiant)
                .Include(q => q.Domaine)
                .Include(q => q.Reponses)
                .Where(q => q.Statut == StatutQuestion.Resolue && 
                           q.Reponses.Any(r => r.EstValidee));

            // Filtrer par domaine si specifie
            if (domaineId.HasValue)
            {
                questionsQuery = questionsQuery.Where(q => q.DomaineId == domaineId.Value);
            }

            // Recherche par mots-cles
            if (!string.IsNullOrWhiteSpace(recherche))
            {
                recherche = recherche.ToLower();
                questionsQuery = questionsQuery.Where(q => 
                    q.Titre.ToLower().Contains(recherche) || 
                    q.Contenu.ToLower().Contains(recherche) ||
                    q.Reponses.Any(r => r.Contenu.ToLower().Contains(recherche)));
            }

            var questions = questionsQuery
                .OrderByDescending(q => q.DateCreation)
                .Take(100) // Limiter a 100 resultats
                .ToList();

            // Charger les domaines pour le filtre
            ViewBag.Domaines = _context.Domaines.Where(d => d.EstActif).ToList();
            ViewBag.RechercheActuelle = recherche;
            ViewBag.DomaineActuel = domaineId;

            // Statistiques
            ViewBag.TotalQuestions = _context.Questions.Count(q => q.Statut == StatutQuestion.Resolue);
            ViewBag.DomainesCount = _context.Domaines.Count(d => d.EstActif);

            return View(questions);
        }

        // GET: FAQ/Details/5
        public ActionResult Details(int id)
        {
            var question = _context.Questions
                .Include(q => q.Etudiant)
                .Include(q => q.Domaine)
                .Include(q => q.Reponses.Select(r => r.Tuteur))
                .FirstOrDefault(q => q.QuestionId == id && q.Statut == StatutQuestion.Resolue);

            if (question == null)
            {
                TempData["ErrorMessage"] = "Question introuvable dans la FAQ";
                return RedirectToAction("Index");
            }

            // Incrementer le compteur de vues (si vous avez ce champ)
            // question.NombreVues++;
            // _context.SaveChanges();

            // Questions similaires
            ViewBag.QuestionsSimilaires = _context.Questions
                .Include(q => q.Domaine)
                .Where(q => q.DomaineId == question.DomaineId && 
                           q.QuestionId != question.QuestionId && 
                           q.Statut == StatutQuestion.Resolue)
                .OrderByDescending(q => q.DateCreation)
                .Take(5)
                .ToList();

            return View(question);
        }

        // GET: FAQ/ParDomaine/5
        public ActionResult ParDomaine(int id)
        {
            var domaine = _context.Domaines.Find(id);
            if (domaine == null)
            {
                return HttpNotFound();
            }

            var questions = _context.Questions
                .Include(q => q.Etudiant)
                .Include(q => q.Reponses)
                .Where(q => q.DomaineId == id && 
                           q.Statut == StatutQuestion.Resolue &&
                           q.Reponses.Any(r => r.EstValidee))
                .OrderByDescending(q => q.DateCreation)
                .ToList();

            ViewBag.Domaine = domaine;
            return View(questions);
        }

        // GET: FAQ/Statistiques
        public ActionResult Statistiques()
        {
            var stats = new
            {
                TotalQuestions = _context.Questions.Count(q => q.Statut == StatutQuestion.Resolue),
                TotalReponses = _context.Reponses.Count(r => r.EstValidee),
                TotalTuteurs = _context.Tuteurs.Count(t => t.EstDisponible),
                
                // Questions par domaine
                QuestionsByDomaine = _context.Questions
                    .Where(q => q.Statut == StatutQuestion.Resolue)
                    .GroupBy(q => q.Domaine.Nom)
                    .Select(g => new { Domaine = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToList(),

                // Top tuteurs
                TopTuteurs = _context.Reponses
                    .Where(r => r.EstValidee)
                    .GroupBy(r => new { r.TuteurId, r.Tuteur.Prenom, r.Tuteur.Nom })
                    .Select(g => new { 
                        Nom = g.Key.Prenom + " " + g.Key.Nom, 
                        Reponses = g.Count() 
                    })
                    .OrderByDescending(x => x.Reponses)
                    .Take(10)
                    .ToList(),

                // Questions recentes
                QuestionsRecentes = _context.Questions
                    .Where(q => q.Statut == StatutQuestion.Resolue)
                    .OrderByDescending(q => q.DateCreation)
                    .Take(10)
                    .Select(q => new { q.QuestionId, q.Titre, q.DateCreation })
                    .ToList()
            };

            return View(stats);
        }

        // GET: FAQ/Recherche (API pour autocomplete)
        public JsonResult Recherche(string term)
        {
            var resultats = _context.Questions
                .Where(q => q.Statut == StatutQuestion.Resolue && 
                           (q.Titre.Contains(term) || q.Contenu.Contains(term)))
                .OrderByDescending(q => q.DateCreation)
                .Take(10)
                .Select(q => new { 
                    id = q.QuestionId, 
                    label = q.Titre,
                    domaine = q.Domaine.Nom
                })
                .ToList();

            return Json(resultats, JsonRequestBehavior.AllowGet);
        }

        // Methode utilitaire pour archiver automatiquement les questions resolues
        public static void ArchiverQuestionsResolues(ThotDbContext context)
        {
            // Trouver toutes les questions avec au moins une reponse validee
            var questionsAArchiver = context.Questions
                .Where(q => q.Statut == StatutQuestion.EnCours && 
                           q.Reponses.Any(r => r.EstValidee))
                .ToList();

            foreach (var question in questionsAArchiver)
            {
                question.Statut = StatutQuestion.Resolue;
            }

            if (questionsAArchiver.Any())
            {
                context.SaveChanges();
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

