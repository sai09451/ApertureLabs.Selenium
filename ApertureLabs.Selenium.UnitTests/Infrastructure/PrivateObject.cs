using System;
using System.Linq;
using System.Reflection;

namespace ApertureLabs.Selenium.UnitTests.Infrastructure
{
    public class PrivateObject
    {
        #region Fields

        private readonly object instance;
        private readonly Type instanceType;

        #endregion

        #region Constructor

        public PrivateObject(object instance)
        {
            this.instance = instance
                ?? throw new ArgumentNullException(nameof(instance));

            this.instanceType = instance.GetType();
        }

        #endregion

        #region Methods

        public object GetField(string name)
        {
            return GetField(name, BindingFlags.Public);
        }

        public object GetField(string name, BindingFlags bindingFlags)
        {
            return instanceType
                .GetField(name, bindingFlags)
                .GetValue(instance);
        }

        public object GetProperty(string name)
        {
            return GetProperty(name, BindingFlags.Public);
        }

        public object GetProperty(string name, BindingFlags bindingFlags)
        {
            return instanceType
                .GetProperty(name, bindingFlags)
                .GetValue(instance);
        }

        public object GetFieldOrProperty(string name)
        {
            return GetFieldOrProperty(name, BindingFlags.Public);
        }

        public object GetFieldOrProperty(string name, BindingFlags bindingFlags)
        {
            if (instanceType.GetFields(bindingFlags).Any(f => f.Name == name))
            {
                return GetField(name, bindingFlags);
            }
            else if (instanceType.GetProperties(bindingFlags).Any(f => f.Name == name))
            {
                return instanceType.GetProperty(name).GetValue(instance);
            }
            else
            {
                throw new Exception();
            }
        }

        public object Invoke(string name, params object[] args)
        {
            var method = instanceType
                .GetRuntimeMethods()
                .First(m => m.Name == name);

            return method.Invoke(instance, args);
        }

        #endregion
    }
}
