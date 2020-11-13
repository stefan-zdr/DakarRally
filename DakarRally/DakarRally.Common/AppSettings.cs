using DakarRally.Common.Helpers;
using System.ComponentModel;
using System.Configuration;

namespace DakarRally.Common
{
    public static class AppSettings
    {
        public static bool Debug { get; private set; }
        public static string LocalPath { get; private set; }
        public static string DatabasePath => LocalPathHelper.GetWithLocalPath(Get<string>(nameof(DatabasePath)));

        private static T Get<T>(string key)
        {
            string setting = ConfigurationManager.AppSettings[key];
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromInvariantString(setting);
        }

        public static void SetDebug(bool debug, string localPath)
        {
            Debug = debug;
            LocalPath = localPath;
        }
    }
}
