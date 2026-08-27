using ImageCombinerChannelExtractor.Components.Settings;
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
            foreach (ComboBoxItem item in ThemeComboBox.Items)
            {
                if (item.Tag?.ToString() == currentAppTheme.ToString())
                {
                    ThemeComboBox.SelectedItem = item;
                    return;
                }
            }
            ThemeComboBox.SelectedIndex = 1; // default to dark mode
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
        #endregion
    }
}
