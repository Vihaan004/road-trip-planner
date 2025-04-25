using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq; // Added for XML handling
using System.IO; // Added for Path
using System.Text; // For building error messages

namespace TempWebApp
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clear status messages on initial load
            if (!IsPostBack)
            {
                lblLoginStatus.Text = "";
                lblSignUpStatus.Text = "";
                
                // Check for RememberMe cookie and populate fields if it exists
                if (Request.Cookies["RememberMe"] != null)
                {
                    HttpCookie rememberCookie = Request.Cookies["RememberMe"];
                    txtLoginUsername.Text = rememberCookie["Username"] ?? "";
                    
                    // If password was saved, retrieve and use it (instead of just showing placeholder)
                    if (!string.IsNullOrEmpty(rememberCookie["Password"]))
                    {
                        // Set the actual password value from cookie
                        txtLoginPassword.Attributes["value"] = rememberCookie["Password"];
                        // Also set the "Remember Me" checkbox to checked state
                        chkRememberMe.Checked = true;
                    }
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Clear sign up status when login is attempted
            lblSignUpStatus.Text = "";
            lblLoginStatus.Text = ""; // Clear previous login status
            lblLoginStatus.CssClass = "status-message"; // Reset CSS

            string username = txtLoginUsername.Text.Trim();
            string password = txtLoginPassword.Text; // Don't trim password

            // --- Manual Validation ---
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(username) || username.Length < 2)
            {
                errors.Add("Username must be at least 2 characters.");
            }
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                errors.Add("Password must be at least 8 characters.");
            }

            if (errors.Any())
            {
                lblLoginStatus.CssClass = "status-message status-error";
                lblLoginStatus.Text = string.Join("<br/>", errors);
                return; // Stop processing if validation fails
            }
            // --- End Manual Validation ---


            bool isAuthenticated = false;
            string xmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), "Member.xml");

            if (File.Exists(xmlFilePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(xmlFilePath);
                    // Changed to case-sensitive comparison
                    var user = doc.Root?.Elements("Member")
                                   .FirstOrDefault(m => m.Element("Username")?.Value.Equals(username, StringComparison.Ordinal) ?? false);

                    if (user != null)
                    {
                        string storedPasswordHash = user.Element("Password")?.Value;

                        // *** IMPORTANT: Replace this with your actual password hashing check ***
                        // Example: if (YourHashingLibrary.VerifyPassword(password, storedPasswordHash))
                        // For now, doing a simple placeholder comparison (REMOVE THIS IN PRODUCTION)
                        if (password == storedPasswordHash) // Replace this line
                        {
                            isAuthenticated = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (optional)
                    lblLoginStatus.CssClass = "status-message status-error";
                    lblLoginStatus.Text = "An error occurred during login.";
                    System.Diagnostics.Trace.WriteLine("Login Error: " + ex.ToString());
                    return; // Exit the method early
                }
            }
            else
            {
                lblLoginStatus.CssClass = "status-message status-error";
                lblLoginStatus.Text = "User data file not found.";
                return; // Exit if the data file doesn't exist
            }


            if (isAuthenticated)
            {
                // Set the last login time cookie
                DateTime currentLoginTime = DateTime.Now;
                HttpCookie lastLoginCookie = new HttpCookie("LastLoginTime");
                lastLoginCookie.Value = currentLoginTime.ToString("o"); // ISO 8601 format
                lastLoginCookie.Expires = DateTime.Now.AddDays(30); // Expires in 30 days
                // Consider adding these for security if using HTTPS:
                // lastLoginCookie.HttpOnly = true;
                // lastLoginCookie.Secure = true;
                Response.Cookies.Add(lastLoginCookie);

                // If Remember Me is checked, create a cookie to auto-fill the login form next time
                if (chkRememberMe.Checked)
                {
                    HttpCookie rememberCookie = new HttpCookie("RememberMe");
                    rememberCookie.Values["Username"] = username;
                    rememberCookie.Values["Password"] = password; // Store the actual password
                    rememberCookie.Expires = DateTime.Now.AddDays(30); // 30 day expiration
                    // Add security attributes if using HTTPS
                    // rememberCookie.Secure = true;
                    Response.Cookies.Add(rememberCookie);
                }
                else
                {
                    // Clear any existing RememberMe cookie if the checkbox is unchecked
                    if (Request.Cookies["RememberMe"] != null)
                    {
                        HttpCookie rememberCookie = new HttpCookie("RememberMe");
                        rememberCookie.Expires = DateTime.Now.AddDays(-1); // Set to expired
                        Response.Cookies.Add(rememberCookie); // Overwrite with expired cookie
                    }
                }

                // Redirect to Home page after successful login
                Response.Redirect("~/Home.aspx");
            }
            else
            {
                lblLoginStatus.CssClass = "status-message status-error";
                lblLoginStatus.Text = "Invalid username or password.";
            }
        }

        // Added Sign Up Button Click Handler
        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            // Clear login status when sign up is attempted
            lblLoginStatus.Text = "";
            lblSignUpStatus.Text = ""; // Clear previous sign up status
            lblSignUpStatus.CssClass = "status-message"; // Reset CSS

            string username = txtSignUpUsername.Text.Trim();
            string password = txtSignUpPassword.Text; // Don't trim password
            string confirmPassword = txtConfirmPassword.Text; // Don't trim password

            // --- Manual Validation ---
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(username) || username.Length < 2)
            {
                errors.Add("Username must be at least 2 characters.");
            }
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                errors.Add("Password must be at least 8 characters.");
            }
            if (string.IsNullOrWhiteSpace(confirmPassword)) // Check if confirm password is empty separately
            {
                 errors.Add("Please confirm your password.");
            }
            else if (password != confirmPassword)
            {
                errors.Add("Passwords do not match.");
            }
            // Note: We don't need a separate length check for confirmPassword if it must match the first password which already has a length check.
            // However, if the first password fails length validation, the mismatch error might be confusing.
            // Adding a separate check for confirm password length might be clearer for the user.
            // else if (confirmPassword.Length < 8) // Optional: Add if you want a specific message for confirm password length
            // {
            //     errors.Add("Confirm Password must be at least 8 characters.");
            // }


            if (errors.Any())
            {
                lblSignUpStatus.CssClass = "status-message status-error";
                lblSignUpStatus.Text = string.Join("<br/>", errors);
                return; // Stop processing if validation fails
            }
            // --- End Manual Validation ---


            // Proceed with sign up logic only if validation passes
            try
            {
                string xmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), "Member.xml");
                string appDataPath = Server.MapPath("~/App_Data");

                // Ensure App_Data directory exists
                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }

                XDocument doc;
                if (!File.Exists(xmlFilePath))
                {
                    doc = new XDocument(new XElement("Members"));
                }
                else
                {
                    try
                    {
                        doc = XDocument.Load(xmlFilePath);
                        if (doc.Root == null || doc.Root.Name != "Members")
                        {
                            doc = new XDocument(new XElement("Members")); // Fix/Overwrite if needed
                        }
                    }
                    catch
                    { // Handle potential XML load errors (e.g., empty file)
                        doc = new XDocument(new XElement("Members"));
                    }
                }

                // Check if username already exists (case-sensitive)
                bool userExists = doc.Root.Elements("Member")
                                     .Any(m => m.Element("Username")?.Value.Equals(username, StringComparison.Ordinal) ?? false);

                if (userExists)
                {
                    lblSignUpStatus.CssClass = "status-message status-error";
                    lblSignUpStatus.Text = "Username already exists. Please choose another or log in.";
                    return;
                }

                // *** IMPORTANT: Implement password hashing here! ***
                string hashedPassword = password; // Placeholder - REPLACE THIS

                XElement newUser = new XElement("Member",
                    new XElement("Username", username),
                    new XElement("Password", hashedPassword)
                );
                doc.Root.Add(newUser);
                doc.Save(xmlFilePath);

                lblSignUpStatus.CssClass = "status-message status-success";
                lblSignUpStatus.Text = "Sign up successful! You can now log in.";
                // Optionally clear sign up fields
                txtSignUpUsername.Text = "";
                txtSignUpPassword.Text = "";
                txtConfirmPassword.Text = "";
            }
            catch (Exception ex)
            {
                lblSignUpStatus.CssClass = "status-message status-error";
                lblSignUpStatus.Text = "An error occurred during sign up.";
                System.Diagnostics.Trace.WriteLine("Sign Up Error: " + ex.ToString());
            }
        }
    }
}