using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace ApertureLabs.Selenium.TagHelpers
{
    [HtmlTargetElement(Attributes = AttributeName)]
    public class PageComponentTagHelper : TagHelper
    {
        public const string AttributeName = "selenium-type";

        public override void Process(TagHelperContext context,
            TagHelperOutput output)
        {
            if (!output.Attributes.TryGetAttribute(AttributeName, out var attribute))
                return;

            // Remove the attribute from the output.
            output.Attributes.Remove(attribute);
        }
    }
}
