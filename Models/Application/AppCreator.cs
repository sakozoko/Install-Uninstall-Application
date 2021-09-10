using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;

namespace URApplication.Models.Application
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

                        DivideRegistryKeyByRegistryKeyAndPath(key, out var hive,
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
            model.Name = (key.GetValue("DisplayName") is string displayName) ? displayName : null;
            model.IconSource = Imaging.CreateBitmapSourceFromHBitmap(myBitmap.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            model.Version = (key.GetValue("DisplayVersion") is string displayVersion) ? displayVersion : null;
            model.InstallDate = (key.GetValue("InstallDate") is string installDate) ? installDate : null;
            model.Publisher = (key.GetValue("Publisher") is string publisher) ? publisher : null;
            model.Weight = key.GetValue("EstimatedSize") is not null
                ? int.Parse(key.GetValue("EstimatedSize").ToString() ?? "0")
                : 0;
            model.UninstallCmd = (string)key.GetValue("UninstallString");
            model.ModifyPath = (string)key.GetValue("ModifyPath");
        }

        private static void DivideRegistryKeyByRegistryKeyAndPath(RegistryKey inKey, out RegistryKey outKey,
            out string path)
        {
            outKey = Registry.LocalMachine;
            if (!inKey.Name.Contains("LOCAL_MACHINE")) outKey = Registry.CurrentUser;
            path = inKey.Name[inKey.Name.IndexOf('\\')..].Trim().Replace("\\", "\\\\")[2..];
        }
    }
}