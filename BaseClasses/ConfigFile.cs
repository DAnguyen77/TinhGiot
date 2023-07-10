using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class ConfigFile
    {
        public static string EmailNoreply
        {
            get
            {
                try
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["EmailOut"] != null)
                        return System.Configuration.ConfigurationManager.AppSettings["EmailOut"].ToString();
                }
                catch { }

                return "noreply@tinhgiot.com";
            }
        }

        public static string EmailPassword
        {
            get
            {
                try
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["PasswordOut"] != null)
                        return System.Configuration.ConfigurationManager.AppSettings["PasswordOut"].ToString();
                }
                catch { }

                return "aucfyehzgY0dqmi-wobBvjk4prn@xs";
            }
        }

        public static string EmailDisplayName
        {
            get
            {
                try
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["EmailDisplayName"] != null)
                        return System.Configuration.ConfigurationManager.AppSettings["EmailDisplayName"].ToString();
                }
                catch { }

                return "";
            }
        }

        public static string MailServer
        {
            get
            {
                try
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["MailServer"] != null)
                        return System.Configuration.ConfigurationManager.AppSettings["MailServer"].ToString();
                }
                catch { }

                return "mail.tinhgiot.com";
            }
        }

        public static string MailPort
        {
            get
            {
                try
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["MailPort"] != null)
                        return System.Configuration.ConfigurationManager.AppSettings["MailPort"].ToString();
                }
                catch { }

                return "2525";
            }
        }

    }
}
