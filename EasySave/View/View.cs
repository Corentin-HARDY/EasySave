using EasySave.Model;
using System;
using System.Collections.Generic;

namespace EasySave.View
{
    public class View
    {
        // Display the main menu
        public void DisplayMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan; 
            Console.WriteLine("Menu:");
            Console.ResetColor();

            // Displaying menu options
            Console.WriteLine("1. Afficher les travaux de sauvegarde");
            Console.WriteLine("2. Ajouter un travail de sauvegarde");
            Console.WriteLine("3. Exécuter un travail de sauvegarde");
            Console.WriteLine("4. Supprimer un travail de sauvegarde");
            Console.WriteLine("5. Choisir une langue");
            Console.WriteLine("6. Quitter");
        }

        // Get the user's choice from the menu
        public int GetUserChoice()
        {
            int choice = 0;
            while (true) // Loop until a valid choice is made
            {
                Console.ForegroundColor = ConsoleColor.Yellow; 
                Console.Write("\nEntrez votre choix : ");
                Console.ResetColor();

                // Try to parse the user input; if successful and within valid range, break out of the loop
                if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= 6)
                {
                    break;
                }
                else
                {
                    // If the input is not valid, display an error message and continue the loop
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nChoix invalide. Veuillez entrer un nombre entre 1 et 6.");
                }
            }
            return choice;
        }

        // Collect information to create a new BackupJob
        public BackupJob GetBackupJobDetails()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Création d'un nouveau BackupJob");
            Console.ResetColor();

            // Prompting for job details: name, source, and target
            Console.Write("\nNom du travail : ");
            string name = Console.ReadLine();
            Console.Write("\nSource : ");
            string source = Console.ReadLine();
            Console.Write("\nCible : ");
            string target = Console.ReadLine();

            BackupType backupType;
            // Loop until a valid BackupType is entered
            while (true)
            {
                Console.WriteLine("\nChoisissez le type de sauvegarde :");
                Console.WriteLine(" - FULL");
                Console.WriteLine(" - DIFFERENTIAL");
                Console.Write("\nVotre choix : ");
                string typeInput = Console.ReadLine().ToUpper();

                // Try to parse the input to a BackupType
                if (Enum.TryParse(typeInput, out backupType) && Enum.IsDefined(typeof(BackupType), backupType))
                {
                    break; // Break the loop if input is valid
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nType de sauvegarde invalide. Veuillez saisir 'FULL' ou 'DIFFERENTIAL'.");
                    Console.ResetColor();
                }
            }

            return new BackupJob(name, source, target, backupType);
        }


        // Display a list of BackupJobs
        public void DisplayBackupJobs(List<BackupJob> backupJobs)
        {
            if (backupJobs.Count == 0) // Check if the list is empty
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Aucun BackupJob n'est disponible.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Liste des BackupJob disponibles : \n");
            Console.ResetColor();

            // Display table headers
            Console.WriteLine($"{"Num",-4} {"Nom",-20} {"Source",-25} {"Cible",-25} {"Type",-15}");
            Console.WriteLine(new string('-', 90)); // Separator

            // Display each BackupJob in a formatted manner
            for (int i = 0; i < backupJobs.Count; i++)
            {
                // Truncate paths to fit the display
                string sourcePath = TruncatePath(backupJobs[i].Source, 22);
                string targetPath = TruncatePath(backupJobs[i].Target, 22);

                // Display the BackupJob details
                Console.WriteLine($"{i + 1,-4} {backupJobs[i].Name,-20} {sourcePath,-25} {targetPath,-25} {backupJobs[i].BackupType,-15}");
            }
        }

        // Truncate a path if it exceeds a certain length
        static string TruncatePath(string path, int maxLength)
        {
            if (string.IsNullOrEmpty(path)) return path;
            if (path.Length <= maxLength) return path;

            return path.Substring(0, maxLength - 3) + "...";
        }

        // Display only the names of BackupJobs
        public void DisplayBackupJobsName(List<BackupJob> backupJobs)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Liste des BackupJob disponibles : \n");
            Console.ResetColor();

            Console.WriteLine($"{"Num",-4} {"Nom",-20} {"Type",-15}");
            Console.WriteLine(new string('-', 40)); // Separator

            // Display each BackupJob name and type
            for (int i = 0; i < backupJobs.Count; i++)
            {
                Console.WriteLine($"{i + 1,-4} {backupJobs[i].Name,-20} {backupJobs[i].BackupType,-15}");
            }
        }

        // Prompt the user to enter the name of a BackupJob
        public string AskForBackupJobName()
        {
            Console.Write("\nEntrez le nom du BackupJob : ");
            return Console.ReadLine();
        }
    }
}
