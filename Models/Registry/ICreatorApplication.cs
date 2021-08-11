using System.Collections.ObjectModel;
using URApplication.Models.ApplicationModels;

namespace URApplication.Models.Registry
{
    public interface ICreatorApplication
    {
        public ObservableCollection<ApplicationModel> GetApps();
    }
}