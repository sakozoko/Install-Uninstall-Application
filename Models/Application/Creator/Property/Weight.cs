using System;
using Microsoft.Win32;

namespace URApplication.Models.Application.Creator.Property
{
    public partial class AppProperties
    {
        public static class EstimatedSize
        {
            public static int Get(RegistryKey hKey)
            {
                int res;
                try
                {
                    if (hKey.GetValue(nameof(EstimatedSize)) is null or "")
                        throw new ArgumentNullException();
                    // ReSharper disable once AssignNullToNotNullAttribute
                    res = int.Parse(hKey.GetValue(nameof(EstimatedSize)).ToString());
                }
                catch (ArgumentNullException)
                {
                    res = 0;
                }
                catch (NullReferenceException)
                {
                    res = 0;
                }

                return res;
            }
        }
    }
}