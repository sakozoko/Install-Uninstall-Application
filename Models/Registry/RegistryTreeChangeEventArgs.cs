using System;
using System.Management;

namespace URApplication.Models.Registry
{
    public class RegistryTreeChangeEventArgs : EventArgs
    {
        public RegistryTreeChangeEventArgs(ManagementBaseObject arrivedEvent)
        {
            Hive = arrivedEvent.Properties[nameof(Hive)].Value as string;
            RootPath = arrivedEvent.Properties[nameof(RootPath)].Value as string;
        }
        public string Hive { get; set; }
        public string RootPath { get; set; }
    }
}