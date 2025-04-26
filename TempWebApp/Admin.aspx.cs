using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.IO;
using System.Data;
using PasswordHashLibrary; // Added for password hashing

namespace TempWebApp
{
    public partial class Admin : System.Web.UI.Page
    {
        // XML file paths
        private readonly string STAFF_XML_PATH = "Staff.xml";
        private readonly string POSTS_XML_PATH = "Posts.xml";
        private readonly string COMMENTS_XML_PATH = "Comments.xml";
        
        // XML root elements
        private readonly string XML_ROOT_ELEMENT = "StaffMembers";
        private readonly string POSTS_ROOT_ELEMENT = "Posts";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load staff members into the GridView
                LoadStaffMembers();
                
                // Load forum posts into the GridView
                LoadForumPosts();
            }
        }

        #region Staff Management Methods

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

            // Password validation using PasswordHashLibrary
            if (password.Length < 8 || !PasswordHasher.ValidatePassword(password))
            {
                ShowMessage("Password must be at least 8 characters long and contain both letters and numbers", false);
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
        /// <param name="password">The password (will be hashed before storing)</param>
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

                // Hash the password using PasswordHashLibrary
                string hashedPassword = PasswordHasher.HashPassword(password);

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

        #endregion

        #region Forum Posts Management Methods

        /// <summary>
        /// Loads forum posts from the XML file into the GridView
        /// </summary>
        private void LoadForumPosts()
        {
            try
            {
                DataTable postsTable = GetForumPostsDataTable();
                gvPosts.DataSource = postsTable;
                gvPosts.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading forum posts: " + ex.Message, false);
            }
        }

        /// <summary>
        /// Creates a DataTable from the Posts.xml file
        /// </summary>
        /// <returns>DataTable containing forum posts information</returns>
        private DataTable GetForumPostsDataTable()
        {
            DataTable postsTable = new DataTable();
            postsTable.Columns.Add("PostId", typeof(int));
            postsTable.Columns.Add("Subject", typeof(string));
            postsTable.Columns.Add("Description", typeof(string));
            postsTable.Columns.Add("PostType", typeof(string));
            postsTable.Columns.Add("UserName", typeof(string));
            postsTable.Columns.Add("PostDate", typeof(DateTime));

            string xmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), POSTS_XML_PATH);
            
            if (!File.Exists(xmlFilePath))
            {
                return postsTable; // Return empty table if file doesn't exist
            }

            try
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                var posts = doc.Root?.Elements("Post");

                if (posts != null)
                {
                    foreach (var post in posts)
                    {
                        int postId = int.Parse(post.Element("PostId").Value);
                        string subject = post.Element("Subject")?.Value ?? string.Empty;
                        string description = post.Element("Description")?.Value ?? string.Empty;
                        string postType = post.Element("PostType")?.Value ?? string.Empty;
                        string userName = post.Element("UserName")?.Value ?? string.Empty;
                        DateTime postDate = DateTime.Parse(post.Element("PostDate").Value);
                        
                        postsTable.Rows.Add(postId, subject, description, postType, userName, postDate);
                    }
                }
            }
            catch
            {
                // If there's an error loading the XML file, return the empty table
            }

            return postsTable;
        }

        /// <summary>
        /// Handles the RowCommand event for the posts GridView
        /// </summary>
        protected void gvPosts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int postId = Convert.ToInt32(e.CommandArgument);
                if (DeleteForumPost(postId))
                {
                    ShowMessage("Forum post deleted successfully", true);
                    LoadForumPosts();
                }
                else
                {
                    ShowMessage("Error deleting forum post", false);
                }
            }
        }

        /// <summary>
        /// Handles the RowDeleting event for the posts GridView
        /// </summary>
        protected void gvPosts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // This method is required for the Delete command to work
            // The actual deletion is handled in gvPosts_RowCommand
        }

        /// <summary>
        /// Deletes a forum post from the XML file and any associated comments
        /// </summary>
        /// <param name="postId">The ID of the post to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        private bool DeleteForumPost(int postId)
        {
            try
            {
                // Delete post from Posts.xml
                string postsXmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), POSTS_XML_PATH);
                
                if (!File.Exists(postsXmlFilePath))
                {
                    ShowMessage("Posts file not found", false);
                    return false;
                }

                XDocument postsDoc = XDocument.Load(postsXmlFilePath);
                var post = postsDoc.Root?.Elements("Post")
                    .FirstOrDefault(p => int.Parse(p.Element("PostId").Value) == postId);
                
                if (post != null)
                {
                    post.Remove();
                    postsDoc.Save(postsXmlFilePath);
                    
                    // Also delete any associated comments
                    DeleteCommentsForPost(postId);
                    
                    return true;
                }
                else
                {
                    ShowMessage("Post not found", false);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting post: " + ex.Message, false);
                return false;
            }
        }

        /// <summary>
        /// Deletes all comments associated with a post
        /// </summary>
        /// <param name="postId">The ID of the post</param>
        private void DeleteCommentsForPost(int postId)
        {
            try
            {
                string commentsXmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), COMMENTS_XML_PATH);
                
                if (!File.Exists(commentsXmlFilePath))
                {
                    return;
                }

                XDocument commentsDoc = XDocument.Load(commentsXmlFilePath);
                var comments = commentsDoc.Root?.Elements("Comment")
                    .Where(c => int.Parse(c.Element("PostId").Value) == postId)
                    .ToList();
                
                if (comments != null && comments.Any())
                {
                    foreach (var comment in comments)
                    {
                        comment.Remove();
                    }
                    
                    commentsDoc.Save(commentsXmlFilePath);
                }
            }
            catch
            {
                // Ignore errors when deleting comments
            }
        }

        #endregion
    }
}