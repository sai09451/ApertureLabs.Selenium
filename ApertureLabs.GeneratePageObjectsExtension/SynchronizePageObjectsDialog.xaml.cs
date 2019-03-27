using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ApertureLabs.GeneratePageObjectsExtension
{
    /// <summary>
    /// Interaction logic for SynchronizePageObjectsDialog.xaml
    /// </summary>
    public partial class SynchronizePageObjectsDialog : DialogWindow
    {
        #region Constructor

        public SynchronizePageObjectsDialog()
        {
            HasMaximizeButton = true;
            HasMinimizeButton = true;
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void RegisterToViewModelEvents()
        {

        }

        #endregion
    }
}
