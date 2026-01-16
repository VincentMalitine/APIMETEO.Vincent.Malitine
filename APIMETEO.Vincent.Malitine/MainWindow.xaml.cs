using Newtonsoft.Json;
using System.Net.Http;
using System.Windows;

namespace APIMETEO.Vincent.Malitine
{
    public partial class MainWindow : Window
    {
        public string City = "Paris";

        public MainWindow()
        {
            InitializeComponent();
            UpdateCityDisplay();
            SetWeatherDataAsync();
        }

        private async void SetWeatherDataAsync()
        {
            while (true)
            {
                await Task.Delay(1000);
                var main = await GetWeatherDataAsync();
                if (main != null)
                {
                    Temp.Text = $"{main.temp:F1}°C";
                    TempHigh.Text = $"{main.temp_max:F1}°C";
                    TempLow.Text = $"{main.temp_min:F1}°C";
                    Precip.Text = $"{main.humidity}%";
                    TempMid.Text = $"{main.feels_like:F1}°C";
                }
            }
        }

        public async Task<Main?> GetWeatherDataAsync()
        {
            try
            {
                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(
                    $"https://api.openweathermap.org/data/2.5/weather?q={City},fr&appid=c21a75b667d6f7abb81f118dcf8d4611&units=metric");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Root? rot = JsonConvert.DeserializeObject<Root>(content);
                    return rot?.main;
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
            // Ouvrir la fenêtre de recherche
            CitySearchWindow searchWindow = new CitySearchWindow
            {
                Owner = this
            };

            // Afficher la fenêtre en mode dialog
            if (searchWindow.ShowDialog() == true)
            {
                // Si l'utilisateur a validé, mettre à jour la ville
                if (!string.IsNullOrEmpty(searchWindow.SelectedCity))
                {
                    City = searchWindow.SelectedCity;
                    UpdateCityDisplay();
                }
            }
        }

        private void UpdateCityDisplay()
        {
            CityNameDisplay.Text = $"{City}, France";
        }
    }

    // Classes de désérialisation JSON
    public class Clouds
    {
        public int all { get; set; }
    }

    public class Coord
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
        public double feels_like { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public int sea_level { get; set; }
        public int grnd_level { get; set; }
    }

    public class Root
    {
        public Coord? coord { get; set; }
        public List<Weather>? weather { get; set; }
        public string? @base { get; set; }
        public Main? main { get; set; }
        public int visibility { get; set; }
        public Wind? wind { get; set; }
        public Clouds? clouds { get; set; }
        public int dt { get; set; }
        public Sys? sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string? name { get; set; }
        public int cod { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string? country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string? main { get; set; }
        public string? description { get; set; }
        public string? icon { get; set; }
    }

    public class Wind
    {
        public double speed { get; set; }
        public int deg { get; set; }
    }
}