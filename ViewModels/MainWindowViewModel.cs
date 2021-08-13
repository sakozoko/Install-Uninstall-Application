using System.Collections.ObjectModel;
using System.Windows.Input;
using URApplication.Commands;
using URApplication.Models;
using URApplication.Models.Registry;
using URApplication.ViewModels.Base;

namespace URApplication.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel()
        {
            UninstallApplicationCommand = new LambdaCommand(OnUninstallApplicationCommandExecute, CanUninstallApplicationCommandExecute);
            ICreatorApplications creator = new RegistryApps();
            Rows = new ObservableCollection<ApplicationModel>(creator.GetApps());
        }

        #region InitializeCommand

        #region UninstallApplicationCommand

        public ICommand UninstallApplicationCommand { get; }

        public bool CanUninstallApplicationCommandExecute(object obj)
        {
            return obj is not null;
        }

        private void OnUninstallApplicationCommandExecute(object obj)
        {
            AppUninstaller.TryUninstall((string)obj);
        }

        #endregion

        #endregion
        #region Rows

        public ObservableCollection<ApplicationModel> Rows { get; set; }

        #endregion

        #region SelectedRow

        private ApplicationModel _selectedModel;

        public ApplicationModel SelectedRow
        {
            get => _selectedModel;
            set
            {
                _selectedModel = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}