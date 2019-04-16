﻿using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Commands;
using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Services;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package
    /// for Visual Studio is to implement the IVsPackage interface and register
    /// itself with the shell. This package uses the helper classes defined
    /// inside the Managed Package Framework (MPF) to do it: it derives from
    /// the Package class that provides the implementation of the IVsPackage
    /// interface and uses the registration attributes defined in the
    /// framework to register itself and its components with the shell. These
    /// attributes tell the pkgdef creation utility what data to put into
    /// .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by
    /// &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in
    /// .vsixmanifest file.
    /// </para>
    /// </remarks>
    [Guid(GeneratePageObjectsPackage.PackageGuidString)]
    //[CodeGeneratorRegistration(
    //    generatorType: typeof(Nullable),
    //    generatorName: "",
    //    contextGuid: "",
    //    GeneratesDesignTimeSource = true,
    //    GeneratesSharedDesignTimeSource = true,
    //    GeneratorRegKeyName = "")]
    [PackageRegistration(
        UseManagedResourcesOnly = true,
        AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(
        productName: "#110",
        productDetails: "#112",
        productId: "1.0",
        IconResourceID = 400)] // Info on this package for Help/About
    [ProvideUIContextRule(
        contextGuid: PackageGuids.guidGeneratePageObjectsPackageString,
        name: "Supported Files",
        expression: "CSharp",
        termNames: new[] { "CSharp" },
        termValues: new[] { "HierSingleSelectionName:.cs$" })]
    //[ProvideOptionPage(
    //    pageType: typeof(OptionsPageGrid),
    //    categoryName: "Generate Page Objects",
    //    pageName: "General",
    //    categoryResourceID: 0,
    //    pageNameResourceID: 0,
    //    supportsAutomation: true,
    //    keywordListResourceId: 0)]
    [ProvideMenuResource(
        resourceID: "Menus.ctmenu",
        version: 1)]
    [ProvideService(
        serviceType: typeof(SGeneratePageObjectsService),
        IsAsyncQueryable = true)]
    [SuppressMessage(
        category: "StyleCop.CSharp.DocumentationRules",
        checkId: "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class GeneratePageObjectsPackage : AsyncPackage
    {
        /// <summary>
        /// GeneratePageObjectsPackage GUID string.
        /// </summary>
        public const string PackageGuidString = PackageGuids.guidGeneratePageObjectsPackageString;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratePageObjectsPackage"/> class.
        /// </summary>
        public GeneratePageObjectsPackage()
        {
            // Inside this method you can place any initialization code that
            // does not require any Visual Studio service because at this point
            // the package object is created but not sited yet inside Visual
            // Studio environment. The place to do all the other initialization
            // is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after
        /// the package is sited, so this is the place where you can put all
        /// the initialization code that rely on services provided by
        /// VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token to monitor for initialization cancellation,
        /// which can occur when VS is shutting down.
        /// </param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>
        /// A task representing the async work of package initialization, or
        /// an already completed task if there is none. Do not return null
        /// from this method.
        /// </returns>
        protected override async Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a
            // background thread at this point. Do any initialization that
            // requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await base.InitializeAsync(cancellationToken, progress);

            AddService(
                typeof(SGeneratePageObjectsService),
                CreateServiceGeneratePageObjectsServiceAsync);

            await GeneratePageObjectsCommand.InitializeAsync(this);
        }

        private async Task<object> CreateServiceGeneratePageObjectsServiceAsync(
            IAsyncServiceContainer container,
            CancellationToken cancellationToken,
            Type serviceType)
        {
            if (typeof(SGeneratePageObjectsService) != serviceType)
                return null;

            var service = new GeneratePageObjectsService();
            await service.InitializeServiceAsync(cancellationToken);

            return service;
        }

        #endregion
    }
}