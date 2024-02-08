﻿using EasySave.View;
using EasySave.Model;
using System;
using System.Collections.Generic;

namespace EasySave.Controller
{
    // Initialize the Controller class
    public class Controller
    {
        private Model.BackupJob model;
        private View.View view;
        private BackupManager backupManager;

        // Constructor initializing model, view, and the backup manager 
        public Controller(Model.BackupJob model, View.View view)
        {
            this.model = model;
            this.view = view;
            this.backupManager = new BackupManager();
        }

        // Create Add method
        private bool AddBackupJob()
        {
            // Verify if we reach the limits
            if (backupManager.GetBackupJobs().Count >= 5)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nLimite maximale de 5 travaux de sauvegarde atteinte. Impossible d'ajouter de nouveaux travaux.");
                Console.ResetColor();
                return false;
            }

            try
            {
                BackupJob backupJob = view.GetBackupJobDetails();
                backupManager.AddBackupJob(backupJob);
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nÉchec de l'ajout du travail de sauvegarde: {ex.Message}");
                Console.ResetColor();
                return false;
            }
        }

        // Create Welcome Method 
        public void DisplayWelcome()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Bienvenue sur EasySave !");
            Console.ResetColor();
            Console.WriteLine("\nAppuyez sur Entrée pour afficher le menu...");
            Console.ReadLine();
           
        }

        // Main method to run the application logic
        public void Run()
        {
            bool quit = false; // Flag to control the application loop

            // Display the welcome message and the main menu
            DisplayWelcome();
            view.DisplayMenu();

            while (!quit)
            {
                int choice = view.GetUserChoice(); // Get user's menu choice
                switch (choice)
                {
                    case 1: // Display backup jobs
                        Console.Clear();
                        var backupJobs = backupManager.GetBackupJobs();
                        view.DisplayBackupJobs(backupJobs);

                        Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
                        Console.ReadKey();
                        view.DisplayMenu();
                        break;

                    case 2: // Add a backup job
                        Console.Clear();
                        bool success = AddBackupJob();

                        if (success)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\nTravail de sauvegarde ajouté avec succès !");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nÉchec de l'ajout du travail de sauvegarde.");
                        }
                        Console.ResetColor();

                        Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
                        Console.ReadKey();

                        view.DisplayMenu();
                        break;

                    case 3: // Execute a backup job or jobs
                        Console.Clear();
                        backupJobs = backupManager.GetBackupJobs();

                        if (backupJobs.Count > 0)
                        {
                            view.DisplayBackupJobs(backupJobs); // Assurez-vous que cette méthode affiche les jobs avec des indices ou des noms.

                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("\nOptions d'exécution:");
                            Console.ResetColor();
                            Console.WriteLine("\nEntrez les noms des travaux à exécuter séparés par ';' pour une sélection individuelle.");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Exemple: 'Job1;Job3' pour exécuter Job1 et Job3.");
                            Console.ResetColor();
                            Console.WriteLine("\nUtilisez '-' pour spécifier un intervalle.");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Exemple: 'Job1-Job3' pour exécuter Job1 à Job3.");
                            Console.ResetColor();

                            string input = Console.ReadLine();

                            // Traiter l'entrée pour des travaux individuels ou un intervalle
                            if (input.Contains(";"))
                            {
                                var jobNames = input.Split(';');
                                foreach (var name in jobNames)
                                {
                                    backupManager.ExecuteBackupJob(name.Trim());
                                }
                            }
                            else if (input.Contains("-"))
                            {
                                var rangeParts = input.Split('-');
                                if (rangeParts.Length == 2)
                                {
                                    backupManager.ExecuteBackupJobsInRange(rangeParts[0].Trim(), rangeParts[1].Trim());
                                }
                            }
                            else
                            {
                                backupManager.ExecuteBackupJob(input.Trim());
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Aucun travail de sauvegarde à exécuter.");
                            Console.ResetColor();
                        }
                       
                        Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
                        Console.ReadKey();
                        view.DisplayMenu();
                        break;


                    case 4: // Delete a backup job
                        Console.Clear();
                        backupJobs = backupManager.GetBackupJobs();

                        if (backupJobs.Count > 0)
                        {
                            view.DisplayBackupJobsName(backupJobs);
                            bool isRemoved = false;
                            do
                            {
                                string jobName = view.AskForBackupJobName();

                                if (string.IsNullOrWhiteSpace(jobName))
                                {
                                    Console.Clear();
                                    view.DisplayMenu();
                                    break; // Exit the loop, return to the menu
                                }

                                isRemoved = backupManager.RemoveBackupJob(jobName);
                                if (isRemoved)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("\nTravail de sauvegarde supprimé avec succès.");
                                    Console.ResetColor();
                                    break; // Exit the loop after successful deletion
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("\nAucun travail de sauvegarde trouvé avec ce nom. Veuillez réessayer ou laisser vide pour retourner au menu.");
                                    Console.ResetColor();
                                }

                            } while (!isRemoved); // Continue until a job is successfully removed
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Aucun travail de sauvegarde disponible pour la suppression.");
                            Console.ResetColor();
                        }

                        view.DisplayMenu();
                        break;

                    case 5:
                        // Code to change language (not implemented)
                        break;

                    case 6: // Quit the application
                        quit = true;
                        break;

                    default:

                        Console.Clear();
                        Console.Write("Choix invalide ! Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                      Console.Clear();
                        break;
                }
            }
        }

    }
}
