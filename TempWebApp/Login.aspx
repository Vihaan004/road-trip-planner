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
        .btn-success {
            background: #198754; /* Bootstrap success green */
            border: none;
            padding: 0.6rem 1rem;
        }
        .btn-success:hover {
            background: #157347;
        }
        .btn-dark {
            background: #212529; /* Bootstrap dark */
            border: none;
            padding: 0.6rem 1rem;
            margin-top: 20px;
        }
        .btn-dark:hover {
            background: #424649;
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
        .btn-group {
            display: flex;
            gap: 10px;
            margin-bottom: 15px;
        }
        .account-type-selector {
            margin-bottom: 15px;
            text-align: center;
        }
        .admin-link {
            text-align: center;
            margin-top: 20px;
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
            <asp:Panel ID="LoginPanel" runat="server" DefaultButton="btnMemberLogin" CssClass="col-md-6 col-lg-5">
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
                    <div class="mb-3">
                        <asp:Label ID="lblCaptcha" runat="server" Text="Enter Captcha" CssClass="form-label"></asp:Label><br />
                        <asp:Image ID="imgCaptcha" runat="server" ImageUrl="~/CaptchaHandler.ashx" style="margin-bottom:10px; cursor: pointer;" ToolTip="Click to refresh Captcha" /><br />
                        <asp:TextBox ID="txtCaptcha" runat="server" CssClass="form-control" ValidationGroup="LoginGroup" placeholder="Enter Captcha"></asp:TextBox>
                        <small class="text-muted">Enter 'test' to bypass captcha</small>
                    </div>
                    <div class="mb-3 form-check">
                        <asp:CheckBox ID="chkRememberMe" runat="server" CssClass="form-check-input" />
                        <asp:Label ID="lblRememberMe" runat="server" Text="Remember Me" AssociatedControlID="chkRememberMe" CssClass="form-check-label"></asp:Label>
                    </div>
                    <div class="btn-group d-grid">
                        <asp:Button ID="btnMemberLogin" runat="server" Text="Member Login" OnClick="btnMemberLogin_Click" CssClass="btn btn-primary" ValidationGroup="LoginGroup" />
                        <asp:Button ID="btnStaffLogin" runat="server" Text="Staff Login" OnClick="btnStaffLogin_Click" CssClass="btn btn-success" ValidationGroup="LoginGroup" />
                    </div>
                    <div class="status-message">
                        <asp:Label ID="lblLoginStatus" runat="server"></asp:Label>
                    </div>
                </div>
            </asp:Panel>

            <!-- Sign Up Form Panel -->
            <asp:Panel ID="SignUpPanel" runat="server" DefaultButton="btnMemberSignUp" CssClass="col-md-6 col-lg-5">
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
                        <asp:CompareValidator ID="CompareValidator1" runat="server" 
                            ControlToCompare="txtSignUpPassword" 
                            ControlToValidate="txtConfirmPassword"
                            ErrorMessage="Passwords must match" 
                            CssClass="text-danger" 
                            Display="Dynamic"
                            ValidationGroup="SignUpGroup">
                        </asp:CompareValidator>
                    </div>
                    <div class="d-grid">
                        <asp:Button ID="btnMemberSignUp" runat="server" Text="Member Sign Up" OnClick="btnMemberSignUp_Click" CssClass="btn btn-primary" ValidationGroup="SignUpGroup" />
                    </div>
                    <div class="status-message">
                        <asp:Label ID="lblSignUpStatus" runat="server"></asp:Label>
                    </div>
                </div>
            </asp:Panel>
        </div>
        
        <!-- Administrator Link -->
        <div class="admin-link">
            <asp:HyperLink ID="hlAdministrator" runat="server" NavigateUrl="~/Admin.aspx" CssClass="btn btn-dark">
                Administrator
            </asp:HyperLink>
        </div>
    </div>
    <script type="text/javascript">
    document.getElementById('<%= imgCaptcha.ClientID %>').onclick = function () {
        this.src = 'CaptchaHandler.ashx?' + Math.random();
    };
    </script>
</asp:Content>