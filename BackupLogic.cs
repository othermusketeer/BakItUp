using System;
using System.IO;
using System.IO.Compression;

namespace BackupProgram
{
    public delegate void StatusUpdateCallback(string update, bool error);
    
    public class BackupLogic
    {
        public BackupSettings settings;

        public StatusUpdateCallback? StatusUpdateCallback { get; set; }

        public BackupLogic()
        {
            settings = new BackupSettings();
        }

        public BackupLogic(string backupFolderPath)
        {
            settings = new BackupSettings(backupFolderPath);
        }

        public BackupLogic(string backupFolderPath, int intervalMinutes)
        {
            settings = new BackupSettings(backupFolderPath, intervalMinutes);
        }

        public BackupLogic(string backupFolderPath, int intervalMinutes, string outputFolderPath)
        {
            settings = new BackupSettings(backupFolderPath, intervalMinutes, outputFolderPath);
        }

        // Load settings from file, no arguments
        public void LoadSettingsFromFile()
        {
            LoadSettingsFromFile("settings.json");
        }

        // Load settings from file
        public void LoadSettingsFromFile(string filePath)
        {
            if (Path.Exists(filePath)) {
                settings.LoadFromFile(filePath);
            } else {
                settings.SaveToFile(filePath);
            }

            // Update status showing loaded settings
           
            StatusUpdateCallback?.Invoke("Loaded settings. Interval: " + settings.IntervalSeconds + " sec,  Backup Folder: " + settings.BackupFolderPath + ",  Output Folder: " + settings.OutputFolderPath, false);
        }

        // Save settings to file
        public void SaveSettingsToFile()
        {
            SaveSettingsToFile("settings.json");
        }

        public void SaveSettingsToFile(string filePath)
        {
            settings.SaveToFile(filePath);
        }

        public void StartBackup()
        {
            while (BackupFiles())
            {
                System.Threading.Thread.Sleep(settings.IntervalSeconds * 1000); // 30 minutes delay
            }
        }

        private bool BackupFiles()
        {
            string outputFolderPath;
            // Implement the logic to backup files in the specified folder to zip file, named with current date and time, to the output folder

            // create the output folder if it doesn't exist
            outputFolderPath = Path.GetFullPath(settings.OutputFolderPath);
            
            try {
                if (!Directory.Exists(outputFolderPath))
                {
                    Directory.CreateDirectory(outputFolderPath);
                }
            } catch (Exception ex) {
                StatusUpdateCallback?.Invoke($"Error creating output folder: {ex.Message}", true);
                return false;
            }

            string pendingError = "";

            try {
                StatusUpdateCallback?.Invoke("Backup started", false);
                
                // Artificial delay, to avoid hitting CTRL+C during a save
                System.Threading.Thread.Sleep(3000);

                pendingError = "Error getting a list of files in the backup folder.";
                // Create a list of the files in the backup folder
                string[] files = Directory.GetFiles(settings.BackupFolderPath);

                // Create a zip file containing all the files in the backup folder
                string zipFilePath = Path.Combine(outputFolderPath, $"backup_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.zip");
                pendingError = "Error creating a zip file (" + zipFilePath + ").";
                using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    foreach (string file in files)
                    {
                        zip.CreateEntryFromFile(file, Path.GetFileName(file));
                    }
                }
            } catch (Exception ex) {
                StatusUpdateCallback?.Invoke($"Error backing up files:\n\t\t{pendingError}\n\t\tError: {ex.Message}", true);
                return true;
            }
            
            StatusUpdateCallback?.Invoke("Backup completed.", false);
            return true;
        }
    }
}