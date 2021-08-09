using System.Collections.ObjectModel;
using System.Windows.Controls;
using URApplication.Models;
using URApplication.ViewModels.Base;

namespace URApplication.ViewModels
{
    internal class MainWindowViewModel: ViewModel
    {
        public ObservableCollection<ApplicationModel> TempMethod()
        {
            var temp = new ObservableCollection<ApplicationModel>();
            temp.Add(new ApplicationModel());
            temp.Add(new ApplicationModel());
            temp.Add(new ApplicationModel());
            return temp;
        }
        public ObservableCollection<ApplicationModel> Rows { get; set; }
        public MainWindowViewModel()
        {
            Rows = new ObservableCollection<ApplicationModel>(TempMethod());
        }
    }
}