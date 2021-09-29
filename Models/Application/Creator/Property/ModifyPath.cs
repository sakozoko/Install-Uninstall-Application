using System;
using Microsoft.Win32;

namespace URApplication.Models.Application.Creator.Property
{
    public partial class AppProperties
    {
    public class ModifyPath
    {
        public static string Get(RegistryKey hKey)
        {
            string res;
            try
            {
                res = hKey.GetValue(nameof(ModifyPath)).ToString();
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