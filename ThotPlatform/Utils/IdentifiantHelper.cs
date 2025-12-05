using System;
using System.Linq;
using ThotPlatform.Models;

namespace ThotPlatform.Utils
{
    public static class IdentifiantHelper
    {
        /// <summary>
        /// Genere un identifiant unique pour un etudiant (format: ETU00001)
        /// </summary>
        public static string GenererIdentifiantEtudiant(ThotDbContext context)
        {
            var dernierEtudiant = context.Etudiants
                .OrderByDescending(e => e.UtilisateurId)
                .FirstOrDefault();
            
            int prochainId = dernierEtudiant != null ? dernierEtudiant.UtilisateurId + 1 : 1;
            return $"ETU{prochainId:D5}";
        }
        
        /// <summary>
        /// Genere un identifiant unique pour un tuteur (format: TUT00001)
        /// </summary>
        public static string GenererIdentifiantTuteur(ThotDbContext context)
        {
            var dernierTuteur = context.Tuteurs
                .OrderByDescending(t => t.UtilisateurId)
                .FirstOrDefault();
            
            int prochainId = dernierTuteur != null ? dernierTuteur.UtilisateurId + 1 : 1;
            return $"TUT{prochainId:D5}";
        }
        
        /// <summary>
        /// Verifie si un identifiant unique existe deja
        /// </summary>
        public static bool IdentifiantExiste(ThotDbContext context, string identifiant)
        {
            return context.Etudiants.Any(e => e.IdentifiantUnique == identifiant) ||
                   context.Tuteurs.Any(t => t.IdentifiantUnique == identifiant);
        }
    }
}

