using System.Collections.ObjectModel;
using URApplication.Models.ApplicationModels;

namespace URApplication.Models.Registry
{
    public class RegistryApps : ICreatorApplication
    {
        private const string UninstallInLocalMachineWow6432 = 
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        private const string UninstallInLocalMachine =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        private const string UninstallInCurrentUser =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public ObservableCollection<ApplicationModel> GetApps()
        {
            var i = 0;
            string[] subKeyNames;
            string path = UninstallInLocalMachineWow6432;
            ObservableCollection<ApplicationModel> applicationModelCollection = new ObservableCollection<ApplicationModel>();
            do
            {
                using (var keyParent = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(path))
                {
                    subKeyNames = keyParent?.GetSubKeyNames();
                }

                applicationModelCollection =
                    GetAppsFromLocalMachine(subKeyNames,path, applicationModelCollection);
                path = UninstallInLocalMachine;
                i++;
            } while (i < 2);
            
            using (var keyParent = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(UninstallInCurrentUser))
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
                    applicationModelCollection.Add(new AppModel()
                    {
                        Name = (string)key.GetValue("DisplayName"),
                    });

                }
            }
            return applicationModelCollection;
        }
        private static ObservableCollection<ApplicationModel> GetAppsFromLocalMachine(string[] folderNames, string path,
            ObservableCollection<ApplicationModel> applicationModelCollection)
        {
            foreach (var subKeyName in folderNames)
            {
                using var key =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(path + @"\" + subKeyName);
                {
                    if (key.GetValue("DisplayName") is null || key.GetValue("ParentKeyName") is not null) continue;
                    if (key.GetValue("SystemComponent") is not null &&
                        (int)key.GetValue("SystemComponent") == 1) continue;
                    applicationModelCollection.Add(new AppModel()
                    {
                        Name = (string)key.GetValue("DisplayName"),
                        InstallDate = (string)key.GetValue("InstallDate")
                    });

                }
            }

            return applicationModelCollection;
        }
    }
}