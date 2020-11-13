using System.IO;

namespace DakarRally.Common.Helpers
{
    public class LocalPathHelper
    {
        public static string GetFixedPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            path = path.Replace('/', '\\');
            if (!path.Contains(":"))
            {
                path = path.TrimStart('\\');
            }
            if (!path.EndsWith("\\") && !Path.HasExtension(path))
            {
                path = $"{path}\\";
            }
            return path;
        }

        public static string GetWithLocalPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            path = GetFixedPath(path);
            if (path != null && !path.Contains(":"))
            {
                path = $"{AppSettings.LocalPath}{path}";
            }

            return path;
        }
    }
}
