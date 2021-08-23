using System;
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

        
        public static void TryReplaceAppIconPath(string path, out string pathIcon, out int number, out string ext)
        {
            if (path is not null)
            {
                if (path.Contains(','))
                {
                    var subPath = path.Split(',', 2, StringSplitOptions.TrimEntries);
                    pathIcon = subPath[0].Trim('"');
                    number = int.Parse(subPath[1].Trim('"'));
                    ext = subPath[0].Split('.', 2, StringSplitOptions.TrimEntries)[1];
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

        public static string TryReplaceUninstallCmd(string cmd)
        {
            return "\"" + cmd + "\"";
        }
    }
}