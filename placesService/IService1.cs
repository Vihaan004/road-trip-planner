using System.ServiceModel;
using System.ServiceModel.Web;

namespace placesService
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebGet(UriTemplate = "SearchNearby?destination={destination}", ResponseFormat = WebMessageFormat.Json)]
        PlaceResults SearchNearby(string destination);
    }
}
