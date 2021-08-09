using System;

namespace URApplication.Models
{
    internal class ApplicationModel
    {
        public string Name { get; } = "Warspear";
        public string Icon { get; } = "some.jpg";
        public string Version { get; } = "1.0.0";
        public string Publisher { get; } = "Somebody";
        public string InstallDate { get; } = "12.11.2012";
        public double Weight { get; } = 120312D;
    }
}