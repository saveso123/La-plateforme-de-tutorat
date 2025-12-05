using System;
using ThotPlatform.Models;

namespace ThotPlatform.ViewModels
{
    /// <summary>
    /// ViewModel pour afficher les etudiants inscrits a un cours
    /// </summary>
    public class EtudiantCoursViewModel
    {
        public int UtilisateurId { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public NiveauScolaire Niveau { get; set; }
        public DateTime DateInscription { get; set; }
    }
}
