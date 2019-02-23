using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Jpp.Ironstone.Core;
using Jpp.Ironstone.Core.ServiceInterfaces;
using Jpp.Ironstone.Core.UI;
using Jpp.Ironstone.Structures.Views;
using Unity;
using RibbonControl = Autodesk.Windows.RibbonControl;
using RibbonPanelSource = Autodesk.Windows.RibbonPanelSource;
using RibbonRowPanel = Autodesk.Windows.RibbonRowPanel;

[assembly: ExtensionApplication(typeof(Jpp.Ironstone.Structures.StructuresExtensionApplication))]

namespace Jpp.Ironstone.Structures
{
    class StructuresExtensionApplication : IIronstoneExtensionApplication
    {
        public ILogger Logger { get; set; }

        public static StructuresExtensionApplication Current
        {
            get { return _current; }
        }

        private static StructuresExtensionApplication _current;

        public void CreateUI()
        {
            RibbonControl rc = Autodesk.Windows.ComponentManager.Ribbon;
            RibbonTab primaryTab = rc.FindTab(Jpp.Ironstone.Core.Constants.IRONSTONE_TAB_ID);

            RibbonPanel Panel = new RibbonPanel();
            RibbonPanelSource source = new RibbonPanelSource();
            RibbonRowPanel stack = new RibbonRowPanel();
            stack.IsTopJustified = true;

            source.Title = Properties.Resources.ExtensionApplication_UI_PanelTitle;

            stack.Items.Add(UIHelper.CreateWindowToggle(Properties.Resources.ExtensionApplication_UI_SoilMenuButton, Properties.Resources.Earth_Small, RibbonItemSize.Standard, System.Windows.Controls.Orientation.Horizontal, new SoilPropertiesView(), "4c7eae1d-ce9f-4a7a-a397-584aced7983c"));

            /*UIHelper.CreateWindowToggle(Properties.Resources.ExtensionApplication_UI_SoilMenuButton,
                Properties.Resources.Earth_Small, RibbonItemSize.Standard, System.Windows.Controls.Orientation.Horizontal,
                new SoilPropertiesView(),
                "4c7eae1d-ce9f-4a7a-a397-584aced7983c");*/

            stack.Items.Add(new RibbonRowBreak());

            /*stack.Items.Add(Utilities.CreateWindowToggle(Properties.Resources.Structures_TreeRing_MenuButton,
                Properties.Resources.Tree_Small, RibbonItemSize.Standard, Orientation.Horizontal, new TreeRingView(),
                "36a12ae4-06b2-432b-a445-64c62daa937f"));*/

            //Build the UI hierarchy
            source.Items.Add(stack);
            Panel.Source = source;

            primaryTab.Panels.Add(Panel);
        }

        public void Initialize()
        {
            _current = this;
            CoreExtensionApplication._current.RegisterExtension(this);
        }

        public void InjectContainer(IUnityContainer container)
        {
            Logger = container.Resolve<ILogger>();
        }

        public void Terminate()
        {

        }
    }
}
