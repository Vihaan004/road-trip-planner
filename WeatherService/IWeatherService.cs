using System.ServiceModel;

namespace WeatherService
{
    [ServiceContract]
    public interface IWeatherService
    {
        [OperationContract]
        string GetGoogleWeather(string location1, string location2);
    }
}
