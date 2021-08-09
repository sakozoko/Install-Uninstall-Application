using System.Collections.ObjectModel;
using System.Windows.Controls;
using URApplication.Models;
using URApplication.Models.Registry;
using URApplication.ViewModels.Base;

namespace URApplication.ViewModels
{
    internal class MainWindowViewModel: ViewModel
    {
        public ObservableCollection<ApplicationModel> TempMethod()
        {
            var temp = new ObservableCollection<ApplicationModel>
            {
                new ApplicationModel(),
                new ApplicationModel(),
                new ApplicationModel()
            };
            return temp;
        }

        public ObservableCollection<ApplicationModel> Rows { get; set; }
        private ApplicationModel _selectedModel;
        public ApplicationModel SelectedRow
        {
            get => _selectedModel;
            set { _selectedModel = value; OnPropertyChanged(); }
        }
        public MainWindowViewModel()
        {
            Rows = new ObservableCollection<ApplicationModel>(Class1.GetApps());
        }
    }
}