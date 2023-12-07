using System;
using System.IO;
using System.Text.Json;

namespace BackupProgram
{
    public class BackupSettings
    {
        public int IntervalMinutes { get; set; }
        public string BackupFolderPath { get; set; }
        public string OutputFolderPath { get; set; }
        public bool SkipUnmodified { get; set; }

        public BackupSettings()
        {
            // Set default values
            IntervalMinutes = 30;
            BackupFolderPath = "./saves";
            OutputFolderPath = "./save_backups";
            SkipUnmodified = false;
        }

        public BackupSettings(string backupFolderPath)
            : this()
        {
            BackupFolderPath = backupFolderPath;
        }

        public BackupSettings(string backupFolderPath, int intervalMinutes)
            : this(backupFolderPath)
        {
            IntervalMinutes = intervalMinutes;
        }

        public BackupSettings(string backupFolderPath, int intervalMinutes, string outputFolderPath)
            : this(backupFolderPath, intervalMinutes)
        {
            OutputFolderPath = outputFolderPath;
        }

        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                //Console.WriteLine("Settings file not found. Using default settings.");
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                BackupSettings loadedSettings = JsonSerializer.Deserialize<BackupSettings>(json);

                // Update the current instance with the loaded settings
                IntervalMinutes = loadedSettings.IntervalMinutes;
                BackupFolderPath = loadedSettings.BackupFolderPath;
                OutputFolderPath = loadedSettings.OutputFolderPath;
                SkipUnmodified = loadedSettings.SkipUnmodified;

                //Console.WriteLine("Settings loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings file: {ex.Message}");
            }
        }

        public void SaveToFile(string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);

                Console.WriteLine("Settings saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings file: {ex.Message}");
            }
        }
    }
}