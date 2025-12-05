using System;

namespace ThotPlatform.Models
{
    /// <summary>
    /// DTO pour afficher les destinataires dans les listes deroulantes
    /// </summary>
    public class DestinataireDto
    {
        public int UtilisateurId { get; set; }
        public string NomComplet { get; set; }
    }
}

