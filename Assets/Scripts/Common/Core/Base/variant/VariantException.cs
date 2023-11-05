using System;

namespace Atom.Variant
{
    //*********************************************************************************************
    public class VariantConvertException : Exception
    {
        public VariantConvertException(VarType whereFrom, VarType whereTo) : base(GetMessage(whereFrom, whereTo))
        {
        }
        public VariantConvertException(VarType whereFrom, Type whereTo) : base(GetMessage(whereFrom, whereTo))
        {
        }

        public VariantConvertException(Type whereFrom, VarType whereTo) : base(GetMessage(whereFrom, whereTo))
        {
        }
        private static string GetMessage (VarType whereFrom, VarType whereTo)
        {
            return "cannot convert type: " + whereFrom + " to type: " + whereTo;
        }

        private static string GetMessage(VarType whereFrom, Type whereTo)
        {
            return "cannot convert type: " + whereFrom + " to type: " + whereTo;
        }

        private static string GetMessage(Type whereFrom, VarType whereTo)
        {
            return "cannot convert type: " + whereFrom + " to type: " + whereTo;
        }
    }
    //*********************************************************************************************
    public class VariantUnsupportedFunctionalityException : Exception
    {
        public VariantUnsupportedFunctionalityException(string message, VarType type) : base(GetMessage(message, type))
        {
        }

        private static string GetMessage(string message, VarType type)
        {
            return "unsupported functionality: " + message + " for type: " + type;
        }
    }
    //*********************************************************************************************
    public class VariantPathException : Exception
    {
        public VariantPathException(string path) : base(GetMessage(path))
        {
        }
        private static string GetMessage(string path)
        {
            return "invalid path: " + path;
        }
    }
    //*********************************************************************************************
}
