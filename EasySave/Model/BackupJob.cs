using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasySave.Model
{
    //Create enum BackupType
    public enum BackupType
    {
        FULL,
        DIFFERENTIAL
    }

    //Initiate BackupJob class
    public class BackupJob
    {
        // --- Attributes ---
        public string Name { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public BackupType BackupType { get; set; } 
        public State State { get; set; }

        // Prepare options to indent JSON Files
        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        // --- Constructors ---
        // Constructor used in Programs.cs
        public BackupJob() { }

        // Constructor used to get BackupJob data from user()
        public BackupJob(string name, string source, string target, BackupType backupType)
        {
            Name = name;
            Source = source;
            Target = target;
            this.BackupType = backupType; 
            State = null;
        }



    }
}

    
