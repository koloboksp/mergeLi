using System;
using System.Collections;
using System.Collections.Generic;

namespace Atom.Variant
{
    [Serializable]
    public sealed partial class Var
    {
        public static readonly Var Null = new Var();

        //-----------------------------------------------------------------------------------------
        public static Var CreateList()
        {
            return new Var { Data = null, Type = VarType.List, mListVars = new List<Var>() };
        }
        //-----------------------------------------------------------------------------------------
        public static Var CreateList(params Var[] data)
        {
            if (data == null)
                throw new ArgumentNullException();
            var v = new Var { Data = null, Type = VarType.List, mListVars = new List<Var>() };

            foreach (var d in data)
                v.Add(d);

            return v;
        }
        //-----------------------------------------------------------------------------------------
        public static Var CreateList(IEnumerable<string> data)
        {
            var v = new Var { Data = null, Type = VarType.List, mListVars = new List<Var>() };

            foreach (var d in data)
                v.Add(d);
            return v;
        }
        //-----------------------------------------------------------------------------------------
        public static Var CreateList(IEnumerable<Var> data)
        {
            if (data == null)
                throw new ArgumentNullException();
            var v = new Var { Data = null, Type = VarType.List, mListVars = new List<Var>() };

            foreach (var d in data)
                v.Add(d);

            return v;
        }
        //-----------------------------------------------------------------------------------------
        public static Var CreateTree()
        {
            return new Var { Data = null, Type = VarType.Tree, mTreeVars = new SortedList() };
        }
        //-----------------------------------------------------------------------------------------
        public static Var CreateTree(IEnumerable<string> data)
        {
            var v = new Var { Data = null, Type = VarType.Tree, mTreeVars = new SortedList() };

            foreach (var d in data)
                v.Insert(d);
            return v;
        }
        //-----------------------------------------------------------------------------------------
        public static Var CreateTree(KeyValuePair<string, Var> data)
        {
            var v = new Var { Data = null, Type = VarType.Tree, mTreeVars = new SortedList() };
            v.Insert(data.Key, data.Value);
            return v;
        }

        //-----------------------------------------------------------------------------------------
        public static Var CreateTree(Dictionary<string, Var> data)
        {
            var v = new Var { Data = null, Type = VarType.Tree, mTreeVars = new SortedList() };

            foreach (var d in data)
                v.Insert(d.Key, d.Value);
            return v;
        }
        //-----------------------------------------------------------------------------------------
        public static Var CreateObject(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new NullReferenceException();

            return new Var
            {
                Type = VarType.Object,
                Data = Conversion.ToByteArray(type),
                mTreeVars = new SortedList()
            };
        }
        //-----------------------------------------------------------------------------------------
        public Var()
        {
            Type = VarType.Null;
            Data = new byte[0];
        }

        //-----------------------------------------------------------------------------------------
        public Var(bool value)
        {
            Type = VarType.Bool;
            Data = Conversion.ToByteArray(value);
        }

        //-----------------------------------------------------------------------------------------
        public Var(int value)
        {
            Type = VarType.Int;
            Data = Conversion.ToByteArray(value);
        }

        //-----------------------------------------------------------------------------------------
        public Var(float value)
        {
            Type = VarType.Float;
            Data = Conversion.ToByteArray(value);
        }
        //-----------------------------------------------------------------------------------------
        public Var(float? value)
        {
            if (!value.HasValue)
            {
                Type = VarType.Null;
                Data = new byte[0];
            }
            else
            {
                Type = VarType.Float;
                Data = Conversion.ToByteArray(value.Value);
            }
        }
        //-----------------------------------------------------------------------------------------
        public Var(string value)
        {
            if (value == null)
                throw new NullReferenceException();

            Type = VarType.String;
            Data = Conversion.ToByteArray(value);
        }

        //-----------------------------------------------------------------------------------------
        public Var(byte[] value)
        {
            if (value == null)
                throw new NullReferenceException();

            Type = VarType.ByteArray;
            Data = Conversion.ToByteArray(value);
        }

        //-----------------------------------------------------------------------------------------
        public Var(VarType value)
        {
            Type = VarType.Type;
            Data = Conversion.ToByteArray((int)value);
        }

