using APIMETEO.Vincent.Malitine.views;
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
                    // Normalise le nom de la ville en retirant les accents
                    City = RemoveAccents(searchWindow.SelectedCity);
                    UpdateCityDisplay();
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

    /// Modèle pour l'affichage des prévisions météo
    public class ForecastDay
    {
        public required string DayName { get; set; }
        public required string Date { get; set; }
        public required string WeatherIcon { get; set; }
        public required string Condition { get; set; }
        public required string Temperature { get; set; }
        public required string TemperatureRange { get; set; }
    }

    #region Modèles de données de l'API prevision-meteo.ch

    // Classes pour les données météo horaires (0H00 à 23H00)
    public class _0H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _1H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _2H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _3H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _4H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _5H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _6H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _7H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _8H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _9H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _10H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _11H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _12H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _13H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _14H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _15H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _16H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _17H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _18H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _19H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _20H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _21H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _22H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class _23H00
    {
        public string? ICON { get; set; }
        public string? CONDITION { get; set; }
        public string? CONDITION_KEY { get; set; }
        public double TMP2m { get; set; }
        public double DPT2m { get; set; }
        public double? WNDCHILL2m { get; set; }
        public double? HUMIDEX { get; set; }
        public int RH2m { get; set; }
        public double PRMSL { get; set; }
        public double APCPsfc { get; set; }
        public int WNDSPD10m { get; set; }
        public int WNDGUST10m { get; set; }
        public int WNDDIR10m { get; set; }
        public string? WNDDIRCARD10 { get; set; }
        public int ISSNOW { get; set; }
        public string? HCDC { get; set; }
        public string? MCDC { get; set; }
        public string? LCDC { get; set; }
        public int HGT0C { get; set; }
        public int KINDEX { get; set; }
        public string? CAPE180_0 { get; set; }
        public int CIN180_0 { get; set; }
    }

    public class CityInfo
    {
        public string? name { get; set; }
        public string? country { get; set; }
        public string? latitude { get; set; }
        public string? longitude { get; set; }
        public string? elevation { get; set; }
        public string? sunrise { get; set; }
        public string? sunset { get; set; }
    }

    public class CurrentCondition
    {
        public string? date { get; set; }
        public string? hour { get; set; }
        public int tmp { get; set; }
        public int wnd_spd { get; set; }
        public int wnd_gust { get; set; }
        public string? wnd_dir { get; set; }
        public double pressure { get; set; }
        public int humidity { get; set; }
        public string? condition { get; set; }
        public string? condition_key { get; set; }
        public string? icon { get; set; }
        public string? icon_big { get; set; }
    }

    public class FcstDay0
    {
        public string? date { get; set; }
        public string? day_short { get; set; }
        public string? day_long { get; set; }
        public int tmin { get; set; }
        public int tmax { get; set; }
        public string? condition { get; set; }
        public string? condition_key { get; set; }
        public string? icon { get; set; }
        public string? icon_big { get; set; }
        public HourlyData? hourly_data { get; set; }
    }

    public class FcstDay1
    {
        public string? date { get; set; }
        public string? day_short { get; set; }
        public string? day_long { get; set; }
        public int tmin { get; set; }
        public int tmax { get; set; }
        public string? condition { get; set; }
        public string? condition_key { get; set; }
        public string? icon { get; set; }
        public string? icon_big { get; set; }
        public HourlyData? hourly_data { get; set; }
    }

    public class FcstDay2
    {
        public string? date { get; set; }
        public string? day_short { get; set; }
        public string? day_long { get; set; }
        public int tmin { get; set; }
        public int tmax { get; set; }
        public string? condition { get; set; }
        public string? condition_key { get; set; }
        public string? icon { get; set; }
        public string? icon_big { get; set; }
        public HourlyData? hourly_data { get; set; }
    }

    public class FcstDay3
    {
        public string? date { get; set; }
        public string? day_short { get; set; }
        public string? day_long { get; set; }
        public int tmin { get; set; }
        public int tmax { get; set; }
        public string? condition { get; set; }
        public string? condition_key { get; set; }
        public string? icon { get; set; }
        public string? icon_big { get; set; }
        public HourlyData? hourly_data { get; set; }
    }

    public class FcstDay4
    {
        public string? date { get; set; }
        public string? day_short { get; set; }
        public string? day_long { get; set; }
        public int tmin { get; set; }
        public int tmax { get; set; }
        public string? condition { get; set; }
        public string? condition_key { get; set; }
        public string? icon { get; set; }
        public string? icon_big { get; set; }
        public HourlyData? hourly_data { get; set; }
    }

    public class ForecastInfo
    {
        public object? latitude { get; set; }
        public object? longitude { get; set; }
        public string? elevation { get; set; }
    }

    public class HourlyData
    {
        [JsonProperty("0H00")] public _0H00? _0H00 { get; set; }
        [JsonProperty("1H00")] public _1H00? _1H00 { get; set; }
        [JsonProperty("2H00")] public _2H00? _2H00 { get; set; }
        [JsonProperty("3H00")] public _3H00? _3H00 { get; set; }
        [JsonProperty("4H00")] public _4H00? _4H00 { get; set; }
        [JsonProperty("5H00")] public _5H00? _5H00 { get; set; }
        [JsonProperty("6H00")] public _6H00? _6H00 { get; set; }
        [JsonProperty("7H00")] public _7H00? _7H00 { get; set; }
        [JsonProperty("8H00")] public _8H00? _8H00 { get; set; }
        [JsonProperty("9H00")] public _9H00? _9H00 { get; set; }
        [JsonProperty("10H00")] public _10H00? _10H00 { get; set; }
        [JsonProperty("11H00")] public _11H00? _11H00 { get; set; }
        [JsonProperty("12H00")] public _12H00? _12H00 { get; set; }
        [JsonProperty("13H00")] public _13H00? _13H00 { get; set; }
        [JsonProperty("14H00")] public _14H00? _14H00 { get; set; }
        [JsonProperty("15H00")] public _15H00? _15H00 { get; set; }
        [JsonProperty("16H00")] public _16H00? _16H00 { get; set; }
        [JsonProperty("17H00")] public _17H00? _17H00 { get; set; }
        [JsonProperty("18H00")] public _18H00? _18H00 { get; set; }
        [JsonProperty("19H00")] public _19H00? _19H00 { get; set; }
        [JsonProperty("20H00")] public _20H00? _20H00 { get; set; }
        [JsonProperty("21H00")] public _21H00? _21H00 { get; set; }
        [JsonProperty("22H00")] public _22H00? _22H00 { get; set; }
        [JsonProperty("23H00")] public _23H00? _23H00 { get; set; }
    }

    public class Root
    {
        public CityInfo? city_info { get; set; }
        public ForecastInfo? forecast_info { get; set; }
        public CurrentCondition? current_condition { get; set; }
        public FcstDay0? fcst_day_0 { get; set; }
        public FcstDay1? fcst_day_1 { get; set; }
        public FcstDay2? fcst_day_2 { get; set; }
        public FcstDay3? fcst_day_3 { get; set; }
        public FcstDay4? fcst_day_4 { get; set; }
    }

    #endregion

    public static class ThemeManager
    {
        // Définition des 5 thèmes prédéfinis
        public static readonly Dictionary<string, ThemeColors> Themes = new()
        {
            {
                "GitHub Dark", new ThemeColors
                {
                    Background = "#0f1419",
                    CardBackground = "#1a1f2e",
                    SecondaryBackground = "#161b22",
                    AccentColor = "#58a6ff",
                    AccentHover = "#79c0ff",
                    TextPrimary = "#ffffff",
                    TextSecondary = "#8b949e"
                }
            },
            {
                "Ocean Blue", new ThemeColors
                {
                    Background = "#0a1929",
                    CardBackground = "#1e3a5f",
                    SecondaryBackground = "#132f4c",
                    AccentColor = "#3399ff",
                    AccentHover = "#66b2ff",
                    TextPrimary = "#ffffff",
                    TextSecondary = "#b0bec5"
                }
            },
            {
                "Forest Green", new ThemeColors
                {
                    Background = "#0d1f0d",
                    CardBackground = "#1a2e1a",
                    SecondaryBackground = "#162316",
                    AccentColor = "#4caf50",
                    AccentHover = "#66bb6a",
                    TextPrimary = "#ffffff",
                    TextSecondary = "#a5d6a7"
                }
            },
            {
                "Sunset Orange", new ThemeColors
                {
                    Background = "#1f0f0a",
                    CardBackground = "#2e1e1a",
                    SecondaryBackground = "#231612",
                    AccentColor = "#ff7043",
                    AccentHover = "#ff8a65",
                    TextPrimary = "#ffffff",
                    TextSecondary = "#ffccbc"
                }
            },
            {
                "Purple Night", new ThemeColors
                {
                    Background = "#1a0f29",
                    CardBackground = "#2a1e3e",
                    SecondaryBackground = "#1f1630",
                    AccentColor = "#9c27b0",
                    AccentHover = "#ba68c8",
                    TextPrimary = "#ffffff",
                    TextSecondary = "#ce93d8"
                }
            }
        };

        // Applique un thème à l'application
        public static void ApplyTheme(string themeName)
        {
            if (!Themes.ContainsKey(themeName))
                return;

            var theme = Themes[themeName];
            var app = Application.Current;

            // Met à jour les ressources de l'application
            app.Resources["BackgroundColor"] = ConvertColor(theme.Background);
            app.Resources["CardBackgroundColor"] = ConvertColor(theme.CardBackground);
            app.Resources["SecondaryBackgroundColor"] = ConvertColor(theme.SecondaryBackground);
            app.Resources["AccentColor"] = ConvertColor(theme.AccentColor);
            app.Resources["AccentHoverColor"] = ConvertColor(theme.AccentHover);
            app.Resources["TextPrimaryColor"] = ConvertColor(theme.TextPrimary);
            app.Resources["TextSecondaryColor"] = ConvertColor(theme.TextSecondary);

            // Sauvegarde le thème sélectionné
            Properties.Settings.Default.SelectedTheme = themeName;
            Properties.Settings.Default.Save();
        }

        // Charge le thème sauvegardé
        public static void LoadSavedTheme()
        {
            string savedTheme = Properties.Settings.Default.SelectedTheme;
            if (string.IsNullOrEmpty(savedTheme))
                savedTheme = "GitHub Dark";

            ApplyTheme(savedTheme);
        }

        // Convertit une couleur hexadécimale en SolidColorBrush
        private static SolidColorBrush ConvertColor(string hex)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }
    }

    // Classe pour définir les couleurs d'un thème
    public class ThemeColors
    {
        public required string Background { get; set; }
        public required string CardBackground { get; set; }
        public required string SecondaryBackground { get; set; }
        public required string AccentColor { get; set; }
        public required string AccentHover { get; set; }
        public required string TextPrimary { get; set; }
        public required string TextSecondary { get; set; }
    }
}