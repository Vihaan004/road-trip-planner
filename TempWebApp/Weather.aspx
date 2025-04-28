<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Weather.aspx.cs" Inherits="TempWebApp.Weather" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Weather Service</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background-color: #f8f9fa;
            padding: 20px;
        }
        .container {
            background-color: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
            margin-top: 20px;
        }
        .header {
            text-align: center;
            margin-bottom: 30px;
        }
        .form-group {
            margin-bottom: 15px;
        }
        .result-panel {
            margin-top: 20px;
            padding: 15px;
            border-radius: 5px;
            background-color: #f8f9fa;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <h2>Weather Service</h2>
                <p>Get weather information for two locations</p>
            </div>

            <div class="form-group">
                <label for="location1Input">Location 1:</label>
                <asp:TextBox ID="location1Input" runat="server" CssClass="form-control" placeholder="Enter first location"></asp:TextBox>
            </div>

            <div class="form-group">
                <label for="location2Input">Location 2:</label>
                <asp:TextBox ID="location2Input" runat="server" CssClass="form-control" placeholder="Enter second location"></asp:TextBox>
            </div>

            <div class="text-center">
                <asp:Button ID="btnGetWeather" runat="server" Text="Get Weather" CssClass="btn btn-primary" OnClick="btnGetWeather_Click" />
                <asp:Button ID="btnBackToServiceDirectory" runat="server" Text="Back to Service Directory" CssClass="btn btn-secondary" OnClick="btnBackToServiceDirectory_Click" />
            </div>

            <asp:Panel ID="ResultsPanel" runat="server" CssClass="result-panel" Visible="false">
                <h4>Weather Results:</h4>
                <asp:Label ID="WeatherResults" runat="server"></asp:Label>
            </asp:Panel>
        </div>
    </form>
</body>
</html>