using APIMETEO.Vincent.Malitine.views;
using APIMETEO.Vincent.Malitine.model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Globalization;
using System.Text;
using System.Windows.Media;

namespace APIMETEO.Vincent.Malitine
{
    /// <summary>
    /// Fenêtre principale de l'application météo
    /// </summary>
    public partial class MainWindow : Window
    {
        public string City = "Annecy";
        private meteopage? _meteoPage;

        public MainWindow()
        {
            InitializeComponent();

            // Charger le thème sauvegardé
            ThemeManager.LoadSavedTheme();

            // Charger la ville par défaut
            LoadDefaultCity();

            // Créer et charger meteopage
            _meteoPage = new meteopage();
            MainFrame.Navigate(_meteoPage);

            UpdateCityDisplay();
            SetWeatherDataAsync();
        }

        private void LoadDefaultCity()
        {
            string defaultCity = Properties.Settings.Default.DefaultCity;
            if (!string.IsNullOrEmpty(defaultCity))
            {
                City = defaultCity;
            }
        }

        private void UpdateCityDisplay()
        {
            if (_meteoPage != null)
            {
                _meteoPage.CityNameDisplay.Text = City;
            }
        }

        /// Boucle de mise à jour des données météo toutes les secondes
        private async void SetWeatherDataAsync()
        {
            while (true)
            {
                // Attend 1 seconde avant la prochaine mise à jour
                await Task.Delay(1000);

                // Récupère les données depuis l'API
                var weatherData = await GetWeatherDataAsync();

                if (weatherData != null && _meteoPage != null)
                {
                    // Mise à jour des conditions actuelles
                    if (weatherData.current_condition != null)
                    {
                        // Affiche la température actuelle
                        _meteoPage.Temp.Text = $"{weatherData.current_condition.tmp}°C";
                        // Affiche le taux d'humidité
                        _meteoPage.Precip.Text = $"{weatherData.current_condition.humidity}%";
                        // Affiche l'icône météo correspondante
                        _meteoPage.Weather.Text = GetWeatherIcon(weatherData.current_condition.condition_key);
                    }

                    // Mise à jour des températures du jour
                    if (weatherData.fcst_day_0 != null)
                    {
                        // Température maximale
                        _meteoPage.TempHigh.Text = $"{weatherData.fcst_day_0.tmax}°C";
                        // Température minimale
                        _meteoPage.TempLow.Text = $"{weatherData.fcst_day_0.tmin}°C";
                        // Calcul et affichage de la température moyenne
                        int tempMoyenne = (weatherData.fcst_day_0.tmax + weatherData.fcst_day_0.tmin) / 2;
                        _meteoPage.TempMid.Text = $"{tempMoyenne}°C";
                    }

                    // Met à jour les prévisions des jours suivants
                    UpdateForecasts(weatherData);
                }
            }
        }

        /// Crée la liste des prévisions pour les 4 prochains jours
        private void UpdateForecasts(Root weatherData)
        {
            if (_meteoPage == null) return;

            var forecasts = new List<ForecastDay>();

            // Ajout des prévisions pour le jour 1
            if (weatherData.fcst_day_1 != null)
            {
                forecasts.Add(new ForecastDay
                {
                    DayName = weatherData.fcst_day_1.day_short ?? "N/A",
                    Date = weatherData.fcst_day_1.date ?? "",
                    WeatherIcon = GetWeatherIcon(weatherData.fcst_day_1.condition_key),
                    Condition = weatherData.fcst_day_1.condition ?? "Non disponible",
                    Temperature = $"{weatherData.fcst_day_1.tmax}°C",
                    TemperatureRange = $"{weatherData.fcst_day_1.tmin}°C / {weatherData.fcst_day_1.tmax}°C"
                });
            }

            // Ajout des prévisions pour le jour 2
            if (weatherData.fcst_day_2 != null)
            {
                forecasts.Add(new ForecastDay
                {
                    DayName = weatherData.fcst_day_2.day_short ?? "N/A",
                    Date = weatherData.fcst_day_2.date ?? "",
                    WeatherIcon = GetWeatherIcon(weatherData.fcst_day_2.condition_key),
                    Condition = weatherData.fcst_day_2.condition ?? "Non disponible",
                    Temperature = $"{weatherData.fcst_day_2.tmax}°C",
                    TemperatureRange = $"{weatherData.fcst_day_2.tmin}°C / {weatherData.fcst_day_2.tmax}°C"
                });
            }

            // Ajout des prévisions pour le jour 3
            if (weatherData.fcst_day_3 != null)
            {
                forecasts.Add(new ForecastDay
                {
                    DayName = weatherData.fcst_day_3.day_short ?? "N/A",
                    Date = weatherData.fcst_day_3.date ?? "",
                    WeatherIcon = GetWeatherIcon(weatherData.fcst_day_3.condition_key),
                    Condition = weatherData.fcst_day_3.condition ?? "Non disponible",
                    Temperature = $"{weatherData.fcst_day_3.tmax}°C",
                    TemperatureRange = $"{weatherData.fcst_day_3.tmin}°C / {weatherData.fcst_day_3.tmax}°C"
                });
            }

            // Ajout des prévisions pour le jour 4
            if (weatherData.fcst_day_4 != null)
            {
                forecasts.Add(new ForecastDay
                {
                    DayName = weatherData.fcst_day_4.day_short ?? "N/A",
                    Date = weatherData.fcst_day_4.date ?? "",
                    WeatherIcon = GetWeatherIcon(weatherData.fcst_day_4.condition_key),
                    Condition = weatherData.fcst_day_4.condition ?? "Non disponible",
                    Temperature = $"{weatherData.fcst_day_4.tmax}°C",
                    TemperatureRange = $"{weatherData.fcst_day_4.tmin}°C / {weatherData.fcst_day_4.tmax}°C"
                });
            }

            // Lie les prévisions à l'interface utilisateur
            _meteoPage.ForecastList.ItemsSource = forecasts;
        }

