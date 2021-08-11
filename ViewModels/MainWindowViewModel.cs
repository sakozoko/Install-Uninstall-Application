using System.Collections.ObjectModel;
using URApplication.Models.ApplicationModels;
using URApplication.Models.Registry;
using URApplication.ViewModels.Base;

namespace URApplication.ViewModels
{
    internal class MainWindowViewModel: ViewModel
    {

        public ObservableCollection<ApplicationModel> Rows { get; set; }
        private ApplicationModel _selectedModel;
        public ApplicationModel SelectedRow
        {
            get => _selectedModel;
            set { _selectedModel = value; OnPropertyChanged(); }
        }
        public MainWindowViewModel()
        {
            Rows = new ObservableCollection<ApplicationModel>(new RegistryApps().GetApps());
        }
    }
}