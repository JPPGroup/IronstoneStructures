using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Jpp.Ironstone.Core;
using Jpp.Ironstone.Core.ServiceInterfaces;
using Jpp.Ironstone.Core.UI;
using Jpp.Ironstone.Core.UI.Autocad;
using Jpp.Ironstone.Structures.ObjectModel;
using Jpp.Ironstone.Structures.ObjectModel.Appraisal;
using Jpp.Ironstone.Structures.ObjectModel.Appraisal.Elements;

namespace Jpp.Ironstone.Structures
{
    public class AppraisalCommands
    {
        public static RibbonPanel BuildUI()
        {
            RibbonPanel Panel = new RibbonPanel();
            RibbonPanelSource source = new RibbonPanelSource();
            source.Title = Properties.Resources.AppraisalCommands_UI_PanelTitle;

            RibbonRowPanel column1 = new RibbonRowPanel();
            column1.IsTopJustified = true;
            column1.Items.Add(UIHelper.CreateButton(Properties.Resources.AppraisalCommands_UI_BeamButton, Properties.Resources.Beam_Small, RibbonItemSize.Standard, UIHelper.GetCommandGlobalName(typeof(AppraisalCommands), nameof(AddBeam))));
            column1.Items.Add(new RibbonRowBreak());
            column1.Items.Add(UIHelper.CreateButton(Properties.Resources.AppraisalCommands_UI_WallButton, Properties.Resources.Brick_Small, RibbonItemSize.Standard, UIHelper.GetCommandGlobalName(typeof(AppraisalCommands), nameof(AddLBWall))));
            
            //Build the UI hierarchy
            source.Items.Add(column1);
            Panel.Source = source;
            return Panel;
        }

        [IronstoneCommand]
        [CommandMethod("S_Appraisal_AddBeam")]
        public static void AddBeam()
        {
            // TODO: Add transient graphics code

            var acDoc = Application.DocumentManager.MdiActiveDocument;
            var ed = acDoc.Editor;
            using (var acTrans = acDoc.TransactionManager.StartTransaction())
            {
                StructuralElementManager manager = DataService.Current.GetStore<StructureDocumentStore>(acDoc.Name).GetManager<StructuralElementManager>();

                Point3d? startPoint = ed.PromptForPosition("\nClick to enter start location: ");
                if (!startPoint.HasValue)
                {
                    acTrans.Abort();
                    return;
                }

                Point3d? endPoint = ed.PromptForPosition("\nClick to enter end location: ");
                if (!endPoint.HasValue)
                {
                    acTrans.Abort();
                    return;
                }

                // TODO: Handle third dimension

                StructuralBeam beam = StructuralBeam.Create(acDoc.Database, new Point2d(startPoint.Value.X, startPoint.Value.Y), new Point2d(endPoint.Value.X, endPoint.Value.Y));
                manager.Add(beam);
            }
        }

        [IronstoneCommand]
        [CommandMethod("S_Appraisal_AddLBWall")]
        public static void AddLBWall()
        {

        }
    }
}
