using System.Collections.ObjectModel;

namespace URApplication.Models.Registry
{
    public interface ICreatorApplications
    {
        public ObservableCollection<ApplicationModel> GetApps();
    }
}