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
    </style>

    <div class="staff-container">
        <div class="staff-header">
            <div class="staff-welcome">Welcome to the Staff Portal, <asp:Label ID="lblStaffName" runat="server" Text="Staff Member"></asp:Label></div>
            <div class="staff-subtitle">Access and manage road trip planning services</div>
            <div class="staff-actions">
                <asp:LinkButton ID="lnkLogout" runat="server" CssClass="btn btn-outline-secondary" OnClick="lnkLogout_Click">
                    <i class="bi bi-box-arrow-right"></i> Logout
                </asp:LinkButton>
                <asp:LinkButton ID="lnkServiceDirectory" runat="server" CssClass="btn btn-outline-primary" PostBackUrl="~/ServiceDirectory.aspx">
                    <i class="bi bi-grid"></i> Service Directory
                </asp:LinkButton>
            </div>
        </div>

        <div class="staff-card">
            <div class="staff-card-title">Staff Dashboard</div>
            <p>This is the restricted area for staff members. As a staff member, you have access to additional features and management tools.</p>
            
            <div class="staff-info">
                <div class="staff-info-item">
                    <div class="staff-info-label">Last Login</div>
                    <asp:Label ID="lblLastLogin" runat="server" Text="First time login"></asp:Label>
                </div>
                <div class="staff-info-item">
                    <div class="staff-info-label">Account Type</div>
                    <asp:Label ID="lblAccountType" runat="server" Text="Staff"></asp:Label>
                </div>
            </div>
        </div>

        <div class="staff-card">
            <div class="staff-card-title">Service Management</div>
            <p>From here you can manage the Road Trip Planning services and access administrative features.</p>
            
            <div class="staff-actions">
                <asp:Button ID="btnManageUsers" runat="server" Text="Manage Users" CssClass="btn btn-primary" OnClick="btnManageUsers_Click" />
                <asp:Button ID="btnViewReports" runat="server" Text="View Reports" CssClass="btn btn-info" OnClick="btnViewReports_Click" />
                <asp:Button ID="btnServiceSettings" runat="server" Text="Service Settings" CssClass="btn btn-secondary" OnClick="btnServiceSettings_Click" />
            </div>
        </div>
    </div>
</asp:Content>