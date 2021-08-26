using System.Windows.Media;
using URApplication.ViewModels.Base;

namespace URApplication.Models.Application
{
    public class ApplicationModel : ViewModel
    {
        #region Properties

        #region Name

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        #endregion

        #region IconSource

        public ImageSource IconSource { get; set; }

        #endregion

        #region Version

        private string _version;

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));
            }
        }

        #endregion

        #region Publisher

        private string _publisher;

        public string Publisher
        {
            get => _publisher;
            set
            {
                _publisher = value;
                OnPropertyChanged(nameof(Publisher));
            }
        }

        #endregion

        #region InstallDate

        private string _installDate;

        public string InstallDate
        {
            get => _installDate;
            set
            {
                _installDate = value;
                OnPropertyChanged(nameof(InstallDate));
            }
        }

        #endregion

        #region Weight

        private int _weight;

        public int Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(Weight));
            }
        }

        #endregion

        #region UninstallCmd

        private string _uninstallCmd;

        public string UninstallCmd
        {
            get => _uninstallCmd;
            set
            {
                _uninstallCmd = value;
                OnPropertyChanged(nameof(UninstallCmd));
            }
        }
        #endregion

        #region ModifyPath

        private string _modifyPath;

        public string ModifyPath
        {
            get => _modifyPath;
            set
            {
                _modifyPath = value;
                OnPropertyChanged(nameof(ModifyPath));
            }
        }

        #endregion

        #region Watcher

        public AppWatcher Watcher { get; set; }

        #endregion

        #endregion
    }
}