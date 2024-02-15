using System.Collections.Generic;
using EasySave.Model;
using System.Text.Json;
using System.IO;
using System.Linq;
using System;

namespace EasySave.Controller
{
    public class BackupManager
    {
        private List<BackupJob> backupJobs = new List<BackupJob>();
        private View.View view;

        // Path to the JSON file
        private readonly string filePath = "backupJobs.json";

        public BackupManager(View.View view)
        {
            this.view = view; // Pass the initialized view from outside
            // Load BackupJobs on startup
            LoadBackupJobs();
        }

        // Adding a BackupJob to the JSON file for persistence
        public void AddBackupJob(BackupJob backupJob)
        {
            backupJobs.Add(backupJob);
            // Save after adding
            SaveBackupJobs();
        }

        // Save the list of BackupJobs to a JSON file
        private void SaveBackupJobs()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(backupJobs, options);
            File.WriteAllText(filePath, jsonString);
        }

        // Load the list of BackupJobs from a JSON file
        private void LoadBackupJobs()
        {
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                backupJobs = JsonSerializer.Deserialize<List<BackupJob>>(jsonString) ?? new List<BackupJob>();
            }
        }

        // Retrieve the BackupJobs
        public List<BackupJob> GetBackupJobs()
        {
            return backupJobs;
        }


        // ----Fonctions------

        // Method to remove a BackupJob
        public bool RemoveBackupJob(string jobName)
        {
            var jobToRemove = backupJobs.FirstOrDefault(job => job.Name == jobName);
            if (jobToRemove != null)
            {
                backupJobs.Remove(jobToRemove);
                // Save after removing
                SaveBackupJobs();
                return true; // Return true if removal was successful
            }
            return false; // Return false if no matching job was found
        }


        //LogFile Location
        public static string LogLocation { get; set; } = @"C:\Temp";

        public static void InitializeLogDirectory()
        {
            // Cela créera le dossier s'il n'existe pas déjà
            Directory.CreateDirectory(LogLocation);
        }



        // Method to execute a BackupJob
        public bool ExecuteBackupJob(string jobName)
        {
            var jobToExecute = backupJobs.FirstOrDefault(job => job.Name == jobName);
            if (jobToExecute != null)
            {
                Console.WriteLine("\n" + this.view.Language.GetMessage("ExecutingBackupJob") + $"'{jobName}'...");
                DateTime startTime = DateTime.Now; // Capture the start of the backupJob

                try
                {
                    switch (jobToExecute.BackupType)
                    {
                        case BackupType.FULL:
                            Console.WriteLine("\n" + this.view.Language.GetMessage("ExecutingFullBackup"));
                            ExecuteFullBackup(jobToExecute);
                            break;
                        case BackupType.DIFFERENTIAL:
                            Console.WriteLine("\n" + this.view.Language.GetMessage("ExecutingDifferentialBackup"));
                            ExecuteDifferentialBackup(jobToExecute);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("UnsupportedBackupType");
                    }

                    // Logique de journalisation ici
                    DateTime endTime = DateTime.Now;
                    TimeSpan duration = endTime - startTime;
                    string elapsedTime = duration.ToString(@"hh\:mm\:ss");

                    // Calcul the total File size.
                    string totalSize = CalculateFolderSizeAsString(LogLocation);

                    static string CalculateFolderSizeAsString(string LogLocation)
                    {
                        long totalSize = 0;
                        string[] fileNames = Directory.GetFiles(LogLocation, "*.*", SearchOption.AllDirectories);
                        foreach (string fileName in fileNames)
                        {
                            FileInfo fileInfo = new FileInfo(fileName);
                            totalSize += fileInfo.Length;
                        }
                        // Get fileSize
                        return FormatSize(totalSize);
                    }

                    static string FormatSize(long bytes)
                    {
                        const int scale = 1024;
                        string[] orders = new string[] { "Go", "Mo", "Ko", "octets" };
                        long max = (long)Math.Pow(scale, orders.Length - 1);

                        foreach (string order in orders)
                        {
                            if (bytes > max)
                                return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                            max /= scale;
                        }
                        return "0 octets";
                    }

                    // Create the go File
                    Log backupLog = new Log(jobName, jobToExecute.Source, jobToExecute.Target, totalSize, startTime.ToString(), elapsedTime);

                    // Write in the log File
                    backupLog.WriteToLogFile(LogLocation);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(this.view.Language.GetMessage($"\nUne erreur s'est produite lors de la sauvegarde : {ex.Message}"));
                    Console.ResetColor();
                    return false;
                }

                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n" + this.view.Language.GetMessage("NoBackupJobWithName") + $"'{jobName}'...");
                Console.ResetColor();
                return false;
            }
        }


        // Execute in range
        public void ExecuteBackupJobsInRange(string startName, string endName)
        {
            var startIndex = backupJobs.FindIndex(job => job.Name.Equals(startName, StringComparison.OrdinalIgnoreCase));
            var endIndex = backupJobs.FindIndex(job => job.Name.Equals(endName, StringComparison.OrdinalIgnoreCase));

            if (startIndex >= 0 && endIndex >= 0 && startIndex <= endIndex)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    ExecuteBackupJob(backupJobs[i].Name);
                }
            }
            else
            {
                Console.WriteLine("\n" + this.view.Language.GetMessage("InvalidJobNameRange"));
            }
        }


        // Execute a full backupJop
        private void ExecuteFullBackup(BackupJob job)
        {
            var sourceDir = new DirectoryInfo(job.Source);
            var targetDir = new DirectoryInfo(job.Target);

            // Create the target directory if it does not exist
            if (!targetDir.Exists)
            {
                targetDir.Create();
            }

            // Copy all files
            foreach (FileInfo file in sourceDir.GetFiles())
            {
                string targetFilePath = Path.Combine(job.Target, file.Name);
                file.CopyTo(targetFilePath, true); // true to overwrite an existing file
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + this.view.Language.GetMessage("FullBackupCompletedSuccessfully"));
            Console.ResetColor();

            // Update and save the state at the end of the full backup
            State state = new State
            {
                LastBackupDate = DateTime.Now // Update the last full backup date
            };
            SaveState(state, job.Name);
        }

        // Execute a differential backupJob
        private void ExecuteDifferentialBackup(BackupJob job)
        {
            State lastFullBackupState = LoadState(job.Name);
            var sourceDir = new DirectoryInfo(job.Source);
            var targetDir = new DirectoryInfo(job.Target);

            if (!targetDir.Exists) targetDir.Create();

            DateTime lastBackupDate = lastFullBackupState?.LastBackupDate ?? DateTime.MinValue;

            foreach (FileInfo file in sourceDir.GetFiles())
            {
                if (file.LastWriteTime > lastBackupDate)
                {
                    string targetFilePath = Path.Combine(job.Target, file.Name);
                    file.CopyTo(targetFilePath, true);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + this.view.Language.GetMessage("DifferentialBackupCompletedSuccessfully"));
            Console.ResetColor();
        }

        // Method to save the state of a backup
        private void SaveState(State state, string jobName)
        {
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Etats");
            string filePath = Path.Combine(directoryPath, $"{jobName}_state.json");

            // Check if the directory exists, if not, create it
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        // Method to load the state of a backup
        private State LoadState(string jobName)
        {
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Etats");
            string filePath = Path.Combine(directoryPath, $"{jobName}_state.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<State>(json);
            }
            return null;
        }


    }
}
