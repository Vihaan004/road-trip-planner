using System.Collections.Generic;

namespace placesService
{
    public class GeocodeResponse
    {
        public List<GeoResult> results { get; set; }
        public string status { get; set; }
    }

    public class GeoResult
    {
        public Geometry geometry { get; set; }
    }
}
