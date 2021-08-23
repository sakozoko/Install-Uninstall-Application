using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;

namespace URApplication.Models.Application.Registry
{
    public class AppCreator
    {
        public static ObservableCollection<ApplicationModel> GetAppsFromRegistry(List<RegistryKey> registryKeys)
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

                        var newInstance = CreateAppModelFromRegistryKey(key);
                        applicationModelCollection.Add(newInstance);

                        RegistryEventCreator.DivideRegistryKeyByRegistryKeyAndPath(key, out var hive,
                            out var outPath);
                        newInstance.Watcher = new AppWatcher(hive, outPath, newInstance);
                    }
                }

            AppWatcher.Dispatcher = Dispatcher.CurrentDispatcher;
            AppWatcher.Models = applicationModelCollection;

            return applicationModelCollection;
        }

        private static ApplicationModel CreateAppModelFromRegistryKey(RegistryKey key)
        {
            var path = (string)key.GetValue("DisplayIcon");
            var myBitmap = key.GetValue("WindowsInstaller") is not null && (int)key.GetValue("WindowsInstaller") == 1
                ? AppIcon.GetIconAppInstaller((string)key.GetValue("DisplayName"))
                : AppIcon.GetIconApp(path);

            var newInstance = new ApplicationModel
            {
                Name = (string)key.GetValue("DisplayName"),
                IconSource = Imaging.CreateBitmapSourceFromHBitmap(myBitmap.GetHbitmap(), IntPtr.Zero,
                    Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
                Version = (string)key.GetValue("DisplayVersion"),
                InstallDate = (string)key.GetValue("InstallDate"),
                Publisher = (string)key.GetValue("Publisher"),
                Weight = key.GetValue("EstimatedSize") is not null
                    ? int.Parse(key.GetValue("EstimatedSize").ToString() ?? "0")
                    : 0,
                UninstallCmd = (string)key.GetValue("UninstallString")
            };
            return newInstance;
        }
    }
}