using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.VisualBasic;
using URApplication.Models;
namespace URApplication.Models.Registry
{
    public class Class1
    {
        private const string UninstallInLocalMachineWow6432 = 
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        private const string UninstallInLocalMachine =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        private const string UniStallInCurrentUser =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public static ObservableCollection<ApplicationModel> GetApps()
        {
            var applicationModelCollection = new ObservableCollection<ApplicationModel>();
            string[] subKeyNames;
            using (var keyParent = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(UninstallInLocalMachineWow6432))
            {
                subKeyNames = keyParent?.GetSubKeyNames();
            }
            foreach (var subKeyName in subKeyNames)
            {
                using var key =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(UninstallInLocalMachineWow6432 + @"\" + subKeyName);
                {
                    if (key.GetValue("DisplayName") is null || key.GetValue("ParentKeyName") is not null) continue;
                    if (key.GetValue("SystemComponent") is not null &&
                        (int)key.GetValue("SystemComponent") == 1)continue;
                    applicationModelCollection.Add(new ApplicationModel()
                        {
                            Name = (string)key.GetValue("DisplayName"),
                            InstallDate = (string)key.GetValue("InstallDate")
                        });

                }
            }


            using (var keyParent = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(UninstallInLocalMachine))
            {
                subKeyNames = keyParent?.GetSubKeyNames();
            }
            foreach (var subKeyName in subKeyNames)
            {
                using var key =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(UninstallInLocalMachine + @"\" + subKeyName);
                {
                    if (key.GetValue("DisplayName") is null || key.GetValue("ParentKeyName") is not null) continue;
                    if (key.GetValue("SystemComponent") is not null &&
                        (int)key.GetValue("SystemComponent") == 1) continue;
                    applicationModelCollection.Add(new ApplicationModel()
                    {
                        Name = (string)key.GetValue("DisplayName"),
                    });

                }
            }

            using (var keyParent = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(UniStallInCurrentUser))
            {
                subKeyNames = keyParent?.GetSubKeyNames();
            }
            foreach (var subKeyName in subKeyNames)
            {
                using var key =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(UninstallInLocalMachine + @"\" + subKeyName);
                {
                    if (key.GetValue("DisplayName") is null || key.GetValue("ParentKeyName") is not null) continue;
                    if (key.GetValue("SystemComponent") is not null &&
                        (int)key.GetValue("SystemComponent") == 1) continue;
                    applicationModelCollection.Add(new ApplicationModel()
                    {
                        Name = (string)key.GetValue("DisplayName"),
                    });

                }
            }
            return applicationModelCollection;
        }
    }
}