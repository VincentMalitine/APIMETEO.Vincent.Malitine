using System;
using System.Windows;
using System.Windows.Input;

namespace APIMETEO.Vincent.Malitine
{
    // Fenêtre de dialogue pour rechercher une ville
    public partial class CitySearchWindow : Window
    {
        public string? SelectedCity { get; private set; }

        public CitySearchWindow()
        {
            InitializeComponent();
            // Focus automatique sur la zone de texte
            Loaded += (s, e) => CityTyping.Focus();
        }

        // Valide la saisie et ferme la fenêtre
        private void ValidateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CityTyping.Text))
            {
                SelectedCity = CityTyping.Text.Trim();
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Veuillez entrer un nom de ville.", "Champ vide", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Annule et ferme la fenêtre
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Gère les touches Entrée et Échap
        private void CityTyping_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidateBtn_Click(sender, e);
            }
            else if (e.Key == Key.Escape)
            {
                CancelBtn_Click(sender, e);
            }
        }
    }
}
