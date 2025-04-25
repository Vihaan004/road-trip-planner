<%@ Page Title="Road Trip Planner" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TempWebApp._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .main-container {
            display: flex;
            justify-content: center;
            align-items: center;
            margin: 0;
        }
        .main-hero {
            background: #ffffff;
            border-radius: 1rem;
            box-shadow: 0 8px 32px 0 rgba(0,0,0,0.08);
            padding: 2rem 1rem 2rem 1rem;
            border: 1px solid black;
            width: 900px;
        }
        .brand-desc {
            color: #3d3d3d;
            font-size: 1.15rem;
        }
        .card {
            border: none;
            border-radius: 1.25rem;
            box-shadow: 0 4px 16px 0 rgba(0,0,0,0.04);
        }
        .btn-primary {
            background: #0d6efd;
            border: none;
        }
        .explore-card {
            background: #ffffff;
            border-radius: 1rem;
            box-shadow: 0 8px 32px 0 rgba(0,0,0,0.08);
            border: 1px solid black;
        }
        .app-name {
            font-size: x-large;
        }
        img {
            filter: brightness(0) invert(0);
        }
        .testing-flow {
            display: flex;
            flex-direction: column;
            align-items: center;
            /* width: 60%; */
            margin: 0 auto;
            border-radius: 1.25rem;
            padding: 1.5rem;
            box-shadow: 0 8px 32px 0 rgba(0,0,0,0.08);
        }
        .testing-flow-header {
            text-align: center;
            font-size: 1rem;
            font-weight: 600;
            color: #343a40;
            margin-bottom: 1rem;
        }
    </style>
    <div class="main-container">
        <div class="main-hero">
            <div class="app-name">
                <h1 class="text-center"><strong>Road Trip Planner</strong></h1>
            </div>
            <div class="row mb-4">
                <div class="col-md-8 mx-auto">
                    <p class="brand-desc text-center">
                        Welcome to your comprehensive road trip planner. Effortlessly plan trips, explore nearby stops, top rated restaurants, gas stations and get frequent weather updates. Get prompt support from our road planner support team as well.&nbsp;
                    </p>
                </div>
            </div>
            <div class="row g-4 justify-content-center">
                <div class="col-md-5">
                    <div class="card h-100 p-4 text-center explore-card">
                        <div class="mb-3">
                            <img src="https://img.icons8.com/ios-filled/100/6366f1/user-group-man-man.png" alt="Member Access" style="width:60px;" />
                        </div>
                        <h5 class="card-title mb-2">User Access</h5>
                        <p class="card-text text-muted mb-4">Get Started!</p>
                        <asp:HyperLink ID="hlLogin" NavigateUrl="~/Login.aspx" Text="Get Started" runat="server" CssClass="btn btn-primary px-4" />
                    </div>
                </div>
                <div class="col-md-5">
                    <div class="card h-100 p-4 text-center explore-card">
                        <div class="mb-3">
                            <img src="https://img.icons8.com/ios-filled/100/6366f1/services.png" alt="Explore Services" style="width:60px;" />
                        </div>
                        <h5 class="card-title mb-2">Services</h5>
                        <p class="card-text text-muted mb-4">Browse the service directory.</p>
                        <asp:HyperLink ID="hlServiceDirectory" NavigateUrl="~/ServiceDirectory.aspx" Text="Go to Service Directory" runat="server" CssClass="btn btn-primary px-4" />
                    </div>
                </div>
            </div>

            <div class="mt-5">
                <div class="testing-flow">
                    <div>
                        <h2 class="testing-flow-header">Testing Flow</h2>
                        <ul>
                            <li>Browse the service directory to view currently implemented local components and test the service utilized</li>
                            <li><b>Sign up:</b> Use the 'get started' button to sign up and create an account.</li>
                            <li><b>Login:</b> The same page has an option to login after creating an account.</li>
                            <li><b>To be implemented: </b>Home page, Comprehensive user control, Staff management.</li>
                        </ul>
                    </div>
                    
                </div>
            </div>
        </div>
    </div>
</asp:Content>
