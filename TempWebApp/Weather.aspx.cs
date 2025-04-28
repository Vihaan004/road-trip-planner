using System;
using System.Web.UI;

namespace TempWebApp
{
    public partial class Weather : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnGetWeather_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(location1Input.Text) || string.IsNullOrWhiteSpace(location2Input.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    "alert('Please enter both locations.');", true);
                return;
            }

            try
            {
                // Create an instance of the Weather Service client
                WeatherService.WeatherServiceClient weatherService = new WeatherService.WeatherServiceClient();

                // Call the GetGoogleWeather method
                string weatherInfo = weatherService.GetGoogleWeather(location1Input.Text, location2Input.Text);

                // Display the results
                WeatherResults.Text = weatherInfo.Replace("\n", "<br />");
                ResultsPanel.Visible = true;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    $"alert('Error getting weather information: {ex.Message}');", true);
            }
        }

        protected void btnBackToServiceDirectory_Click(object sender, EventArgs e)
        {
            // Redirect back to the Service Directory page
            Response.Redirect("~/ServiceDirectory.aspx");
        }
    }
}