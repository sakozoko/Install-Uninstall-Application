using System;
using Microsoft.Win32;

namespace URApplication.Models.Application.Creator.Property
{
    public partial class AppProperties
    {
    public class DisplayName
    {
        public static string Get(RegistryKey hKey)
        {
            string res;
            try
            {
                res = hKey.GetValue(nameof(DisplayName)).ToString();
                if (res is null or "")
                    res = "Unnamed";
            }
            catch (NullReferenceException)
            {
                res = "";
            }

            return res;
        }
    }
    }
}