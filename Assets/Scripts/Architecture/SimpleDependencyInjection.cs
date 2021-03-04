using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

// This is inspired by: https://github.com/wooga/bi2_dependency_injection

namespace Architecture
{

    public interface IInjectable
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    [MeansImplicitUse]
    public sealed class Inject : System.Attribute
    {
    }

    public class SimpleDependencyInjection : Singleton<SimpleDependencyInjection>
    {
        private static readonly List<FieldInfo> tmpList = new List<FieldInfo>(20);
        private static readonly System.Type injectAttribute = typeof(Inject);
        private static readonly Dictionary<System.Type, FieldInfo[]> cache = new Dictionary<System.Type, FieldInfo[]>();
        private readonly Dictionary<System.Type, object> instances = new Dictionary<System.Type, object>();

        private SimpleDependencyInjection()
        {

        }

        public void Bind<T>(object instance)
        {
            instances[instance.GetType()] = instance;
            Debug.Log("Registered Type = " + instance.GetType());
        }

        public void Unbind<T>()
        {
            instances.Remove(typeof(T));
        }

        public void Inject(object injectee)
        {
            FieldInfo[] fields = getInjectableFields(injectee.GetType());

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (instances.TryGetValue(field.FieldType, out object value))
                {
                    field.SetValue(injectee, value);
                }
            }
        }

        private static FieldInfo[] getInjectableFields(System.Type type)
        {
            FieldInfo[] result;

            if (cache.TryGetValue(type, out result))
            {
                return result;
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (fields[i].IsDefined(injectAttribute))
                {
                    tmpList.Add(field);
                }
            }

            result = tmpList.ToArray();
            tmpList.Clear();

            cache[type] = result;

            return result;
        }
    }
}
