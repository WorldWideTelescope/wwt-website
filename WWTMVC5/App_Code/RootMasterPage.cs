

namespace Microsoft.Research.WWT
{
    

    public abstract class RootMasterPage : System.Web.UI.MasterPage
    {
        public abstract NavigationItem SelectedNavItem { get; set; }
    }
}
