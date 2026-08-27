using ImageCombinerChannelExtractor.Components.Settings;
using ImageCombinerChannelExtractor.Components.Settings.Enums;
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

            StartupPageEnum pageSetting = SettingsHelper.GetStartupPageEnum();
            foreach (ComboBoxItem item in StartupPageComboBox.Items)
            {
                if (item.Tag?.ToString() == pageSetting.ToString())
                {
                    StartupPageComboBox.SelectedItem = item;
                    return;
                }
            }
            StartupPageComboBox.SelectedIndex = 0; // default to combiner
        }

        #region User inputs
        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing)
            {
                return;
            }

            if (ThemeComboBox.SelectedItem is ComboBoxItem selected && Enum.TryParse<ApplicationTheme>(selected.Tag?.ToString(), out ApplicationTheme theme))
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

            if (StartupPageComboBox.SelectedItem is ComboBoxItem selected && Enum.TryParse<StartupPageEnum>(selected.Tag?.ToString(), out StartupPageEnum page))
            {
                SettingsHelper.SetStartupPage(page);
            }
        }
        #endregion
    }
}
