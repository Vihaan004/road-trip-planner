<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Places.aspx.cs" Inherits="TempWebApp.Places" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Nearby Places Finder</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            text-align: center;
            margin: 0;
            padding: 0;
        }
        .container {
            width: 60%;
            margin: 50px auto;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
            background-color: #f9f9f9;
        }
        .input-group {
            margin: 15px 0;
        }
        .button {
            margin-top: 10px;
            padding: 8px 15px;
            font-size: 16px;
            cursor: pointer;
        }
        .result {
            font-size: 16px;
            margin-top: 20px;
            text-align: left;
            padding: 10px;
        }
        .back-button {
            margin-top: 20px;
            padding: 6px 12px;
            background-color: #6c757d;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            font-size: 14px;
        }
        .back-button:hover {
            background-color: #5a6268;
        }
        .result-title {
            font-weight: bold;
            margin-top: 10px;
        }
        .result-list {
            list-style-type: none;
            padding-left: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Nearby Places Finder</h2>
            
            <p>Enter a location to find nearby petrol pumps and restaurants.</p>

            <div class="input-group">
                <asp:Label ID="Label1" runat="server" Text="Location: "></asp:Label>
                <asp:TextBox ID="locationInput" runat="server" CssClass="textbox" placeholder="e.g. San Francisco, CA"></asp:TextBox>
            </div>
            
            <asp:Button ID="ButtonSearch" runat="server" Text="Find Nearby Places" CssClass="button" OnClick="ButtonSearch_Click" />

            <div class="result">
                <asp:Panel ID="ResultsPanel" runat="server" Visible="false">
                    <div class="result-title">Petrol Pumps:</div>
                    <asp:BulletedList ID="PetrolPumpsList" runat="server" CssClass="result-list"></asp:BulletedList>
                    
                    <div class="result-title">Restaurants:</div>
                    <asp:BulletedList ID="RestaurantsList" runat="server" CssClass="result-list"></asp:BulletedList>
                </asp:Panel>
            </div>
            
            <div>
                <asp:Button ID="btnBackToServiceDirectory" runat="server" Text="Back to Service Directory" CssClass="back-button" OnClick="btnBackToServiceDirectory_Click" />
            </div>
        </div>
    </form>
</body>
</html>