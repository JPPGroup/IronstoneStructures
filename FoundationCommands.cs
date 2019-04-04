using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Jpp.Ironstone.Core.ServiceInterfaces;
using Jpp.Ironstone.Structures.Objectmodel;
using Jpp.Ironstone.Structures.Objectmodel.Foundations;
using Jpp.Ironstone.Structures.Objectmodel.TreeRings;

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
                                FoundationCentreline foundationCentreline = new FoundationCentreline();
                                foundationCentreline.BaseObject = acEnt.ObjectId;
                                manager.Add(foundationCentreline);
                            }
                        }
                    }

                    manager.UpdateDirty();
                }
            }
        }
    }
}
