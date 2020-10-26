using Jpp.Ironstone.Core.UI;
using Jpp.Ironstone.Structures.ViewModels;

namespace Jpp.Ironstone.Structures.Views
{
    /// <summary>
    /// Interaction logic for SoilPropertiesView.xaml
    /// </summary>
    public partial class SoilPropertiesView : HostedUserControl
    {
        public SoilPropertiesView()
        {
            InitializeComponent();
        }

        public override void Show()
        {
            this.DataContext = new SoilPropertiesViewModel();
        }

        public override void Hide()
        {

        }
    }
}
