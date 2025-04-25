using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TempWebApp
{
    public partial class DistanceMatrix : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            TempWebApp.DistanceMatrixService.Service1Client distanceService = new TempWebApp.DistanceMatrixService.Service1Client();
            string city1 = city1input.Text;
            string city2 = city2input.Text;

            double distance = Math.Round(distanceService.getDistance(city1, city2) * 0.621371, 2);
            string time = distanceService.getTime(city1, city2);

            LabelDistance.Text = distance + " miles";
            LabelTime.Text = time;
        }

        protected void btnBackToServiceDirectory_Click(object sender, EventArgs e)
        {
            // Redirect back to the Service Directory page
            Response.Redirect("~/ServiceDirectory.aspx");
        }
    }
}