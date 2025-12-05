using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using ThotPlatform.Models;

namespace ThotPlatform.Hubs
{
    /// <summary>
    /// Hub SignalR pour le clavardage en temps reel
    /// </summary>
    public class ChatHub : Hub
    {
        private readonly ThotDbContext _context;

        public ChatHub()
        {
            _context = new ThotDbContext();
        }

        /// <summary>
        /// Envoie un message dans une session de clavardage
        /// </summary>
        public async Task SendMessage(int sessionId, string message)
        {
            try
            {
                var userId = int.Parse(Context.User?.Identity?.Name ?? "0");
                if (userId == 0) return;

                // Verifier que l'utilisateur fait partie de la session
                var session = _context.SessionsClavardage.FirstOrDefault(s => s.SessionId == sessionId);
                if (session == null) return;

                if (session.EtudiantId != userId && session.TuteurId != userId) return;

                // Creer le message
                var messageObj = new MessageClavardage
                {
                    SessionId = sessionId,
                    UtilisateurId = userId,
                    Contenu = message,
                    DateEnvoi = DateTime.Now,
                    EstLu = false
                };

                _context.MessagesClavardage.Add(messageObj);
                _context.SaveChanges();

                // Determiner le type d'utilisateur
                var etudiant = _context.Etudiants.FirstOrDefault(e => e.UtilisateurId == userId);
                var tuteur = _context.Tuteurs.FirstOrDefault(t => t.UtilisateurId == userId);

                var nomUtilisateur = etudiant != null 
                    ? $"{etudiant.Prenom} {etudiant.Nom}" 
                    : $"{tuteur.Prenom} {tuteur.Nom}";

                var typeUtilisateur = etudiant != null ? "Etudiant" : "Tuteur";

                // Envoyer le message a tous les clients de la session
                await Clients.Group($"session_{sessionId}").SendAsync("ReceiveMessage", new
                {
                    messageId = messageObj.MessageId,
                    utilisateurId = userId,
                    nomUtilisateur = nomUtilisateur,
                    typeUtilisateur = typeUtilisateur,
                    contenu = message,
                    dateEnvoi = messageObj.DateEnvoi.ToString("HH:mm:ss"),
                    estLu = false
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Erreur: {ex.Message}");
            }
        }

        /// <summary>
        /// Rejoint un groupe de session
        /// </summary>
        public async Task JoinSession(int sessionId)
        {
            try
            {
                var userId = int.Parse(Context.User?.Identity?.Name ?? "0");
                if (userId == 0) return;

                var session = _context.SessionsClavardage.FirstOrDefault(s => s.SessionId == sessionId);
                if (session == null) return;

                if (session.EtudiantId != userId && session.TuteurId != userId) return;

                await Groups.AddToGroupAsync(Context.ConnectionId, $"session_{sessionId}");

                // Notifier les autres utilisateurs
                await Clients.OthersInGroup($"session_{sessionId}").SendAsync("UserJoined", new
                {
                    utilisateurId = userId,
                    dateJoined = DateTime.Now.ToString("HH:mm:ss")
                });

                // Charger l'historique des messages
                var messages = _context.MessagesClavardage
                    .Where(m => m.SessionId == sessionId)
                    .OrderBy(m => m.DateEnvoi)
                    .ToList();

                foreach (var msg in messages)
                {
                    var user = _context.Etudiants.FirstOrDefault(e => e.UtilisateurId == msg.UtilisateurId) as Utilisateur
                        ?? _context.Tuteurs.FirstOrDefault(t => t.UtilisateurId == msg.UtilisateurId) as Utilisateur;

                    if (user != null)
                    {
                        await Clients.Caller.SendAsync("ReceiveMessage", new
                        {
                            messageId = msg.MessageId,
                            utilisateurId = msg.UtilisateurId,
                            nomUtilisateur = $"{user.Prenom} {user.Nom}",
                            typeUtilisateur = _context.Etudiants.Any(e => e.UtilisateurId == msg.UtilisateurId) ? "Etudiant" : "Tuteur",
                            contenu = msg.Contenu,
                            dateEnvoi = msg.DateEnvoi.ToString("HH:mm:ss"),
                            estLu = msg.EstLu
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Erreur: {ex.Message}");
            }
        }

        /// <summary>
        /// Quitte une session
        /// </summary>
        public async Task LeaveSession(int sessionId)
        {
            try
            {
                var userId = int.Parse(Context.User?.Identity?.Name ?? "0");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session_{sessionId}");

                await Clients.OthersInGroup($"session_{sessionId}").SendAsync("UserLeft", new
                {
                    utilisateurId = userId,
                    dateLeft = DateTime.Now.ToString("HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Erreur: {ex.Message}");
            }
        }

        /// <summary>
        /// Marque les messages comme lus
        /// </summary>
        public async Task MarkMessagesAsRead(int sessionId)
        {
            try
            {
                var userId = int.Parse(Context.User?.Identity?.Name ?? "0");
                var messages = _context.MessagesClavardage
                    .Where(m => m.SessionId == sessionId && m.UtilisateurId != userId && !m.EstLu)
                    .ToList();

                foreach (var msg in messages)
                {
                    msg.EstLu = true;
                }

                _context.SaveChanges();

                await Clients.Group($"session_{sessionId}").SendAsync("MessagesRead", new
                {
                    sessionId = sessionId,
                    dateRead = DateTime.Now.ToString("HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Erreur: {ex.Message}");
            }
        }

        public override async Task OnConnected()
        {
            await base.OnConnected();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await base.OnDisconnected(stopCalled);
        }
    }
}
