using ImageCombinerChannelExtractor.Components.Settings;
using ImageCombinerChannelExtractor.Components.Settings.Enums;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Appearance;

namespace ImageCombinerChannelExtractor.Components.Pages
{
    public partial class Options : Page
    {
        private readonly bool _isInitializing = true;

        public Options()
        {
            InitializeComponent();
            SetInitialSelection();

            Version? version = Assembly.GetEntryAssembly()?.GetName().Version;
            if (version != null)
            {
                VersionText.Text = $"Version {version.Major}.{version.Minor}.{version.Build}";
            }
            _isInitializing = false;
        }

        private void SetInitialSelection()
        {
            ApplicationTheme currentAppTheme = SettingsHelper.GetThemeMode();
            ThemeComboBox.SelectedIndex = 1; // default to dark mode
            foreach (ComboBoxItem item in ThemeComboBox.Items)
            {
                if (item.Tag?.ToString() == currentAppTheme.ToString())
                {
                    ThemeComboBox.SelectedItem = item;
                }
            }

            StartupPageComboBox.SelectedIndex = 0; // default to combiner
            StartupPageEnum pageSetting = SettingsHelper.GetStartupPageEnum();
            foreach (ComboBoxItem item in StartupPageComboBox.Items)
            {
                if (item.Tag?.ToString() == pageSetting.ToString())
                {
                    StartupPageComboBox.SelectedItem = item;
                }
            }
            DoNotifCheckbox.IsChecked = SettingsHelper.GetEnableNotifications();
            DoExtrFlickerCheckbox.IsChecked = SettingsHelper.GetDoExtractorScreenFlicker();
        }

        #region User inputs
        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing)
            {
                return;
            }

            if (ThemeComboBox.SelectedItem is ComboBoxItem { Tag: ApplicationTheme theme })
            {
                SettingsHelper.SetThemeMode(theme);
                App.ApplyTheme(theme);
            }
        }

        private void StartupPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing)
            {
                return;
            }

            if (StartupPageComboBox.SelectedItem is ComboBoxItem { Tag: StartupPageEnum page })
            {
                SettingsHelper.SetStartupPage(page);
            }
        }

        private void DoNotifCheckbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitializing)
            {
                return;
            }
            SettingsHelper.SetEnableNotifications(DoNotifCheckbox.IsChecked == true);
        }

        private void DoExtrFlickerCheckbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_isInitializing)
            {
                return;
            }
            SettingsHelper.SetDoExtractorScreenFlicker(DoExtrFlickerCheckbox.IsChecked == true);
        }
        #endregion
    }
}
