using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Jpp.Ironstone.Core;
using Jpp.Ironstone.Core.ServiceInterfaces;
using Jpp.Ironstone.Housing.ObjectModel;
using Jpp.Ironstone.Housing.ObjectModel.Concept;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jpp.Ironstone.Structures
{
    public static class ConceptualPlotCommands
    {
        [IronstoneCommand]
        [Civil3D]
        [CommandMethod("C_ConceptualPlot_EstimateFounds", CommandFlags.UsePickSet)]
        public static void EstimateFounds()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            PromptSelectionResult psr = document.Editor.SelectImplied();

            ILogger<StructuresExtensionApplication> logger = CoreExtensionApplication._current.Container.GetRequiredService<ILogger<StructuresExtensionApplication>>();

            if (psr.Status == PromptStatus.OK)
            {
                using (Transaction trans = document.TransactionManager.StartTransaction())
                {
                    ConceptualPlotManager manager = DataService.Current.GetStore<HousingDocumentStore>(document.Name)
                        .GetManager<ConceptualPlotManager>();

                    foreach (ObjectId objectId in psr.Value.GetObjectIds())
                    {
                        DBObject obj = trans.GetObject(objectId, OpenMode.ForWrite);

                        if (manager.ManagedObjects.Any(cp => cp.BaseObject == objectId))
                        {
                            ConceptualPlot conceptualPlot = manager.ManagedObjects.First(cp => cp.BaseObject == objectId);
                            conceptualPlot.FoundationsEnabled = true;
                        }
                        else
                        {
                            logger.LogWarning("Selected object is not a conceptual plot.");
                        }
                    }

                    trans.Commit();
                }
            }
        }
    }
}
