using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace URApplication.Models.Application
{
    public class AppIcon
    {
        private const string InstallerPathIcons = @"SOFTWARE\Classes\Installer\Products";
        private const string PathOfMsInstaller = "\\msi.dll,2";

        private static readonly string DefaultPathAppIcon =
            Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) + "\\imageres.dll,11";

        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion,
            out IntPtr piSmallVersion, int amountIcons);

        private static Bitmap Extract(string file, int number, bool largeIcon)
        {
            ExtractIconEx(file, number, out var large, out var small, 1);
            try
            {
                return Icon.FromHandle(largeIcon ? large : small).ToBitmap();
            }
            catch
            {
                StringReplace.TryReplaceAppIconPath(DefaultPathAppIcon, out file, out number, out var ext);
                return GetIconOrDefault(file, number, ext, null);
            }
        }

        public static Bitmap GetIconAppInstaller(string displayName)
        {
            var defaultPathLibrary = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) + PathOfMsInstaller;
            using var iconKey =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(InstallerPathIcons);
            var names = iconKey.GetSubKeyNames();
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
            StringReplace.TryReplaceAppIconPath(path, out var pathIcon, out var number, out var ext);
            return GetIconOrDefault(pathIcon, number, ext, defaultPathLibrary);
        }


        private static Bitmap GetIconOrDefault(string path, int number, string ext, string defaultPathLibrary)
        {
            try
            {
                if (!ext.Contains("DLL", StringComparison.OrdinalIgnoreCase))
                    return Icon.ExtractAssociatedIcon(path).ToBitmap();
                var icon = new Bitmap(Extract(path, number, true));
                return icon;
            }
            catch
            {
                defaultPathLibrary ??= DefaultPathAppIcon;
                StringReplace.TryReplaceAppIconPath(defaultPathLibrary, out path, out number, out ext);
                return GetIconOrDefault(path, number, ext, null);
            }
        }
    }
}