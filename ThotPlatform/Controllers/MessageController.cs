using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ThotPlatform.Models;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour la gestion de la messagerie
    /// </summary>
    [Authorize]
    public class MessageController : Controller
    {
        private readonly ThotDbContext _context;

        public MessageController()
        {
            _context = new ThotDbContext();
        }

        // GET: Message
        public ActionResult Index()
        {
            var userId = (int)Session["UserId"];
            
            var messagesRecus = _context.Messages
                .Include(m => m.Expediteur)
                .Where(m => m.DestinatireId == userId)
                .OrderByDescending(m => m.DateEnvoi)
                .ToList();

            var messagesEnvoyes = _context.Messages
                .Include(m => m.Destinataire)
                .Where(m => m.ExpediteurId == userId)
                .OrderByDescending(m => m.DateEnvoi)
                .ToList();

            ViewBag.MessagesRecus = messagesRecus;
            ViewBag.MessagesEnvoyes = messagesEnvoyes;
            ViewBag.NombreNonLus = messagesRecus.Count(m => !m.EstLu);

            return View();
        }

        // GET: Message/Nouveau
        public ActionResult Nouveau(int? destinataireId)
        {
            if (destinataireId.HasValue)
            {
                var destinataire = _context.Set<Utilisateur>().Find(destinataireId.Value);
                if (destinataire != null)
                {
                    ViewBag.DestinataireName = destinataire.NomComplet;
                    ViewBag.DestinatireId = destinataireId.Value;
                }
            }

            ChargerListesDestinataires();
            return View();
        }

        // POST: Message/Nouveau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Nouveau(Message message)
        {
            if (ModelState.IsValid)
            {
                message.ExpediteurId = (int)Session["UserId"];
                message.DateEnvoi = DateTime.Now;
                message.EstLu = false;

                _context.Messages.Add(message);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Message envoye avec succes !";
                return RedirectToAction("Index");
            }

            ChargerListesDestinataires();
            return View(message);
        }

        /// <summary>
        /// Charge les listes de destinataires possibles selon le type d'utilisateur
        /// </summary>
        private void ChargerListesDestinataires()
        {
            // Liste des tuteurs pour les etudiants
            if (Session["UserType"]?.ToString() == "Etudiant")
            {
                ViewBag.Tuteurs = _context.Tuteurs
                    .Select(t => new DestinataireDto
                    { 
                        UtilisateurId = t.UtilisateurId, 
                        NomComplet = t.Prenom + " " + t.Nom 
                    })
                    .ToList();
            }
            // Liste des etudiants pour les tuteurs
            else if (Session["UserType"]?.ToString() == "Tuteur")
            {
                ViewBag.Etudiants = _context.Etudiants
                    .Select(e => new DestinataireDto
                    { 
                        UtilisateurId = e.UtilisateurId, 
                        NomComplet = e.Prenom + " " + e.Nom 
                    })
                    .ToList();
            }
        }

        // GET: Message/Lire/5
        public ActionResult Lire(int id)
        {
            var userId = (int)Session["UserId"];
            var message = _context.Messages
                .Include(m => m.Expediteur)
                .Include(m => m.Destinataire)
                .FirstOrDefault(m => m.MessageId == id && 
                    (m.ExpediteurId == userId || m.DestinatireId == userId));

            if (message == null)
                return HttpNotFound();

            // Marquer comme lu si c'est le destinataire
            if (message.DestinatireId == userId && !message.EstLu)
            {
                message.EstLu = true;
                message.DateLecture = DateTime.Now;
                _context.SaveChanges();
            }

            return View(message);
        }

        // GET: Message/Repondre/5
        public ActionResult Repondre(int id)
        {
            var userId = (int)Session["UserId"];
            var messageOriginal = _context.Messages
                .Include(m => m.Expediteur)
                .FirstOrDefault(m => m.MessageId == id && m.DestinatireId == userId);

            if (messageOriginal == null)
                return HttpNotFound();

            ViewBag.MessageOriginal = messageOriginal;
            ViewBag.DestinatireId = messageOriginal.ExpediteurId;
            ViewBag.DestinataireName = messageOriginal.Expediteur.NomComplet;

            var reponse = new Message
            {
                DestinatireId = messageOriginal.ExpediteurId,
                Sujet = "RE: " + messageOriginal.Sujet,
                ConversationId = messageOriginal.ConversationId ?? messageOriginal.MessageId
            };

            ChargerListesDestinataires();
            return View("Nouveau", reponse);
        }

        // POST: Message/Supprimer/5
        [HttpPost]
        public JsonResult Supprimer(int id)
        {
            try
            {
                var userId = (int)Session["UserId"];
                var message = _context.Messages
                    .FirstOrDefault(m => m.MessageId == id && 
                        (m.ExpediteurId == userId || m.DestinatireId == userId));

                if (message == null)
                    return Json(new { success = false, message = "Message introuvable" });

                _context.Messages.Remove(message);
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Message/Conversation/5
        public ActionResult Conversation(int id)
        {
            var userId = (int)Session["UserId"];
            
            // Recuperer tous les messages de la conversation
            var messages = _context.Messages
                .Include(m => m.Expediteur)
                .Include(m => m.Destinataire)
                .Where(m => (m.ConversationId == id || m.MessageId == id) &&
                    (m.ExpediteurId == userId || m.DestinatireId == userId))
                .OrderBy(m => m.DateEnvoi)
                .ToList();

            if (!messages.Any())
                return HttpNotFound();

            // Marquer tous les messages recus comme lus
            foreach (var msg in messages.Where(m => m.DestinatireId == userId && !m.EstLu))
            {
                msg.EstLu = true;
                msg.DateLecture = DateTime.Now;
            }
            _context.SaveChanges();

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