        /// Retire les accents d'une chaîne de caractères
        private string RemoveAccents(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// Convertit une clé de condition météo en emoji
        private string GetWeatherIcon(string? conditionKey)
        {
            return conditionKey?.ToLower() switch
            {
                "clear" => "☀️",
                "cloudy" => "☁️",
                "partly_cloudy" => "⛅",
                "overcast" => "☁️",
                "lightrain" => "🌦️",
                "light_rain" => "🌦️",
                "rain" => "🌧️",
                "heavy_rain" => "🌧️",
                "storm" => "⛈️",
                "thunder" => "⛈️",
                "snow" => "❄️",
                "light_snow" => "🌨️",
                "heavy_snow" => "❄️",
                "fog" => "🌫️",
                "mist" => "🌫️",
                _ => "☁️"
            };
        }

        /// Récupère les données météo depuis l'API prevision-meteo.ch
        public async Task<Root?> GetWeatherDataAsync()
        {
            try
            {
                City = RemoveAccents(City);
                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync($"https://www.prevision-meteo.ch/services/json/{City}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Root? weatherData = JsonConvert.DeserializeObject<Root>(content);
                    return weatherData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la récupération des données: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        private void SearchCityBtn_Click(object sender, RoutedEventArgs e)
        {
            CitySearchWindow searchWindow = new CitySearchWindow
            {
                Owner = this
            };

            if (searchWindow.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(searchWindow.SelectedCity))
                {
                    City = searchWindow.SelectedCity;
                    UpdateCityDisplay();
                    City = RemoveAccents(City);
                }
            }
        }

        private void BtnMeteoPage_Click(object sender, RoutedEventArgs e)
        {
            // Navigation vers la page météo
            if (_meteoPage == null)
            {
                _meteoPage = new meteopage();
            }
            MainFrame.Navigate(_meteoPage);

            // Mise à jour du style des boutons
            BtnMeteoPage.Style = (Style)FindResource("MenuButtonActiveStyle");
            BtnSettingsPage.Style = (Style)FindResource("MenuButtonStyle");

            // Mise à jour du titre
            PageTitle.Text = "☁️ MÉTÉO";

            // Afficher le bouton de recherche
            SearchCityBtn.Visibility = Visibility.Visible;
        }

        private void BtnSettingsPage_Click(object sender, RoutedEventArgs e)
        {
            // Navigation vers la page paramètres
            MainFrame.Navigate(new settingspage());

            // Mise à jour du style des boutons
            BtnSettingsPage.Style = (Style)FindResource("MenuButtonActiveStyle");
            BtnMeteoPage.Style = (Style)FindResource("MenuButtonStyle");

            // Mise à jour du titre
            PageTitle.Text = "⚙️ PARAMÈTRES";

            // Masquer le bouton de recherche
            SearchCityBtn.Visibility = Visibility.Collapsed;
        }

        public void UpdateDefaultCity(string newCity)
        {
            City = newCity;
            UpdateCityDisplay();
            SetWeatherDataAsync();
        }
    }
}