<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceDirectory.aspx.cs" Inherits="TempWebApp.ServiceDirectory" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Service Directory</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background-color: #f8f9fa;
        }
        
        .header-bar {
            background-color: #ffffff;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            padding: 12px 0;
            position: sticky;
            top: 0;
            z-index: 1000;
        }
        
        .page-container {
            max-width: 1140px;
            margin: 0 auto;
            padding: 40px 20px;
        }
        
        .page-title {
            font-size: 2.25rem;
            font-weight: 600;
            color: #0d6efd;
            margin-bottom: 1.5rem;
            text-align: center;
        }
        .subtitle {
            font-size: 1rem;
            color: #6c757d;
            text-align: center;
        }
        
        .content-card {
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 8px 32px 0 rgba(0,0,0,0.08);
            padding: 30px;
            margin-bottom: 30px;
        }
        
        .service-table th {
            background-color: #f1f4f9;
            font-weight: 600;
        }
        
        .service-table td, .service-table th {
            padding: 12px 15px;
            vertical-align: middle;
        }
        
        .try-button {
            background-color: #198754;
            color: white;
            border: none;
            border-radius: 4px;
            padding: 6px 12px;
            font-weight: 500;
        }
        
        .try-button:hover {
            background-color: #157347;
            color: white;
        }
        .instructions{
            margin: 0 2rem 0 2rem;
            display: flex;
            flex-direction: column;
            align-items: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">        
        <!-- Header Bar -->
        <header class="header-bar">
            <div class="container">
                <div class="d-flex justify-content-between align-items-center">
                    <h1 class="h4 mb-0">Road Trip Planner</h1>
                    <asp:HyperLink ID="hlHome" runat="server" NavigateUrl="~/Default.aspx" CssClass="btn btn-outline-primary">Return Home</asp:HyperLink>
                </div>
            </div>
        </header>
        
        <!-- Main Content -->
        <div class="page-container">
            <div class="content-card">
                <h2 class="page-title">Service Directory</h2>
                <p class="subtitle">Components developed by Vihaan Patel.</p>
                <hr />
                
                <div class="table-responsive">
                    <table class="table table-hover service-table">
                        <thead>
                            <tr>
                                <th scope="col">Developer</th>
                                <th scope="col">Component</th>
                                <th scope="col">Type</th>
                                <th scope="col">Operations</th>
                                <th scope="col">Input</th>
                                <th scope="col">Description</th>
                                <th scope="col">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>Vihaan Patel</td>
                                <td>Distance Matrix Service</td>
                                <td>WCF Service</td>
                                <td>getDistance<br />getTime</td>
                                <td>string origin, string destination</td>
                                <td>Gets the driving distance and time between two cities using the Google Distance Matrix API.</td>
                                <td>
                                    <asp:Button ID="btnTryIt" runat="server" Text="Try It" CssClass="btn try-button" OnClick="btnTryIt_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>Atiman Rohatgi</td>
                                <td>Places Service</td>
                                <td>WCF Service</td>
                                <td>getPlaces<br />findPlaceDetails</td>
                                <td>string location, string type</td>
                                <td>Finds nearby points of interest based on location and category using Google Places API.</td>
                                <td>
                                    <asp:Button ID="btnTryPlaces" runat="server" Text="Try It" CssClass="btn try-button" OnClick="btnTryPlaces_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>Atiman Rohatgi</td>
                                <td>Password Hashing</td>
                                <td>Class Library</td>
                                <td>HashPassword<br />ValidatePassword</td>
                                <td>string password</td>
                                <td>Securely hashes passwords using SHA-256 for secure credential storage.</td>
                                <td>
                                    <asp:Button ID="btnTryHashing" runat="server" Text="Try It" CssClass="btn try-button" OnClick="btnTryHashing_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td>Aryan Yeole</td>
                                <td>Weather Service</td>
                                <td>WCF Service</td>
                                <td>getWeather</td>
                                <td>string city</td>
                                <td>Fetch weather data for a specific city along the planned route.</td>
                                <td>-</td>
                            </tr>
                            <tr>
                                <td>Vihaan Patel</td>
                                <td>User Authentication</td>
                                <td>Data storage (XML)</td>
                                <td>Store/fetch credentials</td>
                                <td>-</td>
                                <td>Stores usernames and passwords to maintain a user database.</td>
                                <td>-</td>
                            </tr>
                            <tr>
                                <td>Aryan Yeole</td>
                                <td>Captcha</td>
                                <td>Authentication</td>
                                <td>Verify Human User</td>
                                <td>-</td>
                                <td>Utilize Captcha service for human authentication at login.</td>
                                <td>-</td>
                            </tr>
                            <tr>
                                <td>Vihaan Patel</td>
                                <td>Client Storage</td>
                                <td>HTTP Cookie</td>
                                <td>Remember Me</td>
                                <td>-</td>
                                <td>Stores client-side information of login details.</td>
                                <td>-</td>
                            </tr>
                            <tr>
                                <td>Vihaan Patel</td>
                                <td>Posts Database</td>
                                <td>Data storage (XML)</td>
                                <td>Store/fetch forum posts</td>
                                <td>-</td>
                                <td>Stores forum posts and comments data for the road trip community section.</td>
                                <td>-</td>
                            </tr>
                            <tr>
                                <td>Vihaan Patel</td>
                                <td>Bootstrap UI</td>
                                <td>CSS Framework</td>
                                <td>User Interface</td>
                                <td>-</td>
                                <td>Consistent web app layout and design using Bootstrap 5.3.</td>
                                <td>-</td>
                            </tr>
                            <tr>
                                <td>Aryan Yeole</td>
                                <td>Broswer Location</td>
                                <td>App Permission</td>
                                <td>Fetch User Location</td>
                                <td>-</td>
                                <td>Obtains the location of website access to utilize for planning a road trip.</td>
                                <td>-</td>
                            </tr>
                            <tr>
                                <td>Atiman Rohatgi</td>
                                <td>Visitor Counter</td>
                                <td>Session Counter</td>
                                <td>-</td>
                                <td>-</td>
                                <td>Tracks the number of visits to the default landing page.</td>
                                <td>-</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="instructions">
                <h3 class="">Testing Instructions</h3>
                <ul class="list-group list-group-flush">
                    <li><b>Service: </b>Click on the "Try It" button to test the Distance Matrix Service.</li>
                    <li>The service will return the driving distance and time between the two cities you enter.</li>
                    <li>User credentials are stored in the app_data folder of the web app (server side)</li>
                    <li>Client-side information is stored in cookies for the "Remember Me" functionality.</li>
                    <li><b>To test the cookie:</b> Return to home (default) page and attempt sign up and login with remember me checked.</li>
                    <li>Once logged in, restart your browser to see credentials being autofilled when attempting to log in.</li>
            </div>
        </div>
    </form>

</body>
</html>