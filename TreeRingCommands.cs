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
using Jpp.Ironstone.Structures.ObjectModel;
using Jpp.Ironstone.Structures.ObjectModel.TreeRings;

namespace Jpp.Ironstone.Structures
{
    public class TreeRingCommands
    {
        [CommandMethod("S_TreeRings_New")]
        public static void NewTree()
        {
            StructuresExtensionApplication.Current.Logger.LogEvent(Event.Command, "S_TreeRings_New");

            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
            {
                NHBCTree newTree = new NHBCTree();
                newTree.Generate();

                //TODO: Add tree determination in here
                PromptKeywordOptions pKeyOpts = new PromptKeywordOptions("");
                pKeyOpts.Message = "\nExisting or Proposed ";
                pKeyOpts.Keywords.Add("Existing");
                pKeyOpts.Keywords.Add("Proposed");
                pKeyOpts.AllowNone = false;

                PromptResult pKeyRes = acDoc.Editor.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.Keyword && pKeyRes.Status != PromptStatus.OK)
                {
                    acTrans.Abort();
                    return;
                }

                switch (pKeyRes.StringResult)
                {
                    case "Existing":
                        newTree.Phase = Phase.Existing;
                        break;

                    case "Proposed":
                        newTree.Phase = Phase.Proposed;
                        break;
                }

                pKeyOpts = new PromptKeywordOptions("");
                pKeyOpts.Message = "\nTree Type ";
                pKeyOpts.Keywords.Add("Deciduous");
                pKeyOpts.Keywords.Add("Coniferous");
                pKeyOpts.AllowNone = false;

                pKeyRes = acDoc.Editor.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.Keyword && pKeyRes.Status != PromptStatus.OK)
                {
                    acTrans.Abort();
                    return;
                }
                if (pKeyRes.StringResult == "Deciduous")
                {
                    newTree.TreeType = TreeType.Deciduous;
                }
                else
                {
                    newTree.TreeType = TreeType.Coniferous;
                }

                pKeyOpts = new PromptKeywordOptions("");
                pKeyOpts.Message = "\nWater deamnd ";
                pKeyOpts.Keywords.Add("High");
                pKeyOpts.Keywords.Add("Medium");
                if (newTree.TreeType == TreeType.Deciduous)
                {
                    pKeyOpts.Keywords.Add("Low");
                }

                pKeyOpts.AllowNone = false;

                pKeyRes = acDoc.Editor.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.Keyword && pKeyRes.Status != PromptStatus.OK)
                {
                    acTrans.Abort();
                    return;
                }
                Dictionary<string, int> speciesList = NHBCTree.DeciduousHigh;
                switch (pKeyRes.StringResult)
                {
                    case "High":
                        newTree.WaterDemand = WaterDemand.High;
                        if (newTree.TreeType == TreeType.Deciduous)
                        {
                            speciesList = NHBCTree.DeciduousHigh;
                        }
                        else
                        {
                            speciesList = NHBCTree.ConiferousHigh;
                        }

                        break;

                    case "Medium":
                        newTree.WaterDemand = WaterDemand.Medium;
                        if (newTree.TreeType == TreeType.Deciduous)
                        {
                            speciesList = NHBCTree.DeciduousMedium;
                        }
                        else
                        {
                            speciesList = NHBCTree.ConiferousMedium;
                        }

                        break;

                    case "Low":
                        newTree.WaterDemand = WaterDemand.Low;
                        if (newTree.TreeType == TreeType.Deciduous)
                        {
                            speciesList = NHBCTree.DeciduousLow;
                        }
                        else
                        {
                            throw new ArgumentException(); //Doesnt exist!!
                        }

                        break;
                }

                pKeyOpts = new PromptKeywordOptions("");
                pKeyOpts.Message = "\nSpecies ";
                foreach (string s in speciesList.Keys)
                {
                    pKeyOpts.Keywords.Add(s);
                }

                pKeyOpts.AllowNone = false;
                pKeyRes = acDoc.Editor.GetKeywords(pKeyOpts);
                if (pKeyRes.Status != PromptStatus.Keyword && pKeyRes.Status != PromptStatus.OK)
                {
                    acTrans.Abort();
                    return;
                }
                newTree.Species = pKeyRes.StringResult;

                float maxHeight = (float) speciesList[newTree.Species];

                PromptStringOptions pStrOptsPlot;
                PromptResult pStrResPlot;
                
