using System;
using System.IO;

namespace NDAWebPortal.Core.ClassLib
{
    public static class ActivityLogger
    {
        public static void Log(Exception exception)
        {
            Log("ERROR>>" + exception.Source, "[" + exception + "]" + exception.Message);
        }

        public static void Log(string messageType, string message)
        {
            while (!LogEngine(messageType, message.Length >= 2000 ? message.Substring(0, 2000) : message))
            { }
        }

        private static bool LogEngine(string messageType, string message)
        {
            try
            {
                try
                {
                    var location = Setting.LogFolder;

                    var dirInfo = new DirectoryInfo(location);
                    if (!dirInfo.Exists)
                    {
                        Directory.CreateDirectory(location);
                    }

                    if (!File.Exists(location + "EdBox_Logs.txt"))
                    {
                        using (var sw = File.CreateText(location + "EdBox_Logs.txt"))
                        {
                            sw.WriteLine("[" + messageType + "] " + DateTime.Now + ": " + message);
                            sw.Close();
                        }
                    }
                    else
                        using (var sw = File.AppendText(location + "EdBox_Logs.txt"))
                        {
                            var fI = new FileInfo(location + "EdBox_Logs.txt");
                            if (fI.Length <= Setting.LogRollOver)
                            {
                                sw.WriteLine("[" + messageType + "] " + DateTime.Now + ": " + message);
                                sw.Close();
                            }
                            else
                            {
                                sw.Close();
                                File.Move(location + "EdBox_Logs.txt", location + "EdBox_Logs_" + DateTime.Now.ToString("yyyymmddhhMMsstt"));

                                using (var sw3 = File.CreateText(location + "EdBox_Logs.txt"))
                                {
                                    sw3.WriteLine("[" + messageType + "] " + DateTime.Now + ": " + message);
                                    sw3.Close();
                                }
                            }
                        }
                }
                catch (Exception)
                {
                    throw;
                }

                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
