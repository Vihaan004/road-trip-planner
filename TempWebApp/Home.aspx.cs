using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.IO;
using System.Xml.Linq;
using TempWebApp.WeatherService;

namespace TempWebApp
{
    public partial class Home : System.Web.UI.Page
    {
        // Session key constants
        private const string SESSION_ROUTE_STOPS = "RouteStops";
        private const string SESSION_ROUTE_DETAILS = "RouteDetails";
        private const string SESSION_POSTS = "ForumPosts";
        private const string SESSION_COMMENTS = "Comments";
        private const string SESSION_USER_TYPE = "UserType";
        private const string SESSION_USERNAME = "Username";
        
        // XML file paths
        private const string POSTS_XML_PATH = "Posts.xml";
        private const string COMMENTS_XML_PATH = "Comments.xml";

        // Maximum number of stops allowed
        private const int MAX_STOPS = 10;

        // Weather service client
        private WeatherService.WeatherServiceClient _weatherServiceClient;
        protected WeatherService.WeatherServiceClient weatherServiceClient 
        {
            get
            {
                if (_weatherServiceClient == null)
                {
                    _weatherServiceClient = new WeatherService.WeatherServiceClient();
                }
                return _weatherServiceClient;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if the user is logged in
                if (Session[SESSION_USERNAME] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                // Display the user's name
                lblUserName.Text = Session[SESSION_USERNAME].ToString();

                // Initialize route if needed
                if (Session[SESSION_ROUTE_STOPS] == null)
                {
                    Session[SESSION_ROUTE_STOPS] = new List<string>();
                    Session[SESSION_ROUTE_DETAILS] = new Dictionary<string, RouteDetail>();
                }

                // Load forum posts and comments from XML
                // Clear the session values first to force reloading from XML
                Session.Remove(SESSION_POSTS);
                Session.Remove(SESSION_COMMENTS);
                
                // Display the route if it exists
                DisplayRoute();

                // Load and display forum posts
                DisplayPosts();
            }
        }

        #region Route Planning Methods

        protected void btnCreateRoute_Click(object sender, EventArgs e)
        {
            string origin = txtOrigin.Text.Trim();
            string destination = txtDestination.Text.Trim();

            if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(destination))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                    "alert('Please enter both origin and destination.');", true);
                return;
            }

            // Clear existing route data
            var stops = new List<string> { origin, destination };
            Session[SESSION_ROUTE_STOPS] = stops;
            Session[SESSION_ROUTE_DETAILS] = new Dictionary<string, RouteDetail>();

