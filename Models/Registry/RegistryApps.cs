using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
namespace URApplication.Models.Registry
{
    public class RegistryApps : ICreatorApplications
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
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(UninstallInLocalMachineWow6432),
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(UninstallInLocalMachine),
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(UninstallInCurrentUser)
            };
            var applicationModelCollection = GetAppsFromRegistry(registryKeys);
            return applicationModelCollection;
        }
     
        private static ObservableCollection<ApplicationModel> GetAppsFromRegistry(List<RegistryKey> registryKeys)
        {
            var applicationModelCollection = new ObservableCollection<ApplicationModel>();
            foreach (RegistryKey keyParent in registryKeys)
            {
                using (keyParent)
                {
                    var subKeyNames = keyParent?.GetSubKeyNames();
                    foreach (var subKeyName in subKeyNames)
                    {
                        using var key = keyParent.OpenSubKey(subKeyName);
                        if (!Validation.Application(key)) continue;

                        var path = (string)key.GetValue("DisplayIcon");

                        var myBitmap = key.GetValue("WindowsInstaller") is not null && (int)key.GetValue("WindowsInstaller")==1
                            ? AppIcon.GetIconAppInstaller((string)key.GetValue("DisplayName"))
                            : AppIcon.GetIconApp(path);
    
                        applicationModelCollection.Add(new ApplicationModel()
                        {
                            Name = (string)key.GetValue("DisplayName"),
                            IconSource = Imaging.CreateBitmapSourceFromHBitmap(myBitmap.GetHbitmap(), IntPtr.Zero,
                                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
                            Version = (string)key.GetValue("DisplayVersion"),
                            InstallDate = Validation.ToDateTime((string)key.GetValue("InstallDate")),
                            Publisher = (string)key.GetValue("Publisher"),
                            Weight = Validation.ToWeight(Convert.ToDouble(key.GetValue("EstimatedSize"))),
                            UninstallCmd = (string)key.GetValue("UninstallString")
                        });
                    }
                }
            }
            return applicationModelCollection;
        }

    }
}