                if (newTree.Phase == Phase.Existing)
                {
                    pKeyOpts = new PromptKeywordOptions("");
                    pKeyOpts.Message = "\nIs tree to be removed? ";
                    pKeyOpts.Keywords.Add("Yes");
                    pKeyOpts.Keywords.Add("No");
                    pKeyOpts.Keywords.Default = "No";
                    pKeyOpts.AllowNone = false;
                    pKeyRes = acDoc.Editor.GetKeywords(pKeyOpts);

                    if (pKeyRes.Status != PromptStatus.Keyword && pKeyRes.Status != PromptStatus.OK)
                    {
                        acTrans.Abort();
                        return;
                    }

                    switch (pKeyRes.StringResult)
                    {
                        case "Yes":
                            pStrOptsPlot =
                                new PromptStringOptions("\nEnter current tree height: ")
                                {
                                    AllowSpaces = false,
                                    DefaultValue = maxHeight.ToString()
                                };

                            pStrResPlot = acDoc.Editor.GetString(pStrOptsPlot);
                            if (pStrResPlot.Status != PromptStatus.OK)
                            {
                                acTrans.Abort();
                                return;
                            }

                            float actualHeight = float.Parse(pStrResPlot.StringResult);

                            if (actualHeight < maxHeight / 2)
                            {
                                newTree.Height = actualHeight;
                            }
                            else
                            {
                                newTree.Height = maxHeight;
                            }
                            break;

                        case "No":
                            newTree.Height = maxHeight;
                            break;
                    }

                    
                }
                else
                {
                    newTree.Height = maxHeight;
                }

                //TreeRingManager treeRingManager = DrawingObjectManagerCollection.Current.Resolve<TreeRingManager>();
                TreeRingManager treeRingManager = DataService.Current.GetStore<StructureDocumentStore>(acDoc.Name).GetManager<TreeRingManager>();

                pStrOptsPlot = new PromptStringOptions("\nEnter tree ID: ") { AllowSpaces = false, DefaultValue = treeRingManager.Trees.Count.ToString() };
                pStrResPlot = acDoc.Editor.GetString(pStrOptsPlot);
                if (pStrResPlot.Status != PromptStatus.OK)
                {
                    acTrans.Abort();
                    return;
                }
                newTree.ID = pStrResPlot.StringResult;
                
                PromptPointOptions pPtOpts = new PromptPointOptions("\nClick to enter location: ");
                PromptPointResult pPtRes = acDoc.Editor.GetPoint(pPtOpts);
                if (pPtRes.Status != PromptStatus.OK)
                {
                    acTrans.Abort();
                    return;
                }

                newTree.Location = new Autodesk.AutoCAD.Geometry.Point3d(pPtRes.Value.X, pPtRes.Value.Y, 0);
                newTree.AddLabel();

                treeRingManager.AddTree(newTree);

                acTrans.Commit();
            }
        }

        [CommandMethod("S_TreeRings_Copy")]
        public static void CopyTree()
        {
            StructuresExtensionApplication.Current.Logger.LogEvent(Event.Command, "S_TreeRings_Copy");

            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
            {
                PromptStringOptions pStrOptsPlot = new PromptStringOptions("\nEnter tree ID: ") { AllowSpaces = false };
                PromptResult pStrResPlot = acDoc.Editor.GetString(pStrOptsPlot);
                if (pStrResPlot.Status != PromptStatus.OK)
                {
                    acTrans.Abort();
                    return;
                }

                string ID = pStrResPlot.StringResult;

                TreeRingManager treeRingManager = DataService.Current.GetStore<StructureDocumentStore>(acDoc.Name).GetManager<TreeRingManager>();
                NHBCTree treeToBeCopied = treeRingManager.Trees.FirstOrDefault(t => t.ID == ID);
                if (treeToBeCopied == null)
                {
                    StructuresExtensionApplication.Current.Logger.Entry($"No tree found matching ID {ID}",
                        Severity.Warning);
                    acTrans.Abort();
                    return;
                }

                PromptPointOptions pPtOpts = new PromptPointOptions("\nEnter location: ");
                PromptPointResult pPtRes = acDoc.Editor.GetPoint(pPtOpts);
                while (pPtRes.Status == PromptStatus.OK)
                {
                    NHBCTree newTree = new NHBCTree();
                    newTree.Height = treeToBeCopied.Height;
                    newTree.Phase = treeToBeCopied.Phase;
                    newTree.Species = treeToBeCopied.Species;
                    newTree.TreeType = treeToBeCopied.TreeType;
                    newTree.WaterDemand = treeToBeCopied.WaterDemand;
                    newTree.ID = treeRingManager.Trees.Count.ToString();

                    newTree.Location = new Autodesk.AutoCAD.Geometry.Point3d(pPtRes.Value.X, pPtRes.Value.Y, 0);
                    newTree.AddLabel();

                    treeRingManager.AddTree(newTree);

                    acDoc.TransactionManager.QueueForGraphicsFlush();
                    pPtRes = acDoc.Editor.GetPoint(pPtOpts);
                }

                acTrans.Commit();
            }
        }
    }
}
