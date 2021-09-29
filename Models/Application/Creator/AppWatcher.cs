using System.Collections.ObjectModel;
using System.Windows.Threading;
using Microsoft.Win32;
using RegistryMonitor;

namespace URApplication.Models.Application.Creator
{
    public class AppWatcher : RegistryWatcher
    {
        public AppWatcher(RegistryKey hive, string keyPath, ApplicationModel model) : base(hive, keyPath)
        {
            Model = model;
        }

        public static Dispatcher Dispatcher { get; set; }
        public ApplicationModel Model { get; }
        public static ObservableCollection<ApplicationModel> Models { get; set; }

        /// <summary>
        /// </summary>
        /// <returns>Return false if need remove the Model from collection, true - if collection update complete</returns>
        public bool TryUpdateModel()
        {
            return Dispatcher.Invoke(() =>
            {
                using var key = Hive.OpenSubKey(KeyPath);
                if (key is null) return false;
                AppCreator.AppModelInitializeFromRegistryKey(Model, key);
                return true;
            });
        }
    }
}