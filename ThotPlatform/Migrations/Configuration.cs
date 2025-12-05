using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using ThotPlatform.Models;
using ThotPlatform.Utils;

namespace ThotPlatform.Migrations
{
    /// <summary>
    /// Configuration des migrations Entity Framework
    /// </summary>
    internal sealed class Configuration : DbMigrationsConfiguration<ThotDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "ThotPlatform.Models.ThotDbContext";
        }

        protected override void Seed(ThotDbContext context)
        {
            // Nettoyer les donnees existantes (sauf utilisateurs)
            CleanDatabase(context);
            
            // Seed les domaines
            SeedDomaines(context);
            context.SaveChanges();
            
            // Seed les tuteurs (garder les utilisateurs existants)
            SeedTuteurs(context);
            context.SaveChanges();
            
            // Seed les cours professionnels
            SeedCours(context);
            context.SaveChanges();
            
            // Seed les modules et ressources
            SeedModulesEtRessources(context);
            context.SaveChanges();
            
            // Seed les quiz
            SeedQuiz(context);
            context.SaveChanges();
            
            // Seed les questions et reponses
            SeedQuestionsEtReponses(context);
            context.SaveChanges();
            
            // Seed les sessions de clavardage
            SeedSessionsClavardage(context);
            context.SaveChanges();
            
            // Seed les rencontres physiques
            SeedRencontresPhysiques(context);
            context.SaveChanges();
            
            // Seed les inscriptions aux cours
            SeedInscriptionsCours(context);
            context.SaveChanges();
            
            // Seed les transactions
            SeedTransactions(context);
            context.SaveChanges();
            
            // Seed les messages
            SeedMessages(context);
            context.SaveChanges();
        }

        private void CleanDatabase(ThotDbContext context)
        {
            // Desactiver les contraintes de cles etrangeres
            context.Database.ExecuteSqlCommand("EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");

            // Supprimer les donnees non-utilisateurs dans le bon ordre
            context.ProgressionsModules.RemoveRange(context.ProgressionsModules);
            context.InscriptionsCours.RemoveRange(context.InscriptionsCours);
            context.RessourcesModules.RemoveRange(context.RessourcesModules);
            context.Quizs.RemoveRange(context.Quizs);
            context.Modules.RemoveRange(context.Modules);
            context.Cours.RemoveRange(context.Cours);
            context.Questions.RemoveRange(context.Questions);
            context.Reponses.RemoveRange(context.Reponses);
            context.SessionsClavardage.RemoveRange(context.SessionsClavardage);
            context.RencontresPhysiques.RemoveRange(context.RencontresPhysiques);
            context.Transactions.RemoveRange(context.Transactions);
            context.SaveChanges();
        }

        private void SeedDomaines(ThotDbContext context)
        {
            var domaines = new List<Domaine>
            {
                new Domaine
                {
                    DomaineId = 1,
                    Nom = "Mathematiques",
                    Description = "Algebre, geometrie, calcul differentiel et integral, statistiques",
                    Icone = "calculator",
                    EstActif = true
                },
                new Domaine
                {
                    DomaineId = 2,
                    Nom = "Physique",
                    Description = "Mecanique, electricite, magnetisme, optique, thermodynamique",
                    Icone = "atom",
                    EstActif = true
                },
                new Domaine
                {
                    DomaineId = 3,
                    Nom = "Chimie",
                    Description = "Chimie organique, inorganique, analytique, physique",
                    Icone = "flask",
                    EstActif = true
                },
                new Domaine
                {
                    DomaineId = 4,
                    Nom = "Informatique",
                    Description = "Programmation, algorithmique, bases de donnees, reseaux",
                    Icone = "code",
                    EstActif = true
                },
                new Domaine
                {
                    DomaineId = 5,
                    Nom = "Electronique",
                    Description = "Circuits electroniques, systemes embarques, microcontroleurs",
                    Icone = "cpu",
                    EstActif = true
                },
                new Domaine
                {
                    DomaineId = 6,
                    Nom = "Biologie",
                    Description = "Biologie cellulaire, genetique, ecologie, anatomie",
                    Icone = "dna",
                    EstActif = true
                }
            };

            context.Domaines.AddOrUpdate(d => d.DomaineId, domaines.ToArray());
        }

        private void SeedTuteurs(ThotDbContext context)
        {
            var tuteurs = new List<Tuteur>
            {
                new Tuteur
                {
                    UtilisateurId = 1,
                    Nom = "Durand",
                    Prenom = "Marie",
                    Email = "marie.durand@thotplatform.com",
                    Username = "marie.durand",
                    MotDePasse = PasswordHelper.HashPassword("Tuteur123!"),
                    Telephone = "514-555-0101",
                    Biographie = "Enseignante de mathematiques avec 10 ans d'experience en algebre lineaire et calcul.",
                    Diplomes = "Maitrise en mathematiques appliquees",
                    AnneesExperience = 10,
                    DisponiblePhysique = true,
                    TarifHorairePhysique = 35.00m,
                    Ville = "Montreal",
                    EstActif = true,
                    EstDisponible = true,
                    PremierChangementMotDePasse = true
                },
                new Tuteur
                {
                    UtilisateurId = 3,
                    Nom = "Gagnon",
                    Prenom = "Pierre",
                    Email = "pierre.gagnon@thotplatform.com",
                    Username = "pierre.gagnon",
                    MotDePasse = PasswordHelper.HashPassword("Tuteur123!"),
                    Telephone = "514-555-0102",
                    Biographie = "Physicien specialise en mecanique classique et electromagnetisme avec 8 ans d'experience.",
                    Diplomes = "Doctorat en physique theorique",
                    AnneesExperience = 8,
                    DisponiblePhysique = true,
                    TarifHorairePhysique = 40.00m,
                    Ville = "Montreal",
                    EstActif = true,
                    EstDisponible = true,
                    PremierChangementMotDePasse = true
                },
                new Tuteur
                {
                    UtilisateurId = 4,
                    Nom = "Cote",
                    Prenom = "Sophie",
                    Email = "sophie.cote@thotplatform.com",
                    Username = "sophie.cote",
                    MotDePasse = PasswordHelper.HashPassword("Tuteur123!"),
                    Telephone = "514-555-0103",
                    Biographie = "Chimiste organique avec 12 ans d'experience en synthese et reactions organiques.",
                    Diplomes = "Maitrise en chimie organique",
                    AnneesExperience = 12,
                    DisponiblePhysique = true,
                    TarifHorairePhysique = 38.00m,
                    Ville = "Laval",
                    EstActif = true,
                    EstDisponible = true,
                    PremierChangementMotDePasse = true
                },
                new Tuteur
                {
                    UtilisateurId = 5,
                    Nom = "Bouchard",
                    Prenom = "Marc",
                    Email = "marc.bouchard@thotplatform.com",
                    Username = "marc.bouchard",
                    MotDePasse = PasswordHelper.HashPassword("Tuteur123!"),
                    Telephone = "514-555-0104",
                    Biographie = "Developpeur senior en C# et architecte logiciel avec 15 ans d'experience.",
                    Diplomes = "Baccalaureat en informatique",
                    AnneesExperience = 15,
                    DisponiblePhysique = true,
                    TarifHorairePhysique = 45.00m,
                    Ville = "Montreal",
                    EstActif = true,
                    EstDisponible = true,
                    PremierChangementMotDePasse = true
                },
                new Tuteur
                {
                    UtilisateurId = 6,
                    Nom = "Leclerc",
                    Prenom = "Isabelle",
                    Email = "isabelle.leclerc@thotplatform.com",
                    Username = "isabelle.leclerc",
                    MotDePasse = PasswordHelper.HashPassword("Tuteur123!"),
                    Telephone = "514-555-0105",
                    Biographie = "Ingenieure electronique specialisee en circuits numeriques et microcontroleurs.",
                    Diplomes = "Baccalaureat en genie electronique",
                    AnneesExperience = 9,
                    DisponiblePhysique = true,
                    TarifHorairePhysique = 42.00m,
                    Ville = "Boucherville",
                    EstActif = true,
                    EstDisponible = true,
                    PremierChangementMotDePasse = true
                },
                new Tuteur
                {
                    UtilisateurId = 7,
                    Nom = "Roy",
                    Prenom = "Luc",
                    Email = "luc.roy@thotplatform.com",
                    Username = "luc.roy",
                    MotDePasse = PasswordHelper.HashPassword("Tuteur123!"),
                    Telephone = "514-555-0106",
                    Biographie = "Biologiste moleculaire avec expertise en genetique et biologie cellulaire.",
                    Diplomes = "Maitrise en biologie moleculaire",
                    AnneesExperience = 11,
                    DisponiblePhysique = true,
                    TarifHorairePhysique = 39.00m,
                    Ville = "Montreal",
                    EstActif = true,
                    EstDisponible = true,
                    PremierChangementMotDePasse = true
                }
            };

            foreach (var tuteur in tuteurs)
            {
                context.Tuteurs.AddOrUpdate(t => t.Email, tuteur);
            }
            context.SaveChanges();

            // Ajouter les domaines d'expertise pour chaque tuteur
            var tuteurDomaines = new List<TuteurDomaine>
            {
                // Marie Durand - Mathematiques et Physique
                new TuteurDomaine { TuteurId = 1, DomaineId = 1, NiveauExpertise = NiveauExpertise.Expert },
                new TuteurDomaine { TuteurId = 1, DomaineId = 2, NiveauExpertise = NiveauExpertise.Avance },
                
                // Pierre Gagnon - Physique
                new TuteurDomaine { TuteurId = 3, DomaineId = 2, NiveauExpertise = NiveauExpertise.Expert },
                
                // Sophie Cote - Chimie
                new TuteurDomaine { TuteurId = 4, DomaineId = 3, NiveauExpertise = NiveauExpertise.Expert },
                
                // Marc Bouchard - Informatique
                new TuteurDomaine { TuteurId = 5, DomaineId = 4, NiveauExpertise = NiveauExpertise.Expert },
                
                // Isabelle Leclerc - electronique et Informatique
                new TuteurDomaine { TuteurId = 6, DomaineId = 5, NiveauExpertise = NiveauExpertise.Expert },
                new TuteurDomaine { TuteurId = 6, DomaineId = 4, NiveauExpertise = NiveauExpertise.Avance },
                
                // Luc Roy - Biologie
                new TuteurDomaine { TuteurId = 7, DomaineId = 6, NiveauExpertise = NiveauExpertise.Expert }
            };

            foreach (var td in tuteurDomaines)
            {
                if (!context.TuteurDomaines.Any(x => x.TuteurId == td.TuteurId && x.DomaineId == td.DomaineId))
                {
                    context.TuteurDomaines.Add(td);
                }
            }
        }

        private void SeedEtudiants(ThotDbContext context)
        {
            var etudiant = new Etudiant
            {
                UtilisateurId = 2,
                Nom = "Tremblay",
                Prenom = "Jean",
                Email = "jean.tremblay@example.com",
                Username = "jean.tremblay",
                MotDePasse = PasswordHelper.HashPassword("Etudiant123!"),
                Telephone = "514-555-0202",
                Niveau = NiveauScolaire.Collegial,
                Etablissement = "Cegep de Montreal",
                Ville = "Montreal",
                EstActif = true,
                EstAbonne = false,
                PremierChangementMotDePasse = false
            };

            context.Etudiants.AddOrUpdate(e => e.Email, etudiant);
        }

        private void SeedCours(ThotDbContext context)
        {
            var tuteur = context.Tuteurs.FirstOrDefault();
            if (tuteur == null) return;

            var cours = new List<Cours>
            {
                new Cours
                {
                    Nom = "Algebre Lineaire Appliquee",
                    Code = "MAT-201",
                    Description = "Maitrisez les concepts fondamentaux de l'algebre lineaire avec applications pratiques en ingenierie et informatique.",
                    TuteurId = tuteur.UtilisateurId,
                    DomaineId = 1,
                    Niveau = NiveauScolaire.Collegial,
                    NombreModules = 8,
                    DureeEstimeeHeures = 40,
                    EstPublie = true,
                    DateCreation = DateTime.Now.AddMonths(-3)
                },
                new Cours
                {
                    Nom = "Calcul Differentiel et Integral",
                    Code = "MAT-202",
                    Description = "Apprenez le calcul avance avec des exercices pratiques et des applications reelles.",
                    TuteurId = tuteur.UtilisateurId,
                    DomaineId = 1,
                    Niveau = NiveauScolaire.Collegial,
                    NombreModules = 10,
                    DureeEstimeeHeures = 50,
                    EstPublie = true,
                    DateCreation = DateTime.Now.AddMonths(-2)
                },
                new Cours
                {
                    Nom = "Mecanique Classique",
                    Code = "PHY-301",
                    Description = "Explorez les principes de la mecanique newtonienne avec des demonstrations interactives.",
                    TuteurId = tuteur.UtilisateurId,
                    DomaineId = 2,
                    Niveau = NiveauScolaire.Collegial,
                    NombreModules = 9,
                    DureeEstimeeHeures = 45,
                    EstPublie = true,
                    DateCreation = DateTime.Now.AddMonths(-1)
                },
                new Cours
                {
                    Nom = "Chimie Organique Fondamentale",
                    Code = "CHM-401",
                    Description = "Decouvrez les reactions organiques, les mecanismes et la synthese organique.",
                    TuteurId = tuteur.UtilisateurId,
                    DomaineId = 3,
                    Niveau = NiveauScolaire.Collegial,
                    NombreModules = 12,
                    DureeEstimeeHeures = 60,
                    EstPublie = true,
                    DateCreation = DateTime.Now.AddMonths(-2)
                },
                new Cours
                {
                    Nom = "Programmation en C# Avancee",
                    Code = "INF-501",
                    Description = "Maitrisez la programmation orientee objet, les patterns et les bonnes pratiques en C#.",
                    TuteurId = tuteur.UtilisateurId,
                    DomaineId = 4,
                    Niveau = NiveauScolaire.Collegial,
                    NombreModules = 15,
                    DureeEstimeeHeures = 75,
                    EstPublie = true,
                    DateCreation = DateTime.Now.AddMonths(-3)
                },
                new Cours
                {
                    Nom = "Circuits electroniques Numeriques",
                    Code = "ELE-601",
                    Description = "Apprenez la conception de circuits numeriques, les portes logiques et les microcontroleurs.",
                    TuteurId = tuteur.UtilisateurId,
                    DomaineId = 5,
                    Niveau = NiveauScolaire.Collegial,
                    NombreModules = 11,
                    DureeEstimeeHeures = 55,
                    EstPublie = true,
                    DateCreation = DateTime.Now.AddMonths(-1)
                },
                new Cours
                {
                    Nom = "Biologie Cellulaire et Moleculaire",
                    Code = "BIO-701",
                    Description = "Explorez la structure cellulaire, l'ADN et les processus biologiques fondamentaux.",
                    TuteurId = tuteur.UtilisateurId,
                    DomaineId = 6,
                    Niveau = NiveauScolaire.Collegial,
                    NombreModules = 10,
                    DureeEstimeeHeures = 50,
                    EstPublie = true,
                    DateCreation = DateTime.Now.AddMonths(-2)
                }
            };

            foreach (var c in cours)
            {
                if (!context.Cours.Any(x => x.Code == c.Code))
                {
                    context.Cours.Add(c);
                }
            }
        }

        private void SeedModulesEtRessources(ThotDbContext context)
        {
            var cours = context.Cours.FirstOrDefault();
            if (cours == null) return;

            // Creer des modules pour le premier cours
            var modules = new List<Module>
            {
                new Module
                {
                    CoursId = cours.CoursId,
                    Titre = "Introduction aux Matrices",
                    Description = "Concepts fondamentaux des matrices et operations",
                    Ordre = 1,
                    EstPublie = true,
                    DateCreation = DateTime.Now
                },
                new Module
                {
                    CoursId = cours.CoursId,
                    Titre = "Determinants et Inverses",
                    Description = "Calcul des determinants et matrices inverses",
                    Ordre = 2,
                    EstPublie = true,
                    DateCreation = DateTime.Now
                },
                new Module
                {
                    CoursId = cours.CoursId,
                    Titre = "Systemes d'equations Lineaires",
                    Description = "Resolution de systemes lineaires avec differentes methodes",
                    Ordre = 3,
                    EstPublie = true,
                    DateCreation = DateTime.Now
                },
                new Module
                {
                    CoursId = cours.CoursId,
                    Titre = "Valeurs et Vecteurs Propres",
                    Description = "Diagonalisation et applications des valeurs propres",
                    Ordre = 4,
                    EstPublie = true,
                    DateCreation = DateTime.Now
                }
            };

            foreach (var m in modules)
            {
                if (!context.Modules.Any(x => x.CoursId == m.CoursId && x.Titre == m.Titre))
                {
                    context.Modules.Add(m);
                }
            }

            context.SaveChanges();

            // Ajouter des ressources aux modules avec la video demo.mp4
            var ressources = new List<RessourceModule>
            {
                new RessourceModule
                {
                    ModuleId = context.Modules.FirstOrDefault(m => m.Titre == "Introduction aux Matrices")?.ModuleId ?? 0,
                    Titre = "Cours - Introduction aux Matrices",
                    Type = TypeRessource.Video,
                    CheminFichier = "/Content/videos/demo.mp4",
                    DateAjout = DateTime.Now
                },
                new RessourceModule
                {
                    ModuleId = context.Modules.FirstOrDefault(m => m.Titre == "Introduction aux Matrices")?.ModuleId ?? 0,
                    Titre = "Exercices Pratiques",
                    Type = TypeRessource.Exercices,
                    CheminFichier = "/Content/videos/demo.mp4",
                    DateAjout = DateTime.Now
                },
                new RessourceModule
                {
                    ModuleId = context.Modules.FirstOrDefault(m => m.Titre == "Determinants et Inverses")?.ModuleId ?? 0,
                    Titre = "Demonstration - Calcul des Determinants",
                    Type = TypeRessource.Video,
                    CheminFichier = "/Content/videos/demo.mp4",
                    DateAjout = DateTime.Now
                },
                new RessourceModule
                {
                    ModuleId = context.Modules.FirstOrDefault(m => m.Titre == "Determinants et Inverses")?.ModuleId ?? 0,
                    Titre = "Feuille de Formules",
                    Type = TypeRessource.DocumentPDF,
                    CheminFichier = "/Content/videos/demo.mp4",
                    DateAjout = DateTime.Now
                },
                new RessourceModule
                {
                    ModuleId = context.Modules.FirstOrDefault(m => m.Titre == "Systemes d'equations Lineaires")?.ModuleId ?? 0,
                    Titre = "Cours - Systemes d'equations",
                    Type = TypeRessource.Video,
                    CheminFichier = "/Content/videos/demo.mp4",
                    DateAjout = DateTime.Now
                },
                new RessourceModule
                {
                    ModuleId = context.Modules.FirstOrDefault(m => m.Titre == "Systemes d'equations Lineaires")?.ModuleId ?? 0,
                    Titre = "Exercices - Systemes",
                    Type = TypeRessource.Exercices,
                    CheminFichier = "/Content/videos/demo.mp4",
                    DateAjout = DateTime.Now
                },
                new RessourceModule
                {
                    ModuleId = context.Modules.FirstOrDefault(m => m.Titre == "Valeurs et Vecteurs Propres")?.ModuleId ?? 0,
                    Titre = "Demonstration - Valeurs Propres",
                    Type = TypeRessource.Video,
                    CheminFichier = "/Content/videos/demo.mp4",
                    DateAjout = DateTime.Now
                },
                new RessourceModule
                {
                    ModuleId = context.Modules.FirstOrDefault(m => m.Titre == "Valeurs et Vecteurs Propres")?.ModuleId ?? 0,
                    Titre = "Feuille de Synthese",
                    Type = TypeRessource.DocumentPDF,
                    CheminFichier = "/Content/videos/demo.mp4",
                    DateAjout = DateTime.Now
                }
            };

            foreach (var r in ressources)
            {
                if (r.ModuleId > 0 && !context.RessourcesModules.Any(x => x.ModuleId == r.ModuleId && x.Titre == r.Titre))
                {
                    context.RessourcesModules.Add(r);
                }
            }
        }

        private void SeedQuiz(ThotDbContext context)
        {
            var modules = context.Modules.Take(4).ToList();
            if (modules.Count == 0) return;

            var quizzes = new List<Quiz>
            {
                new Quiz
                {
                    ModuleId = modules[0].ModuleId,
                    Titre = "Quiz - Introduction aux Matrices",
                    Description = "Testez vos connaissances sur les concepts fondamentaux des matrices",
                    EstPublie = true,
                    DateCreation = DateTime.Now
                },
                new Quiz
                {
                    ModuleId = modules[1].ModuleId,
                    Titre = "Quiz - Determinants",
                    Description = "evaluez votre comprehension des determinants et inverses",
                    EstPublie = true,
                    DateCreation = DateTime.Now
                },
                new Quiz
                {
                    ModuleId = modules[2].ModuleId,
                    Titre = "Quiz - Systemes Lineaires",
                    Description = "Verifiez votre maitrise de la resolution de systemes",
                    EstPublie = true,
                    DateCreation = DateTime.Now
                },
                new Quiz
                {
                    ModuleId = modules[3].ModuleId,
                    Titre = "Quiz - Valeurs Propres",
                    Description = "Testez vos connaissances sur les valeurs et vecteurs propres",
                    EstPublie = true,
                    DateCreation = DateTime.Now
                }
            };

            foreach (var quiz in quizzes)
            {
                if (!context.Quizs.Any(q => q.ModuleId == quiz.ModuleId && q.Titre == quiz.Titre))
                {
                    context.Quizs.Add(quiz);
                }
            }
            context.SaveChanges();
        }

        private void SeedQuestionsEtReponses(ThotDbContext context)
        {
            var etudiant = context.Etudiants.FirstOrDefault();
            var tuteurs = context.Tuteurs.Take(3).ToList();
            
            if (etudiant == null || tuteurs.Count == 0) return;

            var questions = new List<Question>
            {
                new Question
                {
                    EtudiantId = etudiant.UtilisateurId,
                    DomaineId = 1,
                    Titre = "Comment calculer le determinant d'une matrice 3x3?",
                    Contenu = "Je ne comprends pas bien la methode de Sarrus pour calculer les determinants.",
                    Statut = StatutQuestion.Repondue,
                    DateCreation = DateTime.Now.AddDays(-5),
                    DateLimiteReponse = DateTime.Now.AddDays(-3)
                },
                new Question
                {
                    EtudiantId = etudiant.UtilisateurId,
                    DomaineId = 1,
                    Titre = "Quelle est la difference entre une matrice singuliere et non-singuliere?",
                    Contenu = "Je dois comprendre cette distinction pour mon examen.",
                    Statut = StatutQuestion.Repondue,
                    DateCreation = DateTime.Now.AddDays(-3),
                    DateLimiteReponse = DateTime.Now.AddDays(-1)
                },
                new Question
                {
                    EtudiantId = etudiant.UtilisateurId,
                    DomaineId = 2,
                    Titre = "Comment appliquer les lois de Newton en mecanique?",
                    Contenu = "J'ai besoin d'aide pour resoudre des problemes de dynamique.",
                    Statut = StatutQuestion.Repondue,
                    DateCreation = DateTime.Now.AddDays(-2),
                    DateLimiteReponse = DateTime.Now
                }
            };

            foreach (var question in questions)
            {
                if (!context.Questions.Any(q => q.Titre == question.Titre))
                {
                    context.Questions.Add(question);
                }
            }

            context.SaveChanges();

            // Ajouter des reponses
            var questionsAdded = context.Questions.Where(q => q.EtudiantId == etudiant.UtilisateurId).ToList();
            var reponses = new List<Reponse>();

            if (questionsAdded.Count > 0)
            {
                reponses.Add(new Reponse
                {
                    QuestionId = questionsAdded[0].QuestionId,
                    TuteurId = tuteurs[0].UtilisateurId,
                    Contenu = "La methode de Sarrus est une technique simple pour calculer les determinants 3x3. Vous ecrivez la matrice, puis vous repetez les deux premieres colonnes...",
                    EstValidee = true,
                    DateCreation = DateTime.Now.AddDays(-4)
                });

                if (questionsAdded.Count > 1)
                {
                    reponses.Add(new Reponse
                    {
                        QuestionId = questionsAdded[1].QuestionId,
                        TuteurId = tuteurs[1].UtilisateurId,
                        Contenu = "Une matrice singuliere a un determinant egal a zero et n'a pas d'inverse. Une matrice non-singuliere a un determinant non-nul et possede une inverse.",
                        EstValidee = true,
                        DateCreation = DateTime.Now.AddDays(-2)
                    });
                }

                if (questionsAdded.Count > 2)
                {
                    reponses.Add(new Reponse
                    {
                        QuestionId = questionsAdded[2].QuestionId,
                        TuteurId = tuteurs[2].UtilisateurId,
                        Contenu = "Les trois lois de Newton sont: 1) Inertie, 2) F=ma, 3) Action-reaction. Pour resoudre des problemes, identifiez les forces, appliquez F=ma a chaque objet.",
                        EstValidee = true,
                        DateCreation = DateTime.Now.AddDays(-1)
                    });
                }
            }

            foreach (var reponse in reponses)
            {
                if (!context.Reponses.Any(r => r.QuestionId == reponse.QuestionId && r.TuteurId == reponse.TuteurId))
                {
                    context.Reponses.Add(reponse);
                }
            }
        }

        private void SeedSessionsClavardage(ThotDbContext context)
        {
            var etudiant = context.Etudiants.FirstOrDefault();
            var tuteurs = context.Tuteurs.Take(2).ToList();
            
            if (etudiant == null || tuteurs.Count < 2) return;

            var sessions = new List<SessionClavardage>
            {
                new SessionClavardage
                {
                    EtudiantId = etudiant.UtilisateurId,
                    TuteurId = tuteurs[0].UtilisateurId,
                    DomaineId = 1,
                    Type = TypeSession.Normale,
                    Statut = StatutSession.Terminee,
                    DateDebut = DateTime.Now.AddDays(-7),
                    DateFin = DateTime.Now.AddDays(-7).AddHours(1),
                    NoteEtudiant = 5,
                    Commentaire = "Excellente session! Le tuteur a bien explique les concepts."
                },
                new SessionClavardage
                {
                    EtudiantId = etudiant.UtilisateurId,
                    TuteurId = tuteurs[1].UtilisateurId,
                    DomaineId = 2,
                    Type = TypeSession.Immediate,
                    Statut = StatutSession.Terminee,
                    DateDebut = DateTime.Now.AddDays(-3),
                    DateFin = DateTime.Now.AddDays(-3).AddHours(1),
                    NoteEtudiant = 4,
                    Commentaire = "Tres utile pour comprendre la mecanique."
                },
                new SessionClavardage
                {
                    EtudiantId = etudiant.UtilisateurId,
                    TuteurId = tuteurs[0].UtilisateurId,
                    DomaineId = 1,
                    Type = TypeSession.Normale,
                    Statut = StatutSession.EnCours,
                    DateDebut = DateTime.Now.AddHours(-1),
                    Cout = 0
                }
            };

            foreach (var session in sessions)
            {
                if (!context.SessionsClavardage.Any(s => s.EtudiantId == session.EtudiantId && s.DateDebut == session.DateDebut))
                {
                    context.SessionsClavardage.Add(session);
                }
            }
        }

        private void SeedRencontresPhysiques(ThotDbContext context)
        {
            var etudiant = context.Etudiants.FirstOrDefault();
            var tuteurs = context.Tuteurs.Take(2).ToList();
            
            if (etudiant == null || tuteurs.Count < 2) return;

            var rencontres = new List<RencontrePhysique>
            {
                new RencontrePhysique
                {
                    EtudiantId = etudiant.UtilisateurId,
                    TuteurId = tuteurs[0].UtilisateurId,
                    DomaineId = 1,
                    DateHeure = DateTime.Now.AddDays(-14),
                    DureeHeures = 1,
                    Lieu = "Bibliotheque Centrale, Montreal",
                    Description = "Rencontre pour discuter des matrices et determinants",
                    Tarif = 35.00m,
                    TarifPreferentiel = false,
                    Statut = StatutRencontre.Terminee,
                    NoteEtudiant = 5,
                    Commentaire = "Excellente rencontre! Le tuteur a bien explique les concepts difficiles."
                },
                new RencontrePhysique
                {
                    EtudiantId = etudiant.UtilisateurId,
                    TuteurId = tuteurs[1].UtilisateurId,
                    DomaineId = 2,
                    DateHeure = DateTime.Now.AddDays(-7),
                    DureeHeures = 1.5m,
                    Lieu = "Cafe etudiant, Montreal",
                    Description = "Session de revision en mecanique classique",
                    Tarif = 40.00m,
                    TarifPreferentiel = false,
                    Statut = StatutRencontre.Terminee,
                    NoteEtudiant = 4,
                    Commentaire = "Tres utile pour preparer mon examen."
                },
                new RencontrePhysique
                {
                    EtudiantId = etudiant.UtilisateurId,
                    TuteurId = tuteurs[0].UtilisateurId,
                    DomaineId = 1,
                    DateHeure = DateTime.Now.AddDays(3),
                    DureeHeures = 1,
                    Lieu = "Parc Lafontaine, Montreal",
                    Description = "Rencontre pour discuter des systemes d'equations",
                    Tarif = 35.00m,
                    TarifPreferentiel = false,
                    Statut = StatutRencontre.Confirmee
                }
            };

            foreach (var rencontre in rencontres)
            {
                if (!context.RencontresPhysiques.Any(r => r.EtudiantId == rencontre.EtudiantId && r.DateHeure == rencontre.DateHeure))
                {
                    context.RencontresPhysiques.Add(rencontre);
                }
            }
        }

        private void SeedInscriptionsCours(ThotDbContext context)
        {
            var etudiant = context.Etudiants.FirstOrDefault();
            var cours = context.Cours.Take(3).ToList();
            
            if (etudiant == null || cours.Count == 0) return;

            var inscriptions = new List<InscriptionCours>();

            foreach (var c in cours)
            {
                inscriptions.Add(new InscriptionCours
                {
                    EtudiantId = etudiant.UtilisateurId,
                    CoursId = c.CoursId,
                    DateInscription = DateTime.Now.AddDays(-30)
                });
            }

            foreach (var inscription in inscriptions)
            {
                if (!context.InscriptionsCours.Any(i => i.EtudiantId == inscription.EtudiantId && i.CoursId == inscription.CoursId))
                {
                    context.InscriptionsCours.Add(inscription);
                }
            }

            context.SaveChanges();

            // Ajouter les progressions de modules
            var inscriptionsAdded = context.InscriptionsCours.Where(i => i.EtudiantId == etudiant.UtilisateurId).ToList();
            var progressions = new List<ProgressionModule>();

            foreach (var inscription in inscriptionsAdded)
            {
                var modules = context.Modules.Where(m => m.CoursId == inscription.CoursId).Take(2).ToList();
                foreach (var module in modules)
                {
                    progressions.Add(new ProgressionModule
                    {
                        InscriptionId = inscription.InscriptionId,
                        ModuleId = module.ModuleId,
                        EstComplete = false,
                        DateDebut = DateTime.Now.AddDays(-20),
                        TempsPasseMinutes = 450
                    });
                }
            }

            foreach (var progression in progressions)
            {
                if (!context.ProgressionsModules.Any(p => p.InscriptionId == progression.InscriptionId && p.ModuleId == progression.ModuleId))
                {
                    context.ProgressionsModules.Add(progression);
                }
            }
        }

        private void SeedTransactions(ThotDbContext context)
        {
            var etudiant = context.Etudiants.FirstOrDefault();
            
            if (etudiant == null) return;

            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    EtudiantId = etudiant.UtilisateurId,
                    Type = TypeTransaction.AbonnementMensuel,
                    Montant = 19.99m,
                    Statut = StatutTransaction.Completee,
                    PayPalTransactionId = Guid.NewGuid().ToString(),
                    PayPalPaymentId = "PAY-" + Guid.NewGuid().ToString().Substring(0, 8),
                    PayPalStatut = "COMPLETED",
                    Description = "Abonnement mensuel Plateforme Thot",
                    DateTransaction = DateTime.Now.AddMonths(-1)
                },
                new Transaction
                {
                    EtudiantId = etudiant.UtilisateurId,
                    Type = TypeTransaction.QuestionPonctuelle,
                    Montant = 2.00m,
                    Statut = StatutTransaction.Completee,
                    PayPalTransactionId = Guid.NewGuid().ToString(),
                    PayPalPaymentId = "PAY-" + Guid.NewGuid().ToString().Substring(0, 8),
                    PayPalStatut = "COMPLETED",
                    Description = "Question ponctuelle",
                    DateTransaction = DateTime.Now.AddDays(-15)
                },
                new Transaction
                {
                    EtudiantId = etudiant.UtilisateurId,
                    Type = TypeTransaction.SessionImmediate,
                    Montant = 5.00m,
                    Statut = StatutTransaction.Completee,
                    PayPalTransactionId = Guid.NewGuid().ToString(),
                    PayPalPaymentId = "PAY-" + Guid.NewGuid().ToString().Substring(0, 8),
                    PayPalStatut = "COMPLETED",
                    Description = "Session immediate de clavardage",
                    DateTransaction = DateTime.Now.AddDays(-3)
                }
            };

            foreach (var transaction in transactions)
            {
                if (!context.Transactions.Any(t => t.PayPalPaymentId == transaction.PayPalPaymentId))
                {
                    context.Transactions.Add(transaction);
                }
            }
        }

        private void SeedMessages(ThotDbContext context)
        {
            var etudiant = context.Etudiants.FirstOrDefault();
            var tuteurs = context.Tuteurs.Take(2).ToList();
            
            if (etudiant == null || tuteurs.Count < 2) return;

            var messages = new List<Message>
            {
                new Message
                {
                    ExpediteurId = tuteurs[0].UtilisateurId,
                    DestinatireId = etudiant.UtilisateurId,
                    Sujet = "Bienvenue sur la plateforme Thot",
                    Contenu = "Bonjour Jean,\n\nBienvenue sur la plateforme Thot! Je suis Marie Durand, votre tuteur en mathematiques. N'hesitez pas a me contacter si vous avez des questions.",
                    DateEnvoi = DateTime.Now.AddDays(-10),
                    EstLu = true
                },
                new Message
                {
                    ExpediteurId = etudiant.UtilisateurId,
                    DestinatireId = tuteurs[0].UtilisateurId,
                    Sujet = "Question sur les matrices",
                    Contenu = "Bonjour Marie,\n\nJ'ai une question sur le calcul des determinants. Pouvez-vous m'aider?",
                    DateEnvoi = DateTime.Now.AddDays(-9),
                    EstLu = true
                },
                new Message
                {
                    ExpediteurId = tuteurs[0].UtilisateurId,
                    DestinatireId = etudiant.UtilisateurId,
                    Sujet = "Reponse: Question sur les matrices",
                    Contenu = "Bien sur Jean! Je serais ravi de vous aider. Nous pouvons planifier une session de clavardage ou une rencontre en personne.",
                    DateEnvoi = DateTime.Now.AddDays(-8),
                    EstLu = true
                },
                new Message
                {
                    ExpediteurId = tuteurs[1].UtilisateurId,
                    DestinatireId = etudiant.UtilisateurId,
                    Sujet = "Suggestion de cours",
                    Contenu = "Bonjour Jean,\n\nJe vous recommande de suivre le cours de mecanique classique. C'est un excellent complement aux mathematiques.",
                    DateEnvoi = DateTime.Now.AddDays(-5),
                    EstLu = false
                }
            };

            foreach (var message in messages)
            {
                if (!context.Messages.Any(m => m.Sujet == message.Sujet && m.DateEnvoi == message.DateEnvoi))
                {
                    context.Messages.Add(message);
                }
            }
        }
    }
}

