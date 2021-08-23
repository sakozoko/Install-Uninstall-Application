using System;
using System.Management;

namespace URApplication.Models.Application.Registry
{
    public class TreeChangeEventArgs : EventArgs
    {
        public TreeChangeEventArgs(ManagementBaseObject arrivedEvent)
        {
            Hive = arrivedEvent.Properties[nameof(Hive)].Value as string;
            RootPath = arrivedEvent.Properties[nameof(RootPath)].Value as string;
        }
        public string Hive { get; set; }
        public string RootPath { get; set; }
    }
}