using System.Diagnostics;

namespace URApplication.Models.Application
{
    public class Uninstaller
    {
        public static void TryUninstall(string cmd)
        {
            cmd = StringReplace.ReplaceUninstallCmd(cmd);
            Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd",
                Arguments =  "/c "+cmd,
                CreateNoWindow = true
            });

        }
    }
}