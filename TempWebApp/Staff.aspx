<%@ Page Title="Staff Portal" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Staff.aspx.cs" Inherits="TempWebApp.Staff" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .staff-container {
            max-width: 1000px;
            margin: 0 auto;
            padding: 20px;
        }
        .staff-header {
            background-color: #f8f9fa;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }
        .staff-welcome {
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 10px;
        }
        .staff-subtitle {
            color: #6c757d;
            margin-bottom: 20px;
        }
        .staff-card {
            background-color: #fff;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }
        .staff-card-title {
            font-size: 20px;
            font-weight: bold;
            margin-bottom: 10px;
            color: #0d6efd;
        }
        .staff-info {
            display: flex;
            justify-content: space-between;
            margin-bottom: 20px;
        }
        .staff-info-item {
            background-color: #f8f9fa;
            border-radius: 4px;
            padding: 15px;
            width: 48%;
        }
        .staff-info-label {
            font-weight: bold;
            margin-bottom: 5px;
            color: #495057;
        }
        .staff-actions {
            display: flex;
            gap: 10px;
            margin-top: 20px;
        }
        /* Forum styles */
        .forum-post {
            border-left: 4px solid #6c757d;
            background-color: #f8f9fa;
            padding: 15px;
            margin-bottom: 15px;
            border-radius: 6px;
        }
        .forum-post.question {
            border-left-color: #0d6efd;
        }
        .forum-post.tip {
            border-left-color: #198754;
        }
        .forum-post.review {
            border-left-color: #dc3545;
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
        .comment-form {
            margin-top: 10px;
        }
        .section-title {
            font-size: 22px;
            font-weight: 500;
            margin: 30px 0 15px;
            padding-bottom: 10px;
            border-bottom: 1px solid #dee2e6;
            color: #495057;
        }
        .forum-filter {
            margin-bottom: 20px;
        }
    </style>

    <div class="staff-container">
        <div class="staff-header">
            <div class="staff-welcome">Welcome to the Staff Portal, <asp:Label ID="lblStaffName" runat="server" Text="Staff Member"></asp:Label></div>
            <div class="staff-subtitle">Road Trip Community Forum Management</div>
            <div class="staff-actions">
                <asp:LinkButton ID="lnkLogout" runat="server" CssClass="btn btn-outline-secondary" OnClick="lnkLogout_Click">
                    <i class="bi bi-box-arrow-right"></i> Logout
                </asp:LinkButton>
                <asp:LinkButton ID="lnkServiceDirectory" runat="server" CssClass="btn btn-outline-primary" PostBackUrl="~/ServiceDirectory.aspx">
                    <i class="bi bi-grid"></i> Service Directory
                </asp:LinkButton>
            </div>
        </div>

        <!-- Forum/Discussion Section -->
        <h2 class="section-title">Road Trip Community Forum</h2>
        <div class="staff-card">
            <div class="staff-card-title">All Member Posts</div>
            <p>View and respond to all member posts. As a staff member, your comments will be highlighted.</p>
            
            <div class="forum-filter">
                <asp:DropDownList ID="ddlPostTypeFilter" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPostTypeFilter_SelectedIndexChanged">
                    <asp:ListItem Text="All Posts" Value="All"></asp:ListItem>
                    <asp:ListItem Text="Questions Only" Value="Question"></asp:ListItem>
                    <asp:ListItem Text="Tips Only" Value="Tip"></asp:ListItem>
                    <asp:ListItem Text="Reviews Only" Value="Review"></asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <asp:Repeater ID="rptAllPosts" runat="server" OnItemDataBound="rptAllPosts_ItemDataBound">
                <ItemTemplate>
                    <div class="forum-post <%# Eval("PostType").ToString().ToLower() %>">
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
                        
                        <!-- Staff Comment Form -->
                        <div class="comment-form">
                            <asp:HiddenField ID="hdnPostId" runat="server" Value='<%# Eval("PostId") %>' />
                            <div class="input-group">
                                <asp:TextBox ID="txtStaffComment" runat="server" CssClass="form-control" placeholder="Add your response as a staff member..."></asp:TextBox>
                                <asp:Button ID="btnAddStaffComment" runat="server" Text="Respond" CssClass="btn btn-primary" OnClick="btnAddStaffComment_Click" CommandArgument='<%# Eval("PostId") %>' />
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
            <asp:Panel ID="pnlNoPosts" runat="server" Visible="false">
                <div class="alert alert-info mt-3">
                    There are currently no posts in the forum.
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>