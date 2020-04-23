using System;
using System.IO;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Windows;
using Jpp.Ironstone.Core;
using Jpp.Ironstone.Core.ServiceInterfaces;
using Jpp.Ironstone.Core.UI;
using Jpp.Ironstone.Housing.ObjectModel;
using Jpp.Ironstone.Structures.Properties;
using Jpp.Ironstone.Structures.Views;
using Unity;
using Xceed.Wpf.Toolkit;
using RibbonControl = Autodesk.Windows.RibbonControl;
using RibbonPanelSource = Autodesk.Windows.RibbonPanelSource;
using RibbonRowPanel = Autodesk.Windows.RibbonRowPanel;
using RibbonSplitButton = Autodesk.Windows.RibbonSplitButton;

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
            // These lines are required to force assembly to be present in bin directory and resolve correctly.
            ColorPicker cp = new ColorPicker();
            cp.AdvancedTabHeader = "Dummy";

            RibbonControl rc = Autodesk.Windows.ComponentManager.Ribbon;
            RibbonTab primaryTab = rc.FindTab(Jpp.Ironstone.Core.Constants.IRONSTONE_TAB_ID);

            primaryTab.Panels.Add(TreeRingCommands.BuildUI());
            primaryTab.Panels.Add(AppraisalCommands.BuildUI());

            SharedUIHelper.StructuresAvailable = true;
            SharedUIHelper.CreateSharedElements();
            RibbonPanel HousinhPanel = new RibbonPanel();
            RibbonPanelSource housingSource = new RibbonPanelSource();

            RibbonRowPanel hcolumn1 = new RibbonRowPanel();
            hcolumn1.IsTopJustified = true;
            // TODO: Change icon
            var cmdConceptualPlotEstimateFounds = UIHelper.GetCommandGlobalName(typeof(ConceptualPlotCommands), nameof(ConceptualPlotCommands.EstimateFounds));
            var btnConceptualPlotEstimateFounds = UIHelper.CreateButton(Resources.ExtensionApplication_UI_ToggleFoundations, Resources.Earth_Small, RibbonItemSize.Standard, cmdConceptualPlotEstimateFounds);
            hcolumn1.Items.Add(btnConceptualPlotEstimateFounds);

            housingSource.Items.Add(hcolumn1);
            HousinhPanel.Source = housingSource;
            SharedUIHelper.HousingConceptTab.Panels.Add(HousinhPanel);
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
