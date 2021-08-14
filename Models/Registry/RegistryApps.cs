using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management;
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
            foreach (var keyParent in registryKeys)
                using (keyParent)
                {
                    var subKeyNames = keyParent?.GetSubKeyNames();
                    foreach (var subKeyName in subKeyNames)
                    {
                        using var key = keyParent.OpenSubKey(subKeyName);
                        if (!Validation.Application(key)) continue;
                        var path = (string)key.GetValue("DisplayIcon");
                        var myBitmap = key.GetValue("WindowsInstaller") is not null &&
                                       (int)key.GetValue("WindowsInstaller") == 1
                            ? AppIcon.GetIconAppInstaller((string)key.GetValue("DisplayName"))
                            : AppIcon.GetIconApp(path);

                        applicationModelCollection.Add(new ApplicationModel
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
                        string pth = key.Name.Substring(key.Name.IndexOf('\\'));
                        pth = pth.Trim();
                        pth = pth.Replace("\\", "\\\\");
                        pth = pth.Substring(2);
                        string hivename = key.Name.Substring(0, key.Name.IndexOf('\\'));
                        RegistryKey hive = Microsoft.Win32.Registry.LocalMachine;
                        if (!hivename.Contains("LOCAL"))
                        {
                             hive = Microsoft.Win32.Registry.Users;
                            pth = "S-1-5-21-881236052-2142875276-63049226-1001\\\\" + pth;
                            
                        }

                        StartMonitor(hive, pth);
                    }
                }

            return applicationModelCollection;
        }

        private static readonly List<RegistryWatcher> watcher = new List<RegistryWatcher>();
        static bool StartMonitor(RegistryKey hive, string keyPath)
        {
            try
            {
                watcher.Add(new RegistryWatcher(hive, keyPath));
            }

            // The constructor of RegistryWatcher may throw a SecurityException when
            // the key to monitor does not exist. 
            catch (System.ArgumentException ex)
            {
                return false;
            }

            // The constructor of RegistryWatcher may throw a SecurityException when
            // current user does not have the permission to access the key to monitor. 
            catch (System.Security.SecurityException)
            {

            }

            try
            {

                // Set up the handler that will handle the change event.
                watcher[^1].RegistryKeyChangeEvent += new EventHandler<RegistryKeyChangeEventArgs>(
                    watcher_RegistryKeyChangeEvent);

                // Start listening for events.
                watcher[^1].Start();
                return true;
            }
            catch (System.Runtime.InteropServices.COMException comException)
            {
                return false;
            }
            catch (ManagementException managementException)
            {
                return false;
            }

        }
        static void watcher_RegistryKeyChangeEvent(object sender, RegistryKeyChangeEventArgs e)
        {
            string newEventMessage = string.Format(@"{0} The key {1}\{2} changed",
                e.TimeCreated.ToLocalTime(), e.Hive, e.RootPath);
            throw new Exception("ZBS");
        }
    }
}