using ImageCombinerChannelExtractor.Components.Enums;
using ImageCombinerChannelExtractor.Components.Helpers;
using ImageCombinerChannelExtractor.Components.Settings.Enums;
using ImageCombinerChannelExtractor.Components.Shared;
using System.IO;
using System.Text.Json;
using Wpf.Ui.Appearance;

namespace ImageCombinerChannelExtractor.Components.Settings
{
    public static class SettingsHelper
    {
        private static readonly string _settingsFileDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vibrant Pixels", "Image Combiner & Channel Extractor");
        private static readonly string _settingsFilePathWithFile = Path.Combine(_settingsFileDirectoryPath, "settings.json");

        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions 
        {
            WriteIndented = true
        };

        private static SettingsClass _settingsFile { get; set; } = Load();

        public static SettingsClass Load()
        {
            try
            {
                if (File.Exists(_settingsFilePathWithFile))
                {
                    string json = File.ReadAllText(_settingsFilePathWithFile);
                    SettingsClass? loaded = JsonSerializer.Deserialize<SettingsClass>(json);
                    if (loaded != null)
                    {
                        _settingsFile = loaded;
                        return loaded;
                    }
                }
            }
            catch
            {
                // Fallback to default
            }
            _settingsFile = new SettingsClass();
            return _settingsFile;
        }

        public static void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(_settingsFileDirectoryPath);
                string json = JsonSerializer.Serialize(_settingsFile, _serializerOptions);
                File.WriteAllText(_settingsFilePathWithFile, json);
            }
            catch (Exception ex)
            {
                // fails to save
                LogHelper.Log(ex, nameof(SaveSettings));
                App.TriggerNotification(StringLinesInfo.notificationException, StringLinesInfo.notificationExceptionDescr, NotificationTypeEnum.Error);
            }
        }

        #region Get/set values
        public static ApplicationTheme GetThemeMode()
        {
            return _settingsFile.ColorThemeMode;
        }
        public static void SetThemeMode(ApplicationTheme theme)
        {
            _settingsFile.ColorThemeMode = theme;
            SaveSettings();
        }

        public static Type GetStartupPage() => _settingsFile.StartupPage switch
        {
            StartupPageEnum.Extractor => typeof(Pages.ExtractorPage),
            _ => typeof(Pages.CombinerPage)
        };
        public static StartupPageEnum GetStartupPageEnum()
        {
            return _settingsFile.StartupPage;
        }
        public static void SetStartupPage(StartupPageEnum page)
        {
            _settingsFile.StartupPage = page;
            SaveSettings();
        }
        #endregion
    }
}
