using System;
using System.Collections.Generic;
using System.Reflection;

namespace AfterDarkScreensavers
{
    internal static class ReflectionUtility
    {
        /// <summary>
        /// Get's all methods within the current assembly with the specified attribute. 
        /// </summary>
        internal static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
        {
            List<MethodInfo> methods = new List<MethodInfo>();

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var type in types)
            {
                var allMethods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                foreach (var method in allMethods)
                {
                    var attribute = method.GetCustomAttribute(typeof(T));

                    if (attribute == null)
                        continue;

                    methods.Add(method);
                }
            }

            return methods.ToArray();
        }

        /// <summary>
        /// Invokes every method within the specified array. 
        /// </summary>
        internal static void CallMethods(MethodInfo[] methods)
        {
            foreach (var method in methods)
            {
                method.Invoke(null, null);
            }
        }
    }
}
