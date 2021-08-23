using System.Collections.ObjectModel;
using URApplication.Models.Application;

namespace URApplication.Models
{
    public interface ICreatorApplications
    {
        public ObservableCollection<ApplicationModel> GetApps();
    }
}