namespace Atom
{
    //*********************************************************************************************
    public interface IData
    {

    }
    //*********************************************************************************************
    public interface IVoidGet : IData
    {
    }
    //*********************************************************************************************
    public interface ICall : IData
    {
        void Call();
    }
    //*********************************************************************************************
    public interface IBoolSet : IData
    {
        void Set(bool value);
    }
    public interface IBoolGet : IData
    {
        bool Get();
    }
    public interface IBoolGetSet : IBoolGet, IBoolSet
    {
    }
    //*********************************************************************************************
    public interface IIntSet : IData
    {
        void Set(int value);
    }
    public interface IIntGet : IData
    {
        int Get();
    }
    public interface IIntGetSet : IIntGet, IIntSet
    {
    }
    //*********************************************************************************************
    public interface IFloatSet : IData
    {
        void Set(float value);
    }
    public interface IFloatGet : IData
    {
        float Get();
    }
    public interface IFloatGetSet : IFloatGet, IFloatSet
    {
    }
    //*********************************************************************************************
    public interface IStringSet : IData
    {
        void Set(string value);
    }
    public interface IStringGet : IData
    {
        float Get();
    }
    public interface IStringGetSet : IStringGet, IStringSet
    {
    }
    //*********************************************************************************************
}
