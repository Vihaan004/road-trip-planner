<%@ Page Title="Road Trip Planner" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="TempWebApp.Home" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .home-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }
        .page-title {
            font-size: 28px;
            font-weight: 600;
            margin-bottom: 20px;
            color: #0d6efd;
        }
        .section-title {
            font-size: 22px;
            font-weight: 500;
            margin: 30px 0 15px;
            padding-bottom: 10px;
            border-bottom: 1px solid #dee2e6;
            color: #495057;
        }
        .card {
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
            margin-bottom: 20px;
            overflow: hidden;
        }
        .card-header {
            background-color: #f8f9fa;
            padding: 15px 20px;
            font-weight: 500;
        }
        .card-body {
            padding: 20px;
        }
        .route-stop {
            background-color: #f8f9fa;
            border-radius: 6px;
            padding: 15px;
            margin-bottom: 10px;
            border-left: 4px solid #0d6efd;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .stop-buttons {
            display: flex;
            gap: 10px;
        }
        .stop-info {
            flex-grow: 1;
            padding-right: 10px;
        }
        .leg-info {
            background-color: #e9ecef;
            padding: 10px;
            margin: 10px 0;
            border-radius: 6px;
            font-size: 14px;
        }
        .forum-post {
            border-left: 4px solid #6c757d;
            background-color: #f8f9fa;
            padding: 15px;
            margin-bottom: 15px;
            border-radius: 6px;
        }
        .post-header {
            display: flex;
            justify-content: space-between;
            margin-bottom: 10px;
        }
        .post-title {
            font-weight: 500;
            font-size: 18px;
        }
        .post-meta {
            color: #6c757d;
            font-size: 14px;
        }
        .post-content {
            margin-bottom: 10px;
        }
        .post-type {
            display: inline-block;
            padding: 3px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 500;
            margin-left: 10px;
        }
        .post-type-question {
            background-color: #cfe2ff;
            color: #084298;
        }
        .post-type-tip {
            background-color: #d1e7dd;
            color: #0f5132;
        }
        .post-type-review {
            background-color: #f8d7da;
            color: #842029;
        }
        .post-comments {
            margin-top: 10px;
            padding-left: 20px;
            border-left: 2px solid #dee2e6;
        }
        .comment {
            padding: 10px;
            margin-bottom: 8px;
            background-color: #ffffff;
            border-radius: 4px;
            border: 1px solid #dee2e6;
        }
        .staff-comment {
            border-left: 4px solid #0d6efd;
        }
    </style>

    <div class="home-container">
        <h1 class="page-title">Welcome, <asp:Label ID="lblUserName" runat="server" Text="Member"></asp:Label>!</h1>
        
        <div class="d-flex justify-content-end mb-3">
            <asp:LinkButton ID="lnkLogout" runat="server" CssClass="btn btn-outline-secondary" OnClick="lnkLogout_Click">
                <i class="bi bi-box-arrow-right"></i> Logout
            </asp:LinkButton>
        </div>

        <!-- Road Trip Planner Section -->
        <h2 class="section-title">Plan Your Road Trip</h2>
        <div class="card">
            <div class="card-header">
                Create a New Route
            </div>
            <div class="card-body">
                <div class="row mb-3">
                    <div class="col-md-5">
                        <div class="form-group">
                            <label for="txtOrigin" class="form-label">Origin:</label>
                            <asp:TextBox ID="txtOrigin" runat="server" CssClass="form-control" placeholder="Enter starting city"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-5">
                        <div class="form-group">
                            <label for="txtDestination" class="form-label">Destination:</label>
                            <asp:TextBox ID="txtDestination" runat="server" CssClass="form-control" placeholder="Enter final destination"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-2 d-flex align-items-end">
                        <asp:Button ID="btnCreateRoute" runat="server" Text="Create Route" CssClass="btn btn-primary w-100" OnClick="btnCreateRoute_Click" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <div class="form-group">
                            <label for="txtNewStop" class="form-label">Add Stop:</label>
                            <div class="input-group">
                                <asp:TextBox ID="txtNewStop" runat="server" CssClass="form-control" placeholder="Enter city name"></asp:TextBox>
                                <asp:Button ID="btnAddStop" runat="server" Text="Add Stop" CssClass="btn btn-secondary" OnClick="btnAddStop_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-header">
                Your Route
            </div>
            <div class="card-body">
                <asp:Panel ID="pnlRouteInfo" runat="server" Visible="false">
                    <div class="alert alert-info">
                        <strong>Total Distance:</strong> <asp:Label ID="lblTotalDistance" runat="server" Text="0 miles"></asp:Label>
                    </div>
                    
                    <div id="routeContainer" runat="server" class="route-container">
                        <!-- Route stops will be added here programmatically -->
                    </div>
                </asp:Panel>
                
                <asp:Panel ID="pnlNoRoute" runat="server">
                    <div class="alert alert-secondary">
                        No route created yet. Enter origin and destination to start planning your trip.
                    </div>
                </asp:Panel>
            </div>
        </div>

        <!-- Forum/Discussion Section -->
        <h2 class="section-title">Road Trip Community</h2>
        <div class="card">
            <div class="card-header d-flex justify-content-between align-items-center">
                <span>Recent Discussions</span>
                <asp:Button ID="btnNewPost" runat="server" Text="New Post" CssClass="btn btn-sm btn-primary" OnClick="btnNewPost_Click" />
            </div>
            <div class="card-body">
                <asp:Panel ID="pnlNewPost" runat="server" Visible="false" CssClass="mb-4 p-3 border rounded">
                    <h5>Create New Post</h5>
                    <div class="mb-3">
                        <label for="txtPostSubject" class="form-label">Subject:</label>
                        <asp:TextBox ID="txtPostSubject" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label for="txtPostDescription" class="form-label">Description:</label>
                        <asp:TextBox ID="txtPostDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label for="ddlPostType" class="form-label">Post Type:</label>
                        <asp:DropDownList ID="ddlPostType" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Question" Value="Question"></asp:ListItem>
                            <asp:ListItem Text="Tip" Value="Tip"></asp:ListItem>
                            <asp:ListItem Text="Review" Value="Review"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="mt-3">
                        <asp:Button ID="btnSubmitPost" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmitPost_Click" />
                        <asp:Button ID="btnCancelPost" runat="server" Text="Cancel" CssClass="btn btn-outline-secondary ms-2" OnClick="btnCancelPost_Click" />
                    </div>
                </asp:Panel>

                <asp:Repeater ID="rptPosts" runat="server" OnItemDataBound="rptPosts_ItemDataBound">
                    <ItemTemplate>
                        <div class="forum-post">
                            <div class="post-header">
                                <div>
                                    <span class="post-title"><%# Eval("Subject") %></span>
                                    <span class="post-type post-type-<%# Eval("PostType").ToString().ToLower() %>"><%# Eval("PostType") %></span>
                                </div>
                                <div class="post-meta">
                                    Posted by <%# Eval("UserName") %> on <%# Eval("PostDate", "{0:MMM dd, yyyy}") %>
                                </div>
                            </div>
                            <div class="post-content">
                                <%# Eval("Description") %>
                            </div>
                            
                            <!-- Comments Section -->
                            <asp:Panel ID="pnlComments" runat="server" CssClass="post-comments">
                                <asp:Repeater ID="rptComments" runat="server">
                                    <ItemTemplate>
                                        <div class="comment <%# ((bool)Eval("IsStaff")) ? "staff-comment" : "" %>">
                                            <div class="post-meta mb-1">
                                                <strong><%# Eval("UserName") %></strong> 
                                                <%# ((bool)Eval("IsStaff")) ? "<span class='badge bg-primary'>Staff</span>" : "" %>
                                                on <%# Eval("CommentDate", "{0:MMM dd, yyyy}") %>
                                            </div>
                                            <%# Eval("CommentText") %>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                            
                            <!-- Comment form for all users (not just staff) -->
                            <asp:Panel ID="pnlAddComment" runat="server">
                                <div class="mt-2 input-group">
                                    <asp:HiddenField ID="hdnPostId" runat="server" Value='<%# Eval("PostId") %>' />
                                    <asp:TextBox ID="txtComment" runat="server" CssClass="form-control" placeholder="Add a comment..."></asp:TextBox>
                                    <asp:Button ID="btnAddComment" runat="server" Text="Comment" CssClass="btn btn-outline-primary" OnClick="btnAddComment_Click" CommandArgument='<%# Eval("PostId") %>' />
                                </div>
                            </asp:Panel>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlNoPosts" runat="server" Visible="false">  
                    <div class="alert alert-info">
                        No posts yet. Be the first to share your road trip experience or ask a question!
                    </div>
                </asp:Panel>
            </div>
        </div>

        <!-- Nearby Places Section -->
        <h2 class="section-title">Find Nearby Places</h2>
        <div class="card">
            <div class="card-header">
                Search for Restaurants & Gas Stations
            </div>
            <div class="card-body">
                <div class="row mb-3">
                    <div class="col-md-8">
                        <div class="form-group">
                            <label for="txtLocation" class="form-label">Location:</label>
                            <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control" placeholder="Enter Location"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-4 d-flex align-items-end">
                        <asp:Button ID="btnSearchPlaces" runat="server" Text="Search Places" CssClass="btn btn-primary" OnClick="btnSearchPlaces_Click" />
                    </div>
                </div>

                <asp:Panel ID="pnlPlacesResults" runat="server" Visible="false">
                    <div class="row mt-4">
                        <!-- Gas Stations Column -->
                        <div class="col-md-6">
                            <div class="card">
                                <div class="card-header bg-light">
                                    <h5 class="mb-0">Gas Stations</h5>
                                </div>
                                <div class="card-body">
                                    <asp:BulletedList ID="lstGasStations" runat="server" CssClass="list-group list-group-flush" BulletStyle="NotSet">
                                    </asp:BulletedList>
                                </div>
                            </div>
                        </div>
                        
                        <!-- Restaurants Column -->
                        <div class="col-md-6">
                            <div class="card">
                                <div class="card-header bg-light">
                                    <h5 class="mb-0">Restaurants</h5>
                                </div>
                                <div class="card-body">
                                    <asp:BulletedList ID="lstRestaurants" runat="server" CssClass="list-group list-group-flush" BulletStyle="NotSet">
                                    </asp:BulletedList>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
