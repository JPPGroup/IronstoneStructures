﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jpp.Ironstone.Core.UI;
using Jpp.Ironstone.Structures.ViewModels;
using Path = System.IO.Path;

namespace Jpp.Ironstone.Structures.Views
{
    /// <summary>
    /// Interaction logic for SoilPropertiesView.xaml
    /// </summary>
    public partial class SoilPropertiesView : HostedUserControl
    {
        public SoilPropertiesView()
        {
            InitializeComponent();
        }

        public override void Show()
        {
            this.DataContext = new SoilPropertiesViewModel();
        }

        public override void Hide()
        {

        }
    }
}
