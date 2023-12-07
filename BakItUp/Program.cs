using System;

namespace BackupProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            

            BackupLogic backupLogic = new BackupLogic();

            // Set the callback function using a lambda expression
            backupLogic.StatusUpdateCallback = (update, error) =>
            {
                // Code to handle the status update
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] {update}");
            };
            backupLogic.LoadSettingsFromFile(); 
            Console.WriteLine("Backup program started.");
            Console.WriteLine("Press CTRL+C to stop the program.");
            backupLogic.StartBackup();

            Console.WriteLine("Backup program stopped. Press any key to exit.");
            Console.ReadKey();
            backupLogic.SaveSettingsToFile();    
        }
    }
}