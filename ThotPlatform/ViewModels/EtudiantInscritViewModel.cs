using System;
using ThotPlatform.Models;

namespace ThotPlatform.ViewModels
{
    /// <summary>
    /// ViewModel pour afficher les etudiants inscrits aux cours d'un tuteur
    /// </summary>
    public class EtudiantInscritViewModel
    {
        public int UtilisateurId { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public NiveauScolaire Niveau { get; set; }
        public int NombreCours { get; set; }
        public DateTime DerniereInscription { get; set; }
    }
}
