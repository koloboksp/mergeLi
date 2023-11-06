using System.Security.Cryptography;

namespace Atom
{
    public static partial class Math
    {
        public const float FloatPositiveInfinity = float.PositiveInfinity;
        public const float FloatNegativeInfinity = float.NegativeInfinity;
        public const float FloatError = 0.0001f;

        public const float Pi = 3.1415926535897931f;

        public const float Angle0 = 0.0f;
        public const float Angle90 = Pi / 2.0f;
        public const float Angle45 = Pi / 4.0f;
        public const float Angle180 = Pi;
        public const float Angle270 = -Pi / 2.0f;

        public static float ToRad(float value)
        {
            return value * Pi / 180.0f;
        }

        public static float ToDeg(float value)
        {
            return value * 180.0f / Pi ;
        }

        public static float Sin(float angle)
        { 
            return Conversion.ToFloat(System.Math.Sin(angle));
        }

        public static float Cos(float angle)
        {
            return Conversion.ToFloat(System.Math.Cos(angle));
        }

        public static bool CompareFloat(float l, float r)
        {
            if (-FloatError > l - r)
                return false;
            if (FloatError < l - r)
                return false;

            return true;
        }

        public static bool CompareFloatToZero(float v)
        {
            if (-FloatError > v)
                return false;
            if (FloatError < v)
                return false;

            return true;
        }

        public static bool CompareFloatToZero(float v, float error)
        {
            if (-error > v)
                return false;
            if (error < v)
                return false;

            return true;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static float TranslateValue(float valueBegin, float valueEnd, float value, float begin, float end)
        {
            var deltaValue = valueBegin - valueEnd;
            var delta = begin - end;

            if (CompareFloatToZero(deltaValue))
                return begin + delta / 2.0f;

            var percent = (valueBegin - value) / deltaValue;
            var result = begin - percent * delta;
            return result;
        }

        public static float Interpolation(float valueBegin, float valueEnd, float t)
        {
            var deltaValue = valueEnd - valueBegin;
            var result = valueBegin + deltaValue * t;
            return result;
        }

        public static float Sign(float value)
        {
            return value >= 0.0 ? +1.0f : -1.0f;
        }

        public static float PZero(float value)
        {
            return value < 0.0f ? 0.0f : value;
        }

        public static int PZero(int value)
        {
            return value < 0 ? 0 : value;
        }

        public static float POne(float value)
        {
            return value > 1.0f ? 1.0f : value;
        }

        public static float Sqrt(float value)
        {
            return Conversion.ToFloat(System.Math.Sqrt(value));
        }

        public static float Abs(float value)
        {
            return value < 0.0f ? -value : value;
        }

        public static int Abs(int value)
        {
            return (value < 0) ? -value : value;
        }

        /*
        value               | -2.9 | -2.0 | -1.0 | -0.5 | 0.3 | 1.0 | 1.5 | 2.0 | 2.9 |
        --------------------+------+------+------+------+-----+-----+-----+-----+-----+
        Round               | -3.0 | -2.0 | -1.0 |  0.0 | 0.0 | 1.0 | 2.0 | 2.0 | 3.0 |
        Floor               | -3.0 | -2.0 | -1.0 | -1.0 | 0.0 | 1.0 | 1.0 | 2.0 | 2.0 |
        Ceiling             | -2.0 | -2.0 | -1.0 |  0.0 | 1.0 | 1.0 | 2.0 | 2.0 | 3.0 |
        Truncate            | -2.0 | -2.0 | -1.0 |  0.0 | 0.0 | 1.0 | 1.0 | 2.0 | 2.0 |
        */

        public static float Floor(float value)
        {
            return Conversion.ToFloat(System.Math.Floor(value));
            /*
            var truncate = Truncate(value);

            if (value >= 0.0f)
                return truncate;

            return truncate - 1.0f;
            */
            
        }

        public static float Truncate(float value)
        {
            //return (float)System.Math.Truncate(value);
            return Conversion.ToFloat(Conversion.ToInt(value));
        }

        public static float Ceiling(float value)
        {
            return Conversion.ToFloat(System.Math.Ceiling(value));
        }

        public static float Round(float value)
        {
            return Conversion.ToFloat(System.Math.Round(value));
        }

        public static float ToDirection(float direction)
        {
            return direction < 0.0f ? -1.0f : 1.0f;
        }

        public static int Min(int l, int r)
        {
            return l < r ? l : r;
        }

        public static double Min(double l, double r)
        {
            return l < r ? l : r;
        }

        public static float Min(float l, float r)
        {
            return l < r ? l : r;
        }

        public static float Max(float l, float r)
        {
            return l > r ? l : r;
        }

        public static float PZeroOne(float value)
        {
            if (value > 1.0f)
                return 1.0f;
            if (value < 0.0f)
                return 0.0f;

            return value;
        }

        public static float ForwardDirection(float direction, float value)
        {
            if (value < 0.0f)
                value = -value;

            return direction >= 0.0f ? +value : -value;
        }

        public static bool IsForwardDirection(float direction, float value)
        {
            return direction > 0.0f && value > 0.0f || direction < 0.0f && value < 0.0f;
        }

        public static float ReverseDirection(float direction, float value)
        {
            if (value < 0.0f)
                value = -value;

            return direction > 0.0f ? -value : +value;
        }

        public static bool IsReverseDirection(float direction, float value)
        {
            return (direction >= 0.0f && value <= 0.0f) || (direction < 0.0f && value > 0.0f);
        }

        //[min1;max1] segment1
        //[min2-max2] segment2
        //result intersection length
        //-----------------------------------------------------------------------------------------
        public static float IntersectionLengthSign(float minLeft, float maxLeft, float minRight, float maxRight)
        {
            var centerLeft = minLeft + (maxLeft - minLeft) * 0.5f;
            var centerRight = minRight + (maxRight - minRight) * 0.5f;
             
            if(centerRight > centerLeft)
                return maxLeft - minRight;
            else
                return minLeft - maxRight;
        }
        //-----------------------------------------------------------------------------------------
        public static float IntersectionLength(float minLeft, float maxLeft, float minRight, float maxRight)
        {
            var centerLeft = minLeft + (maxLeft - minLeft) * 0.5f;
            var centerRight = minRight + (maxRight - minRight) * 0.5f;

            if (centerRight > centerLeft)
                return maxLeft - minRight;
            else
                return maxRight - minLeft;
        }
        //-----------------------------------------------------------------------------------------
        internal static int ToPow2(int value)
        {
            var pow = -1;
            while (value > 0)
            {
                value >>= 1;
                pow++;
            }

            return pow;
        }
        //-----------------------------------------------------------------------------------------
    }
}
