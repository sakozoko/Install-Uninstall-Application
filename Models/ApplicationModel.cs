using System.Windows.Media;

namespace URApplication.Models
{
    public class ApplicationModel
    {
        public string Name { get; set; }
        public ImageSource IconSource { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public string InstallDate { get; set; }
        public string Weight { get; set; }
        public string UninstallCmd { get; set; }
    }
}