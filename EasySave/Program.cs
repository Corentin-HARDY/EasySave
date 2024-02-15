using EasySave.Controller;
using EasySave.Model;
using EasySave.View;
using System;
using System.IO;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.CreateDirectory(@"C:\Temp");
            Model.BackupJob model = new Model.BackupJob();
            View.View view = new View.View();
            Languages l = new Languages();
            view.Language = l;
            Controller.Controller controller = new Controller.Controller(model, view);
            BackupManager backupManager = new BackupManager(view);
            controller.Run();
        }
    }
}
