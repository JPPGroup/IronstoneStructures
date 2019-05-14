using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Jpp.Ironstone.Core.Autocad;
using Jpp.Ironstone.Core.ServiceInterfaces;
using Jpp.Ironstone.Structures.ObjectModel;
using Jpp.Ironstone.Structures.ObjectModel.Foundations;

namespace Jpp.Ironstone.Structures
{
    public class FoundationCommands
    {
        [CommandMethod("S_Foundation_NewLine")]
        public static void NewFoundationLine()
        {
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            using (Transaction acTrans = acDoc.Database.TransactionManager.StartTransaction())
            {

                
                PromptSelectionResult acSSPrompt = acDoc.Editor.GetSelection();

                // If the prompt status is OK, objects were selected
                if (acSSPrompt.Status == PromptStatus.OK)
                {
                    SelectionSet acSSet = acSSPrompt.Value;
                    FoundationManager manager = DataService.Current.GetStore<StructureDocumentStore>(acDoc.Name)
                        .GetManager<FoundationManager>();

                    SoilProperties sp = DataService.Current.GetStore<StructureDocumentStore>(acDoc.Name).SoilProperties;
                    BlockTableRecord modelSpace = acDoc.Database.GetModelSpace();
                    modelSpace.UpgradeOpen();

                    // Step through the objects in the selection set
                    foreach (SelectedObject acSSObj in acSSet)
                    {
                        // Check to make sure a valid SelectedObject object was returned
                        if (acSSObj != null)
                        {
                            // Open the selected object for write
                            Entity acEnt = acTrans.GetObject(acSSObj.ObjectId, OpenMode.ForRead) as Entity;
                            if (acEnt is Curve)
                            {
                                DBObjectCollection parts = new DBObjectCollection();
                                acEnt.Explode(parts);

                                if (parts.Count >= 1)
                                {
                                    acEnt.UpgradeOpen();
                                    acEnt.Erase();
                                    foreach (DBObject dbObject in parts)
                                    {
                                        FoundationCentreLine foundationCentreline = new FoundationCentreLine(sp);
                                        foundationCentreline.BaseObject = modelSpace.AppendEntity(dbObject as Entity);
                                        manager.Add(foundationCentreline);

                                        acTrans.AddNewlyCreatedDBObject(dbObject, true);
                                    }
                                }
                                else
                                {
                                    FoundationCentreLine foundationCentreline = new FoundationCentreLine(sp);
                                    foundationCentreline.BaseObject = acEnt.ObjectId;
                                    manager.Add(foundationCentreline);
                                }
                            }
                        }
                    }

                    manager.UpdateDirty();
                }

                acTrans.Commit();
            }
        }
    }
}
