using System;
using System.Collections.Generic;
using System.Linq;
using ThotPlatform.Models;
using ThotPlatform.Utils;

namespace ThotPlatform.Migrations
{
    /// <summary>
    /// Classe pour initialiser la base de donnees avec des donnees de test
    /// </summary>
    public static class SeedData
    {
        public static void Initialize(ThotDbContext context)
        {
            // Verifier si les nouveaux tuteurs existent deja
            if (context.Tuteurs.Any(t => t.Email == "fernand.togne@thot.com"))
            {
                return; // Les nouvelles donnees sont deja presentes
            }

            // 1. CREER LES DOMAINES
            var domaines = new List<Domaine>
            {
                new Domaine { Nom = "Mathematiques", Description = "Algebre, Geometrie, Calcul", EstActif = true },
                new Domaine { Nom = "Physique", Description = "Mecanique, Electricite, Optique", EstActif = true },
                new Domaine { Nom = "Chimie", Description = "Chimie organique et inorganique", EstActif = true },
                new Domaine { Nom = "Informatique", Description = "Programmation, Algorithmes, Bases de donnees", EstActif = true },
                new Domaine { Nom = "Electronique", Description = "Circuits, Systemes embarques", EstActif = true },
                new Domaine { Nom = "Biologie", Description = "Biologie cellulaire, Genetique", EstActif = true }
            };
            context.Domaines.AddRange(domaines);
            context.SaveChanges();

            // 2. CREER LES TUTEURS
            var tuteurs = new List<Tuteur>
            {
                new Tuteur
                {
                    Nom = "Togne",
                    Prenom = "Fernand",
                    Email = "fernand.togne@thot.com",
                    Username = "fernand.togne",
                    MotDePasse = PasswordHelper.HashPassword("Tuteur123!"),
                    Telephone = "514-555-0101",
                    Adresse = "123 Rue Universite, Montreal",
                    Ville = "Montreal",
                    EstActif = true,
                    PremierChangementMotDePasse = true,
                    EstDisponible = true,
                    NoteMoyenne = 4.8m,
                    AnneesExperience = 10,
                    DisponiblePhysique = true,
                    TarifHorairePhysique = 35m,
                    Biographie = "Docteur en mathematiques avec 10 ans d'experience en enseignement. Passionne par la transmission du savoir.",
                    Diplomes = "Doctorat en Mathematiques"
                },
                new Tuteur
                {
                    Nom = "Benfriya",
                    Prenom = "Hichem",
                    Email = "hichem.benfriya@thot.com",
                    Username = "hichem.benfriya",
                    MotDePasse = PasswordHelper.HashPassword("Tuteur123!"),
                    Telephone = "514-555-0102",
                    Adresse = "456 Avenue Sciences, Montreal",
                    Ville = "Montreal",
                    EstActif = true,
                    PremierChangementMotDePasse = true,
                    EstDisponible = true,
                    NoteMoyenne = 4.9m,
                    AnneesExperience = 8,
                    DisponiblePhysique = true,
                    TarifHorairePhysique = 40m,
                    Biographie = "Ingenieur logiciel senior, expert en programmation et systemes embarques.",
                    Diplomes = "Maitrise en Informatique"
                },
                new Tuteur
                {
                    Nom = "Alexandre",
                    Prenom = "Styve",
                    Email = "styve.alexandre@thot.com",
                    Username = "styve.alexandre",
                    MotDePasse = PasswordHelper.HashPassword("Tuteur123!"),
                    Telephone = "514-555-0103",
                    Adresse = "789 Boulevard Tech, Montreal",
                    Ville = "Montreal",
                    EstActif = true,
                    PremierChangementMotDePasse = true,
                    EstDisponible = true,
                    NoteMoyenne = 4.7m,
                    AnneesExperience = 6,
                    DisponiblePhysique = true,
                    TarifHorairePhysique = 38m,
                    Biographie = "Chercheur en biochimie, passionne par les sciences de la vie.",
                    Diplomes = "Doctorat en Biochimie"
                },
                new Tuteur
                {
                    Nom = "Elouga",
                    Prenom = "Raoul",
                    Email = "raoul.elouga@thot.com",
                    Username = "raoul.elouga",
                    MotDePasse = PasswordHelper.HashPassword("Tuteur123!"),
                    Telephone = "514-555-0104",
                    Adresse = "321 Rue Education, Montreal",
                    Ville = "Montreal",
                    EstActif = true,
                    PremierChangementMotDePasse = true,
                    EstDisponible = true,
                    NoteMoyenne = 4.6m,
                    AnneesExperience = 12,
                    DisponiblePhysique = true,
                    TarifHorairePhysique = 42m,
                    Biographie = "Physicien experimente, specialiste en electromagnetisme et circuits electroniques.",
                    Diplomes = "Doctorat en Physique"
                }
            };
            // Ajouter les tuteurs seulement s'ils n'existent pas
            foreach (var tuteur in tuteurs)
            {
                if (!context.Tuteurs.Any(t => t.Email == tuteur.Email))
                {
                    context.Tuteurs.Add(tuteur);
                }
            }
            context.SaveChanges();

            // 3. CREER LES ETUDIANTS
            var etudiants = new List<Etudiant>
            {
                new Etudiant
                {
                    Nom = "Ali",
                    Prenom = "Ahmed",
                    Email = "ahmed.ali@student.com",
                    Username = "ahmed.ali",
                    MotDePasse = PasswordHelper.HashPassword("Etudiant123!"),
                    Telephone = "514-555-0201",
                    Adresse = "100 Rue Etudiante, Montreal",
                    Ville = "Montreal",
                    EstActif = true,
                    PremierChangementMotDePasse = true,
                    Niveau = NiveauScolaire.Secondaire,
                    Etablissement = "Ecole Secondaire de Montreal",
                    EstAbonne = true,
                    DateDebutAbonnement = DateTime.Now.AddDays(-15),
                    DateFinAbonnement = DateTime.Now.AddDays(15)
                },
                new Etudiant
                {
                    Nom = "Alice",
                    Prenom = "Sandra",
                    Email = "sandra.alice@student.com",
                    Username = "sandra.alice",
                    MotDePasse = PasswordHelper.HashPassword("Etudiant123!"),
                    Telephone = "514-555-0202",
                    Adresse = "200 Avenue Jeunesse, Montreal",
                    Ville = "Montreal",
                    EstActif = true,
                    PremierChangementMotDePasse = true,
                    Niveau = NiveauScolaire.Collegial,
                    Etablissement = "Cegep de Montreal",
                    EstAbonne = true,
                    DateDebutAbonnement = DateTime.Now.AddDays(-10),
                    DateFinAbonnement = DateTime.Now.AddDays(20)
                },
                new Etudiant
                {
                    Nom = "Dubois",
                    Prenom = "Marie",
                    Email = "marie.dubois@student.com",
                    Username = "marie.dubois",
                    MotDePasse = PasswordHelper.HashPassword("Etudiant123!"),
                    Telephone = "514-555-0203",
                    Adresse = "300 Rue Apprentissage, Montreal",
                    Ville = "Montreal",
                    EstActif = true,
                    PremierChangementMotDePasse = true,
                    Niveau = NiveauScolaire.Primaire,
                    Etablissement = "Ecole Primaire Saint-Laurent",
                    EstAbonne = false
                },
                new Etudiant
                {
                    Nom = "Tremblay",
                    Prenom = "Jean",
                    Email = "jean.tremblay@student.com",
                    Username = "jean.tremblay",
                    MotDePasse = PasswordHelper.HashPassword("Etudiant123!"),
                    Telephone = "514-555-0204",
                    Adresse = "400 Boulevard Savoir, Montreal",
                    Ville = "Montreal",
                    EstActif = true,
                    PremierChangementMotDePasse = true,
                    Niveau = NiveauScolaire.Secondaire,
                    Etablissement = "Ecole Secondaire Jean-Eudes",
                    EstAbonne = true,
                    DateDebutAbonnement = DateTime.Now.AddDays(-20),
                    DateFinAbonnement = DateTime.Now.AddDays(10)
                },
                new Etudiant
                {
                    Nom = "Lavoie",
                    Prenom = "Sophie",
                    Email = "sophie.lavoie@student.com",
                    Username = "sophie.lavoie",
                    MotDePasse = PasswordHelper.HashPassword("Etudiant123!"),
                    Telephone = "514-555-0205",
                    Adresse = "500 Rue Connaissance, Montreal",
                    Ville = "Montreal",
                    EstActif = true,
                    PremierChangementMotDePasse = true,
                    Niveau = NiveauScolaire.Collegial,
                    Etablissement = "Cegep du Vieux-Montreal",
                    EstAbonne = false
                }
            };
            // Ajouter les etudiants seulement s'ils n'existent pas
            foreach (var etudiant in etudiants)
            {
                if (!context.Etudiants.Any(e => e.Email == etudiant.Email))
                {
                    context.Etudiants.Add(etudiant);
                }
            }
            context.SaveChanges();

            // 4. ASSOCIER TUTEURS AUX DOMAINES
            var tuteurDomaines = new List<TuteurDomaine>
            {
                // Fernand Togne - Mathematiques et Physique
                new TuteurDomaine { TuteurId = tuteurs[0].UtilisateurId, DomaineId = domaines[0].DomaineId },
                new TuteurDomaine { TuteurId = tuteurs[0].UtilisateurId, DomaineId = domaines[1].DomaineId },
                
                // Hichem Benfriya - Informatique et Electronique
                new TuteurDomaine { TuteurId = tuteurs[1].UtilisateurId, DomaineId = domaines[3].DomaineId },
                new TuteurDomaine { TuteurId = tuteurs[1].UtilisateurId, DomaineId = domaines[4].DomaineId },
                
                // Styve Alexandre - Chimie et Biologie
                new TuteurDomaine { TuteurId = tuteurs[2].UtilisateurId, DomaineId = domaines[2].DomaineId },
                new TuteurDomaine { TuteurId = tuteurs[2].UtilisateurId, DomaineId = domaines[5].DomaineId },
                
                // Raoul Elouga - Physique et Electronique
                new TuteurDomaine { TuteurId = tuteurs[3].UtilisateurId, DomaineId = domaines[1].DomaineId },
                new TuteurDomaine { TuteurId = tuteurs[3].UtilisateurId, DomaineId = domaines[4].DomaineId }
            };
            context.TuteurDomaines.AddRange(tuteurDomaines);
            context.SaveChanges();

            // 5. CREER DES COURS
            var cours = new List<Cours>
            {
                new Cours
                {
                    Nom = "Algebre Lineaire - Niveau Avance",
                    Code = "MATH-301",
                    Description = "Etude approfondie des espaces vectoriels, matrices, determinants et applications lineaires. Ce cours couvre les concepts fondamentaux de l'algebre lineaire avec de nombreux exemples pratiques.",
                    TuteurId = tuteurs[0].UtilisateurId,
                    DomaineId = domaines[0].DomaineId,
                    Niveau = NiveauScolaire.Collegial,
                    NombreModules = 8,
                    DureeEstimeeHeures = 40,
                    EstPublie = true,
                    NombreInscrits = 15,
                    NoteMoyenne = 4.7m
                },
                new Cours
                {
                    Nom = "Programmation Python pour Debutants",
                    Code = "INFO-101",
                    Description = "Introduction a la programmation avec Python. Apprenez les bases : variables, boucles, fonctions, et creez vos premiers programmes. Aucune experience prealable requise.",
                    TuteurId = tuteurs[1].UtilisateurId,
                    DomaineId = domaines[3].DomaineId,
                    Niveau = NiveauScolaire.Secondaire,
                    NombreModules = 12,
                    DureeEstimeeHeures = 30,
                    EstPublie = true,
                    NombreInscrits = 28,
                    NoteMoyenne = 4.9m
                },
                new Cours
                {
                    Nom = "Chimie Organique - Les Bases",
                    Code = "CHIM-201",
                    Description = "Decouvrez le monde fascinant de la chimie organique : structure des molecules, nomenclature, reactions chimiques et mecanismes reactionnels.",
                    TuteurId = tuteurs[2].UtilisateurId,
                    DomaineId = domaines[2].DomaineId,
                    Niveau = NiveauScolaire.Collegial,
                    NombreModules = 10,
                    DureeEstimeeHeures = 35,
                    EstPublie = true,
                    NombreInscrits = 12,
                    NoteMoyenne = 4.6m
                },
                new Cours
                {
                    Nom = "Electronique Numerique",
                    Code = "ELEC-202",
                    Description = "Circuits logiques, portes logiques, bascules, compteurs et systemes numeriques. Theorie et pratique avec simulations.",
                    TuteurId = tuteurs[3].UtilisateurId,
                    DomaineId = domaines[4].DomaineId,
                    Niveau = NiveauScolaire.Collegial,
                    NombreModules = 9,
                    DureeEstimeeHeures = 38,
                    EstPublie = true,
                    NombreInscrits = 18,
                    NoteMoyenne = 4.8m
                },
                new Cours
                {
                    Nom = "Physique - Mecanique Classique",
                    Code = "PHYS-101",
                    Description = "Les lois de Newton, cinematique, dynamique, travail et energie. Comprendre les principes fondamentaux de la mecanique avec des exemples concrets.",
                    TuteurId = tuteurs[0].UtilisateurId,
                    DomaineId = domaines[1].DomaineId,
                    Niveau = NiveauScolaire.Secondaire,
                    NombreModules = 7,
                    DureeEstimeeHeures = 25,
                    EstPublie = true,
                    NombreInscrits = 22,
                    NoteMoyenne = 4.5m
                }
            };
            context.Cours.AddRange(cours);
            context.SaveChanges();

            // 6. CREER DES MODULES POUR CHAQUE COURS
            var modules = new List<Module>();

            // Modules pour Algebre Lineaire
            modules.AddRange(new[]
            {
                new Module
                {
                    CoursId = cours[0].CoursId,
                    Titre = "Introduction aux Vecteurs",
                    Description = "Definition des vecteurs, operations de base, produit scalaire et vectoriel",
                    Ordre = 1,
                    DureeMinutes = 45,
                    ContenuTexte = "<h3>Les Vecteurs</h3><p>Un vecteur est une quantite qui possede une magnitude et une direction...</p>",
                    EstPublie = true
                },
                new Module
                {
                    CoursId = cours[0].CoursId,
                    Titre = "Matrices et Determinants",
                    Description = "Operations sur les matrices, calcul de determinants, matrices inversibles",
                    Ordre = 2,
                    DureeMinutes = 60,
                    ContenuTexte = "<h3>Les Matrices</h3><p>Une matrice est un tableau rectangulaire de nombres...</p>",
                    EstPublie = true
                },
                new Module
                {
                    CoursId = cours[0].CoursId,
                    Titre = "Espaces Vectoriels",
                    Description = "Definition d'un espace vectoriel, sous-espaces, base et dimension",
                    Ordre = 3,
                    DureeMinutes = 55,
                    ContenuTexte = "<h3>Espaces Vectoriels</h3><p>Un espace vectoriel est un ensemble muni de deux operations...</p>",
                    EstPublie = true
                }
            });

            // Modules pour Python
            modules.AddRange(new[]
            {
                new Module
                {
                    CoursId = cours[1].CoursId,
                    Titre = "Installation et Premier Programme",
                    Description = "Installer Python, configurer l'environnement, ecrire 'Hello World'",
                    Ordre = 1,
                    DureeMinutes = 30,
                    ContenuTexte = "<h3>Bienvenue en Python</h3><p>Python est un langage de programmation puissant et facile a apprendre...</p>",
                    EstPublie = true
                },
                new Module
                {
                    CoursId = cours[1].CoursId,
                    Titre = "Variables et Types de Donnees",
                    Description = "Entiers, flottants, chaines de caracteres, booleens",
                    Ordre = 2,
                    DureeMinutes = 40,
                    ContenuTexte = "<h3>Les Variables</h3><p>Une variable est un conteneur pour stocker des donnees...</p>",
                    EstPublie = true
                },
                new Module
                {
                    CoursId = cours[1].CoursId,
                    Titre = "Structures de Controle",
                    Description = "Conditions if/else, boucles for et while",
                    Ordre = 3,
                    DureeMinutes = 50,
                    ContenuTexte = "<h3>Controler le Flux</h3><p>Les structures de controle permettent de prendre des decisions...</p>",
                    EstPublie = true
                }
            });

            context.Modules.AddRange(modules);
            context.SaveChanges();

            // 7. CREER DES QUESTIONS
            var questions = new List<Question>
            {
                new Question
                {
                    EtudiantId = etudiants[0].UtilisateurId,
                    DomaineId = domaines[0].DomaineId,
                    Titre = "Comment calculer un determinant 3x3 ?",
                    Contenu = "J'ai du mal a comprendre la methode de Sarrus pour calculer un determinant 3x3. Pourriez-vous m'expliquer avec un exemple ?",
                    Statut = StatutQuestion.Repondue,
                    EstPrioritaire = false
                },
                new Question
                {
                    EtudiantId = etudiants[1].UtilisateurId,
                    DomaineId = domaines[3].DomaineId,
                    Titre = "Difference entre liste et tuple en Python ?",
                    Contenu = "Quelle est la difference entre une liste et un tuple ? Quand utiliser l'un ou l'autre ?",
                    Statut = StatutQuestion.EnAttente,
                    EstPrioritaire = true
                },
                new Question
                {
                    EtudiantId = etudiants[2].UtilisateurId,
                    DomaineId = domaines[1].DomaineId,
                    Titre = "Calcul de la force de friction",
                    Contenu = "Comment calculer la force de friction sur un plan incline ? Quelles sont les formules a utiliser ?",
                    Statut = StatutQuestion.EnCours,
                    EstPrioritaire = false
                }
            };
            context.Questions.AddRange(questions);
            context.SaveChanges();

            // 8. CREER DES REPONSES
            var reponses = new List<Reponse>
            {
                new Reponse
                {
                    QuestionId = questions[0].QuestionId,
                    TuteurId = tuteurs[0].UtilisateurId,
                    Contenu = "La methode de Sarrus est tres simple ! Voici comment proceder :\n\n1. Ecrivez la matrice 3x3\n2. Recopiez les deux premieres colonnes a droite\n3. Calculez les produits des diagonales descendantes (positifs)\n4. Calculez les produits des diagonales montantes (negatifs)\n5. Faites la somme\n\nExemple avec la matrice :\n| 1  2  3 |\n| 4  5  6 |\n| 7  8  9 |\n\nDet = (1󬊉 + 2󬝳 + 3󫶜) - (3󬊇 + 1󬝴 + 2󫶝) = 0",
                    EstValidee = true,
                    Note = 5
                }
            };
            context.Reponses.AddRange(reponses);
            context.SaveChanges();

            // 9. CREER DES INSCRIPTIONS AUX COURS
            var inscriptions = new List<InscriptionCours>
            {
                new InscriptionCours { EtudiantId = etudiants[0].UtilisateurId, CoursId = cours[0].CoursId },
                new InscriptionCours { EtudiantId = etudiants[0].UtilisateurId, CoursId = cours[4].CoursId },
                new InscriptionCours { EtudiantId = etudiants[1].UtilisateurId, CoursId = cours[1].CoursId },
                new InscriptionCours { EtudiantId = etudiants[1].UtilisateurId, CoursId = cours[2].CoursId },
                new InscriptionCours { EtudiantId = etudiants[3].UtilisateurId, CoursId = cours[1].CoursId },
                new InscriptionCours { EtudiantId = etudiants[3].UtilisateurId, CoursId = cours[4].CoursId }
            };
            context.InscriptionsCours.AddRange(inscriptions);
            context.SaveChanges();

            // 10. CREER DES TRANSACTIONS (Abonnements)
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    EtudiantId = etudiants[0].UtilisateurId,
                    Type = TypeTransaction.AbonnementMensuel,
                    Montant = 17.99m,
                    Statut = StatutTransaction.Completee,
                    PayPalTransactionId = "TXN-" + Guid.NewGuid().ToString().Substring(0, 8),
                    PayPalPaymentId = "PAY-" + Guid.NewGuid().ToString().Substring(0, 8),
                    Description = "Abonnement mensuel - Ahmed Ali"
                },
                new Transaction
                {
                    EtudiantId = etudiants[1].UtilisateurId,
                    Type = TypeTransaction.AbonnementMensuel,
                    Montant = 17.99m,
                    Statut = StatutTransaction.Completee,
                    PayPalTransactionId = "TXN-" + Guid.NewGuid().ToString().Substring(0, 8),
                    PayPalPaymentId = "PAY-" + Guid.NewGuid().ToString().Substring(0, 8),
                    Description = "Abonnement mensuel - Sandra Alice"
                },
                new Transaction
                {
                    EtudiantId = etudiants[3].UtilisateurId,
                    Type = TypeTransaction.AbonnementMensuel,
                    Montant = 17.99m,
                    Statut = StatutTransaction.Completee,
                    PayPalTransactionId = "TXN-" + Guid.NewGuid().ToString().Substring(0, 8),
                    PayPalPaymentId = "PAY-" + Guid.NewGuid().ToString().Substring(0, 8),
                    Description = "Abonnement mensuel - Jean Tremblay"
                }
            };
            context.Transactions.AddRange(transactions);
            context.SaveChanges();

            Console.WriteLine("? Base de donnees initialisee avec succes !");
            Console.WriteLine($"   - {domaines.Count} domaines");
            Console.WriteLine($"   - {tuteurs.Count} tuteurs");
            Console.WriteLine($"   - {etudiants.Count} etudiants");
            Console.WriteLine($"   - {cours.Count} cours");
            Console.WriteLine($"   - {modules.Count} modules");
            Console.WriteLine($"   - {questions.Count} questions");
            Console.WriteLine($"   - {reponses.Count} reponses");
        }
    }
}

