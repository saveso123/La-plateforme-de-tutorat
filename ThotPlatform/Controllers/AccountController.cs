using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ThotPlatform.Models;
using ThotPlatform.Utils;
using ThotPlatform.ViewModels;

namespace ThotPlatform.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des comptes utilisateurs (authentification, inscription)
    /// </summary>
    public class AccountController : Controller
    {
        private readonly ThotDbContext _context;

        public AccountController()
        {
            _context = new ThotDbContext();
        }

        // GET: Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Rechercher l'utilisateur (Etudiant ou Tuteur)
            var etudiant = _context.Etudiants.FirstOrDefault(e => e.Username == model.Username && e.EstActif);
            var tuteur = _context.Tuteurs.FirstOrDefault(t => t.Username == model.Username && t.EstActif);

            Utilisateur utilisateur = (Utilisateur)etudiant ?? tuteur;

            if (utilisateur == null)
            {
                ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect");
                return View(model);
            }

            // Verifier le mot de passe
            if (!PasswordHelper.VerifyPassword(model.Password, utilisateur.MotDePasse))
            {
                ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect");
                return View(model);
            }

            // Mettre a jour la derniere connexion
            utilisateur.DerniereConnexion = DateTime.Now;
            _context.SaveChanges();

            // Creer le cookie d'authentification
            FormsAuthentication.SetAuthCookie(utilisateur.Username, model.RememberMe);

            // Stocker les informations dans la session
            Session["UserId"] = utilisateur.UtilisateurId;
            Session["Username"] = utilisateur.Username;
            Session["UserType"] = utilisateur is Etudiant ? "Etudiant" : "Tuteur";
            Session["NomComplet"] = utilisateur.NomComplet;

            // Verifier si c'est la premiere connexion
            if (!utilisateur.PremierChangementMotDePasse)
            {
                return RedirectToAction("ChangePassword", new { firstTime = true });
            }

            // Redirection selon le type d'utilisateur
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (utilisateur is Etudiant)
            {
                return RedirectToAction("Index", "Etudiant");
            }
            else
            {
                return RedirectToAction("Index", "Tuteur");
            }
        }

        // GET: Account/Register
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verifier si l'email existe deja
            var emailExists = _context.Etudiants.Any(e => e.Email == model.Email) ||
                            _context.Tuteurs.Any(t => t.Email == model.Email);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Cette adresse email est deja utilisee");
                return View(model);
            }

            // Verifier si le username existe deja
            var usernameExists = _context.Etudiants.Any(e => e.Username == model.Username) ||
                               _context.Tuteurs.Any(t => t.Username == model.Username);

            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Ce nom d'utilisateur est deja utilise");
                return View(model);
            }

            // Creer le nouvel etudiant
            var etudiant = new Etudiant
            {
                Nom = model.Nom,
                Prenom = model.Prenom,
                Email = model.Email,
                Username = model.Username,
                IdentifiantUnique = Utils.IdentifiantHelper.GenererIdentifiantEtudiant(_context),
                MotDePasse = PasswordHelper.HashPassword(model.Password),
                Telephone = model.Telephone,
                Niveau = model.Niveau,
                Etablissement = model.Etablissement,
                Ville = model.Ville,
                LanguePreferee = "fr",
                EstActif = true,
                EstAbonne = false,
                PremierChangementMotDePasse = true
            };

            _context.Etudiants.Add(etudiant);
            _context.SaveChanges();

            // Envoyer un email de bienvenue (asynchrone, sans attendre)
            EmailHelper.SendWelcomeEmailAsync(etudiant.Email, etudiant.Username, "Votre mot de passe");

            TempData["SuccessMessage"] = "Inscription reussie ! Vous pouvez maintenant vous connecter.";
            return RedirectToAction("Login");
        }

        // GET: Account/ChangePassword
        [HttpGet]
        public ActionResult ChangePassword(bool firstTime = false)
        {
            ViewBag.FirstTime = firstTime;
            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = (int)Session["UserId"];
            var userType = Session["UserType"].ToString();

            Utilisateur utilisateur;
            if (userType == "Etudiant")
            {
                utilisateur = _context.Etudiants.Find(userId);
            }
            else
            {
                utilisateur = _context.Tuteurs.Find(userId);
            }

            if (utilisateur == null)
            {
                return RedirectToAction("Login");
            }

            // Verifier l'ancien mot de passe
            if (!PasswordHelper.VerifyPassword(model.OldPassword, utilisateur.MotDePasse))
            {
                ModelState.AddModelError("OldPassword", "L'ancien mot de passe est incorrect");
                return View(model);
            }

            // Mettre a jour le mot de passe
            utilisateur.MotDePasse = PasswordHelper.HashPassword(model.NewPassword);
            utilisateur.PremierChangementMotDePasse = true;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Mot de passe modifie avec succes";

            // Rediriger selon le type d'utilisateur
            if (userType == "Etudiant")
            {
                return RedirectToAction("Index", "Etudiant");
            }
            else
            {
                return RedirectToAction("Index", "Tuteur");
            }
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
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

