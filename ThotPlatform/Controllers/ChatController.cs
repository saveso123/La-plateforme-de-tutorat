using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ThotPlatform.Models;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des sessions de clavardage
    /// </summary>
    public class ChatController : Controller
    {
        private readonly ThotDbContext _context;

        public ChatController()
        {
            _context = new ThotDbContext();
        }

        // GET: Chat/MesSessions
        public ActionResult MesSessions()
        {
            var userId = (int)Session["UserId"];
            var userType = Session["UserType"]?.ToString();

            var sessions = userType == "Etudiant"
                ? _context.SessionsClavardage
                    .Include(s => s.Tuteur)
                    .Include(s => s.Domaine)
                    .Where(s => s.EtudiantId == userId)
                    .OrderByDescending(s => s.DateDebut)
                    .ToList()
                : _context.SessionsClavardage
                    .Include(s => s.Etudiant)
                    .Include(s => s.Domaine)
                    .Where(s => s.TuteurId == userId)
                    .OrderByDescending(s => s.DateDebut)
                    .ToList();

            return View(sessions);
        }

        // GET: Chat/Session/5
        public ActionResult Session(int id)
        {
            var userId = (int)Session["UserId"];
            var session = _context.SessionsClavardage
                .Include(s => s.Etudiant)
                .Include(s => s.Tuteur)
                .Include(s => s.Domaine)
                .Include(s => s.Messages)
                .FirstOrDefault(s => s.SessionId == id);

            if (session == null)
                return HttpNotFound();

            // Verifier l'acces
            if (session.EtudiantId != userId && session.TuteurId != userId)
                return HttpNotFound();

            // Marquer les messages comme lus
            var messagesNonLus = session.Messages.Where(m => m.UtilisateurId != userId && !m.EstLu).ToList();
            foreach (var msg in messagesNonLus)
            {
                msg.EstLu = true;
            }
            _context.SaveChanges();

            return View(session);
        }

        // POST: Chat/StartSession
        [HttpPost]
        public ActionResult StartSession(int sessionId)
        {
            var userId = (int)Session["UserId"];
            var session = _context.SessionsClavardage.FirstOrDefault(s => s.SessionId == sessionId);

            if (session == null || (session.EtudiantId != userId && session.TuteurId != userId))
                return HttpNotFound();

            session.Statut = StatutSession.EnCours;
            session.DateDebut = DateTime.Now;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Session demarree";
            return RedirectToAction("Session", new { id = sessionId });
        }

        // POST: Chat/EndSession
        [HttpPost]
        public ActionResult EndSession(int sessionId)
        {
            var userId = (int)Session["UserId"];
            var session = _context.SessionsClavardage.FirstOrDefault(s => s.SessionId == sessionId);

            if (session == null || (session.EtudiantId != userId && session.TuteurId != userId))
                return HttpNotFound();

            session.Statut = StatutSession.Terminee;
            session.DateFin = DateTime.Now;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Session terminee";
            return RedirectToAction("MesSessions");
        }

        // POST: Chat/RateSession
        [HttpPost]
        public ActionResult RateSession(int sessionId, int note, string commentaire)
        {
            var userId = (int)Session["UserId"];
            var session = _context.SessionsClavardage.FirstOrDefault(s => s.SessionId == sessionId && s.EtudiantId == userId);

            if (session == null)
                return HttpNotFound();

            session.NoteEtudiant = note;
            session.Commentaire = commentaire;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "evaluation enregistree";
            return RedirectToAction("MesSessions");
        }

        // GET: Chat/SessionsEnAttente (pour tuteur)
        public ActionResult SessionsEnAttente()
        {
            if (Session["UserType"]?.ToString() != "Tuteur")
                return RedirectToAction("Index", "Home");

            var userId = (int)Session["UserId"];
            var sessions = _context.SessionsClavardage
                .Include(s => s.Etudiant)
                .Include(s => s.Domaine)
                .Where(s => s.TuteurId == userId && s.Statut == StatutSession.EnAttente)
                .OrderBy(s => s.DateDebut)
                .AsNoTracking()
                .ToList();

            return View(sessions);
        }

        // GET: Chat/MesMessages
        public ActionResult MesMessages()
        {
            var userId = (int)Session["UserId"];
            var messages = _context.Messages
                .Include(m => m.Expediteur)
                .Include(m => m.Destinataire)
                .Where(m => m.DestinatireId == userId || m.ExpediteurId == userId)
                .OrderByDescending(m => m.DateEnvoi)
                .ToList();

            return View(messages);
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
