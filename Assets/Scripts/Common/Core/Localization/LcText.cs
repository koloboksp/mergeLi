using System.Collections.Generic;

namespace Atom.Localization
{
    //*********************************************************************************************
    public sealed class LcText
    {
        private readonly byte[] mBuffer;
        private readonly Dictionary<GuidEx, int> mOffsets = new Dictionary<GuidEx, int>();
        private readonly Dictionary<GuidEx, string> mStrings = new Dictionary<GuidEx, string>();
        //-----------------------------------------------------------------------------------------
        public LcText(byte[] buffer)
        {
            mBuffer = buffer;
            var offset = 0;
            var count = Memory.UnpackingInt(mBuffer, ref offset);

            for (var i = 0; i != count; i++)
            {
                var guid = Memory.UnpackingArray(buffer, ref offset);
                var begin = Memory.UnpackingInt(mBuffer, ref offset);
                mOffsets.Add(Conversion.ToGuidEx(guid), begin);
            }

            mStrings[GuidEx.Empty] = string.Empty;
        }
        //-----------------------------------------------------------------------------------------
        public string GetText(GuidEx guid)
        {
            var text = FindText(guid);
            
            if(text == null)
                return "text by identifier " + guid + " not found";

            return text;
        }
        //-----------------------------------------------------------------------------------------
        public string FindText(GuidEx guid)
        {
            if (mStrings.TryGetValue(guid, out var value))
                return value;

            if (!mOffsets.TryGetValue(guid, out var begin))
                return null;

            var text = Memory.UnpackingString(mBuffer, begin);
            mStrings.Add(guid, text);
            return text;
        }
        //-----------------------------------------------------------------------------------------
    }
    //*********************************************************************************************     
}
