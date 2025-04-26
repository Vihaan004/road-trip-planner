<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Hashing.aspx.cs" Inherits="TempWebApp.Hashing" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Password Hashing Demo</title>
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
            padding: 10px;
            background-color: #e9ecef;
            border-radius: 5px;
            word-wrap: break-word;
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
        .textbox {
            padding: 8px;
            width: 80%;
            border: 1px solid #ced4da;
            border-radius: 4px;
        }
        .validation-message {
            color: #dc3545;
            font-size: 14px;
            margin-top: 5px;
        }
        .valid-message {
            color: #28a745;
            font-size: 14px;
            margin-top: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Password Hashing Demo</h2>
            
            <p>Enter any text to see its secure SHA-256 hash value.</p>

            <div class="input-group">
                <asp:Label ID="Label1" runat="server" Text="Text to Hash: "></asp:Label>
                <asp:TextBox ID="txtInput" runat="server" CssClass="textbox" placeholder="Enter text to hash"></asp:TextBox>
            </div>
            
            <div>
                <asp:Label ID="lblValidation" runat="server" CssClass="validation-message" Visible="false"></asp:Label>
            </div>

            <asp:Button ID="btnHash" runat="server" Text="Generate Hash" CssClass="button" OnClick="btnHash_Click" />

            <div class="result" runat="server" id="resultPanel" visible="false">
                <h4>Hash Result:</h4>
                <asp:Label ID="lblHashResult" runat="server" Text=""></asp:Label>
                
                <div style="margin-top: 15px;">
                    <h4>Password Validation:</h4>
                    <asp:Label ID="lblPasswordValidation" runat="server" Text=""></asp:Label>
                </div>
            </div>
            
            <div>
                <asp:Button ID="btnBackToServiceDirectory" runat="server" Text="Back to Service Directory" CssClass="back-button" OnClick="btnBackToServiceDirectory_Click" />
            </div>
        </div>
    </form>
</body>
</html>