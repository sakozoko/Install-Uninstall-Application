using System.Collections.ObjectModel;

namespace URApplication.Models
{
    public interface ICreatorApplications
    {
        public ObservableCollection<ApplicationModel> GetApps();
    }
}