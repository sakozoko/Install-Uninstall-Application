﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace URApplication.Models.Registry
{
    public class AppIcon
    {
        private const string InstallerPathIcons = @"SOFTWARE\Classes\Installer\Products";
        private static readonly string DefaultPathAppIcon = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) + "\\imageres.dll,11";
        private const string PathOfMsInstaller = "\\msi.dll,2";
        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

        private static Icon Extract(string file, int number, bool largeIcon)
        {
            ExtractIconEx(file, number, out IntPtr large, out IntPtr small, 1);
            try
            {
                return Icon.FromHandle(largeIcon ? large : small);
            }
            catch
            {
                return null;
            }
        }

        public static Bitmap GetIconAppInstaller(string displayName)
        {
            var defaultPathLibrary = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) + PathOfMsInstaller;
            using var iconKey =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(InstallerPathIcons);
            string[] names = iconKey.GetSubKeyNames();
            foreach (var name in names)
            {
                using var installerKey = iconKey.OpenSubKey(name);
                if (displayName != (string)installerKey.GetValue("ProductName")) continue;
                var path = (string)installerKey.GetValue("ProductIcon");
                return GetIconApp(path, defaultPathLibrary);
            }
            return GetIconApp(null, defaultPathLibrary);
        }



        public static Bitmap GetIconApp(string path)
        { 
            return GetIconApp(path, DefaultPathAppIcon);
        }

        private static Bitmap GetIconApp(string path, string defaultPathLibrary)
        {
            Validation.TryReplaceAppIconPath(path, out string pathIcon, out int number, out string ext);
            return GetIconOrDefault(pathIcon, number, ext, defaultPathLibrary);
        }


        private static Bitmap GetIconOrDefault(string path, int number, string ext, string defaultPathLibrary)
        {
            try
            {
                if (ext.Contains("DLL", StringComparison.OrdinalIgnoreCase))
                    return new Bitmap(Extract(path, number, true)
                        .ToBitmap());
                return Icon.ExtractAssociatedIcon(path).ToBitmap();
            }
            catch
            {
                defaultPathLibrary ??= DefaultPathAppIcon;
                Validation.TryReplaceAppIconPath(defaultPathLibrary, out path, out number, out ext);
                return GetIconOrDefault(path, number, ext, null);

            }

        }
    }
}