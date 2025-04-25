using System;
using System.Net.Http;
using DistanceMatrixService;
using Newtonsoft.Json;

namespace DistanceMatrixService
{
    public class Service1 : IService1
    {
        private static readonly string API_KEY = "AIzaSyCC6s6ZAj4xmJ8RyB83kHHN-52mBb2Trxw"; // Replace with actual API key
        private static readonly string API_URL = "https://maps.googleapis.com/maps/api/distancematrix/json";

        public int getDistance(string city1, string city2)
        {
            try
            {
                string url = $"{API_URL}?origins={Uri.EscapeDataString(city1)}&destinations={Uri.EscapeDataString(city2)}&key={API_KEY}";

                using (HttpClient client = new HttpClient())
                {
                    var response = client.GetStringAsync(url).Result;
                    var jsonResponse = JsonConvert.DeserializeObject<GoogleApiResponse>(response);

                    if (jsonResponse?.rows.Length > 0 &&
                        jsonResponse.rows[0].elements.Length > 0)
                    {
                        var element = jsonResponse.rows[0].elements[0];

                        if (element.status == "OK")
                        {
                            return element.distance.value / 1000; // Convert meters to km
                        }
                        else if (element.status == "ZERO_RESULTS")
                        {
                            return 0; // No route available
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return 0; // Default to 0 in case of failure
        }

        public string getTime(string city1, string city2)
        {
            try
            {
                string url = $"{API_URL}?origins={Uri.EscapeDataString(city1)}&destinations={Uri.EscapeDataString(city2)}&key={API_KEY}";

                using (HttpClient client = new HttpClient())
                {
                    var response = client.GetStringAsync(url).Result;
                    var jsonResponse = JsonConvert.DeserializeObject<GoogleApiResponse>(response);

                    if (jsonResponse?.rows.Length > 0 &&
                        jsonResponse.rows[0].elements.Length > 0)
                    {
                        var element = jsonResponse.rows[0].elements[0];

                        if (element.status == "OK")
                        {
                            return element.duration.text;
                        }
                        else if (element.status == "ZERO_RESULTS")
                        {
                            return "No route found"; // No route between cities
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return "Unavailable"; // Default error message
        }
    }
}