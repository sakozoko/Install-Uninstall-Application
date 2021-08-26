using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            foreach (var keyParent in registryKeys.Where(keyParent => keyParent is not null))
                using (keyParent)
                {
                    var subKeyNames = keyParent.GetSubKeyNames();
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
            ApplicationModel newInstance = new();
            AppModelInitializeFromRegistryKey(newInstance, key);
            return newInstance;
        }

        public static void AppModelInitializeFromRegistryKey(ApplicationModel model, RegistryKey key)
        {
            var path = (string)key.GetValue("DisplayIcon");
            var myBitmap = key.GetValue("WindowsInstaller") is not null && (int)key.GetValue("WindowsInstaller") == 1
                ? AppIcon.GetIconAppInstaller((string)key.GetValue("DisplayName"))
                : AppIcon.GetIconApp(path);
            model.Name = (string)key.GetValue("DisplayName");
            model.IconSource = Imaging.CreateBitmapSourceFromHBitmap(myBitmap.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            model.Version = (string)key.GetValue("DisplayVersion");
            model.InstallDate = (string)key.GetValue("InstallDate");
            model.Publisher = (string)key.GetValue("Publisher");
            model.Weight = key.GetValue("EstimatedSize") is not null
                ? int.Parse(key.GetValue("EstimatedSize").ToString() ?? "0")
                : 0;
            model.UninstallCmd = (string)key.GetValue("UninstallString");
            model.ModifyPath = (string)key.GetValue("ModifyPath");
        }
    }
}