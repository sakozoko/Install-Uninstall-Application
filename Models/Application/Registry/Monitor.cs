using System;
using System.Management;
using Microsoft.Win32;

namespace URApplication.Models.Application.Registry
{
    public class RegistryWatcher : ManagementEventWatcher
    {
        public RegistryWatcher(RegistryKey hive, string keyPath)
        {
            Hive = hive;
            KeyPath = keyPath;
            if (hive.OpenSubKey(keyPath) != null)
            {
                if (hive.Name.Contains("CURRENT_USER")) EventCurrentUser();
                var queryString =
                    $@"SELECT * FROM RegistryTreeChangeEvent WHERE Hive = '{Hive.Name}' AND RootPath = '{KeyPath}' ";
                var query = new WqlEventQuery
                {
                    QueryString = queryString,
                    EventClassName = "RegistryTreeChangeEvent",
                    WithinInterval = new TimeSpan(0, 0, 0, 1)
                };
                Query = query;

                EventArrived += RegistryWatcher_EventArrived;
            }
            else
            {
                var message = $@"The registry key {hive.Name}\{keyPath} does not exist";
                throw new ArgumentException(message);
            }
        }

        public RegistryKey Hive { get; private set; }
        public string KeyPath { get; private set; }
        private static string SidUser { get; set; }

        private void EventCurrentUser()
        {
            if (SidUser is null)
            {
                using var keyFolderWithUserName = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Volatile Environment");
                var username = (string)keyFolderWithUserName.GetValue("USERNAME");
                using var keyFolderWithUserProfiles =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                        "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\ProfileList");
                foreach (var keyName in keyFolderWithUserProfiles.GetSubKeyNames())
                {
                    using var keyFolderWithUserProfile = keyFolderWithUserProfiles.OpenSubKey(keyName);
                    if (!(keyFolderWithUserProfile.GetValue("ProfileImagePath") as string).Contains(username ?? string.Empty)) continue;
                    SidUser = keyName + "\\\\";
                    break;
                }
            }
            Hive = Microsoft.Win32.Registry.Users;
            KeyPath = SidUser + KeyPath;
        }

        public event EventHandler<TreeChangeEventArgs> RegistryTreeChangeEvent;
        private void RegistryWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (RegistryTreeChangeEvent == null) return;
            var args = new TreeChangeEventArgs(e.NewEvent);
            RegistryTreeChangeEvent(sender, args);
        }
    }



}