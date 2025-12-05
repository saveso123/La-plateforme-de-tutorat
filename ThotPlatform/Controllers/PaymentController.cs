using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ThotPlatform.Models;
using ThotPlatform.Utils;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des paiements PayPal
    /// </summary>
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ThotDbContext _context;

        public PaymentController()
        {
            _context = new ThotDbContext();
        }

        // GET: Payment/Abonnement
        public ActionResult Abonnement()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                TempData["ErrorMessage"] = "Seuls les etudiants peuvent s'abonner";
                return RedirectToAction("Index", "Home");
            }

            var userId = (int)Session["UserId"];
            var etudiant = _context.Etudiants.Find(userId);

            if (etudiant.AbonnementActif)
            {
                TempData["ErrorMessage"] = "Vous avez deja un abonnement actif";
                return RedirectToAction("MonAbonnement", "Etudiant");
            }

            var montant = decimal.Parse(ConfigurationManager.AppSettings["AbonnementMensuelPrix"], System.Globalization.CultureInfo.InvariantCulture);
            ViewBag.Montant = montant;

            return View();
        }

        // POST: Payment/CreateAbonnement
        [HttpPost]
        public async Task<ActionResult> CreateAbonnement()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = (int)Session["UserId"];
            var montant = decimal.Parse(ConfigurationManager.AppSettings["AbonnementMensuelPrix"], System.Globalization.CultureInfo.InvariantCulture);

            var returnUrl = Url.Action("ExecuteAbonnement", "Payment", null, Request.Url.Scheme);
            var cancelUrl = Url.Action("Cancel", "Payment", null, Request.Url.Scheme);

            var result = await PayPalHelper.CreatePaymentAsync(
                montant,
                "Abonnement mensuel Plateforme Thot",
                returnUrl,
                cancelUrl
            );

            if (result.Success)
            {
                // Enregistrer la transaction
                var transaction = new Transaction
                {
                    EtudiantId = userId,
                    Type = TypeTransaction.AbonnementMensuel,
                    Montant = montant,
                    Statut = StatutTransaction.EnAttente,
                    PayPalTransactionId = Guid.NewGuid().ToString(),
                    PayPalPaymentId = result.PaymentId,
                    Description = "Abonnement mensuel"
                };
                _context.Transactions.Add(transaction);
                _context.SaveChanges();

                // Rediriger vers PayPal
                return Redirect(result.ApprovalUrl);
            }
            else
            {
                TempData["ErrorMessage"] = "Erreur lors de la creation du paiement: " + result.ErrorMessage;
                return RedirectToAction("Abonnement");
            }
        }

        // GET: Payment/ExecuteAbonnement
        public async Task<ActionResult> ExecuteAbonnement(string paymentId, string PayerID)
        {
            if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(PayerID))
            {
                TempData["ErrorMessage"] = "Parametres de paiement invalides";
                return RedirectToAction("Abonnement");
            }

            var result = await PayPalHelper.ExecutePaymentAsync(paymentId, PayerID);

            if (result.Success)
            {
                var userId = (int)Session["UserId"];
                var etudiant = _context.Etudiants.Find(userId);
                var transaction = _context.Transactions
                    .FirstOrDefault(t => t.PayPalPaymentId == paymentId);

                if (transaction != null)
                {
                    transaction.Statut = StatutTransaction.Completee;
                    transaction.PayPalTransactionId = result.TransactionId;
                    transaction.PayPalStatut = result.State;
                }

                // Activer l'abonnement
                etudiant.EstAbonne = true;
                etudiant.DateDebutAbonnement = DateTime.Now;
                etudiant.DateFinAbonnement = DateTime.Now.AddMonths(1);

                _context.SaveChanges();

                // Envoyer email de confirmation
                await EmailHelper.SendSubscriptionConfirmationAsync(
                    etudiant.Email,
                    etudiant.DateDebutAbonnement.Value,
                    etudiant.DateFinAbonnement.Value,
                    transaction.Montant
                );

                TempData["SuccessMessage"] = "Abonnement active avec succes ! Valable jusqu'au " + 
                    etudiant.DateFinAbonnement.Value.ToString("dd/MM/yyyy");
                return RedirectToAction("MonAbonnement", "Etudiant");
            }
            else
            {
                TempData["ErrorMessage"] = "Erreur lors de l'execution du paiement: " + result.ErrorMessage;
                return RedirectToAction("Abonnement");
            }
        }

        // GET: Payment/SessionImmediate
        public ActionResult SessionImmediate()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                TempData["ErrorMessage"] = "Seuls les etudiants peuvent acheter des sessions";
                return RedirectToAction("Index", "Home");
            }

            var montant = decimal.Parse(ConfigurationManager.AppSettings["SessionImmediatePrix"], System.Globalization.CultureInfo.InvariantCulture);
            ViewBag.Montant = montant;
            ViewBag.Domaines = new SelectList(_context.Domaines.Where(d => d.EstActif), "DomaineId", "Nom");

            return View();
        }

        // POST: Payment/CreateSessionImmediate
        [HttpPost]
        public async Task<ActionResult> CreateSessionImmediate(int? domaineId)
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Index", "Home");
            }

            if (!domaineId.HasValue || domaineId.Value == 0)
            {
                TempData["ErrorMessage"] = "Veuillez selectionner un domaine";
                return RedirectToAction("SessionImmediate");
            }

            var userId = (int)Session["UserId"];
            var montant = decimal.Parse(ConfigurationManager.AppSettings["SessionImmediatePrix"], System.Globalization.CultureInfo.InvariantCulture);

            var returnUrl = Url.Action("ExecuteSessionImmediate", "Payment", new { domaineId }, Request.Url.Scheme);
            var cancelUrl = Url.Action("Cancel", "Payment", null, Request.Url.Scheme);

            var result = await PayPalHelper.CreatePaymentAsync(
                montant,
                "Session immediate de clavardage",
                returnUrl,
                cancelUrl
            );

            if (result.Success)
            {
                // Enregistrer la transaction
                var transaction = new Transaction
                {
                    EtudiantId = userId,
                    Type = TypeTransaction.SessionImmediate,
                    Montant = montant,
                    Statut = StatutTransaction.EnAttente,
                    PayPalTransactionId = Guid.NewGuid().ToString(),
                    PayPalPaymentId = result.PaymentId,
                    Description = "Session immediate",
                    ItemId = domaineId
                };
                _context.Transactions.Add(transaction);
                _context.SaveChanges();

                return Redirect(result.ApprovalUrl);
            }
            else
            {
                TempData["ErrorMessage"] = "Erreur lors de la creation du paiement: " + result.ErrorMessage;
                return RedirectToAction("SessionImmediate");
            }
        }

        // GET: Payment/ExecuteSessionImmediate
        public async Task<ActionResult> ExecuteSessionImmediate(string paymentId, string PayerID, int? domaineId = null)
        {
            if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(PayerID))
            {
                TempData["ErrorMessage"] = "Parametres de paiement invalides";
                return RedirectToAction("SessionImmediate");
            }

            var result = await PayPalHelper.ExecutePaymentAsync(paymentId, PayerID);

            if (result.Success)
            {
                var userId = (int)Session["UserId"];
                var transaction = _context.Transactions
                    .FirstOrDefault(t => t.PayPalPaymentId == paymentId);

                if (transaction != null)
                {
                    transaction.Statut = StatutTransaction.Completee;
                    transaction.PayPalTransactionId = result.TransactionId;
                    transaction.PayPalStatut = result.State;
                }

                // Creer la session de clavardage
                // Trouver un tuteur disponible dans ce domaine
                var tuteurDisponible = _context.TuteurDomaines
                    .Where(td => td.DomaineId == domaineId && td.Tuteur.EstDisponible)
                    .Select(td => td.Tuteur)
                    .FirstOrDefault();

                if (tuteurDisponible != null && domaineId.HasValue)
                {
                    var session = new SessionClavardage
                    {
                        EtudiantId = userId,
                        TuteurId = tuteurDisponible.UtilisateurId,
                        DomaineId = domaineId.Value,
                        Type = TypeSession.Immediate,
                        Statut = StatutSession.EnAttente,
                        Cout = transaction.Montant,
                        DateDebut = DateTime.Now
                    };
                    _context.SessionsClavardage.Add(session);
                    transaction.ItemId = session.SessionId;
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Paiement effectue avec succes ! Un tuteur va vous contacter sous peu.";
                return RedirectToAction("Index", "Etudiant");
            }
            else
            {
                TempData["ErrorMessage"] = "Erreur lors de l'execution du paiement: " + result.ErrorMessage;
                return RedirectToAction("SessionImmediate");
            }
        }

        // GET: Payment/Cancel
        public ActionResult Cancel()
        {
            TempData["ErrorMessage"] = "Paiement annule";
            return RedirectToAction("Index", "Home");
        }

        // GET: Payment/Historique
        public ActionResult Historique()
        {
            if (Session["UserType"]?.ToString() != "Etudiant")
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = (int)Session["UserId"];
            var transactions = _context.Transactions
                .Where(t => t.EtudiantId == userId)
                .OrderByDescending(t => t.DateTransaction)
                .ToList();

            return View(transactions);
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

