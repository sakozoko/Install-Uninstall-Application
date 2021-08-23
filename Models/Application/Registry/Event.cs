using Microsoft.Win32;

namespace URApplication.Models.Application.Registry
{
    public class RegistryEventCreator
    {
        public static void DivideRegistryKeyByRegistryKeyAndPath(RegistryKey inKey, out RegistryKey outKey, out string path)
        {
            outKey = Microsoft.Win32.Registry.LocalMachine;
            if (!inKey.Name.Contains("LOCAL_MACHINE"))
            {
                outKey = Microsoft.Win32.Registry.CurrentUser;
            }
            path = inKey.Name[inKey.Name.IndexOf('\\')..].Trim().Replace("\\","\\\\")[2..];
        }
    }
}