using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace URApplication.Models.Registry
{
    public static class Validation
    {
        public static bool Application(RegistryKey key)
        {
            return key.GetValue("DisplayName") is not null && key.GetValue("ParentKeyName") is null &&
                   (key.GetValue("SystemComponent") is null ||
                    (int)key.GetValue("SystemComponent") != 1);
        }

        public static string ToWeight(double value)
        {
            string str = (value / 1024) switch
            {
                > 0 and < 1000 => $"{(float)(value / 1024):0.##}" + " MB",
                >= 1000 => $"{(float)(value / 1048576):0.##}" + " GB",
                _ => value == 0d ? "" : $"{value:0.##}" + " KB"
            };

            return str;
        }

        public static string ToDateTime(string value)
        {
            if (value is null)
                return DateTime.Now.ToString("dd.MM.yy");
            if (DateTime.TryParse(value, out var dateTime))
            {
                return dateTime.ToString("dd.MM.yy");
            }

            string newValue = value[0..4]+"."+value[4..6]+"."+value[6..^0];
            return DateTime.TryParse(newValue, out dateTime) ? dateTime.ToString("dd.MM.yy") : value;
        }

        public static void TryReplaceAppIconPath(string path, out string pathIcon, out int number, out string ext)
        {
            if (path is not null)
            {
                if (path.Contains(','))
                {
                    var pathes = path.Split(',', 2, StringSplitOptions.TrimEntries);
                    pathIcon = pathes[0].Trim('"');
                    number = int.Parse(pathes[1].Trim('"'));
                    ext = pathes[0].Split('.', 2, StringSplitOptions.TrimEntries)[1];
                }
                else
                {
                    pathIcon = path.Trim('"');
                    number = 0;
                    ext = "EXE";
                }
            }
            else
            {
                pathIcon = null;
                number = 0;
                ext = "";
            }
        }
    }
}