        //-----------------------------------------------------------------------------------------
        public Var(params Var[] data)
        {
            if (data == null)
                throw new NullReferenceException();

            Type = VarType.List;
            mListVars = new List<Var>();

            foreach (var i in data)
                mListVars.Add(i);
        }

        //-----------------------------------------------------------------------------------------
        internal Var(VarType type, byte[] data)
        {
            Type = type;
            Data = data;
        }

        //-----------------------------------------------------------------------------------------
        public static Var CreateDefault(VarType type)
        {
            switch (type)
            {
                case VarType.Null: return new Var();
                case VarType.Bool: return new Var(false);
                case VarType.Int: return new Var(0);
                case VarType.Float: return new Var(0.0f);
                case VarType.String: return new Var("");
                case VarType.ByteArray: return new Var(new byte[0]);
                case VarType.Tree: return CreateTree();
                case VarType.List: return CreateList();
            }

            throw new VariantUnsupportedFunctionalityException("CreateDefault(VarType)", type);
        }

        //-----------------------------------------------------------------------------------------
        public bool CanAssignTo(Type t)
        {
            switch (Type)
            {
                case VarType.Bool: return t == typeof(bool);
                case VarType.Int: return t == typeof(int);
                case VarType.Float: return t == typeof(float);
                case VarType.String: return t == typeof(string);
                case VarType.ByteArray: return t == typeof(byte[]);
                case VarType.Type: return t == typeof(VarType);
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------
        public override string ToString()
        {
            return this;
        }

        //-----------------------------------------------------------------------------------------
        public static implicit operator bool(Var v)
        {
            if (v.Type != VarType.Bool)
                throw new VariantConvertException(v.Type, typeof(bool));

            return Memory.UnpackingBool(v.Data);
        }

        //-----------------------------------------------------------------------------------------
        public static implicit operator int(Var v)
        {
            if (v.Type != VarType.Int)
                throw new VariantConvertException(v.Type, typeof(int));

            return Memory.UnpackingInt(v.Data);
        }

        //-----------------------------------------------------------------------------------------
        public static implicit operator float(Var v)
        {
            if (v.Type != VarType.Float)
                throw new VariantConvertException(v.Type, typeof(float));

            return Memory.UnpackingFloat(v.Data);
        }
        //-----------------------------------------------------------------------------------------
        public static implicit operator float?(Var v)
        {
            if (v.Type == VarType.Null)
                return null;
             if (v.Type != VarType.Float)
                throw new VariantConvertException(v.Type, typeof(float?));

            return Memory.UnpackingFloat(v.Data);
        }
        //-----------------------------------------------------------------------------------------
        public static implicit operator string(Var v)
        {
            if (v == null)
                return null;

            switch (v.Type)
            {
                case VarType.Null: return string.Empty;
                case VarType.Bool: return Conversion.ToString(Memory.UnpackingBool(v.Data));
                case VarType.Int: return Conversion.ToString(Memory.UnpackingInt(v.Data));
                case VarType.Float: return Conversion.ToString(Memory.UnpackingFloat(v.Data));
                case VarType.String: return Conversion.ToString(Conversion.ToString(v.Data));
                case VarType.ByteArray: return Conversion.ByteArrayToHexString(v.Data);
                case VarType.Type: return Conversion.ToString((VarType)Memory.UnpackingInt(v.Data));
                case VarType.List: return "List: " + Conversion.ToString(Conversion.ToInt(v.Count));
                case VarType.Tree: return "Tree: " + Conversion.ToString(Conversion.ToInt(v.Count));
                case VarType.Object: return "Object: " + Conversion.ToString(Conversion.ToInt(v.Count));

            }

            throw new VariantConvertException(v.Type, typeof(string));
        }

        //-----------------------------------------------------------------------------------------
        public static implicit operator byte[](Var v)
        {
            if (v.Type != VarType.ByteArray)
                throw new VariantConvertException(v.Type, typeof(byte[]));

            return v.Data;
        }

        //-----------------------------------------------------------------------------------------
        public static implicit operator VarType(Var v)
        {
            if (v.Type != VarType.Type)
                throw new VariantConvertException(v.Type, typeof(VarType));

            return (VarType)Memory.UnpackingInt(v.Data);
        }

        //-----------------------------------------------------------------------------------------
        public static implicit operator Var(bool b)
        {
            return new Var(b);
        }

        //-----------------------------------------------------------------------------------------
        public static implicit operator Var(int i)
        {
            return new Var(i);
        }

        //-----------------------------------------------------------------------------------------
        public static implicit operator Var(float f)
        {
            return new Var(f);
        }
        //-----------------------------------------------------------------------------------------
        public static implicit operator Var(float? f)
        {
            return new Var(f);
        }
        //-----------------------------------------------------------------------------------------
        public static implicit operator Var(string s)
        {
            return new Var(s);
        }

        //-----------------------------------------------------------------------------------------
        public static implicit operator Var(byte[] b)
        {
            return new Var(b);
        }

        //-----------------------------------------------------------------------------------------
        public static implicit operator Var(VarType t)
        {
            return new Var(t);
        }

        //-----------------------------------------------------------------------------------------
        public int Count
        {
            get
            {
                if (IsList())
                    return mListVars.Count;
                if (IsTree() || IsObject())
                    return mTreeVars.Count;

                throw new VariantUnsupportedFunctionalityException("Count", Type);
            }
        }

        //-----------------------------------------------------------------------------------------
        //
        //Remove
        //
        //A|B|C|D => Remove(A|B|C|E) => A|B|C|D
        //     |E                            |F|G
        //     |F|G
        //
        //A|B|C|0 => Remove(A|B|C|1) => A|B|C|0
        //     |1                            |1|D
        //     |2|D
        public Var Remove(string path)
        {
            return Remove(Conversion.ToRoute(path));
        }

        //-----------------------------------------------------------------------------------------
        public Var Remove(NodeRoute path)
        {
            if (IsTree() || IsObject())
            {
                if(path.RouteType != NodeRouteType.Name)
                    throw new VariantPathException(path.Path);

                var index = mTreeVars.IndexOfKey(path.Name);

                if (index == -1)
                    throw new VariantPathException(path.Path);

                if (path.Next != null)
                    return ((Var)mTreeVars.GetByIndex(index)).Remove(path.Next);

                return Remove(index);
            }

            if (IsList())
            {
                if (path.RouteType != NodeRouteType.Index)
                    throw new VariantPathException(path.Path);

                if (!(0 <= path.Index && path.Index < mListVars.Count))
                    throw new VariantPathException(path.Path);

                if (path.Next != null)
                    return mListVars[path.Index].Remove(path.Next);

                return Remove(path.Index);
            }

            throw new VariantUnsupportedFunctionalityException("Remove(NodePath)", Type);
        }

        //-----------------------------------------------------------------------------------------
        public Var Remove(int index)
        {
            if (IsTree() || IsObject())
            {
                var ret = (Var)mTreeVars.GetByIndex(index);
                mTreeVars.RemoveAt(index);
                ret.Parent = null;
                return ret;
            }

            if (IsList())
            {
                var ret = mListVars[index];
                mListVars.RemoveAt(index);
                ret.Parent = null;
                return ret;
            }

            throw new VariantUnsupportedFunctionalityException("Remove(int)", Type);
        }

        //-----------------------------------------------------------------------------------------
        public Var Remove()
        {
            if (Parent == null)
            {
            }
            else if (Parent.IsObject() || Parent.IsTree())
            {
                for (var i = 0; i != Parent.mTreeVars.Count; i++)
                {
                    if (Parent.mTreeVars.GetByIndex(i) != this)
                        continue;
                    Parent.mTreeVars.RemoveAt(i);
                    Parent = null;
                    return this;
                }
            }
            else if (Parent.IsList())
            {
                for (var i = 0; i != Parent.mListVars.Count; i++)
                {
                    if (Parent.mListVars[i] != this)
                        continue;
                    Parent.mListVars.RemoveAt(i);
                    Parent = null;
                    return this;
                }
            }

            return this;
        }
        //-----------------------------------------------------------------------------------------

        //
        //Add
        //

        //only for: list. add item to end.
        public Var Add(Var v)
        {
            if (!IsList())
                throw new VariantUnsupportedFunctionalityException("Add(Var)", Type);

            mListVars.Add(v);
            v.Parent = this;
            return v;
        }

        //-----------------------------------------------------------------------------------------
        //only for: tree, object. add item.
        public Var Add(string key, Var v)
        {
            if (!(IsTree() || IsObject()))
                throw new VariantUnsupportedFunctionalityException("Add(string, Var)", Type);

            mTreeVars.Add(key, v);
            v.Parent = this;
            return v;
        }
        //-----------------------------------------------------------------------------------------
        public Var AddIfNotNull(string key, Var v)
        {
            if (v == null)
                return null;

            if (!(IsTree() || IsObject()))
                throw new VariantUnsupportedFunctionalityException("Add(string, Var)", Type);

            mTreeVars.Add(key, v);
            v.Parent = this;
            return v;
        }
        //----------------------------------------------------------------------------------------- 

        //
        //Insert
        //
        //A|B|C|D => Add(A|B|C|E) => A|B|C|D
        //     |F|G                       |E   <=
        //                                |F|G
        //
        //A|B|C|0 => Add(A|B|C) => A|B|C|0
        //     |1|D                     |1|D 
        //                              |2   <=
        //
        //A|B|C|0 => Add(A|B|C|1) => A|B|C|0
        //     |1|D                       |1   <= 
        //                                |2|D



        //only for: list, tree, object. insert an item at the specified path
        public Var Insert(string path, Var v)
        {
            return Insert(Conversion.ToRoute(path), v);
        }

        //-----------------------------------------------------------------------------------------
        //only for: list, tree, object. insert an item at the specified path
        public Var Insert(string path)
        {
            return Insert(Conversion.ToRoute(path), new Var());
        }

        //-----------------------------------------------------------------------------------------
        //only for: list, tree, object. insert an item at the specified path
        private Var Insert(NodeRoute path, Var v)
        {
            if (IsTree() || IsObject())
            {
                if (path.RouteType != NodeRouteType.Name)
                    throw new VariantPathException(path.Path);

                var index = mTreeVars.IndexOfKey(path.Name);

                if (path.Next != null)
                {
                    if (index == -1)
                        throw new VariantPathException(path.Path);

                    return ((Var)mTreeVars.GetByIndex(index)).Insert(path.Next, v);
                }

                if (index != -1)
                {
                    if (((Var)mTreeVars[path.Name]).IsList())
                        return ((Var)mTreeVars[path.Name]).Add(v);

                    throw new VariantPathException(path.Path);
                }

                return Add(path.Name, v);
            }

            if (IsList())
            {
                if (path.RouteType != NodeRouteType.Index)
                    throw new VariantPathException(path.Path);

                if (!(0 <= path.Index && path.Index < mListVars.Count + (path.Next != null ? 0 : 1)))
                    throw new VariantPathException(path.Path);

                if (path.Next != null)
                    return mListVars[path.Index].Insert(path.Next, v);

                mListVars.Insert(path.Index, v);
                v.Parent = this;
                return v;
            }

            throw new VariantUnsupportedFunctionalityException("Insert(NodePath, Var)", Type);
        }

        //-----------------------------------------------------------------------------------------
        //only for: list, tree, object. insert an item at the specified path
        public Var Insert(int index, Var value)
        {
            return Insert(Conversion.ToRoute(Conversion.ToString(index)), value);
        }
        //----------------------------------------------------------------------------------------- 
        //
        //Replace
        //
        //A|B|C|D:111 => Replace(A|B|C|D:true) => A|B|C|D:true
        //     |F|G                                    |F|G   
        //                            
        //A|B|C|0:111 => Replace(A|B|C|0:true) => A|B|C|0:true
        //     |1|G                                    |1|G   

        //only for: list, tree, object. replacement of one value by another along the specified path with the return of the old value
        public Var Replace(string path, Var v)
        {
            return Replace(Conversion.ToRoute(path), v);
        }

        //-----------------------------------------------------------------------------------------
        //only for: list, tree, object. replacement of one value by another along the specified path with the return of the old value
        public Var Replace(NodeRoute path, Var v)
        {
            Var old = null;
            if (IsTree() || IsObject())
            {
                if (path.RouteType != NodeRouteType.Name)
                    throw new VariantPathException(path.Path);

                var index = mTreeVars.IndexOfKey(path.Name);

                if (index == -1 && path.Next != null)
                    throw new VariantPathException(path.Path);

                if (index == -1 && path.Next == null)
                {
                    mTreeVars.Add(path.Name, v);
                    v.Parent = this;
                }
                else if (path.Next != null)
                    ((Var)mTreeVars.GetByIndex(index)).Replace(path.Next, v);
                else
                {
                    old = (Var)mTreeVars[path.Name];
                    mTreeVars[path.Name] = v;
                    v.Parent = this;
                }
            }
            else if (IsList())
            {
                if (path.RouteType != NodeRouteType.Index)
                    throw new VariantPathException(path.Path);

                if (!(0 <= path.Index && path.Index < mListVars.Count))
                    throw new VariantPathException(path.Path);

                if (path.Next != null)
                    mListVars[path.Index].Replace(path.Next, v);
                else
                {
                    old = mListVars[path.Index];
                    mListVars[path.Index] = v;
                    v.Parent = this;
                }
            }
            else
                throw new VariantUnsupportedFunctionalityException("Replace(NodePath, Var)", Type);

            if (old != null)
                old.Parent = null;

            return old;
        }

        //----------------------------------------------------------------------------------------- 
        //
        //Find
        // 
        /*
        public int IndexOfKey(string name)
        {
            if (IsTree() || IsObject())
                return mTreeVars.IndexOfKey(name);

            throw new VariantUnsupportedFunctionalityException("FindIndex(string)", Type);
        }
        */
        public bool TryGetValue(string path, out Var node)
        {
            node = Find(path);
            return node != null;
        }
            
        public bool ContainsKey(string path)
        {
            var result = Find(path);
            return result != null;
        }

        //-----------------------------------------------------------------------------------------
        //only for: list, tree, object. search for the value in the specified path, if the value is not found, the result will be null
        public Var Find(string path)
        {
            return Find(Conversion.ToRoute(path));
        }

        //-----------------------------------------------------------------------------------------
        //only for: list, tree, object. search for the value in the specified path, if the value is not found, the result will be null
        private Var Find(NodeRoute path)
        {
            if (IsTree() || IsObject())
            {
                if (path.RouteType != NodeRouteType.Name)
                    return null;

                var index = mTreeVars.IndexOfKey(path.Name);

                if (index == -1)
                    return null;

                if (path.Next != null)
                    return ((Var)mTreeVars.GetByIndex(index)).Find(path.Next);

                return (Var)mTreeVars.GetByIndex(index);
            }

            if (IsList())
            {
                if (path.RouteType != NodeRouteType.Index)
                    return null;

                if (!(0 <= path.Index && path.Index < mListVars.Count))
                    return null;

                return path.Next != null ? mListVars[path.Index].GetValue(path.Next) : mListVars[path.Index];
            }

            return null;
        }
        //----------------------------------------------------------------------------------------- 
        //
        //Get
        // 

        //only for: list, tree, object. getting the value in the specified path, if the value is not found, the result will be an exception
        public Var GetValue(string path)
        {
            return GetValue(Conversion.ToRoute(path));
        }

        //-----------------------------------------------------------------------------------------
        //only for: list, tree, object. getting the value in the specified path, if the value is not found, the result will be an exception
        public Var GetValue(NodeRoute path)
        {
            if (IsTree() || IsObject())
            {
                if (path.RouteType != NodeRouteType.Name)
                    throw new VariantPathException(path.Path);

                var index = mTreeVars.IndexOfKey(path.Name);

                if (index == -1)
                    throw new VariantPathException(path.Path);

                if (path.Next != null)
                    return ((Var)mTreeVars.GetByIndex(index)).GetValue(path.Next);

                return (Var)mTreeVars.GetByIndex(index);
            }

            if (IsList())
            {
                if (path.RouteType != NodeRouteType.Index)
                    throw new VariantPathException(path.Path);

                if (!(0 <= path.Index && path.Index < mListVars.Count))
                    throw new VariantPathException(path.Path);

                return path.Next != null ? mListVars[path.Index].GetValue(path.Next) : mListVars[path.Index];
            }

            throw new VariantUnsupportedFunctionalityException("Get(NodePath)", Type);
        }
        //----------------------------------------------------------------------------------------- 
        //
        //GetKey
        // 

        //only for: tree, object
        public string GetKey(int index)
        {
            return (string)mTreeVars.GetKey(index);
        }
        //----------------------------------------------------------------------------------------- 
        //
        //GetValue
        // 

        //only for: list, tree, object
        public Var GetValue(int index)
        {
            if (IsList())
                return mListVars[index];
            if (IsTree() || IsObject())
                return (Var)mTreeVars.GetByIndex(index);

            throw new VariantUnsupportedFunctionalityException(" GetValue(int)", Type);
        }

        //-----------------------------------------------------------------------------------------
        public Var this[int index] => GetValue(index);

        //-----------------------------------------------------------------------------------------
        public Var this[string path] => GetValue(path);

        //-----------------------------------------------------------------------------------------
        /*
        public bool Equals(Var x, Var y)
        {
            return Equal(x, y);
        }

        //-----------------------------------------------------------------------------------------
        public static bool Equal(Var l, Var r)
        {
            if (l != null && r != null)
            {
                if (l.Type != r.Type)
                    return false;

                if (l.Type != VarType.List && l.Type != VarType.Tree &&
                    l.Type != VarType.Object)
                {
                    return Memory.Memcmp(l.Data, r.Data);
                }

                //need to write a recursive function
                return Memory.Memcmp(SaveToBinaryStream(l), SaveToBinaryStream(r));
            }

            return false;
        }
        */
        //-----------------------------------------------------------------------------------------
        public void Clear()
        {
            switch (Type)
            {
                case VarType.Tree:
                    mTreeVars.Clear();
                    return;
                case VarType.List:
                    mListVars.Clear();
                    return;
                case VarType.Object:
                    mTreeVars.Clear();
                    return;
            }

            throw new VariantUnsupportedFunctionalityException("Clear()", Type);
        }

        //-----------------------------------------------------------------------------------------
        public string GetObjectType()
        {
            if (IsObject())
                return Data.Length == 0 ? null : Conversion.ToString(Data);

            throw new VariantUnsupportedFunctionalityException("GetObjectType()", Type);
        }

        //-----------------------------------------------------------------------------------------
        public string GetMyName()
        {
            if (Parent == null)
                return null;

            if (Parent.IsObject() || Parent.IsTree())
            {
                for (var i = 0; i != Parent.mTreeVars.Count; i++)
                {
                    if (Parent.mTreeVars.GetByIndex(i) != this)
                        continue;

                    return Parent.GetKey(i);
                }
            }

            if (Parent.IsList())
            {
                for (var i = 0; i != Parent.mListVars.Count; i++)
                {
                    if (Parent.mListVars[i] != this)
                        continue;

                    return Conversion.ToString(i);
                }
            }

            return null;
        }

        //-----------------------------------------------------------------------------------------
        public string GetMyPath(char separator = '|')
        {
            if (Parent == null)
                return null;

            var parent = this;
            var path = parent.GetMyName();

            if (parent.Parent == null)
                return path;

            while (true)
            {
                parent = parent.Parent;
                var name = parent.GetMyName();
                if (name == null)
                    break;
                path = name + separator + path;
            }

            return path;
        }

        //-----------------------------------------------------------------------------------------
        public int GetMyIndex()
        {
            if (Parent == null)
                return -1;

            if (Parent.IsObject() || Parent.IsTree())
            {
                for (var i = 0; i != Parent.mTreeVars.Count; i++)
                {
                    if (Parent.mTreeVars.GetByIndex(i) != this)
                        continue;

                    return i;
                }
            }

            if (Parent.IsList())
            {
                for (var i = 0; i != Parent.mListVars.Count; i++)
                {
                    if (Parent.mListVars[i] != this)
                        continue;

                    return i;
                }
            }

            return -1;
        }

        //-----------------------------------------------------------------------------------------
        public bool IsNull()
        {
            return Type == VarType.Null;
        }

        //-----------------------------------------------------------------------------------------
        public bool IsBool()
        {
            return Type == VarType.Bool;
        }

        //-----------------------------------------------------------------------------------------
        public bool IsInt()
        {
            return Type == VarType.Int;
        }

        //-----------------------------------------------------------------------------------------
        public bool IsFloat()
        {
            return Type == VarType.Float;
        }

        //-----------------------------------------------------------------------------------------
        public bool IsString()
        {
            return Type == VarType.String;
        }

        //-----------------------------------------------------------------------------------------
        public bool IsByteArray()
        {
            return Type == VarType.ByteArray;
        }

        //-----------------------------------------------------------------------------------------
        public bool IsType()
        {
            return Type == VarType.Type;
        }

        //-----------------------------------------------------------------------------------------
        public bool IsTree()
        {
            return Type == VarType.Tree;
        }

        //-----------------------------------------------------------------------------------------
        public bool IsList()
        {
            return Type == VarType.List;
        }

        //-----------------------------------------------------------------------------------------
        public bool IsObject()
        {
            return Type == VarType.Object;
        }

        //-----------------------------------------------------------------------------------------
        public bool IsNumeric()
        {
            return IsFloat() || IsInt();
        }

        //-----------------------------------------------------------------------------------------
        public bool IsComplex()
        {
            return IsList() || IsTree() || IsObject();
        }

        //----------------------------------------------------------------------------------------- 
        public static Var Clone(Var v)
        {
            if (v == null)
                return null;

            return Clone(v, null);
        }

        //-----------------------------------------------------------------------------------------
        private static Var Clone(Var v, Var parent)
        {
            Var ret;

            if (!v.IsTree() && !v.IsList() && !v.IsObject())
                ret = new Var { Data = (byte[])v.Data.Clone(), Type = v.Type, Parent = parent };
            else if (v.IsList())
            {
                ret = new Var { Type = v.Type, mListVars = new List<Var>(v.Count), Parent = parent };
                for (var i = 0; i < v.Count; i++)
                    ret.Add(Clone(v.GetValue(i)));
            }
            else
            {
                ret = new Var
                {
                    Type = v.Type,
                    Data = v.IsObject() ? (byte[])v.Data.Clone() : null,
                    mTreeVars = new SortedList(v.Count),
                    Parent = parent
                };
                for (var i = 0; i < v.Count; i++)
                    ret.Insert(Conversion.ToRoute(v.GetKey(i)), Clone(v.GetValue(i)));
            }

            return ret;
        }

        //-----------------------------------------------------------------------------------------
        public Var SetData(Var v)
        {
            if (v.IsComplex())
                throw new VariantUnsupportedFunctionalityException("SetData(Var v)", Type);

            if (Type != v.Type)
                throw new Exception("different types");

            Data = v.Data;
            return this;
        }

        //-----------------------------------------------------------------------------------------
        public int Length => Data.Length;

        //-----------------------------------------------------------------------------------------
        public VarType Type { get; private set; }

        public byte[] Data { get; private set; }
        private SortedList mTreeVars;
        private List<Var> mListVars;

        public Var Parent { get; private set; }
    }
}

namespace Atom
{
    using Variant;
    public static partial class Conversion
    {
        public static bool IsVar(VarType type, string value)
        {
            switch (type)
            {
                case VarType.Null: return true;
                case VarType.Bool: return IsBool(value);
                case VarType.Int: return IsInt(value);
                case VarType.Float: return IsFloat(value);
                case VarType.String: return IsString(value);
                case VarType.ByteArray: return IsByteArray(value, true);
                case VarType.Type: return IsVarType(value);
            }

            return false;
        }

