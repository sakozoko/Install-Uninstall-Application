using System;

namespace URApplication.Models
{
    public class StringReplace
    {
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

        public static string ReplaceUninstallCmd(string cmd)
        {
            return "\"" + cmd + "\"";
        }
    }
}