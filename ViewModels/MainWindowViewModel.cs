using System.Collections.ObjectModel;
using System.Threading.Tasks;
using URApplication.Commands;
using URApplication.Models.Application;
using URApplication.ViewModels.Base;

namespace URApplication.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel()
        {
            InitRowsAsync();
        }

        #region Rows

        public ObservableCollection<ApplicationModel> Rows { get; set; }

        #endregion

        private async void InitRowsAsync()
        {
            var creator = new AppModelsCreator();
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

        private void WatcherRegistryTreeChangeEvent(object sender, Models.Application.Registry.TreeChangeEventArgs e)
        {
            if (!(sender as AppWatcher).TryUpdateModel())
                AppWatcher.Dispatcher.Invoke(() => { Rows.Remove((sender as AppWatcher).Model); });
        }

        #endregion


        #region InitializeCommand

        #region ModifyApplicationCommand
        private LambdaCommand _modifyApplicationCommand;

        public LambdaCommand ModifyApplicationCommand => _modifyApplicationCommand ??= new LambdaCommand(OnModifyApplicationCommandExecute,
                    CanModifyApplicationCommandExecute);

        public bool CanModifyApplicationCommandExecute(object obj)
        {
            return obj is not null;
        }

        private void OnModifyApplicationCommandExecute(object obj)
        {
            Uninstaller.TryModify((string)obj);
        }

        #endregion

        #region UninstallApplicationCommand

        private LambdaCommand _uninstallApplicationCommand;

        public LambdaCommand UninstallApplicationCommand
        {
            get
            {
                return _uninstallApplicationCommand ??= new LambdaCommand(OnUninstallApplicationCommandExecute,
                    CanUninstallApplicationCommandExecute);
            }
        }

        public bool CanUninstallApplicationCommandExecute(object obj)
        {
            return obj is not null;
        }

        private void OnUninstallApplicationCommandExecute(object obj)
        {
            Uninstaller.TryUninstall((string)obj);
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