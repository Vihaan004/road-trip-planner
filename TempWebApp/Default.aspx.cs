using System;
using System.Web.UI;
// Add the namespace for the service reference
using TempWebApp.DistanceMatrixService;
// Add reference for WebControls
using System.Web.UI.WebControls;

namespace TempWebApp
{
    public partial class _Default : Page 
    {
        protected TextBox txtCity1;
        protected TextBox txtCity2;
        protected Button btnGetDistance;
        protected Label lblDistanceResult;
        protected Button btnGetTime;
        protected Label lblTimeResult;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGetDistance_Click(object sender, EventArgs e)
        {
            // Create an instance of the WCF service client
            using (Service1Client client = new Service1Client())
            {
                try
                {
                    string city1 = txtCity1.Text;
                    string city2 = txtCity2.Text;
                    // Call the service method
                    int distance = client.getDistance(city1, city2);
                    // result
                    lblDistanceResult.Text = $"Distance: {distance} km";
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    lblDistanceResult.Text = "Error: " + ex.Message;
                }
            }
        }

        protected void btnGetTime_Click(object sender, EventArgs e)
        {
            // Create an instance of the WCF service client
            using (Service1Client client = new Service1Client())
            {
                try
                {
                    string city1 = txtCity1.Text;
                    string city2 = txtCity2.Text;
                    // Call the service method
                    string time = client.getTime(city1, city2);
                    // result
                    lblTimeResult.Text = $"Time: {time}";
                }
                catch (Exception ex)
                {
                    lblTimeResult.Text = "Error: " + ex.Message;
                }
            }
        }
    }
}