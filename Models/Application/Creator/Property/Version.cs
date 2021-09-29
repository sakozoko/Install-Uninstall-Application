using System;
using Microsoft.Win32;

namespace URApplication.Models.Application.Creator.Property
{
    public partial class AppProperties
    {
          public class DisplayVersion
          {
              public static string Get(RegistryKey hKey)
              {
                  
                  string res;
                  try
                  {
                      res = hKey.GetValue(nameof(DisplayVersion)).ToString();
                  }
                  catch (InvalidCastException)
                  {
                      res = "";
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