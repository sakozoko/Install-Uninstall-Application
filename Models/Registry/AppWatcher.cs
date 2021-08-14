using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;

namespace URApplication.Models.Registry
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
        /// 
        /// </summary>
        /// <returns>Return false if need remove the Model from collection, true - if collection update complete</returns>
        public bool TryUpdateModel()
        {
            return Dispatcher.Invoke(() =>
            {
                using var key = Hive.OpenSubKey(KeyPath);
                if (key is null) return false;
                var path = (string)key.GetValue("DisplayIcon");
                var myBitmap = key.GetValue("WindowsInstaller") is not null &&
                               (int)key.GetValue("WindowsInstaller") == 1
                    ? AppIcon.GetIconAppInstaller((string)key.GetValue("DisplayName"))
                    : AppIcon.GetIconApp(path);
                Model.Name = (string)key.GetValue("DisplayName");
                Model.IconSource = Imaging.CreateBitmapSourceFromHBitmap(myBitmap.GetHbitmap(), IntPtr.Zero,
                    Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                Model.Version = (string)key.GetValue("DisplayVersion");
                Model.InstallDate = Validation.ToDateTime((string)key.GetValue("InstallDate"));
                Model.Publisher = (string)key.GetValue("Publisher");
                Model.Weight = Validation.ToWeight(Convert.ToDouble(key.GetValue("EstimatedSize")));
                Model.UninstallCmd = (string)key.GetValue("UninstallString");
                return true;
            });
        }
    }


}