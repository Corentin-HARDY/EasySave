using EasySave.Controller;
using EasySave.Model;
using EasySave.View;
using System;

namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            Model.BackupJob model = new Model.BackupJob();
            View.View view = new View.View();
            Controller.Controller controller = new Controller.Controller(model, view);

            controller.Run();
        }
    }
}
