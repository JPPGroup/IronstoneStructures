using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Jpp.Ironstone.Core.ServiceInterfaces;
using Jpp.Ironstone.Core.UI;
using Jpp.Ironstone.Core.UI.Autocad;
using Jpp.Ironstone.Structures.ObjectModel;
using Jpp.Ironstone.Structures.ObjectModel.TreeRings;
using Jpp.Ironstone.Structures.Views;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Jpp.Ironstone.Structures
{
    public class TreeRingCommands
    {
        public static RibbonPanel BuildUI()
        {
            RibbonPanel Panel = new RibbonPanel();
            RibbonPanelSource source = new RibbonPanelSource();
            source.Title = Properties.Resources.ExtensionApplication_UI_PanelTitle;

            RibbonRowPanel column1 = new RibbonRowPanel();
            column1.IsTopJustified = true;
            column1.Items.Add(UIHelper.CreateWindowToggle(Properties.Resources.ExtensionApplication_UI_SoilMenuButton, Properties.Resources.Earth_Small, RibbonItemSize.Standard, new SoilPropertiesView(), "4c7eae1d-ce9f-4a7a-a397-584aced7983c"));
            column1.Items.Add(new RibbonRowBreak());

            RibbonSplitButton rsb = new RibbonSplitButton();
            rsb.ShowText = true;
            rsb.Items.Add(UIHelper.CreateButton("Add Tree", Properties.Resources.Tree_Small, RibbonItemSize.Standard, "S_TreeRings_New"));
            rsb.Items.Add(UIHelper.CreateButton("Copy Tree", Properties.Resources.Tree_Small, RibbonItemSize.Standard, "S_TreeRings_Copy"));
            rsb.Items.Add(UIHelper.CreateButton("Add Hedge Row", Properties.Resources.Tree_Small, RibbonItemSize.Standard, "S_Hedgerow_New"));
            column1.Items.Add(rsb);

            //Build the UI hierarchy
            source.Items.Add(column1);
            Panel.Source = source;
            return Panel;
        }

        [CommandMethod("S_Hedgerow_New")]
        public static void NewHedgerow()
        {
            try
            {
                StructuresExtensionApplication.Current.Logger.LogCommand(typeof(TreeRingCommands), nameof(NewHedgerow));

                var acDoc = Application.DocumentManager.MdiActiveDocument;
                var ed = acDoc.Editor;
                var entResult = ed.PromptForEntity("\nSelect hedge row centre line: ", typeof(Polyline),"Only polylines allowed.");
                if (!entResult.HasValue) return;

                using (var acTrans = acDoc.TransactionManager.StartTransaction())
                {
                    var treeRingManager = DataService.Current.GetStore<StructureDocumentStore>(acDoc.Name).GetManager<TreeRingManager>();
                    var newHedgeRow = BuildTreeOptions<HedgeRow>(treeRingManager);

                    if (newHedgeRow == null)
                    {
                        acTrans.Abort();
                        return;
                    }

                    newHedgeRow.BaseObject = entResult.Value;
                    newHedgeRow.Generate();
                    newHedgeRow.AddLabel();

                    treeRingManager.AddTree(newHedgeRow);

                    acTrans.Commit();
                }
            }
            catch (System.Exception e)
            {
                StructuresExtensionApplication.Current.Logger.LogException(e);
                throw;
            }
        }

        [CommandMethod("S_TreeRings_New")]
        public static void NewTree()
        {
            try
            {
                StructuresExtensionApplication.Current.Logger.LogCommand(typeof(TreeRingCommands), nameof(NewTree));

                var acDoc = Application.DocumentManager.MdiActiveDocument;
                var ed = acDoc.Editor;
                using (var acTrans = acDoc.TransactionManager.StartTransaction())
                {
                    var treeRingManager = DataService.Current.GetStore<StructureDocumentStore>(acDoc.Name).GetManager<TreeRingManager>();
                    var newTree = BuildTreeOptions<Tree>(treeRingManager);

                    if (newTree == null)
                    {
                        acTrans.Abort();
                        return;
                    }

                    var point = ed.PromptForPosition("\nClick to enter location: ");
                    if (!point.HasValue)
                    {
                        acTrans.Abort();
                        return;
                    }

                    newTree.Location = new Point3d(point.Value.X, point.Value.Y, 0);

                    newTree.Generate();
                    newTree.AddLabel();

                    treeRingManager.AddTree(newTree);

                    acTrans.Commit();
                }
            }
            catch (System.Exception e)
            {
                StructuresExtensionApplication.Current.Logger.LogException(e);
                throw;
            }
        }

        [CommandMethod("S_TreeRings_Copy")]
        public static void CopyTree()
        {
            try
            {
                StructuresExtensionApplication.Current.Logger.LogCommand(typeof(TreeRingCommands), nameof(CopyTree));

                Document acDoc = Application.DocumentManager.MdiActiveDocument;

                using (Transaction acTrans = acDoc.TransactionManager.StartTransaction())
                {
                    PromptStringOptions pStrOptsPlot = new PromptStringOptions("\nEnter tree ID: ") { AllowSpaces = false };
                    PromptResult pStrResPlot = acDoc.Editor.GetString(pStrOptsPlot);
                    if (pStrResPlot.Status != PromptStatus.OK)
                    {
                        acTrans.Abort();
                        return;
                    }

                    string id = pStrResPlot.StringResult;

                    TreeRingManager treeRingManager = DataService.Current.GetStore<StructureDocumentStore>(acDoc.Name).GetManager<TreeRingManager>();
                    Tree treeToBeCopied = treeRingManager.ManagedObjects.FirstOrDefault(t => t.ID == id);
                    if (treeToBeCopied == null)
                    {
                        StructuresExtensionApplication.Current.Logger.Entry($"No tree found matching ID {id}",
                            Severity.Warning);
                        acTrans.Abort();
                        return;
                    }

                    PromptPointOptions pPtOpts = new PromptPointOptions("\nEnter location: ");
                    PromptPointResult pPtRes = acDoc.Editor.GetPoint(pPtOpts);
                    while (pPtRes.Status == PromptStatus.OK)
                    {
                        Tree newTree = new Tree
                        {
                            Height = treeToBeCopied.Height,
                            Phase = treeToBeCopied.Phase,
                            Species = treeToBeCopied.Species,
                            TreeType = treeToBeCopied.TreeType,
                            WaterDemand = treeToBeCopied.WaterDemand,
                            ID = treeRingManager.ManagedObjects.Count.ToString(),
                            Location = new Point3d(pPtRes.Value.X, pPtRes.Value.Y, 0)
                        };

                        newTree.AddLabel();

                        treeRingManager.AddTree(newTree);

                        acDoc.TransactionManager.QueueForGraphicsFlush();
                        pPtRes = acDoc.Editor.GetPoint(pPtOpts);
                    }

                    acTrans.Commit();
                }
            }
            catch (System.Exception e)
            {
                StructuresExtensionApplication.Current.Logger.LogException(e);
                throw;
            }
        }

        private static T BuildTreeOptions<T>(TreeRingManager manager) where T : Tree, new()
        {
            var phase = GetTreePhase();
            if (!phase.HasValue) return null;

            var type = GetTreeType();
            if (!type.HasValue) return null;

            var waterDemand = GetTreeWaterDemand(type.Value);
            if (!waterDemand.HasValue) return null;

            var speciesList = GetSpeciesList(waterDemand.Value, type.Value);
            if(!speciesList.Any()) return null;

            var species = GetTreeSpecies(speciesList);
            if(string.IsNullOrEmpty(species)) return null;

            var maxSpeciesHeight = (float)speciesList[species];
            var height = GetTreeHeight(phase.Value, maxSpeciesHeight);
            if (!height.HasValue) return null;

            var id = GetTreeId(manager);
            if (string.IsNullOrEmpty(id)) return null;

            return new T
            {
                Phase = phase.Value,
                TreeType = type.Value,
                WaterDemand = waterDemand.Value,
                Species = species,
                Height = height.Value,
                ID = id,
                ExceedsNHBC = height > maxSpeciesHeight
            };
        }

        private static Phase? GetTreePhase()
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var result = ed.PromptForKeywords("\nExisting or Proposed: ", Enum.GetNames(typeof(Phase)).ToArray());

            if (Enum.TryParse(result, out Phase phase)) return phase;

            return null;
        }

        private static TreeType? GetTreeType()
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var result = ed.PromptForKeywords("\nTree type: ", Enum.GetNames(typeof(TreeType)).ToArray());

            if (Enum.TryParse(result, out TreeType type)) return type;

            return null;
        }

        private static WaterDemand? GetTreeWaterDemand(TreeType type)
        {
            var k = Enum.GetNames(typeof(WaterDemand)).ToList();
            if (type != TreeType.Deciduous) k.Remove(WaterDemand.Low.ToString());

            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var result = ed.PromptForKeywords("\nWater demand: ", k.ToArray());

            if (Enum.TryParse(result, out WaterDemand waterDemand)) return waterDemand;

            return null;
        }

        private static Dictionary<string, int> GetSpeciesList(WaterDemand waterDemand, TreeType type)
        {
            switch (waterDemand)
            {
                case WaterDemand.High when type == TreeType.Deciduous:
                    return Tree.DeciduousHigh;
                case WaterDemand.High when type == TreeType.Coniferous:
                    return Tree.ConiferousHigh;
                case WaterDemand.Medium when type == TreeType.Deciduous:
                    return Tree.DeciduousMedium;
                case WaterDemand.Medium when type == TreeType.Coniferous:
                    return Tree.ConiferousMedium;
                case WaterDemand.Low when type == TreeType.Deciduous:
                    return Tree.DeciduousLow;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetTreeSpecies(Dictionary<string, int> speciesList)
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var result = ed.PromptForKeywords("\nSpecies: ", speciesList.Keys.ToArray());

            return result;
        }

        //TODO: A test case is required  for when this is moved to design calcs
        private static float? GetTreeHeight(Phase phase, float maxSpeciesHeight)
        {
            if (phase != Phase.Existing) return maxSpeciesHeight;

            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var keyResult = ed.PromptForKeywords("\nIs tree to be removed? ", new[] { "Yes", "No" }, "No");
            if (keyResult != "Yes") return maxSpeciesHeight;

            var strResult = ed.PromptForString("\nEnter current tree height: ", maxSpeciesHeight.ToString(CultureInfo.InvariantCulture));

            if (string.IsNullOrWhiteSpace(strResult)) return null;

            var actualHeight = float.Parse(strResult);

            if (actualHeight > maxSpeciesHeight)
            {
                return actualHeight;
            }

            return actualHeight < maxSpeciesHeight / 2 ? actualHeight : maxSpeciesHeight;
        }

        private static string GetTreeId(TreeRingManager manager)
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            return ed.PromptForString("\nEnter tree ID: ", manager.ManagedObjects.Count.ToString());
        }
    }
}
