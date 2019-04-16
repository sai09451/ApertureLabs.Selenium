using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApertureLabs.Tools.CodeGeneration.Core.CodeGeneration
{
    public partial class PageObjectTemplate
    {
        private readonly Type model;

        public PageObjectTemplate(Type model)
        {
            this.model = model
                ?? throw new ArgumentNullException(nameof(model));
        }

        public IEnumerable<string> GetSelectors()
        {
            throw new NotImplementedException();
        }

        public IList<string> GetDependencyFields()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetFields()
        {
            var fields = model.GetFields(BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                var fullTypeDecl = new List<string>()
                {
                    field.IsPrivate ? "private" : "public"
                };

                if (field.IsInitOnly)
                    fullTypeDecl.Add("readonly");

                fullTypeDecl.Add(field.FieldType.FullName);

                if (field.IsSpecialName)
                    fullTypeDecl.Add("@" + field.Name);
                else
                    fullTypeDecl.Add(field.Name);

                fullTypeDecl.Add(";");

                yield return String.Join(' ', fullTypeDecl);
            }
        }
    }
}
