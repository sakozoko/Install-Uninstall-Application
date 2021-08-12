using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
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
        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);
        public static Icon Extract(string file, int number, bool largeIcon)
        {
            IntPtr large;
            IntPtr small;
            ExtractIconEx(file, number, out large, out small, 1);
            try
            {
                return Icon.FromHandle(largeIcon ? large : small);
            }
            catch
            {
                return null;
            }
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
                        if (Validation.Application(key))
                        {
                            string path = (string)key.GetValue("DisplayIcon");
                            Bitmap myBitmap = new Bitmap(Extract("C:\\Windows\\System32\\imageres.dll", 11, true)
                                .ToBitmap());
                            if(path is not null)
                            {
                                path = path.Trim('"');
                                //Icon icon = new(path);
                                if (path.EndsWith(".EXE", StringComparison.OrdinalIgnoreCase) ||
                                    path.EndsWith(".ICO", StringComparison.OrdinalIgnoreCase))
                                {
                                    try
                                    {
                                        myBitmap = new Bitmap(Icon.ExtractAssociatedIcon(path)
                                            .ToBitmap());
                                    }
                                    catch (System.IO.FileNotFoundException)
                                    {
                                        myBitmap = new Bitmap(
                                                Extract("C:\\Windows\\System32\\imageres.dll", 11, true)
                                                    .ToBitmap());
                                    }
                                }
                                else if (path[..^2].EndsWith(".DLL", StringComparison.OrdinalIgnoreCase))
                                {
                                    int de = path.LastIndexOf(',')+1;
                                    int index = int.Parse(path[de..]);
                                    path = path[..^2];
                                    try
                                    {
                                        myBitmap = new Bitmap(Extract(path, index, true)
                                            .ToBitmap());
                                    }
                                    catch (System.IO.FileNotFoundException)
                                    {
                                        myBitmap = new Bitmap(Extract("C:\\Windows\\System32\\imageres.dll", 11, true)
                                            .ToBitmap());
                                    }
                                }else if (path[..^2].EndsWith(".EXE", StringComparison.OrdinalIgnoreCase))
                                {
                                    path = path[..^2];
                                    try
                                    {
                                        myBitmap = new Bitmap(Icon.ExtractAssociatedIcon(path)
                                            .ToBitmap());
                                    }
                                    catch (System.IO.FileNotFoundException)
                                    {
                                        myBitmap = new Bitmap(Extract("C:\\Windows\\System32\\imageres.dll", 11, true)
                                            .ToBitmap());
                                    }
                                }

                            }
                            else if(key.GetValue("WindowsInstaller") is not null)
                            {
                                if(keyParent.Name.Contains("HKEY_LOCAL_MACHINE"))
                                {
                                    using (var iconKey =
                                        Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                                            @"SOFTWARE\Classes\Installer\Products"))
                                    {
                                        string[] names = iconKey.GetSubKeyNames();
                                        foreach (var name in names)
                                        {
                                            using var myiconKey = iconKey.OpenSubKey(name);
                                            if ((string)key.GetValue("DisplayName") !=
                                                (string)myiconKey.GetValue("ProductName")) continue;
                                            try
                                            {
                                                if (myiconKey.GetValue("ProductIcon") is not null)
                                                    myBitmap = new Bitmap(Icon
                                                        .ExtractAssociatedIcon(
                                                            (string)myiconKey.GetValue("ProductIcon"))
                                                        .ToBitmap());
                                                break;
                                            }
                                            catch (System.IO.FileNotFoundException)
                                            {
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    using (var iconKey =
                                        Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                                            @"SOFTWARE\Classes\Installer\Products"))
                                    {
                                        string[] names = iconKey.GetSubKeyNames();
                                        foreach (var name in names)
                                        {
                                            using var myiconKey = iconKey.OpenSubKey(name);
                                            if ((string)key.GetValue("DisplayName") !=
                                                (string)myiconKey.GetValue("ProductName")) continue;
                                            try
                                            {
                                                if (myiconKey.GetValue("ProductIcon") is not null)
                                                    myBitmap = new Bitmap(Icon
                                                        .ExtractAssociatedIcon(
                                                            (string)myiconKey.GetValue("ProductIcon"))
                                                        .ToBitmap());
                                                break;
                                            }
                                            catch (System.IO.FileNotFoundException)
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                            applicationModelCollection.Add(new ApplicationModel()
                            {
                                Name = (string)key.GetValue("DisplayName"),
                                Icon = myBitmap,
                                IconSource = Imaging.CreateBitmapSourceFromHBitmap(myBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
                                ,
                                Version = (string)key.GetValue("DisplayVersion"),
                                InstallDate = Validation.ToDateTime((string)key.GetValue("InstallDate")),
                                Publisher = (string)key.GetValue("Publisher"),
                                Weight = Validation.ToWeight(Convert.ToDouble(key.GetValue("EstimatedSize")))
                            });
                        }
                    }
                }
            }
            return applicationModelCollection;
        }

    }
}