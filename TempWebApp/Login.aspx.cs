using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq; // Added for XML handling
using System.IO; // Added for Path
using System.Text; // For building error messages
using System.Web.Security; // For Forms Authentication
using PasswordHashLibrary; // Added for password hashing

namespace TempWebApp
{
    public partial class Login : System.Web.UI.Page
    {
        // Constants for XML file paths
        private readonly string MEMBER_XML_PATH = "Member.xml";
        private readonly string STAFF_XML_PATH = "Staff.xml";

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

        // Common validation for username and password
        private List<string> ValidateCredentials(string username, string password, string confirmPassword = null)
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(username) || username.Length < 2)
            {
                errors.Add("Username must be at least 2 characters.");
            }
            
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                errors.Add("Password must be at least 8 characters.");
            }
            else if (!PasswordHasher.ValidatePassword(password))
            {
                errors.Add("Password must contain at least one letter and one number.");
            }
            
            if (confirmPassword != null)
            {
                if (string.IsNullOrWhiteSpace(confirmPassword))
                {
                    errors.Add("Please confirm your password.");
                }
                else if (password != confirmPassword)
                {
                    errors.Add("Passwords do not match.");
                }
            }
            return errors;
        }

        // Member Login Button Click Handler
        protected void btnMemberLogin_Click(object sender, EventArgs e)
        {
            ProcessLogin(MEMBER_XML_PATH, "Member", "~/Home.aspx");
        }

        // Staff Login Button Click Handler
        protected void btnStaffLogin_Click(object sender, EventArgs e)
        {
            // Check for the pre-approved TA account
            string username = txtLoginUsername.Text.Trim();
            string password = txtLoginPassword.Text; // Don't trim password

            if (username == "TA" && password == "Cse445!")
            {
                // TA login successful - special case
                SetupAuthenticationCookie(username, "Staff");
                Response.Redirect("~/Staff.aspx");
                return;
            }

            // Normal staff login
            ProcessLogin(STAFF_XML_PATH, "Staff", "~/Staff.aspx");
        }

        // Common login processing method for both member and staff
        private void ProcessLogin(string xmlFileName, string userType, string redirectUrl)
        {
            // Clear status messages
            lblSignUpStatus.Text = "";
            lblLoginStatus.Text = "";
            lblLoginStatus.CssClass = "status-message";

            string username = txtLoginUsername.Text.Trim();
            string password = txtLoginPassword.Text; // Don't trim password

            // Validate input
            List<string> errors = ValidateCredentials(username, password);
            if (errors.Any())
            {
                lblLoginStatus.CssClass = "status-message status-error";
                lblLoginStatus.Text = string.Join("<br/>", errors);
                return;
            }

            bool isAuthenticated = false;
            string xmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), xmlFileName);

            if (File.Exists(xmlFilePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(xmlFilePath);
                    string rootElementName = userType == "Member" ? "Members" : "StaffMembers";
                    
                    // Changed to case-sensitive comparison
                    var user = doc.Root?.Elements(userType)
                                   .FirstOrDefault(m => m.Element("Username")?.Value.Equals(username, StringComparison.Ordinal) ?? false);

                    if (user != null)
                    {
                        string storedPasswordHash = user.Element("Password")?.Value;
                        
                        // Hash the provided password and compare with stored hash
                        string inputPasswordHash = PasswordHasher.HashPassword(password);
                        if (inputPasswordHash == storedPasswordHash) 
                        {
                            isAuthenticated = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblLoginStatus.CssClass = "status-message status-error";
                    lblLoginStatus.Text = "An error occurred during login.";
                    System.Diagnostics.Trace.WriteLine($"{userType} Login Error: " + ex.ToString());
                    return;
                }
            }
            else
            {
                lblLoginStatus.CssClass = "status-message status-error";
                lblLoginStatus.Text = $"{userType} data file not found.";
                return;
            }

            if (isAuthenticated)
            {
                // Set up cookies and authentication
                SetupAuthenticationCookie(username, userType);

                // Handle "Remember Me" functionality
                HandleRememberMe(username, password);

                // Redirect to appropriate page
                Response.Redirect(redirectUrl);
            }
            else
            {
                lblLoginStatus.CssClass = "status-message status-error";
                lblLoginStatus.Text = "Invalid username or password.";
            }
        }

        private void SetupAuthenticationCookie(string username, string userType)
        {
            // Set authentication cookie
            FormsAuthentication.SetAuthCookie(username, false);

            // Set user type cookie
            HttpCookie userTypeCookie = new HttpCookie("UserType");
            userTypeCookie.Value = userType;
            userTypeCookie.Expires = DateTime.Now.AddMinutes(30); // Session cookie
            Response.Cookies.Add(userTypeCookie);

            // Set the last login time cookie
            DateTime currentLoginTime = DateTime.Now;
            HttpCookie lastLoginCookie = new HttpCookie("LastLoginTime");
            lastLoginCookie.Value = currentLoginTime.ToString("o"); // ISO 8601 format
            lastLoginCookie.Expires = DateTime.Now.AddDays(30); // Expires in 30 days
            Response.Cookies.Add(lastLoginCookie);
        }

        private void HandleRememberMe(string username, string password)
        {
            // If Remember Me is checked, create a cookie to auto-fill the login form next time
            if (chkRememberMe.Checked)
            {
                HttpCookie rememberCookie = new HttpCookie("RememberMe");
                rememberCookie.Values["Username"] = username;
                rememberCookie.Values["Password"] = password; // Store the actual password
                rememberCookie.Expires = DateTime.Now.AddDays(30); // 30 day expiration
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
        }

        // Member Sign Up Button Click Handler
        protected void btnMemberSignUp_Click(object sender, EventArgs e)
        {
            ProcessMemberSignUp();
        }

        // Process member sign up
        private void ProcessMemberSignUp()
        {
            // Clear status messages
            lblLoginStatus.Text = "";
            lblSignUpStatus.Text = "";
            lblSignUpStatus.CssClass = "status-message";

            string username = txtSignUpUsername.Text.Trim();
            string password = txtSignUpPassword.Text; // Don't trim password
            string confirmPassword = txtConfirmPassword.Text; // Don't trim password

            // Validate input
            List<string> errors = ValidateCredentials(username, password, confirmPassword);
            if (errors.Any())
            {
                lblSignUpStatus.CssClass = "status-message status-error";
                lblSignUpStatus.Text = string.Join("<br/>", errors);
                return;
            }

            try
            {
                string memberXmlPath = Path.Combine(Server.MapPath("~/App_Data"), MEMBER_XML_PATH);
                string staffXmlPath = Path.Combine(Server.MapPath("~/App_Data"), STAFF_XML_PATH);
                string appDataPath = Server.MapPath("~/App_Data");

                // Check if username already exists in either Member.xml or Staff.xml
                bool userExists = false;

                // Ensure App_Data directory exists
                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }

                // Check Member.xml
                if (File.Exists(memberXmlPath))
                {
                    try
                    {
                        XDocument memberDoc = XDocument.Load(memberXmlPath);
                        userExists = memberDoc.Root?.Elements("Member")
                            .Any(m => m.Element("Username")?.Value.Equals(username, StringComparison.OrdinalIgnoreCase) ?? false) ?? false;
                    }
                    catch { /* Ignore errors loading Member.xml */ }
                }

                // If not found in Member.xml, check Staff.xml
                if (!userExists && File.Exists(staffXmlPath))
                {
                    try
                    {
                        XDocument staffDoc = XDocument.Load(staffXmlPath);
                        userExists = staffDoc.Root?.Elements("Staff")
                            .Any(s => s.Element("Username")?.Value.Equals(username, StringComparison.OrdinalIgnoreCase) ?? false) ?? false;
                    }
                    catch { /* Ignore errors loading Staff.xml */ }
                }

                if (userExists)
                {
                    lblSignUpStatus.CssClass = "status-message status-error";
                    lblSignUpStatus.Text = "Username already exists. Please choose another.";
                    return;
                }

                // Load or create the target XML file
                string xmlFilePath = Path.Combine(appDataPath, MEMBER_XML_PATH);
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
                            doc = new XDocument(new XElement("Members"));
                        }
                    }
                    catch
                    {
                        doc = new XDocument(new XElement("Members"));
                    }
                }

                // Hash the password using PasswordHashLibrary
                string hashedPassword = PasswordHasher.HashPassword(password);

                // Add the new user to the XML file
                XElement newUser = new XElement("Member",
                    new XElement("Username", username),
                    new XElement("Password", hashedPassword)
                );
                doc.Root.Add(newUser);
                doc.Save(xmlFilePath);

                lblSignUpStatus.CssClass = "status-message status-success";
                lblSignUpStatus.Text = "Member sign up successful! You can now log in.";
                
                // Clear sign up fields
                txtSignUpUsername.Text = "";
                txtSignUpPassword.Text = "";
                txtConfirmPassword.Text = "";
            }
            catch (Exception ex)
            {
                lblSignUpStatus.CssClass = "status-message status-error";
                lblSignUpStatus.Text = "An error occurred during sign up.";
                System.Diagnostics.Trace.WriteLine("Member Sign Up Error: " + ex.ToString());
            }
        }
    }
}