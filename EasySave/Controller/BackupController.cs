using EasySave.View;
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
        Log logInstance = new Log();

        // Constructor initializing model, view, and the backup manager 
        public Controller(Model.BackupJob model, View.View view)
        {
            this.model = model;
            this.view = view;
            this.backupManager = new BackupManager(view);
        }

        // Create Add method
        private bool AddBackupJob()
        {
            // Verify if we reach the limits
            if (backupManager.GetBackupJobs().Count >= 5)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(this.view.Language.GetMessage("MaxBackupJobsReached"));
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
                Console.WriteLine("\n" + this.view.Language.GetMessage("AddBackupJobFailed") + " " + ex.Message);
                Console.ResetColor();
                return false;
            }
        }

        // Create Welcome Method 
        public void DisplayWelcome()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(this.view.Language.GetMessage("WelcomeEasySave"));
            Console.ResetColor();
            Console.WriteLine("\n" + this.view.Language.GetMessage("AccessMenu"));
            Console.ReadLine();
           
        }

        // change language 
        private void ChangeLanguage()
        {
            Console.WriteLine(this.view.Language.GetMessage("ChooseLangue"));
            Console.WriteLine("\n" + this.view.Language.GetMessage("English"));
            Console.WriteLine(this.view.Language.GetMessage("French"));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n" + this.view.Language.GetMessage("EnterYourChoice"));
            Console.ResetColor();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    view.Language.CurrentLangue = ChooseLangue.En;
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("\nWelcome to the english version !");
                    Console.ResetColor();
                    break;
                case "2":
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    view.Language.CurrentLangue = ChooseLangue.Fr;
                    Console.WriteLine("\nBienvenue sur la version française !");
                    Console.ResetColor();
                    break;
            }
            view.Language.LoadPhrases();
            Console.WriteLine("\n" + this.view.Language.GetMessage("PressKeyToReturnToMenu"));
            Console.ReadKey();
            view.DisplayMenu();
        }


        //Choose file type 
        public void ChooseLogFormat()
        {
            Console.WriteLine(this.view.Language.GetMessage("LogFileFormat"));
            Console.WriteLine("\n1. JSON");
            Console.WriteLine("2. XML");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n" + this.view.Language.GetMessage("EnterYourChoice"));
            Console.ResetColor();
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n" + this.view.Language.GetMessage("JSON"));
                    Console.ResetColor();
                    Console.WriteLine("\n" + this.view.Language.GetMessage("Path"));
                    Console.WriteLine("\n" + this.view.Language.GetMessage("PressKeyToReturnToMenu"));
                    Console.ReadKey();
                    view.DisplayMenu();
                    break;
                case "2":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n" + this.view.Language.GetMessage("XML"));
                    Console.ResetColor();
                    Console.WriteLine("\n" + this.view.Language.GetMessage("Path"));
                    Console.WriteLine("\n" + this.view.Language.GetMessage("PressKeyToReturnToMenu"));
                    Console.ReadKey();
                    view.DisplayMenu();
                    break;

            }

        }


        // Main method to run the application logic
        public void Run()
        {
            bool quit = false;

            // Display the welcome message and the main menu
            DisplayWelcome();
            view.DisplayMenu();

            while (!quit)
            {
                int choice = view.GetUserChoice(); 
                switch (choice)
                {
                    case 1: // Display backup jobs
                        Console.Clear();
                        var backupJobs = backupManager.GetBackupJobs();
                        view.DisplayBackupJobs(backupJobs);

                        Console.WriteLine("\n" + this.view.Language.GetMessage("PressKeyToReturnToMenu"));
                        Console.ReadKey();
                        view.DisplayMenu();
                        break;

                    case 2: // Add a backup job
                        Console.Clear();
                        bool success = AddBackupJob();

                        if (success)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n" + this.view.Language.GetMessage("BackupJobAddedSuccessfully"));
                            Console.ResetColor();
                            Console.WriteLine("\n" + this.view.Language.GetMessage("PressKeyToReturnToMenu"));
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n" + this.view.Language.GetMessage("BackupJobAddedFailed"));
                        }
                        Console.ResetColor();
                        Console.WriteLine("\n" + this.view.Language.GetMessage("\nPressKeyToReturnToMenu"));
                        Console.ReadKey();

                        view.DisplayMenu();
                        break;

                    case 3: // Execute a backup job or jobs
                        Console.Clear();
                        backupJobs = backupManager.GetBackupJobs();

                        if (backupJobs.Count > 0)
                        {
                            view.DisplayBackupJobs(backupJobs); 

                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("\n" + this.view.Language.GetMessage("ExecutionOptions"));
                            Console.ResetColor();
                            Console.WriteLine("\n" + this.view.Language.GetMessage("EnterBackupJobNames"));
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine(this.view.Language.GetMessage("ExampleJobExecution"));
                            Console.ResetColor();
                            Console.WriteLine("\n" + this.view.Language.GetMessage("UseDashForRange"));
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine(this.view.Language.GetMessage("ExampleRangeExecution"));
                            Console.ResetColor();
                            Console.Write("\n" + this.view.Language.GetMessage("EnterYourChoice"));
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
                            Console.WriteLine("\n" + this.view.Language.GetMessage("NoBackupJobsToExecute"));
                            Console.ResetColor();
                        }
                       
                        Console.WriteLine("\n" + this.view.Language.GetMessage("PressKeyToReturnToMenu"));
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

                                isRemoved = backupManager.RemoveBackupJob(jobName);
                                if (isRemoved)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("\n" + this.view.Language.GetMessage("BackupJobDeletedSuccessfully"));
                                    Console.ResetColor();
                                    Console.WriteLine("\n" + this.view.Language.GetMessage("PressKeyToReturnToMenu"));
                                    Console.ReadLine(); 
                                    break; // Exit the loop after successful deletion
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("\n" + this.view.Language.GetMessage("NoBackupJobsFoundedWithName"));
                                    Console.ResetColor();
                                }

                            } while (!isRemoved); // Continue until a job is successfully removed
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(this.view.Language.GetMessage("NoBackupJobsAvailableForDeletion"));
                            Console.ResetColor();
                        }

                        view.DisplayMenu();
                        break;

                    case 5:
                        // Change languages
                        Console.Clear();
                        ChangeLanguage();
                        break;

                    case 6:
                        // Change type log
                        Console.Clear();
                        ChooseLogFormat();
                           break;

                    case 7: // Quit the application
                        quit = true;
                        break;

                    default:

                        Console.Clear();
                        Console.Write("\n" + this.view.Language.GetMessage("InvalidOption"));
                        Console.ReadKey();
                      Console.Clear();
                        break;
                }
            }
        }

    }
}
