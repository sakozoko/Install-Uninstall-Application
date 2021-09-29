using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using Microsoft.Win32;
using URApplication.Models.Application.Creator.Property;

namespace URApplication.Models.Application.Creator
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
            model.Name = AppProperties.DisplayName.Get(key);
            model.IconSource = AppProperties.IconSource.Get(key);
            model.Version = AppProperties.DisplayVersion.Get(key);
            model.InstallDate = AppProperties.InstallDate.Get(key);
            model.Publisher = AppProperties.Publisher.Get(key);
            model.Weight = AppProperties.EstimatedSize.Get(key);
            model.UninstallCmd = AppProperties.UninstallString.Get(key);
            model.ModifyPath = AppProperties.ModifyPath.Get(key);
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