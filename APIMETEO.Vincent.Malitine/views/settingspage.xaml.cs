using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using APIMETEO.Vincent.Malitine.model;
using Settings = APIMETEO.Vincent.Malitine.Properties.Settings;

namespace APIMETEO.Vincent.Malitine.views
{
    /// <summary>
    /// Logique d'interaction pour settingspage.xaml
    /// </summary>
    public partial class settingspage : Page
    {
        private ObservableCollection<ThemeItem> _themes = new();

        public settingspage()
        {
            InitializeComponent();
            LoadThemes();
            LoadDefaultCity();
        }

        private void LoadThemes()
        {
            string currentTheme = Settings.Default.SelectedTheme;
            if (string.IsNullOrEmpty(currentTheme))
                currentTheme = "GitHub Dark";

            _themes.Clear();

            foreach (var theme in ThemeManager.Themes)
            {
                _themes.Add(new ThemeItem
                {
                    Name = theme.Key,
                    CardColor = ConvertColor(theme.Value.CardBackground),
                    AccentColor = ConvertColor(theme.Value.AccentColor),
                    SecondaryColor = ConvertColor(theme.Value.SecondaryBackground),
                    BorderColor = theme.Key == currentTheme ? ConvertColor(theme.Value.AccentColor) : Brushes.Transparent,
                    IsSelected = theme.Key == currentTheme
                });
            }

            ThemesListControl.ItemsSource = _themes;
        }

        private void LoadDefaultCity()
        {
            string defaultCity = Settings.Default.DefaultCity;
            if (string.IsNullOrEmpty(defaultCity))
                defaultCity = "Annecy";

            DefaultCityTextBox.Text = defaultCity;
            CurrentCityDisplay.Text = defaultCity;
        }

        private void ThemeCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string themeName)
            {
                // Applique le nouveau thème
                ThemeManager.ApplyTheme(themeName);

                // Met à jour l'affichage
                LoadThemes();

                // Message de confirmation
                MessageBox.Show($"Thème '{themeName}' appliqué avec succès!", 
                    "Thème changé", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveCityBtn_Click(object sender, RoutedEventArgs e)
        {
            string newCity = DefaultCityTextBox.Text.Trim();

            if (string.IsNullOrEmpty(newCity))
            {
                MessageBox.Show("Veuillez entrer un nom de ville valide.", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Sauvegarde la nouvelle ville par défaut
            Settings.Default.DefaultCity = newCity;
            Settings.Default.Save();

            // Met à jour l'affichage
            CurrentCityDisplay.Text = newCity;

            // Informe la MainWindow de mettre à jour la ville
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.UpdateDefaultCity(newCity);
            }

            MessageBox.Show($"Ville par défaut changée en '{newCity}'.\nLa météo sera mise à jour automatiquement.", 
                "Ville enregistrée", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private SolidColorBrush ConvertColor(string hex)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }
    }

    public class ThemeItem
    {
        public required string Name { get; set; }
        public required SolidColorBrush CardColor { get; set; }
        public required SolidColorBrush AccentColor { get; set; }
        public required SolidColorBrush SecondaryColor { get; set; }
        public required SolidColorBrush BorderColor { get; set; }
        public bool IsSelected { get; set; }
    }
}