            try
            {
                // Calculate route details between origin and destination
                CalculateRouteDetail(origin, destination);
                DisplayRoute();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                    $"alert('Error creating route: {ex.Message}');", true);
            }
        }

        protected void btnAddStop_Click(object sender, EventArgs e)
        {
            var stops = GetRouteStops();
            if (stops.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                    "alert('Please create a route first by entering origin and destination.');", true);
                return;
            }

            if (stops.Count >= MAX_STOPS + 2) // +2 for origin and destination
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                    $"alert('Maximum {MAX_STOPS} intermediate stops allowed.');", true);
                return;
            }

            string newStop = txtNewStop.Text.Trim();
            if (string.IsNullOrEmpty(newStop))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                    "alert('Please enter a stop name.');", true);
                return;
            }

            // Insert the new stop before the destination
            stops.Insert(stops.Count - 1, newStop);
            Session[SESSION_ROUTE_STOPS] = stops;

            try
            {
                // Recalculate route details
                RecalculateEntireRoute();
                DisplayRoute();
                
                // Clear the text box
                txtNewStop.Text = string.Empty;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                    $"alert('Error adding stop: {ex.Message}');", true);
            }
        }

        protected void btnReorderStops_Click(object sender, EventArgs e)
        {
            var stops = GetRouteStops();
            if (stops.Count <= 2) return;  // No intermediate stops to reorder

            string eventArgument = Request.Params["__EVENTARGUMENT"];
            if (string.IsNullOrEmpty(eventArgument)) return;

            string[] indices = eventArgument.Split(',');
            if (indices.Length != 2) return;

            if (int.TryParse(indices[0], out int sourceIndex) && int.TryParse(indices[1], out int targetIndex))
            {
                // Adjust indices to account for the prepended origin
                sourceIndex += 1;  // Skip the origin (which is at index 0)
                targetIndex += 1;  // Skip the origin (which is at index 0)

                // Ensure we don't move the origin or destination
                if (sourceIndex <= 0 || sourceIndex >= stops.Count - 1 || 
                    targetIndex <= 0 || targetIndex >= stops.Count - 1)
                {
                    return;
                }

                // Move the stop
                string stopToMove = stops[sourceIndex];
                stops.RemoveAt(sourceIndex);
                stops.Insert(targetIndex, stopToMove);

                Session[SESSION_ROUTE_STOPS] = stops;

                try
                {
                    // Recalculate route details
                    RecalculateEntireRoute();
                    DisplayRoute();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                        $"alert('Error reordering stops: {ex.Message}');", true);
                }
            }
        }

        protected void GetPlacesForStop(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int stopIndex = Convert.ToInt32(btn.CommandArgument);
            var stops = GetRouteStops();

            if (stopIndex < 0 || stopIndex >= stops.Count)
            {
                return;
            }

            string stopCity = stops[stopIndex];
            
            try
            {
                // Call the Places Service
                TempWebApp.PlacesService.Service1Client placesService = new TempWebApp.PlacesService.Service1Client();
                TempWebApp.PlacesService.PlaceResults results = placesService.SearchNearby(stopCity);

                // Get the places results container
                HtmlGenericControl resultsDiv = (HtmlGenericControl)FindControl($"placesResults_{stopIndex}");
                if (resultsDiv != null)
                {
                    resultsDiv.Controls.Clear();

                    // Create restaurants section
                    HtmlGenericControl restaurantsSection = new HtmlGenericControl("div");
                    restaurantsSection.Attributes["class"] = "mb-3";
                    HtmlGenericControl restaurantsTitle = new HtmlGenericControl("h6");
                    restaurantsTitle.InnerText = "Restaurants";
                    restaurantsSection.Controls.Add(restaurantsTitle);

                    HtmlGenericControl restaurantsList = new HtmlGenericControl("ul");
                    if (results.Restaurants != null && results.Restaurants.Length > 0)
                    {
                        foreach (string restaurant in results.Restaurants)
                        {
                            HtmlGenericControl item = new HtmlGenericControl("li");
                            item.InnerText = restaurant;
                            restaurantsList.Controls.Add(item);
                        }
                    }
                    else
                    {
                        HtmlGenericControl item = new HtmlGenericControl("li");
                        item.InnerText = "No restaurants found.";
                        restaurantsList.Controls.Add(item);
                    }
                    restaurantsSection.Controls.Add(restaurantsList);
                    resultsDiv.Controls.Add(restaurantsSection);

                    // Create petrol pumps section
                    HtmlGenericControl petrolSection = new HtmlGenericControl("div");
                    petrolSection.Attributes["class"] = "mb-3";
                    HtmlGenericControl petrolTitle = new HtmlGenericControl("h6");
                    petrolTitle.InnerText = "Gas Stations";
                    petrolSection.Controls.Add(petrolTitle);

                    HtmlGenericControl petrolList = new HtmlGenericControl("ul");
                    if (results.PetrolPumps != null && results.PetrolPumps.Length > 0)
                    {
                        foreach (string pump in results.PetrolPumps)
                        {
                            HtmlGenericControl item = new HtmlGenericControl("li");
                            item.InnerText = pump;
                            petrolList.Controls.Add(item);
                        }
                    }
                    else
                    {
                        HtmlGenericControl item = new HtmlGenericControl("li");
                        item.InnerText = "No gas stations found.";
                        petrolList.Controls.Add(item);
                    }
                    petrolSection.Controls.Add(petrolList);
                    resultsDiv.Controls.Add(petrolSection);

                    // Make the results visible
                    resultsDiv.Style["display"] = "block";
                }

                // JavaScript to toggle visibility after AJAX postback
                ScriptManager.RegisterStartupScript(this, GetType(), $"showPlaces_{stopIndex}",
                    $"document.getElementById('placesResults_{stopIndex}').style.display = 'block';", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    $"alert('Error loading places for {stopCity}: {ex.Message}');", true);
            }
        }

        protected void RemoveStop(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int stopIndex = Convert.ToInt32(btn.CommandArgument);
                var stops = GetRouteStops();

                // Make sure we don't remove origin or destination
                if (stopIndex <= 0 || stopIndex >= stops.Count - 1)
                {
                    return;
                }

                // Store stops for later use
                string previousStop = stops[stopIndex - 1];
                string currentStop = stops[stopIndex];
                string nextStop = stops[stopIndex + 1];

                // Remove the stop
                stops.RemoveAt(stopIndex);
                Session[SESSION_ROUTE_STOPS] = stops;

                // Get route details dictionary and make a working copy to avoid modifying during iteration
                var routeDetails = GetRouteDetails();
                var updatedRouteDetails = new Dictionary<string, RouteDetail>(routeDetails);

                // Remove entries involving the removed stop
                string key1 = $"{previousStop}-{currentStop}";
                string key2 = $"{currentStop}-{nextStop}";
                
                if (updatedRouteDetails.ContainsKey(key1))
                {
                    updatedRouteDetails.Remove(key1);
                }
                
                if (updatedRouteDetails.ContainsKey(key2))
                {
                    updatedRouteDetails.Remove(key2);
                }
                
                // Save the updated route details
                Session[SESSION_ROUTE_DETAILS] = updatedRouteDetails;

                // Calculate the new direct connection between previous and next stops
                try
                {
                    CalculateRouteDetail(previousStop, nextStop);
                }
                catch (Exception ex)
                {
                    // Log the error but continue - the route should still display without this segment
                    System.Diagnostics.Debug.WriteLine($"Error calculating new route segment: {ex.Message}");
                }

                // Always display the route, even if some calculations failed
                DisplayRoute();
            }
            catch (Exception ex)
            {
                // Show error but ensure route is still displayed
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    $"alert('Error removing stop: {ex.Message}');", true);
                
                // Force the route to display regardless of error
                DisplayRoute();
            }
        }

        private string GetWeatherForCity(string city)
        {
            try
            {
                string weather = weatherServiceClient.getWeather(city);
                return weather;
            }
            catch (Exception ex)
            {
                return "Weather data unavailable";
            }
        }

        private void DisplayRoute()
        {
            var stops = GetRouteStops();
            if (stops.Count < 2)
            {
                pnlRouteInfo.Visible = false;
                pnlNoRoute.Visible = true;
                return;
            }

            pnlRouteInfo.Visible = true;
            pnlNoRoute.Visible = false;

            // Clear the container
            routeContainer.Controls.Clear();

            var routeDetails = GetRouteDetails();
            double totalDistance = 0;
            int totalMinutes = 0;

            // Add each stop to the UI
            for (int i = 0; i < stops.Count; i++)
            {
                HtmlGenericControl stopDiv = new HtmlGenericControl("div");
                stopDiv.Attributes["class"] = "route-stop";

                // Add stop information
                HtmlGenericControl stopInfo = new HtmlGenericControl("div");
                stopInfo.Attributes["class"] = "stop-info";

                string stopLabel = i == 0 ? "Origin" : (i == stops.Count - 1 ? "Destination" : $"Stop {i}");
                HtmlGenericControl stopTitle = new HtmlGenericControl("div");
                stopTitle.InnerHtml = $"<strong>{stopLabel}:</strong> {stops[i]}";
                stopInfo.Controls.Add(stopTitle);

                // Add weather information
                string weather = GetWeatherForCity(stops[i]);
                HtmlGenericControl weatherInfo = new HtmlGenericControl("div");
                weatherInfo.Attributes["class"] = "weather-info";
                weatherInfo.InnerHtml = $"<span class='text-muted'>Weather:</span> <strong>{weather}</strong>";
                stopInfo.Controls.Add(weatherInfo);

                // Add leg information if this is not the last stop
                if (i < stops.Count - 1)
                {
                    string routeKey = $"{stops[i]}-{stops[i + 1]}";
                    if (routeDetails.TryGetValue(routeKey, out RouteDetail detail))
                    {
                        totalDistance += detail.DistanceMiles;
                        totalMinutes += detail.TimeMinutes;

                        HtmlGenericControl legInfo = new HtmlGenericControl("div");
                        legInfo.Attributes["class"] = "leg-info";
                        legInfo.InnerHtml = $"<span class='text-muted'>To next stop:</span> <strong>{detail.DistanceMiles} miles</strong> | <strong>{detail.FormattedTime}</strong>";
                        stopInfo.Controls.Add(legInfo);
                    }
                }

                stopDiv.Controls.Add(stopInfo);

                // Add action buttons
                HtmlGenericControl buttonDiv = new HtmlGenericControl("div");
                buttonDiv.Attributes["class"] = "stop-buttons";

                // Only intermediate stops can be removed
                if (i > 0 && i < stops.Count - 1)
                {
                    Button removeBtn = new Button();
                    removeBtn.Text = "Remove";
                    removeBtn.CssClass = "btn btn-sm btn-outline-danger";
                    removeBtn.CommandArgument = i.ToString();
                    removeBtn.Click += RemoveStop;
                    buttonDiv.Controls.Add(removeBtn);
                }

                // Add "Places Nearby" button for all stops
                Button placesBtn = new Button();
                placesBtn.Text = "Places Nearby";
                placesBtn.CssClass = "btn btn-sm btn-outline-primary";
                placesBtn.CommandArgument = i.ToString();
                placesBtn.OnClientClick = $"togglePlacesResults({i}); return false;";
                placesBtn.Click += GetPlacesForStop;
                buttonDiv.Controls.Add(placesBtn);

                stopDiv.Controls.Add(buttonDiv);
                routeContainer.Controls.Add(stopDiv);

                // Add a container for places results
                HtmlGenericControl placesResults = new HtmlGenericControl("div");
                placesResults.ID = $"placesResults_{i}";
                placesResults.Attributes["class"] = "places-results";
                placesResults.Style["display"] = "none";
                routeContainer.Controls.Add(placesResults);
            }

            // Update total distance and time
            lblTotalDistance.Text = $"{totalDistance:F2} miles";
            
            // Format the total time
            string formattedTime;
            if (totalMinutes < 60)
            {
                formattedTime = $"{totalMinutes} minute{(totalMinutes != 1 ? "s" : "")}";
            }
            else
            {
                int hours = totalMinutes / 60;
                int minutes = totalMinutes % 60;
                
                if (minutes == 0)
                {
                    // Only show hours if there are no minutes
                    formattedTime = $"{hours} hour{(hours != 1 ? "s" : "")}";
                }
                else
                {
                    // Show both hours and minutes
                    formattedTime = $"{hours} hour{(hours != 1 ? "s" : "")} {minutes} minute{(minutes != 1 ? "s" : "")}";
                }
            }
            lblTotalTime.Text = formattedTime;
        }

        private void RecalculateEntireRoute()
        {
            var stops = GetRouteStops();
            var routeDetails = new Dictionary<string, RouteDetail>();

            for (int i = 0; i < stops.Count - 1; i++)
            {
                string start = stops[i];
                string end = stops[i + 1];
                CalculateRouteDetail(start, end);
            }
        }

        private void CalculateRouteDetail(string start, string end)
        {
            var routeDetails = GetRouteDetails();
            string routeKey = $"{start}-{end}";

            // Skip if we already calculated this leg
            if (routeDetails.ContainsKey(routeKey))
            {
                return;
            }

            try
            {
                // Call the DistanceMatrix service
                TempWebApp.DistanceMatrixService.Service1Client distanceService = new TempWebApp.DistanceMatrixService.Service1Client();
                
                // Get distance in kilometers and convert to miles
                double distanceKm = distanceService.getDistance(start, end);
                double distanceMiles = Math.Round(distanceKm * 0.621371, 2);
                
                // Get time as a formatted string and estimate minutes
                string timeStr = distanceService.getTime(start, end);
                int estimatedMinutes = EstimateMinutesFromTimeString(timeStr);

                // Store the route detail
                routeDetails[routeKey] = new RouteDetail
                {
                    DistanceMiles = distanceMiles,
                    FormattedTime = timeStr,
                    TimeMinutes = estimatedMinutes
                };

                Session[SESSION_ROUTE_DETAILS] = routeDetails;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not calculate distance between {start} and {end}: {ex.Message}");
            }
        }

        private int EstimateMinutesFromTimeString(string timeStr)
        {
            int minutes = 0;

            // Parse a time string like "2 hours 35 mins"
            if (string.IsNullOrEmpty(timeStr))
            {
                return 0;
            }

            // Extract hours part
            if (timeStr.Contains("hour"))
            {
                string[] hourParts = timeStr.Split(new[] { "hour" }, StringSplitOptions.None);
                if (hourParts.Length > 0)
                {
                    if (int.TryParse(hourParts[0].Trim(), out int hours))
                    {
                        minutes += hours * 60;
                    }
                }

                // Look for minutes in the remaining part
                if (hourParts.Length > 1 && hourParts[1].Contains("min"))
                {
                    string minPart = hourParts[1].Trim();
                    // Extract numeric portion before "min"
                    int minIndex = minPart.IndexOf("min");
                    if (minIndex > 0)
                    {
                        string minValue = minPart.Substring(0, minIndex).Trim();
                        if (int.TryParse(minValue, out int mins))
                        {
                            minutes += mins;
                        }
                    }
                }
            }
            else if (timeStr.Contains("min"))
            {
                // Only minutes format (e.g., "35 mins")
                int minIndex = timeStr.IndexOf("min");
                if (minIndex > 0)
                {
                    string minValue = timeStr.Substring(0, minIndex).Trim();
                    if (int.TryParse(minValue, out int mins))
                    {
                        minutes = mins;
                    }
                }
            }

            return minutes;
        }

        private List<string> GetRouteStops()
        {
            return Session[SESSION_ROUTE_STOPS] as List<string> ?? new List<string>();
        }

        private Dictionary<string, RouteDetail> GetRouteDetails()
        {
            return Session[SESSION_ROUTE_DETAILS] as Dictionary<string, RouteDetail> ?? new Dictionary<string, RouteDetail>();
        }

        #endregion

        #region Forum Methods

        protected void btnNewPost_Click(object sender, EventArgs e)
        {
            pnlNewPost.Visible = true;
            txtPostSubject.Text = string.Empty;
            txtPostDescription.Text = string.Empty;
            ddlPostType.SelectedIndex = 0;
        }

        protected void btnCancelPost_Click(object sender, EventArgs e)
        {
            pnlNewPost.Visible = false;
        }

        protected void btnSubmitPost_Click(object sender, EventArgs e)
        {
            string subject = txtPostSubject.Text.Trim();
            string description = txtPostDescription.Text.Trim();
            string postType = ddlPostType.SelectedValue;

            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(description))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                    "alert('Please enter both subject and description.');", true);
                return;
            }

            try
            {
                // Get current username from session
                string username = Session[SESSION_USERNAME]?.ToString() ?? "Anonymous";
                
                // Add new post to XML
                AddPostToXml(subject, description, postType, username);
                
                // Clear form fields and hide panel
                pnlNewPost.Visible = false;
                
                // Refresh displayed posts
                DisplayPosts();
                
                // Show success message
                ScriptManager.RegisterStartupScript(this, GetType(), "postSuccess", 
                    "alert('Your post has been published!');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                    $"alert('Error posting: {ex.Message}');", true);
            }
        }

        protected void btnAddComment_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;
            HiddenField hdnPostId = (HiddenField)item.FindControl("hdnPostId");
            TextBox txtComment = (TextBox)item.FindControl("txtComment");

            int postId = Convert.ToInt32(hdnPostId.Value);
            string commentText = txtComment.Text.Trim();

            if (string.IsNullOrEmpty(commentText))
            {
                return;
            }

            try
            {
                // Get current username from session
                string username = Session[SESSION_USERNAME]?.ToString() ?? "Anonymous";
                bool isStaff = IsUserStaff();
                
                // Add comment to XML
                AddCommentToXml(postId, commentText, username, isStaff);
                
                // Clear comment text
                txtComment.Text = string.Empty;
                
                // Refresh displayed posts
                DisplayPosts();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", 
                    $"alert('Error adding comment: {ex.Message}');", true);
            }
        }

        protected void rptPosts_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
                    var filteredComments = comments.AsEnumerable()
                        .Where(r => Convert.ToInt32(r["PostId"]) == postId)
                        .OrderBy(r => r["CommentDate"]);
                    
                    // Check if there are any comments for this post
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

        private void DisplayPosts()
        {
            var posts = GetForumPosts();
            
            // Sort posts by date descending and take the 5 most recent
            var recentPosts = posts.AsEnumerable()
                .OrderByDescending(r => r.Field<DateTime>("PostDate"))
                .Take(5);
                
            // Check if there are any posts
            if (recentPosts.Any())
            {
                var recentPostsTable = recentPosts.CopyToDataTable();
                rptPosts.DataSource = recentPostsTable;
                rptPosts.DataBind();
                pnlNoPosts.Visible = false;
            }
            else
            {
                rptPosts.DataSource = null;
                rptPosts.DataBind();
                pnlNoPosts.Visible = true;
            }
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
                catch (Exception ex)
                {
                    // Just log the error, don't create sample posts
                    System.Diagnostics.Debug.WriteLine($"Error loading posts: {ex.Message}");
                }
            }
            // No else clause - we don't create sample posts if the file doesn't exist
            
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
                catch (Exception ex)
                {
                    // Just log the error, don't create sample comments
                    System.Diagnostics.Debug.WriteLine($"Error loading comments: {ex.Message}");
                }
            }
            // No else clause - we don't create sample comments if the file doesn't exist
            
            // Store in session for faster access next time
            Session[SESSION_COMMENTS] = commentsTable;
            
            return commentsTable;
        }

        private void AddPostToXml(string subject, string description, string postType, string username)
        {
            // Get existing posts
            var posts = GetForumPosts();
            
            // Find the next post ID
            int newId = 1;
            if (posts.Rows.Count > 0)
            {
                newId = posts.AsEnumerable().Max(r => Convert.ToInt32(r["PostId"])) + 1;
            }
            
            // Add new post to the DataTable
            DataRow newPost = posts.NewRow();
            newPost["PostId"] = newId;
            newPost["Subject"] = subject;
            newPost["Description"] = description;
            newPost["PostType"] = postType;
            newPost["UserName"] = username;
            newPost["PostDate"] = DateTime.Now;
            posts.Rows.Add(newPost);
            
            // Update session
            Session[SESSION_POSTS] = posts;
            
            // Save to XML
            SavePostsToXml(posts);
        }

        private void AddCommentToXml(int postId, string commentText, string username, bool isStaff)
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
            newComment["IsStaff"] = isStaff;
            comments.Rows.Add(newComment);
            
            // Update session
            Session[SESSION_COMMENTS] = comments;
            
            // Save to XML
            SaveCommentsToXml(comments);
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

        protected bool IsUserStaff()
        {
            return Session[SESSION_USER_TYPE] != null && Session[SESSION_USER_TYPE].ToString() == "Staff";
        }

        #endregion

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            // Clear all session data
            Session.Clear();
            
            // Redirect to login page
            Response.Redirect("~/Login.aspx");
        }
    }

    // Helper class for storing route details
    public class RouteDetail
    {
        public double DistanceMiles { get; set; }
        public string FormattedTime { get; set; }
        public int TimeMinutes { get; set; }
    }
}