using System.Collections.Generic;

namespace placesService
{
    public class GoogleResponse
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }

    public class Result
    {
        public Geometry geometry { get; set; }
        public string name { get; set; }
        public string vicinity { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
