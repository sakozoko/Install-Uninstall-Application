using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
            UninstallApplicationCommand = new LambdaCommand(OnUninstallApplicationCommandExecute,
                CanUninstallApplicationCommandExecute);
            InitRowsAsync();
        }

        #region Rows

        public ObservableCollection<ApplicationModel> Rows { get; set; }

        #endregion

        private async void InitRowsAsync()
        {
            ICreatorApplications creator = new RegistryApps();
            Rows = creator.GetApps();
            await Task.Run(() =>
            {
                #region Binding and start AppWatcher

                BindingAndStartAppWatcher();

                #endregion
            });
        }

        private void BindingAndStartAppWatcher()
        {
            foreach (var applicationModel in Rows)
                {
                    applicationModel.Watcher.RegistryTreeChangeEvent += WatcherRegistryTreeChangeEvent;
                    applicationModel.Watcher.Start();
                }
        }

        #region RegistryChangeEvent

        private void WatcherRegistryTreeChangeEvent(object sender, RegistryTreeChangeEventArgs e)
        {
            if (!(sender as AppWatcher).TryUpdateModel())
                AppWatcher.Dispatcher.Invoke(() => { Rows.Remove((sender as AppWatcher).Model); });
        }

        #endregion

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