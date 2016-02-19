using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Net.Mail;
using System.Configuration;

namespace UniMail
{
  public class Template
  {
    public string FromName;
    public string FromEmail;
    public string ToEmail;
    public string Subject;
    public bool IsHTML;

    public string Body;
    public Encoding Encoding = Encoding.Unicode;

    public string PlainTextBody;
    public Encoding AlternateEncoding = Encoding.ASCII;

    public IDictionary<object, object> Data = new Dictionary<object, object>();
    public List<Attachment> AttachItem = new List<Attachment>();

    public List<LinkedResource> LinkedResourceCollection = new List<LinkedResource>();

    public Dictionary<string, string> EmailHeader = new Dictionary<string, string>();

    /// <summary>
    /// Constructs template form given ABSOLUTE file path.
    /// First line in template is subject. 
    /// Template should look like:
    /// Subject: Hello there
    /// 
    /// Message text goes here
    /// </summary>
    /// <param name="filename">File containing template</param>
    /// <returns></returns>

    public Template(string filename)
      : this(filename, "mail")
    {
    }
    public Template(StringBuilder email)
      : this(email, "mail")
    {
    }
    public Template(string filename, bool IsHTML)
      : this(filename, IsHTML, "mail")
    {
    }
    public Template(StringBuilder email, string subject, bool IsHTML)
      : this(email, subject, IsHTML, "mail")
    {
    }

