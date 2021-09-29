using System.Diagnostics;
using URApplication.Models.Application.Creator;

namespace URApplication.Models.Application
{
    public class Uninstaller
    {
        public static void TryUninstall(string cmd)
        {
            cmd = StringReplace.ReplaceUninstallCmd(cmd);
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/c " + cmd,
                CreateNoWindow = true
            });
        }

        public static void TryModify(string cmd)
        {
            TryUninstall(cmd);
        }
    }
}