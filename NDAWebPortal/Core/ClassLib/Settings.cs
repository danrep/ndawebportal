using System;
using System.Configuration;

namespace NDAWebPortal.Core.ClassLib
{
    public static class Setting
    {
        public static dynamic MailSettings()
        {
            return new
            {
                SmtpMailFrom = ConfigurationManager.AppSettings["LogRollOver"] ?? "info@NDAWebPortal.com",
                SmtpMailHead = ConfigurationManager.AppSettings["SmtpMailHead"] ?? "NDA Web Portal Messaging",
                SmtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com",
                SmtpUsername = ConfigurationManager.AppSettings["SmtpUsername"] ?? "notification@codesistance.com",
                SmtpPassword = ConfigurationManager.AppSettings["SmtpPassword"] ?? "skyRunn3r",
                SmtpSslMode = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSslMode"] ?? "true"),
                SmtpServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpServerPort"] ?? "587")
            };
        }

        public static string LogFolder => ConfigurationManager.AppSettings["LogFolder"] ?? "\\";
        public static long LogRollOver => Convert.ToInt64(ConfigurationManager.AppSettings["LogRollOver"] ?? "100000");
    }
}
