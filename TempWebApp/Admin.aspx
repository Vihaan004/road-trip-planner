<%@ Page Title="Administrator" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="TempWebApp.Admin" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .admin-container {
            max-width: 1000px;
            margin: 0 auto;
            padding: 20px;
        }
        .admin-header {
            background-color: #212529;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            color: white;
        }
        .admin-title {
            font-size: 28px;
            font-weight: bold;
            margin-bottom: 10px;
        }
        .admin-subtitle {
            color: #adb5bd;
            margin-bottom: 20px;
            font-size: 16px;
        }
        .admin-card {
            background-color: #fff;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
            border: 1px solid #dee2e6;
        }
        .admin-card-title {
            font-size: 20px;
            font-weight: bold;
            margin-bottom: 15px;
            color: #212529;
            border-bottom: 1px solid #dee2e6;
            padding-bottom: 10px;
        }
        .form-group {
            margin-bottom: 15px;
        }
        .form-control {
            width: 100%;
            padding: 8px;
            border: 1px solid #ced4da;
            border-radius: 4px;
        }
        .btn-add {
            background-color: #198754;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 4px;
            cursor: pointer;
        }
        .btn-delete {
            background-color: #dc3545;
            color: white;
            border: none;
            padding: 4px 8px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 12px;
        }
        .grid-container {
            width: 100%;
            margin-top: 20px;
            overflow-x: auto;
        }
        .staff-grid {
            width: 100%;
            border-collapse: collapse;
        }
        .staff-grid th {
            background-color: #f8f9fa;
            font-weight: bold;
            text-align: left;
            padding: 10px;
            border-bottom: 2px solid #dee2e6;
        }
        .staff-grid td {
            padding: 10px;
            border-bottom: 1px solid #dee2e6;
        }
        .message {
            margin: 15px 0;
            padding: 10px;
            border-radius: 4px;
        }
        .success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        .error {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        .navigation-links {
            margin-bottom: 20px;
        }
        .navigation-links a {
            margin-right: 10px;
            text-decoration: none;
        }
    </style>

    <div class="admin-container">
        <div class="admin-header">
            <div class="admin-title">Administrator Panel</div>
            <div class="admin-subtitle">Manage Staff Accounts</div>
            <div class="navigation-links">
                <asp:HyperLink ID="hlHome" runat="server" NavigateUrl="~/Default.aspx" CssClass="btn btn-outline-light">Home</asp:HyperLink>
                <asp:HyperLink ID="hlBack" runat="server" NavigateUrl="~/Login.aspx" CssClass="btn btn-outline-light">Back to Login</asp:HyperLink>
            </div>
        </div>

        <!-- Message Display -->
        <asp:Panel ID="pnlMessage" runat="server" CssClass="message" Visible="false">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Add New Staff Member -->
        <div class="admin-card">
            <div class="admin-card-title">Add New Staff Member</div>
            <div class="form-group">
                <asp:Label ID="lblUsername" runat="server" Text="Username" AssociatedControlID="txtUsername" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter username"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvUsername" runat="server" 
                    ControlToValidate="txtUsername" 
                    ErrorMessage="Username is required" 
                    CssClass="text-danger"
                    Display="Dynamic"
                    ValidationGroup="StaffCreateGroup">
                </asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <asp:Label ID="lblPassword" runat="server" Text="Password" AssociatedControlID="txtPassword" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" 
                    ControlToValidate="txtPassword" 
                    ErrorMessage="Password is required" 
                    CssClass="text-danger"
                    Display="Dynamic"
                    ValidationGroup="StaffCreateGroup">
                </asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revPassword" runat="server" 
                    ControlToValidate="txtPassword"
                    ValidationExpression=".{8,}" 
                    ErrorMessage="Password must be at least 8 characters long" 
                    CssClass="text-danger"
                    Display="Dynamic"
                    ValidationGroup="StaffCreateGroup">
                </asp:RegularExpressionValidator>
            </div>
            <div class="form-group">
                <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm Password" AssociatedControlID="txtConfirmPassword" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Confirm password"></asp:TextBox>
                <asp:CompareValidator ID="cvPassword" runat="server" 
                    ControlToCompare="txtPassword" 
                    ControlToValidate="txtConfirmPassword" 
                    ErrorMessage="Passwords do not match" 
                    CssClass="text-danger"
                    Display="Dynamic"
                    ValidationGroup="StaffCreateGroup">
                </asp:CompareValidator>
            </div>
            <div class="form-group">
                <asp:Button ID="btnAddStaff" runat="server" Text="Add Staff Member" CssClass="btn-add" OnClick="btnAddStaff_Click" ValidationGroup="StaffCreateGroup" />
            </div>
        </div>

        <!-- Staff Members List -->
        <div class="admin-card">
            <div class="admin-card-title">Current Staff Members</div>
            <div class="grid-container">
                <asp:GridView ID="gvStaffMembers" runat="server" 
                    AutoGenerateColumns="False" 
                    CssClass="staff-grid" 
                    OnRowCommand="gvStaffMembers_RowCommand" 
                    OnRowDeleting="gvStaffMembers_RowDeleting"
                    DataKeyNames="Username">
                    <Columns>
                        <asp:BoundField DataField="Username" HeaderText="Username" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnDelete" runat="server" Text="Delete" 
                                    CommandName="Delete" 
                                    CommandArgument='<%# Eval("Username") %>' 
                                    CssClass="btn-delete"
                                    OnClientClick="return confirm('Are you sure you want to delete this staff member?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center p-3">No staff members found.</div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>