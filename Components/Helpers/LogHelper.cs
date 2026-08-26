using ImageCombinerChannelExtractor.Components.Enums;
using System.IO;
using System.Text;

namespace ImageCombinerChannelExtractor.Components.Helpers
{
    public static class LogHelper
    {
        private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "error.log");
        private static readonly object LockObj = new();

        public static void Log(NotificationTypeEnum notifType, string message)
        {
            WriteToFile($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {EnumFriendlyNameHelper.GetLogName(notifType)} {message}");
        }

        public static void Log(Exception ex, string contextMessage)
        {
            StringBuilder sb = new StringBuilder();
            string header = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {EnumFriendlyNameHelper.GetLogName(NotificationTypeEnum.Error)} {contextMessage}";

            sb.AppendLine(header);
            sb.AppendLine($"Type: {ex.GetType().FullName}");
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"Stack Trace:\n{ex.StackTrace}");

            if (ex.InnerException != null)
            {
                sb.AppendLine($"Inner Exception Exception: {ex.InnerException.GetType().FullName}");
                sb.AppendLine($"Inner Exception Message: {ex.InnerException.Message}");
                sb.AppendLine($"Inner Exception Stack Trace:\n{ex.InnerException.StackTrace}");
            }
            sb.AppendLine(new string('-', 50));
            WriteToFile(sb.ToString());
        }

        private static void WriteToFile(string text)
        {
            lock (LockObj)
            {
                File.AppendAllText(LogFilePath, text + Environment.NewLine);
            }
        }
    }
}
