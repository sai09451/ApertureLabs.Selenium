//using System;
//using System.IO;
//using System.Reflection;
//using System.Runtime.Loader;

//namespace ApertureLabs.Tools.CodeGeneration.Core
//{
//    internal class PluginLoadContext : AssemblyLoadContext
//    {
//        private readonly AssemblyDependencyResolver resolver;
//        private readonly string pathToAssemblies;

//        public PluginLoadContext(string path)
//        {
//            string cleanPath;
            
//            // Verify path is a directory.
//            if (Directory.Exists(path))
//                cleanPath = path;
//            else if (File.Exists(path))
//                cleanPath = new FileInfo(path).Directory.FullName;
//            else
//                throw new FileNotFoundException(path);

//            resolver = new AssemblyDependencyResolver(cleanPath);
//            pathToAssemblies = cleanPath;
//        }

//        protected override Assembly Load(AssemblyName assemblyName)
//        {
//            string assemblyPath = resolver.ResolveAssemblyToPath(assemblyName)
//                ?? Path.Combine(pathToAssemblies, $"{assemblyName.Name}.dll");

//            if (assemblyName.Name.Equals("netstandard", StringComparison.Ordinal))
//            {
//                // TODO: Resolve location?
//            }

//            return assemblyPath != null
//                ? LoadFromAssemblyPath(assemblyPath)
//                : null;
//        }

//        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
//        {
//            string libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

//            return libraryPath != null
//                ? LoadUnmanagedDllFromPath(libraryPath)
//                : IntPtr.Zero;
//        }
//    }
//}
