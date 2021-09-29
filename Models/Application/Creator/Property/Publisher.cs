using System;
using Microsoft.Win32;

namespace URApplication.Models.Application.Creator.Property
{
    public partial class AppProperties
    {
    public class Publisher
    {
        public static string Get(RegistryKey hKey)
        {
            string res;
            try
            {
                res = hKey.GetValue(nameof(Publisher)).ToString();
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