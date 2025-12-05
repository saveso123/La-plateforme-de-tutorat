using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace ThotPlatform.Utils
{
    /// <summary>
    /// Classe utilitaire pour la gestion des uploads de fichiers
    /// </summary>
    public static class FileUploadHelper
    {
        private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".txt", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg", ".zip" };
        private static readonly long MaxFileSize = long.Parse(ConfigurationManager.AppSettings["MaxFileSize"] ?? "10485760"); // 10 MB par defaut

        /// <summary>
        /// Televerse un fichier et retourne le chemin relatif
        /// </summary>
        public static string UploadFile(HttpPostedFileBase file, string subfolder = "")
        {
            if (file == null || file.ContentLength == 0)
                throw new ArgumentException("Aucun fichier n'a ete fourni");

            // Verifier la taille du fichier
            if (file.ContentLength > MaxFileSize)
                throw new InvalidOperationException($"Le fichier est trop volumineux. Taille maximale : {MaxFileSize / 1024 / 1024} MB");

            // Verifier l'extension
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException($"Type de fichier non autorise. Extensions autorisees : {string.Join(", ", AllowedExtensions)}");

            // Creer un nom de fichier unique
            var fileName = $"{Guid.NewGuid()}{extension}";

            // Determiner le chemin de destination
            var uploadPath = ConfigurationManager.AppSettings["UploadPath"] ?? "~/Uploads/";
            var fullPath = HttpContext.Current.Server.MapPath(uploadPath);

            if (!string.IsNullOrEmpty(subfolder))
            {
                fullPath = Path.Combine(fullPath, subfolder);
            }

            // Creer le repertoire s'il n'existe pas
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            // Sauvegarder le fichier
            var filePath = Path.Combine(fullPath, fileName);
            file.SaveAs(filePath);

            // Retourner le chemin relatif
            var relativePath = string.IsNullOrEmpty(subfolder)
                ? $"~/Uploads/{fileName}"
                : $"~/Uploads/{subfolder}/{fileName}";

            return relativePath;
        }

        /// <summary>
        /// Televerse une image et retourne le chemin relatif
        /// </summary>
        public static string UploadImage(HttpPostedFileBase file, string subfolder = "Images")
        {
            if (file == null || file.ContentLength == 0)
                throw new ArgumentException("Aucune image n'a ete fournie");

            // Verifier que c'est bien une image
            var extension = Path.GetExtension(file.FileName).ToLower();
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg" };

            if (!imageExtensions.Contains(extension))
                throw new InvalidOperationException("Le fichier doit etre une image (jpg, jpeg, png, gif, webp, bmp, svg)");

            return UploadFile(file, subfolder);
        }

        /// <summary>
        /// Televerse une video et retourne le chemin relatif
        /// </summary>
        public static string UploadVideo(HttpPostedFileBase file, string subfolder = "Videos")
        {
            if (file == null || file.ContentLength == 0)
                throw new ArgumentException("Aucune video n'a ete fournie");

            // Verifier que c'est bien une video
            var extension = Path.GetExtension(file.FileName).ToLower();
            var videoExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".flv" };

            if (!videoExtensions.Contains(extension))
                throw new InvalidOperationException("Le fichier doit etre une video (mp4, avi, mov, wmv, flv)");

            // Pour les videos, on peut autoriser une taille plus grande
            var maxVideoSize = 524288000; // 500 MB
            if (file.ContentLength > maxVideoSize)
                throw new InvalidOperationException($"La video est trop volumineuse. Taille maximale : {maxVideoSize / 1024 / 1024} MB");

            // Creer un nom de fichier unique
            var fileName = $"{Guid.NewGuid()}{extension}";

            // Determiner le chemin de destination
            var uploadPath = ConfigurationManager.AppSettings["UploadPath"] ?? "~/Uploads/";
            var fullPath = HttpContext.Current.Server.MapPath(uploadPath);
            fullPath = Path.Combine(fullPath, subfolder);

            // Creer le repertoire s'il n'existe pas
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            // Sauvegarder le fichier
            var filePath = Path.Combine(fullPath, fileName);
            file.SaveAs(filePath);

            // Retourner le chemin relatif
            return $"~/Uploads/{subfolder}/{fileName}";
        }

        /// <summary>
        /// Supprime un fichier
        /// </summary>
        public static bool DeleteFile(string relativePath)
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath))
                    return false;

                var fullPath = HttpContext.Current.Server.MapPath(relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtient la taille d'un fichier en octets
        /// </summary>
        public static long GetFileSize(string relativePath)
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath))
                    return 0;

                var fullPath = HttpContext.Current.Server.MapPath(relativePath);

                if (File.Exists(fullPath))
                {
                    var fileInfo = new FileInfo(fullPath);
                    return fileInfo.Length;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Formate la taille d'un fichier pour l'affichage
        /// </summary>
        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }
}

