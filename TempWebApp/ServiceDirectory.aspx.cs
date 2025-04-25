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
    }
}