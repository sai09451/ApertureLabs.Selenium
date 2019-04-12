using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MockServer.Pages.kendo._2014._1._318
{
    public class KGridModel : PageModel
    {
        public void OnGet()
        {
        }

        public bool ShowNames => true;

        public IEnumerable<string> Names => new[] { "Bob", "Tim", "John" };
    }
}