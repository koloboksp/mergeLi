using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace Assets.Scripts.Common.Shared
{ 
    public static class SerializedPropertyExtension
    {
        //-----------------------------------------------------------------------------------------
        private static object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }
        //-----------------------------------------------------------------------------------------
        private static object GetValue(object source, string name, int index)
        {
            if (!(GetValue(source, name) is IEnumerable enumerable)) 
                return null;
            
            var enm = enumerable.GetEnumerator();

            for (var i = 0; i <= index; i++)
                if (!enm.MoveNext()) 
                    return null;

            return enm.Current;
        }
#if UNITY_EDITOR
        //-----------------------------------------------------------------------------------------
        public static object GetTargetObjectOfProperty(this SerializedProperty prop)
        {
            var route = ToRoute(prop.propertyPath);
            object obj = prop.serializedObject.targetObject;

            for (Atom.NodeRoute node = route; node != null; node++)
            {
                if (node.Next != null && node.Next.RouteType == Atom.NodeRouteType.Index)
                {
                    obj = GetValue(obj, node.Name, node.Next.Index);
                    node++;
                }
                else
                    obj = GetValue(obj, node.Name);
            }
            return obj;
        }
        //-----------------------------------------------------------------------------------------

        public static void SetTargetObjectOfProperty(this SerializedProperty prop, object value)
        {
            var route = ToRoute(prop.propertyPath);
            object obj = prop.serializedObject.targetObject;

            Atom.NodeRoute node = route;
            while (true)
            {
                if (node.Next == null)
                    break;
                if (node.Next.Next != null && node.Next.Next.RouteType == Atom.NodeRouteType.Index)
                    break;

                if (node.Next != null && node.Next.RouteType == Atom.NodeRouteType.Index)
                {
                     if (node.Next.Next == null)
                        break; 
                     
                     obj = GetValue(obj, node.Name, node.Next.Index);
                    
                    node++; 
                }
                else
                    obj = GetValue(obj, node.Name);

                node++;
            }

            if (obj == null)
                return;

            try
            {
                if (node.Next != null && node.Next.RouteType == Atom.NodeRouteType.Index)
                {
                    if (GetValueDirect(obj, node.Name, node.Next.Index) is IList arr) 
                        arr[node.Next.Index] = value;
                }
                else
                {
                    SetValueDirect(obj, node.Name, value);
                }

            }
            catch
            {
                // ignored
            }
        }
#endif
        //-----------------------------------------------------------------------------------------
        private static object GetValueDirect(object parent, string name, params object[] args)
        {
            try
            {
                var parentType = parent.GetType();

                while (parentType != null)
                {
                    var memberInfo = parent.GetType().GetMember(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (memberInfo.Length == 0) 
                        return null;

                    var first = memberInfo[0];

                    switch (memberInfo[0].MemberType)
                    {
                        case MemberTypes.Field: var field = first as FieldInfo; return field?.GetValue(parent);
                        case MemberTypes.Property: var prop = first as PropertyInfo; return prop?.GetValue(parent, args);
                    }
                    parentType = parentType.BaseType;
                }
            }
            catch
            {
                // ignored
            }

            return null;
        }
        //-----------------------------------------------------------------------------------------
        private static void SetValueDirect(object parent, string name, object value, params object[] index)
        {
            try
            {
                var memberInfo = parent.GetType().GetMember(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (memberInfo.Length == 0)
                    return;

                var first = memberInfo[0];

                switch (first.MemberType)
                {
                    case MemberTypes.Field: (first as FieldInfo)?.SetValue(parent, value); return;
                    case MemberTypes.Property: (first as PropertyInfo)?.SetValue(parent, value, index); return;
                }
            }
            catch
            {
                // ignored
            }
        }
        //-----------------------------------------------------------------------------------------
        private static Atom.Route ToRoute(string path)
        {
            lock (mCache)
            {
                if (!mCache.ContainsKey(path))
                    mCache.Add(path, Atom.Conversion.ToRoute(path.Replace("Array.data[", "").Replace("]", "")));

                return mCache[path];
            }
        }
        //-----------------------------------------------------------------------------------------
        private static readonly Dictionary<string, Atom.Route> mCache = new();
    }
}