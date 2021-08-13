using System.Diagnostics;

namespace URApplication.Models.Registry
{
    public class AppUninstaller
    {
        public static void TryUninstall(string cmd)
        {
            cmd = Validation.TryReplaceUninstallCmd(cmd);
            Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd",
                Arguments =  "/c "+cmd,
                CreateNoWindow = true
            });

        }
    }
}