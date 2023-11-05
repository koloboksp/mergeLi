namespace Atom.Variant
{
    public sealed partial class Var
    {
        //-----------------------------------------------------------------------------------------
        private static int GetSizeOfPackedVarType()
        {
            return sizeof(VarType);
        }
        //-----------------------------------------------------------------------------------------
        public static int GetBinarySize(Var v)
        {
            var size = 0;
            GetSizeInMemoryBinary(ref size, v);
            return size;
        }
        //-----------------------------------------------------------------------------------------
        private static void GetSizeInMemoryBinary(ref int size, Var v)
        {
            size += GetSizeOfPackedVarType();

            if (v.IsComplex())
            {
                if (v.IsObject())
                    size += Memory.GetSizeOfPackedString(v.GetObjectType());

                size += Memory.GetSizeOfPackedInt();

                for (var i = 0; i != v.Count; i++)
                {
                    if (v.IsTree() || v.IsObject())
                        size += Memory.GetSizeOfPackedString(v.GetKey(i));
                    GetSizeInMemoryBinary(ref size, v.GetValue(i));
                }
            }
            else
                size += Memory.GetSizeOfPackedArray(v.Data);
        }
        //-----------------------------------------------------------------------------------------
        public static void SaveToMemoryBinary(IPacking memory, Var v)
        {
            memory.Packing((int)v.Type);

            if (v.IsComplex())
            {
                if (v.IsObject())
                    memory.Packing(v.GetObjectType());

                memory.Packing(v.Count);

                for (var i = 0; i != v.Count; i++)
                {
                    if (v.IsTree() || v.IsObject())
                        memory.Packing(v.GetKey(i));

                    SaveToMemoryBinary(memory, v.GetValue(i));
                }
            }
            else
                memory.Packing(v.Data);
        }
        //-----------------------------------------------------------------------------------------
        public static Var LoadFromMemoryBinary(IUnpacking memory)
        {
            Var result;

            var type = (VarType)memory.UnpackingInt();

            if (type == VarType.List)
            {
                result = CreateList();
                
                var size = memory.UnpackingInt();

                for (var i = 0; i != size; ++i)
                    result.Add(LoadFromMemoryBinary(memory));
            }
            else if (type == VarType.Tree || type == VarType.Object)
            {
                result = type == VarType.Object ? CreateObject(memory.UnpackingString()) : CreateTree();
                
                var size = memory.UnpackingInt();

                for (var i = 0; i != size; ++i)
                    result.Add(memory.UnpackingString(), LoadFromMemoryBinary(memory));
            }
            else
                result = new Var(type, memory.UnpackingArray());

            return result;
        }
        //-----------------------------------------------------------------------------------------
        public static void SaveToFileBinary(string fileName, Var v)
        { 
            var size = GetBinarySize(v);
            var memory = new MemoryPacking(new byte[size]);
            SaveToMemoryBinary(memory, v);
            Conversion.ByteArrayToFile(fileName, memory.Buffer);
        }
        //-----------------------------------------------------------------------------------------
        public static Var LoadFromFileBinary(string fileName)
        {
            var data = Conversion.FileToByteArray(fileName);
            var memory = new MemoryUnpacking(data);
            return LoadFromMemoryBinary(memory);
        }
        //-----------------------------------------------------------------------------------------
    }
}