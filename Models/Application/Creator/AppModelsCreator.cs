using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace URApplication.Models.Application.Creator
{
    public class AppModelsCreator : ICreatorApplications
    {
        private const string UninstallInLocalMachineWow6432 =
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        private const string UninstallInLocalMachine =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        private const string UninstallInCurrentUser =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";


        public ObservableCollection<ApplicationModel> GetApps()
        {
            var registryKeys = new List<RegistryKey>
            {
                Registry.LocalMachine.OpenSubKey(UninstallInLocalMachineWow6432),
                Registry.LocalMachine.OpenSubKey(UninstallInLocalMachine),
                Registry.CurrentUser.OpenSubKey(UninstallInCurrentUser)
            };

            var applicationModelCollection = AppCreator.GetAppsFromRegistry(registryKeys);
            return applicationModelCollection;
        }
    }
}