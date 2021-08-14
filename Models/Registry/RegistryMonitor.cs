using System;
using System.Management;
using Microsoft.Win32;

namespace URApplication.Models.Registry
{
    internal class RegistryWatcher : ManagementEventWatcher, IDisposable
    {
        public RegistryWatcher(RegistryKey hive, string keyPath)
        {
            Hive = hive;
            KeyPath = keyPath;
            KeyToMonitor = hive.OpenSubKey(keyPath);

            if (KeyToMonitor != null)
            {
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

        public RegistryKey Hive { get; }
        public string KeyPath { get; }
        public RegistryKey KeyToMonitor { get; }

        /// <summary>
        ///     Dispose the RegistryKey.
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            KeyToMonitor?.Dispose();
        }

        public event EventHandler<RegistryKeyChangeEventArgs> RegistryKeyChangeEvent;

        private void RegistryWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (RegistryKeyChangeEvent == null) return;
            // Get RegistryKeyChangeEventArgs from EventArrivedEventArgs.NewEvent.Properties.
            var args = new RegistryKeyChangeEventArgs(e.NewEvent);

            // Raise the event handler. 
            RegistryKeyChangeEvent(sender, args);
        }
    }


    internal class RegistryKeyChangeEventArgs : EventArgs
    {
        public RegistryKeyChangeEventArgs(ManagementBaseObject arrivedEvent)
        {
            Hive = arrivedEvent.Properties[nameof(Hive)].Value as string;
            RootPath = arrivedEvent.Properties[nameof(RootPath)].Value as string;
            TimeCreated = new DateTime(
                (long)(ulong)arrivedEvent.Properties[nameof(TimeCreated)].Value,
                DateTimeKind.Utc).AddYears(1600);
        }
        public string Hive { get; set; }
        public string RootPath { get; set; }
        public DateTime TimeCreated { get; set; }
    }
}