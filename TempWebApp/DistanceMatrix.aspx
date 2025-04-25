<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DistanceMatrix.aspx.cs" Inherits="TempWebApp.DistanceMatrix" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Distance & Drive Time Finder</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            text-align: center;
            margin: 0;
            padding: 0;
        }
        .container {
            width: 50%;
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
            font-size: 18px;
            font-weight: bold;
            margin-top: 20px;
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Distance & Drive Time Finder</h2>
            
            <p>Enter two cities to find the driving distance and estimated travel time.</p>

            <div class="input-group">
                <asp:Label ID="Label1" runat="server" Text="City 1: "></asp:Label>
                <asp:TextBox ID="city1input" runat="server" CssClass="textbox"></asp:TextBox>
            </div>
            
            <div class="input-group">
                <asp:Label ID="Label2" runat="server" Text="City 2: "></asp:Label>
                <asp:TextBox ID="city2input" runat="server" CssClass="textbox"></asp:TextBox>
            </div>
            
            <asp:Button ID="Button1" runat="server" Text="Find Distance & Time" CssClass="button" OnClick="Button1_Click" />

            <div class="result">
                <asp:Label ID="LabelDistance" runat="server" Text=""></asp:Label>
                <br />
                <asp:Label ID="LabelTime" runat="server" Text=""></asp:Label>
            </div>
            
            <div>
                <asp:Button ID="btnBackToServiceDirectory" runat="server" Text="Back to Service Directory" CssClass="back-button" OnClick="btnBackToServiceDirectory_Click" />
            </div>
        </div>
    </form>
</body>
</html>
