using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Jpp.Ironstone.Core.ServiceInterfaces;
using Jpp.Ironstone.Core.UI.ViewModels;
using Jpp.Ironstone.Structures.ObjectModel;

namespace Jpp.Ironstone.Structures.ViewModels
{
    class SoilPropertiesViewModel
    {
        private SoilProperties Model;

        public string[] ShrinkageTypes => Enum.GetNames(typeof(Shrinkage));

        public SurfaceSelectViewModel ExistingSurfaceSelector { get; set; }

        public SurfaceSelectViewModel ProposedSurfaceSelector { get; set; }

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

        public ObservableCollection<DepthBand> DepthBands
        {
            get { return Model.DepthBands; }
        }

        public SoilPropertiesViewModel()
        {
            Model = DataService.Current.GetStore<StructureDocumentStore>(Application.DocumentManager.MdiActiveDocument.Name).SoilProperties;
            ExistingSurfaceSelector = new SurfaceSelectViewModel();
            ExistingSurfaceSelector.SelectedSurfaceName = Model.ExistingGroundSurfaceName;
            ExistingSurfaceSelector.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                Model.ExistingGroundSurfaceName = ExistingSurfaceSelector.SelectedSurfaceName;
            };
            ProposedSurfaceSelector = new SurfaceSelectViewModel();
            ProposedSurfaceSelector.SelectedSurfaceName = ProposedSurfaceSelector.SelectedSurfaceName;
            ProposedSurfaceSelector.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                Model.ProposedGroundSurfaceName = ProposedSurfaceSelector.SelectedSurfaceName;
            };
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
