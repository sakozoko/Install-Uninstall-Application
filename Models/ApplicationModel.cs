using System;

namespace URApplication.Models
{
    public class ApplicationModel
    {
        public string Name { get; set; } = "Warspear";
        public string Icon { get; set; } = "some.jpg";
        public string Version { get; set; } = "1.0.0";
        public string Publisher { get; set; } = "Somebody";
        public string InstallDate { get; set; } = "12.11.2012";
        public double Weight { get; set; } = 120312D;
    }
}