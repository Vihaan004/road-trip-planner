using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;

public class CaptchaHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        string captchaText = GenerateCaptcha(6);
        context.Session["CaptchaCode"] = captchaText;

        using (Bitmap bmp = new Bitmap(240, 80))
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.LightBlue);

            Random rand = new Random();

            // Draw CAPTCHA characters
            using (Font font = new Font("Arial", 28, FontStyle.Bold))
            {
                for (int i = 0; i < captchaText.Length; i++)
                {
                    float angle = rand.Next(-30, 30);
                    g.TranslateTransform(30 + i * 25, 30);
                    g.RotateTransform(angle);
                    g.DrawString(captchaText[i].ToString(), font, Brushes.DarkBlue, 0, 0);
                    g.ResetTransform();
                }
            }

            // Draw noise lines
            for (int i = 0; i < 5; i++)
            {
                int x1 = rand.Next(bmp.Width);
                int y1 = rand.Next(bmp.Height);
                int x2 = rand.Next(bmp.Width);
                int y2 = rand.Next(bmp.Height);
                g.DrawLine(new Pen(Color.Gray, 1), x1, y1, x2, y2);
            }

            // Add random dots
            for (int i = 0; i < 50; i++)
            {
                int x = rand.Next(bmp.Width);
                int y = rand.Next(bmp.Height);
                bmp.SetPixel(x, y, Color.FromArgb(rand.Next()));
            }

            context.Response.ContentType = "image/png";
            bmp.Save(context.Response.OutputStream, ImageFormat.Png);
        }
    }

    public bool IsReusable => false;

    private string GenerateCaptcha(int length)
    {
        string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        Random rand = new Random();
        char[] result = new char[length];
        for (int i = 0; i < length; i++)
            result[i] = chars[rand.Next(chars.Length)];
        return new string(result);
    }
}