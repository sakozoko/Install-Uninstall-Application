using System;
using System.Globalization;
using Microsoft.Win32;
using RegistryMonitor;

namespace URApplication.Models.Application.Creator.Property
{
    public partial class AppProperties
    {
    public class InstallDate
    {
        public static string Get(RegistryKey hKey)
        {
            string res;
            try
            {
                if (hKey is null)
                    throw new ArgumentNullException(nameof(hKey));
                res = hKey.GetValue(nameof(InstallDate)).ToString();
                if (res is "")
                    throw new NullReferenceException();
            }
            catch (InvalidCastException)
            {
                res = RegistryWatcher.GetRegistryKeyLastWriteTime(hKey).ToString("dd.MM.yy", new DateTimeFormatInfo());
            }
            catch (NullReferenceException)
            {
                res = RegistryWatcher.GetRegistryKeyLastWriteTime(hKey).ToString("dd.MM.yy", new DateTimeFormatInfo());
            }


            return res;
        }
    }
    }
}