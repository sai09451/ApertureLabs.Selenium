using Microsoft.AspNetCore.Mvc.RazorPages;
using MockServer.Factories;
using MockServer.Models;
using System.Collections.Generic;

namespace MockServer.Pages
{
    public class IndexModel : PageModel
    {
        #region Fields

        private readonly IFrameworkModelFactory frameworkModelFactory;

        #endregion

        #region Constructor

        public IndexModel(IFrameworkModelFactory frameworkModelFactory)
        {
            this.frameworkModelFactory = frameworkModelFactory;
        }

        #endregion

        #region Properties

        public IList<FrameworkModel> Frameworks { get; set; }

        #endregion

        #region Methods

        public void OnGet()
        {
            Frameworks = frameworkModelFactory.PrepareFrameworkModels();
        }

        #endregion
    }
}