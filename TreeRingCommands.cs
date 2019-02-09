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
using Jpp.Ironstone.Structures.Objectmodel.TreeRings;

namespace Jpp.Ironstone.Structures
{
    public class TreeRingCommands
    {
        [CommandMethod("S_TreeRings_New")]
        public static void NewTree()
        {
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
            {
                NHBCTree newTree = new NHBCTree();
                newTree.Generate();

                //TODO: Add tree determination in here
                PromptKeywordOptions pKeyOpts = new PromptKeywordOptions("");
                pKeyOpts.Message = "\nTree Type ";
                pKeyOpts.Keywords.Add("Deciduous");
                pKeyOpts.Keywords.Add("Coniferous");
                pKeyOpts.AllowNone = false;

                PromptResult pKeyRes = acDoc.Editor.GetKeywords(pKeyOpts);
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
                newTree.Species = pKeyRes.StringResult;

                float maxHeight = (float) speciesList[newTree.Species];

                PromptStringOptions pStrOptsPlot =
                    new PromptStringOptions("\nEnter tree height: ")
                    {
                        AllowSpaces = false,
                        DefaultValue = maxHeight.ToString()
                    };
                PromptResult pStrResPlot = acDoc.Editor.GetString(pStrOptsPlot);

                float actualHeight = float.Parse(pStrResPlot.StringResult);

                if (actualHeight < maxHeight / 2)
                {
                    newTree.Height = actualHeight;
                }
                else
                {
                    newTree.Height = maxHeight;
                }

                //TreeRingManager treeRingManager = DrawingObjectManagerCollection.Current.Resolve<TreeRingManager>();
                TreeRingManager treeRingManager = DataService.Current.GetStore<StructureDocumentStore>(acDoc.Name).GetManager<TreeRingManager>();

                pStrOptsPlot = new PromptStringOptions("\nEnter tree ID: ") { AllowSpaces = false, DefaultValue = treeRingManager.Trees.Count.ToString() };
                pStrResPlot = acDoc.Editor.GetString(pStrOptsPlot);
                newTree.ID = pStrResPlot.StringResult;

                pKeyOpts = new PromptKeywordOptions("");
                pKeyOpts.Message = "\nExisting or Proposed ";
                pKeyOpts.Keywords.Add("Existing");
                pKeyOpts.Keywords.Add("Proposed");
                pKeyOpts.AllowNone = false;

                pKeyRes = acDoc.Editor.GetKeywords(pKeyOpts);
                switch (pKeyRes.StringResult)
                {
                    case "Existing":
                        newTree.Phase = Phase.Existing;
                        break;

                    case "Proposed":
                        newTree.Phase = Phase.Proposed;
                        break;
                }

                PromptPointOptions pPtOpts = new PromptPointOptions("\nClick to enter location: ");
                PromptPointResult pPtRes = acDoc.Editor.GetPoint(pPtOpts);

                newTree.Location = new Autodesk.AutoCAD.Geometry.Point3d(pPtRes.Value.X, pPtRes.Value.Y, 0);
                newTree.AddLabel();

                treeRingManager.AddTree(newTree);

                acTrans.Commit();
            }
        }
    }
}
