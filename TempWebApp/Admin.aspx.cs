using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.IO;
using System.Data;

namespace TempWebApp
{
    public partial class Admin : System.Web.UI.Page
    {
        // XML file path
        private readonly string STAFF_XML_PATH = "Staff.xml";
        private readonly string XML_ROOT_ELEMENT = "StaffMembers";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load staff members into the GridView
                LoadStaffMembers();
            }
        }

        /// <summary>
        /// Loads staff members from the XML file into the GridView
        /// </summary>
        private void LoadStaffMembers()
        {
            try
            {
                DataTable staffTable = GetStaffDataTable();
                gvStaffMembers.DataSource = staffTable;
                gvStaffMembers.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading staff members: " + ex.Message, false);
            }
        }

        /// <summary>
        /// Creates a DataTable from the Staff.xml file
        /// </summary>
        /// <returns>DataTable containing staff member information</returns>
        private DataTable GetStaffDataTable()
        {
            DataTable staffTable = new DataTable();
            staffTable.Columns.Add("Username", typeof(string));

            string xmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), STAFF_XML_PATH);
            
            // Check if the file exists, if not, create an empty XML structure
            if (!File.Exists(xmlFilePath))
            {
                EnsureStaffXmlExists();
            }

            try
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                var staffMembers = doc.Root?.Elements("Staff");

                if (staffMembers != null)
                {
                    foreach (var staff in staffMembers)
                    {
                        string username = staff.Element("Username")?.Value ?? string.Empty;
                        staffTable.Rows.Add(username);
                    }
                }
            }
            catch
            {
                // If there's an error loading the XML file, return the empty table
            }

            return staffTable;
        }

        /// <summary>
        /// Ensures that the Staff.xml file exists with proper structure
        /// </summary>
        private void EnsureStaffXmlExists()
        {
            string appDataPath = Server.MapPath("~/App_Data");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            string xmlFilePath = Path.Combine(appDataPath, STAFF_XML_PATH);
            if (!File.Exists(xmlFilePath))
            {
                XDocument doc = new XDocument(new XElement(XML_ROOT_ELEMENT));
                doc.Save(xmlFilePath);
            }
        }

        /// <summary>
        /// Handles adding a new staff member
        /// </summary>
        protected void btnAddStaff_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // Additional validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowMessage("Username and password are required", false);
                return;
            }

            // Check if the username already exists in Staff.xml
            if (StaffUsernameExists(username))
            {
                ShowMessage("Username already exists. Please choose another.", false);
                return;
            }

            // Check if the username already exists in Member.xml
            if (MemberUsernameExists(username))
            {
                ShowMessage("Username already exists as a member. Staff and member usernames must be unique.", false);
                return;
            }

            // Add the new staff member
            if (AddStaffMember(username, password))
            {
                // Clear form fields
                txtUsername.Text = string.Empty;
                txtPassword.Text = string.Empty;
                txtConfirmPassword.Text = string.Empty;

                // Show success message
                ShowMessage("Staff member added successfully", true);

                // Refresh the GridView
                LoadStaffMembers();
            }
        }

        /// <summary>
        /// Adds a new staff member to the XML file
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password (should be hashed in production)</param>
        /// <returns>True if successful, false otherwise</returns>
        private bool AddStaffMember(string username, string password)
        {
            try
            {
                string xmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), STAFF_XML_PATH);
                
                // Ensure XML file exists
                EnsureStaffXmlExists();

                // Load XML document
                XDocument doc = XDocument.Load(xmlFilePath);

                // TODO: Hash the password using a secure method (like PasswordHashLibrary)
                string hashedPassword = password; // Replace with actual hashing

                // Create new staff element
                XElement newStaff = new XElement("Staff",
                    new XElement("Username", username),
                    new XElement("Password", hashedPassword)
                );

                // Add to document
                doc.Root.Add(newStaff);
                
                // Save changes
                doc.Save(xmlFilePath);
                
                return true;
            }
            catch (Exception ex)
            {
                ShowMessage("Error adding staff member: " + ex.Message, false);
                return false;
            }
        }

        /// <summary>
        /// Checks if a username exists in the Staff.xml file
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if exists, false otherwise</returns>
        private bool StaffUsernameExists(string username)
        {
            try
            {
                string xmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), STAFF_XML_PATH);
                
                if (!File.Exists(xmlFilePath))
                {
                    return false;
                }

                XDocument doc = XDocument.Load(xmlFilePath);
                var staffMember = doc.Root?.Elements("Staff")
                    .FirstOrDefault(s => s.Element("Username")?.Value.Equals(username, StringComparison.OrdinalIgnoreCase) ?? false);
                
                return staffMember != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a username exists in the Member.xml file
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if exists, false otherwise</returns>
        private bool MemberUsernameExists(string username)
        {
            try
            {
                string xmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), "Member.xml");
                
                if (!File.Exists(xmlFilePath))
                {
                    return false;
                }

                XDocument doc = XDocument.Load(xmlFilePath);
                var member = doc.Root?.Elements("Member")
                    .FirstOrDefault(m => m.Element("Username")?.Value.Equals(username, StringComparison.OrdinalIgnoreCase) ?? false);
                
                return member != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Handles row command events for the GridView
        /// </summary>
        protected void gvStaffMembers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                string username = e.CommandArgument.ToString();
                
                // Prevent deletion of the TA account
                if (username.Equals("TA", StringComparison.OrdinalIgnoreCase))
                {
                    ShowMessage("The TA account cannot be deleted.", false);
                    return;
                }

                // Delete the staff member
                if (DeleteStaffMember(username))
                {
                    ShowMessage("Staff member deleted successfully", true);
                    LoadStaffMembers();
                }
            }
        }

        /// <summary>
        /// Handles row deleting events for the GridView
        /// </summary>
        protected void gvStaffMembers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Event is handled by RowCommand
            e.Cancel = true;
        }

        /// <summary>
        /// Deletes a staff member from the XML file
        /// </summary>
        /// <param name="username">The username of the staff member to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        private bool DeleteStaffMember(string username)
        {
            try
            {
                string xmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), STAFF_XML_PATH);
                
                if (!File.Exists(xmlFilePath))
                {
                    ShowMessage("Staff file not found", false);
                    return false;
                }

                XDocument doc = XDocument.Load(xmlFilePath);
                var staffMember = doc.Root?.Elements("Staff")
                    .FirstOrDefault(s => s.Element("Username")?.Value.Equals(username, StringComparison.OrdinalIgnoreCase) ?? false);
                
                if (staffMember != null)
                {
                    staffMember.Remove();
                    doc.Save(xmlFilePath);
                    return true;
                }
                else
                {
                    ShowMessage("Staff member not found", false);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting staff member: " + ex.Message, false);
                return false;
            }
        }

        /// <summary>
        /// Shows a message to the user
        /// </summary>
        /// <param name="message">The message text</param>
        /// <param name="isSuccess">True for success message, false for error message</param>
        private void ShowMessage(string message, bool isSuccess)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = isSuccess ? "message success" : "message error";
            lblMessage.Text = message;
        }
    }
}