using System;

namespace URApplication.Models
{
    public class ApplicationModel
    {
        public string Name { get; set; } 
        public string Icon { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; } 
        public string InstallDate { get; set; }
        public string Weight { get; set; }
        public bool Appx { get; set; }
    }
}