    public Template(string filename, string mainconfigname)
    {
      MailerConfiguration readConfig = ConfigurationHandler.GetSection(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MailConfig"], mainconfigname);
      FromName = readConfig.SenderName;
      FromEmail = readConfig.SenderEmail;
      try
      {
        System.IO.TextReader fileReader = new System.IO.StreamReader(filename);
        StringBuilder templateBuilder = new StringBuilder("");
        Subject = fileReader.ReadLine();
        Subject.Remove(0, 8);
        string line;
        // Read lines from the file until the end of the file is reached.
        while ((line = fileReader.ReadLine()) != null)
        {
          templateBuilder.AppendLine(line);
        }
        this.Body = templateBuilder.ToString();
      }
      catch (System.Exception) { }
    }

    public Template(StringBuilder email, string mainconfigname)
    {
      MailerConfiguration readConfig = ConfigurationHandler.GetSection(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MailConfig"], mainconfigname);

      FromName = readConfig.SenderName;
      FromEmail = readConfig.SenderEmail;

      try
      {
        int len = email.ToString().IndexOf(".");
        Subject = email.ToString().Substring(0, len);
        email.Remove(0, len + 1);
        Body = email.ToString();
      }
      catch (System.Exception) { }
    }

    public Template(StringBuilder email, string subject, bool IsHTML, string mainconfigname)
    {
      MailerConfiguration readConfig = ConfigurationHandler.GetSection(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MailConfig"], mainconfigname);

      FromName = readConfig.SenderName;
      FromEmail = readConfig.SenderEmail;

      Subject = subject;
      Body = email.ToString();

      this.IsHTML = IsHTML;
    }

    public Template(string filename, bool is_html, string mainconfigname)
    {
      MailerConfiguration readConfig = ConfigurationHandler.GetSection(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MailConfig"], mainconfigname);
      FromName = readConfig.SenderName;
      FromEmail = readConfig.SenderEmail;

      IsHTML = is_html;

      try
      {
        System.IO.TextReader fileReader = new System.IO.StreamReader(filename);
        StringBuilder templateBuilder = new StringBuilder("");

        Subject = fileReader.ReadLine();
        Subject.Remove(0, 8);

        string line;
        // Read lines from the file until the end of 
        // the file is reached.
        while ((line = fileReader.ReadLine()) != null)
        {
          templateBuilder.AppendLine(line);
        }

        this.Body = templateBuilder.ToString();
      }
      catch (System.Exception) { }
    }

    public Template()
    {
      //MailerConfiguration readConfig = (MailerConfiguration)System.Configuration.ConfigurationManager.GetSection("mail");
      MailerConfiguration readConfig = ConfigurationHandler.GetSection(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MailConfig"], "mail");
      FromName = readConfig.SenderName;
      FromEmail = readConfig.SenderEmail;
    }

    public Template(string filename, string directory, string mainconfigname)
    {
      MailerConfiguration readConfig = ConfigurationHandler.GetSection(directory, mainconfigname);
      FromName = readConfig.SenderName;
      FromEmail = readConfig.SenderEmail;
      try
      {
        System.IO.TextReader fileReader = new System.IO.StreamReader(filename);
        StringBuilder templateBuilder = new StringBuilder("");
        Subject = fileReader.ReadLine();
        string line;
        while ((line = fileReader.ReadLine()) != null)
        {
          templateBuilder.AppendLine(line);
        }
        this.Body = templateBuilder.ToString();
      }
      catch (System.Exception) { }
    }

    public MailMessage Render()
    {
      MailMessage message = new MailMessage(new MailAddress(FromEmail, FromName), new MailAddress(ToEmail));
      message.Subject = Subject;
      message.Body = Body;
      message.IsBodyHtml = IsHTML;
      message.BodyEncoding = Encoding;

      if (AttachItem != null)
      {
        foreach (Attachment at in AttachItem)
          message.Attachments.Add(at);
      }
      StringBuilder AlternateBody = new StringBuilder(PlainTextBody);
      if (Data != null && Data.Count() > 0)
      {
        foreach (object mask in Data.Keys)
        {
          if (!Data.ContainsKey(mask)) continue;
          message.Body = message.Body.Replace(mask.ToString(), Data[mask.ToString()].ToString());
          message.Subject = message.Subject.Replace(mask.ToString(), Data[mask.ToString()].ToString());
          if (AlternateBody.Length > 0)
            AlternateBody.Replace(mask.ToString(), Data[mask.ToString()].ToString());
        }
      }
      if (AlternateBody.Length > 0)
      {
        message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(AlternateBody.ToString(), AlternateEncoding, "text/plain"));
      }
      return message;
    }

    public MailMessage RenderHTML()
    {
      MailMessage message = new MailMessage(new MailAddress(FromEmail, FromName), new MailAddress(ToEmail));
      if (Data != null)
      {
        foreach (object mask in Data.Keys)
        {
          if (Data[mask.ToString()] == null) continue;
          Body = Body.Replace(mask.ToString(), Data[mask.ToString()].ToString());
          Subject = Subject.Replace(mask.ToString(), Data[mask.ToString()].ToString());
        }
      }
      message.Subject = Subject;
      AlternateView avHTML = AlternateView.CreateAlternateViewFromString(Body, Encoding, MediaTypeNames.Text.Html);
      foreach (LinkedResource lr in LinkedResourceCollection) avHTML.LinkedResources.Add(lr);

      message.AlternateViews.Add(avHTML);

      if (EmailHeader.Count > 0)
      {
        foreach (var nvc in EmailHeader)
        {
          message.Headers.Add(nvc.Key, nvc.Value);
        }
      }
      if (AttachItem != null)
      {
        foreach (Attachment at in AttachItem)
          message.Attachments.Add(at);
      }
      return message;
    }

    public static MailMessage RenderPlainHTMLEmail(string mainconfigname, string subject, string htmlText, string plainText, string email, string nameto, Dictionary<string, string> emailHeaders)
    {
      MailerConfiguration readConfig = ConfigurationHandler.GetSection(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MailConfig"], mainconfigname);
      MailMessage message = new MailMessage(new MailAddress(readConfig.SenderEmail, readConfig.SenderName), String.IsNullOrEmpty(nameto) ? new MailAddress(email) : new MailAddress(email, nameto));
      try
      {
        message.Subject = subject;
        message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plainText, Encoding.UTF8, MediaTypeNames.Text.Plain));
        message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlText, Encoding.UTF8, MediaTypeNames.Text.Html));
        message.Headers.Add("List-Unsubscribe", "<mailto:" + readConfig.SenderEmail + "?subject=" + email + ">");
        if (emailHeaders != null && emailHeaders.Count > 0)
          foreach (var nvc in emailHeaders)
            message.Headers.Add(nvc.Key, nvc.Value);
      }
      catch (Exception)
      {

      }
      return message;
    }

    public static MailMessage RenderPlainHTMLEmail(string subject, string htmlText, string plainText, string email, string nameto, Dictionary<string, string> emailHeaders)
    {
      return RenderPlainHTMLEmail("mail", subject, htmlText, plainText, email, nameto, emailHeaders);
    }

  }
}
