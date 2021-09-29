using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace URApplication.Models.Application.Creator.Property
{
    public partial class AppProperties
    {
    public class IconSource
    {
        public static ImageSource Get(RegistryKey hKey)
        {
            var path = (string)hKey.GetValue("DisplayIcon");
            var myBitmap = hKey.GetValue("WindowsInstaller") is not null && (int)hKey.GetValue("WindowsInstaller") == 1
                ? AppIcon.GetIconAppInstaller((string)hKey.GetValue("DisplayName"))
                : AppIcon.GetIconApp(path);
            return Imaging.CreateBitmapSourceFromHBitmap(myBitmap.GetHbitmap(), IntPtr.Zero,
                Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

    }
    }
}