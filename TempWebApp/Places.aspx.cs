using System;
using System.Web.UI;
// Add the namespace for the service reference
using TempWebApp.PlacesService;
using System.Web.UI.WebControls;

namespace TempWebApp
{
    public partial class Places : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No initialization needed
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(locationInput.Text))
            {
                // Display a simple error message if the location is empty
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", 
                    "alert('Please enter a location.');", true);
                return;
            }

            // Create an instance of the Places Service client
            TempWebApp.PlacesService.Service1Client placesService = new TempWebApp.PlacesService.Service1Client();

            try
            {
                // Call the SearchNearby method with the location specified by the user
                TempWebApp.PlacesService.PlaceResults results = placesService.SearchNearby(locationInput.Text);

                // Clear any existing items in the lists
                PetrolPumpsList.Items.Clear();
                RestaurantsList.Items.Clear();

                // Add petrol pumps to the list
                if (results.PetrolPumps != null && results.PetrolPumps.Length > 0)
                {
                    foreach (string pump in results.PetrolPumps)
                    {
                        PetrolPumpsList.Items.Add(pump);
                    }
                }
                else
                {
                    PetrolPumpsList.Items.Add("No petrol pumps found nearby.");
                }

                // Add restaurants to the list
                if (results.Restaurants != null && results.Restaurants.Length > 0)
                {
                    foreach (string restaurant in results.Restaurants)
                    {
                        RestaurantsList.Items.Add(restaurant);
                    }
                }
                else
                {
                    RestaurantsList.Items.Add("No restaurants found nearby.");
                }

                // Make the results panel visible
                ResultsPanel.Visible = true;
            }
            catch (Exception ex)
            {
                // Display error message to user
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", 
                    $"alert('Error finding places: {ex.Message}');", true);
            }
        }

        protected void btnBackToServiceDirectory_Click(object sender, EventArgs e)
        {
            // Redirect back to the Service Directory page
            Response.Redirect("~/ServiceDirectory.aspx");
        }
    }
}