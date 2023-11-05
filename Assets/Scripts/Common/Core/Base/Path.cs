using System;
using System.Collections.Generic;

namespace Atom
{
    public enum NodeRouteType
    {
        Name,
        Index
    }

    public class NodeRoute
    {
        internal NodeRoute(string name)
        {
            Name = name;
            RouteType = NodeRouteType.Name;
            mCache = Conversion.ToString(name);
        }

        internal NodeRoute(int index)
        {
            Index = index;
            RouteType = NodeRouteType.Index;
            mCache = Conversion.ToString(index);
        }

        public static implicit operator string(NodeRoute v)
        {
            return v.ToString(); 
        }

        public string Path => (Prev != null ? Prev.Path + "." : "") + this;

        public override string ToString()
        {
            return mCache;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NodeRoute route))
                return false;

            return this == route;
        }

        public override int GetHashCode()
        {
            return mCache.GetHashCode();
        }

        public static bool operator ==(NodeRoute l, NodeRoute r)
        {
            if (l is null && r is null)
                return true;

            if (l is null || r is null)
                return false;

            if (l.RouteType != r.RouteType)
                return false;

            if (l.RouteType == NodeRouteType.Name)
                return l.Name == r.Name;
             
            if (l.Index == r.Index)
                return true;

            return false;
        }

        public static bool operator !=(NodeRoute l, NodeRoute r)
        {
            return !(l == r);
        }

        public static NodeRoute operator ++(NodeRoute node)
        {
            return node.Next;
        }

        public NodeRoute Next { get; internal set; }
        public NodeRoute Prev { get; internal set; }

        public string Name { get; internal set; }
        public int Index { get; internal set; }
        public NodeRouteType RouteType { get; }

        private readonly string mCache;
    }

    public class Route
    {
        private Route(string path)
        {
            Calculate(path);
        }

        public static Route CheckPath(string path)
        {
            return new Route(path);
        }

        //сравнение путей
        public static bool CheckPath(NodeRoute path1, NodeRoute path2)
        {
            if (path1 == null && path2 == null)
                return true;

            if (path1 != null && path2 != null)
                if (path1 == path2)
                    return CheckPath(path1.Next, path2.Next);

            return false;
        }

        private void Calculate(string path)
        {
            if (!Conversion.IsRoute(path))
                throw new Exception("invalid path: " + path);

            var index = 0;

            NodeRoute parent = null;

            while (index != path.Length)
            {
                var i = path.IndexOf('.', index);
                var name = i == -1 ? path.Substring(index) : path.Substring(index, i - index);
                index = i == -1 ? path.Length : i + 1;

                var node = Conversion.FastCheckInt(name, 0, name.Length) ?
                    new NodeRoute(Conversion.ToInt(name)) :
                    new NodeRoute(name);

                if (parent == null)
                {
                    Path = node;
                    parent = node;
                }
                else
                {
                    parent.Next = node;
                    node.Prev = parent;
                    parent = node;
                }
            }
        }

        public static implicit operator NodeRoute(Route v)
        {
            return v.Path;
        }

        public static implicit operator string(Route v)
        {
            var result = v.Path.ToString();
            var p = v.Path;
            while (true)
            {
                p = p.Next;
                if (p == null)
                    break;
                result += '.' + p;
            }

            return result;
        }

        public override string ToString()
        {
            return this;
        }

        //A.B.C.D
        //разрешённые операции
        //D->A D->B D->C 
        //C->A C->B
        //B->A
        //запрещёные
        //A->D B->D C->D D->D
        //B->C B->D 
        //C->D
        public static bool CheckSubstringPath(NodeRoute pathBase, NodeRoute pathCheck)
        {
            if (pathBase != null && pathCheck != null)
            {
                if (pathBase != pathCheck)
                    return true;

                return CheckSubstringPath(pathBase.Next, pathCheck.Next);
            }

            return pathBase == null && pathCheck != null;
        }

        //public NodeRoute Name { get; internal set; }
        public NodeRoute Path { get; internal set; }

        //public int Level { get; internal set; }

        public static Route CalculateRoute(string path)
        {
            lock (mCache)
            {
                if (!mCache.ContainsKey(path))
                    mCache.Add(path, new Route(path));

                return mCache[path];
            }
        }
        private static readonly Dictionary<string, Route> mCache = new Dictionary<string, Route>();
    }   
}

namespace Atom
{
    public static partial class Conversion
    {
        public static Route ToRoute(string value)
        {
            return Route.CalculateRoute(value);
        }
        public static bool IsRoute(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
                return false;

            var elements = route.Split('.');

            for (var i = 0; i != elements.Length; ++i)
            {
                var data = elements[i];

                if (string.IsNullOrWhiteSpace(data))
                    return false;

                if (!FastCheckName(data, 0, data.Length) && !FastCheckInt(data, 0, data.Length))
                    return false;
            }

            return true;
        }
    }
}
    
