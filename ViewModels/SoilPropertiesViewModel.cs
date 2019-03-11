using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Jpp.Ironstone.Core.ServiceInterfaces;
using Jpp.Ironstone.Structures.ObjectModel;

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

    public class StepSizeValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            float stepSize = 0.3f;
            try
            {
                if (((string)value).Length > 0)
                    stepSize = float.Parse((String)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }

            if (stepSize < 0.1f || stepSize > 1f)
            {
                return new ValidationResult(false, "Invalid step size");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
