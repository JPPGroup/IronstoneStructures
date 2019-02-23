using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Jpp.Ironstone.Core.ServiceInterfaces;
using Jpp.Ironstone.Structures.Objectmodel;

namespace Jpp.Ironstone.Structures.ViewModels
{
    class SoilPropertiesViewModel
    {
        private SoilProperties Model;

        public Shrinkage Shrinkage
        {
            get { return Model.SoilShrinkability; }
            set { Model.SoilShrinkability = value; }
        }

        public Boolean Granular
        {
            get { return Model.Granular; }
            set { Model.Granular = value; }
        }

        public float TargetStepSize
        {
            get { return Model.TargetStepSize; }
            set { Model.TargetStepSize = value; }
        }

        public SoilPropertiesViewModel()
        {
            Model = DataService.Current.GetStore<StructureDocumentStore>(Application.DocumentManager.MdiActiveDocument.Name).SoilProperties;
        }
    }
}
