using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ApertureLabs.Tools.CodeGeneration.Core.CodeGeneration
{
    public class ClassGeneration
    {
        public string Namespace { get; set; }

        public string ClassName { get; set; }

        private void Test()
        {
            var assemblyName = new AssemblyName("ApertureLabs.Selenium.Generated");
            var ab = AssemblyBuilder.DefineDynamicAssembly(
                name: assemblyName,
                access: AssemblyBuilderAccess.Run);
            var mb = ab.DefineDynamicModule("ApertureLabs.Selenium.Generated");
            //var tb = mb.DefineType(
            //    name: ,
            //    attr: TypeAttributes.Public,
            //    parent: ,
            //    interfaces: );
            //tb.CreateType();
        }
    }
}
