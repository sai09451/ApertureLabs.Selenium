using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Converters;
using Microsoft;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xaml;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.MarkupExtensions
{
    public class BooleanToBrushConverterExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var rootObjectProvider = serviceProvider
                .GetService(typeof(IRootObjectProvider))
                as IRootObjectProvider;
            Assumes.Present(rootObjectProvider);

            var trueColor = Color.FromArgb(120, 0, 255, 0);
            var falseColor = Color.FromArgb(120, 255, 0, 0);

            var rootObject = rootObjectProvider.RootObject;

            if (rootObject is FrameworkElement frameWorkElement)
            {
                // TODO: Determine the true color and false color.
                var resourceTrueColor = frameWorkElement.TryFindResource("trueColor");
                var resourceFalseColor = frameWorkElement.TryFindResource("falseColor");

                if (resourceTrueColor is Color tColor)
                {
                    trueColor = tColor;
                }

                if (resourceFalseColor is Color fColor)
                {
                    falseColor = fColor;
                }
            }

            return new BooleanToBrushConverter(trueColor, falseColor);
        }
    }
}
