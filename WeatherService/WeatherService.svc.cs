using System;
using System.Net;
using Newtonsoft.Json;

namespace WeatherService
{
    public class WeatherService : IWeatherService
    {
        private const string googleApiKey = "AIzaSyCC6s6ZAj4xmJ8RyB83kHHN-52mBb2Trxw";
        private const string openWeatherApiKey = "cdbe24ac4c030ffb606ae58943dbee6f";

        public string GetGoogleWeather(string location1, string location2)
        {
            string weather1 = GetWeatherData(location1);
            string weather2 = GetWeatherData(location2);
            return $"{weather1}\n\n{weather2}";
        }

        public string GetWeatherData(string location)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    // Step 1: Geocode to get lat/lon
                    string geoUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(location)}&key={googleApiKey}";
                    string geoResponse = client.DownloadString(geoUrl);
                    dynamic geoJson = JsonConvert.DeserializeObject(geoResponse);

                    double lat = geoJson.results[0].geometry.location.lat;
                    double lng = geoJson.results[0].geometry.location.lng;

                    // Step 2: Call OpenWeatherMap API
                    string weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lng}&units=metric&appid={openWeatherApiKey}";
                    string weatherResponse = client.DownloadString(weatherUrl);
                    dynamic weatherJson = JsonConvert.DeserializeObject(weatherResponse);

                    string condition = weatherJson.weather[0].main;
                    double temp = weatherJson.main.temp;
                    int humidity = weatherJson.main.humidity;

                    return $"Weather for {location}:\nCondition: {condition}, Temp: {temp}°C, Humidity: {humidity}%";
                }
            }
            catch (Exception ex)
            {
                return $"Error retrieving weather for {location}: {ex.Message}";
            }
        }
    }
}
