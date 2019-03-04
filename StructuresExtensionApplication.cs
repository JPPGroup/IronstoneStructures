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
            source.Title = Properties.Resources.ExtensionApplication_UI_PanelTitle;

            RibbonRowPanel column1 = new RibbonRowPanel();
            column1.IsTopJustified = true;
            column1.Items.Add(UIHelper.CreateWindowToggle(Properties.Resources.ExtensionApplication_UI_SoilMenuButton, Properties.Resources.Earth_Small, RibbonItemSize.Standard, System.Windows.Controls.Orientation.Horizontal, new SoilPropertiesView(), "4c7eae1d-ce9f-4a7a-a397-584aced7983c"));
            column1.Items.Add(new RibbonRowBreak());
            column1.Items.Add(UIHelper.CreateButton("Add Tree", Properties.Resources.Tree_Small, RibbonItemSize.Standard, "S_TreeRings_New "));
            //source.Items.Add(new RibbonRowBreak());

            //Build the UI hierarchy
            source.Items.Add(column1);
            
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
