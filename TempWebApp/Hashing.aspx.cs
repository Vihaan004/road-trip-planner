using System;
using System.Web.UI;
using PasswordHashLibrary;

namespace TempWebApp
{
    public partial class Hashing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No initialization needed
        }

        protected void btnHash_Click(object sender, EventArgs e)
        {
            string inputText = txtInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(inputText))
            {
                lblValidation.Text = "Please enter some text to hash.";
                lblValidation.Visible = true;
                resultPanel.Visible = false;
                return;
            }

            // Generate the hash using our Password Hasher library
            string hashedText = PasswordHasher.HashPassword(inputText);

            // Check if the input would be a valid password according to our rules
            bool isValidPassword = PasswordHasher.ValidatePassword(inputText);
            
            // Display the results
            lblValidation.Visible = false;
            lblHashResult.Text = hashedText;
            
            if (isValidPassword)
            {
                lblPasswordValidation.Text = "✓ This input meets basic password requirements (contains both letters and numbers).";
                lblPasswordValidation.CssClass = "valid-message";
            }
            else
            {
                lblPasswordValidation.Text = "✗ This input does NOT meet password requirements. A valid password must contain both letters and numbers.";
                lblPasswordValidation.CssClass = "validation-message";
            }
            
            resultPanel.Visible = true;
        }

        protected void btnBackToServiceDirectory_Click(object sender, EventArgs e)
        {
            // Redirect back to the Service Directory page
            Response.Redirect("~/ServiceDirectory.aspx");
        }
    }
}