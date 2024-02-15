using EasySave.Model;
using System;
using System.Collections.Generic;

namespace EasySave.View
{
    public class View
    {
        Languages languages;
        public Languages Language { get => languages; set => languages = value; }

        // Display the main menu
        public void DisplayMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(this.Language.GetMessage("Menu"));
            Console.ResetColor();

            Console.WriteLine("\n" + this.Language.GetMessage("DisplayBackupJobs"));
            Console.WriteLine("\n" + this.Language.GetMessage("AddBackupJob"));
            Console.WriteLine("\n" + this.Language.GetMessage("ExecuteBackupJob"));
            Console.WriteLine("\n" + this.Language.GetMessage("DeleteBackupJob"));
            Console.WriteLine("\n" + this.Language.GetMessage("ChooseLanguage"));
            Console.WriteLine("\n" + this.Language.GetMessage("LogType"));
            Console.WriteLine("\n" + this.Language.GetMessage("Quit"));
        }

        // Get user Choice
        public int GetUserChoice()
        {
            int choice = 0;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n" + this.Language.GetMessage("EnterYourChoice"));
                Console.ResetColor();

                if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= 7)
                {
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n" + this.Language.GetMessage("InvalidChoicePleaseEnterNumber"));
                    Console.ResetColor();
                }
            }
            return choice;
        }

        // Get backupJob details
        public BackupJob GetBackupJobDetails()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(this.Language.GetMessage("CreateBackupJob"));
            Console.ResetColor();

            Console.Write("\n" + this.Language.GetMessage("ChooseName"));
            string name = Console.ReadLine();
            Console.Write("\n" + this.Language.GetMessage("Source"));
            string source = Console.ReadLine();
            Console.Write("\n" + this.Language.GetMessage("Target"));
            string target = Console.ReadLine();

            BackupType backupType;
            while (true)
            {
                Console.WriteLine("\n" + this.Language.GetMessage("BackupJobType"));
                Console.WriteLine(this.Language.GetMessage("Full"));
                Console.WriteLine(this.Language.GetMessage("Differential"));
                Console.Write(this.Language.GetMessage("EnterYourChoice"));
                string typeInput = Console.ReadLine().ToUpper();

                if (Enum.TryParse(typeInput, out backupType) && Enum.IsDefined(typeof(BackupType), backupType))
                {
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n" + this.Language.GetMessage("InvalidBackupType"));
                    Console.ResetColor();
                }
            }

            return new BackupJob(name, source, target, backupType);
        }

        //Display BackupJob List 
        public void DisplayBackupJobs(List<BackupJob> backupJobs)
        {
            if (backupJobs.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(this.Language.GetMessage("NoBackupJobs"));
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(this.Language.GetMessage("BackupJobsList"));
            Console.ResetColor();

            Console.WriteLine($"{"\n" + this.Language.GetMessage("Name"),-20} {this.Language.GetMessage("Src"),-25} {this.Language.GetMessage("Trg"),-25} {this.Language.GetMessage("Type"),-15}");
            Console.WriteLine(new string('-', 80));

            for (int i = 0; i < backupJobs.Count; i++)
            {
                string sourcePath = TruncatePath(backupJobs[i].Source, 22);
                string targetPath = TruncatePath(backupJobs[i].Target, 22);
                Console.WriteLine($"{backupJobs[i].Name,-20} {sourcePath,-25} {targetPath,-25} {backupJobs[i].BackupType,-15}");
            }
        }

        //Display backupJob name 
        public void DisplayBackupJobsName(List<BackupJob> backupJobs)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(this.Language.GetMessage("BackupJobsList"));
            Console.ResetColor();
            Console.WriteLine($"{"\n" + this.Language.GetMessage("Name"),-20} {this.Language.GetMessage("Type"),-15}");

            Console.WriteLine(new string('-', 30));

            for (int i = 0; i < backupJobs.Count; i++)
            {
                Console.WriteLine($"{backupJobs[i].Name,-20} {backupJobs[i].BackupType,-15}");
            }
        }

        // Ask For backupJob Name
        public string AskForBackupJobName()
        {
            Console.Write("\n" + this.Language.GetMessage("EnterBackupJobName"));
            return Console.ReadLine();
        }

        static string TruncatePath(string path, int maxLength)
        {
            if (string.IsNullOrEmpty(path)) return path;
            if (path.Length <= maxLength) return path;

            return path.Substring(0, maxLength - 3) + "...";
        }
    }
}
