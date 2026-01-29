using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace APIMETEO.Vincent.Malitine.model
{
    /// <summary>
    /// Modèle pour l'affichage des prévisions météo
    /// </summary>
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

    public class ForecastInfo
    {
        public object? latitude { get; set; }
        public object? longitude { get; set; }
        public string? elevation { get; set; }
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

    public class HourlyData
    {
        [JsonProperty("0H00")] public HourData? _0H00 { get; set; }
        [JsonProperty("1H00")] public HourData? _1H00 { get; set; }
        [JsonProperty("2H00")] public HourData? _2H00 { get; set; }
        [JsonProperty("3H00")] public HourData? _3H00 { get; set; }
        [JsonProperty("4H00")] public HourData? _4H00 { get; set; }
        [JsonProperty("5H00")] public HourData? _5H00 { get; set; }
        [JsonProperty("6H00")] public HourData? _6H00 { get; set; }
        [JsonProperty("7H00")] public HourData? _7H00 { get; set; }
        [JsonProperty("8H00")] public HourData? _8H00 { get; set; }
        [JsonProperty("9H00")] public HourData? _9H00 { get; set; }
        [JsonProperty("10H00")] public HourData? _10H00 { get; set; }
        [JsonProperty("11H00")] public HourData? _11H00 { get; set; }
        [JsonProperty("12H00")] public HourData? _12H00 { get; set; }
        [JsonProperty("13H00")] public HourData? _13H00 { get; set; }
        [JsonProperty("14H00")] public HourData? _14H00 { get; set; }
        [JsonProperty("15H00")] public HourData? _15H00 { get; set; }
        [JsonProperty("16H00")] public HourData? _16H00 { get; set; }
        [JsonProperty("17H00")] public HourData? _17H00 { get; set; }
        [JsonProperty("18H00")] public HourData? _18H00 { get; set; }
        [JsonProperty("19H00")] public HourData? _19H00 { get; set; }
        [JsonProperty("20H00")] public HourData? _20H00 { get; set; }
        [JsonProperty("21H00")] public HourData? _21H00 { get; set; }
        [JsonProperty("22H00")] public HourData? _22H00 { get; set; }
        [JsonProperty("23H00")] public HourData? _23H00 { get; set; }
    }

    public class HourData
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

    #endregion

    #region Gestion des thèmes

    public static class ThemeManager
    {
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

        public static void ApplyTheme(string themeName)
        {
            if (!Themes.ContainsKey(themeName))
                return;

            var theme = Themes[themeName];
            var app = Application.Current;

            app.Resources["BackgroundColor"] = ConvertColor(theme.Background);
            app.Resources["CardBackgroundColor"] = ConvertColor(theme.CardBackground);
            app.Resources["SecondaryBackgroundColor"] = ConvertColor(theme.SecondaryBackground);
            app.Resources["AccentColor"] = ConvertColor(theme.AccentColor);
            app.Resources["AccentHoverColor"] = ConvertColor(theme.AccentHover);
            app.Resources["TextPrimaryColor"] = ConvertColor(theme.TextPrimary);
            app.Resources["TextSecondaryColor"] = ConvertColor(theme.TextSecondary);

            APIMETEO.Vincent.Malitine.Properties.Settings.Default.SelectedTheme = themeName;
            APIMETEO.Vincent.Malitine.Properties.Settings.Default.Save();
        }

        public static void LoadSavedTheme()
        {
            string savedTheme = APIMETEO.Vincent.Malitine.Properties.Settings.Default.SelectedTheme;
            if (string.IsNullOrEmpty(savedTheme))
                savedTheme = "GitHub Dark";

            ApplyTheme(savedTheme);
        }

        private static SolidColorBrush ConvertColor(string hex)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }
    }

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

    #endregion
}
