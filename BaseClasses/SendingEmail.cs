using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Globalization;
using System.Net.Sockets;
using System.IO;
using System.Drawing;

namespace BaseClasses
{
    public class SendingEmail
    {
        private enum SMTPResponse : int
        {
            CONNECT_SUCCESS = 220,
            GENERIC_SUCCESS = 250,
            DATA_SUCCESS = 354,
            QUIT_SUCCESS = 221
        }
        private string From = "";
        private string[] To;
        private string[] CC;
        private string Subject;
        private string MsgBody;
        private string[] Attachments;
        private bool invalid = false;
        private MailMessage oMailMsg;
        public SendingEmail(string from, string[] To, string[] CC, string Subject, string MsgBody, string[] Attachments)
        {
            this.From = from;
            this.To = To;
            this.CC = CC;
            this.Subject = Subject;
            this.MsgBody = MsgBody;
            this.Attachments = Attachments;

            oMailMsg = new MailMessage();

            oMailMsg.From = new MailAddress(this.From, ConfigFile.EmailDisplayName);
            oMailMsg.Subject = this.Subject;
            oMailMsg.SubjectEncoding = Encoding.UTF8;
            oMailMsg.IsBodyHtml = true;
            oMailMsg.Body = this.MsgBody;
            oMailMsg.BodyEncoding = Encoding.UTF8;
            PopulateTo();
            PopulateCC();
            PopulateAttachments();
        }

        public SendingEmail(string from, string[] To, string[] CC, string Subject, string MsgBody, string[] Attachments, string[] LocationAttachments, bool inline)
        {
            this.From = from;
            this.To = To;
            this.CC = CC;
            this.Subject = Subject;
            this.MsgBody = MsgBody;
            this.Attachments = Attachments;
            oMailMsg = new MailMessage();
            oMailMsg.From = new MailAddress(this.From, ConfigFile.EmailDisplayName);
            oMailMsg.Subject = this.Subject;
            oMailMsg.SubjectEncoding = Encoding.UTF8;
            oMailMsg.IsBodyHtml = true;
            oMailMsg.Body = this.MsgBody;
            oMailMsg.BodyEncoding = Encoding.UTF8;
            PopulateTo();
            PopulateCC();
            PopulateAttachments();

        }


        public SendingEmail(string from, string[] To, string[] CC, string Subject, AlternateView htmlView)
        {

            this.From = from;
            this.To = To;
            this.CC = CC;
            this.Subject = Subject;
            oMailMsg = new MailMessage();
            oMailMsg.From = new MailAddress(this.From, ConfigFile.EmailDisplayName);
            oMailMsg.Subject = this.Subject;
            oMailMsg.SubjectEncoding = Encoding.UTF8;
            oMailMsg.IsBodyHtml = true;
            oMailMsg.AlternateViews.Add(htmlView);
            oMailMsg.BodyEncoding = Encoding.UTF8;
            PopulateTo();
            PopulateCC();

        }

        public SendingEmail()
        {

        }

        private void PopulateTo()
        {
            if (To != null)
            {
                foreach (string to in To)
                {
                    if (to.Length > 0)
                        oMailMsg.To.Add(new MailAddress(to));
                }
            }
        }

        private void PopulateCC()
        {
            if (CC != null)
            {
                foreach (string cc in CC)
                {
                    if (cc.Length > 0)
                        oMailMsg.CC.Add(new MailAddress(cc));
                }
            }
        }

        private void PopulateAttachments()
        {
            if (Attachments != null)
            {
                foreach (string attachment in Attachments)
                {
                    oMailMsg.Attachments.Add(new Attachment(attachment));

                }
            }
        }

        private void PopulateAttachments(string[] LocationAttachments, bool inline)
        {
            if (Attachments != null)
            {
                for (int count = 0; count < LocationAttachments.Length; count++)
                {
                    //    Attachment oAttachment = new Attachment(Attachments[count], System.Net.Mime.TransferEncoding.Base64);
                    //  oMailMsg.Attachments.Add(new Attachment(attachment));
                }
            }
        }

        public bool isValidEmail(string email)
        {
            invalid = false;
            if (string.IsNullOrEmpty(email))
                return false;
            if (!email.Contains("@"))
                return false;
            email = Regex.Replace(email, @"(@)(.+)$", this.DomainMapper);
            if (invalid)
                return false;
            invalid = Regex.IsMatch(email,
              @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
              @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
              RegexOptions.IgnoreCase);

            string[] host = (email.Split('@'));
            string hostname = host[1];
            return invalid;
        }

        private void Senddata(Socket s, string msg)
        {
            byte[] _msg = Encoding.ASCII.GetBytes(msg);
            s.Send(_msg, 0, _msg.Length, SocketFlags.None);
        }
        private bool Check_Response(Socket s, SMTPResponse response_expected)
        {
            string sResponse;
            int response;
            byte[] bytes = new byte[1024];
            while (s.Available == 0)
            {
                System.Threading.Thread.Sleep(100);
            }

            s.Receive(bytes, 0, s.Available, SocketFlags.None);
            sResponse = Encoding.ASCII.GetString(bytes);
            response = Convert.ToInt32(sResponse.Substring(0, 3));
            if (response != (int)response_expected)
                return false;
            return true;
        }


        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

        public string Send()
        {
            string str = "Please check Mail Server Detail";

            SmtpClient client = new SmtpClient(ConfigFile.MailServer);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Port = int.Parse(ConfigFile.MailPort);
            //  client.EnableSsl = true;
            client.Credentials = new NetworkCredential(ConfigFile.EmailNoreply, ConfigFile.EmailPassword);//  
            try
            {
                client.Send(oMailMsg);
                str = "succeed";
            }
            catch (SmtpException ex)
            {
                str = ex.InnerException.Message;//.Message;
            }
            client.Dispose();
            oMailMsg.Dispose();
            return str;
        }

        public static string ToBase64String(string filename, string imageFormat)
        {
            string base64String = string.Empty;
            StreamReader streamReader = new StreamReader(filename);
            Bitmap bmp = new Bitmap(streamReader.BaseStream);

            MemoryStream memoryStream = new MemoryStream();
            bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

            memoryStream.Position = 0;
            byte[] byteBuffer = memoryStream.ToArray();

            streamReader.Close();
            memoryStream.Close();

            base64String = Convert.ToBase64String(byteBuffer);
            byteBuffer = null;

            return base64String;
        }

        public static string ImageTag(string filename, string imageFormat)
        {
            string imgTag = string.Empty;
            string base64String = string.Empty;

            base64String = ToBase64String(filename, imageFormat);

            imgTag = string.Format("<img src=\"data:image/{0};base64,{1}\"></img>", imageFormat.ToString(), base64String);

            return imgTag;
        }

        public string SendWithoutCredential()
        {
            string str = "Please check Mail Server Detail";

            SmtpClient client = new SmtpClient();
            client.Host =ConfigFile.MailServer;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = CredentialCache.DefaultNetworkCredentials;
            try
            {
                client.Send(oMailMsg);
                str = "succeed";
            }
            catch (SmtpException ex)
            {
                str = ex.Message;
            }
            oMailMsg.Dispose();
            return str;
        }

    }

}
