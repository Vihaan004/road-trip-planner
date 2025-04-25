<%@ Page Title="Login / Sign Up" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TempWebApp.Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">    
    <style>
        body {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }
        .main-hero {
            background: #fff;
            border-radius: 1rem;
            box-shadow: 0 8px 32px 0 rgba(0,0,0,0.08);
            padding: 2rem 1rem 2rem 1rem;
            margin-top: 3rem;
            margin-bottom: 3rem;
            width: 700px;
            
            border: 1px solid black;
        }
        .app-name {
            font-size: x-large;
        }
        .form-container {
            background: #fff;
            border-radius: 1rem; /* Rounded corners */
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            padding: 2rem;
            margin-top: 2rem;
            margin-bottom: 2rem;
            height: 90%;
            border: 1px solid black;
        }
        .form-title {
            font-size: 1.75rem;
            font-weight: 600;
            color: #343a40; /* Darker title */
            margin-bottom: 1.5rem;
            text-align: center;
        }
        label, .form-label {
            color: #495057;
            font-weight: 500;
            margin-bottom: 0.5rem;
        }
        .btn-primary {
            background: #0d6efd; /* Bootstrap primary blue */
            border: none;
            padding: 0.6rem 1rem;
            
        }
        .btn-primary:hover {
            background: #0b5ed7;
        }
        .status-message {
            min-height: 24px; /* Reserve space for messages */
            margin-top: 1rem;
            text-align: center;
            font-weight: 500;
        }
        .status-success {
            color: green;
        }
        .status-error {
            color: red;
        }
    </style>

    <!-- Navbar -->
    <nav class="navbar navbar-expand-lg bg-body-tertiary mb-4 sticky-top">
        <div class="container-fluid" style="max-width: 1000px;">
            <div class="app-name">
                <h1>Road Trip Planner</h1>
            </div>
            <div class="d-flex ms-auto">
                <asp:HyperLink ID="hlHome" runat="server" NavigateUrl="~/Default.aspx" CssClass="btn btn-outline-primary">Return</asp:HyperLink>
            </div>
        </div>
    </nav>

    <div class="main-hero">
        <div class="row justify-content-center g-4">
            <!-- Login Form Panel -->
            <asp:Panel ID="LoginPanel" runat="server" DefaultButton="btnLogin" CssClass="col-md-6 col-lg-5">
                <div class="form-container">
                    <div class="form-title">Login</div>
                    <div class="mb-3">
                        <asp:Label ID="lblLoginUsername" runat="server" Text="Username" AssociatedControlID="txtLoginUsername" CssClass="form-label"></asp:Label>
                        <asp:TextBox ID="txtLoginUsername" runat="server" CssClass="form-control" ValidationGroup="LoginGroup"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <asp:Label ID="lblLoginPassword" runat="server" Text="Password" AssociatedControlID="txtLoginPassword" CssClass="form-label"></asp:Label>
                        <asp:TextBox ID="txtLoginPassword" runat="server" TextMode="Password" CssClass="form-control" autocomplete="current-password" ValidationGroup="LoginGroup"></asp:TextBox>
                    </div>
                    <div class="mb-3 form-check">
                        <asp:CheckBox ID="chkRememberMe" runat="server" CssClass="form-check-input" />
                        <asp:Label ID="lblRememberMe" runat="server" Text="Remember Me" AssociatedControlID="chkRememberMe" CssClass="form-check-label"></asp:Label>
                    </div>
                    <div class="d-grid">
                        <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" CssClass="btn btn-primary" ValidationGroup="LoginGroup" />
                    </div>
                    <div class="status-message">
                        <asp:Label ID="lblLoginStatus" runat="server"></asp:Label>
                    </div>
                </div>
            </asp:Panel>

            <!-- Sign Up Form Panel -->
            <asp:Panel ID="SignUpPanel" runat="server" DefaultButton="btnSignUp" CssClass="col-md-6 col-lg-5">
                <div class="form-container">
                    <div class="form-title">Sign Up</div>
                    <div class="mb-3">
                        <asp:Label ID="lblSignUpUsername" runat="server" Text="Username" AssociatedControlID="txtSignUpUsername" CssClass="form-label"></asp:Label>
                        <asp:TextBox ID="txtSignUpUsername" runat="server" CssClass="form-control" ValidationGroup="SignUpGroup"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <asp:Label ID="lblSignUpPassword" runat="server" Text="Create Password" AssociatedControlID="txtSignUpPassword" CssClass="form-label"></asp:Label>
                        <asp:TextBox ID="txtSignUpPassword" runat="server" TextMode="Password" CssClass="form-control" ValidationGroup="SignUpGroup" autocomplete="new-password"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm Password" AssociatedControlID="txtConfirmPassword" CssClass="form-label"></asp:Label>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" ValidationGroup="SignUpGroup" autocomplete="new-password"></asp:TextBox>
                    </div>
                    <div class="d-grid">
                        <asp:Button ID="btnSignUp" runat="server" Text="Sign Up" OnClick="btnSignUp_Click" CssClass="btn btn-primary" ValidationGroup="SignUpGroup" />
                    </div>
                    <div class="status-message">
                        <asp:Label ID="lblSignUpStatus" runat="server"></asp:Label>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
