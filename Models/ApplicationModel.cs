using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace URApplication.Models
{
    public class ApplicationModel
    {
        public string Name { get; set; }
        public Bitmap Icon { get; set; }
        public ImageSource IconSource { get; set; }

        public string Version { get; set; }
        public string Publisher { get; set; } 
        public string InstallDate { get; set; }
        public string Weight { get; set; }
    }
}