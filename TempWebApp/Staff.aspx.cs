using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TempWebApp
{
    public partial class Staff : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                // If not authenticated, redirect to login page
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Check if the user is a staff member
            HttpCookie userTypeCookie = Request.Cookies["UserType"];
            if (userTypeCookie == null || userTypeCookie.Value != "Staff")
            {
                // If not staff, redirect to appropriate page
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Set the staff name
                lblStaffName.Text = User.Identity.Name;

                // Check for last login time
                HttpCookie lastLoginCookie = Request.Cookies["LastLoginTime"];
                if (lastLoginCookie != null)
                {
                    try
                    {
                        DateTime lastLogin = DateTime.Parse(lastLoginCookie.Value);
                        lblLastLogin.Text = lastLogin.ToString("f"); // Long date and time format
                    }
                    catch
                    {
                        lblLastLogin.Text = "Unknown";
                    }
                }
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            // Sign out the user
            FormsAuthentication.SignOut();
            
            // Clear all authentication cookies
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            authCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(authCookie);
            
            // Clear user type cookie
            HttpCookie userTypeCookie = new HttpCookie("UserType");
            userTypeCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(userTypeCookie);
            
            // Redirect to the default page
            Response.Redirect("~/Default.aspx");
        }

        protected void btnManageUsers_Click(object sender, EventArgs e)
        {
            // For future implementation - User management functionality
            Response.Write("<script>alert('User management feature will be available in a future update.');</script>");
        }

        protected void btnViewReports_Click(object sender, EventArgs e)
        {
            // For future implementation - Reports functionality
            Response.Write("<script>alert('Reports feature will be available in a future update.');</script>");
        }

        protected void btnServiceSettings_Click(object sender, EventArgs e)
        {
            // For future implementation - Service settings functionality
            Response.Write("<script>alert('Service settings feature will be available in a future update.');</script>");
        }
    }
}