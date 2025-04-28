using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml.Linq;

namespace TempWebApp
{
    public partial class Staff : System.Web.UI.Page
    {
        // Session key constants
        private const string SESSION_POSTS = "ForumPosts";
        private const string SESSION_COMMENTS = "Comments";
        private const string SESSION_USER_TYPE = "UserType";
        private const string SESSION_USERNAME = "Username";
        
        // XML file paths
        private const string POSTS_XML_PATH = "Posts.xml";
        private const string COMMENTS_XML_PATH = "Comments.xml";

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

                // Store the current username in session for future use
                Session[SESSION_USERNAME] = User.Identity.Name;
                Session[SESSION_USER_TYPE] = "Staff";

                // Load all forum posts
                LoadForumPosts();
            }
        }

        #region Forum Methods

        private void LoadForumPosts()
        {
            var posts = GetForumPosts();
            string selectedPostType = ddlPostTypeFilter.SelectedValue;

            // Apply filter if selected
            if (selectedPostType != "All")
            {
                var filteredPosts = posts.AsEnumerable()
                    .Where(r => r.Field<string>("PostType") == selectedPostType)
                    .OrderByDescending(r => r.Field<DateTime>("PostDate")); // Sort by date descending
                    
                if (filteredPosts.Any())
                {
                    var filteredTable = filteredPosts.CopyToDataTable();
                    rptAllPosts.DataSource = filteredTable;
                    pnlNoPosts.Visible = filteredTable.Rows.Count == 0;
                }
                else
                {
                    rptAllPosts.DataSource = null;
                    rptAllPosts.DataBind();
                    pnlNoPosts.Visible = true;
                }
            }
            else
            {
                // Sort all posts by date descending
                    var sortedPostsQuery = posts.AsEnumerable()
                    .OrderByDescending(r => r.Field<DateTime>("PostDate"));
                
                if (sortedPostsQuery.Any())
                {
                    rptAllPosts.DataSource = sortedPostsQuery.CopyToDataTable();
                }
                else
                {
                    rptAllPosts.DataSource = null;
                }
                
                pnlNoPosts.Visible = posts.Rows.Count == 0;
            }

            rptAllPosts.DataBind();
        }

        protected void ddlPostTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadForumPosts();
        }

        protected void rptAllPosts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = e.Item.DataItem as DataRowView;
                if (row != null)
                {
                    int postId = Convert.ToInt32(row["PostId"]);
                    Repeater rptComments = (Repeater)e.Item.FindControl("rptComments");

                    // Get comments for this post
                    var comments = GetComments();
                    
                    // Filter comments for this post
                    var filteredComments = comments.AsEnumerable()
                        .Where(r => Convert.ToInt32(r["PostId"]) == postId)
                        .OrderBy(r => r.Field<DateTime>("CommentDate"));
                    
                    if (filteredComments.Any())
                    {
                        var postComments = filteredComments.CopyToDataTable();
                        rptComments.DataSource = postComments;
                        rptComments.DataBind();

                        Panel pnlComments = (Panel)e.Item.FindControl("pnlComments");
                        pnlComments.Visible = true;
                    }
                    else
                    {
                        Panel pnlComments = (Panel)e.Item.FindControl("pnlComments");
                        pnlComments.Visible = false;
                    }
                }
            }
        }

        protected void btnAddStaffComment_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;
            TextBox txtStaffComment = (TextBox)item.FindControl("txtStaffComment");
            int postId = Convert.ToInt32(btn.CommandArgument);

            string commentText = txtStaffComment.Text.Trim();
            if (string.IsNullOrEmpty(commentText))
            {
                return;
            }

            try
            {
                // Get current username from session
                string username = Session[SESSION_USERNAME]?.ToString() ?? "Staff";
                
                // Add comment to XML
                AddCommentToXml(postId, commentText, username);
                
                // Clear comment text box
                txtStaffComment.Text = string.Empty;
                
                // Reload forum posts
                LoadForumPosts();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    $"alert('Error adding comment: {ex.Message}');", true);
            }
        }

        private void AddCommentToXml(int postId, string commentText, string username)
        {
            // Get existing comments
            var comments = GetComments();
            
            // Find the next comment ID
            int newId = 1;
            if (comments.Rows.Count > 0)
            {
                newId = comments.AsEnumerable().Max(r => Convert.ToInt32(r["CommentId"])) + 1;
            }
            
            // Add new comment to the DataTable
            DataRow newComment = comments.NewRow();
            newComment["CommentId"] = newId;
            newComment["PostId"] = postId;
            newComment["CommentText"] = commentText;
            newComment["UserName"] = username;
            newComment["CommentDate"] = DateTime.Now;
            newComment["IsStaff"] = true; // Always true for staff comments from staff page
            
            comments.Rows.Add(newComment);
            
            // Update session
            Session[SESSION_COMMENTS] = comments;
            
            // Save to XML
            SaveCommentsToXml(comments);
        }

        private DataTable GetForumPosts()
        {
            // Try to get from session first for performance
            if (Session[SESSION_POSTS] != null)
            {
                return Session[SESSION_POSTS] as DataTable;
            }
            
            // Load from XML
            var postsTable = CreateEmptyPostsTable();
            string xmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), POSTS_XML_PATH);
            
            if (File.Exists(xmlFilePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(xmlFilePath);
                    if (doc.Root != null)
                    {
                        foreach (var postElement in doc.Root.Elements("Post"))
                        {
                            DataRow newRow = postsTable.NewRow();
                            newRow["PostId"] = int.Parse(postElement.Element("PostId").Value);
                            newRow["Subject"] = postElement.Element("Subject").Value;
                            newRow["Description"] = postElement.Element("Description").Value;
                            newRow["PostType"] = postElement.Element("PostType").Value;
                            newRow["UserName"] = postElement.Element("UserName").Value;
                            newRow["PostDate"] = DateTime.Parse(postElement.Element("PostDate").Value);
                            
                            postsTable.Rows.Add(newRow);
                        }
                    }
                }
                catch
                {
                    // If there's an error loading, create sample posts
                    postsTable = CreateSamplePosts();
                }
            }
            else
            {
                // If file doesn't exist, create sample posts and save them
                postsTable = CreateSamplePosts();
                SavePostsToXml(postsTable);
            }
            
            // Store in session for faster access next time
            Session[SESSION_POSTS] = postsTable;
            
            return postsTable;
        }

        private DataTable GetComments()
        {
            // Try to get from session first for performance
            if (Session[SESSION_COMMENTS] != null)
            {
                return Session[SESSION_COMMENTS] as DataTable;
            }
            
            // Load from XML
            var commentsTable = CreateEmptyCommentsTable();
            string xmlFilePath = Path.Combine(Server.MapPath("~/App_Data"), COMMENTS_XML_PATH);
            
            if (File.Exists(xmlFilePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(xmlFilePath);
                    if (doc.Root != null)
                    {
                        foreach (var commentElement in doc.Root.Elements("Comment"))
                        {
                            DataRow newRow = commentsTable.NewRow();
                            newRow["CommentId"] = int.Parse(commentElement.Element("CommentId").Value);
                            newRow["PostId"] = int.Parse(commentElement.Element("PostId").Value);
                            newRow["CommentText"] = commentElement.Element("CommentText").Value;
                            newRow["UserName"] = commentElement.Element("UserName").Value;
                            newRow["CommentDate"] = DateTime.Parse(commentElement.Element("CommentDate").Value);
                            newRow["IsStaff"] = bool.Parse(commentElement.Element("IsStaff").Value);
                            
                            commentsTable.Rows.Add(newRow);
                        }
                    }
                }
                catch
                {
                    // If there's an error loading, create sample comments
                    commentsTable = CreateSampleComments();
                }
            }
            else
            {
                // If file doesn't exist, create sample comments and save them
                commentsTable = CreateSampleComments();
                SaveCommentsToXml(commentsTable);
            }
            
            // Store in session for faster access next time
            Session[SESSION_COMMENTS] = commentsTable;
            
            return commentsTable;
        }
        
        private void SavePostsToXml(DataTable postsTable)
        {
            // Create App_Data directory if it doesn't exist
            string appDataPath = Server.MapPath("~/App_Data");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            
            string xmlFilePath = Path.Combine(appDataPath, POSTS_XML_PATH);
            
            // Create XML document
            XDocument doc = new XDocument(new XElement("Posts"));
            
            // Add each post to the XML
            foreach (DataRow row in postsTable.Rows)
            {
                XElement postElement = new XElement("Post",
                    new XElement("PostId", row["PostId"]),
                    new XElement("Subject", row["Subject"]),
                    new XElement("Description", row["Description"]),
                    new XElement("PostType", row["PostType"]),
                    new XElement("UserName", row["UserName"]),
                    new XElement("PostDate", ((DateTime)row["PostDate"]).ToString("o")) // ISO 8601 format
                );
                
                doc.Root.Add(postElement);
            }
            
            // Save the XML file
            doc.Save(xmlFilePath);
        }

        private void SaveCommentsToXml(DataTable commentsTable)
        {
            // Create App_Data directory if it doesn't exist
            string appDataPath = Server.MapPath("~/App_Data");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            
            string xmlFilePath = Path.Combine(appDataPath, COMMENTS_XML_PATH);
            
            // Create XML document
            XDocument doc = new XDocument(new XElement("Comments"));
            
            // Add each comment to the XML
            foreach (DataRow row in commentsTable.Rows)
            {
                XElement commentElement = new XElement("Comment",
                    new XElement("CommentId", row["CommentId"]),
                    new XElement("PostId", row["PostId"]),
                    new XElement("CommentText", row["CommentText"]),
                    new XElement("UserName", row["UserName"]),
                    new XElement("CommentDate", ((DateTime)row["CommentDate"]).ToString("o")), // ISO 8601 format
                    new XElement("IsStaff", row["IsStaff"])
                );
                
                doc.Root.Add(commentElement);
            }
            
            // Save the XML file
            doc.Save(xmlFilePath);
        }

        private DataTable CreateEmptyPostsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("PostId", typeof(int));
            table.Columns.Add("Subject", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("PostType", typeof(string));
            table.Columns.Add("UserName", typeof(string));
            table.Columns.Add("PostDate", typeof(DateTime));
            return table;
        }

        private DataTable CreateEmptyCommentsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CommentId", typeof(int));
            table.Columns.Add("PostId", typeof(int));
            table.Columns.Add("CommentText", typeof(string));
            table.Columns.Add("UserName", typeof(string));
            table.Columns.Add("CommentDate", typeof(DateTime));
            table.Columns.Add("IsStaff", typeof(bool));
            return table;
        }

        private DataTable CreateSamplePosts()
        {
            DataTable table = CreateEmptyPostsTable();

            // Add some sample posts
            table.Rows.Add(1, "Best route from LA to San Francisco?", 
                "I'm planning a trip from LA to SF. Any recommended routes or stops along the way?", 
                "Question", "RoadTripper123", DateTime.Now.AddDays(-5));
            
            table.Rows.Add(2, "Great restaurant in San Diego", 
                "Found an amazing seafood restaurant in San Diego called Oceanview. Highly recommend!", 
                "Tip", "TravelFoodie", DateTime.Now.AddDays(-3));
            
            table.Rows.Add(3, "Review of Coast Highway drive", 
                "Just completed the Coast Highway drive. The views were breathtaking but traffic was heavy.", 
                "Review", "CoastalDriver", DateTime.Now.AddDays(-1));
            
            table.Rows.Add(4, "Hidden gems in Arizona?", 
                "Planning a trip through Arizona. Any hidden gems or underrated spots I should visit?", 
                "Question", "DesertExplorer", DateTime.Now.AddDays(-7));
            
            table.Rows.Add(5, "Budget-friendly accommodations", 
                "Looking for budget-friendly accommodations along Route 66. Any suggestions?", 
                "Question", "BudgetTraveler", DateTime.Now.AddDays(-2));
            
            table.Rows.Add(6, "Great hiking spot near Denver", 
                "Just discovered an amazing hiking trail 30 minutes from Denver. Beautiful views and not too crowded.", 
                "Tip", "MountainHiker", DateTime.Now.AddDays(-10));
            
            table.Rows.Add(7, "Road trip apps recommendation", 
                "What are your favorite road trip planning apps? Looking for something that works offline too.", 
                "Question", "TechTraveler", DateTime.Now.AddDays(-4));

            return table;
        }

        private DataTable CreateSampleComments()
        {
            DataTable table = CreateEmptyCommentsTable();

            // Add some sample comments
            table.Rows.Add(1, 1, "I recommend taking Highway 1 for the coastal views. It takes longer but worth it!", 
                "CoastalExpert", DateTime.Now.AddDays(-4), false);
            
            table.Rows.Add(2, 1, "Make sure to stop at Big Sur. There are great hiking trails there.", 
                "TripAdvisor", DateTime.Now.AddDays(-3), true);
            
            table.Rows.Add(3, 3, "Thanks for sharing your experience. We've been noticing increased traffic on weekends.", 
                "RoadTripSupport", DateTime.Now.AddDays(-1), true);
            
            table.Rows.Add(4, 4, "Check out Sedona! Beautiful red rocks and great hiking. Less crowded than Grand Canyon.", 
                "ArizonaFan", DateTime.Now.AddDays(-6), false);
            
            table.Rows.Add(5, 5, "I recommend checking out hostels or using apps like HotelTonight for last-minute deals.", 
                "BudgetStaffer", DateTime.Now.AddDays(-1), true);
            
            table.Rows.Add(6, 7, "I recommend GasBuddy for finding cheap gas and Roadtrippers for planning your route.", 
                "AppDeveloper", DateTime.Now.AddDays(-3), false);

            return table;
        }

        #endregion

        #region Button Event Handlers

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

        #endregion
    }
}