        public static Var ToVar(VarType type, string value)
        {
            switch (type)
            {
                case VarType.Null: return new Var();
                case VarType.Bool: return new Var(ToBool(value));
                case VarType.Int: return new Var(ToInt(value));
                case VarType.Float: return new Var(ToFloat(value));
                case VarType.String: return new Var(ToString(value));
                case VarType.ByteArray: return new Var(HexStringToByteArray(value));
                case VarType.Type: return new Var(ToVarType(value));
            }

            throw new VariantConvertException(typeof(string), type);
        }

        public static Var ToVar(Param value)
        {
            if (value.Type == ParamType.Bool)
                return new Var((bool)value);
            if (value.Type == ParamType.Int)
                return new Var((int)value);
            if (value.Type == ParamType.Float)
                return new Var((float)value);

            throw new Exception("impossible to convert param to var");
        }

        public static Param ToParam(Var value)
        {
            if (value.Type == VarType.Bool)
                return new Param((bool)value);
            if (value.Type == VarType.Int)
                return new Param((int)value);
            if (value.Type == VarType.Float)
                return new Param((float)value);

            throw new Exception("impossible to convert var to param");
        }

        public static Var ToVar(Exception exception)
        {
            var list = Var.CreateList();

            var ex = exception;
            do
            {
                list.Add(ex.Message);
                ex = ex.InnerException;

            } while (ex != null);


            return list;
        }
    }
}