using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models;
using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Services;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using System;
using System.Windows;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Xaml
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

        #region Properties

        public IGeneratePageObjectsService GeneratePageObjectsService { get; set; }

        #endregion

        #region Methods

        public void OnProgress(int progress)
        {
            throw new NotImplementedException();
        }

        public void ChooseFolder()
        {
            if (!(DataContext is SynchronizePageObjectsModel model))
                return;

            var newProject = model.AvailableProjects[0];
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = newProject.FullPath
            };

            fileDialog.ShowDialog(this);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void RegisterToViewModelEvents()
        {

        }

        #endregion

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChooseFolder_Button_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open file dialog.
            //var dialog = new OpenFileDialog();
        }

        private void Synchronize_Button_Click(object sender, RoutedEventArgs e)
        {
            // Close the modal.
            Close();

            // Start the task.
            GeneratePageObjectsService.GeneratePageObjectsAsync(
                DataContext as SynchronizePageObjectsModel);
        }
    }
}
