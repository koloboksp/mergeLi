namespace Atom
{
    //*********************************************************************************************
    public interface IPacking
    {
        //bool
        IPacking Packing(int offsetBuffer, bool value);
        IPacking Packing(bool value);

        //int
        IPacking Packing(int offsetBuffer, int value);
        IPacking Packing(int value);

        //float
        IPacking Packing(int offsetBuffer, float value);
        IPacking Packing(float value);

        //byte
        IPacking Packing(int offsetBuffer, byte value);
        IPacking Packing(byte value);

        //double
        IPacking Packing(int offsetBuffer, double value);
        IPacking Packing(double value);

        //short
        IPacking Packing(int offsetBuffer, short value);
        IPacking Packing(short value);

        //param
        IPacking Packing(int offsetBuffer, Param value);
        IPacking Packing(Param value);

        //array
        IPacking Packing(byte[] array, int offsetArray, int size);
        IPacking Packing(byte[] array);

        //string
        IPacking Packing(int offsetBuffer, string value);
        IPacking Packing(string value);

        //GuidEx
        IPacking Packing(int offsetBuffer, GuidEx value);
        IPacking Packing(GuidEx value);
    }
    //*********************************************************************************************
}
