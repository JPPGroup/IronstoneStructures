using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Windows;
using Autodesk.AutoCAD.Runtime;
using Jpp.Ironstone.Core;
using Jpp.Ironstone.Core.Autocad;
using Jpp.Ironstone.Core.UI;
using Jpp.Ironstone.Core.UI.Autocad;
using Jpp.Ironstone.Structures.ObjectModel.Foundations;
using Jpp.Ironstone.Structures.Properties;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Jpp.Ironstone.Structures
{
    public class DetailPlotCommands
    {
        public static RibbonPanel BuildUI()
        {
            RibbonPanel Panel = new RibbonPanel();
            RibbonPanelSource source = new RibbonPanelSource();
            source.Title = Properties.Resources.DetailPlotCommands_UI_MasterPanelTitle;

            RibbonRowPanel column1 = new RibbonRowPanel();
            column1.IsTopJustified = true;
            /*column1.Items.Add(UIHelper.CreateWindowToggle(Properties.Resources.ExtensionApplication_UI_SoilMenuButton, Properties.Resources.Earth_Small, RibbonItemSize.Standard, new SoilPropertiesView(), "4c7eae1d-ce9f-4a7a-a397-584aced7983c"));
            column1.Items.Add(new RibbonRowBreak());*/
            column1.Items.Add(UIHelper.CreateButton(Resources.DetailPlotCommands_UI_AddCentrelineButton, Resources.Centreline_Small, RibbonItemSize.Standard, UIHelper.GetCommandGlobalName(typeof(DetailPlotCommands), nameof(AddCentreline))));
            
            //Build the UI hierarchy
            source.Items.Add(column1);
            Panel.Source = source;
            return Panel;
        }

        [IronstoneCommand]
        [CommandMethod("S_DetailMaster_AddCentreline")]
        public static void AddCentreline()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Point3d? startPoint = doc.Editor.PromptForPosition("Select start position:");
            if (!startPoint.HasValue)
                return;

            Point3d? endPoint = doc.Editor.PromptForPosition("Select end position:");
            if (!endPoint.HasValue)
                return;

            double? load = doc.Editor.PromptForDouble("Please enter line load in kN/m: ");
            if(!load.HasValue)
                return;

            using (Transaction trans = doc.TransactionManager.StartTransaction())
            {
                // TODO: Add code for setting layer
                LineDrawingObject lineDrawingObject = LineDrawingObject.Create(doc.Database, startPoint.Value, endPoint.Value);
                lineDrawingObject[FoundationGroup.FOUNDATION_CENTRE_LOAD_KEY] = load.Value.ToString();

                trans.Commit();
            }
        }
    }
}
