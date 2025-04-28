using System;

namespace TempWebApp
{
    public partial class ServiceDirectory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No functionality needed
        }

        protected void btnTryIt_Click(object sender, EventArgs e)
        {
            // Navigate to the DistanceMatrix.aspx page
            Response.Redirect("~/DistanceMatrix.aspx");
        }
        
        protected void btnTryPlaces_Click(object sender, EventArgs e)
        {
            // Navigate to the Places.aspx page (renamed from PlacesService.aspx)
            Response.Redirect("~/Places.aspx");
        }
        
        protected void btnTryHashing_Click(object sender, EventArgs e)
        {
            // Navigate to the Hashing.aspx page
            Response.Redirect("~/Hashing.aspx");
        }

        protected void btnTryWeather_Click(object sender, EventArgs e)
        {
            // Navigate to the Weather.aspx page
            Response.Redirect("~/Weather.aspx");
        }
    }
}