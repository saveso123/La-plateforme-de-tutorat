using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ThotPlatform.Utils
{
    /// <summary>
    /// Classe utilitaire pour l'envoi d'emails via SMTP
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// Envoie un email de maniere asynchrone
        /// </summary>
        public static async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                var smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                var smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
                var smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
                var smtpEnableSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"]);
                var emailFrom = ConfigurationManager.AppSettings["EmailFrom"];

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = smtpEnableSsl;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(emailFrom, "Plateforme Thot"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = isHtml
                    };

                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log l'erreur (a implementer selon vos besoins)
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'envoi d'email: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Envoie un email de bienvenue a un nouvel utilisateur
        /// </summary>
        public static async Task<bool> SendWelcomeEmailAsync(string toEmail, string username, string tempPassword)
        {
            var subject = "Bienvenue sur la Plateforme Thot";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Bienvenue sur la Plateforme Thot ??</h2>
                    <p>Bonjour,</p>
                    <p>Votre compte a ete cree avec succes. Voici vos identifiants de connexion :</p>
                    <ul>
                        <li><strong>Nom d'utilisateur :</strong> {username}</li>
                        <li><strong>Mot de passe temporaire :</strong> {tempPassword}</li>
                    </ul>
                    <p><strong>Important :</strong> Vous devrez changer votre mot de passe lors de votre premiere connexion.</p>
                    <p>Connectez-vous des maintenant pour acceder a nos services de tutorat et E-learning.</p>
                    <p>Cordialement,<br/>L'equipe Thot</p>
                </body>
                </html>
            ";

            return await SendEmailAsync(toEmail, subject, body);
        }

        /// <summary>
        /// Envoie une notification de nouvelle question a un tuteur
        /// </summary>
        public static async Task<bool> SendNewQuestionNotificationAsync(string toEmail, string questionTitle, string domaine)
        {
            var subject = "Nouvelle question disponible";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Nouvelle question dans votre domaine d'expertise</h2>
                    <p>Bonjour,</p>
                    <p>Une nouvelle question a ete posee dans le domaine <strong>{domaine}</strong> :</p>
                    <p><strong>{questionTitle}</strong></p>
                    <p>Connectez-vous pour y repondre et aider un etudiant.</p>
                    <p>Cordialement,<br/>L'equipe Thot</p>
                </body>
                </html>
            ";

            return await SendEmailAsync(toEmail, subject, body);
        }

        /// <summary>
        /// Envoie une notification de reponse a un etudiant
        /// </summary>
        public static async Task<bool> SendAnswerNotificationAsync(string toEmail, string questionTitle, string tuteurNom)
        {
            var subject = "Votre question a recu une reponse";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Reponse a votre question</h2>
                    <p>Bonjour,</p>
                    <p>Votre question <strong>{questionTitle}</strong> a recu une reponse de {tuteurNom}.</p>
                    <p>Connectez-vous pour consulter la reponse complete.</p>
                    <p>Cordialement,<br/>L'equipe Thot</p>
                </body>
                </html>
            ";

            return await SendEmailAsync(toEmail, subject, body);
        }

        /// <summary>
        /// Envoie une confirmation d'abonnement
        /// </summary>
        public static async Task<bool> SendSubscriptionConfirmationAsync(string toEmail, DateTime dateDebut, DateTime dateFin, decimal montant)
        {
            var subject = "Confirmation de votre abonnement";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Abonnement confirme ??</h2>
                    <p>Bonjour,</p>
                    <p>Votre abonnement mensuel a la Plateforme Thot a ete confirme.</p>
                    <ul>
                        <li><strong>Date de debut :</strong> {dateDebut:dd/MM/yyyy}</li>
                        <li><strong>Date de fin :</strong> {dateFin:dd/MM/yyyy}</li>
                        <li><strong>Montant :</strong> {montant:C}</li>
                    </ul>
                    <p>Vous beneficiez maintenant de :</p>
                    <ul>
                        <li>Questions illimitees</li>
                        <li>Reponses garanties sous 2 heures</li>
                        <li>Sessions de clavardage</li>
                        <li>Acces complet a la FAQ</li>
                    </ul>
                    <p>Cordialement,<br/>L'equipe Thot</p>
                </body>
                </html>
            ";

            return await SendEmailAsync(toEmail, subject, body);
        }

        /// <summary>
        /// Envoie une confirmation de rencontre physique
        /// </summary>
        public static async Task<bool> SendMeetingConfirmationAsync(string toEmail, string tuteurNom, DateTime dateHeure, string lieu)
        {
            var subject = "Confirmation de rencontre physique";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Rencontre confirmee</h2>
                    <p>Bonjour,</p>
                    <p>Votre rencontre avec {tuteurNom} a ete confirmee :</p>
                    <ul>
                        <li><strong>Date et heure :</strong> {dateHeure:dd/MM/yyyy HH:mm}</li>
                        <li><strong>Lieu :</strong> {lieu}</li>
                    </ul>
                    <p>Nous vous rappelons d'etre ponctuel(le).</p>
                    <p>Cordialement,<br/>L'equipe Thot</p>
                </body>
                </html>
            ";

            return await SendEmailAsync(toEmail, subject, body);
        }
    }
}

