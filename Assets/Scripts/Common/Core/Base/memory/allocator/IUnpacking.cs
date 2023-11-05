namespace Atom
{
    //*********************************************************************************************
    public interface IUnpacking
    {
        void Reset();

        //bool
        bool UnpackingBool(int offsetBuffer);
        bool UnpackingBool();

        //int
        int UnpackingInt(int offsetBuffer);
        int UnpackingInt();

        //float
        float UnpackingFloat(int offsetBuffer);
        float UnpackingFloat();

        //byte
        byte UnpackingByte(int offsetBuffer);
        byte UnpackingByte();

        //double
        double UnpackingDouble(int offsetBuffer);
        double UnpackingDouble();

        //short
        short UnpackingShort(int offsetBuffer);
        short UnpackingShort();

        //param
        Param UnpackingParam(int offsetBuffer);
        Param UnpackingParam();

        //byte[]
        byte[] UnpackingArray(int offsetBuffer);
        byte[] UnpackingArray();

        //string
        string UnpackingString(int offsetBuffer);
        string UnpackingString();

        //GuidEx
        GuidEx UnpackingGuidEx(int offsetBuffer);
        GuidEx UnpackingGuidEx();
    }
    //*********************************************************************************************
}
