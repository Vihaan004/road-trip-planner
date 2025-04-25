using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace placesService
{
    public class Service1 : IService1
    {
        private string apiKey = "AIzaSyCC6s6ZAj4xmJ8RyB83kHHN-52mBb2Trxw";

        public PlaceResults SearchNearby(string destination)
        {
            //TLS 1.2 for secure connections
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            (double lat, double lng) = GeocodeAddress(destination);

            List<string> petrolPumps = SearchPlaces(lat, lng, "petrol pump", 3);

            List<string> restaurants = SearchPlaces(lat, lng, "restaurant", 3);

            return new PlaceResults
            {
                PetrolPumps = petrolPumps,
                Restaurants = restaurants
            };
        }

        private (double, double) GeocodeAddress(string address)
        {
            //Encode the address properly for the URL
            string encodedAddress = HttpUtility.UrlEncode(address);

            string geocodeUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(geocodeUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string jsonResponse = "";

            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                jsonResponse = sr.ReadToEnd();
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            GeocodeResponse geoData = js.Deserialize<GeocodeResponse>(jsonResponse);

            if (geoData.status == "OK")
            {
                double lat = geoData.results[0].geometry.location.lat;
                double lng = geoData.results[0].geometry.location.lng;
                return (lat, lng);
            }
            else
            {
                throw new Exception("Unable to geocode the address."); 
            }
        }

        private List<string> SearchPlaces(double lat, double lng, string keyword, int topResults)
        {
            //Encode the keyword properly for the URL
            string encodedKeyword = HttpUtility.UrlEncode(keyword);

            string searchUrl = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={lat},{lng}&radius=1500&keyword={encodedKeyword}&key={apiKey}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(searchUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string jsonResponse = "";

            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                jsonResponse = sr.ReadToEnd();
            }

            JavaScriptSerializer js = new JavaScriptSerializer();
            GoogleResponse placeData = js.Deserialize<GoogleResponse>(jsonResponse);

            List<string> placeNames = new List<string>();

            foreach (var result in placeData.results)
            {
                if (placeNames.Count >= topResults) break;
                placeNames.Add(result.name + " - " + result.vicinity);
            }

            return placeNames;
        }
    }
}
